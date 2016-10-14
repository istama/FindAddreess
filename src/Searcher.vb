'
' 日付: 2016/10/10
'
Imports System.Threading.Tasks
Imports System.Collections.Concurrent
Imports System.Linq

Public Class Searcher
  Private searchingWords As AddressWords
  
  Private tasks As ConcurrentQueue(Of Task)
  Private running As Boolean
  Private _errorHandler As Action(Of Exception)
  
  Public Sub New(words As AddressWords)
    Me.searchingWords = words
  End Sub
  
  Public Function IsRunning As Boolean
    Return Me.running
  End Function
  
  Public Sub Halt
    Me.running = False
  End Sub
  
  Public Sub Wait
    While IsRunning
      Threading.Thread.Sleep(100)
    End While
  End Sub
  
  Public Function Count As Integer
    Return tasks.Count
  End Function
  
  Public WriteOnly Property ErrorHandler As Action(Of Exception)
    Set (h As Action(Of Exception))
      Me._errorHandler = h      
    End Set
  End Property
  
  Public Sub Run(callback As Action(Of AddressWords))
    Run(callback, Nothing)
  End Sub
  
  Public Sub Run(callback As Action(Of AddressWords), finallyCallback As Action(Of ConcurrentQueue(Of Task)))
    If callback Is Nothing Then Throw New NullReferenceException("callback is null")
    
    Me.tasks   = New System.Collections.Concurrent.ConcurrentQueue(Of Task)
    Me.running = True
    Search(Me.searchingWords, callback)
    
    Task.Factory.StartNew(Sub() FinallyTask(finallyCallback))    
  End Sub
  
  Private Sub Search(words As AddressWords, callback As Action(Of AddressWords))
    If Not Me.running Then Return
    
    tasks.Enqueue(Task.Factory.StartNew(Sub() SearchTask(words, callback)))
    'SearchTask(words, callback)
  End Sub
  
  Private Sub SearchTask(words As AddressWords, callback As Action(Of AddressWords))
    If Not Me.running Then Return
    
    Dim foundZipList As New List(Of String)
    
    Try 
      Dim param As New DatabaseAccessParameter(words)
      DatabaseAccessor.Read(
        param,
        Sub(csv)
          ' 停止命令があった場合
          If Not Me.running Then Return
          ' 既に取得した郵便番号ならこの先の処理は行わないようにする
          Dim fields As String() = csv.Split(","c)
          If fields.Count > 0 AndAlso foundZipList.Contains(fields(0)) Then Return
          
          Try
            Dim addr As AddressWords = CreateAddressWords(csv, param.SearchingAddressItem)
            ' 取得した住所が検索ワードにかかるか判定する
            If Me.searchingWords.Matching(addr) Then
              ' 既に取得した住所を処理しないように、郵便番号をリストに格納しておく
              foundZipList.Add(fields(0))
              
              If param.SearchingAddressItem = AddressItem.Zipcode Then
                ' 郵便番号から取得した住所情報をフルの住所情報なので、コールバック関数に引き渡す
                SyncLock Me
                  callback(addr)
                End SyncLock
              Else
                Search(addr, callback)
              End If
            End If
          Catch ex As Exception
            Console.WriteLine(ex.ToString)          
          End Try
        End Sub)
    Catch ex As Exception
      If Me._errorHandler IsNot Nothing Then
        Me._errorHandler(ex)
      End If
    End Try
  End Sub
  
  Private Sub FinallyTask(callback As Action(Of ConcurrentQueue(Of Task)))
    While True
      Threading.Thread.Sleep(50)
      If Task.WaitAll(tasks.ToArray, 0) Then
        Me.running = False
        Exit While
      End If
    End While
    
    If callback IsNot Nothing Then
      callback(tasks)
    End If
  End Sub
  
  Private Function CreateAddressWords(csv As String, searchedAddressItem As AddressItem) As AddressWords
    If searchedAddressItem = AddressItem.Zipcode Then
      Return AddressWords.CreateFromCsvOfFullAddress(csv)
    ElseIf searchedAddressItem = AddressItem.Prefecture Then
      Return AddressWords.CreateFromCsvOfFullAddress(csv & ",,,")
    ElseIf searchedAddressItem = AddressItem.City
      Return AddressWords.CreateFromCsvOfZipAndCity(csv)
    Else
      Return AddressWords.CreateFromCsvOfZipAndTownArea(csv)
    End If
  End Function
End Class
