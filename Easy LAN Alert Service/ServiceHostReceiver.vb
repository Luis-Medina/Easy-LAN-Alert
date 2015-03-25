Imports Easy_LAN_Alert_DLL
Imports System.ServiceModel
Imports System.Threading

Public Class ServiceHostReceiver
    Implements ClientToServiceInterface

    Private Sub SendStatus() Implements ClientToServiceInterface.QueryStatus
        Try
            Dim callback = OperationContext.Current.GetCallbackChannel(Of ClientToServiceCallback)()
            callback.GetRunningStatus(EasyLANAlertService.isRunning, EasyLANAlertService.lastRunTime, EasyLANAlertService.nextRunTime, EasyLANAlertService.nodeList)
        Catch ex As Exception
            Debug.WriteLine(ex.Message)
        End Try
    End Sub

    Private Sub StartScan() Implements ClientToServiceInterface.StartScan
        If Not EasyLANAlertService.isRunning Then
            EasyLANAlertService.serviceTimer.Stop()
            EasyLANAlertService.cameFromUI = True
            EasyLANAlertService.OnTimedEvent()
        End If
    End Sub

End Class

