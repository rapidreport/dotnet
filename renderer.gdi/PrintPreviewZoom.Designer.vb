<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class PrintPreviewZoom
    Inherits System.Windows.Forms.UserControl

    'UserControl はコンポーネント一覧をクリーンアップするために dispose をオーバーライドします。
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

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PrintPreviewZoom))
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TxtZoom = New System.Windows.Forms.TextBox()
        Me.BtnZoomFit = New System.Windows.Forms.Button()
        Me.BtnZoomOut = New System.Windows.Forms.Button()
        Me.BtnZoomIn = New System.Windows.Forms.Button()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.BtnZoomFitWidth = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(76, 10)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(29, 12)
        Me.Label4.TabIndex = 11
        Me.Label4.Text = "倍率"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(144, 10)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(11, 12)
        Me.Label3.TabIndex = 10
        Me.Label3.Text = "%"
        '
        'TxtZoom
        '
        Me.TxtZoom.Location = New System.Drawing.Point(107, 7)
        Me.TxtZoom.MaxLength = 3
        Me.TxtZoom.Name = "TxtZoom"
        Me.TxtZoom.Size = New System.Drawing.Size(34, 19)
        Me.TxtZoom.TabIndex = 2
        Me.TxtZoom.Text = "100"
        Me.TxtZoom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'BtnZoomFit
        '
        Me.BtnZoomFit.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control
        Me.BtnZoomFit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnZoomFit.Image = CType(resources.GetObject("BtnZoomFit.Image"), System.Drawing.Image)
        Me.BtnZoomFit.Location = New System.Drawing.Point(194, 0)
        Me.BtnZoomFit.Name = "BtnZoomFit"
        Me.BtnZoomFit.Size = New System.Drawing.Size(32, 32)
        Me.BtnZoomFit.TabIndex = 4
        Me.ToolTip.SetToolTip(Me.BtnZoomFit, "画面サイズに合わせて拡大/縮小")
        Me.BtnZoomFit.UseVisualStyleBackColor = True
        '
        'BtnZoomOut
        '
        Me.BtnZoomOut.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control
        Me.BtnZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnZoomOut.Image = CType(resources.GetObject("BtnZoomOut.Image"), System.Drawing.Image)
        Me.BtnZoomOut.Location = New System.Drawing.Point(38, 0)
        Me.BtnZoomOut.Name = "BtnZoomOut"
        Me.BtnZoomOut.Size = New System.Drawing.Size(32, 32)
        Me.BtnZoomOut.TabIndex = 1
        Me.ToolTip.SetToolTip(Me.BtnZoomOut, "縮小")
        Me.BtnZoomOut.UseVisualStyleBackColor = True
        '
        'BtnZoomIn
        '
        Me.BtnZoomIn.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control
        Me.BtnZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnZoomIn.Image = CType(resources.GetObject("BtnZoomIn.Image"), System.Drawing.Image)
        Me.BtnZoomIn.Location = New System.Drawing.Point(0, 0)
        Me.BtnZoomIn.Name = "BtnZoomIn"
        Me.BtnZoomIn.Size = New System.Drawing.Size(32, 32)
        Me.BtnZoomIn.TabIndex = 0
        Me.ToolTip.SetToolTip(Me.BtnZoomIn, "拡大")
        Me.BtnZoomIn.UseVisualStyleBackColor = True
        '
        'BtnZoomFitWidth
        '
        Me.BtnZoomFitWidth.FlatAppearance.BorderColor = System.Drawing.SystemColors.Control
        Me.BtnZoomFitWidth.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.BtnZoomFitWidth.Image = CType(resources.GetObject("BtnZoomFitWidth.Image"), System.Drawing.Image)
        Me.BtnZoomFitWidth.Location = New System.Drawing.Point(161, 0)
        Me.BtnZoomFitWidth.Name = "BtnZoomFitWidth"
        Me.BtnZoomFitWidth.Size = New System.Drawing.Size(32, 32)
        Me.BtnZoomFitWidth.TabIndex = 3
        Me.ToolTip.SetToolTip(Me.BtnZoomFitWidth, "画面幅に合わせて拡大/縮小")
        Me.BtnZoomFitWidth.UseVisualStyleBackColor = True
        '
        'PrintPreviewZoom
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TxtZoom)
        Me.Controls.Add(Me.BtnZoomFitWidth)
        Me.Controls.Add(Me.BtnZoomFit)
        Me.Controls.Add(Me.BtnZoomOut)
        Me.Controls.Add(Me.BtnZoomIn)
        Me.Name = "PrintPreviewZoom"
        Me.Size = New System.Drawing.Size(229, 32)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TxtZoom As System.Windows.Forms.TextBox
    Friend WithEvents BtnZoomFit As System.Windows.Forms.Button
    Friend WithEvents BtnZoomOut As System.Windows.Forms.Button
    Friend WithEvents BtnZoomIn As System.Windows.Forms.Button
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents BtnZoomFitWidth As System.Windows.Forms.Button

End Class
