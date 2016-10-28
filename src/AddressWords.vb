'
' 日付: 2016/10/09
'
Imports Common.Extensions
Imports Common.Text

Public Structure AddressWords
  Private ReadOnly _zipcode As Common.Text.MatchingText
  Private ReadOnly _prefecture As Common.Text.MatchingText
  Private ReadOnly _city As Common.Text.MatchingText
  Private ReadOnly _townArea As Common.Text.MatchingText
  Private ReadOnly _office As Common.Text.MatchingText
  
  Public Sub New(
    zipcode As String,
    matchingModeOfZipcode As MatchingMode,
    prefecture As String,
    matchingModeOfPrefecture As MatchingMode,
    city As String,
    matchingModeOfCity As MatchingMode,
    townArea As String,
    matchingModeOfTownArea As MatchingMode,
    office As String,
    matchingModeOfOffice As MatchingMode)
    If zipcode    Is Nothing Then Throw New NullReferenceException("zipcode is null")
    If prefecture Is Nothing Then Throw New NullReferenceException("prefecture is null")
    If city       Is Nothing Then Throw New NullReferenceException("city is null")
    If townArea   Is Nothing Then Throw New NullReferenceException("townArea is null")
    If office     Is Nothing Then Throw New NullReferenceException("office is null")
    
    Me._zipcode    = New MatchingText(zipcode,    matchingModeOfZipcode)
    Me._prefecture = New MatchingText(prefecture, matchingModeOfPrefecture)
    Me._city       = New MatchingText(city,       matchingModeOfCity)
    Me._townArea   = New MatchingText(townArea,   matchingModeOfTownArea)
    Me._office     = New MatchingText(office,     matchingModeOfOffice)
  End Sub
  
  Public Sub New(zipcode As String, prefecture As String, city As String, townArea As String)
    Me.New(
      zipcode,      MatchingMode.Forward,
      prefecture,   MatchingMode.Forward,
      city,         MatchingMode.Forward,
      townArea,     MatchingMode.Forward,
      String.Empty, MatchingMode.Forward)
  End Sub
  
  Public Shared Function CreateFromCsvOfFullAddressAndOffice(csv As String) As AddressWords
    Dim fields As String() = csv.Split(","c)
    Return New AddressWords(fields(0), fields(1), fields(2), fields(3), fields(4))
  End Function
  
  Public Shared Function CreateFromCsvOfAddressTextAndOffice(csv As String) As AddressWords
    Dim fields As String() = csv.Split(","c)
    Return New AddressWords(fields(0), fields(1), "", "", fields(4))
  End Function
  
  Public Shared Function CreateFromCsvOfFullAddress(csv As String) As AddressWords
    Dim fields As String() = csv.Split(","c)
    Return New AddressWords(fields(0), fields(1), fields(2), fields(3))
  End Function
  
  Public Shared Function CreateFromCsvOfZipAndCity(csv As String) As AddressWords
    Dim fields As String() = csv.Split(","c)
    Return New AddressWords(fields(0), "", fields(1), "")
  End Function
  
  Public Shared Function CreateFromCsvOfZipAndTownArea(csv As String) As AddressWords
    Dim fields As String() = csv.Split(","c)
    Return New AddressWords(fields(0), "", "", fields(1))
  End Function
  
  Public Shared Function CreateFromCsvOfZipAndOffice(csv As String) As AddressWords
    Dim fields As String() = csv.Split(","c)
    Return New AddressWords(fields(0), "", "", "", fields(1))
  End Function
  
  Public Function Zipcode As List(Of String)
    Return _zipcode.Words
  End Function
  
  Public Function MatchingModeOfZipcode As MatchingMode
    Return _zipcode.Mode
  End Function
  
  Public Function ZipcodeHeader As List(Of String)
    Dim l = _
      _zipcode.Words.
        Filter(Function(w) w.Length > 0).
        Convert(
          Function(w)
            If w.Length >= 3 Then
              Return w.Substring(0, 3)
            Else
              Return w
            End If
          End Function)
    Return DirectCast(l, List(Of String))
  End Function
  
  Public Function Prefecture As List(Of String)
    Return _prefecture.Words
  End Function
  
  Public Function MatchingModeOfPrefecture As MatchingMode
    Return _prefecture.Mode  
  End Function
  
  Public Function City As List(Of String)
    Return _city.Words
  End Function
  
  Public Function MatchingModeOfCity As MatchingMode
    Return _city.Mode
  End Function
  
  Public Function TownArea As List(Of String)
    Return _townArea.Words
  End Function
  
  Public Function MatchingModeOfTownArea As MatchingMode
    Return _townArea.Mode
  End Function
  
  Public Function Office As List(Of String)
    Return _office.Words
  End Function
  
  Public Function MatchingModeOfOffice As MatchingMode
    Return _office.Mode
  End Function
  
  Public Function FullName() As String
    Return Me._Prefecture.Word & Me._City.Word & Me._TownArea.Word
  End Function
  
  Public Function Matching(words As AddressWords) As Boolean
    Return _
      Me._zipcode.Matching(words._zipcode, True)       AndAlso
      Me._prefecture.Matching(words._prefecture, True) AndAlso
      Me._city.Matching(words._city, True)             AndAlso
      Me._townArea.Matching(words._townArea, True)     AndAlso
      Me._office.Matching(words._office, True)
  End Function
  
  Public Overrides Function ToString() As String
    Return _zipcode.Word & " " & FullName
  End Function
End Structure
