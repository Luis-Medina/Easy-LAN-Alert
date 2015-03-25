Public Class Network

    Public Property name As String
    Public Property id As Long
    Public Property monitor As Boolean
    Public Property type As String
    Public Property isValid As Boolean

    Public Sub New(name As String)
        Me.name = name
        type = "N/A"
        monitor = True
    End Sub

    Public Overrides Function toString() As String
        Return "ID: " & id & " | NAME: " & name & " | MONITOR = " & monitor
    End Function

End Class
