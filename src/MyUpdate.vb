'
' 日付: 2016/05/07
'
Imports Common.Util

Public Class MyUpdate
  Inherits Update
  Private latestExecutableFilesPath As String
  
  Public Sub New(latestVersionFilePath As String, latestExecutableFilesPath As String)
    MyBase.New(latestVersionFilePath, "./__update__.bat")
    Me.latestExecutableFilesPath = latestExecutableFilesPath
  End Sub
  
  Protected Overrides Function Script() As String
    If latestExecutableFilesPath = String.Empty Then
      Return ""
    Else
      Dim path As String = System.IO.Directory.GetCurrentDirectory
      Return New CopyBat(latestExecutableFilesPath, path).ToString()
    End If
  End Function
End Class

Public Class CopyBat
  Private bat As String
  
  Public Sub New(from As String, _to As String)
    bat = String.Format("XCOPY /Y {0} {1}", from, _to)	
  End Sub
  
  Public Overrides Function ToString() As String
    Return bat
  End Function
End Class
