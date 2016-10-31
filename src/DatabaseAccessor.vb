﻿'
' 日付: 2016/10/10
'
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Linq

Imports Common.IO
Imports Common.Extensions

''' <summary>
''' データベースにアクセスするクラス。
''' </summary>
Public Class DatabaseAccessor
  ''' データベースのトップディレクトリのパス  
  Private Shared _databasePath As String = "AddressDB"
  Public Shared Property DBPath As String
    Get
      Return _databasePath
    End Get
    
    Set (path As String)
      If Not Directory.Exists(path) Then Throw New ArgumentException("存在しないディレクトリです。")
      
      _databasePath = path
    End Set
  End Property
  
  ''' データベース内のデータファイルの拡張子
  Public Const FILE_EXTENTION = ".txt"
  
  ''' <summary>
  ''' ファイルにアクセスし、１行ごとにコールバック関数を呼び出し、読み込んだテキストを渡す。
  ''' </summary>
  Public Shared Sub Read(param As DatabaseAccessParameter, addrType As AddressType, callback As Action(Of String))
    ' ファイル名が正規表現になっているかどうかで読み込み方法を替える
    If Regex.IsMatch(param.FileNameRegexp, "^[^.]+$") Then
      AccessByText(param, addrType, callback)
    Else
      AccessByRegexp(param, addrType, callback)
    End If
  End Sub
  
  ''' <summary>
  ''' ファイル名を正規表現としてファイルアクセスする。
  ''' あいまいなファイル名や一度に複数のファイルを読み込めるがアクセス速度は遅い。
  ''' </summary>
  Private Shared Sub AccessByRegexp(param As DatabaseAccessParameter, addrType As AddressType, f As Action(Of String))
    Dim dir As String = DatabaseDir(param, addrType)
    Dim fileNameRegexp As String = param.FileNameRegexp()
    
    Directory.GetFiles(dir).
      Where(Function(filepath) Regex.IsMatch(Path.GetFileNameWithoutExtension(filepath), fileNameRegexp)).
      ForEach(
        Sub(filepath)
          Dim file As New TextFile(filepath, System.Text.Encoding.UTF8)
          If file.Exists Then
            file.Read(f)
          End IF
        End Sub)
  End Sub
  
  ''' <summary>
  ''' 一意のファイルにアクセスする。
  ''' </summary>
  Private Shared Sub AccessByText(param As DatabaseAccessParameter, addrType As AddressType, f As Action(Of String))
    Dim path As String = IO.Path.Combine(DatabaseDir(param, addrType), param.FileNameRegexp() & FILE_EXTENTION)
    
    Dim file As New TextFile(path, System.Text.Encoding.UTF8)
    If file.Exists Then
      file.Read(f)
    End If
  End Sub
  
  ''' <summary>
  ''' データベースのディレクトリを指し示すパスを取得する。
  ''' </summary>
  Private Shared Function DatabaseDir(param As DatabaseAccessParameter, addrType As AddressType) As String
    Dim addrTypePath As String = Path.Combine(_databasePath, addrType.ToString)
    Return Path.Combine(addrTypePath, param.SearchingAddressItem.ToString) 
  End Function
  
End Class

