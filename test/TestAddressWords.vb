'
' SharpDevelopによって生成
' ユーザ: Blue
' 日付: 2016/10/10
' 時刻: 0:57
' 
' このテンプレートを変更する場合「ツール→オプション→コーディング→標準ヘッダの編集」
'
Imports NUnit.Framework

Imports Common.Text

<TestFixture> _
Public Class TestAddressWords
  
  <Test> _
  Public Sub TestCreateFromCsv
    Dim addr1 As AddressWords = AddressWords.CreateFromCsvOfFullAddress("1600001,東京都,新宿区,西新宿")
    Assert.AreEqual("1600001", addr1.Zipcode(0))
    Assert.AreEqual("東京都",  addr1.Prefecture(0))
    Assert.AreEqual("新宿区",  addr1.City(0))
    Assert.AreEqual("西新宿",  addr1.TownArea(0))
    
    Dim addr2 As AddressWords = AddressWords.CreateFromCsvOfZipAndCity("1600001,新宿区")
    Assert.AreEqual("1600001", addr2.Zipcode(0))
    Assert.AreEqual("",  　　　　addr2.Prefecture(0))
    Assert.AreEqual("新宿区",  addr2.City(0))
    Assert.AreEqual("",  　　　　addr2.TownArea(0))   
    
    Dim addr3 As AddressWords = AddressWords.CreateFromCsvOfZipAndTownArea("1600001,西新宿")
    Assert.AreEqual("1600001", addr3.Zipcode(0))
    Assert.AreEqual("",  　　　　addr3.Prefecture(0))
    Assert.AreEqual("",  　　　　addr3.City(0))
    Assert.AreEqual("西新宿",  addr3.TownArea(0))   
  End Sub
  
  <Test> _
  Public Sub TestProperty
    Dim addr1 As AddressWords = AddressWords.CreateFromCsvOfFullAddress("1600001 123 45,,新宿区 豊島区,西新宿")
    Assert.AreEqual("1600001", addr1.Zipcode(0))
    Assert.AreEqual("123",     addr1.Zipcode(1))
    Assert.AreEqual("45",      addr1.Zipcode(2))
    Assert.AreEqual("160",     addr1.ZipcodeHeader(0))
    Assert.AreEqual("123",     addr1.ZipcodeHeader(1))
    Assert.AreEqual("45",      addr1.ZipcodeHeader(2))
    Assert.AreEqual("",        addr1.Prefecture(0))
    Assert.AreEqual("新宿区",  addr1.City(0))
    Assert.AreEqual("豊島区",  addr1.City(1))
    Assert.AreEqual("西新宿",  addr1.TownArea(0))    
    
    Dim addr2 As AddressWords = AddressWords.CreateFromCsvOfAddressTextAndOffice("1000001,東京都千代田区,丸の内オフィス")
    Assert.AreEqual("1000001",       addr2.Zipcode(0))
    Assert.AreEqual("東京都千代田区", addr2.TownArea(0))
    Assert.AreEqual("丸の内オフィス",    addr2.Office(0))
    
    Dim addr3 As AddressWords = AddressWords.CreateFromCsvOfZipAndOffice("1600001,新宿ビル")
    Assert.AreEqual("1600001", addr3.Zipcode(0))
    Assert.AreEqual("新宿ビル", addr3.Office(0))
  End Sub
  
  <Test> _
  Public Sub TestMatching
    Dim words As New AddressWords(
      "160 161", MatchingMode.Forward,
      "",        MatchingMode.Forward,
      "新.",     MatchingMode.Perfection,
      "",        MatchingMode.Forward)
    
    Dim addr1 As AddressWords = AddressWords.CreateFromCsvOfFullAddress("160,,新宿,西新宿")
    Assert.True(words.Matching(addr1), "addr1 fail")
    
    Dim addr2 As AddressWords = AddressWords.CreateFromCsvOfFullAddress("161,東京都,新橋,")
    Assert.True(words.Matching(addr2), "addr2 fail")
    
    Dim addr3 As AddressWords = AddressWords.CreateFromCsvOfFullAddress("160,,,")
    Assert.True(words.Matching(addr3), "addr3 fail")
    
    Dim addr4 As AddressWords = AddressWords.CreateFromCsvOfFullAddress("161,,新小平,")
    Assert.False(words.Matching(addr4), "addr4 fail")
    
    Dim words2 As New AddressWords(
      "",     MatchingMode.Forward,
      "県",   MatchingMode.Backward,
      "",     MatchingMode.Perfection,
      "山川", MatchingMode.Part)
    
    Dim addr5 As AddressWords = AddressWords.CreateFromCsvOfFullAddress("200,神奈川県,箱根市,新山川町")
    Assert.True(words2.Matching(addr5), "addr5 fail")
    
    Dim addr6 As AddressWords = AddressWords.CreateFromCsvOfFullAddress("200,京都府,,新山川町")
    Assert.False(words2.Matching(addr6), "addr6 fail")
  End Sub
End Class
