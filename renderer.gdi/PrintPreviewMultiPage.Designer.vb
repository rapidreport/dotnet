<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PrintPreviewMultiPage
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PrintPreviewMultiPage))
        Me.BtnMultiPage = New System.Windows.Forms.Button()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.SuspendLayout()
        '
        'BtnMultiPage
        '
        Me.BtnMultiPage.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control
        Me.BtnMultiPage.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnMultiPage.Image = CType(resources.GetObject("BtnMultiPage.Image"), System.Drawing.Image)
        Me.BtnMultiPage.Location = New System.Drawing.Point(0, 0)
        Me.BtnMultiPage.Name = "BtnMultiPage"
        Me.BtnMultiPage.Size = New System.Drawing.Size(34, 32)
        Me.BtnMultiPage.TabIndex = 1
        Me.ToolTip.SetToolTip(Me.BtnMultiPage, "見開き表示")
        Me.BtnMultiPage.UseVisualStyleBackColor = True
        '
        'PrintPreviewMultiPage
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.BtnMultiPage)
        Me.Name = "PrintPreviewMultiPage"
        Me.Size = New System.Drawing.Size(34, 32)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents BtnMultiPage As Button
    Friend WithEvents ToolTip As ToolTip
End Class
