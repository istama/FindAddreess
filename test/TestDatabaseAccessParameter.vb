'
' 日付: 2016/10/10
'
Imports NUnit.Framework

Imports Common.Text.MatchingMode

<TestFixture> _
Public Class TestDatabaseAccessParameter
  <Test> _
  Public Sub TestProperty
    ' 郵便番号がアクセス先になる
    Dim addr1 As AddressWords = AddressWords.CreateFromCsvOfFullAddress("160,東京都,新宿区,西新宿")
    Dim param1 As New DatabaseAccessParameter(addr1)
    Assert.AreEqual(AddressItem.Zipcode, param1.SearchingAddressItem)
    Assert.AreEqual("160", param1.FileNameRegexp)
    
    ' 町域がアクセス先になる
    Dim addr2 As AddressWords = AddressWords.CreateFromCsvOfFullAddress("16,東京都,新宿区,西.宿")
    Dim param2 As New DatabaseAccessParameter(addr2)
    Assert.AreEqual(AddressItem.TownArea, param2.SearchingAddressItem)
    Assert.AreEqual(AscW("西").ToString.Substring(0, 3), param2.FileNameRegexp) 
    
    ' 市町村がアクセス先になる
    Dim addr3 As New AddressWords("16", Forward, "東京都", Forward, "新宿.", Forward, "西新宿", Part)
    Dim param3 As New DatabaseAccessParameter(addr3)
    Assert.AreEqual(AddressItem.City, param3.SearchingAddressItem)
    Assert.AreEqual(AscW("新").ToString.Substring(0, 3), param3.FileNameRegexp) 
    
    ' 都道府県がアクセス先になる
    Dim addr4 As New AddressWords("16", Forward, ".京都", Forward, ".宿区", Forward, "西新宿", Part)
    Dim param4 As New DatabaseAccessParameter(addr4)
    Assert.AreEqual(AddressItem.Prefecture, param4.SearchingAddressItem)
    Assert.AreEqual("^.京都.*", param4.FileNameRegexp)
    
    ' 郵便番号がアクセス先になる
    Dim addr5 As New AddressWords(".6", Forward, "", Forward, "西宿区", Backward, ".新宿", Perfection)
    Dim param5 As New DatabaseAccessParameter(addr5)
    Assert.AreEqual(AddressItem.Zipcode, param5.SearchingAddressItem)
    Assert.AreEqual(".6.", param5.FileNameRegexp)
    
    ' 事業所がアクセス先になる
    Dim addr6 As AddressWords = AddressWords.CreateFromCsvOfFullAddressAndOffice("16,東京都,新宿区,西.宿,コクーンビル")
    Dim param6 As New DatabaseAccessParameter(addr6)
    Assert.AreEqual(AddressItem.Office, param6.SearchingAddressItem)
    Assert.AreEqual(AscW("コ").ToString.Substring(0, 3), param6.FileNameRegexp)
    
    ' 例外
    Try
      Dim addrEx As New AddressWords("...", Forward, "", Forward, "西宿区", Backward, ".新宿", Perfection)
      Dim paramEx As New DatabaseAccessParameter(addrEx)
      Assert.True(False)
    Catch ex As Exception
    End Try 
    
  End Sub
End Class
