Public Class NetworkNode

    Public Property ipAddress As String
    Public Property macAddress As String
    Public Property vendorName As String
    Public Property status As String
    Public Property hostName As String
    Public Property trustedIP As Boolean
    Public Property trustedMAC As Boolean
    Public Property trustedHostName As Boolean
    Public Property networkID As Long
    Public Property id As Long
    Public Const DEFAULT_MAC As String = "00-00-00-00-00-00"
    Public Const DEFAULT_VENDOR As String = "Unknown vendor"
    Public Const DEFAULT_STATUS As String = "N/A"

    Public Sub New()
        ipAddress = ""
        macAddress = DEFAULT_MAC
        vendorName = DEFAULT_VENDOR
        status = DEFAULT_STATUS
        hostName = ""
    End Sub

    Public Sub New(ipAddress As String)
        Me.New()
        Me.ipAddress = ipAddress
    End Sub

    Public Overrides Function toString() As String
        Return "IP: " & ipAddress & " | MAC: " & macAddress & " | STATUS: " & status & " | HOSTNAME: " & hostName & " | TIP: " & trustedIP.ToString & " | TMAC: " & trustedMAC.ToString & " | TNAME: " & trustedHostName.ToString
    End Function

    Public Function isUntrusted() As Boolean
        If Not trustedIP And Not trustedMAC And Not trustedHostName Then
            Return True
        End If
        Return False
    End Function

    Public Function toJSONString() As String
        Return "{""IP"":""" & ipAddress & """, ""MAC"":""" & macAddress & """, ""HOSTNAME"":""" & hostName & """, ""VENDOR"":""" & vendorName & """}"
    End Function

End Class
