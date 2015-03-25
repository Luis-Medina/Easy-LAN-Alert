Public Class Vendor

    Public Property name As String
    Public Property hwId As String

    Public Sub New(name As String, hwID As String)
        Me.name = name
        Me.hwId = hwID
    End Sub

    Public Overrides Function ToString() As String
        Return "MAC: " & hwId & " | VENDOR: " & name
    End Function

End Class
