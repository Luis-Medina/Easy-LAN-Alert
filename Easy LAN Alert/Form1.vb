Imports System.IO
Imports System.Net
Imports System.ComponentModel
Imports System.Threading
Imports System.Net.NetworkInformation
Imports System.Text.RegularExpressions
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.ServiceProcess
Imports System.Collections.Specialized
Imports Easy_LAN_Alert_DLL
Imports System.ServiceModel

<ServiceBehavior(UseSynchronizationContext:=False,
    InstanceContextMode:=ServiceModel.InstanceContextMode.PerSession,
    ConcurrencyMode:=ServiceModel.ConcurrencyMode.Reentrant)>
Public Class Form1
    Implements ClientToServiceCallback

    Private macListRegex As String = "^([0-9A-Fa-f]{2}[\\.:]){2}([0-9A-Fa-f]{2}).*"
    Private nodeList As New List(Of NetworkNode)
    Private ipAddress As String
    Private Shared theDatabase As Database
    Private host As ServiceHost
    Private pipeFactory As DuplexChannelFactory(Of ClientToServiceInterface)
    Private pipeProxy As ClientToServiceInterface
    Dim sc As New ServiceController("Easy LAN Alert Service")
    'Private syncContext As SynchronizationContext
    'Private hostContext As SynchronizationContext
    Private badColor As Color = Color.LightSalmon
    Private defaultColor As Color

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        sc.Refresh()
        If sc.Status <> ServiceControllerStatus.Running Then
            Me.Cursor = Cursors.WaitCursor
            sc.Start()
            sc.WaitForStatus(ServiceControllerStatus.Running)
            ServiceControl.Text = "Stop Service"
            Me.Cursor = Cursors.Default
        End If

        SetUIForScanStart()
        sendScanStart()
    End Sub

    Private Sub sendScanStart()
        createPipeProxy()
        Try
            pipeProxy.StartScan()
        Catch ex As Exception
            pipeFactory.Abort()
            createPipeProxy()
            pipeProxy.StartScan()
        End Try
    End Sub

    Private Sub queryStatus()
        createPipeProxy()
        Try
            pipeProxy.QueryStatus()
        Catch ex As Exception
            pipeFactory.Abort()
            createPipeProxy()
            pipeProxy.QueryStatus()
        End Try
    End Sub

    Private Sub SetUIForScanStart()
        Label6.Visible = False
        Button1.Enabled = False
        ProgressBar1.Visible = True
        DataGridView1.DataSource = Nothing
        nodeList.Clear()
    End Sub

    Private Sub SetUIForScanDone()
        setDataGridViewRows()
        ProgressBar1.Visible = False
        Button1.Enabled = True
        Label6.Text = nodeList.Count & " device(s) found"
        Label6.Visible = True
    End Sub

    Private Sub setDataGridViewRows(Optional ByVal row As Integer = -1)
        If row >= 0 Then
            Try
                Dim node As NetworkNode = CType(DataGridView1.Rows(row).DataBoundItem, NetworkNode)
                If node.isUntrusted Then
                    DataGridView1.Rows(row).DefaultCellStyle.BackColor = badColor
                Else
                    DataGridView1.Rows(row).DefaultCellStyle.BackColor = defaultColor
                End If
            Catch ex As Exception
            End Try
        Else
            DataGridView1.DataSource = Nothing
            DataGridView1.DataSource = nodeList
            If nodeList.Count > 0 Then
                For i As Integer = 0 To nodeList.Count - 1
                    If nodeList(i).isUntrusted Then
                        DataGridView1.Rows(i).DefaultCellStyle.BackColor = badColor
                    Else
                        DataGridView1.Rows(i).DefaultCellStyle.BackColor = defaultColor
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        theDatabase.CloseConnection()
        Try
            host.BeginClose(Nothing, Nothing)
        Catch ex As Exception
        End Try
        Try
            pipeFactory.BeginClose(Nothing, Nothing)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Form1_Shown(sender As Object, e As System.EventArgs) Handles Me.Shown
        Dim theInterfaceToUse = (From nic In NetworkInterface.GetAllNetworkInterfaces()
                    Where nic.OperationalStatus = OperationalStatus.Up And nic.NetworkInterfaceType <> NetworkInterfaceType.Loopback
                    Select nic
                    ).FirstOrDefault
        Dim mac As PhysicalAddress = theInterfaceToUse.GetPhysicalAddress()
        Label5.Text = String.Join(":", (From z In mac.GetAddressBytes() Select z.ToString("X2")).ToArray())

        Dim ip = (From address In theInterfaceToUse.GetIPProperties.UnicastAddresses
                    Where Not address.Address.ToString.Contains(":")
                    Select address.Address
                ).FirstOrDefault
        ipAddress = ip.ToString
        Label4.Text = ipAddress

        theDatabase = Database.getInstance()
        If theDatabase Is Nothing Then
            MsgBox("Unable to open database")
            Application.Exit()
        End If
        Settings.loadSettings()


        defaultColor = DataGridView1.DefaultCellStyle.BackColor

        'hostContext = SynchronizationContext.Current
        initServiceHost()


        If sc.Status = ServiceControllerStatus.Running Then
            pipeProxy.QueryStatus()
        Else
            ServiceControl.Text = "Start Service"
        End If

        Debug.WriteLine(NetworkNode.DEFAULT_MAC = "00-00-00-00-00-00")
    End Sub

    Private Sub DataGridView1_CellMouseUp(sender As Object, e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles DataGridView1.CellMouseUp
        Try
            If e.ColumnIndex <> -1 And e.RowIndex <> -1 Then
                If DataGridView1.Columns(e.ColumnIndex).CellType Is GetType(DataGridViewCheckBoxCell) Then
                    DataGridView1.EndEdit()
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub DataGridView1_CellValueChanged(sender As Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView1.CellValueChanged
        Try
            If e.ColumnIndex <> -1 And e.RowIndex <> -1 Then
                Dim node As NetworkNode = CType(DataGridView1.Rows(e.RowIndex).DataBoundItem, NetworkNode)
                If e.ColumnIndex = DataGridView1.Columns("TrustedIPDataGridViewCheckBoxColumn").Index Then
                    theDatabase.setIPTrusted(node)
                    setDataGridViewRows(e.RowIndex)
                ElseIf e.ColumnIndex = DataGridView1.Columns("TrustedMACDataGridViewCheckBoxColumn").Index Then
                    theDatabase.setMACTrusted(node)
                    setDataGridViewRows(e.RowIndex)
                ElseIf e.ColumnIndex = DataGridView1.Columns("TrustedHostNameDataGridViewCheckBoxColumn").Index Then
                    theDatabase.setHostNameTrusted(node)
                    setDataGridViewRows(e.RowIndex)
                End If
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub initServiceHost()
        host = New ServiceHost(GetType(CallBackImpl))
        host.AddServiceEndpoint(GetType(IServiceToClient), New NetNamedPipeBinding(), "net.pipe://localhost/SSSERT")
        host.Open()
    End Sub

    Public Sub SetUIForStatus(isRunning As Boolean, lastRunTime As DateTime?, nextRunTime As DateTime?, nodeList As List(Of NetworkNode)) Implements ClientToServiceCallback.GetRunningStatus
        If isRunning Then
            SetUIForScanStart()
        Else
            SetServiceStatus(lastRunTime, nextRunTime)
            Me.nodeList.AddRange(nodeList)
            setDataGridViewRows()
        End If
    End Sub

    Public Sub SetServiceStatus(lastRunTime As DateTime?, nextRunTime As DateTime?)
        If lastRunTime IsNot Nothing Then
            Label7.Text = "Last Run: " & lastRunTime.Value.ToLongTimeString
        Else
            Label7.Text = String.Empty
        End If
        If nextRunTime IsNot Nothing Then
            Label8.Text = "Next Run: " & nextRunTime.Value.ToLongTimeString
        Else
            Label8.Text = String.Empty
        End If
    End Sub

    Public Sub ScanIsDone(nodes As List(Of NetworkNode), lastRunTime As DateTime?, nextRunTime As DateTime?)
        nodeList.Clear()
        nodeList.AddRange(nodes)
        SetUIForScanDone()
        SetServiceStatus(lastRunTime, nextRunTime)
    End Sub

    Public Sub setTrustForAll(type As Integer, trustIP As Boolean, trustMAC As Boolean, trustName As Boolean)
        Try
            For Each node In nodeList
                Dim trustValue As Boolean = If(type = TrustSelect.TRUST_ALL, True, False)
                If trustIP Then
                    node.trustedIP = trustValue
                    theDatabase.setIPTrusted(node)
                End If
                If trustMAC Then
                    node.trustedMAC = trustValue
                    theDatabase.setMACTrusted(node)
                End If
                If trustName Then
                    node.trustedHostName = trustValue
                    theDatabase.setMACTrusted(node)
                End If
            Next
            setDataGridViewRows()
        Catch ex As Exception

        End Try
    End Sub


    Private Sub createPipeProxy()
        If pipeFactory Is Nothing Then
            Try
                pipeFactory = New DuplexChannelFactory(Of ClientToServiceInterface)(Me, New NetNamedPipeBinding(), New EndpointAddress("net.pipe://localhost/ClientToService"))
            Catch ex As Exception
            End Try
        Else
            If pipeFactory.State = CommunicationState.Faulted Or pipeFactory.State = CommunicationState.Closed Then
                Try
                    pipeFactory = New DuplexChannelFactory(Of ClientToServiceInterface)(Me, New NetNamedPipeBinding(), New EndpointAddress("net.pipe://localhost/ClientToService"))
                    pipeProxy = pipeFactory.CreateChannel()
                Catch ex As Exception
                End Try
            End If
        End If
        If pipeProxy Is Nothing Then
            Try
                pipeProxy = pipeFactory.CreateChannel
            Catch ex As Exception
            End Try
        Else

        End If
    End Sub


    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        'syncContext = SynchronizationContext.Current

        createPipeProxy()

    End Sub

    Private Sub TrustAll_Click(sender As System.Object, e As System.EventArgs) Handles TrustAll.Click
        If nodeList.Count > 0 Then
            Dim trustSel As New TrustSelect(TrustSelect.TRUST_ALL)
            trustSel.ShowDialog()
        End If
    End Sub

    Private Sub TrustNone_Click(sender As System.Object, e As System.EventArgs) Handles TrustNone.Click
        If nodeList.Count > 0 Then
            Dim trustSel As New TrustSelect(TrustSelect.TRUST_NONE)
            trustSel.ShowDialog()
        End If
    End Sub


    Private Sub UpdateDatabase_Click(sender As System.Object, e As System.EventArgs) Handles UpdateDatabase.Click
        Me.Cursor = Cursors.WaitCursor
        Try
            My.Computer.Network.DownloadFile("http://www.luismedinaweb.com/downloads/maclist.txt", "maclist.txt", String.Empty, String.Empty, True, 5000, True, FileIO.UICancelOption.ThrowException)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.Critical)
            Me.Cursor = Cursors.Default
            Exit Sub
        End Try
        Dim streamReader As New StreamReader("maclist.txt")
        Dim vendorList As New List(Of Vendor)
        Try
            Using streamReader
                While Not streamReader.EndOfStream
                    Dim line As String = streamReader.ReadLine
                    If Regex.IsMatch(line, macListRegex) Then
                        Dim tokens() As String = line.Split(" ".ToCharArray, 3, StringSplitOptions.RemoveEmptyEntries)
                        Dim macAddr As String = Nothing
                        Dim vendorName As String = Nothing
                        If tokens(0).StartsWith("00:00:00") Then
                            macAddr = "00:00:00"
                            vendorName = "XEROX CORPORATION"
                        Else
                            macAddr = tokens(0).Split(CType(vbTab, Char))(0)
                            If macAddr.Length > 8 Then
                                macAddr = macAddr.Substring(0, 17)
                            End If
                            Try
                                If tokens.Count >= 3 Then
                                    vendorName = tokens(2)
                                Else
                                    vendorName = tokens(0).Split(CType(vbTab, Char))(1)
                                End If
                            Catch ex1 As Exception
                            End Try
                        End If
                        If vendorName IsNot Nothing Then
                            If vendorName.StartsWith("#") Then
                                vendorName = vendorName.Substring(1)
                            End If
                            Dim myVendor As New Vendor(vendorName, macAddr)
                            vendorList.Add(myVendor)
                            'Debug.WriteLine(myVendor)
                        End If
                    End If
                End While
            End Using
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
        Try
            My.Computer.FileSystem.DeleteFile("maclist.txt")
        Catch ex As Exception
        End Try
        Me.Cursor = Cursors.Default
        If vendorList.Count > 0 Then
            Dim recordCount As Integer = theDatabase.updateVendors(vendorList)
            Debug.WriteLine(recordCount & " updated")
            MsgBox("Update Finished!")
        End If
    End Sub

    Private Sub ScanInterval_Click(sender As System.Object, e As System.EventArgs) Handles ScanInterval.Click
        Dim response = InputBox("Enter time in minutes (current is " & Settings.scanInterval & ")", "Scan Interval Time")
        If Not String.IsNullOrEmpty(response) Then
            Try
                Dim currentInterval = Integer.Parse(response)
                If currentInterval > 0 Then
                    Settings.scanInterval = currentInterval
                    Settings.saveSettings()
                End If
            Catch ex As Exception
                MsgBox("Unable to save settings!" & ex.Message, vbCritical)
            End Try
        End If
    End Sub

    Private Sub AlertSettings_Click(sender As System.Object, e As System.EventArgs) Handles AlertSettings.Click
        Dim response = InputBox("Enter the email that will receive alerts (current is " & Settings.email & ")", "Alert email")
        If Not String.IsNullOrEmpty(response) Then
            Try
                Settings.email = response.Trim
                Settings.saveSettings()
            Catch ex As Exception
                MsgBox("Unable to save settings!" & ex.Message, vbCritical)
            End Try
        End If
    End Sub

    Private Sub ServiceControl_Click(sender As System.Object, e As System.EventArgs) Handles ServiceControl.Click
        Me.Cursor = Cursors.WaitCursor
        If ServiceControl.Text.Contains("Stop") Then
            sc.Refresh()
            If sc.Status = ServiceControllerStatus.Running Then
                sc.Stop()
                sc.WaitForStatus(ServiceControllerStatus.Stopped)
                ServiceControl.Text = "Start Service"
            End If
            Label8.Text = String.Empty
            If ProgressBar1.Visible Then
                SetUIForScanDone()
            End If
        ElseIf ServiceControl.Text.Contains("Start") Then
            sc.Refresh()
            If sc.Status = ServiceControllerStatus.Stopped Then
                sc.Start()
                sc.WaitForStatus(ServiceControllerStatus.Running)
                ServiceControl.Text = "Stop Service"
            End If
            Label7.Text = String.Empty
        End If
        Me.Cursor = Cursors.Default
    End Sub
End Class
