<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PrintPreviewPage
    Inherits System.Windows.Forms.UserControl

    'UserControl はコンポーネント一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PrintPreviewPage))
        Me.TxtPageTotal = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TxtPage = New System.Windows.Forms.TextBox()
        Me.BtnLast = New System.Windows.Forms.Button()
        Me.BtnNext = New System.Windows.Forms.Button()
        Me.BtnPrev = New System.Windows.Forms.Button()
        Me.BtnFirst = New System.Windows.Forms.Button()
        Me.SlPage = New jp.co.systembase.report.renderer.gdi.Slider()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.SuspendLayout()
        '
        'TxtPageTotal
        '
        Me.TxtPageTotal.AutoSize = True
        Me.TxtPageTotal.Location = New System.Drawing.Point(125, 6)
        Me.TxtPageTotal.Name = "TxtPageTotal"
        Me.TxtPageTotal.Size = New System.Drawing.Size(29, 12)
        Me.TxtPageTotal.TabIndex = 11
        Me.TxtPageTotal.Text = "9999"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(155, 6)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(35, 12)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "ページ"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(113, 6)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(11, 12)
        Me.Label1.TabIndex = 10
        Me.Label1.Text = "/"
        '
        'TxtPage
        '
        Me.TxtPage.Location = New System.Drawing.Point(76, 3)
        Me.TxtPage.MaxLength = 6
        Me.TxtPage.Name = "TxtPage"
        Me.TxtPage.Size = New System.Drawing.Size(34, 19)
        Me.TxtPage.TabIndex = 8
        Me.TxtPage.Text = "1"
        Me.TxtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'BtnEnd
        '
        Me.BtnLast.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control
        Me.BtnLast.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnLast.Image = CType(resources.GetObject("BtnEnd.Image"), System.Drawing.Image)
        Me.BtnLast.Location = New System.Drawing.Point(231, 0)
        Me.BtnLast.Name = "BtnEnd"
        Me.BtnLast.Size = New System.Drawing.Size(32, 32)
        Me.BtnLast.TabIndex = 13
        Me.ToolTip.SetToolTip(Me.BtnLast, "最後のページへ移動")
        Me.BtnLast.UseVisualStyleBackColor = True
        '
        'BtnNext
        '
        Me.BtnNext.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control
        Me.BtnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnNext.Image = CType(resources.GetObject("BtnNext.Image"), System.Drawing.Image)
        Me.BtnNext.Location = New System.Drawing.Point(193, 0)
        Me.BtnNext.Name = "BtnNext"
        Me.BtnNext.Size = New System.Drawing.Size(32, 32)
        Me.BtnNext.TabIndex = 12
        Me.ToolTip.SetToolTip(Me.BtnNext, "次のページへ移動")
        Me.BtnNext.UseVisualStyleBackColor = True
        '
        'BtnPrev
        '
        Me.BtnPrev.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control
        Me.BtnPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPrev.Image = CType(resources.GetObject("BtnPrev.Image"), System.Drawing.Image)
        Me.BtnPrev.Location = New System.Drawing.Point(38, 0)
        Me.BtnPrev.Name = "BtnPrev"
        Me.BtnPrev.Size = New System.Drawing.Size(32, 32)
        Me.BtnPrev.TabIndex = 7
        Me.ToolTip.SetToolTip(Me.BtnPrev, "前のページへ移動")
        Me.BtnPrev.UseVisualStyleBackColor = True
        '
        'BtnBegin
        '
        Me.BtnFirst.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control
        Me.BtnFirst.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnFirst.Image = CType(resources.GetObject("BtnBegin.Image"), System.Drawing.Image)
        Me.BtnFirst.Location = New System.Drawing.Point(0, 0)
        Me.BtnFirst.Name = "BtnBegin"
        Me.BtnFirst.Size = New System.Drawing.Size(32, 32)
        Me.BtnFirst.TabIndex = 6
        Me.ToolTip.SetToolTip(Me.BtnFirst, "最初のページへ移動")
        Me.BtnFirst.UseVisualStyleBackColor = True
        '
        'SlPage
        '
        Me.SlPage.Location = New System.Drawing.Point(76, 20)
        Me.SlPage.MaxValue = 10
        Me.SlPage.MinValue = 0
        Me.SlPage.Name = "SlPage"
        Me.SlPage.Size = New System.Drawing.Size(114, 12)
        Me.SlPage.TabIndex = 14
        Me.SlPage.Text = "USlider1"
        Me.SlPage.Value = 0
        '
        'PrintPreviewPage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.SlPage)
        Me.Controls.Add(Me.TxtPageTotal)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.TxtPage)
        Me.Controls.Add(Me.BtnLast)
        Me.Controls.Add(Me.BtnNext)
        Me.Controls.Add(Me.BtnPrev)
        Me.Controls.Add(Me.BtnFirst)
        Me.Name = "PrintPreviewPage"
        Me.Size = New System.Drawing.Size(263, 32)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TxtPageTotal As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TxtPage As System.Windows.Forms.TextBox
    Friend WithEvents BtnLast As System.Windows.Forms.Button
    Friend WithEvents BtnNext As System.Windows.Forms.Button
    Friend WithEvents BtnPrev As System.Windows.Forms.Button
    Friend WithEvents BtnFirst As System.Windows.Forms.Button
    Friend WithEvents SlPage As jp.co.systembase.report.renderer.gdi.Slider
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip

End Class
