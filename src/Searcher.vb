'
' 日付: 2016/10/10
'
Imports System.Threading.Tasks
Imports System.Collections.Concurrent
Imports System.Linq
Imports Common.IO

''' <summary>
''' アドレスを検索するクラス。
''' </summary>
Public Class Searcher
  ''' 検索ワード  
  Private searchingWords As AddressWords
  ''' 検索する住所のタイプ
  Private addrType As AddressType
  
  ''' 検索時に起動するタスクのリスト
  Private tasks As ConcurrentQueue(Of Task)
  ''' 現在、検索中かどうか
  Private running As Boolean
  ''' 検索時に例外が発生した場合に実行するハンドラ
  Private _errorHandler As Action(Of Exception)
  
  Public Sub New(words As AddressWords, addrType As AddressType)
    Me.searchingWords = words
    Me.addrType       = addrType
    Log.SetFilePath("log.txt")
  End Sub
  
  ''' <summary>
  ''' 現在、検索中ならtrueを返す。
  ''' </summary>
  Public Function IsRunning As Boolean
    Return Me.running
  End Function
  
  ''' <summary>
  ''' 検索を停止する。
  ''' </summary>
  Public Sub Halt
    Me.running = False
  End Sub
  
  ''' <summary>
  ''' 検索が終了するまで待機する。
  ''' </summary>
  Public Sub Wait
    While IsRunning
      Threading.Thread.Sleep(100)
    End While
  End Sub
  
  ''' <summary>
  ''' 検索時に稼動したスレッドの数。
  ''' </summary>
  Public Function Count As Integer
    Return tasks.Count
  End Function
  
  ''' <summary>
  ''' 例外発生時に実行するハンドラをセットする。
  ''' </summary>
  Public WriteOnly Property ErrorHandler As Action(Of Exception)
    Set (h As Action(Of Exception))
      Me._errorHandler = h      
    End Set
  End Property
  
  ''' <summary>
  ''' 検索を実行する。
  ''' バックグラウンドで実行するため処理はすぐ戻る。
  ''' </summary>
  ''' <param name="callback">アドレスが検索ワードにヒットするたびに呼び出されるコールバック</param>
  Public Sub Run(callback As Action(Of AddressWords))
    Run(callback, Nothing)
  End Sub
  
  ''' <summary>
  ''' 検索を実行する。
  ''' バックグラウンドで実行するため処理はすぐ戻る。
  ''' </summary>
  ''' <param name="callback">アドレスが検索ワードにヒットするたびに呼び出されるコールバック</param>
  ''' <param name="finallyCallback">検索終了時に呼び出されるコールバック</param>
  Public Sub Run(callback As Action(Of AddressWords), finallyCallback As Action(Of ConcurrentQueue(Of Task)))
    If callback Is Nothing Then Throw New NullReferenceException("callback is null")
    
    ' 検索タスクを保持するリストを作成
    Me.tasks   = New System.Collections.Concurrent.ConcurrentQueue(Of Task)
    Me.running = True
    ' 検索を実行する
    Search(Me.searchingWords, callback)
    
    ' 検索終了時に実行されるタスクを実行
    DoEnd(finallyCallback)
  End Sub
  
  ''' <summary>
  ''' 非同期で住所を検索する。
  ''' </summary>
  Private Sub Search(words As AddressWords, callback As Action(Of AddressWords))
    If Not Me.running Then Return
    
    tasks.Enqueue(Task.Factory.StartNew(Sub() SearchTask(words, callback)))
    'SearchTask(words, callback)
  End Sub
  
  ''' <summary>
  ''' 住所を検索する。
  ''' </summary>
  Private Sub SearchTask(words As AddressWords, callback As Action(Of AddressWords))
    If Not Me.running Then Return
    
    ' 検索にヒットした郵便番号を保持しておくリスト
    ' １度ヒットした郵便番号を２度処理しないためのもの
    Dim foundZipList As New List(Of String)
    
    Try 
      ' 住所データベースを読み込む
      ' １行読まれるたびにコールバックが呼び出される
      Dim param As New DatabaseAccessParameter(words)
      DatabaseAccessor.Read(
        param,
        Me.addrType,
        Sub(csv)
          ' 停止命令があった場合、処理を終了する
          If Not Me.running Then Return
          
          ' 既に取得した郵便番号ならこの先の処理は行わないようにする
          ' ただし、同じ郵便番号の郵便局があるため、郵便番号による検索はこれに含めない
          Dim fields As String() = csv.Split(","c)
          If _
            param.SearchingAddressItem <> AddressItem.Zipcode AndAlso
            fields.Count > 0                                   AndAlso
            foundZipList.Contains(fields(0)) Then
            Return
          End If
          
          Try
            ' 読み込んだCSVから住所ワードオブジェクトを作成
            Dim addr As AddressWords = CreateAddressWords(csv, Me.addrType, param.SearchingAddressItem)
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
                ' ヒットした住所ワードを元により詳細な住所情報を求めて検索する
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
  
  ''' <summary>
  '''検索終了時にタスクを実行する。
  ''' </summary>
  Private Sub DoEnd(callback As Action(Of ConcurrentQueue(Of Task)))
    Task.Factory.StartNew(
      Sub()
        ' すべてのタスクが終了するまで待機
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
      End Sub)
  End Sub
  
  ''' <summary>
  ''' CSVから住所ワードオブジェクトを作成する。
  ''' </summary>
  ''' <param name="csv">CSVテキスト</param>
  ''' <param name="addrType">このCSVテキストを得るために検索した住所タイプ</param>
  ''' <param name="searchedAddressItem">このCSVテキストを得るために検索した住所項目</param>
  ''' <returns></returns>
  Private Function CreateAddressWords(csv As String, addrType As AddressType, searchedAddressItem As AddressItem) As AddressWords
    If searchedAddressItem = AddressItem.Zipcode Then
      If addrType = AddressType.Address Then
        Return AddressWords.CreateFromCsvOfFullAddress(csv)
      Else
        Return AddressWords.CreateFromCsvOfAddressTextAndOffice(csv)        
      End If
    ElseIf searchedAddressItem = AddressItem.Prefecture Then
      Return AddressWords.CreateFromCsvOfFullAddress(csv & ",,,")
    ElseIf searchedAddressItem = AddressItem.City
      Return AddressWords.CreateFromCsvOfZipAndCity(csv)
    ElseIf searchedAddressItem = AddressItem.TownArea
      Return AddressWords.CreateFromCsvOfZipAndTownArea(csv)
    Else
      Return AddressWords.CreateFromCsvOfZipAndOffice(csv)
    End If
  End Function
End Class
