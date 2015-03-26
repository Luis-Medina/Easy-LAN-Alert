Imports System.Threading
Imports System.Net.NetworkInformation
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.Net
Imports System.Collections.Specialized
Imports Easy_LAN_Alert_DLL

<ServiceBehavior(ConcurrencyMode:=ServiceModel.ConcurrencyMode.Reentrant,
    UseSynchronizationContext:=True)>
Public Class EasyLANAlertService

    Private Shared ipRegex As String = "\s\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"
    Private Shared macRegex As String = "([0-9A-F]{2}[:-]){5}([0-9A-F]{2})"
    Public Shared nodeList As New List(Of NetworkNode)
    Private Shared resetEvent As ManualResetEvent
    Private Shared pingDoneEvent As ManualResetEvent
    Private Shared remainingPingThreads As Integer
    Private Shared ipAddress As String
    Private Shared theDatabase As Database
    Private Shared currentNetwork As Network = Nothing
    Private host As ServiceHost
    Public Shared isRunning As Boolean
    Private Shared pipeProxy As IServiceToClient
    Private Shared pipeFactory As ChannelFactory(Of IServiceToClient)
    Public Shared serviceTimer As System.Timers.Timer
    Private Shared syncPoint As Integer = 0
    'Private hostContext As SynchronizationContext
    Private Shared myEventLog As EventLog
    Public Shared cameFromUI As Boolean
    Private Shared key As String = "Sq0JoAeAA#$RKlgZZWtC3f2bgt!NugY3Y6jjXMl6AB#qQG8!LzGts!g78z#%CGwRH$qac0"
    Public Shared lastRunTime As DateTime? = Nothing
    Public Shared nextRunTime As DateTime? = Nothing

    Protected Overrides Sub OnStart(ByVal args() As String)
        ' Add code here to start your service. This method should set things
        ' in motion so your service can do its work.
        Dim found As Boolean = False
        Try
            found = System.Diagnostics.EventLog.SourceExists(EventLog1.Source)
        Catch
            found = False
        End Try
        If Not found Then
            Try
                System.Diagnostics.EventLog.CreateEventSource(EventLog1.Source, "Application")
            Catch ex As Exception
                Debug.WriteLine(ex.Message)
            End Try
        End If

        theDatabase = Database.getInstance()
        If theDatabase Is Nothing Then
            Alert("Unable to open database", EventLogEntryType.Error)
            Me.Stop()
        End If
        Try
            Settings.loadSettings()
        Catch ex As Exception

        End Try
        myEventLog = EventLog1
        'hostContext = SynchronizationContext.Current


        host = New ServiceHost(GetType(ServiceHostReceiver))
        host.AddServiceEndpoint(GetType(ClientToServiceInterface), New NetNamedPipeBinding(), "net.pipe://localhost/ClientToService")
        host.Open()

        Try
            createPipeProxy()
        Catch ex As Exception
            Alert(ex.Message, EventLogEntryType.Error)
        End Try

        Dim interval As Integer = Settings.scanInterval * 60 * 1000
        nextRunTime = DateTime.Now.AddMilliseconds(interval)
        serviceTimer = New System.Timers.Timer(interval)
        AddHandler serviceTimer.Elapsed, AddressOf OnTimedEvent
        sendNextScanTime()
        serviceTimer.Enabled = True

    End Sub

    Private Shared Sub createPipeProxy()
        If pipeFactory Is Nothing Then
            Try
                pipeFactory = New ChannelFactory(Of IServiceToClient)(New NetNamedPipeBinding(), New EndpointAddress("net.pipe://localhost/SSSERT"))
            Catch ex As Exception
            End Try
        Else
            If pipeFactory.State = CommunicationState.Faulted Or pipeFactory.State = CommunicationState.Closed Then
                Try
                    pipeFactory = New ChannelFactory(Of IServiceToClient)(New NetNamedPipeBinding(), New EndpointAddress("net.pipe://localhost/SSSERT"))
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

    Private Shared Sub sendScanStarted()
        createPipeProxy()
        Try
            pipeProxy.ScanStarted()
        Catch ex As Exception
            pipeFactory.Abort()
        End Try
    End Sub

    Private Shared Sub sendScanFinished()
        createPipeProxy()
        Try
            pipeProxy.ScanFinished(nodeList, lastRunTime, nextRunTime)
        Catch ex As Exception
            pipeFactory.Abort()
        End Try
    End Sub

    Private Shared Sub sendNextScanTime()
        createPipeProxy()
        Try
            pipeProxy.NextScanTime(nextRunTime)
        Catch ex As Exception
            pipeFactory.Abort()
        End Try
    End Sub

    Public Shared Sub OnTimedEvent()
        Dim sync As Integer = Interlocked.CompareExchange(syncPoint, 1, 0)
        If sync = 0 Then
            Settings.loadSettings()
            isRunning = True

            serviceTimer.Stop()
            sendScanStarted()

            setCurrentIPAddress()
            Dim subnet As String = ipAddress.Substring(0, ipAddress.LastIndexOf(".") + 1)
            currentNetwork = theDatabase.getNetwork(subnet)
            If currentNetwork Is Nothing Then
                currentNetwork = New Network(subnet)
                If Not theDatabase.upsertNetwork(currentNetwork) Then
                    Alert("Unable to save the current network " & currentNetwork.name, EventLogEntryType.Error)
                End If
            End If
            pingDoneEvent = New ManualResetEvent(False)
            remainingPingThreads = 253
            nodeList.Clear()

            For i As Integer = 1 To 254
                Dim thisNode As New NetworkNode(subnet & i)
                thisNode.networkID = currentNetwork.id
                Dim trd As New Thread(DirectCast(Sub() processPing(thisNode), ThreadStart))
                trd.Priority = ThreadPriority.Lowest
                trd.IsBackground = True
                trd.Start()
            Next

            pingDoneEvent.WaitOne(1000 * 60 * 10)

            getAllMacAddresses()
            SyncLock nodeList
                nodeList.RemoveAll(Function(x) x.macAddress = NetworkNode.DEFAULT_MAC Or x.macAddress = "00:00:00:00:00:00")
                nodeList.Sort(Function(x, y) Integer.Parse(x.ipAddress.Split(CType(".", Char))(3)).CompareTo(Integer.Parse(y.ipAddress.Split(CType(".", Char))(3))))
                nodeList.RemoveAll(Function(x) x.macAddress = NetworkNode.DEFAULT_MAC Or x.macAddress = "00:00:00:00:00:00")
            End SyncLock

            Debug.WriteLine("Starting ARP Read")
            pingDoneEvent = New ManualResetEvent(False)
            getAllHostNames()

            pingDoneEvent.WaitOne(1000 * 60 * 10)
            isRunning = False
            lastRunTime = DateTime.Now
            Dim interval As Integer = Settings.scanInterval * 60 * 1000
            nextRunTime = DateTime.Now.AddMilliseconds(interval)
            serviceTimer.Interval = interval
            serviceTimer.Start()
            sendScanFinished()
            If Not cameFromUI Then
                sendAlertEmail()
            Else
                cameFromUI = False
            End If
            syncPoint = 0
        End If
    End Sub

    Private Shared Sub sendAlertEmail()
        If String.IsNullOrEmpty(Settings.email) Then
            Alert("No alert email set.", EventLogEntryType.Warning)
        Else
            If nodeList.Count > 0 Then
                Dim valueCollection As New NameValueCollection()
                Dim alertString As String = String.Empty
                For i As Integer = 0 To nodeList.Count - 1
                    If nodeList(i).isUntrusted Then
                        valueCollection.Add("device[" & i & "]", nodeList(i).toJSONString)
                        alertString &= nodeList(i).ipAddress & ";" & nodeList(i).macAddress & ";" & nodeList(i).hostName
                    End If
                Next
                If valueCollection.Count > 0 And Settings.lastAlert <> alertString Then
                    Settings.lastAlert = alertString
                    Settings.saveSettings("lastAlert")
                    valueCollection.Add("key", key)
                    valueCollection.Add("email", Settings.email)
                    Using client As New WebClient
                        client.UploadValuesAsync(New Uri("https://web2.secure-secure.co.uk/luismedinaweb.com/mailer.php"), valueCollection)
                    End Using
                End If
            End If
        End If
    End Sub

    Private Shared Sub processPing(node As NetworkNode)
        Dim pinger As New Ping
        Dim pingReply As PingReply = pinger.Send(node.ipAddress, 2000)
        node.status = pingReply.Status.ToString

        SyncLock nodeList
            nodeList.Add(node)
        End SyncLock

        If (Interlocked.Decrement(remainingPingThreads)) = 0 Then
            pingDoneEvent.Set()
        End If
    End Sub

    Private Shared Sub getAllMacAddresses()
        'Start the child process.
        Dim myProcess As New Process()
        Dim myProcessStartInfo As New ProcessStartInfo("arp.exe", "-a")
        myProcessStartInfo.UseShellExecute = False
        myProcessStartInfo.CreateNoWindow = True
        myProcessStartInfo.RedirectStandardOutput = True
        myProcessStartInfo.RedirectStandardError = True
        myProcessStartInfo.RedirectStandardInput = True
        myProcess.StartInfo = myProcessStartInfo
        myProcess.Start()
        Dim myStreamReader As StreamReader = myProcess.StandardOutput
        While Not myStreamReader.EndOfStream
            Dim myString As String = myStreamReader.ReadLine()
            If myString.StartsWith("Interface") And myString.Contains(ipAddress) Then
                While Not myStreamReader.EndOfStream
                    myString = myStreamReader.ReadLine
                    If myString.StartsWith("Interface") Then
                        Exit While
                    End If

                    Dim macAddr As String = Nothing
                    Dim ipAddr As String = Nothing
                    Dim result = Regex.Match(myString, macRegex, RegexOptions.IgnoreCase)
                    If result IsNot Nothing Then
                        macAddr = result.Value.Trim.ToUpper()
                    End If
                    result = Regex.Match(myString, ipRegex, RegexOptions.IgnoreCase)
                    If result IsNot Nothing Then
                        ipAddr = result.Value.Trim
                    End If
                    If macAddr IsNot Nothing And ipAddr IsNot Nothing Then
                        Try
                            Dim existingNode = nodeList.First(Function(x) x.ipAddress = ipAddr)
                            If existingNode IsNot Nothing Then
                                existingNode.macAddress = macAddr
                            End If
                        Catch ex As Exception
                        End Try
                    End If
                End While
                Exit While
            End If
        End While

        myProcess.WaitForExit()
        myProcess.Close()
    End Sub

    Private Shared Sub getAllHostNames()
        SyncLock nodeList
            For Each node In nodeList
                getHostName(node)
                node.trustedIP = theDatabase.getIsIPTrusted(node)
                node.trustedMAC = theDatabase.getIsMACTrusted(node)
                node.trustedHostName = theDatabase.getIsHostNameTrusted(node)
                node.vendorName = theDatabase.getVendorName(node)
                theDatabase.upsertNode(node)
            Next
        End SyncLock
        pingDoneEvent.Set()
    End Sub

    Private Shared Sub getHostName(node As NetworkNode)
        Dim myProcess As New Process()
        Dim myProcessStartInfo As New ProcessStartInfo("nslookup.exe", node.ipAddress)
        myProcessStartInfo.UseShellExecute = False
        myProcessStartInfo.CreateNoWindow = True
        myProcessStartInfo.RedirectStandardOutput = True
        myProcessStartInfo.RedirectStandardError = True
        myProcessStartInfo.RedirectStandardInput = True
        myProcess.StartInfo = myProcessStartInfo
        myProcess.Start()
        Dim myStreamReader As StreamReader = myProcess.StandardOutput
        While Not myStreamReader.EndOfStream
            Dim myString As String = myStreamReader.ReadLine()
            If myString.StartsWith("Name") Then
                Dim hostName As String
                Try
                    hostName = myString.Split(CType(":", Char))(1).Trim
                    If Not myStreamReader.EndOfStream Then
                        myString = myStreamReader.ReadLine
                        If myString.StartsWith("Address") Then
                            Dim ipAddress As String = Nothing
                            For Each match As Match In Regex.Matches(myString, ipRegex, RegexOptions.IgnoreCase)
                                ipAddress = match.Value.Trim
                            Next
                            If ipAddress IsNot Nothing Then
                                If ipAddress = node.ipAddress Then
                                    node.hostName = hostName
                                    Exit While
                                End If
                            End If
                        End If
                    End If
                Catch ex As Exception
                End Try
            End If
        End While

        Try
            myStreamReader.Close()
        Catch ex As Exception
            Debug.WriteLine("Stream already closed")
        End Try

        myProcess.WaitForExit()
        myProcess.Close()

        If String.IsNullOrEmpty(node.hostName) Then
            node.hostName = node.ipAddress
        End If
    End Sub

    Private Shared Sub setCurrentIPAddress()
        Dim theInterfaceToUse = (From nic In NetworkInterface.GetAllNetworkInterfaces()
                    Where nic.OperationalStatus = OperationalStatus.Up And nic.NetworkInterfaceType <> NetworkInterfaceType.Loopback
                    Select nic
                    ).FirstOrDefault

        Dim ip = (From address In theInterfaceToUse.GetIPProperties.UnicastAddresses
                    Where Not address.Address.ToString.Contains(":")
                    Select address.Address
                ).FirstOrDefault
        ipAddress = ip.ToString
    End Sub

    Protected Overrides Sub OnStop()
        ' Add code here to perform any tear-down necessary to stop your service.
        Try
            host.Close()
        Catch ex As Exception
        End Try
        Try
            theDatabase.CloseConnection()
        Catch ex As Exception
        End Try
    End Sub

    Public Shared Sub Alert(ByVal theError As String, ByVal errorType As EventLogEntryType)
        Try
            myEventLog.WriteEntry(theError, errorType)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub


End Class
