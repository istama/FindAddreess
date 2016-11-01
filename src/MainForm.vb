'
' 日付: 2016/10/08
' 
Imports System.IO
Imports System.Data
'Imports System.Linq
Imports System.Threading.Tasks
Imports System.Collections.Concurrent
Imports Common.Text
Imports Common.Extensions
Imports System.Threading

''' <summary>
''' メインフォーム。
''' </summary>
Public Partial Class MainForm
  Private Const COLUMN_NAME_ZIP As String     = "郵便番号"
  Private Const COLUMN_NAME_ADDRESS As String = "住所"
  Private Const COLUMN_NAME_OFFICE As String  = "事業所"
  
  ''' このアプリケーションのバージョン番号	
  Private version As Version
  ''' このアプリケーションの設定
  Private properties As MyProperties
  
  ''' 住所検索オブジェクト
  Private searcher As Searcher
  ''' 検索中かどうか
  Private running As Boolean
  
  ''' 検索した住所情報を格納するテーブル
'  Private addressDataTable As DataTable
  
  Public Sub New()
    Me.InitializeComponent()
  End Sub
  
  Sub MainFormLoad(sender As Object, e As EventArgs)
    
		' バージョン情報取得
		version = My.Application.Info.Version
		' アプリケーションプロパティ生成
		properties = New MyProperties("./application.properties")
		' アップデート
		AutoUpdate()    
		
		Me.running = False
    
    Try
      InitDatabaseAccessor()
      InitAllCbox()
      LoadTitleBar()
      
      ExchangeAcceptButton()
    Catch ex As Exception
      MsgBox.ShowError(ex)
    End Try
  End Sub  
  
	''' <summary>
	''' 最新バージョンファイルがある場合、アップデートスクリプトを実行する。
	''' </summary>
	Private Sub AutoUpdate()
		' アップデートを有効にしているかどうかチェックする
		If Me.properties.IsUpdateRunnable Then
		  ' バージョンファイルのパスと更新スクリプトのパスが設定されているかチェックする
			Dim versionFilePath As String = String.Empty
			Dim exeFilesDir     As String = String.Empty
			If Me.properties.TryGetVersionFilePath(versionFilePath) AndAlso _
			   Me.properties.TryGetLatestExeFilesDir(exeFilesDir) Then
				' バージョンが更新されたならスクリプトを実行する。
				Dim update As New MyUpdate(versionFilePath, exeFilesDir)
				If update.existsUpdateVersion(Me.version) Then
				  MsgBox.Show(
						"最新バージョンにアップデートします。" & Environment.NewLine &
						"再起動してください。",
						"更新")
					' スクリプトを作成
					update.CreateUpdateBatch()
					Me.Close()
					' スクリプトを実行
					update.RunUpdateBatch(Me.version)
				End If
			End If
		End If
	End Sub
	
	''' <summary>
	''' データベースへアクセスするクラスを初期化する。
	''' </summary>
  Sub InitDatabaseAccessor()
    Dim path As String = ""
    If Me.properties.TryGetAddressDBDir(path) Then
      DatabaseAccessor.DBPath = path
    End If
  End Sub
  
  ''' <summary>
  ''' コンボボックスの初期化する。
  ''' </summary>
  Sub InitAllCbox()
    InitCboxOfMatchingMode(Me.cboxMatchingModeOfCity)
    InitCboxOfMatchingMode(Me.cboxMatchingModeOfTownArea)
    InitCboxOfMatchingMode(Me.cboxMatchingModeOfAddress)
    InitCboxOfMatchingMode(Me.cboxMatchingModeOfPostOffice)
  End Sub
  
  ''' <summary>
  ''' マッチング方法を選択するコンボボックスを初期化する。
  ''' </summary>
  Sub InitCboxOfMatchingMode(cbox As ComboBox)
    Dim DISPLAY As String = "display"
    Dim VALUE   As String = "value"
    
    Dim dataTable As New DataTable
    dataTable.Columns.Add(DISPLAY, GetType(String))
    dataTable.Columns.Add(VALUE, GetType(MatchingMode))
    
    For Each type As MatchingMode In MatchingMode.GetValues(GetType(MatchingMode))
      Dim row As DataRow = dataTable.NewRow
      row(DISPLAY) = GetComboBoxItemTitleOfMatchingMode(type)
      row(VALUE)   = type	' 選択した要素から取得できる値
      dataTable.Rows.Add(row)        
    Next
    
    dataTable.AcceptChanges
    cbox.DataSource    = dataTable
    cbox.DisplayMember = DISPLAY
    cbox.ValueMember   = VALUE    
  End Sub
  
  ''' <summary>
  ''' マッチング方法に対応するタイトルを取得する。
  ''' </summary>
  Private Function GetComboBoxItemTitleOfMatchingMode(mode As MatchingMode) As String
    If mode = MatchingMode.Forward Then
      Return "前方一致"
    ElseIf mode = MatchingMode.Perfection
      Return "完全一致"
    ElseIf mode = MatchingMode.Part
      Return "部分一致"
    Else
      Return "後方一致"
    End If
  End Function
  
  ''' <summary>
  ''' Enterキーを押したときに押されるボタンを切り替える。
  ''' </summary>
  Private Sub ExchangeAcceptButton()
    Dim selectedTabPage As TabPage = Me.tabControl1.SelectedTab
    If selectedTabPage.Text = "住所" Then
      Me.AcceptButton = Me.btnSearch
    ElseIf selectedTabPage.Text = "郵便局"
      Me.AcceptButton = Me.btnSearchForPostOffice
    End If
  End Sub
  
  ''' <summary>
  ''' タブのページが切り替わったときのイベントハンドラ
  ''' </summary>
  Sub tabControlIndexChanged(sender As Object, e As EventArgs)
    ExchangeAcceptButton()
  End Sub
  
  ''' 郵便番号が入力されたときのイベントハンドラ
  Sub TboxZipcodeKeyPress(sender As Object, e As KeyPressEventArgs)
    ' ? バックスペース　数値 以外の入力は許可しない
    e.Handled = _
      e.KeyChar <> "?"c AndAlso
      e.KeyChar <> Chr(Keys.Back) AndAlso
      (e.KeyChar < "0"c OrElse e.KeyChar > "9"c)
  End Sub
  
  ''' 都道府県、市町村、町域が入力されたときのイベントハンドラ
  Sub TboxKeyPress(sender As Object, e As KeyPressEventArgs)
    ' ? 以外の正規表現で使用されそうな文字列の入力を許可しない
    e.Handled = _
      e.KeyChar = "."c OrElse e.KeyChar = "("c OrElse e.KeyChar = ")"c OrElse e.KeyChar = "$"c OrElse
      e.KeyChar = "%"c OrElse e.KeyChar = "&"c OrElse e.KeyChar = "["c OrElse e.KeyChar = "]"c OrElse
      e.KeyChar = "|"c OrElse e.KeyChar = "*"c OrElse e.KeyChar = "+"c OrElse e.KeyChar = "-"c OrElse
      e.KeyChar = "@"c OrElse e.KeyChar = "#"c OrElse e.KeyChar = "{"c OrElse e.KeyChar = "}"c OrElse
      e.KeyChar = "'"c OrElse e.KeyChar = "`"c OrElse e.KeyChar = "\"c
  End Sub
  
  ''' <summary>
  ''' 住所検索ボタンが押されたときのイベントハンドラ
  ''' </summary>
  Sub BtnSearchClick(sender As Object, e As EventArgs)
    Search(Me.btnSearch, CreateSearchingWords(), AddressType.Address)
  End Sub
  
  ''' <summary>
  ''' 郵便局検索ボタンが押されたときのイベントハンドラ
  ''' </summary>
  Sub BtnSearchForPostOfficeClick(sender As Object, e As EventArgs)
    Search(Me.btnSearchForPostOffice, CreateSearchingWordsForPostOffice(), AddressType.PostOffice)
  End Sub
  
  ''' <summary>
  ''' 住所検索オブジェクトを起動する。
  ''' </summary>
  Private Sub Search(button As Button, addrWords As AddressWords, addrType As AddressType)
    Try
      If Not Me.running Then
        Me.running = True
        SetTextToControl(button, "停止")
        SetTextToControl(Me.lblMessage, "検索中です...")
        AsyncSearch(
          addrWords,
          addrType,
          Sub(tasks) 
            SetTextToControl(button, "検索")
            SetTextToControl(Me.lblMessage, "検索終了")
            SetTextToControl(Me.lblFoundAddr, "")
            Me.running = False
          End Sub)
      Else
        Halt()
      End If
    Catch ex As Exception
      MsgBox.ShowError(ex)
      Me.running = False
      SetTextToControl(button, "検索")
    End Try        
  End Sub
  
  ''' <summary>
  ''' 非同期で住所検索する。
  ''' </summary>
  Private Sub AsyncSearch(addrWords As AddressWords, addrType As AddressType, endCallback As Action(Of ConcurrentQueue(Of Task)))
    ' DataGridViewの表示を初期化
    Dim addrTable As DataTable = InitDataGridViewTable(addrType)
    
    Me.searcher = New Searcher(addrWords, addrType)
    
    ' 検索中に例外が発生したときハンドラを設定
    searcher.ErrorHandler =
      Sub(ex As Exception)
        If Me.InvokeRequired Then
          Me.Invoke(
            New SetExceptionDelegate(AddressOf ThreadingErrorHandler),
            New Object() { ex })          
        Else
          ThreadingErrorHandler(ex)
        End If
      End Sub
    
    ' 検索開始
    searcher.Run(
      Sub(addr) ShowAddress(addr),
      Sub(task) endCallback(task))
  End Sub
  
  ''' <summary>
  ''' 検索を途中で停止させる。
  ''' </summary>
  Private Sub Halt()
    Me.searcher.Halt()
  End Sub
  
  ''' <summary>
  ''' 検索ワードオブジェクトを生成する。
  ''' </summary>
  Private Function CreateSearchingWords() As AddressWords
    Dim zip  As String = Me.tboxZipcode.Text.Replace("?"c, ".").Replace("？", "."c)
    Dim pre  As String = Me.tboxPrefecture.Text.Replace("?"c, ".").Replace("？", "."c)
    Dim city As String = Me.tboxCity.Text.Replace("?"c, ".").Replace("？", "."c)
    Dim town As String = Me.tboxTownArea.Text.Replace("?"c, ".").Replace("？", "."c)
    
    If zip = String.Empty AndAlso pre = String.Empty AndAlso city = String.Empty AndAlso town = String.Empty Then
      Throw New ArgumentException("検索ワードが入力されていません。")
    End If
    
    Return _
      New AddressWords(
        zip,  MatchingMode.Forward,
        pre,  MatchingMode.Forward,
        city, DirectCast(Me.cboxMatchingModeOfCity.SelectedValue, MatchingMode),
        town, DirectCast(Me.cboxMatchingModeOfTownArea.SelectedValue, MatchingMode))
  End Function
  
  ''' <summary>
  ''' 郵便局検索ワードオブジェクトを生成する。
  ''' </summary>
  Private Function CreateSearchingWordsForPostOffice() As AddressWords
    Dim zip    As String = Me.tboxZipcodeOfPostOffice.Text.Replace("?"c, ".").Replace("？", "."c)
    Dim pre    As String = Me.tboxPrefectureOfPostOffice.Text.Replace("?"c, ".").Replace("？", "."c)
    Dim office As String = Me.tboxPostOffice.Text.Replace("?"c, ".").Replace("？", "."c)
    
    If zip = String.Empty AndAlso pre = String.Empty AndAlso office = String.Empty Then
      Throw New ArgumentException("検索ワードが入力されていません。")
    End If
    
    Return _
      New AddressWords(
        zip,          MatchingMode.Forward,
        String.Empty, MatchingMode.Forward,
        String.Empty, MatchingMode.Forward,
        pre,          DirectCast(Me.cboxMatchingModeOfAddress.SelectedValue, MatchingMode),
        office,       DirectCast(Me.cboxMatchingModeOfPostOffice.SelectedValue, MatchingMode))
  End Function
  
  ''' <summary>
  ''' DataGridViewの表示テーブルを初期化する。
  ''' </summary>
  Private Function InitDataGridViewTable(addrType As AddressType) As DataTable
    Dim table As DataTable
    ' 普通の住所を検索する場合
    If addrType = AddressType.Address Then
      table = CreateAddressTable()
      Me.dataGridView1.DataSource = table
      ' 列の表示幅と表示順を設定
      Me.dataGridView1.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
      Me.dataGridView1.Columns(0).DisplayIndex = 0
      Me.dataGridView1.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
      Me.dataGridView1.Columns(1).DisplayIndex = 1
    ' 郵便局を検索する場合
    Else
      table = CreateAddressAndOfficeTable()
      Me.dataGridView1.DataSource = table
      ' 列の表示幅と表示順を設定
      Me.dataGridView1.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
      Me.dataGridView1.Columns(0).DisplayIndex = 0
      Me.dataGridView1.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
      Me.dataGridView1.Columns(1).DisplayIndex = 1
      Me.dataGridView1.Columns(2).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
      Me.dataGridView1.Columns(2).DisplayIndex = 2
    End If
    
    Return table    
  End Function
  
  ''' <summary>
  ''' 住所を格納するテーブルを作成。
  ''' </summary>
  Private Function CreateAddressTable() As DataTable
    Dim table As New DataTable
    table.Columns.Add(CreateTableColumn(COLUMN_NAME_ZIP))
    table.Columns.Add(CreateTableColumn(COLUMN_NAME_ADDRESS))
    
    Return table
  End Function
  
  ''' <summary>
  ''' 郵便局の住所を格納するテーブルを作成。
  ''' </summary>
  Private Function CreateAddressAndOfficeTable() As DataTable
    Dim table As New DataTable
    table.Columns.Add(CreateTableColumn(COLUMN_NAME_OFFICE))
    table.Columns.Add(CreateTableColumn(COLUMN_NAME_ZIP))
    table.Columns.Add(CreateTableColumn(COLUMN_NAME_ADDRESS))
    
    Return table
  End Function
  
  ''' <summary>
  ''' テーブルの列を生成。
  ''' </summary>
  Private Function CreateTableColumn(colName As String) As DataColumn
    Dim col As New DataColumn
    col.DataType = GetType(String)
    col.ColumnName = colName
    col.AutoIncrement = False
    
    Return col
  End Function
  
  ''' <summary>
  ''' テーブルにアドレスを格納し、表示を更新する。
  ''' </summary>
  ''' <param name="table"></param>
  ''' <param name="addr"></param>
  Private Sub ShowAddress(addr As AddressWords)
    If Me.dataGridView1.InvokeRequired Then
      Me.dataGridView1.Invoke(
        New SetAddrDelegate(AddressOf ShowAddress),
        New Object() { addr })
    Else
      Dim table = DirectCast(Me.dataGridView1.DataSource, DataTable)
      
      Dim row As DataRow = table.NewRow
      If table.Columns.Contains(COLUMN_NAME_OFFICE) Then
        row(COLUMN_NAME_OFFICE) = addr.Office(0)
      End If
      row(COLUMN_NAME_ZIP)     = addr.Zipcode(0)
      row(COLUMN_NAME_ADDRESS) = addr.FullName
      table.Rows.Add(row)
      
      Me.lblFoundAddr.Text   = addr.FullName
      Me.lblResultCount.Text = table.Rows.Count.ToString & " 件"
      Me.dataGridView1.Update
    End If
  End Sub
  
  ''' <summary>
  ''' DataGridViewのテキストを編集しようとしたときに発生するイベントハンドラ
  ''' </summary>
  Private Sub dataGridView1_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) 
    ' 編集しようとしてるセルのTextBoxを取得し、ReadOnlyにして編集できないようにする。
    ' DataGridView自体をReadOnlyにするとセルのテキストを部分的に選択することができないので、
    ' このような回りくどい方法を行っている。
    Dim tbox = DirectCast(e.Control, TextBox)
    tbox.ReadOnly = True
  End Sub
  
  Private Sub SetTextToControl(control As Control, msg As String)
    If control.InvokeRequired Then
      control.Invoke(
        New SetControlAndStringDelegate(AddressOf SetTextToControl),
        New Object() { control, msg })
    Else
      control.Text = msg
    End If    
  End Sub
  
  ''' <summary>
  ''' タイトルバーにアプリケーション名とバージョン番号をセットする。
  ''' </summary>
  Private Sub LoadTitleBar()
    Dim title As String = properties.GetValue(MyProperties.KEY_APPLICATION_TITLE).GetOrDefault("")
    Me.Text = title & " Ver." & version.ToString
  End Sub
	 
  Private Sub ThreadingErrorHandler(ex As Exception)
    MsgBox.ShowError(ex.Message)
  End Sub
  
  Private Delegate Sub SetAddrDelegate(addr As AddressWords)
  Private Delegate Sub SetControlAndStringDelegate(control As Control, str As String)
  Private Delegate Sub SetExceptionDelegate(ex As Exception)

End Class
