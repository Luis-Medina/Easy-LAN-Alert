<System.ComponentModel.RunInstaller(True)> Partial Class ProjectInstaller
    Inherits System.Configuration.Install.Installer

    'Installer overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.EasyLANAlertServiceProcessInstaller = New System.ServiceProcess.ServiceProcessInstaller()
        Me.EasyLANAlertServiceInstaller = New System.ServiceProcess.ServiceInstaller()
        '
        'EasyLANAlertServiceProcessInstaller
        '
        Me.EasyLANAlertServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem
        Me.EasyLANAlertServiceProcessInstaller.Password = Nothing
        Me.EasyLANAlertServiceProcessInstaller.Username = Nothing
        '
        'EasyLANAlertServiceInstaller
        '
        Me.EasyLANAlertServiceInstaller.Description = "Easy LAN Alert Service"
        Me.EasyLANAlertServiceInstaller.DisplayName = "Easy LAN Alert Service"
        Me.EasyLANAlertServiceInstaller.ServiceName = "Easy LAN Alert"
        Me.EasyLANAlertServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic
        '
        'ProjectInstaller
        '
        Me.Installers.AddRange(New System.Configuration.Install.Installer() {Me.EasyLANAlertServiceProcessInstaller, Me.EasyLANAlertServiceInstaller})

    End Sub
    Friend WithEvents EasyLANAlertServiceProcessInstaller As System.ServiceProcess.ServiceProcessInstaller
    Friend WithEvents EasyLANAlertServiceInstaller As System.ServiceProcess.ServiceInstaller

End Class
