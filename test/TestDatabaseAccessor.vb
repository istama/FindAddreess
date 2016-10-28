'
' SharpDevelopによって生成
' ユーザ: Blue
' 日付: 2016/10/10
' 時刻: 18:40
' 
' このテンプレートを変更する場合「ツール→オプション→コーディング→標準ヘッダの編集」
'
Imports NUnit.Framework

<TestFixture> _
Public Class TestDatabaseAccessor
  <Test> _
  Public Sub TestRead
    ' 郵便番号がアクセス先になる
    Dim res1 As List(Of String) = Access("161,東京都,新宿区,西新宿")
    Assert.True(res1.Contains("1610034,東京都,新宿区,上落合"))
    Assert.True(res1.Contains("1610031,東京都,新宿区,西落合"))
    Assert.AreEqual(5, res1.Count)
    
    ' 町域がアクセス先になる
    Dim res2 As List(Of String) = Access("08,,,鼈奴")
    Assert.True(res2.Contains("089,鼈奴"))
    Assert.True(res2.Contains("879,鼎"))
    Assert.AreEqual(24, res2.Count)
    
    Dim res3 As List(Of String) = Access("87,,,鼎")
    Assert.True(res3.Contains("089,鼈奴"))
    Assert.True(res3.Contains("879,鼎"))
    Assert.AreEqual(24, res3.Count)
    
    ' 市町村がアクセス先になる
    Dim res4 As List(Of String) = Access("52,,彦根市,")
    Assert.True(res4.Contains("100,御蔵島村"))
    Assert.True(res4.Contains("773,徳島市"))
    Assert.AreEqual(13, res4.Count)
    
    ' 都道府県がアクセス先になる
    Dim res5 As List(Of String) = Access(",大分県,,")
    Assert.True(res5.Contains("870"), "not exists 870")
    Assert.True(res5.Contains("873"), "not exists 873")
    Assert.AreEqual(11, res5.Count)    
    
    ' 複数の都道府県にアクセスできる
    Dim res6 As List(Of String) = Access(",山.,,")
    Assert.True(res6.Contains("990"), "not exists 990")
    Assert.True(res6.Contains("744"), "not exists 744")
    Assert.True(res6.Contains("406"), "not exists 406")
    Assert.AreEqual(39, res6.Count) 
    
    ' 複数の郵便番号にアクセスできる
    Dim res7 As List(Of String) = Access(".09,,,")
    Assert.AreEqual(1391, res7.Count) 
  End Sub
  
  Private Function Access(csv As String) As List(Of String)
    Dim addr As AddressWords = AddressWords.CreateFromCsvOfFullAddress(csv)
    Dim param As New DatabaseAccessParameter(addr)
    Dim res As New List(Of String)
    DatabaseAccessor.Read(param, AddressType.Address, Sub(txt) res.Add(txt))
    
    Return res
  End Function
End Class
