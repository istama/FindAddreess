'
' SharpDevelopによって生成
' ユーザ: Blue
' 日付: 2016/10/08
' 時刻: 22:30
' 
' このテンプレートを変更する場合「ツール→オプション→コーディング→標準ヘッダの編集」
'
Imports Microsoft.VisualBasic.ApplicationServices

Namespace My
  ' This file controls the behaviour of the application.
  Partial Class MyApplication
    Public Sub New()
      MyBase.New(AuthenticationMode.Windows)
      Me.IsSingleInstance = False
      Me.EnableVisualStyles = True
      Me.SaveMySettingsOnExit = True
      Me.ShutDownStyle = ShutdownMode.AfterMainFormCloses
    End Sub
    
    Protected Overrides Sub OnCreateMainForm()
      Me.MainForm = My.Forms.MainForm
    End Sub
  End Class
End Namespace
