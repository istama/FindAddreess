﻿'
' 日付: 2016/10/08
'
Partial Class MainForm
  Inherits System.Windows.Forms.Form
  
  ''' <summary>
  ''' Designer variable used to keep track of non-visual components.
  ''' </summary>
  Private components As System.ComponentModel.IContainer
  
  ''' <summary>
  ''' Disposes resources used by the form.
  ''' </summary>
  ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
  Protected Overrides Sub Dispose(ByVal disposing As Boolean)
    If disposing Then
      If components IsNot Nothing Then
        components.Dispose()
      End If
    End If
    MyBase.Dispose(disposing)
  End Sub
  
  ''' <summary>
  ''' This method is required for Windows Forms designer support.
  ''' Do not change the method contents inside the source code editor. The Forms designer might
  ''' not be able to load this method if it was changed manually.
  ''' </summary>
  Private Sub InitializeComponent()
    Me.label1 = New System.Windows.Forms.Label()
    Me.tboxZipcode = New System.Windows.Forms.TextBox()
    Me.label2 = New System.Windows.Forms.Label()
    Me.tboxPrefecture = New System.Windows.Forms.TextBox()
    Me.label3 = New System.Windows.Forms.Label()
    Me.label4 = New System.Windows.Forms.Label()
    Me.tboxCity = New System.Windows.Forms.TextBox()
    Me.tboxTownArea = New System.Windows.Forms.TextBox()
    Me.cboxMatchingModeOfCity = New System.Windows.Forms.ComboBox()
    Me.cboxMatchingModeOfTownArea = New System.Windows.Forms.ComboBox()
    Me.dataGridView1 = New System.Windows.Forms.DataGridView()
    Me.label5 = New System.Windows.Forms.Label()
    Me.btnSearch = New System.Windows.Forms.Button()
    Me.label6 = New System.Windows.Forms.Label()
    Me.label7 = New System.Windows.Forms.Label()
    Me.lblResultCount = New System.Windows.Forms.Label()
    Me.label9 = New System.Windows.Forms.Label()
    Me.label10 = New System.Windows.Forms.Label()
    Me.label11 = New System.Windows.Forms.Label()
    Me.lblMessage = New System.Windows.Forms.Label()
    Me.lblFoundAddr = New System.Windows.Forms.Label()
    Me.tabControl1 = New System.Windows.Forms.TabControl()
    Me.tabPage1 = New System.Windows.Forms.TabPage()
    Me.tabPage2 = New System.Windows.Forms.TabPage()
    CType(Me.dataGridView1,System.ComponentModel.ISupportInitialize).BeginInit
    Me.tabControl1.SuspendLayout
    Me.tabPage1.SuspendLayout
    Me.SuspendLayout
    '
    'label1
    '
    Me.label1.AutoSize = true
    Me.label1.Location = New System.Drawing.Point(15, 41)
    Me.label1.Name = "label1"
    Me.label1.Size = New System.Drawing.Size(53, 12)
    Me.label1.TabIndex = 0
    Me.label1.Text = "郵便番号"
    '
    'tboxZipcode
    '
    Me.tboxZipcode.ImeMode = System.Windows.Forms.ImeMode.Alpha
    Me.tboxZipcode.Location = New System.Drawing.Point(84, 38)
    Me.tboxZipcode.Name = "tboxZipcode"
    Me.tboxZipcode.Size = New System.Drawing.Size(152, 19)
    Me.tboxZipcode.TabIndex = 1
    AddHandler Me.tboxZipcode.KeyPress, AddressOf Me.TboxZipcodeKeyPress
    '
    'label2
    '
    Me.label2.AutoSize = true
    Me.label2.Location = New System.Drawing.Point(15, 66)
    Me.label2.Name = "label2"
    Me.label2.Size = New System.Drawing.Size(53, 12)
    Me.label2.TabIndex = 2
    Me.label2.Text = "都道府県"
    '
    'tboxPrefecture
    '
    Me.tboxPrefecture.ImeMode = System.Windows.Forms.ImeMode.Hiragana
    Me.tboxPrefecture.Location = New System.Drawing.Point(84, 63)
    Me.tboxPrefecture.Name = "tboxPrefecture"
    Me.tboxPrefecture.Size = New System.Drawing.Size(152, 19)
    Me.tboxPrefecture.TabIndex = 2
    AddHandler Me.tboxPrefecture.KeyPress, AddressOf Me.TboxKeyPress
    '
    'label3
    '
    Me.label3.AutoSize = true
    Me.label3.Location = New System.Drawing.Point(15, 91)
    Me.label3.Name = "label3"
    Me.label3.Size = New System.Drawing.Size(41, 12)
    Me.label3.TabIndex = 4
    Me.label3.Text = "市町村"
    '
    'label4
    '
    Me.label4.AutoSize = true
    Me.label4.Location = New System.Drawing.Point(15, 115)
    Me.label4.Name = "label4"
    Me.label4.Size = New System.Drawing.Size(29, 12)
    Me.label4.TabIndex = 5
    Me.label4.Text = "町域"
    '
    'tboxCity
    '
    Me.tboxCity.ImeMode = System.Windows.Forms.ImeMode.Hiragana
    Me.tboxCity.Location = New System.Drawing.Point(84, 88)
    Me.tboxCity.Name = "tboxCity"
    Me.tboxCity.Size = New System.Drawing.Size(152, 19)
    Me.tboxCity.TabIndex = 3
    AddHandler Me.tboxCity.KeyPress, AddressOf Me.TboxKeyPress
    '
    'tboxTownArea
    '
    Me.tboxTownArea.ImeMode = System.Windows.Forms.ImeMode.Hiragana
    Me.tboxTownArea.Location = New System.Drawing.Point(84, 112)
    Me.tboxTownArea.Name = "tboxTownArea"
    Me.tboxTownArea.Size = New System.Drawing.Size(152, 19)
    Me.tboxTownArea.TabIndex = 4
    AddHandler Me.tboxTownArea.KeyPress, AddressOf Me.TboxKeyPress
    '
    'cboxMatchingModeOfCity
    '
    Me.cboxMatchingModeOfCity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    Me.cboxMatchingModeOfCity.FormattingEnabled = true
    Me.cboxMatchingModeOfCity.Items.AddRange(New Object() {"前方一致", "完全一致", "部分一致"})
    Me.cboxMatchingModeOfCity.Location = New System.Drawing.Point(246, 87)
    Me.cboxMatchingModeOfCity.Name = "cboxMatchingModeOfCity"
    Me.cboxMatchingModeOfCity.Size = New System.Drawing.Size(102, 20)
    Me.cboxMatchingModeOfCity.TabIndex = 5
    '
    'cboxMatchingModeOfTownArea
    '
    Me.cboxMatchingModeOfTownArea.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    Me.cboxMatchingModeOfTownArea.FormattingEnabled = true
    Me.cboxMatchingModeOfTownArea.Items.AddRange(New Object() {"前方一致", "完全一致", "部分一致"})
    Me.cboxMatchingModeOfTownArea.Location = New System.Drawing.Point(246, 111)
    Me.cboxMatchingModeOfTownArea.Name = "cboxMatchingModeOfTownArea"
    Me.cboxMatchingModeOfTownArea.Size = New System.Drawing.Size(102, 20)
    Me.cboxMatchingModeOfTownArea.TabIndex = 6
    '
    'dataGridView1
    '
    Me.dataGridView1.AllowUserToAddRows = false
    Me.dataGridView1.AllowUserToDeleteRows = false
    Me.dataGridView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
    Me.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
    Me.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter
    Me.dataGridView1.Location = New System.Drawing.Point(12, 218)
    Me.dataGridView1.Name = "dataGridView1"
    Me.dataGridView1.RowTemplate.Height = 21
    Me.dataGridView1.Size = New System.Drawing.Size(405, 209)
    Me.dataGridView1.TabIndex = 10
    Me.dataGridView1.TabStop = false
    AddHandler Me.dataGridView1.EditingControlShowing, AddressOf Me.dataGridView1_EditingControlShowing
    '
    'label5
    '
    Me.label5.AutoSize = true
    Me.label5.Location = New System.Drawing.Point(73, 134)
    Me.label5.Name = "label5"
    Me.label5.Size = New System.Drawing.Size(163, 12)
    Me.label5.TabIndex = 11
    Me.label5.Text = "「？」はすべての文字に一致します"
    AddHandler Me.label5.Click, AddressOf Me.Label5Click
    '
    'btnSearch
    '
    Me.btnSearch.Location = New System.Drawing.Point(246, 134)
    Me.btnSearch.Name = "btnSearch"
    Me.btnSearch.Size = New System.Drawing.Size(102, 25)
    Me.btnSearch.TabIndex = 7
    Me.btnSearch.Text = "検索"
    Me.btnSearch.UseVisualStyleBackColor = true
    AddHandler Me.btnSearch.Click, AddressOf Me.BtnSearchClick
    '
    'label6
    '
    Me.label6.AutoSize = true
    Me.label6.Location = New System.Drawing.Point(246, 41)
    Me.label6.Name = "label6"
    Me.label6.Size = New System.Drawing.Size(53, 12)
    Me.label6.TabIndex = 13
    Me.label6.Text = "前方一致"
    '
    'label7
    '
    Me.label7.AutoSize = true
    Me.label7.Location = New System.Drawing.Point(246, 66)
    Me.label7.Name = "label7"
    Me.label7.Size = New System.Drawing.Size(53, 12)
    Me.label7.TabIndex = 14
    Me.label7.Text = "前方一致"
    '
    'lblResultCount
    '
    Me.lblResultCount.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
    Me.lblResultCount.Location = New System.Drawing.Point(356, 436)
    Me.lblResultCount.Name = "lblResultCount"
    Me.lblResultCount.Size = New System.Drawing.Size(62, 12)
    Me.lblResultCount.TabIndex = 15
    Me.lblResultCount.Text = "0 件"
    Me.lblResultCount.TextAlign = System.Drawing.ContentAlignment.TopRight
    '
    'label9
    '
    Me.label9.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
    Me.label9.AutoSize = true
    Me.label9.Location = New System.Drawing.Point(297, 436)
    Me.label9.Name = "label9"
    Me.label9.Size = New System.Drawing.Size(53, 12)
    Me.label9.TabIndex = 16
    Me.label9.Text = "検索結果"
    '
    'label10
    '
    Me.label10.AutoSize = true
    Me.label10.Location = New System.Drawing.Point(84, 16)
    Me.label10.Name = "label10"
    Me.label10.Size = New System.Drawing.Size(57, 12)
    Me.label10.TabIndex = 18
    Me.label10.Text = "検索ワード"
    '
    'label11
    '
    Me.label11.AutoSize = true
    Me.label11.Location = New System.Drawing.Point(246, 16)
    Me.label11.Name = "label11"
    Me.label11.Size = New System.Drawing.Size(53, 12)
    Me.label11.TabIndex = 19
    Me.label11.Text = "検索方法"
    '
    'lblMessage
    '
    Me.lblMessage.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
    Me.lblMessage.Location = New System.Drawing.Point(13, 436)
    Me.lblMessage.Name = "lblMessage"
    Me.lblMessage.Size = New System.Drawing.Size(75, 12)
    Me.lblMessage.TabIndex = 20
    '
    'lblFoundAddr
    '
    Me.lblFoundAddr.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
    Me.lblFoundAddr.AutoSize = true
    Me.lblFoundAddr.Location = New System.Drawing.Point(91, 436)
    Me.lblFoundAddr.Name = "lblFoundAddr"
    Me.lblFoundAddr.Size = New System.Drawing.Size(0, 12)
    Me.lblFoundAddr.TabIndex = 21
    '
    'tabControl1
    '
    Me.tabControl1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
    Me.tabControl1.Controls.Add(Me.tabPage1)
    Me.tabControl1.Controls.Add(Me.tabPage2)
    Me.tabControl1.Location = New System.Drawing.Point(9, 8)
    Me.tabControl1.Name = "tabControl1"
    Me.tabControl1.SelectedIndex = 0
    Me.tabControl1.Size = New System.Drawing.Size(412, 204)
    Me.tabControl1.TabIndex = 22
    Me.tabControl1.TabStop = false
    '
    'tabPage1
    '
    Me.tabPage1.BackColor = System.Drawing.Color.Transparent
    Me.tabPage1.Controls.Add(Me.tboxCity)
    Me.tabPage1.Controls.Add(Me.label1)
    Me.tabPage1.Controls.Add(Me.tboxZipcode)
    Me.tabPage1.Controls.Add(Me.label11)
    Me.tabPage1.Controls.Add(Me.label2)
    Me.tabPage1.Controls.Add(Me.label10)
    Me.tabPage1.Controls.Add(Me.tboxPrefecture)
    Me.tabPage1.Controls.Add(Me.label3)
    Me.tabPage1.Controls.Add(Me.label4)
    Me.tabPage1.Controls.Add(Me.label7)
    Me.tabPage1.Controls.Add(Me.tboxTownArea)
    Me.tabPage1.Controls.Add(Me.label6)
    Me.tabPage1.Controls.Add(Me.cboxMatchingModeOfCity)
    Me.tabPage1.Controls.Add(Me.btnSearch)
    Me.tabPage1.Controls.Add(Me.cboxMatchingModeOfTownArea)
    Me.tabPage1.Controls.Add(Me.label5)
    Me.tabPage1.Location = New System.Drawing.Point(4, 22)
    Me.tabPage1.Name = "tabPage1"
    Me.tabPage1.Padding = New System.Windows.Forms.Padding(3)
    Me.tabPage1.Size = New System.Drawing.Size(404, 178)
    Me.tabPage1.TabIndex = 0
    Me.tabPage1.Text = "住所"
    '
    'tabPage2
    '
    Me.tabPage2.BackColor = System.Drawing.Color.Transparent
    Me.tabPage2.Location = New System.Drawing.Point(4, 22)
    Me.tabPage2.Name = "tabPage2"
    Me.tabPage2.Padding = New System.Windows.Forms.Padding(3)
    Me.tabPage2.Size = New System.Drawing.Size(398, 178)
    Me.tabPage2.TabIndex = 1
    Me.tabPage2.Text = "郵便局"
    '
    'MainForm
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 12!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(430, 457)
    Me.Controls.Add(Me.tabControl1)
    Me.Controls.Add(Me.lblFoundAddr)
    Me.Controls.Add(Me.lblMessage)
    Me.Controls.Add(Me.label9)
    Me.Controls.Add(Me.lblResultCount)
    Me.Controls.Add(Me.dataGridView1)
    Me.MaximizeBox = false
    Me.Name = "MainForm"
    Me.Text = "FindAddress"
    AddHandler Load, AddressOf Me.MainFormLoad
    CType(Me.dataGridView1,System.ComponentModel.ISupportInitialize).EndInit
    Me.tabControl1.ResumeLayout(false)
    Me.tabPage1.ResumeLayout(false)
    Me.tabPage1.PerformLayout
    Me.ResumeLayout(false)
    Me.PerformLayout
  End Sub
  Private tabPage2 As System.Windows.Forms.TabPage
  Private tabPage1 As System.Windows.Forms.TabPage
  Private tabControl1 As System.Windows.Forms.TabControl
  Private lblFoundAddr As System.Windows.Forms.Label
  Private lblMessage As System.Windows.Forms.Label
  Private label11 As System.Windows.Forms.Label
  Private label10 As System.Windows.Forms.Label
  Private label9 As System.Windows.Forms.Label
  Private lblResultCount As System.Windows.Forms.Label
  Private label7 As System.Windows.Forms.Label
  Private label6 As System.Windows.Forms.Label
  Private btnSearch As System.Windows.Forms.Button
  Private label5 As System.Windows.Forms.Label
  Private dataGridView1 As System.Windows.Forms.DataGridView
  Private cboxMatchingModeOfTownArea As System.Windows.Forms.ComboBox
  Private cboxMatchingModeOfCity As System.Windows.Forms.ComboBox
  Private tboxTownArea As System.Windows.Forms.TextBox
  Private tboxCity As System.Windows.Forms.TextBox
  Private label4 As System.Windows.Forms.Label
  Private label3 As System.Windows.Forms.Label
  Private tboxPrefecture As System.Windows.Forms.TextBox
  Private label2 As System.Windows.Forms.Label
  Private tboxZipcode As System.Windows.Forms.TextBox
  Private label1 As System.Windows.Forms.Label
  

  
  Sub Label5Click(sender As Object, e As EventArgs)
    
  End Sub
  

End Class
