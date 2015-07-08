Public Class FmTest

    Private Sub FmTest_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Me.CmbTest.Items.AddRange(Tests.ToArray)
        Me.CmbTest.SelectedIndex = Tests.Count - 1
    End Sub

    Private Sub BtnRun_Click(sender As System.Object, e As System.EventArgs) Handles BtnRun.Click
        Dim i As Object = Me.CmbTest.SelectedItem
        If i IsNot Nothing Then
            i.Run()
        End If
    End Sub

    Private Sub BtnOpenOut_Click(sender As System.Object, e As System.EventArgs) Handles BtnOpenOut.Click
        System.Diagnostics.Process.Start("out")
    End Sub

End Class
