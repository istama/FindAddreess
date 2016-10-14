'
' 日付: 2016/10/10
'
Imports NUnit.Framework

Imports Common.Text.MatchingMode

<TestFixture> _
Public Class TestSearcher
  
  <Test> _
  Public Sub TestRun
    Dim addr1 As New AddressWords("", Forward, "", Forward, "多摩", Forward, "永山", Forward)
    Dim res1 As List(Of AddressWords) = Access(addr1)
    Assert.AreEqual(1, res1.Count)
    Assert.AreEqual("206", res1(0).ZipcodeHeader(0))
    Assert.AreEqual("東京都", res1(0).Prefecture(0))
    
    Dim addr2 As New AddressWords("", Forward, "", Forward, "多摩", Forward, "桜", Part)
    Dim res2 As List(Of AddressWords) = Access(addr2)
    Assert.AreEqual(1, res2.Count)
    Assert.AreEqual("206", res2(0).ZipcodeHeader(0))
    Assert.AreEqual("東京都", res2(0).Prefecture(0))
    Assert.AreEqual("桜ヶ丘", res2(0).TownArea(0))
    
    Dim addr3 As New AddressWords("", Forward, "..都", Forward, "多摩", Part, ".山", Perfection)
    Dim res3 As List(Of AddressWords) = Access(addr3)
    Assert.AreEqual(1, res3.Count)
    Assert.AreEqual("206", res3(0).ZipcodeHeader(0))
    Assert.AreEqual("永山", res3(0).TownArea(0))
  End Sub
  
  Private Function Access(addr As AddressWords) As List(Of AddressWords)
    Dim res As New List(Of AddressWords)
    Dim s As New Searcher(addr)
    s.Run(Sub(a) res.Add(a))
    s.Wait
    
    Return res
  End Function
End Class
