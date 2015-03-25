Imports System.ServiceModel

<ServiceContract(CallbackContract:=GetType(ClientToServiceCallback))>
Public Interface ClientToServiceInterface

    <OperationContract(IsOneWay:=True)> Sub QueryStatus()
    <OperationContract(IsOneWay:=True)> Sub StartScan()

End Interface

Public Interface ClientToServiceCallback

    <OperationContract(IsOneWay:=True)> Sub GetRunningStatus(isRunning As Boolean, lastRunTime As DateTime?, nextRunTime As DateTime?, nodeList As List(Of NetworkNode))

End Interface

<ServiceContract()>
Public Interface IServiceToClient

    <OperationContract(IsOneWay:=True)> Sub ScanStarted()
    <OperationContract(IsOneWay:=True)> Sub ScanFinished(nodes As List(Of NetworkNode), lastRunTime As DateTime?, nextRunTime As DateTime?)
    <OperationContract(IsOneWay:=True)> Sub NextScanTime(nextRunTime As DateTime?)

End Interface
