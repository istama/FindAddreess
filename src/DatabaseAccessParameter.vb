'
' 日付: 2016/10/10
'
Imports System.Text.RegularExpressions
Imports Common.Text
Imports Common.Text.MatchingMode
Imports Common.Util

''' <summary>
''' ファイルにアクセスするための要素をまとめた構造体。
''' </summary>
Public Structure DatabaseAccessParameter
  Private _searchingAddressItem As AddressItem
  Private _fileRegexp As String
  
  Public Sub New(words As AddressWords)
    Me._searchingAddressItem = PrioritySearchingItem(words)
    Me._fileRegexp = FileNameRegexp(words, Me._searchingAddressItem)
  End Sub
  
  Public Function SearchingAddressItem As AddressItem
    Return Me._searchingAddressItem
  End Function
  
  Public Function FileNameRegexp As String
    Return Me._fileRegexp
  End Function  
  
  ''' <summary>
  ''' 検索ワードからアクセスする検索項目を判断し、その項目を返す。
  ''' </summary>
  Private Function PrioritySearchingItem(words As AddressWords) As AddressItem
    ''' TODO
    ''' 検索ワードをスペースで分割する機能は現段階では未実装とする
    ''' 
    
    If AllowToSearchByZipcode(words) Then
      Return AddressItem.Zipcode
    ElseIf AllowToSearchByTownArea(words)
      Return AddressItem.TownArea
    ElseIf AllowToSearchByCity(words)
      Return AddressItem.City
    ElseIf AllowToSearchByPrefecture(words)
      Return AddressItem.Prefecture
    ElseIf AllowToSearchByFuzzyZipcode(words)
      Return AddressItem.Zipcode
    Else
      Return AddressItem.Prefecture
'      Throw New ArgumentException(
'        "この検索条件では検索に非常に時間がかかってしまいます。" & vbCrLf &
'        "検索条件を指定しなおしてください。" & vbCrLf & vbCrLf &
'        "【ヒント】" & vbCrLf &
'        "・「先頭一致」か「完全一致」を使用する。" & vbCrLf &
'        "・検索ワードの一文字目に「？」を使用しない。" & vbCrLf &
'        "・上記の方法が無理なら都道府県を入力する。"
'        )
    End If
  End Function
  
  Private Function AllowToSearchByZipcode(words As AddressWords) As Boolean
    Dim word As String = words.Zipcode(0)
    Dim mm As MatchingMode = words.MatchingModeOfZipcode
    Return word.Length >= 3 AndAlso word.Chars(0) <> "." AndAlso (mm = Forward OrElse mm = Perfection)
  End Function
  
  Private Function AllowToSearchByTownArea(words As AddressWords) As Boolean
    Dim word As String = words.TownArea(0)
    Dim mm As MatchingMode = words.MatchingModeOfTownArea
    Return word.Length > 0  AndAlso word.Chars(0) <> "." AndAlso (mm = Forward OrElse mm = Perfection)
  End Function
  
  Private Function AllowToSearchByCity(words As AddressWords) As Boolean
    Dim word As String = words.City(0)
    Dim mm As MatchingMode = words.MatchingModeOfCity
    Return word.Length > 0  AndAlso word.Chars(0) <> "." AndAlso (mm = Forward OrElse mm = Perfection)
  End Function
   
  Private Function AllowToSearchByPrefecture(words As AddressWords) As Boolean
    Dim word As String = words.Prefecture(0)
    Dim mm As MatchingMode = words.MatchingModeOfPrefecture
    Return word.Length > 0
  End Function 
  
  Private Function AllowToSearchByFuzzyZipcode(words As AddressWords) As Boolean
    Dim word As String = words.Zipcode(0)
    Dim mm As MatchingMode = words.MatchingModeOfZipcode
    Return word.Length > 0 AndAlso Regex.IsMatch(word, "[0-9]")
  End Function
  
  ''' <summary>
  ''' 検索するファイル名の正規表現を取得する。
  ''' </summary>
  Private Function FileNameRegexp(words As AddressWords, searchingItem As AddressItem) As String
    If searchingItem = AddressItem.Zipcode Then
      Return FileNameRegexpOfZipcode(words)
    ElseIf searchingItem = AddressItem.Prefecture
      Return FileNameRegexpOfPrefecture(words)
    ElseIf searchingItem = AddressItem.City
      Return FileNameOfCity(words)
    Else
      Return FileNameOfTownArea(words)
    End If
  End Function
  
  Private Function FileNameRegexpOfZipcode(words As AddressWords) As String
    Dim word As String = words.ZipcodeHeader(0)
    Return word.PadRight(3, "."c)
  End Function
  
  Private Function FileNameRegexpOfPrefecture(words As AddressWords) As String
    Dim word As String = words.Prefecture(0)
    Return "^" & word & ".*"
  End Function
  
  Private Function FileNameOfCity(words As AddressWords) As String
    Dim word As String = words.City(0)
    Return TextUtils.ToCharCode(word, 3).ToString
  End Function
  
  Private Function FileNameOfTownArea(words As AddressWords) As String
    Dim word As String = words.TownArea(0)
    Return TextUtils.ToCharCode(word, 3).ToString
  End Function
End Structure
