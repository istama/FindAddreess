'
' 日付: 2016/05/05
'
Imports Common.IO
Imports Common.Util

Public Class MyProperties
  Inherits AppProperties
  
  ' プロパティのキー値（Constを付けたフィールドは静的定数になる）
  Public Const KEY_APPLICATION_TITLE    = "ApplicationTitle"
  
  Public Const KEY_VERSION_FILEPATH     = "VersionFilePath"
  Public Const KEY_LATEST_EXE_FILES_DIR = "LatestExeFilesDir"
  Public Const KEY_IS_UPDATE_RUNNABLE   = "IsAutoUpdateRunnable"
  
  Public Const KEY_ADDRESS_DATABASE_DIR = "AddressDatabaseDir"
  
  ''' <summary>
  ''' コンストラクタ。
  ''' </summary>
  ''' <param name="filePath">このアプリケーションのプロパティファイルのパス</param>
  Public Sub New(filePath As String)
    MyBase.New(filePath)
  End Sub
  
  ''' <summary>
  ''' プロパティのデフォルト値。
  ''' </summary>
  ''' <returns></returns>
  Protected Overrides Function DefaultProperties() As IDictionary(Of String, String)
    Dim p As New Dictionary(Of String, String)
    
    p.Add(KEY_APPLICATION_TITLE, "FindAddress")				
    
    p.Add(KEY_IS_UPDATE_RUNNABLE, "False")
    p.Add(KEY_VERSION_FILEPATH, "")
    p.Add(KEY_LATEST_EXE_FILES_DIR, "")
    p.Add(KEY_ADDRESS_DATABASE_DIR, "")
    
    Return p
  End Function
  
  ''' <summary>
  ''' デフォルトにないプロパティがプロパティファイルにあることを認めるかどうか。
  ''' </summary>
  ''' <returns></returns>
  Protected Overrides Function AllowNonDefaultProperty() As Boolean
    Return False
  End Function
  
  ''' <summary>
  ''' アップデート機能をＯＮしているかどうか。
  ''' </summary>
  ''' <returns></returns>
  Public Function IsUpdateRunnable() As Boolean
    Dim isUpdate As String = Me.GetValue(MyProperties.KEY_IS_UPDATE_RUNNABLE).GetOrDefault("")
    Return isUpdate.ToLower = "true"
  End Function
  
  ''' <summary>
  ''' 最新バージョンが記述されたファイルのパスを取得する。
  ''' パスが存在すれば参照渡しの引数にパスをセットし、Trueを返す。
  ''' 存在しなければ何もせずFalseを返す。
  ''' </summary>
  Public Function TryGetVersionFilePath(ByRef path As String) As Boolean
    Dim tmp As String = Me.GetValue(MyProperties.KEY_VERSION_FILEPATH).GetOrDefault("")
    If tmp <> String.Empty Then
      path = tmp
      Return True
    Else
      Return False
    End If
  End Function
  
  ''' <summary>
  ''' 最新バージョンの実行ファイルが格納されたディレクトリのパスを取得する。
  ''' パスが存在すれば参照渡しの引数にパスをセットし、Trueを返す。
  ''' 存在しなければ何もせずFalseを返す。
  ''' </summary>
  Public Function TryGetLatestExeFilesDir(ByRef dir As String) As Boolean
    Dim tmp As String = Me.GetValue(MyProperties.KEY_LATEST_EXE_FILES_DIR).GetOrDefault("")
    If tmp <> String.Empty Then
      dir = tmp
      Return True
    Else
      Return False
    End If    
  End Function
  
  ''' <summary>
  ''' 住所データベースのディレクトリのパスを取得する。
  ''' パスが存在すれば参照渡しの引数にパスをセットし、Trueを返す。
  ''' 存在しなければ何もせずFalseを返す。
  ''' </summary>
  Public Function TryGetAddressDBDir(ByRef dir As String) As Boolean
    Dim tmp As String = Me.GetValue(MyProperties.KEY_ADDRESS_DATABASE_DIR).GetOrDefault("")
    If tmp <> String.Empty Then
      dir = tmp
      Return True
    Else
      Return False
    End If    
  End Function
End Class
