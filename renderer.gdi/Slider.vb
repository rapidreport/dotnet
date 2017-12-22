Public Class Slider
    Inherits Control

    Public Event ValueChanged(sender As Object)

    Public ChangeUnit As Integer = 1
    Private _MinValue As Integer = 0
    Private _MaxValue As Integer = 10
    Private _Value As Integer = 0

    Private _mouseDown As Boolean = False
    Private _gripped As Boolean = False
    Private _pointedValue As Integer = 0

    Private WithEvents timer As New Timer

    Public Sub New()
        Me.DoubleBuffered = True
        Me.SetStyle(ControlStyles.Selectable, False)
    End Sub

    Private Sub USlider_HandleDestroyed(sender As Object, e As System.EventArgs) Handles Me.HandleDestroyed
        Me.timer.Enabled = False
        Me.timer.Dispose()
        Me.timer = Nothing
    End Sub

    Public Property MaxValue() As Integer
        Get
            Return Me._MaxValue
        End Get
        Set(value As Integer)
            If value >= Me.MinValue Then
                Me._MaxValue = value
            End If
            If Me.Value > Me.MaxValue Then
                Me.Value = Me.MaxValue
            End If
        End Set
    End Property

    Public Property MinValue() As Integer
        Get
            Return Me._MinValue
        End Get
        Set(value As Integer)
            If value <= Me.MaxValue Then
                Me._MinValue = value
            End If
            If Me.Value < Me.MinValue Then
                Me.Value = Me.MinValue
            End If
        End Set
    End Property

    Public Property Value() As Integer
        Get
            Return Me._Value
        End Get
        Set(value As Integer)
            Dim bak As Integer = Me.Value
            Me._Value = value
            If Me._Value > Me.MaxValue Then
                Me._Value = Me.MaxValue
            End If
            If Me._Value < Me.MinValue Then
                Me._Value = Me.MinValue
            End If
            If Me.Value <> bak Then
                RaiseEvent ValueChanged(Me)
            End If
            Me.Invalidate()
        End Set
    End Property

    Private Sub USlider_Paint(sender As Object, e As System.Windows.Forms.PaintEventArgs) Handles Me.Paint
        Dim g As Graphics = e.Graphics
        Dim w As Integer = Me.Width - 10
        With Nothing
            Dim y As Integer = Me.Height / 2
            g.DrawLine(Pens.Gray, New Point(4, y - 2), New Point(4 + w, y - 2))
            g.DrawLine(Pens.Gray, New Point(4, y - 2), New Point(4, y + 2))
            g.DrawLine(Pens.White, New Point(4, y + 2), New Point(4 + w, y + 2))
            g.DrawLine(Pens.White, New Point(4 + w, y - 2), New Point(4 + w, y + 2))
        End With
        Dim x As Integer = w / 2
        With Nothing
            Dim d As Integer = Me.MaxValue - Me.MinValue
            If d > 0 Then
                x = (w / d) * (Me.Value - Me.MinValue)
            End If
        End With
        With Nothing
            Dim rect As New Rectangle(x, Me.Height / 2 - 5, 8, 10)
            Dim c As Color = IIf(Me._mouseDown, Color.SkyBlue, Color.LightGray)
            Dim b As New Drawing2D.LinearGradientBrush( _
              rect, _
              Color.White, _
              c, _
              Drawing2D.LinearGradientMode.Vertical)
            g.FillRectangle(b, rect)
            g.DrawLine(Pens.Gray, rect.Left, rect.Top + 1, rect.Left, rect.Bottom - 1)
            g.DrawLine(Pens.Gray, rect.Right, rect.Top + 1, rect.Right, rect.Bottom - 1)
            g.DrawLine(Pens.Gray, rect.Left + 1, rect.Top, rect.Right - 1, rect.Top)
            g.DrawLine(Pens.Gray, rect.Left + 1, rect.Bottom, rect.Right - 1, rect.Bottom)
        End With
    End Sub

    Private Sub USlider_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Me._mouseDown = True
            Me._pointedValue = xToValue(e.Location.X)
            Me.moveValue()
            Me.timer.Interval = 500
            Me.timer.Enabled = True
        End If
    End Sub

    Private Sub USlider_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Me._mouseDown = False
            Me._gripped = False
            Me.timer.Enabled = False
            Me.Invalidate()
        End If
    End Sub

    Private Sub USlider_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        If Me._mouseDown Then
            Me._pointedValue = xToValue(e.Location.X)
            If Me._gripped Then
                Me.Value = Me._pointedValue
            End If
        End If
    End Sub

    Private Sub timer_Tick(sender As Object, e As System.EventArgs) Handles timer.Tick
        Me.moveValue()
        If Me._gripped Then
            Me.timer.Enabled = False
        Else
            Me.timer.Interval = 70
        End If
    End Sub

    Private Function xToValue(x As Integer)
        Return ((Me.MaxValue - Me.MinValue) * (x - 5)) / (Me.Width - 10) + Me.MinValue
    End Function

    Private Sub moveValue()
        If Math.Abs(Me.Value - Me._pointedValue) <= Me.ChangeUnit Then
            Me._gripped = True
            Me.Value = Me._pointedValue
        Else
            If Me._pointedValue < Me.Value Then
                Me.Value -= Me.ChangeUnit
            Else
                Me.Value += Me.ChangeUnit
            End If
        End If
    End Sub

End Class
