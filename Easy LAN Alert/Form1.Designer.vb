<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.TrustAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.TrustNone = New System.Windows.Forms.ToolStripMenuItem()
        Me.UpdateDatabase = New System.Windows.Forms.ToolStripMenuItem()
        Me.ServiceControl = New System.Windows.Forms.ToolStripMenuItem()
        Me.ScanInterval = New System.Windows.Forms.ToolStripMenuItem()
        Me.AlertSettings = New System.Windows.Forms.ToolStripMenuItem()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.IpAddressDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HostNameDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.MacAddressDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.VendorNameDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.StatusDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.TrustedIPDataGridViewCheckBoxColumn = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.TrustedMACDataGridViewCheckBoxColumn = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.TrustedHostNameDataGridViewCheckBoxColumn = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.IdDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.NetworkNodeBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.MenuStrip1.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.NetworkNodeBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TrustAll, Me.TrustNone, Me.UpdateDatabase, Me.ServiceControl, Me.ScanInterval, Me.AlertSettings})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(825, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'TrustAll
        '
        Me.TrustAll.Name = "TrustAll"
        Me.TrustAll.Size = New System.Drawing.Size(63, 20)
        Me.TrustAll.Text = "Trust All"
        '
        'TrustNone
        '
        Me.TrustNone.Name = "TrustNone"
        Me.TrustNone.Size = New System.Drawing.Size(78, 20)
        Me.TrustNone.Text = "Trust None"
        '
        'UpdateDatabase
        '
        Me.UpdateDatabase.Name = "UpdateDatabase"
        Me.UpdateDatabase.Size = New System.Drawing.Size(149, 20)
        Me.UpdateDatabase.Text = "Update Vendor Database"
        '
        'ServiceControl
        '
        Me.ServiceControl.Name = "ServiceControl"
        Me.ServiceControl.Size = New System.Drawing.Size(83, 20)
        Me.ServiceControl.Text = "Stop Service"
        '
        'ScanInterval
        '
        Me.ScanInterval.Name = "ScanInterval"
        Me.ScanInterval.Size = New System.Drawing.Size(86, 20)
        Me.ScanInterval.Text = "Scan Interval"
        '
        'AlertSettings
        '
        Me.AlertSettings.Name = "AlertSettings"
        Me.AlertSettings.Size = New System.Drawing.Size(89, 20)
        Me.AlertSettings.Text = "Alert Settings"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(12, 55)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "Scan"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Location = New System.Drawing.Point(105, 55)
        Me.ProgressBar1.MarqueeAnimationSpeed = 5
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(494, 23)
        Me.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee
        Me.ProgressBar1.TabIndex = 2
        Me.ProgressBar1.Visible = False
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(617, 55)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(61, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "IP Address:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(617, 75)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(74, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "MAC Address:"
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.AllowUserToResizeRows = False
        Me.DataGridView1.AutoGenerateColumns = False
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.IpAddressDataGridViewTextBoxColumn, Me.HostNameDataGridViewTextBoxColumn, Me.MacAddressDataGridViewTextBoxColumn, Me.VendorNameDataGridViewTextBoxColumn, Me.StatusDataGridViewTextBoxColumn, Me.TrustedIPDataGridViewCheckBoxColumn, Me.TrustedMACDataGridViewCheckBoxColumn, Me.TrustedHostNameDataGridViewCheckBoxColumn, Me.IdDataGridViewTextBoxColumn})
        Me.DataGridView1.DataSource = Me.NetworkNodeBindingSource
        Me.DataGridView1.Location = New System.Drawing.Point(12, 121)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.Size = New System.Drawing.Size(801, 371)
        Me.DataGridView1.TabIndex = 7
        '
        'IpAddressDataGridViewTextBoxColumn
        '
        Me.IpAddressDataGridViewTextBoxColumn.DataPropertyName = "ipAddress"
        Me.IpAddressDataGridViewTextBoxColumn.HeaderText = "IP Address"
        Me.IpAddressDataGridViewTextBoxColumn.Name = "IpAddressDataGridViewTextBoxColumn"
        '
        'HostNameDataGridViewTextBoxColumn
        '
        Me.HostNameDataGridViewTextBoxColumn.DataPropertyName = "hostName"
        Me.HostNameDataGridViewTextBoxColumn.HeaderText = "Name"
        Me.HostNameDataGridViewTextBoxColumn.Name = "HostNameDataGridViewTextBoxColumn"
        '
        'MacAddressDataGridViewTextBoxColumn
        '
        Me.MacAddressDataGridViewTextBoxColumn.DataPropertyName = "macAddress"
        Me.MacAddressDataGridViewTextBoxColumn.HeaderText = "MAC Address"
        Me.MacAddressDataGridViewTextBoxColumn.Name = "MacAddressDataGridViewTextBoxColumn"
        Me.MacAddressDataGridViewTextBoxColumn.Width = 120
        '
        'VendorNameDataGridViewTextBoxColumn
        '
        Me.VendorNameDataGridViewTextBoxColumn.DataPropertyName = "vendorName"
        Me.VendorNameDataGridViewTextBoxColumn.HeaderText = "Vendor"
        Me.VendorNameDataGridViewTextBoxColumn.Name = "VendorNameDataGridViewTextBoxColumn"
        Me.VendorNameDataGridViewTextBoxColumn.Width = 240
        '
        'StatusDataGridViewTextBoxColumn
        '
        Me.StatusDataGridViewTextBoxColumn.DataPropertyName = "status"
        Me.StatusDataGridViewTextBoxColumn.HeaderText = "PING"
        Me.StatusDataGridViewTextBoxColumn.Name = "StatusDataGridViewTextBoxColumn"
        '
        'TrustedIPDataGridViewCheckBoxColumn
        '
        Me.TrustedIPDataGridViewCheckBoxColumn.DataPropertyName = "trustedIP"
        Me.TrustedIPDataGridViewCheckBoxColumn.HeaderText = "Trust IP"
        Me.TrustedIPDataGridViewCheckBoxColumn.Name = "TrustedIPDataGridViewCheckBoxColumn"
        Me.TrustedIPDataGridViewCheckBoxColumn.Width = 40
        '
        'TrustedMACDataGridViewCheckBoxColumn
        '
        Me.TrustedMACDataGridViewCheckBoxColumn.DataPropertyName = "trustedMAC"
        Me.TrustedMACDataGridViewCheckBoxColumn.HeaderText = "Trust MAC"
        Me.TrustedMACDataGridViewCheckBoxColumn.Name = "TrustedMACDataGridViewCheckBoxColumn"
        Me.TrustedMACDataGridViewCheckBoxColumn.Width = 40
        '
        'TrustedHostNameDataGridViewCheckBoxColumn
        '
        Me.TrustedHostNameDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.TrustedHostNameDataGridViewCheckBoxColumn.DataPropertyName = "trustedHostName"
        Me.TrustedHostNameDataGridViewCheckBoxColumn.HeaderText = "Trust Name"
        Me.TrustedHostNameDataGridViewCheckBoxColumn.Name = "TrustedHostNameDataGridViewCheckBoxColumn"
        '
        'IdDataGridViewTextBoxColumn
        '
        Me.IdDataGridViewTextBoxColumn.DataPropertyName = "id"
        Me.IdDataGridViewTextBoxColumn.HeaderText = "id"
        Me.IdDataGridViewTextBoxColumn.Name = "IdDataGridViewTextBoxColumn"
        Me.IdDataGridViewTextBoxColumn.Visible = False
        '
        'NetworkNodeBindingSource
        '
        Me.NetworkNodeBindingSource.DataSource = GetType(Easy_LAN_Alert_DLL.NetworkNode)
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 105)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(46, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Devices"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(699, 55)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(88, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "255.255.255.255"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(699, 75)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(94, 13)
        Me.Label5.TabIndex = 10
        Me.Label5.Text = "77-77-77-77-77-77"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 512)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(39, 13)
        Me.Label6.TabIndex = 11
        Me.Label6.Text = "Label6"
        Me.Label6.Visible = False
        '
        'Label7
        '
        Me.Label7.Location = New System.Drawing.Point(604, 502)
        Me.Label7.Name = "Label7"
        Me.Label7.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label7.Size = New System.Drawing.Size(209, 13)
        Me.Label7.TabIndex = 12
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label8
        '
        Me.Label8.Location = New System.Drawing.Point(604, 523)
        Me.Label8.Name = "Label8"
        Me.Label8.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.Label8.Size = New System.Drawing.Size(209, 13)
        Me.Label8.TabIndex = 13
        Me.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(825, 545)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Easy LAN Alert"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.NetworkNodeBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents TrustAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TrustNone As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UpdateDatabase As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ServiceControl As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ScanInterval As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AlertSettings As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents IpAddressDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents HostNameDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents MacAddressDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents VendorNameDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents StatusDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents TrustedIPDataGridViewCheckBoxColumn As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents TrustedMACDataGridViewCheckBoxColumn As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents TrustedHostNameDataGridViewCheckBoxColumn As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents IdDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents NetworkNodeBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label

End Class
