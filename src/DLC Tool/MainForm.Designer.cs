using System.Drawing;
namespace DLC_Tool
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.btnOpenBCM = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.openFileDialogBCM = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogBCM = new System.Windows.Forms.SaveFileDialog();
            this.btnNewDLC = new System.Windows.Forms.Button();
            this.tbSavePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbBCMVer = new System.Windows.Forms.TextBox();
            this.gbChars = new System.Windows.Forms.GroupBox();
            this.btnCharsAdd = new System.Windows.Forms.Button();
            this.tbListPath = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbSaveListInDLC = new System.Windows.Forms.CheckBox();
            this.gbHairs2 = new System.Windows.Forms.GroupBox();
            this.btnHStylesDelete = new System.Windows.Forms.Button();
            this.btnHStylesAdd = new System.Windows.Forms.Button();
            this.dgvHStyles = new System.Windows.Forms.DataGridView();
            this.clmType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmHair = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmFace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnCharsDelete = new System.Windows.Forms.Button();
            this.btnOpenState = new System.Windows.Forms.Button();
            this.gbFiles = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvFiles = new System.Windows.Forms.DataGridView();
            this.FilePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label4 = new System.Windows.Forms.Label();
            this.btnFilesAdd = new System.Windows.Forms.Button();
            this.cb4H = new System.Windows.Forms.CheckBox();
            this.cb3H = new System.Windows.Forms.CheckBox();
            this.cb2H = new System.Windows.Forms.CheckBox();
            this.cb1H = new System.Windows.Forms.CheckBox();
            this.cbTMCL = new System.Windows.Forms.CheckBox();
            this.cbTMC = new System.Windows.Forms.CheckBox();
            this.cbP = new System.Windows.Forms.CheckBox();
            this.cbC = new System.Windows.Forms.CheckBox();
            this.btnFilesDelete = new System.Windows.Forms.Button();
            this.lbFiles = new System.Windows.Forms.ListBox();
            this.dgvChars = new System.Windows.Forms.DataGridView();
            this.clmName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmCos = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmInner = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clmComment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnSaveState = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.catMale = new System.Windows.Forms.ToolStripMenuItem();
            this.id3 = new System.Windows.Forms.ToolStripMenuItem();
            this.id31 = new System.Windows.Forms.ToolStripMenuItem();
            this.id14 = new System.Windows.Forms.ToolStripMenuItem();
            this.id6 = new System.Windows.Forms.ToolStripMenuItem();
            this.id0 = new System.Windows.Forms.ToolStripMenuItem();
            this.id41 = new System.Windows.Forms.ToolStripMenuItem();
            this.id2 = new System.Windows.Forms.ToolStripMenuItem();
            this.id9 = new System.Windows.Forms.ToolStripMenuItem();
            this.id24 = new System.Windows.Forms.ToolStripMenuItem();
            this.id11 = new System.Windows.Forms.ToolStripMenuItem();
            this.id19 = new System.Windows.Forms.ToolStripMenuItem();
            this.id46 = new System.Windows.Forms.ToolStripMenuItem();
            this.id29 = new System.Windows.Forms.ToolStripMenuItem();
            this.id4 = new System.Windows.Forms.ToolStripMenuItem();
            this.id8 = new System.Windows.Forms.ToolStripMenuItem();
            this.catFemale = new System.Windows.Forms.ToolStripMenuItem();
            this.id13 = new System.Windows.Forms.ToolStripMenuItem();
            this.id7 = new System.Windows.Forms.ToolStripMenuItem();
            this.id5 = new System.Windows.Forms.ToolStripMenuItem();
            this.id20 = new System.Windows.Forms.ToolStripMenuItem();
            this.id10 = new System.Windows.Forms.ToolStripMenuItem();
            this.id32 = new System.Windows.Forms.ToolStripMenuItem();
            this.id1 = new System.Windows.Forms.ToolStripMenuItem();
            this.id44 = new System.Windows.Forms.ToolStripMenuItem();
            this.id33 = new System.Windows.Forms.ToolStripMenuItem();
            this.id21 = new System.Windows.Forms.ToolStripMenuItem();
            this.id43 = new System.Windows.Forms.ToolStripMenuItem();
            this.id45 = new System.Windows.Forms.ToolStripMenuItem();
            this.id42 = new System.Windows.Forms.ToolStripMenuItem();
            this.id30 = new System.Windows.Forms.ToolStripMenuItem();
            this.id39 = new System.Windows.Forms.ToolStripMenuItem();
            this.id15 = new System.Windows.Forms.ToolStripMenuItem();
            this.id40 = new System.Windows.Forms.ToolStripMenuItem();
            this.id12 = new System.Windows.Forms.ToolStripMenuItem();
            this.id16 = new System.Windows.Forms.ToolStripMenuItem();
            this.コピーToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.リストの追加読み込みToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialogState = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialogState = new System.Windows.Forms.SaveFileDialog();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.コピーCtrlCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.貼り付けCtrlVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.削除DeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearPasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsSlot = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnCmpSave = new System.Windows.Forms.Button();
            this.cbDAT = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmsAddDelCom = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.smiAddComColumn = new System.Windows.Forms.ToolStripMenuItem();
            this.smiDelComColumn = new System.Windows.Forms.ToolStripMenuItem();
            this.gbChars.SuspendLayout();
            this.gbHairs2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHStyles)).BeginInit();
            this.gbFiles.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChars)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.cmsAddDelCom.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOpenBCM
            // 
            this.btnOpenBCM.Font = new System.Drawing.Font("メイリオ", 9F);
            this.btnOpenBCM.Location = new System.Drawing.Point(100, 7);
            this.btnOpenBCM.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOpenBCM.Name = "btnOpenBCM";
            this.btnOpenBCM.Size = new System.Drawing.Size(88, 31);
            this.btnOpenBCM.TabIndex = 1;
            this.btnOpenBCM.Text = "BCMを開く";
            this.btnOpenBCM.UseVisualStyleBackColor = true;
            this.btnOpenBCM.Click += new System.EventHandler(this.btnOpenBCM_Click);
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Font = new System.Drawing.Font("メイリオ", 9F);
            this.btnSave.Location = new System.Drawing.Point(194, 7);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(88, 31);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // openFileDialogBCM
            // 
            this.openFileDialogBCM.Filter = "*.bcm|*.bcm";
            // 
            // saveFileDialogBCM
            // 
            this.saveFileDialogBCM.Filter = "*.bcm|*.bcm";
            // 
            // btnNewDLC
            // 
            this.btnNewDLC.Font = new System.Drawing.Font("メイリオ", 9F);
            this.btnNewDLC.Location = new System.Drawing.Point(6, 7);
            this.btnNewDLC.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnNewDLC.Name = "btnNewDLC";
            this.btnNewDLC.Size = new System.Drawing.Size(88, 31);
            this.btnNewDLC.TabIndex = 0;
            this.btnNewDLC.Text = "新規作成";
            this.btnNewDLC.UseVisualStyleBackColor = true;
            this.btnNewDLC.Click += new System.EventHandler(this.btnNewDLC_Click);
            // 
            // tbSavePath
            // 
            this.tbSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSavePath.Location = new System.Drawing.Point(6, 46);
            this.tbSavePath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbSavePath.Name = "tbSavePath";
            this.tbSavePath.Size = new System.Drawing.Size(612, 25);
            this.tbSavePath.TabIndex = 3;
            this.tbSavePath.TextChanged += new System.EventHandler(this.tbSavePath_TextChanged);
            this.tbSavePath.DoubleClick += new System.EventHandler(this.tbSavePath_DoubleClick);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(538, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 18);
            this.label1.TabIndex = 4;
            this.label1.Text = "BCM ver.";
            // 
            // tbBCMVer
            // 
            this.tbBCMVer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBCMVer.Location = new System.Drawing.Point(594, 10);
            this.tbBCMVer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tbBCMVer.MaxLength = 3;
            this.tbBCMVer.Name = "tbBCMVer";
            this.tbBCMVer.Size = new System.Drawing.Size(24, 25);
            this.tbBCMVer.TabIndex = 4;
            this.tbBCMVer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbBCMVer.TextChanged += new System.EventHandler(this.ParseValue);
            this.tbBCMVer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RestrictKeys);
            // 
            // gbChars
            // 
            this.gbChars.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbChars.Controls.Add(this.btnCharsAdd);
            this.gbChars.Controls.Add(this.tbListPath);
            this.gbChars.Controls.Add(this.label3);
            this.gbChars.Controls.Add(this.cbSaveListInDLC);
            this.gbChars.Controls.Add(this.gbHairs2);
            this.gbChars.Controls.Add(this.btnCharsDelete);
            this.gbChars.Controls.Add(this.btnOpenState);
            this.gbChars.Controls.Add(this.gbFiles);
            this.gbChars.Controls.Add(this.dgvChars);
            this.gbChars.Controls.Add(this.btnSaveState);
            this.gbChars.Location = new System.Drawing.Point(6, 78);
            this.gbChars.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbChars.Name = "gbChars";
            this.gbChars.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbChars.Size = new System.Drawing.Size(612, 660);
            this.gbChars.TabIndex = 8;
            this.gbChars.TabStop = false;
            this.gbChars.Text = "キャラクター";
            // 
            // btnCharsAdd
            // 
            this.btnCharsAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCharsAdd.Enabled = false;
            this.btnCharsAdd.Font = new System.Drawing.Font("メイリオ", 9F);
            this.btnCharsAdd.Location = new System.Drawing.Point(423, 25);
            this.btnCharsAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCharsAdd.Name = "btnCharsAdd";
            this.btnCharsAdd.Size = new System.Drawing.Size(88, 31);
            this.btnCharsAdd.TabIndex = 1;
            this.btnCharsAdd.Text = "追加";
            this.btnCharsAdd.UseVisualStyleBackColor = true;
            this.btnCharsAdd.Click += new System.EventHandler(this.btnCharsAdd_Click);
            // 
            // tbListPath
            // 
            this.tbListPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbListPath.Location = new System.Drawing.Point(196, 30);
            this.tbListPath.Name = "tbListPath";
            this.tbListPath.Size = new System.Drawing.Size(197, 25);
            this.tbListPath.TabIndex = 21;
            this.tbListPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbListPath.TextChanged += new System.EventHandler(this.tbListPath_TextChanged);
            this.tbListPath.DoubleClick += new System.EventHandler(this.tbListPath_DoubleClick);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(311, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 18);
            this.label3.TabIndex = 25;
            this.label3.Text = ".lst";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // cbSaveListInDLC
            // 
            this.cbSaveListInDLC.AutoSize = true;
            this.cbSaveListInDLC.Enabled = false;
            this.cbSaveListInDLC.Location = new System.Drawing.Point(196, 11);
            this.cbSaveListInDLC.Name = "cbSaveListInDLC";
            this.cbSaveListInDLC.Size = new System.Drawing.Size(207, 22);
            this.cbSaveListInDLC.TabIndex = 25;
            this.cbSaveListInDLC.Text = "同じ名前でDLCフォルダにも保存";
            this.cbSaveListInDLC.UseVisualStyleBackColor = true;
            // 
            // gbHairs2
            // 
            this.gbHairs2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gbHairs2.Controls.Add(this.btnHStylesDelete);
            this.gbHairs2.Controls.Add(this.btnHStylesAdd);
            this.gbHairs2.Controls.Add(this.dgvHStyles);
            this.gbHairs2.Location = new System.Drawing.Point(469, 361);
            this.gbHairs2.Name = "gbHairs2";
            this.gbHairs2.Size = new System.Drawing.Size(136, 291);
            this.gbHairs2.TabIndex = 10;
            this.gbHairs2.TabStop = false;
            this.gbHairs2.Text = "髪型";
            // 
            // btnHStylesDelete
            // 
            this.btnHStylesDelete.Enabled = false;
            this.btnHStylesDelete.Font = new System.Drawing.Font("メイリオ", 9F);
            this.btnHStylesDelete.Location = new System.Drawing.Point(71, 25);
            this.btnHStylesDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnHStylesDelete.Name = "btnHStylesDelete";
            this.btnHStylesDelete.Size = new System.Drawing.Size(59, 31);
            this.btnHStylesDelete.TabIndex = 5;
            this.btnHStylesDelete.Text = "削除";
            this.btnHStylesDelete.UseVisualStyleBackColor = true;
            this.btnHStylesDelete.Click += new System.EventHandler(this.btnHStylesDelete_Click);
            // 
            // btnHStylesAdd
            // 
            this.btnHStylesAdd.Enabled = false;
            this.btnHStylesAdd.Font = new System.Drawing.Font("メイリオ", 9F);
            this.btnHStylesAdd.Location = new System.Drawing.Point(7, 25);
            this.btnHStylesAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnHStylesAdd.Name = "btnHStylesAdd";
            this.btnHStylesAdd.Size = new System.Drawing.Size(59, 31);
            this.btnHStylesAdd.TabIndex = 4;
            this.btnHStylesAdd.Text = "追加";
            this.btnHStylesAdd.UseVisualStyleBackColor = true;
            this.btnHStylesAdd.Click += new System.EventHandler(this.btnHStylesAdd_Click);
            // 
            // dgvHStyles
            // 
            this.dgvHStyles.AllowUserToAddRows = false;
            this.dgvHStyles.AllowUserToDeleteRows = false;
            this.dgvHStyles.AllowUserToResizeColumns = false;
            this.dgvHStyles.AllowUserToResizeRows = false;
            this.dgvHStyles.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvHStyles.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvHStyles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvHStyles.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgvHStyles.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("メイリオ", 9F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvHStyles.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvHStyles.ColumnHeadersHeight = 25;
            this.dgvHStyles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvHStyles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmType,
            this.clmHair,
            this.clmFace});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("メイリオ", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvHStyles.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvHStyles.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvHStyles.GridColor = System.Drawing.SystemColors.Control;
            this.dgvHStyles.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dgvHStyles.Location = new System.Drawing.Point(7, 64);
            this.dgvHStyles.Name = "dgvHStyles";
            this.dgvHStyles.RowHeadersVisible = false;
            this.dgvHStyles.RowTemplate.Height = 21;
            this.dgvHStyles.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvHStyles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHStyles.ShowCellToolTips = false;
            this.dgvHStyles.Size = new System.Drawing.Size(122, 219);
            this.dgvHStyles.TabIndex = 3;
            this.dgvHStyles.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.gv_CellValidated);
            this.dgvHStyles.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.gv_CellValidating);
            this.dgvHStyles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvHStyles_KeyDown);
            this.dgvHStyles.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvHStyles_KeyUp);
            this.dgvHStyles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvHStyles_MouseDown);
            // 
            // clmType
            // 
            this.clmType.Frozen = true;
            this.clmType.HeaderText = "タイプ";
            this.clmType.Name = "clmType";
            this.clmType.ReadOnly = true;
            this.clmType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmType.Width = 60;
            // 
            // clmHair
            // 
            this.clmHair.Frozen = true;
            this.clmHair.HeaderText = "髪";
            this.clmHair.Name = "clmHair";
            this.clmHair.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmHair.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmHair.Width = 30;
            // 
            // clmFace
            // 
            this.clmFace.Frozen = true;
            this.clmFace.HeaderText = "顔";
            this.clmFace.Name = "clmFace";
            this.clmFace.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmFace.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmFace.Width = 30;
            // 
            // btnCharsDelete
            // 
            this.btnCharsDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCharsDelete.Enabled = false;
            this.btnCharsDelete.Font = new System.Drawing.Font("メイリオ", 9F);
            this.btnCharsDelete.Location = new System.Drawing.Point(517, 25);
            this.btnCharsDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCharsDelete.Name = "btnCharsDelete";
            this.btnCharsDelete.Size = new System.Drawing.Size(88, 31);
            this.btnCharsDelete.TabIndex = 2;
            this.btnCharsDelete.Text = "削除";
            this.btnCharsDelete.UseVisualStyleBackColor = true;
            this.btnCharsDelete.Click += new System.EventHandler(this.btnCharsDelete_Click);
            // 
            // btnOpenState
            // 
            this.btnOpenState.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenState.Location = new System.Drawing.Point(8, 25);
            this.btnOpenState.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOpenState.Name = "btnOpenState";
            this.btnOpenState.Size = new System.Drawing.Size(88, 31);
            this.btnOpenState.TabIndex = 5;
            this.btnOpenState.Text = "リストを開く";
            this.btnOpenState.UseVisualStyleBackColor = true;
            this.btnOpenState.Click += new System.EventHandler(this.btnOpenState_Click);
            // 
            // gbFiles
            // 
            this.gbFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFiles.Controls.Add(this.groupBox1);
            this.gbFiles.Controls.Add(this.label4);
            this.gbFiles.Controls.Add(this.btnFilesAdd);
            this.gbFiles.Controls.Add(this.cb4H);
            this.gbFiles.Controls.Add(this.cb3H);
            this.gbFiles.Controls.Add(this.cb2H);
            this.gbFiles.Controls.Add(this.cb1H);
            this.gbFiles.Controls.Add(this.cbTMCL);
            this.gbFiles.Controls.Add(this.cbTMC);
            this.gbFiles.Controls.Add(this.cbP);
            this.gbFiles.Controls.Add(this.cbC);
            this.gbFiles.Controls.Add(this.btnFilesDelete);
            this.gbFiles.Controls.Add(this.lbFiles);
            this.gbFiles.Location = new System.Drawing.Point(7, 361);
            this.gbFiles.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbFiles.Name = "gbFiles";
            this.gbFiles.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gbFiles.Size = new System.Drawing.Size(456, 292);
            this.gbFiles.TabIndex = 20;
            this.gbFiles.TabStop = false;
            this.gbFiles.Text = "ファイル";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dgvFiles);
            this.groupBox1.Location = new System.Drawing.Point(8, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(440, 218);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            // 
            // dgvFiles
            // 
            this.dgvFiles.AllowUserToAddRows = false;
            this.dgvFiles.AllowUserToDeleteRows = false;
            this.dgvFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFiles.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvFiles.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFiles.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvFiles.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgvFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFiles.ColumnHeadersVisible = false;
            this.dgvFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.FilePath});
            this.dgvFiles.GridColor = System.Drawing.SystemColors.ActiveBorder;
            this.dgvFiles.Location = new System.Drawing.Point(0, 0);
            this.dgvFiles.Name = "dgvFiles";
            this.dgvFiles.ReadOnly = true;
            this.dgvFiles.RowHeadersVisible = false;
            this.dgvFiles.RowTemplate.Height = 18;
            this.dgvFiles.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvFiles.Size = new System.Drawing.Size(440, 218);
            this.dgvFiles.TabIndex = 17;
            this.dgvFiles.SelectionChanged += new System.EventHandler(this.dgvFiles_SelectionChanged);
            this.dgvFiles.DoubleClick += new System.EventHandler(this.dgvFiles_DoubleClick);
            this.dgvFiles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvFiles_KeyDown);
            this.dgvFiles.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvFiles_KeyUp);
            this.dgvFiles.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvFiles_MouseDown);
            // 
            // FilePath
            // 
            this.FilePath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FilePath.HeaderText = "";
            this.FilePath.Name = "FilePath";
            this.FilePath.ReadOnly = true;
            this.FilePath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label4.Location = new System.Drawing.Point(7, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(442, 220);
            this.label4.TabIndex = 26;
            // 
            // btnFilesAdd
            // 
            this.btnFilesAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilesAdd.Enabled = false;
            this.btnFilesAdd.Font = new System.Drawing.Font("メイリオ", 9F);
            this.btnFilesAdd.Location = new System.Drawing.Point(267, 24);
            this.btnFilesAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnFilesAdd.Name = "btnFilesAdd";
            this.btnFilesAdd.Size = new System.Drawing.Size(88, 31);
            this.btnFilesAdd.TabIndex = 7;
            this.btnFilesAdd.Text = "追加";
            this.btnFilesAdd.UseVisualStyleBackColor = true;
            this.btnFilesAdd.Click += new System.EventHandler(this.btnFilesAdd_Click);
            // 
            // cb4H
            // 
            this.cb4H.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb4H.AutoCheck = false;
            this.cb4H.AutoSize = true;
            this.cb4H.Enabled = false;
            this.cb4H.Location = new System.Drawing.Point(205, 38);
            this.cb4H.Name = "cb4H";
            this.cb4H.Size = new System.Drawing.Size(63, 22);
            this.cb4H.TabIndex = 16;
            this.cb4H.Text = "4.--H*";
            this.cb4H.ThreeState = true;
            this.cb4H.UseVisualStyleBackColor = true;
            this.cb4H.Click += new System.EventHandler(this.cb4H_Click);
            // 
            // cb3H
            // 
            this.cb3H.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb3H.AutoCheck = false;
            this.cb3H.AutoSize = true;
            this.cb3H.Enabled = false;
            this.cb3H.Location = new System.Drawing.Point(139, 38);
            this.cb3H.Name = "cb3H";
            this.cb3H.Size = new System.Drawing.Size(63, 22);
            this.cb3H.TabIndex = 15;
            this.cb3H.Text = "3.--H*";
            this.cb3H.ThreeState = true;
            this.cb3H.UseVisualStyleBackColor = true;
            this.cb3H.Click += new System.EventHandler(this.cb3H_Click);
            // 
            // cb2H
            // 
            this.cb2H.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb2H.AutoCheck = false;
            this.cb2H.AutoSize = true;
            this.cb2H.Enabled = false;
            this.cb2H.Location = new System.Drawing.Point(73, 38);
            this.cb2H.Name = "cb2H";
            this.cb2H.Size = new System.Drawing.Size(63, 22);
            this.cb2H.TabIndex = 14;
            this.cb2H.Text = "2.--H*";
            this.cb2H.ThreeState = true;
            this.cb2H.UseVisualStyleBackColor = true;
            this.cb2H.Click += new System.EventHandler(this.cb2H_Click);
            // 
            // cb1H
            // 
            this.cb1H.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cb1H.AutoCheck = false;
            this.cb1H.AutoSize = true;
            this.cb1H.Enabled = false;
            this.cb1H.Location = new System.Drawing.Point(7, 38);
            this.cb1H.Name = "cb1H";
            this.cb1H.Size = new System.Drawing.Size(63, 22);
            this.cb1H.TabIndex = 13;
            this.cb1H.Text = "1.--H*";
            this.cb1H.ThreeState = true;
            this.cb1H.UseVisualStyleBackColor = true;
            this.cb1H.Click += new System.EventHandler(this.cb1H_Click);
            // 
            // cbTMCL
            // 
            this.cbTMCL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbTMCL.AutoCheck = false;
            this.cbTMCL.AutoSize = true;
            this.cbTMCL.Enabled = false;
            this.cbTMCL.Location = new System.Drawing.Point(205, 20);
            this.cbTMCL.Name = "cbTMCL";
            this.cbTMCL.Size = new System.Drawing.Size(60, 22);
            this.cbTMCL.TabIndex = 12;
            this.cbTMCL.Text = "TMCL";
            this.cbTMCL.ThreeState = true;
            this.cbTMCL.UseVisualStyleBackColor = true;
            this.cbTMCL.Click += new System.EventHandler(this.cbTMCL_Click);
            // 
            // cbTMC
            // 
            this.cbTMC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbTMC.AutoCheck = false;
            this.cbTMC.AutoSize = true;
            this.cbTMC.Enabled = false;
            this.cbTMC.Location = new System.Drawing.Point(139, 20);
            this.cbTMC.Name = "cbTMC";
            this.cbTMC.Size = new System.Drawing.Size(53, 22);
            this.cbTMC.TabIndex = 11;
            this.cbTMC.Text = "TMC";
            this.cbTMC.ThreeState = true;
            this.cbTMC.UseVisualStyleBackColor = true;
            this.cbTMC.Click += new System.EventHandler(this.cbTMC_Click);
            // 
            // cbP
            // 
            this.cbP.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbP.AutoCheck = false;
            this.cbP.AutoSize = true;
            this.cbP.Enabled = false;
            this.cbP.Location = new System.Drawing.Point(73, 20);
            this.cbP.Name = "cbP";
            this.cbP.Size = new System.Drawing.Size(44, 22);
            this.cbP.TabIndex = 10;
            this.cbP.Text = "--P";
            this.cbP.ThreeState = true;
            this.cbP.UseVisualStyleBackColor = true;
            this.cbP.Click += new System.EventHandler(this.cbP_Click);
            // 
            // cbC
            // 
            this.cbC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbC.AutoCheck = false;
            this.cbC.AutoSize = true;
            this.cbC.Enabled = false;
            this.cbC.Location = new System.Drawing.Point(7, 20);
            this.cbC.Name = "cbC";
            this.cbC.Size = new System.Drawing.Size(50, 22);
            this.cbC.TabIndex = 9;
            this.cbC.Text = "---C";
            this.cbC.ThreeState = true;
            this.cbC.UseVisualStyleBackColor = true;
            this.cbC.Click += new System.EventHandler(this.cbC_Click);
            // 
            // btnFilesDelete
            // 
            this.btnFilesDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFilesDelete.Enabled = false;
            this.btnFilesDelete.Font = new System.Drawing.Font("メイリオ", 9F);
            this.btnFilesDelete.Location = new System.Drawing.Point(361, 24);
            this.btnFilesDelete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnFilesDelete.Name = "btnFilesDelete";
            this.btnFilesDelete.Size = new System.Drawing.Size(88, 31);
            this.btnFilesDelete.TabIndex = 8;
            this.btnFilesDelete.Text = "削除";
            this.btnFilesDelete.UseVisualStyleBackColor = true;
            this.btnFilesDelete.Click += new System.EventHandler(this.btnFilesDelete_Click);
            // 
            // lbFiles
            // 
            this.lbFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbFiles.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbFiles.Font = new System.Drawing.Font("メイリオ", 9F);
            this.lbFiles.FormattingEnabled = true;
            this.lbFiles.ItemHeight = 18;
            this.lbFiles.Location = new System.Drawing.Point(7, 64);
            this.lbFiles.Name = "lbFiles";
            this.lbFiles.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbFiles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbFiles.Size = new System.Drawing.Size(442, 220);
            this.lbFiles.TabIndex = 6;
            // 
            // dgvChars
            // 
            this.dgvChars.AllowDrop = true;
            this.dgvChars.AllowUserToAddRows = false;
            this.dgvChars.AllowUserToDeleteRows = false;
            this.dgvChars.AllowUserToOrderColumns = true;
            this.dgvChars.AllowUserToResizeColumns = false;
            this.dgvChars.AllowUserToResizeRows = false;
            this.dgvChars.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvChars.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvChars.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvChars.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvChars.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgvChars.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("メイリオ", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvChars.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvChars.ColumnHeadersHeight = 25;
            this.dgvChars.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvChars.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmName,
            this.clmCos,
            this.clmInner,
            this.clmComment});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("メイリオ", 9F);
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvChars.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvChars.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvChars.GridColor = System.Drawing.SystemColors.Control;
            this.dgvChars.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.dgvChars.Location = new System.Drawing.Point(7, 64);
            this.dgvChars.MultiSelect = false;
            this.dgvChars.Name = "dgvChars";
            this.dgvChars.RowHeadersVisible = false;
            this.dgvChars.RowTemplate.Height = 21;
            this.dgvChars.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvChars.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvChars.ShowCellToolTips = false;
            this.dgvChars.Size = new System.Drawing.Size(598, 291);
            this.dgvChars.TabIndex = 0;
            this.dgvChars.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvChars_CellEnter);
            this.dgvChars.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.gv_CellValidated);
            this.dgvChars.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.gv_CellValidating);
            this.dgvChars.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvChars_ColumnHeaderMouseClick);
            this.dgvChars.SelectionChanged += new System.EventHandler(this.dgvChars_SelectionChanged);
            this.dgvChars.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvChars_DragDrop);
            this.dgvChars.DragEnter += new System.Windows.Forms.DragEventHandler(this.dgvChars_DragEnter);
            this.dgvChars.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvChars_KeyDown);
            this.dgvChars.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dgvChars_KeyUp);
            this.dgvChars.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvChars_MouseDown);
            this.dgvChars.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dgvChars_MouseMove);
            this.dgvChars.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgvChars_MouseUp);
            // 
            // clmName
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clmName.DefaultCellStyle = dataGridViewCellStyle4;
            this.clmName.Frozen = true;
            this.clmName.HeaderText = "名前";
            this.clmName.Name = "clmName";
            this.clmName.ReadOnly = true;
            this.clmName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clmName.Width = 106;
            // 
            // clmCos
            // 
            this.clmCos.Frozen = true;
            this.clmCos.HeaderText = "ｽﾛｯﾄ";
            this.clmCos.Name = "clmCos";
            this.clmCos.ReadOnly = true;
            this.clmCos.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmCos.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.clmCos.Width = 40;
            // 
            // clmInner
            // 
            this.clmInner.Frozen = true;
            this.clmInner.HeaderText = "ｲﾝﾅｰ";
            this.clmInner.Name = "clmInner";
            this.clmInner.ReadOnly = true;
            this.clmInner.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.clmInner.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.clmInner.Width = 40;
            // 
            // clmComment
            // 
            this.clmComment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.clmComment.DefaultCellStyle = dataGridViewCellStyle5;
            this.clmComment.HeaderText = "コメント";
            this.clmComment.Name = "clmComment";
            this.clmComment.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // btnSaveState
            // 
            this.btnSaveState.Enabled = false;
            this.btnSaveState.Font = new System.Drawing.Font("メイリオ", 9F);
            this.btnSaveState.Location = new System.Drawing.Point(102, 25);
            this.btnSaveState.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSaveState.Name = "btnSaveState";
            this.btnSaveState.Size = new System.Drawing.Size(88, 31);
            this.btnSaveState.TabIndex = 6;
            this.btnSaveState.Text = "リストを保存";
            this.btnSaveState.UseVisualStyleBackColor = true;
            this.btnSaveState.Click += new System.EventHandler(this.btnSaveState_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "All supported formats|*.---C;*.--P;*.TMC;*.TMCL;*.--H;*.--HL";
            this.openFileDialog.Multiselect = true;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.catMale,
            this.catFemale,
            this.コピーToolStripMenuItem,
            this.リストの追加読み込みToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(159, 92);
            // 
            // catMale
            // 
            this.catMale.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.catMale.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.id3,
            this.id31,
            this.id14,
            this.id6,
            this.id0,
            this.id41,
            this.id2,
            this.id9,
            this.id24,
            this.id11,
            this.id19,
            this.id46,
            this.id29,
            this.id4,
            this.id8});
            this.catMale.Name = "catMale";
            this.catMale.Size = new System.Drawing.Size(158, 22);
            this.catMale.Text = "男 :";
            // 
            // id3
            // 
            this.id3.Name = "id3";
            this.id3.Size = new System.Drawing.Size(142, 22);
            this.id3.Text = "アイン";
            this.id3.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id31
            // 
            this.id31.Name = "id31";
            this.id31.Size = new System.Drawing.Size(142, 22);
            this.id31.Text = "アキラ";
            this.id31.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id14
            // 
            this.id14.Name = "id14";
            this.id14.Size = new System.Drawing.Size(142, 22);
            this.id14.Text = "エリオット";
            this.id14.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id6
            // 
            this.id6.Name = "id6";
            this.id6.Size = new System.Drawing.Size(142, 22);
            this.id6.Text = "ゲン・フー";
            this.id6.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id0
            // 
            this.id0.Name = "id0";
            this.id0.Size = new System.Drawing.Size(142, 22);
            this.id0.Text = "ザック";
            this.id0.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id41
            // 
            this.id41.Name = "id41";
            this.id41.Size = new System.Drawing.Size(142, 22);
            this.id41.Text = "ジャッキー";
            this.id41.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id2
            // 
            this.id2.Name = "id2";
            this.id2.Size = new System.Drawing.Size(142, 22);
            this.id2.Text = "ジャン・リー";
            this.id2.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id9
            // 
            this.id9.Name = "id9";
            this.id9.Size = new System.Drawing.Size(142, 22);
            this.id9.Text = "バース";
            this.id9.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id24
            // 
            this.id24.Name = "id24";
            this.id24.Size = new System.Drawing.Size(142, 22);
            this.id24.Text = "バイマン";
            this.id24.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id11
            // 
            this.id11.Name = "id11";
            this.id11.Size = new System.Drawing.Size(142, 22);
            this.id11.Text = "ハヤテ";
            this.id11.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id19
            // 
            this.id19.Name = "id19";
            this.id19.Size = new System.Drawing.Size(142, 22);
            this.id19.Text = "ブラッド・ウォン";
            this.id19.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id46
            // 
            this.id46.Name = "id46";
            this.id46.Size = new System.Drawing.Size(142, 22);
            this.id46.Text = "雷道";
            this.id46.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id29
            // 
            this.id29.Name = "id29";
            this.id29.Size = new System.Drawing.Size(142, 22);
            this.id29.Text = "リグ";
            this.id29.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id4
            // 
            this.id4.Name = "id4";
            this.id4.Size = new System.Drawing.Size(142, 22);
            this.id4.Text = "リュウ・ハヤブサ";
            this.id4.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id8
            // 
            this.id8.Name = "id8";
            this.id8.Size = new System.Drawing.Size(142, 22);
            this.id8.Text = "レオン";
            this.id8.Click += new System.EventHandler(this.AddCharacter);
            // 
            // catFemale
            // 
            this.catFemale.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.id13,
            this.id7,
            this.id5,
            this.id20,
            this.id10,
            this.id32,
            this.id1,
            this.id44,
            this.id33,
            this.id21,
            this.id43,
            this.id45,
            this.id42,
            this.id30,
            this.id39,
            this.id15,
            this.id40,
            this.id12,
            this.id16});
            this.catFemale.Name = "catFemale";
            this.catFemale.Size = new System.Drawing.Size(158, 22);
            this.catFemale.Text = "女 :";
            // 
            // id13
            // 
            this.id13.Name = "id13";
            this.id13.Size = new System.Drawing.Size(131, 22);
            this.id13.Text = "あやね";
            this.id13.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id7
            // 
            this.id7.Name = "id7";
            this.id7.Size = new System.Drawing.Size(131, 22);
            this.id7.Text = "エレナ";
            this.id7.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id5
            // 
            this.id5.Name = "id5";
            this.id5.Size = new System.Drawing.Size(131, 22);
            this.id5.Text = "かすみ";
            this.id5.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id20
            // 
            this.id20.Name = "id20";
            this.id20.Size = new System.Drawing.Size(131, 22);
            this.id20.Text = "クリスティ";
            this.id20.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id10
            // 
            this.id10.Name = "id10";
            this.id10.Size = new System.Drawing.Size(131, 22);
            this.id10.Text = "こころ";
            this.id10.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id32
            // 
            this.id32.Name = "id32";
            this.id32.Size = new System.Drawing.Size(131, 22);
            this.id32.Text = "サラ";
            this.id32.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id1
            // 
            this.id1.Name = "id1";
            this.id1.Size = new System.Drawing.Size(131, 22);
            this.id1.Text = "ティナ";
            this.id1.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id44
            // 
            this.id44.Name = "id44";
            this.id44.Size = new System.Drawing.Size(131, 22);
            this.id44.Text = "女天狗";
            this.id44.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id33
            // 
            this.id33.Name = "id33";
            this.id33.Size = new System.Drawing.Size(131, 22);
            this.id33.Text = "パイ・チェン";
            this.id33.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id21
            // 
            this.id21.Name = "id21";
            this.id21.Size = new System.Drawing.Size(131, 22);
            this.id21.Text = "ヒトミ";
            this.id21.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id43
            // 
            this.id43.Name = "id43";
            this.id43.Size = new System.Drawing.Size(131, 22);
            this.id43.Text = "PHASE-4";
            this.id43.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id45
            // 
            this.id45.Name = "id45";
            this.id45.Size = new System.Drawing.Size(131, 22);
            this.id45.Text = "ほのか";
            this.id45.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id42
            // 
            this.id42.Name = "id42";
            this.id42.Size = new System.Drawing.Size(131, 22);
            this.id42.Text = "マリー・ローズ";
            this.id42.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id30
            // 
            this.id30.Name = "id30";
            this.id30.Size = new System.Drawing.Size(131, 22);
            this.id30.Text = "ミラ";
            this.id30.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id39
            // 
            this.id39.Name = "id39";
            this.id39.Size = new System.Drawing.Size(131, 22);
            this.id39.Text = "紅葉";
            this.id39.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id15
            // 
            this.id15.Name = "id15";
            this.id15.Size = new System.Drawing.Size(131, 22);
            this.id15.Text = "リサ";
            this.id15.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id40
            // 
            this.id40.Name = "id40";
            this.id40.Size = new System.Drawing.Size(131, 22);
            this.id40.Text = "レイチェル";
            this.id40.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id12
            // 
            this.id12.Name = "id12";
            this.id12.Size = new System.Drawing.Size(131, 22);
            this.id12.Text = "レイファン";
            this.id12.Click += new System.EventHandler(this.AddCharacter);
            // 
            // id16
            // 
            this.id16.Name = "id16";
            this.id16.Size = new System.Drawing.Size(131, 22);
            this.id16.Text = "Alpha-152";
            this.id16.Click += new System.EventHandler(this.AddCharacter);
            // 
            // コピーToolStripMenuItem
            // 
            this.コピーToolStripMenuItem.Name = "コピーToolStripMenuItem";
            this.コピーToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.コピーToolStripMenuItem.Text = "選択データのコピー";
            this.コピーToolStripMenuItem.Click += new System.EventHandler(this.コピーToolStripMenuItem_Click);
            // 
            // リストの追加読み込みToolStripMenuItem
            // 
            this.リストの追加読み込みToolStripMenuItem.Name = "リストの追加読み込みToolStripMenuItem";
            this.リストの追加読み込みToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.リストの追加読み込みToolStripMenuItem.Text = "リストの追加読込";
            this.リストの追加読み込みToolStripMenuItem.Click += new System.EventHandler(this.リストの追加読み込みToolStripMenuItem_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.AddExtension = false;
            // 
            // openFileDialogState
            // 
            this.openFileDialogState.Filter = "*.lst;*.rst|*.lst;*.rst|元ツールのステートデータ|*.*";
            // 
            // saveFileDialogState
            // 
            this.saveFileDialogState.Filter = "*.lst|*.lst|元ツールのステートデータ|*.*";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.コピーCtrlCToolStripMenuItem,
            this.貼り付けCtrlVToolStripMenuItem,
            this.削除DeleteToolStripMenuItem,
            this.ClearPasteToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(280, 92);
            // 
            // コピーCtrlCToolStripMenuItem
            // 
            this.コピーCtrlCToolStripMenuItem.Name = "コピーCtrlCToolStripMenuItem";
            this.コピーCtrlCToolStripMenuItem.Size = new System.Drawing.Size(279, 22);
            this.コピーCtrlCToolStripMenuItem.Text = "コピー (Ctrl+C)";
            this.コピーCtrlCToolStripMenuItem.Click += new System.EventHandler(this.コピーCtrlCToolStripMenuItem_Click);
            // 
            // 貼り付けCtrlVToolStripMenuItem
            // 
            this.貼り付けCtrlVToolStripMenuItem.Name = "貼り付けCtrlVToolStripMenuItem";
            this.貼り付けCtrlVToolStripMenuItem.Size = new System.Drawing.Size(279, 22);
            this.貼り付けCtrlVToolStripMenuItem.Text = "貼り付け (Ctrl+V)";
            this.貼り付けCtrlVToolStripMenuItem.Click += new System.EventHandler(this.貼り付けCtrlVToolStripMenuItem_Click);
            // 
            // 削除DeleteToolStripMenuItem
            // 
            this.削除DeleteToolStripMenuItem.Name = "削除DeleteToolStripMenuItem";
            this.削除DeleteToolStripMenuItem.Size = new System.Drawing.Size(279, 22);
            this.削除DeleteToolStripMenuItem.Text = "削除 (Delete)";
            this.削除DeleteToolStripMenuItem.Click += new System.EventHandler(this.削除DeleteToolStripMenuItem_Click);
            // 
            // ClearPasteToolStripMenuItem
            // 
            this.ClearPasteToolStripMenuItem.Name = "ClearPasteToolStripMenuItem";
            this.ClearPasteToolStripMenuItem.Size = new System.Drawing.Size(279, 22);
            this.ClearPasteToolStripMenuItem.Text = "全削除＋貼り付け (服装選択後に Ctrl+V)";
            this.ClearPasteToolStripMenuItem.Click += new System.EventHandler(this.ClearPasteToolStripMenuItem_Click);
            // 
            // cmsSlot
            // 
            this.cmsSlot.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.cmsSlot.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.cmsSlot.Name = "cmsSlot";
            this.cmsSlot.ShowImageMargin = false;
            this.cmsSlot.Size = new System.Drawing.Size(36, 4);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(6, 43);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(612, 31);
            this.progressBar.TabIndex = 21;
            // 
            // btnCmpSave
            // 
            this.btnCmpSave.Enabled = false;
            this.btnCmpSave.Font = new System.Drawing.Font("メイリオ", 9F);
            this.btnCmpSave.Location = new System.Drawing.Point(288, 7);
            this.btnCmpSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCmpSave.Name = "btnCmpSave";
            this.btnCmpSave.Size = new System.Drawing.Size(88, 31);
            this.btnCmpSave.TabIndex = 22;
            this.btnCmpSave.Text = "圧縮保存";
            this.btnCmpSave.UseVisualStyleBackColor = true;
            this.btnCmpSave.Click += new System.EventHandler(this.btnCompSave_Click);
            // 
            // cbDAT
            // 
            this.cbDAT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDAT.FormattingEnabled = true;
            this.cbDAT.Location = new System.Drawing.Point(411, 9);
            this.cbDAT.Name = "cbDAT";
            this.cbDAT.Size = new System.Drawing.Size(121, 26);
            this.cbDAT.TabIndex = 23;
            this.cbDAT.SelectedIndexChanged += new System.EventHandler(this.cbDAT_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(308, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 18);
            this.label2.TabIndex = 24;
            this.label2.Text = "DAT";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // cmsAddDelCom
            // 
            this.cmsAddDelCom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smiAddComColumn,
            this.smiDelComColumn});
            this.cmsAddDelCom.Name = "cmsAddDelCom";
            this.cmsAddDelCom.Size = new System.Drawing.Size(174, 48);
            // 
            // smiAddComColumn
            // 
            this.smiAddComColumn.Name = "smiAddComColumn";
            this.smiAddComColumn.Size = new System.Drawing.Size(173, 22);
            this.smiAddComColumn.Text = "右にコメント列を追加";
            this.smiAddComColumn.Click += new System.EventHandler(this.smiAddComColumn_Click);
            // 
            // smiDelComColumn
            // 
            this.smiDelComColumn.Name = "smiDelComColumn";
            this.smiDelComColumn.Size = new System.Drawing.Size(173, 22);
            this.smiDelComColumn.Text = "コメント列を削除";
            this.smiDelComColumn.Click += new System.EventHandler(this.smiDelComColumn_Click);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 743);
            this.Controls.Add(this.cbDAT);
            this.Controls.Add(this.btnCmpSave);
            this.Controls.Add(this.tbSavePath);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.gbChars);
            this.Controls.Add(this.tbBCMVer);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnNewDLC);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnOpenBCM);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("メイリオ", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(640, 640);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DLC Tool ほげほげば～じょん 2015.11.20.1";
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.gbChars.ResumeLayout(false);
            this.gbChars.PerformLayout();
            this.gbHairs2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHStyles)).EndInit();
            this.gbFiles.ResumeLayout(false);
            this.gbFiles.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChars)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.cmsAddDelCom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenBCM;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.OpenFileDialog openFileDialogBCM;
        private System.Windows.Forms.SaveFileDialog saveFileDialogBCM;
        private System.Windows.Forms.Button btnNewDLC;
        private System.Windows.Forms.TextBox tbSavePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbBCMVer;
        private System.Windows.Forms.GroupBox gbChars;
        private System.Windows.Forms.Button btnCharsDelete;
        private System.Windows.Forms.Button btnCharsAdd;
        private System.Windows.Forms.GroupBox gbFiles;
        private System.Windows.Forms.Button btnFilesDelete;
        private System.Windows.Forms.Button btnFilesAdd;
        private System.Windows.Forms.ListBox lbFiles;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem catMale;
        private System.Windows.Forms.ToolStripMenuItem id31;
        private System.Windows.Forms.ToolStripMenuItem catFemale;
        private System.Windows.Forms.ToolStripMenuItem id9;
        private System.Windows.Forms.ToolStripMenuItem id24;
        private System.Windows.Forms.ToolStripMenuItem id19;
        private System.Windows.Forms.ToolStripMenuItem id3;
        private System.Windows.Forms.ToolStripMenuItem id14;
        private System.Windows.Forms.ToolStripMenuItem id6;
        private System.Windows.Forms.ToolStripMenuItem id4;
        private System.Windows.Forms.ToolStripMenuItem id11;
        private System.Windows.Forms.ToolStripMenuItem id41;
        private System.Windows.Forms.ToolStripMenuItem id2;
        private System.Windows.Forms.ToolStripMenuItem id8;
        private System.Windows.Forms.ToolStripMenuItem id46;
        private System.Windows.Forms.ToolStripMenuItem id29;
        private System.Windows.Forms.ToolStripMenuItem id0;
        private System.Windows.Forms.ToolStripMenuItem id13;
        private System.Windows.Forms.ToolStripMenuItem id20;
        private System.Windows.Forms.ToolStripMenuItem id7;
        private System.Windows.Forms.ToolStripMenuItem id21;
        private System.Windows.Forms.ToolStripMenuItem id45;
        private System.Windows.Forms.ToolStripMenuItem id5;
        private System.Windows.Forms.ToolStripMenuItem id10;
        private System.Windows.Forms.ToolStripMenuItem id12;
        private System.Windows.Forms.ToolStripMenuItem id15;
        private System.Windows.Forms.ToolStripMenuItem id16;
        private System.Windows.Forms.ToolStripMenuItem id42;
        private System.Windows.Forms.ToolStripMenuItem id30;
        private System.Windows.Forms.ToolStripMenuItem id39;
        private System.Windows.Forms.ToolStripMenuItem id44;
        private System.Windows.Forms.ToolStripMenuItem id33;
        private System.Windows.Forms.ToolStripMenuItem id43;
        private System.Windows.Forms.ToolStripMenuItem id40;
        private System.Windows.Forms.ToolStripMenuItem id32;
        private System.Windows.Forms.ToolStripMenuItem id1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Button btnOpenState;
        private System.Windows.Forms.Button btnSaveState;
        private System.Windows.Forms.OpenFileDialog openFileDialogState;
        private System.Windows.Forms.SaveFileDialog saveFileDialogState;
        private System.Windows.Forms.GroupBox gbHairs2;
        private System.Windows.Forms.DataGridView dgvHStyles;
        private System.Windows.Forms.Button btnHStylesDelete;
        private System.Windows.Forms.Button btnHStylesAdd;
        private System.Windows.Forms.DataGridView dgvChars;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmType;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmHair;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmFace;
        private System.Windows.Forms.ToolStripMenuItem リストの追加読み込みToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem コピーToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem コピーCtrlCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 貼り付けCtrlVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 削除DeleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ClearPasteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip cmsSlot;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnCmpSave;
        private System.Windows.Forms.ComboBox cbDAT;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbListPath;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbSaveListInDLC;
        private System.Windows.Forms.CheckBox cb4H;
        private System.Windows.Forms.CheckBox cb3H;
        private System.Windows.Forms.CheckBox cb2H;
        private System.Windows.Forms.CheckBox cb1H;
        private System.Windows.Forms.CheckBox cbTMCL;
        private System.Windows.Forms.CheckBox cbTMC;
        private System.Windows.Forms.CheckBox cbP;
        private System.Windows.Forms.CheckBox cbC;
        private System.Windows.Forms.ContextMenuStrip cmsAddDelCom;
        private System.Windows.Forms.ToolStripMenuItem smiAddComColumn;
        private System.Windows.Forms.ToolStripMenuItem smiDelComColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmName;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmCos;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmInner;
        private System.Windows.Forms.DataGridViewTextBoxColumn clmComment;
        private System.Windows.Forms.DataGridView dgvFiles;
        private System.Windows.Forms.DataGridViewTextBoxColumn FilePath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
    }
}

