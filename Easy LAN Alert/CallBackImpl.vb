Imports System.ServiceModel
Imports System.Threading
Imports Easy_LAN_Alert_DLL

Public Class CallBackImpl
    Implements IServiceToClient

    Public Sub ScanStarted() Implements IServiceToClient.ScanStarted
        Dim form As Form1 = CType(Application.OpenForms(0), Form1)
        Debug.Assert(form IsNot Nothing)

        form.SetUIForStatus(True, Nothing, Nothing, Nothing)
    End Sub

    Public Sub ScanFinished(nodes As List(Of NetworkNode), lastRunTime As DateTime?, nextRunTime As DateTime?) Implements IServiceToClient.ScanFinished
        Dim form As Form1 = CType(Application.OpenForms(0), Form1)
        Debug.Assert(form IsNot Nothing)

        form.ScanIsDone(nodes, lastRunTime, nextRunTime)
    End Sub

    Public Sub ReceivedNextScanTime(nextRunTime As DateTime?) Implements IServiceToClient.NextScanTime
        Dim form As Form1 = CType(Application.OpenForms(0), Form1)
        Debug.Assert(form IsNot Nothing)

        form.SetServiceStatus(Nothing, nextRunTime)
    End Sub

End Class
