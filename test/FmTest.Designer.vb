<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FmTest
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
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

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.CmbTest = New System.Windows.Forms.ComboBox()
        Me.BtnRun = New System.Windows.Forms.Button()
        Me.BtnOpenOut = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'CmbTest
        '
        Me.CmbTest.FormattingEnabled = True
        Me.CmbTest.Location = New System.Drawing.Point(12, 12)
        Me.CmbTest.Name = "CmbTest"
        Me.CmbTest.Size = New System.Drawing.Size(260, 20)
        Me.CmbTest.TabIndex = 0
        '
        'BtnRun
        '
        Me.BtnRun.Location = New System.Drawing.Point(152, 38)
        Me.BtnRun.Name = "BtnRun"
        Me.BtnRun.Size = New System.Drawing.Size(120, 23)
        Me.BtnRun.TabIndex = 1
        Me.BtnRun.Text = "実行"
        Me.BtnRun.UseVisualStyleBackColor = True
        '
        'BtnOpenOut
        '
        Me.BtnOpenOut.Location = New System.Drawing.Point(152, 67)
        Me.BtnOpenOut.Name = "BtnOpenOut"
        Me.BtnOpenOut.Size = New System.Drawing.Size(120, 23)
        Me.BtnOpenOut.TabIndex = 2
        Me.BtnOpenOut.Text = "出力フォルダを開く"
        Me.BtnOpenOut.UseVisualStyleBackColor = True
        '
        'FmTest
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 97)
        Me.Controls.Add(Me.BtnOpenOut)
        Me.Controls.Add(Me.BtnRun)
        Me.Controls.Add(Me.CmbTest)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "FmTest"
        Me.Text = "test"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents CmbTest As System.Windows.Forms.ComboBox
    Friend WithEvents BtnRun As System.Windows.Forms.Button
    Friend WithEvents BtnOpenOut As System.Windows.Forms.Button

End Class
