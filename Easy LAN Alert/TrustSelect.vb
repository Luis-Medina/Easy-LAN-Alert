Public Class TrustSelect

    Public Const TRUST_ALL As Integer = 1
    Public Const TRUST_NONE As Integer = 2
    Private Shared type As Integer

    Private Sub TrustSelect_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If type = TRUST_ALL Then
            Label1.Text = "Select trust for all devices:"
        ElseIf type = TRUST_NONE Then
            Label1.Text = "Remove trust for all devices:"
        Else
            MsgBox("Invalid type", MsgBoxStyle.Critical)
            Me.Close()
        End If
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub New(trustType As Integer)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        type = trustType
    End Sub

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Dim form As Form1 = CType(Application.OpenForms(0), Form1)
        Debug.Assert(form IsNot Nothing)

        form.setTrustForAll(type, If(CheckBox1.Checked, True, False), If(CheckBox2.Checked, True, False), If(CheckBox3.Checked, True, False))
        Me.Close()
    End Sub
End Class