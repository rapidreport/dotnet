<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PrintPreviewSearchPanel
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

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.TxtKeyword = New System.Windows.Forms.TextBox()
        Me.BtnPrev = New System.Windows.Forms.Button()
        Me.BtnNext = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'TxtKeyword
        '
        Me.TxtKeyword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TxtKeyword.Location = New System.Drawing.Point(0, 1)
        Me.TxtKeyword.Name = "TxtKeyword"
        Me.TxtKeyword.Size = New System.Drawing.Size(140, 19)
        Me.TxtKeyword.TabIndex = 0
        '
        'BtnPrev
        '
        Me.BtnPrev.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder
        Me.BtnPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnPrev.Location = New System.Drawing.Point(140, 1)
        Me.BtnPrev.Name = "BtnPrev"
        Me.BtnPrev.Size = New System.Drawing.Size(20, 19)
        Me.BtnPrev.TabIndex = 1
        Me.BtnPrev.Text = "<"
        Me.BtnPrev.UseVisualStyleBackColor = True
        '
        'BtnNext
        '
        Me.BtnNext.FlatAppearance.BorderColor = System.Drawing.SystemColors.ActiveBorder
        Me.BtnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnNext.Location = New System.Drawing.Point(160, 1)
        Me.BtnNext.Name = "BtnNext"
        Me.BtnNext.Size = New System.Drawing.Size(20, 19)
        Me.BtnNext.TabIndex = 2
        Me.BtnNext.Text = ">"
        Me.BtnNext.UseVisualStyleBackColor = True
        '
        'PrintPreviewSearchPanel
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.BtnNext)
        Me.Controls.Add(Me.BtnPrev)
        Me.Controls.Add(Me.TxtKeyword)
        Me.Name = "PrintPreviewSearchPanel"
        Me.Size = New System.Drawing.Size(180, 20)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TxtKeyword As System.Windows.Forms.TextBox
    Friend WithEvents BtnPrev As System.Windows.Forms.Button
    Friend WithEvents BtnNext As System.Windows.Forms.Button

End Class
