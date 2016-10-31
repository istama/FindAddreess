'
' 日付: 2016/10/08
' 
Imports System.IO
Imports System.Data
'Imports System.Linq
Imports Common.Text
Imports Common.Extensions
Imports System.Threading

Public Partial Class MainForm
  Private Const COLUMN_NAME_ZIP As String     = "郵便番号"
  Private Const COLUMN_NAME_ADDRESS As String = "住所"
  
  ''' このアプリケーションのバージョン番号	
  Private version As Version
  ''' このアプリケーションの設定
  Private properties As MyProperties
  
  ''' 住所検索オブジェクト
  Private searcher As Searcher
  ''' 検索中かどうか
  Private running As Boolean
  
  ''' 検索した住所情報を格納するテーブル
  Private addressDataTable As DataTable
  
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
      
      Me.AcceptButton = Me.btnSearch
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
  
  
  Sub BtnSearchClick(sender As Object, e As EventArgs)
    Try
      If Not Me.running Then
        Me.running = True
        SetTextToControl(Me.btnSearch, "停止")
        SetTextToControl(Me.lblMessage, "検索中です...")
        Search(CreateAddressWords(), AddressType.Address)
      Else
        Halt()
      End If
    Catch ex As Exception
      MsgBox.ShowError(ex)
      Me.running = False
      SetTextToControl(Me.btnSearch, "検索")
    End Try
  End Sub
  
  Sub BtnSearchForPostOfficeClick(sender As Object, e As EventArgs)
    Try
      If Not Me.running Then
        Me.running = True
        SetTextToControl(Me.btnSearchForPostOffice, "停止")
        SetTextToControl(Me.lblMessage, "検索中です...")
        Search(CreatePostOfficeAddressWords(), AddressType.PostOffice)
      Else
        Halt()
      End If
    Catch ex As Exception
      MsgBox.ShowError(ex)
      Me.running = False
      SetTextToControl(Me.btnSearchForPostOffice, "検索")
    End Try    
  End Sub
  
  Private Sub Search(addrWords As AddressWords, addrType As AddressType)
    ClearAddressView()
    
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
      Sub(addr)
        AddAddressToTable(addr)
      End Sub,
      Sub(tasks)
        UpdateAddressView()
        SetTextToControl(Me.btnSearch, "検索")
        SetTextToControl(Me.btnSearchForPostOffice, "検索")
        SetTextToControl(Me.lblMessage, "検索終了")
        SetTextToControl(Me.lblFoundAddr, "")
        Me.running = False
      End Sub)
  End Sub
  
  Private Sub Halt()
    Me.searcher.Halt()
  End Sub
    
  Private Function CreateAddressWords() As AddressWords
    Dim zip As String  = Me.tboxZipcode.Text.Replace("?"c, ".").Replace("？", "."c)
    Dim pre As String  = Me.tboxPrefecture.Text.Replace("?"c, ".").Replace("？", "."c)
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
  
  Private Function CreatePostOfficeAddressWords() As AddressWords
    Dim zip As String = Me.tboxZipcodeOfPostOffice.Text.Replace("?"c, ".").Replace("？", "."c)
    Dim pre As String = Me.tboxPrefectureOfPostOffice.Text.Replace("?"c, ".").Replace("？", "."c)
    Dim office As String = Me.tboxPostOffice.Text.Replace("?"c, ".").Replace("？", "."c)
    
    If zip = String.Empty AndAlso pre = String.Empty AndAlso office = String.Empty Then
      Throw New ArgumentException("検索ワードが入力されていません。")
    End If
    
    Return _
      New AddressWords(
        zip,          MatchingMode.Forward,
        String.Empty, MatchingMode.Forward,
        String.Empty, MatchingMode.Forward,
        pre,          MatchingMode.Forward,
        office,       DirectCast(Me.cboxMatchingModeOfPostOffice.SelectedValue, MatchingMode))
  End Function
  

  

  
  Private Sub SetLblFoundAddr(msg As String)
    If Me.lblFoundAddr.InvokeRequired Then
      Me.lblFoundAddr.Invoke(
        New SetStringDelegate(AddressOf SetLblFoundAddr),
        New Object() { msg })
    Else
      Me.lblFoundAddr.Text = msg
    End If
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
  
  Private Sub ClearAddressView()
    If Me.dataGridView1.InvokeRequired Then
      Me.dataGridView1.Invoke(
        New NoArgumentsDelegate(AddressOf ClearAddressView))
    Else
      Me.addressDataTable = CreateAddressTable()
      Me.dataGridView1.DataSource = Me.addressDataTable
      Me.dataGridView1.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
      Me.dataGridView1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders)
    End If
  End Sub
  
  ''' <summary>
  ''' 検索するアドレスの種類を取得する。
  ''' </summary>
  Private Function GetSearchAddressType() As AddressType
    Dim page As Integer = Me.tabControl1.SelectedIndex
    If page = 0 Then
      Return AddressType.Address
    ElseIf page = 1
      Return AddressType.PostOffice
    Else
      Throw New InvalidOperationException("selected invalid tab page")
    End If
  End Function
  
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

  Private Function CreateAddressTable() As DataTable
    Dim table As New DataTable
    table.Columns.Add(CreateAddressTableColumn(COLUMN_NAME_ZIP))
    table.Columns.Add(CreateAddressTableColumn(COLUMN_NAME_ADDRESS))
    
    Return table
  End Function
  
  Private Function CreateAddressTableColumn(colName As String) As DataColumn
    Dim col As New DataColumn
    col.DataType = GetType(String)
    col.ColumnName = colName
    col.AutoIncrement = False
    
    Return col
  End Function
  
  Private Sub AddAddressToTable(addr As AddressWords)
    If Me.dataGridView1.InvokeRequired Then
      Me.dataGridView1.Invoke(
        New SetAddrDelegate(AddressOf AddAddressToTable),
        New Object() { addr })
    Else
      Dim row As DataRow = Me.addressDataTable.NewRow
      row(COLUMN_NAME_ZIP)     = addr.Zipcode(0)
      row(COLUMN_NAME_ADDRESS) = addr.FullName
      Me.addressDataTable.Rows.Add(row)
      
      Me.lblFoundAddr.Text   = addr.FullName
      Me.lblResultCount.Text = Me.addressDataTable.Rows.Count.ToString & " 件"
      Me.dataGridView1.Update
    End If
  End Sub
  
  Private Sub UpdateAddressView()
    If Me.dataGridView1.InvokeRequired Then
      Me.dataGridView1.Invoke(
        New NoArgumentsDelegate(AddressOf UpdateAddressView))
    Else
      Me.lblResultCount.Text = Me.addressDataTable.Rows.Count.ToString & " 件"
      Me.dataGridView1.DataSource = Me.addressDataTable
      Me.dataGridView1.Update
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
  
  Private Delegate Sub NoArgumentsDelegate()
  Private Delegate Sub SetStringDelegate(str As String)
  Private Delegate Sub SetAddrDelegate(addr As AddressWords)
  Private Delegate Sub SetControlAndStringDelegate(control As Control, str As String)
  Private Delegate Sub SetExceptionDelegate(ex As Exception)

End Class
