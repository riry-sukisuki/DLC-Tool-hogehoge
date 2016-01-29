namespace DLC_Tool
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Threading.Tasks;
    using System.Windows.Forms;


    // ini 用
    using System.Text;
    using System.Runtime.InteropServices;




    public partial class MainForm : Form
    {
        // ini 用
        class IniFileHandler
        {
            [DllImport("KERNEL32.DLL")]
            public static extern uint
              GetPrivateProfileString(string lpAppName,
              string lpKeyName, string lpDefault,
              StringBuilder lpReturnedString, uint nSize,
              string lpFileName);

            [DllImport("KERNEL32.DLL",
                EntryPoint = "GetPrivateProfileStringA")]
            public static extern uint
              GetPrivateProfileStringByByteArray(string lpAppName,
              string lpKeyName, string lpDefault,
              byte[] lpReturnedString, uint nSize,
              string lpFileName);

            [DllImport("KERNEL32.DLL")]
            public static extern uint
              GetPrivateProfileInt(string lpAppName,
              string lpKeyName, int nDefault, string lpFileName);

            [DllImport("KERNEL32.DLL")]
            public static extern uint WritePrivateProfileString(
              string lpAppName,
              string lpKeyName,
              string lpString,
              string lpFileName);
        }
        public static string iniPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"DLC Tool.ini");
        public static void SaveIniString(string section, string key, string data)
        {
            try
            {
                SaveIniStringWithError(section, key, data);
            }
            catch { }
        }
        public static void SaveIniStringWithError(string section, string key, string data)
        {
            IniFileHandler.WritePrivateProfileString(section, key, data, iniPath);
        }
        public static string LoadIniString(string section, string key)
        {
            try
            {
                return LoadIniStringWithError(section, key);
            }
            catch
            {
                return "";
            }
        }
        public static string LoadIniStringWithError(string section, string key)
        {
            StringBuilder sb = new StringBuilder(1024);
            IniFileHandler.GetPrivateProfileString(section, key, "", sb, (uint)sb.Capacity, iniPath);
            return sb.ToString();
        }

        public static readonly string[] FileOrder = new string[12] { ".TMC", ".TMCL", ".---C", "1.--H", "1.--HL", "2.--H", "2.--HL", "3.--H", "3.--HL", "4.--H", "4.--HL", ".--P" };

        private static DLCData dlcData;

        private static bool newDlc;

        private System.Drawing.Point mouseDownPoint = System.Drawing.Point.Empty;

        private string clikedForm = "";
        private string setText = "";
        private int setIndex = -1;

        private bool DeleteKeyUp = true;
        private bool CKeyUp = true;
        private bool VKeyUp = true;
        private static string[] DATs;
        private static int DatSelectedIndex = 0;
        public static string DAT { get { return DATs[DatSelectedIndex]; } }

        private int[] dragStartIndexes = null;
        private int[] dragPrevIndexes = null;
        private int dragHoldRelIndex = -1;

        private bool NeedShowChar = true;


        // 60 秒ごとにリストファイルを保存
        // 異常終了時のための安全装置なので複雑な処理は行わないようにする。
        // ハンドルされない例外が出ても勝手にタイマーが止まったりはしないので注意
        public class FormsTimerTest
        {
            public Timer timer;
            public void Run()
            {
                timer = new Timer();
                timer.Tick += new EventHandler(MyClock);
                timer.Interval = 60000;
                timer.Enabled = true; // timer.Start()と同じ

                //Application.Run(); // メッセージ・ループを開始
            }

            public void Stop()
            {
                timer.Enabled = false;
            }

            public void MyClock(object sender, EventArgs e)
            {
                SaveCurrentState();
            }

            public static void SaveCurrentState()
            {

                if (dlcData != null)
                {
                    try
                    {
                        string temp = LoadIniStringWithError("Text", "tbListPath");

                        SaveIniStringWithError("Text", "tbListPath", temp);
                        // 書き込みテスト

                        dlcData.SavePath = tbSavePath_Text_static;
                        // dlcData 編集中にこのイベントが発生すると何が起こるか分からない
                        // エラーが起こっても current.lst 本体は傷つかない
                        try
                        {
                            var pt = Path.GetDirectoryName(Application.ExecutablePath);
                            Program.SaveState(dlcData, Path.Combine(pt, @"current.lst"), Path.Combine(pt, @"current.lst.temp"));

                        }
                        catch
                        {

                            SaveIniStringWithError("Text", "tbListPath", temp);


                            // これのエラーはユーザーまで届ける
                            return;
                        }
                    }
                    catch
                    {
                        return;
                    }


                    // 以下のエラーはユーザーまで届ける
                    SaveIniStringWithError("Text", "tbListPath", tbListPath_Text_static);
                    string bcmName = "";
                    if (!newDlc)
                    {
                        try
                        {
                            if (File.Exists(tbSavePath_Text_static))
                            {
                                bcmName = Path.GetFileName(tbSavePath_Text_static);
                            }
                        }
                        catch { }
                    }
                    SaveIniStringWithError("Text", "BCMName", bcmName);
                }

            }
        }


        private FormsTimerTest TimerForSave;
        public MainForm()
        {

            InitializeComponent();
            setVersion();
            SetCharNames();
            TranslateInitialUI(true);
            SetDATList();
            //dgvChars.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            string[] cmds = System.Environment.GetCommandLineArgs();

            if (cmds.Length > 1 && System.Text.RegularExpressions.Regex.IsMatch(cmds[1], @"\.[lr]st$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                OpenStateFile(cmds[1]);
            }

            else
            {
                string curList = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"current.lst");
                try
                {
                    if (File.Exists(curList))
                    {
                        var tbListPathText = LoadIniString("Text", "tbListPath");
                        OpenStateFile(curList, tbListPathText + ".lst");
                        tbListPath.Text = tbListPathText;


                        tbListPath.Select(tbListPath.Text.Length, 0);
                        tbListPath.ScrollToCaret();


                        string bcmName = LoadIniString("Text", "BCMName");
                        if (bcmName != "")
                        {
                            OpenFile(Path.Combine(tbSavePath.Text, bcmName));
                        }


                        setEgvCharsSlotColor();
                        setEgvCharsNameColor();
                        setEgvCharsTextsColor();
                    }
                }
                catch
                {
                    try
                    {
                        File.Delete(curList);
                    }
                    catch { }
                }

            }


            // 終了時のイベントハンドラを登録
            //ApplicationExitイベントハンドラを追加
            Application.ApplicationExit += new EventHandler(Application_ApplicationExit);

            // 異常終了時は単にタイマーを止める
            Application.ThreadException +=
                new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            // タイマー開始
            TimerForSave = new FormsTimerTest();
            TimerForSave.Run();
        }

        //ApplicationExitイベントハンドラ
        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            //ApplicationExitイベントハンドラを削除
            Application.ApplicationExit -= new EventHandler(Application_ApplicationExit);

            // タイマーを止める
            TimerForSave.Stop();


            //保存を行う

            if (dlcData != null)
            {
                FormsTimerTest.SaveCurrentState();
            }
        }

        // 異常終了時は単にタイマーを止める

        //ThreadExceptionイベントハンドラ
        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {

            // タイマーを止める
            TimerForSave.Stop();

            try
            {
                //エラーメッセージを表示する
                MessageBox.Show(e.Exception.Message, Program.dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //アプリケーションを終了する
                Application.Exit();
            }
        }

        private void setVersion()
        {

            string ver = System.Reflection.Assembly.GetExecutingAssembly().FullName;
            string pre = "Version=";
            int start = ver.IndexOf(pre) + pre.Length;
            ver = ver.Substring(start);
            int stop = ver.IndexOf(",");
            ver = ver.Substring(0, stop);
            if (int.Parse(ver.Substring(0, ver.IndexOf("."))) > 2000)
            {
                this.Text += " 開発版 " + ver;
            }
            else
            {
                this.Text += " " + ver.Substring(0, ver.Length - 2);
            }
        }

        private void ClearCharsUI()
        {
            dgvHStyles.Rows.Clear();
            dgvFiles.Rows.Clear();
        }

        private void ClearMainUI()
        {
            dgvChars.Rows.Clear();
            while (dgvChars.Columns.Count > 4)
            {
                dgvChars.Columns.RemoveAt(dgvChars.Columns.Count - 1);
            }

            btnCmpSave.Text = Program.dicLanguage["SaveCompressedDLC"];

            ClearCharsUI();
            tbBCMVer.Text = string.Empty;
            btnSave.Text = "Save";
            btnCmpSave.Enabled = btnSave.Enabled = false;
            btnSaveState.Enabled = false;
            btnCharsAdd.Enabled = false;
            btnCharsDelete.Enabled = false;
            btnHStylesAdd.Enabled = false;
            btnHStylesDelete.Enabled = false;
            btnFilesAdd.Enabled = false;
            btnFilesDelete.Enabled = false;
            dgvChars.BackgroundColor = System.Drawing.SystemColors.Window;
            //dgvChars.DefaultCellStyle.BackColor = System.Drawing.SystemColors.Window;
            //dgvChars.DefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlText;

            dgvChars.Columns[0].DefaultCellStyle.BackColor = System.Drawing.SystemColors.Window;
            dgvChars.Columns[0].DefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlText;
            dgvChars.Columns[1].DefaultCellStyle.BackColor = System.Drawing.SystemColors.Window;
            dgvChars.Columns[1].DefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlText;
            dgvChars.Columns[2].DefaultCellStyle.BackColor = System.Drawing.SystemColors.Window;
            dgvChars.Columns[2].DefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlText;


            cbC.CheckState = cbP.CheckState = cbTMC.CheckState = cbTMCL.CheckState = cb1H.CheckState = cb2H.CheckState = cb3H.CheckState = cb4H.CheckState = CheckState.Unchecked;
            cbC.Enabled = cbP.Enabled = cbTMC.Enabled = cbTMCL.Enabled = cb1H.Enabled = cb2H.Enabled = cb3H.Enabled = cb4H.Enabled = false;
            tbListPath.Text = "";
        }

        private void btnOpenBCM_Click(object sender, EventArgs e)
        {
            var pt = GetOwnOFolderrExistingParent(LoadIniString("InitialDirectory", "BCM"));
            if (pt != "") openFileDialogBCM.InitialDirectory = pt;
            openFileDialogBCM.FileName = Path.GetFileName(saveFileDialogBCM.FileName);
            if (openFileDialogBCM.ShowDialog() == DialogResult.OK)
            {
                OpenFile(openFileDialogBCM.FileName);

                btnSaveState.Enabled = true;

                //btnCmpSave.Enabled = false;
                //btnCmpSave.Enabled = true;
                //btnCmpSave.Text = Program.dicLanguage["ExtractFiles"];

                SaveIniString("InitialDirectory", "BCM", Path.GetDirectoryName(openFileDialogBCM.FileName));


            }
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            Task.Factory.StartNew(() =>
            {
                this.Invoke((Action)delegate
                {
                    if (Path.GetExtension(fileNames[0]).ToLower() == ".bcm")
                    {
                        OpenFile(fileNames[0]);
                    }

                    if (newDlc)
                    {
                        AddFiles(fileNames, false);
                    }
                });
            });
        }

        private void OpenFile(string fileName)
        {
            try
            {
                newDlc = false;
                ClearMainUI();
                tbSavePath.Text = fileName;
                //dlcData = Program.OpenBCM(fileName);
                //通常のオープンファイルもこっちにしちゃおう
                dlcData = Program.OpenBCM_超原始的修正(fileName);


                //btnSave.Text = Program.dicLanguage["SaveBCM"];
                setBtnSave();

                btnCmpSave.Enabled = true;
                btnCmpSave.Text = Program.dicLanguage["ExtractFiles"];


                //btnCmpSave.Enabled = 
                btnSave.Enabled = true;
                btnHStylesAdd.Enabled = true;
                //clmCos.ReadOnly = true;
                clmInner.ReadOnly = true;

                dgvChars.BackgroundColor = System.Drawing.SystemColors.MenuBar;
                //dgvChars.DefaultCellStyle.BackColor = System.Drawing.SystemColors.MenuBar;
                //dgvChars.DefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
                dgvChars.Columns[0].DefaultCellStyle.BackColor = System.Drawing.SystemColors.MenuBar;
                dgvChars.Columns[0].DefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
                dgvChars.Columns[1].DefaultCellStyle.BackColor = System.Drawing.SystemColors.MenuBar;
                dgvChars.Columns[1].DefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
                dgvChars.Columns[2].DefaultCellStyle.BackColor = System.Drawing.SystemColors.MenuBar;
                dgvChars.Columns[2].DefaultCellStyle.ForeColor = System.Drawing.SystemColors.ControlDarkDark;

                tbBCMVer.Text = dlcData.BcmVer.ToString();
                dlcData.SavePath = fileName;

                cbSaveListInDLC.Checked = false;
                cbSaveListInDLC.Enabled = false;

                for (int i = 0; i < dlcData.Chars.Count; i++)
                {
                    dgvChars.Rows.Add();
                    dgvChars.Rows[i].Cells[0].Value = GetCharNamesJpn(dlcData.Chars[i].ID);// Program.CharNamesJpn[dlcData.Chars[i].ID];
                    dgvChars.Rows[i].Cells[1].Value = dlcData.Chars[i].CostumeSlot.ToString();
                    dgvChars.Rows[i].Cells[2].Value = dlcData.Chars[i].AddTexsCount.ToString();
                    //dgvChars.Rows[i].Cells[3].Value = dlcData.Chars[i].Comment;
                    showComment(i);
                }
                dgvChars.Rows[0].Selected = true;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool AddFiles(string[] fileNames, bool testMode)
        {
            return AddFiles(fileNames, testMode, true);
        }
        private bool AddFiles(string[] fileNames, bool testMode, bool drawWindow)
        {
            if (dgvChars.SelectedRows.Count != 1) return false;

            bool add = false;
            bool テクスチャ含む = false;
            try
            {
                if (fileNames.Length <= 0)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            foreach (string fileName in fileNames)
            {
                //                if (dlcData.Chars[dgvChars.SelectedRows[0].Index].Female)
                //                {
                for (int i = 0; i < FileOrder.Length; i++)
                {
                    if (fileName.EndsWith(FileOrder[i], true, null))
                    {
                        add = true;
                        if (!testMode)
                        {
                            dlcData.Chars[dgvChars.SelectedRows[0].Index].Files[i] = fileName;
                            if ((!テクスチャ含む) && System.Text.RegularExpressions.Regex.IsMatch(fileName, @"\.--HL?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            {
                                テクスチャ含む = true;
                            }
                        }
                        break;
                    }
                }
                //                }
                //                else
                //                {
                //                    for (int i = 0; i < 3; i++)
                //                    {
                //                        if (fileName.EndsWith(FileOrder[i], true, null))
                //                        {
                //                            dlcData.Chars[dgvChars.SelectedRows[0].Index].Files[i] = fileName;
                //                            break;
                //                        }
                //                    }
                //
                //                    if (fileName.EndsWith(FileOrder[11], true, null))
                //                    {
                //                        dlcData.Chars[dgvChars.SelectedRows[0].Index].Files[11] = fileName;
                //                    }
                //                }
            }

            if (テクスチャ含む)
            {
                dlcData.Chars[dgvChars.SelectedRows[0].Index].AddTexsCount = getAvailableTextsCount(dlcData.Chars[dgvChars.SelectedRows[0].Index]);

                dgvChars.Rows[dgvChars.SelectedRows[0].Index].Cells[1].Value = dlcData.Chars[dgvChars.SelectedRows[0].Index].CostumeSlot.ToString();
                dgvChars.Rows[dgvChars.SelectedRows[0].Index].Cells[2].Value = dlcData.Chars[dgvChars.SelectedRows[0].Index].AddTexsCount.ToString();


                // setEgvCharsSlotColor(); これは流石に不要

            }

            // ファイルチェックを兼ねてテクスチャを含まなくてもやる
            if (drawWindow && !testMode)
            {
                setEgvCharsTextsColor();
                setEgvCharsNameColor();

                ShowFiles(dgvChars.SelectedRows[0].Index);

                // 同じ名前のファイル名は二つ同時には登録できないという前提（今の実装では成立する）で、
                // 追加されたファイルを選択状態にする
                for (int i = 0; i < dgvFiles.Rows.Count; i++)
                {
                    string path = dgvFiles.Rows[i].Cells[0].Value.ToString();
                    if (Array.IndexOf(fileNames, path) >= 0)
                    {
                        dgvFiles.Rows[i].Selected = true;
                    }
                }
            }

            return add;
        }

        private bool dgvChars_SelectionChanged_RepairingSelection = false;
        private void dgvChars_SelectionChanged(object sender, EventArgs e)
        {

            if (dgvChars_SelectionChanged_RepairingSelection) return;



            if (dgvChars_MouseDown_LeftShiftPreClickedIndex >= 0)
            {
                //MessageBox.Show("a");

                var temp = NeedShowChar;
                NeedShowChar = false;
                dgvChars_SelectionChanged_RepairingSelection = true;
                var p = dgvChars.PointToClient(Cursor.Position);
                DataGridView.HitTestInfo hit = dgvChars.HitTest(p.X, p.Y);
                int lbound, ubound;
                if (dgvChars_MouseDown_LeftShiftPreClickedIndex >= hit.RowIndex)
                {
                    ubound = dgvChars_MouseDown_LeftShiftPreClickedIndex;
                    lbound = hit.RowIndex;
                }
                else
                {
                    lbound = dgvChars_MouseDown_LeftShiftPreClickedIndex;
                    ubound = hit.RowIndex;
                }


                for (int i = 0; i < dgvChars.Rows.Count; i++)
                {
                    dgvChars.Rows[i].Selected = (Array.IndexOf(dragStartIndexes, i) >= 0 || (lbound <= i && i <= ubound));
                }



                dgvChars_SelectionChanged_RepairingSelection = false;
                NeedShowChar = temp;
                dgvChars_MouseDown_LeftShiftPreClickedIndex = -1;
                dragStartIndexes = null;
            }


            // dragStartIndexes が null で無くて、今選択されているものがただひとつで、しかもその中に含まれているか手動ソート中なら、
            // その選択の変更は望まないものなので、dragStartIndexes の状態に戻す
            if (dragStartIndexes != null && dgvChars.SelectedRows.Count == 1 && Array.IndexOf(dragStartIndexes, dgvChars.SelectedRows[0].Index) >= 0 && mouseDownPoint != System.Drawing.Point.Empty)
            {
                dgvChars_SelectionChanged_RepairingSelection = true;

                var temp = NeedShowChar;
                NeedShowChar = false;

                int[] target;
                //if(dragPrevIndexes != null)
                {
                    //    target = dragPrevIndexes;
                }
                //else
                {
                    target = dragStartIndexes;
                }

                dgvChars.ClearSelection();
                for (int i = 0; i < target.Length; i++)
                {
                    dgvChars.Rows[target[i]].Selected = true;
                }


                dgvChars_SelectionChanged_RepairingSelection = false;
                NeedShowChar = temp;
            }



            if (dgvChars.SelectedRows.Count == 1)
            {
                //MessageBox.Show("dgvChars_SelectionChanged");
                ShowCharacter(dgvChars.SelectedRows[0].Index);
            }
            else
            {
                ClearCharsUI();
                cbC.CheckState = cbP.CheckState = cbTMC.CheckState = cbTMCL.CheckState = cb1H.CheckState = cb2H.CheckState = cb3H.CheckState = cb4H.CheckState = CheckState.Unchecked;
                cbC.Enabled = cbP.Enabled = cbTMC.Enabled = cbTMCL.Enabled = cb1H.Enabled = cb2H.Enabled = cb3H.Enabled = cb4H.Enabled = false;
                btnFilesAdd.Enabled = false;
                btnHStylesAdd.Enabled = false;
                btnFilesDelete.Enabled = false;
                btnHStylesDelete.Enabled = false;

                //MessageBox.Show("1");
            }
        }

        private void ShowCharacter(int idx)
        {
            if (!NeedShowChar)
            {
                return;
            }

            ClearCharsUI();

            ShowHairstyles(idx);
            if (newDlc)
            {
                //clmCos.ReadOnly = false;
                clmInner.ReadOnly = false;
                btnCharsDelete.Enabled = true;
                btnFilesAdd.Enabled = true;
                ShowFiles(idx);
            }

        }

        private void ShowHairstyles(int idx)
        {
            if (dgvHStyles.Rows.Count > 0)
            {
                dgvHStyles.Rows.Clear();
            }

            for (int i = 0; i < dlcData.Chars[idx].HStyles.Count; i++)
            {
                dgvHStyles.Rows.Add();
                dgvHStyles.Rows[i].Cells[0].Value = (char)(65 + i) + " " + Program.dicLanguage["Type"];
                dgvHStyles.Rows[i].Cells[1].Value = dlcData.Chars[idx].HStyles[i].Hair.ToString();
                dgvHStyles.Rows[i].Cells[2].Value = dlcData.Chars[idx].HStyles[i].Face.ToString();
            }

            if (dlcData.Chars[idx].HStyles.Count < 8)
            {
                btnHStylesAdd.Enabled = true;
            }
            else
            {
                btnHStylesAdd.Enabled = false;
            }

            if (dgvHStyles.Rows.Count > 0)
            {
                dgvHStyles.Rows[0].Selected = false;
                btnHStylesDelete.Enabled = false;
            }

            if (dgvHStyles.Rows.Count <= 1)
            {
                btnHStylesDelete.Enabled = false;
            }
        }

        private void RedrawFiles(int charIndex, bool chkLink)
        {

            if (dgvChars.SelectedRows.Count != 1)
            {


                return;
            }

            // 表示するファイル名とファイルの存在を取得
            System.Collections.Generic.List<string> paths = new System.Collections.Generic.List<string>();
            System.Collections.Generic.List<int> fexists = new System.Collections.Generic.List<int>();
            bool fileok = true;
            for (int i = 0; i < dlcData.Chars[charIndex].Files.Length; i++)
            {
                string fileName = dlcData.Chars[charIndex].Files[i];
                if (fileName != null)
                {
                    paths.Add(fileName);
                    if (chkLink)
                    {
                        try
                        {
                            if (File.Exists(fileName))
                            {
                                fexists.Add(2);
                            }
                            else
                            {
                                fexists.Add(1);
                                if (i <= 2 || 11 <= i)
                                {
                                    fileok = false;
                                }
                            }
                        }
                        catch
                        {
                            fexists.Add(-1);
                            fileok = false;
                        }
                    }
                }
                else if (i <= 2 || 11 <= i)
                {
                    fileok = false;
                }
            }

            // 画面表示に使う長さと最大文字長を取得
            var g = dgvFiles.CreateGraphics();
            int[] showlen = new int[paths.Count];
            float[] lens = new float[paths.Count];
            float maxlen = 0;
            for (int i = 0; i < paths.Count; i++)
            {
                showlen[i] = -1;
                lens[i] = (1000f / 1024f) * g.MeasureString(paths[i], dgvFiles.Font).Width;
                // なぜこうするとうまくいくのかは不明
                if (lens[i] > maxlen)
                {
                    maxlen = lens[i];
                }
            }
            for (int i = 0; i < paths.Count; i++)
            {
                // 既に設定済みならスキップ
                if (showlen[i] >= 0)
                {
                    continue;
                }

                // 親フォルダを取得
                string ppath = Path.GetDirectoryName(paths[i]);

                // 同じ親フォルダを持つファイルパスの中で表示が一番長いものの長さを取得
                float max = lens[i];
                for (int j = i + 1; j < paths.Count; j++)
                {
                    if (ppath == Path.GetDirectoryName(paths[j]) && lens[j] > max)
                    {
                        max = lens[j];
                    }
                }

                // 最大との差分だけ右側にパティングすればいいのだと思う。
                for (int j = i; j < paths.Count; j++)
                {
                    if (ppath == Path.GetDirectoryName(paths[j]))
                    {
                        showlen[j] = (int)(max + 0.5);
                    }
                }

            }

            // 一番長い文字列も入るように dgv を右に拡張
            var loc = dgvFiles.Location;
            var siz = dgvFiles.Size;
            int Width0 = dgvFiles.Size.Width;
            int X0 = loc.X;
            loc.X += Width0 - (int)(maxlen + 10);
            if (loc.X > 0)
            {
                loc.X = 0;
            }
            siz.Width += X0 - loc.X;
            dgvFiles.Location = loc;
            dgvFiles.Size = siz;

            chkLink = (chkLink || dgvFiles.Rows.Count != paths.Count);

            if (chkLink && dgvFiles.Rows.Count > 0)
            {
                dgvFiles.Rows.Clear();
            }

            // 右詰めで表示
            for (int i = 0; i < paths.Count; i++)
            {
                if (chkLink)
                {
                    dgvFiles.Rows.Add();
                }
                var cell = dgvFiles.Rows[i].Cells[0];
                if (chkLink)
                {
                    try
                    {

                        if (fexists[i] >= 2)
                        {
                            cell.Style.BackColor = System.Drawing.Color.Empty;
                            cell.Style.SelectionBackColor = System.Drawing.Color.Empty;
                        }
                        else
                        {
                            cell.Style.BackColor = System.Drawing.Color.LightGray;
                            cell.Style.SelectionBackColor = System.Drawing.Color.DimGray;
                        }
                    }
                    catch { }
                }


                cell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;

                cell.Style.Padding = new Padding(siz.Width - showlen[i], 0, 0, 0);

                cell.Value = paths[i];
            }


            if (chkLink)
            {
                if (fileok)
                {
                    dgvChars.SelectedRows[0].Cells[0].Style.BackColor = System.Drawing.Color.Empty;
                    dgvChars.SelectedRows[0].Cells[0].Style.SelectionBackColor = System.Drawing.Color.Empty;
                }
                else
                {
                    dgvChars.SelectedRows[0].Cells[0].Style.BackColor = System.Drawing.Color.LightGray;
                    dgvChars.SelectedRows[0].Cells[0].Style.SelectionBackColor = System.Drawing.Color.DimGray;
                }
            }


        }

        private void ShowFiles(int charIndex)
        {

            setFileCheckbox();
            RedrawFiles(charIndex, true);



            // 選択を解除
            for (int i = 0; i < dgvFiles.Rows.Count; i++)
            {
                dgvFiles.Rows[i].Selected = false;
            }

            btnFilesDelete.Enabled = false;
        }




        private void SaveDLC(bool Compression)
        {

            try
            {
                /* この仕様はリストを DLC に保存機能を使わない人にとっては迷惑極まりないので廃止
                if (cbSaveListInDLC.Enabled && cbSaveListInDLC.Checked)
                {
                    string name;
                    try
                    {
                        name = Path.GetFileName(tbListPath.Text);
                    }
                    catch
                    {
                        name = "";
                    }
                    bool GoodName = (name != "" && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0);
                    if (!GoodName)
                    {
                        throw new Exception(Program.dicLanguage["InputCorrectListFileName"]);
                    }
                }
                */



                if (dgvChars.Rows.Count == 0)
                {
                    throw new Exception(Program.dicLanguage["AddCharacter"]);
                }

                string dlcName = "";
                if (newDlc)
                {
                    bool direxists = false;
                    try
                    {
                        if (tbSavePath.Text != string.Empty)
                        {
                            direxists = Directory.Exists(Path.GetDirectoryName(tbSavePath.Text));
                        }
                    }
                    catch { }
                    if (!direxists)
                    {

                        var pt = LoadIniString("InitialDirectory", "BCM"); // ここも BMC で。
                        if (pt != "") saveFileDialog.InitialDirectory = GetOwnOFolderrExistingParent((Path.GetDirectoryName(pt))); // ただし親を見る。
                        saveFileDialog.FileName = Path.GetFileName(openFileDialog.FileName);

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            System.Text.RegularExpressions.Regex regex_doa = new System.Text.RegularExpressions.Regex(@"^(.*\\([^\\]+))\\\2\.bcm$",
                                System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                            if (regex_doa.IsMatch(saveFileDialog.FileName))
                            {
                                tbSavePath.Text = regex_doa.Replace(saveFileDialog.FileName, "$1");
                                SaveIniString("InitialDirectory", "BCM", Path.GetDirectoryName(saveFileDialog.FileName));
                            }
                            else
                            {
                                tbSavePath.Text = saveFileDialog.FileName;
                                SaveIniString("InitialDirectory", "BCM", saveFileDialog.FileName);
                            }
                        }
                        else
                        {
                            throw new Exception(Program.dicLanguage["SetDestinationToSave"]);
                        }

                    }

                    if (!DirectoryIsPureDLC(tbSavePath.Text))
                    {
                        MessageBox.Show(Program.dicLanguage["NotPureDLCFolder"], Program.dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbSavePath_TextChanged(null, null);//念のため
                        return;
                    }

                    dlcName = Path.GetFileNameWithoutExtension(tbSavePath.Text);


                    // 保存用の dlcData 変数
                    DLCData dlcData4Save = new DLCData();
                    dlcData4Save.SavePath = dlcData.SavePath;
                    dlcData4Save.BcmVer = dlcData.BcmVer;
                    dlcData4Save.skipRead = dlcData.skipRead;

                    // 問題のもの以外をコピー
                    Program.SlotTable<int> slotCount = new Program.SlotTable<int>(0);
                    for (int i = 0; i < dgvChars.Rows.Count; i++)
                    {
                        slotCount[dlcData.Chars[i]]++;
                    }
                    //int k = 0;
                    for (int i = 0; i < dlcData.Chars.Count; i++)
                    {
                        if (CheckCharFile(dlcData.Chars[i]) && slotCount[dlcData.Chars[i]] == 1 && dlcData.Chars[i].AddTexsCount <= getAvailableTextsCount(dlcData.Chars[i]))
                        {
                            dlcData4Save.Chars.Add(dlcData.Chars[i]);
                        }
                    }

                    if (dlcData4Save.Chars.Count > 0)
                    {
                        bool comp = Compression;// cbComp.Checked;

                        // DLC Tool 1.1 より
                        if (!Program.SaveDLC(dlcData4Save, tbSavePath.Text, dlcName, comp))
                        {
                            tbSavePath_TextChanged(null, null);//念のため
                            return;
                        }



                        /*
                        Directory.CreateDirectory(tbSavePath.Text + @"\data\");
                        Program.SaveBCM(dlcData4Save, tbSavePath.Text + @"\" + dlcName + ".bcm", dlcName);
                        var nameIndexes = Program.SaveBIN(dlcData4Save, tbSavePath.Text + @"\data\" + dlcName + ".bin", dlcName);
                        var fileSizes = Program.SaveLNK(dlcData4Save, tbSavePath.Text + @"\data\" + dlcName + ".lnk", nameIndexes.Count);
                        Program.SaveBLP(tbSavePath.Text + @"\data\" + dlcName + ".blp", nameIndexes, fileSizes);
                        */


                        string message;
                        if (dlcData4Save.Chars.Count == dlcData.Chars.Count)
                        {
                            message = Program.dicLanguage["SavedDLC"];
                        }
                        else
                        {
                            message = Program.dicLanguage["SavedPartialDLC"];
                        }

                        string path = tbSavePath.Text + @"\" + Path.GetFileName(tbListPath.Text) + ".lst";
                        if (cbSaveListInDLC.Enabled && cbSaveListInDLC.Checked)
                        {

                            string name;
                            try
                            {
                                name = Path.GetFileName(tbListPath.Text);
                            }
                            catch
                            {
                                name = "";
                            }
                            bool GoodName = (name != "" && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0);
                            if (GoodName)
                            {

                                dlcData.SavePath = tbSavePath.Text;


                                Program.SaveState(dlcData, path); //dlcData4Save とどっちか迷うところだけど、バックアップ機能を兼ねると思えばオリジナルのほうで良いのでは。
                            }
                        }
                        else if (cbSaveListInDLC.Enabled && !cbSaveListInDLC.Checked)
                        {
                            // ファイルが存在していてそれがユーザーが保存したもので無ければ消す
                            try
                            {
                                if (path != tbListPath.Text + ".lst" && File.Exists(path))
                                {
                                    File.Delete(path);
                                }
                            }
                            catch { }
                        }

                        // ショートカットを探す
                        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\\DLC\\\d+$");
                        string shortcut = "";

                        // ゲームフォルダ内のショートカットを探す
                        if (shortcut == "" && regex.IsMatch(tbSavePath.Text))
                        {
                            shortcut = regex.Replace(tbSavePath.Text, @"\game");
                            if (!OpenWithShortcut(shortcut, "", true))
                            {
                                shortcut = "";
                            }
                        }


                        // Applications フォルダ内のショートカットを探す
                        if (shortcut == "")
                        {
                            shortcut = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"Applications\game");
                            if (!OpenWithShortcut(shortcut, "", true))
                            {
                                shortcut = "";
                            }
                        }

                        // ゲームそのものを探す
                        string DOA5EXE = "";
                        if (shortcut == "" && regex.IsMatch(tbSavePath.Text))
                        {
                            DOA5EXE = regex.Replace(tbSavePath.Text, @"\game.exe");
                            if (!File.Exists(DOA5EXE))
                            {
                                DOA5EXE = "";
                            }
                        }


                        if (shortcut == "" && DOA5EXE == "")
                        {
                            MessageBox.Show(message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            if (MessageBox.Show(message + "\n\n" + Program.dicLanguage["DoYouStartDOA5"], "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                //System.Diagnostics.Process.Start( "\"" + DOA5EXE + "\"");

                                if (shortcut != "")
                                {
                                    OpenWithShortcut(shortcut, "", false);
                                }
                                else
                                {
                                    // 念のためカレントディレクトリを動かしておく
                                    string sCD = System.Environment.CurrentDirectory;
                                    System.Environment.CurrentDirectory = Path.GetDirectoryName(DOA5EXE); ;//System.IO.Path.GetDirectoryName(fileName);
                                    System.Diagnostics.Process.Start("\"" + DOA5EXE + "\"");
                                    System.Environment.CurrentDirectory = sCD;
                                }
                            }
                        }

                        /*
                        // 色んな所からゲームを探す
                        string DOA5EXE = "";
                        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\\DLC\\\d+$");

                        // ゲームフォルダ内のショートカットを探す
                        if (DOA5EXE == "" && regex.IsMatch(tbSavePath.Text))
                        {
                            DOA5EXE = regex.Replace(tbSavePath.Text, @"\default.lnk");
                            if (!File.Exists(DOA5EXE))
                            {
                                DOA5EXE = "";
                            }
                        }

                        // ツールフォルダ内のショートカットを探す
                        if (DOA5EXE == "")
                        {
                            string defaultLNK = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"default.lnk");
                            if (DOA5EXE == "" && File.Exists(defaultLNK))
                            {
                                DOA5EXE = defaultLNK;
                            }
                        }
                        
                        // ゲームそのものを探す
                        if (DOA5EXE == "" && regex.IsMatch(tbSavePath.Text))
                        {
                            DOA5EXE = regex.Replace(tbSavePath.Text, @"\game.exe");
                            if (!File.Exists(DOA5EXE))
                            {
                                DOA5EXE = "";
                            }
                        }

                        if (DOA5EXE == "")
                        {
                            MessageBox.Show(message, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            if (MessageBox.Show(message + "\n\n" + Program.dicLanguage["DoYouStartDOA5"], "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                //System.Diagnostics.Process.Start( "\"" + DOA5EXE + "\"");

                                // 念のためカレントディレクトリを動かしておく
                                string sCD = System.Environment.CurrentDirectory;
                                System.Environment.CurrentDirectory = Path.GetDirectoryName(DOA5EXE); ;//System.IO.Path.GetDirectoryName(fileName);
                                System.Diagnostics.Process.Start("\"" + DOA5EXE + "\"");
                                System.Environment.CurrentDirectory = sCD;
                            }
                        }
                        */
                    }
                    else
                    {
                        //MessageBox.Show(Program.dicLanguage["NotSavedDLC"], "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        throw new Exception(Program.dicLanguage["NotSavedDLC"]);
                    }
                }
                else
                {

                    bool direxists = false;
                    try
                    {
                        if (tbSavePath.Text != string.Empty)
                        {
                            direxists = Directory.Exists(tbSavePath.Text);
                        }
                    }
                    catch { }

                    if (!direxists || dlcData.skipRead > 0)
                    {
                        if (dlcData.skipRead > 0)
                        {
                            MessageBox.Show(Program.dicLanguage["OverwritingKillsSkipedItem"], Program.dicLanguage["Notice"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }


                        var pt = GetOwnOFolderrExistingParent(LoadIniString("InitialDirectory", "BCM"));
                        if (pt != "") saveFileDialogBCM.InitialDirectory = pt;
                        saveFileDialogBCM.FileName = Path.GetFileName(openFileDialogBCM.FileName);
                        if (saveFileDialogBCM.ShowDialog() == DialogResult.OK)
                        {
                            SaveIniString("InitialDirectory", "BCM", Path.GetDirectoryName(saveFileDialogBCM.FileName));
                            tbSavePath.Text = saveFileDialogBCM.FileName;
                        }
                        else
                        {
                            throw new Exception(Program.dicLanguage["SetDestinationToSave"]);
                        }
                    }


                    if (!DirectoryIsPureDLC(Path.GetDirectoryName(tbSavePath.Text)))
                    {
                        MessageBox.Show(Program.dicLanguage["NotPureDLCFolder"], Program.dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbSavePath_TextChanged(null, null);//念のため
                        return;
                    }

                    dlcName = Path.GetFileNameWithoutExtension(tbSavePath.Text);

                    Program.SaveBCM(dlcData, tbSavePath.Text, dlcName);
                    MessageBox.Show(Program.dicLanguage["SavedBCM"], "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                if (ex is OverflowException)
                {
                    MessageBox.Show(Program.dicLanguage["DecreaseDLCNum"], Program.dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(ex.Message, Program.dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            tbSavePath_TextChanged(null, null);//これは念のためじゃなくて必須
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveDLC(false);
        }


        private void btnCompSave_Click(object sender, EventArgs e)
        {
            if (newDlc)
            {
                SaveDLC(true);
            }
            else
            {
                btnExtractFiles();
            }
        }

        private void RestrictKeys(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == 8);
        }

        private void ParseValue(object sender, EventArgs e)
        {

            var inputTB = (TextBox)sender;
            if ((dlcData != null) && (inputTB.Text != string.Empty))
            {
                int inputValue = int.Parse(inputTB.Text);
                if (inputValue > 255)
                {
                    inputValue = 255;
                    inputTB.Text = "255";
                }

                if (inputTB.Name == "tbBCMVer")
                {
                    dlcData.BcmVer = (byte)inputValue;
                }
                else
                {

                    if (dgvChars.SelectedRows.Count != 1)
                    {
                        MessageBox.Show("複数選択状態で項目が編集されました。これは想定されない動作です。作者へご報告頂けると幸いです。");

                        return;
                    }

                    if (dgvChars.SelectedRows[0].Index != -1)
                    {
                        if (inputTB.Name == "tbCosSlot")
                        {
                            dlcData.Chars[dgvChars.SelectedRows[0].Index].CostumeSlot = (byte)inputValue;
                        }

                        if (inputTB.Name == "tbAddTexs")
                        {
                            if (inputValue > 4)
                            {
                                inputValue = 4;
                                inputTB.Text = "4";
                            }

                            if (inputValue < 1)
                            {
                                inputValue = 1;
                                inputTB.Text = "1";
                            }

                            dlcData.Chars[dgvChars.SelectedRows[0].Index].AddTexsCount = (byte)inputValue;
                        }
                    }
                }
            }
        }

        private void gv_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (e.RowIndex == dgv.NewRowIndex || !dgv.IsCurrentCellDirty)
            {
                return;
            }


            int idx = e.RowIndex; // 以下では idx がこうであるべき部分

            if (dgv.Columns[e.ColumnIndex].Name == "clmComment")
            {
                /*
                int[] d2i = new int[dgvChars.Columns.Count];
                for (int j = 3; j < dgvChars.Columns.Count; j++)
                {
                    d2i[dgvChars.Columns[j].DisplayIndex] = j;
                }
                */

                // ゼロオリジン何番目のコメントか
                int comIndex = e.ColumnIndex - 3;

                // Char に格納する書式を作成
                string s = "";
                for (int i = 3; i < dgvChars.Columns.Count; i++)
                {
                    if (i == e.ColumnIndex)
                    {
                        setText = e.FormattedValue.ToString().Replace(",", " "); // グローバル変数。Valuated へのメッセージ
                        setIndex = i; // グローバル変数
                        s += setText;
                    }
                    else
                    {
                        s += dgvChars.Rows[idx].Cells[i].Value;
                    }
                    if (i < dgvChars.Columns.Count - 1)
                    {
                        s += ",";
                    }
                }

                dgvChars.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = SortOrder.None;
                dlcData.Chars[idx].Comment = s; //SetComment()

                //MessageBox.Show(s);

                // 複数行を実装する前
                /*
                //MessageBox.Show(idx.ToString());
                if(dlcData.Chars[idx].Comment != e.FormattedValue.ToString())
                {
                    dgvChars.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = SortOrder.None;
                    dlcData.Chars[idx].Comment = e.FormattedValue.ToString(); //SetComment()
                    
                   //MessageBox.Show(dlcData.Chars[idx].Comment.ToString());
                }
                */
                return;
            }

            // SelectedRows は必要最低限の使用に留めるべき
            idx = dgvChars.SelectedRows[0].Index;

            if (e.FormattedValue.ToString() == "")
            {
                //入力した値をキャンセルして元に戻す
                dgv.CancelEdit();
                return;
                //キャンセルする
                //e.Cancel = true; // 何故かこれを実行するとうまく動作しない。初めからコメントアウトされていたからオリジナル作者もよく分かってない？
            }



            if (dgv.Columns[e.ColumnIndex].Name == "clmHair")
            {
                try
                {
                    dlcData.Chars[idx].HStyles[e.RowIndex].Hair = Byte.Parse(e.FormattedValue.ToString());
                }
                catch
                {
                    dgv.CancelEdit();
                    //e.Cancel = true;
                    return;
                }
            }

            if (dgv.Columns[e.ColumnIndex].Name == "clmFace")
            {
                try
                {
                    dlcData.Chars[idx].HStyles[e.RowIndex].Face = Byte.Parse(e.FormattedValue.ToString());
                }
                catch
                {
                    dgv.CancelEdit();
                    //e.Cancel = true;
                    return;
                }
            }


            idx = e.RowIndex; // 以下では idx はこうであるべき

            if (dgv.Columns[e.ColumnIndex].Name == "clmCos")
            {
                try
                {
                    dlcData.Chars[idx].CostumeSlot = Byte.Parse(e.FormattedValue.ToString());

                }
                catch
                {
                    dgv.CancelEdit();
                    //e.Cancel = true;
                    return;
                }
            }


            else if (dgv.Columns[e.ColumnIndex].Name == "clmInner")
            {
                try
                {
                    if (Byte.Parse(e.FormattedValue.ToString()) == 0)
                    {
                        dgv.CancelEdit();
                        //e.Cancel = true;
                        return;
                    }
                    dlcData.Chars[idx].AddTexsCount = Byte.Parse(e.FormattedValue.ToString());

                }
                catch
                {
                    dgv.CancelEdit();
                    //e.Cancel = true;
                    return;
                }
            }



            setEgvCharsSlotColor();
            setEgvCharsNameColor(); // ほとんどの場合に不要だが実体の有無が変化した時にこのタイミングで反映されないのは不自然なので
            setEgvCharsTextsColor();
        }

        private void gv_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            int idx = e.RowIndex;
            if (dgv.Columns[e.ColumnIndex].Name == "clmCos")
            {
                if (dlcData.Chars[idx].CostumeSlot >= Program.NumOfSlots[dlcData.Chars[idx].ID])
                {
                    dlcData.Chars[idx].CostumeSlot = (byte)(Program.NumOfSlots[dlcData.Chars[idx].ID] - 1);
                    dgvChars.Rows[idx].Cells[1].Value = dlcData.Chars[idx].CostumeSlot.ToString();
                    setEgvCharsSlotColor();
                    setEgvCharsNameColor(); // ほとんどの場合に不要だが実体の有無が変化した時にこのタイミングで反映されないのは不自然なので
                    setEgvCharsTextsColor(); // ほとんどの場合に不要だが実体の有無が変化した時にこのタイミングで反映されないのは不自然なので

                }
            }


            else if (dgv.Columns[e.ColumnIndex].Name == "clmInner")
            {

                if (dlcData.Chars[idx].AddTexsCount >= 5)
                {
                    dlcData.Chars[idx].AddTexsCount = (byte)(4);
                    dgvChars.Rows[idx].Cells[2].Value = "4";
                    setEgvCharsSlotColor();
                    setEgvCharsNameColor(); // ほとんどの場合に不要だが実体の有無が変化した時にこのタイミングで反映されないのは不自然なので
                    setEgvCharsTextsColor(); // ほとんどの場合に不要だが実体の有無が変化した時にこのタイミングで反映されないのは不自然なので

                }

            }

            else if (dgv.Columns[e.ColumnIndex].Name == "clmComment")
            {
                if (setIndex >= 0 && setText != (string)dgvChars.Rows[idx].Cells[setIndex].Value)
                {
                    dgvChars.Rows[idx].Cells[setIndex].Value = setText;
                }
                setIndex = -1;
            }

        }

        private void btnHStylesAdd_Click(object sender, EventArgs e)
        {
            if (dgvChars.SelectedRows.Count != 1)
            {
                MessageBox.Show("コスチューム非選択または複数選択時に髪追加の操作が行われました。これは想定されない動作です。作者へご報告頂けると幸いです。");
                return;
            }

            dlcData.Chars[dgvChars.SelectedRows[0].Index].HStyles.Add(new Hairstyle(1, 1));
            ShowHairstyles(dgvChars.SelectedRows[0].Index);
            for (int i = 0; i < dgvHStyles.Rows.Count; i++)
            {
                dgvHStyles.Rows[i].Selected = false;
            }
            dgvHStyles.Rows[dgvHStyles.Rows.Count - 1].Selected = true;
            btnHStylesDelete.Enabled = true;

            if (dgvHStyles.Rows.Count >= 8)
            {
                btnHStylesAdd.Enabled = false;
            }
        }

        private void btnHStylesDelete_Click(object sender, EventArgs e)
        {
            if (dgvChars.SelectedRows.Count != 1)
            {
                MessageBox.Show("コスチューム非選択または複数選択時に髪削除の操作が行われました。これは想定されない動作です。作者へご報告頂けると幸いです。");
                return;
            }

            // デリートキーのイベントハンドラからヌルヌルで呼び出しているので注意

            if (dlcData.Chars[dgvChars.SelectedRows[0].Index].HStyles.Count > 1) // 他が正しければ要らないけど安全のため
            {
                // 複数選択への対応＋削除時に選択状態を解除するように仕様変更
                DataGridViewSelectedRowCollection SelectedRows;
                int SelectedRowsCount;
                int CharSelected;
                int[] arraySelected;
                bool tryToAllDelete;
                try
                {
                    SelectedRows = dgvHStyles.SelectedRows;
                    SelectedRowsCount = SelectedRows.Count;
                    CharSelected = dgvChars.SelectedRows[0].Index;
                    arraySelected = new int[SelectedRowsCount];
                    for (int i = 0; i < SelectedRowsCount; i++)
                    {
                        arraySelected[i] = SelectedRows[i].Index;
                    }
                    tryToAllDelete = (SelectedRowsCount >= dgvHStyles.Rows.Count);
                }
                catch
                {
                    return;
                }
                Array.Sort(arraySelected);
                for (int i = 0; i < SelectedRowsCount; i++)
                {
                    dlcData.Chars[CharSelected].HStyles.RemoveAt(arraySelected[0]); // ココは i じゃなくて 0 で正解

                    if (dlcData.Chars[CharSelected].HStyles.Count <= 1)
                    {
                        btnHStylesDelete.Enabled = false;
                        break;
                    }
                }
                ShowHairstyles(dgvChars.SelectedRows[0].Index);

                for (int i = 0; i < dgvHStyles.Rows.Count; i++)
                {
                    dgvHStyles.Rows[i].Selected = false;
                }

                if (tryToAllDelete && dgvHStyles.Rows.Count > 0)
                {
                    dgvHStyles.Rows[0].Selected = true;
                }

                /*
                int idx = dgvHStyles.SelectedRows[0].Index;
                dlcData.Chars[dgvChars.SelectedRows[0].Index].HStyles.RemoveAt(idx);
                ShowHairstyles(dgvChars.SelectedRows[0].Index);
                for(int i=0;i< dgvHStyles.Rows.Count;i++)
                {
                    dgvHStyles.Rows[i].Selected = false;
                }
                dgvHStyles.Rows[idx< dgvHStyles.Rows.Count? idx :(idx-1)].Selected = true;
                if (dgvHStyles.Rows.Count <= 1)
                {
                    btnHStylesDelete.Enabled = false;
                }
                */
            }
        }

        private void btnNewDLC_Click(object sender, EventArgs e)
        {

            var pt = LoadIniString("InitialDirectory", "BCM");
            if (pt != "") saveFileDialog.InitialDirectory = GetOwnOFolderrExistingParent(Path.GetDirectoryName(pt));
            saveFileDialog.FileName = Path.GetFileName(openFileDialog.FileName);
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {

                //MessageBox.Show("a");
                System.Text.RegularExpressions.Regex regex_doa = new System.Text.RegularExpressions.Regex(@"^(.*\\([^\\]+))\\\2\.bcm$",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (regex_doa.IsMatch(saveFileDialog.FileName))
                {
                    tbSavePath.Text = regex_doa.Replace(saveFileDialog.FileName, "$1");
                    SaveIniString("InitialDirectory", "BCM", Path.GetDirectoryName(saveFileDialog.FileName));

                }
                else
                {
                    tbSavePath.Text = saveFileDialog.FileName;
                    SaveIniString("InitialDirectory", "BCM", saveFileDialog.FileName);
                }

                newDlc = true;
                ClearMainUI();
                //tbSavePath.Text = saveFileDialog.FileName;
                dlcData = new DLCData();
                //btnSave.Text = Program.dicLanguage["SaveDLC"];
                setBtnSave();
                btnCmpSave.Enabled = btnSave.Enabled = true;
                btnSaveState.Enabled = true;
                btnCharsAdd.Enabled = true;
                //clmCos.ReadOnly = false;
                clmInner.ReadOnly = false;
                dlcData.BcmVer = 9;
                tbBCMVer.Text = "9";
                //dlcData.SavePath = saveFileDialog.FileName;

                cbSaveListInDLC.Enabled = true;
                cbSaveListInDLC.Checked = true;

            }
        }

        private void OpenStateFile(string fileName) { OpenStateFile(fileName, null); }
        private void OpenStateFile(string fileName, string ListFilePath)
        {
            try
            {
                newDlc = true;
                ClearMainUI();
                dlcData = Program.OpenState(fileName);

                MakeColumnsFromDLCData(dlcData);

                //btnSave.Text = Program.dicLanguage["SaveDLC"];
                setBtnSave();
                btnCmpSave.Enabled = btnSave.Enabled = true;
                btnSaveState.Enabled = true;
                btnCharsAdd.Enabled = true;
                btnHStylesAdd.Enabled = true;
                //clmCos.ReadOnly = false;
                clmInner.ReadOnly = false;
                if (dlcData.Chars.Count > 0)
                {
                    tbSavePath.Text = dlcData.SavePath;
                    setBtnSave(); // もう一回
                    tbBCMVer.Text = dlcData.BcmVer.ToString();
                    for (int i = 0; i < dlcData.Chars.Count; i++)
                    {
                        dgvChars.Rows.Add();
                        dgvChars.Rows[i].Cells[0].Value = GetCharNamesJpn(dlcData.Chars[i].ID);// Program.CharNamesJpn[dlcData.Chars[i].ID];
                        dgvChars.Rows[i].Cells[1].Value = dlcData.Chars[i].CostumeSlot.ToString();
                        dgvChars.Rows[i].Cells[2].Value = dlcData.Chars[i].AddTexsCount.ToString();

                        //dlcData.Chars[i].Comment = "";// GetComment()";
                        //dgvChars.Rows[i].Cells[3].Value = dlcData.Chars[i].Comment;
                        showComment(i);

                        /*
                        ((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).Items.Add("0");
                        ((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).Items.Add("1");
                        ((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).Items.Add("2");
                        ((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).Items.Add("3");
                        ((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).Items.Add("4");
                        ((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).Items.Add("×39");
                        ((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).Items.Add("39-");
                        ((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).Items.Add("39*");
                        ((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).Items.Add("39?");
                        ((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).Value = "2";
                        //((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[2]).Items[3].Ba
                        //((ComboBox)dgvChars.Rows[i].Cells[1]).DrawItem += new DrawItemEventHandler(ComboBox1_DrawItem);
                        //((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).DisplayStyle;
                        //MessageBox.Show(((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).DropDownWidth.ToString());
                        //((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).DropDownWidth = 2;
                        ((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                        ((DataGridViewComboBoxCell)dgvChars.Rows[i].Cells[1]).FlatStyle = FlatStyle.Flat;
                        */
                    }

                    dgvChars.Rows[0].Selected = true;

                }

                // リストファイルのパスを表示
                string ext = Path.GetExtension(fileName).ToLower();
                if (ext == ".lst" || ext == ".rst")
                {
                    tbListPath.Text = fileName.Substring(0, fileName.Length - 4);
                }
                else
                {
                    tbListPath.Text = fileName;
                }
                tbListPath.Select(tbListPath.Text.Length, 0);
                tbListPath.ScrollToCaret();

                // 既にリストファイルが保存してあって、そのリストファイルがその DLC 用のものである場合に限ってチェックボックスオン
                /*
                try
                {
                    string listInDLC = Path.Combine(dlcData.SavePath, Path.GetFileName(fileName));
                    if(listInDLC == fileName || (File.Exists(listInDLC) && (Program.OpenState(listInDLC).SavePath == dlcData.SavePath)))
                    {
                        cbSaveListInDLC.Checked = true;
                    }
                }
                catch { }*/
                //　・・・は止めて、既に DLC フォルダが存在して、その中に保存するべきリストファイルが存在しない場合を除いてチェックボックスオン
                // わざわざ中身までは見ないことにする
                try
                {
                    if (ListFilePath == null)
                    {
                        string listInDLC = Path.Combine(dlcData.SavePath, Path.GetFileName(fileName));
                        cbSaveListInDLC.Checked = (listInDLC == fileName || (!Directory.Exists(dlcData.SavePath)) || File.Exists(listInDLC));
                    }
                    else
                    {
                        string listInDLC = Path.Combine(dlcData.SavePath, Path.GetFileName(ListFilePath));
                        cbSaveListInDLC.Checked = (listInDLC == fileName || (!Directory.Exists(dlcData.SavePath)) || File.Exists(listInDLC));
                    }
                }
                catch
                {
                    cbSaveListInDLC.Checked = true;
                }


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Program.dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnOpenState_Click(object sender, EventArgs e)
        {

            try
            {

                var pt = GetOwnOFolderrExistingParent(LoadIniString("InitialDirectory", "LST"));
                if (pt != "") openFileDialogState.InitialDirectory = pt;
                openFileDialogState.FileName = Path.GetFileName(saveFileDialogState.FileName);
                if (openFileDialogState.ShowDialog() == DialogResult.OK)
                {
                    SaveIniString("InitialDirectory", "LST", Path.GetDirectoryName(openFileDialogState.FileName));
                    //dgvChars.Columns[0].HeaderCell.SortGlyphDirection = SortOrder.None;
                    //dgvChars.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.None;
                    for (int i = 0; i < dgvChars.Columns.Count; i++)
                    {
                        dgvChars.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
                    }



                    if (openFileDialogState.FilterIndex == 1)
                    {
                        string orgpath = openFileDialogState.FileName;
                        string ext = Path.GetExtension(orgpath).ToLower();
                        //newDlc = true; // OpenStateFile で行われるけど tbListPath.Text 編集イベントの前にかえておかないといけない



                        OpenStateFile(openFileDialogState.FileName);

                    }
                    else
                    {

                        var bf = new BinaryFormatter();
                        using (var fs = new FileStream(openFileDialogState.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            dlcData = (DLCData)bf.Deserialize(fs);
                        }

                        newDlc = true;
                        tbListPath.Text = "";// newDLC より後で。
                        ClearMainUI();

                        MakeColumnsFromDLCData(dlcData);

                        tbSavePath.Text = dlcData.SavePath;
                        btnSave.Text = Program.dicLanguage["SaveDLC"]; ;
                        btnCmpSave.Enabled = btnSave.Enabled = true;
                        btnSaveState.Enabled = true;
                        btnCharsAdd.Enabled = true;
                        //clmCos.ReadOnly = false;
                        clmInner.ReadOnly = false;
                        tbBCMVer.Text = dlcData.BcmVer.ToString();

                        for (int i = 0; i < dlcData.Chars.Count; i++)
                        {
                            dgvChars.Rows.Add();
                            dgvChars.Rows[i].Cells[0].Value = GetCharNamesJpn(dlcData.Chars[i].ID);// Program.CharNamesJpn[dlcData.Chars[i].ID];
                            dgvChars.Rows[i].Cells[1].Value = dlcData.Chars[i].CostumeSlot.ToString();
                            dgvChars.Rows[i].Cells[2].Value = dlcData.Chars[i].AddTexsCount.ToString();

                            dlcData.Chars[i].Comment = "";// GetComment()";
                            //dgvChars.Rows[i].Cells[3].Value = dlcData.Chars[i].Comment;
                            showComment(i);

                        }
                        if (dlcData.Chars.Count > 0)
                        {
                            dgvChars.Rows[0].Selected = true;
                        }

                    }
                    saveFileDialogState.FileName = openFileDialogState.FileName;



                    setEgvCharsSlotColor();
                    setEgvCharsNameColor();
                    setEgvCharsTextsColor();
                    if (dgvChars.Rows.Count <= 0)
                    {
                        btnCharsDelete.Enabled = false;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void MakeColumnsFromDLCData(DLCData dlcData)
        {

            // 読み込んだ dlcData.Chars の中でコンマが一番多いもののコンマの数を取得
            int max = 0;
            for (int i = 0; i < dlcData.Chars.Count; i++)
            {
                int start = 0;
                int commacount = 0;
                string com = dlcData.Chars[i].Comment;
                while (true)
                {
                    start = com.IndexOf(',', start) + 1;
                    if (start > 0)
                    {
                        commacount++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (commacount > max)
                {
                    max = commacount;
                }
            }

            // 現在のコメント数
            int corcom = dgvChars.Columns.Count - 3;

            // max + 1 - corcom 列だけ末尾にコメント列を追加
            //MessageBox.Show(max.ToString());
            for (int i = 0; i < max + 1 - corcom; i++)
            {
                AddComColumn(i + 2 + corcom, true);
            }
        }

        private void btnSaveState_Click(object sender, EventArgs e)
        {
            try
            {
                bool PathOK;
                bool showDialog;
                string path = "";
                if (tbListPath.Text == "")
                {
                    saveFileDialogState.InitialDirectory = LoadIniString("InitialDirectory", "LST");
                    saveFileDialogState.FileName = Path.GetFileName(openFileDialogState.FileName);
                    PathOK = (saveFileDialogState.ShowDialog() == DialogResult.OK);
                    if (PathOK) SaveIniString("InitialDirectory", "LST", Path.GetDirectoryName(saveFileDialogState.FileName));
                    if (PathOK) path = saveFileDialogState.FileName;
                    showDialog = true;
                }
                else
                {
                    string filename = Path.GetFileName(tbListPath.Text);
                    bool FileOK = (filename.IndexOfAny(Path.GetInvalidFileNameChars()) < 0);
                    PathOK = (FileOK && Directory.Exists(Path.GetDirectoryName(tbListPath.Text)));
                    showDialog = false;
                    path = tbListPath.Text + ".lst";
                    //MessageBox.Show(Path.GetFileName(tbListPath.Text) + "\n" + PathOK.ToString());
                    if (!PathOK)
                    {
                        if (FileOK)
                        {
                            saveFileDialogState.FileName = filename;
                        }
                        else
                        {
                            saveFileDialogState.FileName = Path.GetFileName(saveFileDialogState.FileName);
                        }
                        saveFileDialogState.InitialDirectory = LoadIniString("InitialDirectory", "LST");
                        saveFileDialogState.FileName = Path.GetFileName(openFileDialogState.FileName);
                        PathOK = (saveFileDialogState.ShowDialog() == DialogResult.OK);
                        if (PathOK) SaveIniString("InitialDirectory", "LST", Path.GetDirectoryName(saveFileDialogState.FileName));
                        if (PathOK) path = saveFileDialogState.FileName;
                        showDialog = true;
                    }
                }



                if (PathOK)
                {
                    dlcData.SavePath = tbSavePath.Text;
                    bool lst = false;
                    if ((!showDialog) || saveFileDialogState.FilterIndex == 1)
                    {
                        Program.SaveState(dlcData, path);
                        lst = true;
                    }
                    else
                    {
                        var bf = new BinaryFormatter();
                        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                        {
                            bf.Serialize(fs, dlcData);
                        }

                    }
                    if (lst)
                    {
                        //btnSaveState.Text = Program.dicLanguage["OverwriteState"];
                        tbListPath.Text = path.Substring(0, path.Length - 4);
                        tbListPath.Select(tbListPath.Text.Length, 0);
                        tbListPath.ScrollToCaret();
                    }

                    // DLC フォルダにもリストを保存する設定になっていて、DLC フォルダが既に存在していたら
                    // そっちにも保存する。

                    string pathInDLC = tbSavePath.Text + @"\" + Path.GetFileName(tbListPath.Text) + ".lst";
                    if (cbSaveListInDLC.Enabled && cbSaveListInDLC.Checked)
                    {

                        string name;
                        try
                        {
                            name = Path.GetFileName(tbListPath.Text);
                        }
                        catch
                        {
                            name = "";
                        }
                        bool GoodName = (name != "" && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0);
                        if (GoodName)
                        {

                            bool DLCFolderExists = false;
                            try
                            {
                                DLCFolderExists = Directory.Exists(Path.GetDirectoryName(pathInDLC));
                            }
                            catch { }
                            if (DLCFolderExists)
                            {
                                dlcData.SavePath = tbSavePath.Text;
                                Program.SaveState(dlcData, pathInDLC); //dlcData4Save とどっちか迷うところだけど、バックアップ機能を兼ねると思えばオリジナルのほうで良いのでは。
                            }
                        }
                    }
                    else if (cbSaveListInDLC.Enabled && !cbSaveListInDLC.Checked)
                    {
                        // ファイルが存在していてそれが今保存したもので無ければ消す
                        try
                        {
                            if (pathInDLC != path && File.Exists(pathInDLC))
                            {
                                File.Delete(pathInDLC);
                            }
                        }
                        catch { }
                    }


                    MessageBox.Show(Program.dicLanguage["SavedState"], "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCharsAdd_Click(object sender, EventArgs e)
        {
            clikedForm = "btnCharsAdd";
            contextMenuStrip.Show(Cursor.Position.X, Cursor.Position.Y);
        }

        private int GetFirstSelectedCharIndex()
        {
            if (dgvChars.SelectedRows.Count <= 0)
            {
                return -1;
            }
            else
            {
                int result = dgvChars.SelectedRows[0].Index;
                for (int i = 1; i < dgvChars.SelectedRows.Count; i++)
                {
                    if (result > dgvChars.SelectedRows[i].Index)
                    {
                        result = dgvChars.SelectedRows[i].Index;
                    }
                }
                return result;
            }
        }

        private int[] GetSelectedCharsIndexesArray() // どうでもいいけど Index の複数形はこれもアリのようです。
        {
            var result = new int[dgvChars.SelectedRows.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = dgvChars.SelectedRows[i].Index;
            }
            return result;
        }

        private void AddCharacter(object sender, EventArgs e)
        {

            //dgvChars.Columns[0].HeaderCell.SortGlyphDirection = SortOrder.None;
            //dgvChars.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.None;
            for (int i = 0; i < dgvChars.Columns.Count; i++)
            {
                dgvChars.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            var inputMI = (ToolStripMenuItem)sender;
            var newChar = new Character();
            newChar.ID = byte.Parse(inputMI.Name.Substring(2));
            if (Program.FemaleIDs.Contains(newChar.ID))
            {
                newChar.Female = true;
            }

            newChar.CostumeSlot = 0;
            newChar.AddTexsCount = 1;
            newChar.HStyles.Add(new Hairstyle(1, 1));


            /* ここから追加処理*/
            string prtDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string lstPath = Path.Combine(prtDir, @"default.lst");
            string rstPath = Path.Combine(prtDir, @"default.rst");
            string lrPath = "";
            if (System.IO.File.Exists(rstPath)) // rst 優先
            {
                lrPath = rstPath;
            }
            else if (System.IO.File.Exists(lstPath))
            {
                lrPath = lstPath;
            }
            if (lrPath != "")
            {
                DLCData dlcData2 = Program.OpenState(lrPath);
                for (int i = 0; i < dlcData2.Chars.Count; i++)
                {
                    if (newChar.ID == dlcData2.Chars[i].ID)
                    {
                        newChar = dlcData2.Chars[i];
                        break;
                    }
                }
            }
            /* ここまで追加処理 */



            if (clikedForm == "dgvChars")
            {

                // これが挿入処理
                int index;
                index = GetFirstSelectedCharIndex();
                if (index <= 0)
                {
                    index = dgvChars.Rows.Count;
                }
                /*
                try
                {
                    index = dgvChars.SelectedRows[0].Index;
                }
                catch
                {
                    index = dgvChars.Rows.Count;
                }
                */

                // 貼り付けじゃないコピーとは位置が一つ違う
                // あっちは index + 1 だけど index == Length を許してないから間違いではない。
                dlcData.Chars.Insert(index, newChar);
                dgvChars.Rows.Insert(index);
                dgvChars.Rows[index].Cells[0].Value = GetCharNamesJpn(newChar.ID);// Program.CharNamesJpn[newChar.ID];
                dgvChars.Rows[index].Cells[1].Value = newChar.CostumeSlot.ToString();
                dgvChars.Rows[index].Cells[2].Value = newChar.AddTexsCount.ToString();
                //newChar.Comment = "";// GetComment()";
                //dgvChars.Rows[index].Cells[3].Value = newChar.Comment;
                showComment(index);
                dgvChars.ClearSelection();
                dgvChars.Rows[index].Selected = true;

            }
            else if (clikedForm == "btnCharsAdd")
            {
                // これは一番下に追加する処理
                dlcData.Chars.Add(newChar);
                dgvChars.Rows.Add();
                dgvChars.Rows[dgvChars.Rows.Count - 1].Cells[0].Value = GetCharNamesJpn(newChar.ID);// Program.CharNamesJpn[newChar.ID];
                dgvChars.Rows[dgvChars.Rows.Count - 1].Cells[1].Value = newChar.CostumeSlot.ToString();
                dgvChars.Rows[dgvChars.Rows.Count - 1].Cells[2].Value = newChar.AddTexsCount.ToString();
                //newChar.Comment = "";// GetComment()";
                //dgvChars.Rows[dgvChars.Rows.Count - 1].Cells[3].Value = newChar.Comment;
                showComment(dgvChars.Rows.Count - 1);
                dgvChars.ClearSelection();
                dgvChars.Rows[dgvChars.Rows.Count - 1].Selected = true;
            }
            else
            {
                // ここには到達しない
            }


            //コピーじゃないのでこれは必要
            btnCharsDelete.Enabled = true;
            btnFilesAdd.Enabled = true;

            setEgvCharsSlotColor(); // これらも追加
            setEgvCharsNameColor();
            setEgvCharsTextsColor(); // デフォルト機能により初めから問題があることも多々
        }

        private void btnCharsDelete_Click(object sender, EventArgs e)
        {
            // デリートキー入力イベントハンドラからヌルヌルで呼び出しているので書き換える場合は注意

            // ソートされた選択インデックスの列を取得
            var SortedIndexes = GetSelectedCharsIndexesArray();
            Array.Sort(SortedIndexes);
            //MessageBox.Show(SortedIndexes.Length + ", " + SortedIndexes[0]);

            // 非選択時にこれが呼び出されるのは想定外
            if (SortedIndexes.Length <= 0)
            {
                //MessageBox.Show("コスチューム非選択時にコスチューム削除の操作が行われました。これは想定されない動作です。作者へご報告頂けると幸いです。");
                return;
            }

            // 下から削除していけばインデックスが崩れることはない
            for (int i = SortedIndexes.Length - 1; i >= 0; i--)
            {
                dlcData.Chars.RemoveAt(SortedIndexes[i]);
                dgvChars.Rows.RemoveAt(SortedIndexes[i]);
            }

            //int idx = dgvChars.SelectedRows[0].Index;
            //dlcData.Chars.RemoveAt(idx);
            //dgvChars.Rows.RemoveAt(idx);

            // 削除後の選択状態は削除前の先頭の選択行で決める
            int idx = SortedIndexes[0];
            if (idx < dgvChars.Rows.Count)
            {
                dgvChars.ClearSelection();
                dgvChars.Rows[idx].Selected = true;
            }
            else
            {
                if (dgvChars.Rows.Count > 0)
                {
                    dgvChars.ClearSelection();
                    dgvChars.Rows[dgvChars.Rows.Count - 1].Selected = true;
                }
            }


            if (dgvChars.Rows.Count <= 0)
            {
                ClearCharsUI();
                btnCharsDelete.Enabled = false;
                btnHStylesAdd.Enabled = false;
                btnFilesAdd.Enabled = false;
            }

            setEgvCharsSlotColor(); // これは絶対に必要
        }

        private void btnFilesAdd_Click(object sender, EventArgs e)
        {
            var pt = GetOwnOFolderrExistingParent(LoadIniString("InitialDirectory", "DATA"));
            if (pt != "") openFileDialog.InitialDirectory = pt;
            // ファイルの追加は前回のファイル名を思い出す必要はない
            openFileDialog.FileName = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SaveIniString("InitialDirectory", "DATA", Path.GetDirectoryName(openFileDialog.FileName));

                AddFiles(openFileDialog.FileNames, false);

                /* AddFiles の中でやることにした
                //setEgvCharsSlotColor(); // 流石にこれは不要
                setEgvCharsNameColor();
                setEgvCharsTextsColor();
                */
            }
        }

        private void btnFilesDelete_Click(object sender, EventArgs e)
        {
            // デリートキーのイベントハンドラからヌルヌルで呼び出しているので注意

            bool テクスチャ含む = false;

            if (dgvChars.SelectedRows.Count != 1)
            {
                MessageBox.Show("コスチューム非選択またはコスチューム複数選択時にファイル削除の操作が行われました。これは想定されない動作です。作者へご報告頂けると幸いです。");
                return;
            }
            if (dgvFiles.SelectedRows.Count <= 0)
            {
                MessageBox.Show("ファイル非選択時にファイル削除の操作が行われました。これは想定されない動作です。作者へご報告頂けると幸いです。");
                return;
            }

            foreach (DataGridViewRow row in dgvFiles.SelectedRows)
            {
                string selectedFile = row.Cells[0].Value.ToString();
                for (int i = 0; i < FileOrder.Length; i++)
                {
                    if (selectedFile.EndsWith(FileOrder[i], true, null))
                    {

                        if ((!テクスチャ含む) && System.Text.RegularExpressions.Regex.IsMatch(dlcData.Chars[dgvChars.SelectedRows[0].Index].Files[i], @"\.--HL?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                        {
                            テクスチャ含む = true;
                        }

                        dlcData.Chars[dgvChars.SelectedRows[0].Index].Files[i] = null;
                        break;
                    }
                }
            }


            if (テクスチャ含む)
            {
                dlcData.Chars[dgvChars.SelectedRows[0].Index].AddTexsCount = getAvailableTextsCount(dlcData.Chars[dgvChars.SelectedRows[0].Index]);

                dgvChars.Rows[dgvChars.SelectedRows[0].Index].Cells[1].Value = dlcData.Chars[dgvChars.SelectedRows[0].Index].CostumeSlot.ToString();
                dgvChars.Rows[dgvChars.SelectedRows[0].Index].Cells[2].Value = dlcData.Chars[dgvChars.SelectedRows[0].Index].AddTexsCount.ToString();


                // setEgvCharsSlotColor(); これは流石に不要

            }

            //setEgvCharsSlotColor(); // 流石にこれは不要
            setEgvCharsNameColor();
            setEgvCharsTextsColor();

            ShowFiles(dgvChars.SelectedRows[0].Index);
        }



        private void dgvChars_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // コメント列の追加と削除
            if (e.Button == MouseButtons.Right)
            {
                // 起動時には右クリックできないようにしておく
                if (!btnSave.Enabled)
                {
                    return;
                }

                /*
                // MouseDownイベント発生時の (x,y)座標を取得
                DataGridView.HitTestInfo hit = dgvChars.HitTest(e.X, e.Y);
                int clickedColumn = hit.;
                MessageBox.Show(clickedColumn.ToString());
                */

                // インナーよりも左なら行追加は出来ないように
                // コメント行でなければ行削除はできないように
                // コメント行が一つなら行削除はできないように
                smiAddComColumn.Enabled = (e.ColumnIndex >= 2 && dgvChars.Columns.Count < 6);
                smiDelComColumn.Enabled = (e.ColumnIndex >= 3 && dgvChars.Columns.Count > 4);
                //MessageBox.Show(dgvChars.Rows.Count.ToString());

                clikedForm = "dgvColumn" + e.ColumnIndex.ToString();
                cmsAddDelCom.Show(Cursor.Position.X, Cursor.Position.Y);
            }

            // 自動ソート
            else if (e.Button == MouseButtons.Left)
            {
                int dataLength;
                try
                {
                    dataLength = dlcData.Chars.Count;
                }
                catch
                {
                    dataLength = 0;
                }
                if (dataLength > 0)
                {
                    int direct = 0;

                    SortOrder so; // これを設定するのは最後

                    // 必要がある場合に、昇順のみ行い、SortGlyphDirection を更新
                    if (e.ColumnIndex == 0 && dgvChars.Columns[0].HeaderCell.SortGlyphDirection != SortOrder.Ascending)
                    {
                        so = SortOrder.Ascending;
                        //dgvChars.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.None; // 現在のソートルールでは不要
                        for (int i = 3; i < dgvChars.Columns.Count; i++)
                        {
                            dgvChars.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
                        }
                    }
                    else if (e.ColumnIndex == 1 /*&& dgvChars.Columns[1].HeaderCell.SortGlyphDirection != SortOrder.Ascending*/) // 本当はスロット変更のイベントハンドラを書き換えたほうがいい
                    {
                        so = SortOrder.Ascending;
                        //dgvChars.Columns[0].HeaderCell.SortGlyphDirection = SortOrder.None; // 現在のソートルールでは不要
                        for (int i = 3; i < dgvChars.Columns.Count; i++)
                        {
                            dgvChars.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
                        }
                    }
                    else if (e.ColumnIndex >= 3)
                    {
                        if (dgvChars.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection != SortOrder.Ascending)
                        {
                            so = SortOrder.Ascending;
                            direct = 1;
                        }
                        else
                        {
                            so = SortOrder.Descending;
                            direct = -1;
                        }
                        for (int i = 0; i < dgvChars.Columns.Count; i++)
                        {
                            dgvChars.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
                        }
                    }
                    else
                    {
                        return;
                    }

                    // 編集状態を終了する
                    gbChars.Focus();

                    // ソート結果を格納する
                    Character[] result = new Character[dataLength];

                    // それが一番上かどうかを格納する
                    bool[] top = new bool[dataLength];
                    for (int i = 0; i < dataLength; i++)
                    {
                        top[i] = true;
                    }

                    if (e.ColumnIndex == 0 || e.ColumnIndex == 1)
                    {
                        int k = 0; // 汎用。キャラソートではこのスコープでないといけない。
                        for (int i = 0; i < dataLength; i++)
                        {
                            var id = dlcData.Chars[i].ID;
                            var ccount = 1;
                            if (top[i])
                            {
                                for (int j = i + 1; j < dataLength; j++)
                                {
                                    if (dlcData.Chars[j].ID == id)
                                    {
                                        top[j] = false;
                                        ccount++;
                                    }
                                }
                            }
                            else
                            {
                                continue;
                            }

                            if (e.ColumnIndex == 0) // キャラでのソート
                            {
                                for (int j = i; j < dataLength; j++)
                                {
                                    if (dlcData.Chars[j].ID == id)
                                    {
                                        result[k++] = dlcData.Chars[j];
                                    }
                                }
                            }
                            else if (e.ColumnIndex == 1) // スロットでのソート
                            {
                                int[] tc = new int[ccount];
                                k = 0;
                                for (int j = i; j < dataLength; j++)
                                {
                                    if (dlcData.Chars[j].ID == id)
                                    {
                                        tc[k++] = j;
                                    }
                                }
                                for (int s = 0; s < ccount; s++)
                                {
                                    for (int t = 1; t < ccount - s; t++)
                                    {
                                        if (dlcData.Chars[tc[t - 1]].CostumeSlot > dlcData.Chars[tc[t]].CostumeSlot)
                                        {
                                            var temp = tc[t];
                                            tc[t] = tc[t - 1];
                                            tc[t - 1] = temp;
                                        }
                                    }
                                }
                                k = 0;
                                for (int j = i; j < dataLength; j++)
                                {
                                    if (dlcData.Chars[j].ID == id)
                                    {
                                        result[j] = dlcData.Chars[tc[k++]];
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // 選択列の文字列を覚えておく
                        string[] sortTarget = new string[dataLength];
                        for (int i = 0; i < dataLength; i++)
                        {
                            sortTarget[i] = (string)dgvChars.Rows[i].Cells[e.ColumnIndex].Value;
                        }

                        for (int i = 0; i < dataLength; i++)
                        {
                            result[i] = dlcData.Chars[i];
                        }

                        // 安定で遅いバブルソート
                        for (int i = 0; i < dataLength; i++)
                        {
                            for (int j = 1; j < dataLength - i; j++)
                            {
                                int order = 0;
                                //if (result[j - 1].Comment == "" && result[j].Comment != "")
                                if (sortTarget[j - 1] == "" && sortTarget[j] != "")
                                {
                                    order = 1;
                                }
                                //else if (result[j - 1].Comment != "" && result[j].Comment == "")
                                else if (sortTarget[j - 1] != "" && sortTarget[j] == "")
                                {
                                    order = -1;
                                }
                                else
                                {
                                    //order = result[j - 1].Comment.CompareTo(result[j].Comment);
                                    order = sortTarget[j - 1].CompareTo(sortTarget[j]);
                                }
                                if (direct * order > 0)
                                {
                                    var temp = result[j - 1];
                                    result[j - 1] = result[j];
                                    result[j] = temp;

                                    var temp2 = sortTarget[j - 1];
                                    sortTarget[j - 1] = sortTarget[j];
                                    sortTarget[j] = temp2;
                                }
                            }

                        }
                    }

                    // Character の参照に対して選択状態を紐付ける
                    var Selected = new System.Collections.Generic.Dictionary<Character, bool>();
                    for (var i = 0; i < dlcData.Chars.Count; i++)
                    {
                        Selected[dlcData.Chars[i]] = dgvChars.Rows[i].Selected;
                    }

                    // 結果を格納
                    for (int i = 0; i < dataLength; i++)
                    {
                        dlcData.Chars[i] = result[i];
                        byte ID = dlcData.Chars[i].ID;
                        dgvChars.Rows[i].Cells[0].Value = GetCharNamesJpn(ID);// Program.CharNamesJpn[ID];
                        dgvChars.Rows[i].Cells[1].Value = dlcData.Chars[i].CostumeSlot.ToString();
                        dgvChars.Rows[i].Cells[2].Value = dlcData.Chars[i].AddTexsCount.ToString();
                        //dgvChars.Rows[i].Cells[3].Value = dlcData.Chars[i].Comment;
                        showComment(i);
                    }

                    // Character の参照を手がかりに選択状態を復元する
                    dgvChars.ClearSelection();
                    for (var i = 0; i < dlcData.Chars.Count; i++)
                    {
                        dgvChars.Rows[i].Selected = Selected[dlcData.Chars[i]];
                    }


                    dgvChars.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = so;

                    // 色を再描画
                    setEgvCharsSlotColor();
                    setEgvCharsNameColor();
                    setEgvCharsTextsColor();
                }
            }
        }

        private bool dgvChars_MouseDown_dontSelectOne = false;
        private SortOrder[] dgvChars_MouseDown_SortDirectionList = null;
        private int dgvChars_MouseDown_LeftShiftPreClickedIndex = -1;
        private void dgvChars_MouseDown(object sender, MouseEventArgs e)
        {
            // マウスの左ボタンが押されている場合
            var eb = e.Button;
            if (eb == MouseButtons.Left)
            {

                // MouseDownイベント発生時の (x,y)座標を取得
                DataGridView.HitTestInfo hit = dgvChars.HitTest(e.X, e.Y);
                if (hit.RowIndex < 0)
                {
                    // クリックした場所がコメントじゃないカラムヘッダだった場合、その列が移動できないようにする
                    // マウス UP イベントで解除
                    if (hit.Type == DataGridViewHitTestType.ColumnHeader && hit.ColumnIndex < 3)
                    {
                        dgvChars.AllowUserToOrderColumns = false;
                        return;
                    }


                    return;
                }



                // コントロールキーが押されていたらどこをクリックしたかによらずその行の選択をトグルする
                // シフトキーが押されていたらカレントからガバッと選択する
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control /*|| (Control.ModifierKeys & Keys.Shift) == Keys.Shift*/)
                {
                    //MessageBox.Show("a");
                    // しかしここで変更してもその後クリックが発生して意味がなくなるので
                    // ここでは単にその後の動作のキャンセルにとどまる
                    // ・・・そしてそうしたらそれだけで勝手にできるようになった。
                    dgvChars_MouseDown_dontSelectOne = true;
                    return;


                    //dgvChars.Rows[hit.RowIndex].Selected = true; //!dgvChars.Rows[hit.RowIndex].Selected;
                    //MessageBox.Show("1");

                }

                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    //MessageBox.Show(dgvChars.CurrentCell.Value.ToString());
                    dgvChars_MouseDown_dontSelectOne = true;
                    dgvChars_MouseDown_LeftShiftPreClickedIndex = dgvChars.CurrentCell.RowIndex;
                    dragStartIndexes = GetSelectedCharsIndexesArray();
                    return;
                }


                // 該当行を選択状態にする←これをするとクリックイベントが起きなくなる？
                if (!dgvChars.Rows[hit.RowIndex].Selected)
                    dgvChars.ClearSelection();
                dgvChars.Rows[hit.RowIndex].Selected = true;

                // そうでなくて、更にそれがスロットのところで newDLC ならスロットメニューを表示
                if (newDlc && hit.ColumnIndex == 1)
                {
                    var r = dgvChars.GetCellDisplayRectangle(1, hit.RowIndex, false);

                    cmsSlot.Items.Clear();

                    // 選択キャラ
                    Character Char = dlcData.Chars[hit.RowIndex];

                    byte tempCostumeSlot = Char.CostumeSlot;

                    byte bound = Program.NumOfSlots[Char.ID];
                    Program.SlotTable<int> slotCount = new Program.SlotTable<int>(0);
                    for (int i = 0; i < dgvChars.Rows.Count; i++)
                    {
                        // 自分以外の情報は今は不要
                        if (dlcData.Chars[i].ID == Char.ID)
                        {
                            slotCount[dlcData.Chars[i]]++;
                        }
                    }

                    // 自分自身は数えられているはずだという前提がここではほとんど成立しないので
                    for (byte i = 0; i < Program.NumOfSlots[Char.ID]; i++)
                    {
                        if (i != tempCostumeSlot)
                        {
                            Char.CostumeSlot = i;
                            slotCount[Char]++;
                        }
                    }


                    // common, initial を exe に埋め込んでいたときの記述
                    /*
                    Program.SlotTable<bool> NotComIniSlotTable = GetNotComIniSlotTable();
                    Program.SlotTable<string> ComIniSlotCommentTable = GetComIniSlotCommentTable();
                    Program.SlotTable<string> SlotOwnerTable = GetSlotOwnerTable();
                    Program.SlotTable<bool> UsableSlotTable = GetNotComIniSlotTable();
                    */
                    Program.SlotTable<string> ComIniSlotCommentTable = GetComIniSlotCommentTable();
                    Program.SlotTable<bool> NotComIniSlotTable = new Program.SlotTable<bool>(true);
                    for (int i = 0; i < ComIniSlotCommentTable.Count(); i++)
                    {
                        for (int j = 0; j < ComIniSlotCommentTable[i].Length; j++)
                        {
                            var ComIniSlotComment = ComIniSlotCommentTable[i, j];
                            NotComIniSlotTable[i, j] = (ComIniSlotCommentTable[i, j] == "");
                        }
                    }
                    Program.SlotTable<string> SlotOwnerTable = GetSlotOwnerTable();
                    Program.SlotTable<bool> UsableSlotTable = new Program.SlotTable<bool>(true);

                    for (int i = 0; i < UsableSlotTable.Count(); i++)
                    {
                        for (int j = 0; j < UsableSlotTable[i].Length; j++)
                        {
                            UsableSlotTable[i, j] = (SlotOwnerTable[i, j] == "");
                        }
                    }
                    for (byte i = 0; i < bound; i++)
                    {
                        Char.CostumeSlot = i;

                        if (UsableSlotTable[Char] && NotComIniSlotTable[Char])
                        {
                            cmsSlot.Items.Add(i.ToString());
                        }
                        else if (NotComIniSlotTable[Char])
                        {
                            cmsSlot.Items.Add(i.ToString() + " (" + SlotOwnerTable[Char] + ")");
                        }
                        else if (UsableSlotTable[Char])
                        {
                            cmsSlot.Items.Add(i.ToString() + " (" + ComIniSlotCommentTable[Char] + ")");
                        }
                        else
                        {
                            cmsSlot.Items.Add(i.ToString() + " ((" + ComIniSlotCommentTable[Char] + ", " + SlotOwnerTable[Char] + ")");
                        }

                        cmsSlot.Items[i].BackColor = getSlotBackColor(slotCount, NotComIniSlotTable, UsableSlotTable, Char);

                        cmsSlot.Items[i].Click += ChangeSlot;





                    }


                    // 直ぐに元に戻す
                    Char.CostumeSlot = tempCostumeSlot;

                    // 矢印を頭につける
                    //cmsSlot.Items[tempCostumeSlot].Text = "▶" + cmsSlot.Items[tempCostumeSlot].Text;


                    // フォント変更（と言うか下線付加）
                    //現在のフォントを覚えておく
                    var oldFont = cmsSlot.Items[tempCostumeSlot].Font;
                    //現在のフォントにBoldを付加したフォントを作成する
                    //なおBoldを取り消す場合は、「oldFont.Style & ~FontStyle.Bold」とする
                    var newFont = new System.Drawing.Font(oldFont, oldFont.Style | System.Drawing.FontStyle.Underline | System.Drawing.FontStyle.Bold);
                    //Boldを付加したフォントを設定する
                    cmsSlot.Items[tempCostumeSlot].Font = newFont;
                    //前のフォントを解放する
                    oldFont.Dispose();



                    System.Drawing.Point location = dgvChars.PointToScreen(r.Location);
                    //MessageBox.Show("a");
                    cmsSlot.Show(location);
                    //cmsSlot.Items.Clear();
                }

                else // それも違ってたら手動ソート開始
                {

                    mouseDownPoint = new System.Drawing.Point(e.X, e.Y);
                    dragStartIndexes = GetSelectedCharsIndexesArray();

                    dgvChars_MouseDown_SortDirectionList = new SortOrder[dgvChars.Columns.Count];
                    for (int i = 0; i < dgvChars.Columns.Count; i++)
                    {
                        dgvChars_MouseDown_SortDirectionList[i] = dgvChars.Columns[i].HeaderCell.SortGlyphDirection;
                    }
                }


            }
            else
            {
                mouseDownPoint = System.Drawing.Point.Empty;

                // 右クリック
                if (eb == MouseButtons.Right && btnCharsAdd.Enabled)
                {
                    if (dragPrevIndexes != null)
                    {
                        slideChar(dragPrevIndexes, dragStartIndexes);


                        // 擬似選択状態を本当の選択状態にコピー
                        for (int i = 0; i < dgvChars.Rows.Count; i++)
                        {
                            dgvChars.Rows[i].Selected = (Array.IndexOf(dragPrevIndexes, i) >= 0);
                        }


                        dragPrevIndexes = dragStartIndexes = null;
                        dragHoldRelIndex = -1;

                        this.Cursor = Cursors.Default;




                        // 選択表示を普通の方法に戻す
                        for (int i = 0; i < dgvChars.Rows.Count; i++)
                        {
                            var row = dgvChars.Rows[i];
                            for (int j = 0; j < row.Cells.Count; j++)
                            {
                                var stl = row.Cells[j].Style;
                                stl.SelectionBackColor = System.Drawing.Color.Empty;
                                stl.SelectionForeColor = System.Drawing.Color.Empty;
                                stl.BackColor = System.Drawing.Color.Empty;
                                stl.ForeColor = System.Drawing.Color.Empty;
                            }
                        }

                        // キーの下をカレントに
                        DataGridView.HitTestInfo hitc = dgvChars.HitTest(e.X, e.Y);
                        if (hitc.Type == DataGridViewHitTestType.Cell)
                        {
                            dgvChars.CurrentCell = dgvChars.Rows[hitc.RowIndex].Cells[hitc.ColumnIndex];
                        }

                        // コイツを復活させる。
                        dgvChars_SelectionChanged_RepairingSelection = false;

                        // 色を再描画
                        setEgvCharsSlotColor();
                        //MessageBox.Show("2");
                        setEgvCharsNameColor();
                        setEgvCharsTextsColor();
                        //MessageBox.Show("1");

                        dgvChars.Focus();

                        dgvChars_MouseDown_dontSelectOne = true;

                        if (dgvChars_MouseDown_SortDirectionList != null)
                        {
                            for (int i = 0; i < dgvChars.Columns.Count; i++)
                            {
                                dgvChars.Columns[i].HeaderCell.SortGlyphDirection = dgvChars_MouseDown_SortDirectionList[i];
                            }
                            dgvChars_MouseDown_SortDirectionList = null;
                        }

                        return;
                    }



                    // MouseDownイベント発生時の (x,y)座標を取得
                    DataGridView.HitTestInfo hit = dgvChars.HitTest(e.X, e.Y);
                    int index = hit.RowIndex;
                    if (index < 0)
                    {
                        // カラムヘッダの右クリック時は何もしない
                        if (hit.Type == DataGridViewHitTestType.ColumnHeader)
                        {
                            return;
                        }


                        index = dgvChars.Rows.Count - 1;
                    }


                    // 編集状態で、なおかつそこをクリックした場合は何もしない
                    var cc = dgvChars.CurrentCell;
                    if (dgvChars.IsCurrentCellInEditMode && hit.ColumnIndex == cc.ColumnIndex && hit.RowIndex == cc.RowIndex)
                    {
                        return;
                    }

                    // 間違っでもこれはやらないように
                    //mouseDownPoint = new System.Drawing.Point(e.X, e.Y); ;


                    // 該当行を選択状態にする
                    if (index >= 0)
                    {
                        dgvChars.ClearSelection();
                        dgvChars.Rows[index].Selected = true;
                    }


                    clikedForm = "dgvChars";



                    contextMenuStrip.Items.Add(Program.dicLanguage["DeleteD"]);
                    contextMenuStrip.Items[contextMenuStrip.Items.Count - 1].Click += btnCharsDelete_Click;
                    contextMenuStrip.Show(Cursor.Position.X, Cursor.Position.Y);
                    contextMenuStrip.Closed += DeleteLastCMSItem;

                    //cmsAddDeleteChars.Show(Cursor.Position.X, Cursor.Position.Y);
                }

            }

        }


        private void DeleteLastCMSItem(object sender, EventArgs e)
        {
            var cms = (ContextMenuStrip)sender;
            cms.Items.RemoveAt(cms.Items.Count - 1);
            cms.Closed -= DeleteLastCMSItem;
        }

        private void MainForm_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ChangeSlot(object sender, EventArgs e)
        {

            if (dgvChars.SelectedRows.Count != 1)
            {
                // これって知らない操作で結構起こりそうだからエラーは出さないでおくか。
                //MessageBox.Show("コスチューム非選択またはコスチューム複数選択時にスロット変更の操作が行われました。これは想定されない動作です。作者へご報告頂けると幸いです。");
                return;
            }


            // 選択キャラ
            Character Char = dlcData.Chars[dgvChars.SelectedRows[0].Index];


            var inputMI = (ToolStripMenuItem)sender;
            var Slot = byte.Parse((inputMI.ToString() + " ").Substring(0, 2));
            Char.CostumeSlot = Slot;
            dgvChars.Rows[dgvChars.SelectedRows[0].Index].Cells[1].Value = Slot.ToString();
            setEgvCharsSlotColor(); // 絶対やらないといけないのはこれだけだけど
            setEgvCharsNameColor();
            setEgvCharsTextsColor();


        }

        private void slideChar(int[] fromIndexes, int toIndex)
        {
            var offset = toIndex - (fromIndexes[0] + dragHoldRelIndex);
            var newChars = new System.Collections.Generic.List<Character>();


            // 端の方に言った場合、掴んでいる場所を取り変えることで辻褄を合わせる。
            // 掴んでいるところの変更はそのまま引きずることにする
            if (fromIndexes[0] + offset < 0)
            {
                dragHoldRelIndex += offset + fromIndexes[0];
                offset = -fromIndexes[0];

            }
            else if (fromIndexes[fromIndexes.Length - 1] + offset >= dlcData.Chars.Count)
            {
                dragHoldRelIndex += offset - (dlcData.Chars.Count - 1 - fromIndexes[fromIndexes.Length - 1]);
                offset = dlcData.Chars.Count - 1 - fromIndexes[fromIndexes.Length - 1];
            }



            // これ別の書き方無いものか
            for (int i = 0; i < dlcData.Chars.Count; i++) newChars.Add(null);

            // dlcData を並べ替え＋描画
            for (int i = 0; i < fromIndexes.Length; i++)
            {
                //try {
                newChars[fromIndexes[i] + offset] = dlcData.Chars[fromIndexes[i]];
                //}
                //catch
                //{
                //    MessageBox.Show(fromIndexes[i] + ", " + offset + ", " + newChars.Count);
                //}
                dlcData.Chars[fromIndexes[i]] = null;
            }
            int pos = -1;
            for (int i = 0; i < dlcData.Chars.Count; i++)
            {
                if (dlcData.Chars[i] != null)
                {
                    while (newChars[++pos] != null) ;
                    newChars[pos] = dlcData.Chars[i];
                }
            }
            for (int i = 0; i < dlcData.Chars.Count; i++)
            {
                dlcData.Chars[i] = newChars[i];
                dgvChars.Rows[i].Cells[0].Value = GetCharNamesJpn(dlcData.Chars[i].ID, false);//  Program.CharNamesJpn[dlcData.Chars[i].ID];
                dgvChars.Rows[i].Cells[1].Value = dlcData.Chars[i].CostumeSlot.ToString();
                dgvChars.Rows[i].Cells[2].Value = dlcData.Chars[i].AddTexsCount.ToString();
                showComment(i);
            }
            newChars = null;




            /*
            // 移動した行を選択
            NeedShowChar = false;
            dgvChars.ClearSelection();
            for (int i = 0; i < fromIndexes.Length; i++)
            {
                dgvChars.Rows[fromIndexes[i] + offset].Selected = true;
            }
            NeedShowChar = true;
            */

            // ソートが崩れたことを記憶
            for (int i = 0; i < dgvChars.Columns.Count; i++)
            {
                dgvChars.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            //MessageBox.Show("1");


            // 選択表示を普通の方法に戻す
            for (int i = 0; i < dgvChars.Rows.Count; i++)
            {
                var row = dgvChars.Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    var stl = row.Cells[j].Style;
                    stl.SelectionBackColor = System.Drawing.Color.Empty;
                    stl.SelectionForeColor = System.Drawing.Color.Empty;
                    stl.BackColor = System.Drawing.Color.Empty;
                    stl.ForeColor = System.Drawing.Color.Empty;
                }
            }

            // 色を再描画
            setEgvCharsSlotColor();
            //MessageBox.Show("2");
            setEgvCharsNameColor();
            setEgvCharsTextsColor();



            //MessageBox.Show("1");

            // fromIndexes を更新する
            for (int i = 0; i < fromIndexes.Length; i++)
            {
                fromIndexes[i] += offset;
            }


            ColoringLikeSelection();


        }
        private void slideChar(int[] fromIndexes, int[] toIndexes)
        {

            var newChars = new System.Collections.Generic.List<Character>();




            // これ別の書き方無いものか
            for (int i = 0; i < dlcData.Chars.Count; i++) newChars.Add(null);

            // dlcData を並べ替え＋描画
            for (int i = 0; i < fromIndexes.Length; i++)
            {
                newChars[toIndexes[i]] = dlcData.Chars[fromIndexes[i]];
                dlcData.Chars[fromIndexes[i]] = null;
            }
            int pos = -1;
            for (int i = 0; i < dlcData.Chars.Count; i++)
            {
                if (dlcData.Chars[i] != null)
                {
                    while (newChars[++pos] != null) ;
                    newChars[pos] = dlcData.Chars[i];
                }
            }
            for (int i = 0; i < dlcData.Chars.Count; i++)
            {
                dlcData.Chars[i] = newChars[i];
                dgvChars.Rows[i].Cells[0].Value = GetCharNamesJpn(dlcData.Chars[i].ID, false);//  Program.CharNamesJpn[dlcData.Chars[i].ID];
                dgvChars.Rows[i].Cells[1].Value = dlcData.Chars[i].CostumeSlot.ToString();
                dgvChars.Rows[i].Cells[2].Value = dlcData.Chars[i].AddTexsCount.ToString();
                showComment(i);
            }
            newChars = null;

            // ソートが崩れたことを記憶
            for (int i = 0; i < dgvChars.Columns.Count; i++)
            {
                dgvChars.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
            }



            // 選択表示を普通の方法に戻す
            for (int i = 0; i < dgvChars.Rows.Count; i++)
            {
                var row = dgvChars.Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    var stl = row.Cells[j].Style;
                    stl.SelectionBackColor = System.Drawing.Color.Empty;
                    stl.SelectionForeColor = System.Drawing.Color.Empty;
                    stl.BackColor = System.Drawing.Color.Empty;
                    stl.ForeColor = System.Drawing.Color.Empty;
                }
            }

            // 色を再描画
            setEgvCharsSlotColor();
            //MessageBox.Show("2");
            setEgvCharsNameColor();
            setEgvCharsTextsColor();



            //MessageBox.Show("1");

            // fromIndexes を更新する
            for (int i = 0; i < fromIndexes.Length; i++)
            {
                fromIndexes[i] = toIndexes[i];
            }


            ColoringLikeSelection();

        }


        private void ColoringLikeSelection()
        {

            // 色づけて擬似的に選択させる
            // 手動ソート中ということを表すためにちょっとくらい色が違ってもいいのでは。。
            for (int i = 0; i < dgvChars.Rows.Count; i++)
            {
                var row = dgvChars.Rows[i];
                for (int j = 0; j < row.Cells.Count; j++)
                {
                    var stl = row.Cells[j].Style;
                    if (Array.IndexOf(dragPrevIndexes, i) >= 0)
                    {
                        if (stl.BackColor == System.Drawing.Color.Empty)
                        {
                            stl.SelectionBackColor = System.Drawing.Color.Black;
                            stl.SelectionForeColor = System.Drawing.Color.White;
                            stl.BackColor = System.Drawing.Color.Black;
                            stl.ForeColor = System.Drawing.Color.White;
                        }
                        else
                        {
                            stl.BackColor = stl.SelectionBackColor;
                            stl.ForeColor = System.Drawing.Color.White;//stl.SelectionForeColor;
                        }
                    }
                    else
                    {

                        if (stl.BackColor == System.Drawing.Color.Empty)
                        {
                            stl.SelectionBackColor = System.Drawing.Color.White;
                            stl.SelectionForeColor = System.Drawing.Color.Black;
                            stl.BackColor = System.Drawing.Color.White;
                            stl.ForeColor = System.Drawing.Color.Black;
                        }
                        else
                        {
                            stl.SelectionBackColor = stl.BackColor;
                            stl.SelectionForeColor = System.Drawing.Color.Black; //stl.ForeColor;
                        }
                    }
                }
            }
        }

        private void slideChar(int fromIndex, int toIndex)
        {


            //dgvChars.Rows[toIndex].Selected = true;


            Character temp = dlcData.Chars[fromIndex];
            int step = (toIndex > fromIndex ? 1 : -1);
            for (int i = fromIndex; i != toIndex; i += step)
            {
                dlcData.Chars[i] = dlcData.Chars[i + step];
                dgvChars.Rows[i].Cells[0].Value = GetCharNamesJpn(dlcData.Chars[i].ID, false);//  Program.CharNamesJpn[dlcData.Chars[i].ID];
                dgvChars.Rows[i].Cells[1].Value = dlcData.Chars[i].CostumeSlot.ToString();
                dgvChars.Rows[i].Cells[2].Value = dlcData.Chars[i].AddTexsCount.ToString();
                //dgvChars.Rows[i].Cells[3].Value = dlcData.Chars[i].Comment;
                showComment(i);
            }
            dlcData.Chars[toIndex] = temp;
            dgvChars.Rows[toIndex].Cells[0].Value = GetCharNamesJpn(dlcData.Chars[toIndex].ID, false);//  Program.CharNamesJpn[dlcData.Chars[toIndex].ID];
            dgvChars.Rows[toIndex].Cells[1].Value = dlcData.Chars[toIndex].CostumeSlot.ToString();
            dgvChars.Rows[toIndex].Cells[2].Value = dlcData.Chars[toIndex].AddTexsCount.ToString();
            //dgvChars.Rows[ind2].Cells[3].Value = dlcData.Chars[toIndex].Comment;
            showComment(toIndex);



            // 移動した行をフォーカスしキャラを再描画
            //ShowCharacter(toIndex);
            //NeedShowChar = false;
            //dgvChars.CurrentCell = dgvChars[1, toIndex];
            //NeedShowChar = true;

            // ソートが崩れたことを記憶
            //dgvChars.Columns[0].HeaderCell.SortGlyphDirection = SortOrder.None;
            //dgvChars.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.None;
            for (int i = 0; i < dgvChars.Columns.Count; i++)
            {
                dgvChars.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            // 色を再描画
            setEgvCharsSlotColor();
            setEgvCharsNameColor();
            setEgvCharsTextsColor();
        }

        private void dgvChars_DragDrop(object sender, DragEventArgs e)
        {

            //MessageBox.Show("ドロップイベント");


            // ドラッグされているデーターが行番号（int型）で、かつ、
            // ドラッグ ソースのデータは、ドロップ先に複写するよう指示さ
            // れている場合（すなわち、移動等の別の指示ではない場合）
            /*if (e.Data.GetDataPresent(typeof(int))
                && (e.Effect == DragDropEffects.Move))
            {

                // DragDropイベント発生時の (x,y)座標を取得
                System.Drawing.Point clientPoint = dgvChars.PointToClient(new System.Drawing.Point(e.X, e.Y));
                DataGridView.HitTestInfo hit = dgvChars.HitTest(clientPoint.X, clientPoint.Y);
                try
                {
                    dgvChars.Rows[hit.RowIndex].Selected = true;   // 該当行を選択状態に
                }
                catch
                {

                    slideChar(dragPrevIndex, dragStartIndex);
                    dragPrevIndex = dragStartIndex = -1;

                    return; // 下過ぎた場合
                }



                // ドラッグ ソースの行番号（int）を取得
                int ind = (int)e.Data.GetData(typeof(int));

                // ドロップ先としての指定位置が、有効な場合
                // （x,y座標値の取得に成功している場合）
                if (hit.RowIndex != -1 && hit.RowIndex != ind)
                {

                    int ind2 = hit.RowIndex;
                    //slideChar(ind, ind2);
                    slideChar(dragPrevIndex, ind2);
                    dragPrevIndex = dragStartIndex = -1;
                    

                }
                // ドロップ先としての指定位置が、有効でない場合
                // （x,y座標値の取得に失敗した場合）
                else
                {
                    //元に戻す


                    slideChar(dragPrevIndex, dragStartIndex);
                    dragPrevIndex = dragStartIndex = -1;
                }

            }
            else */
            if (e.Data.GetDataPresent(DataFormats.FileDrop)
         && (e.Effect == DragDropEffects.Copy))
            {

                //コントロール内にドロップされたとき実行される
                //ドロップされたすべてのファイル名を取得する
                string[] fileName =
                (string[])e.Data.GetData(DataFormats.FileDrop, false);

                if (fileName.Length <= 0) // 世の中何があるかわからないものです。
                {
                    return;
                }

                int charCount;
                try
                {
                    charCount = dlcData.Chars.Count;
                }
                catch
                {
                    charCount = 0;
                }

                int index = 0;
                if (charCount > 0)
                {

                    // Shift の押し下げで追加かどうかを切り替え
                    //（ここで MessageBox を表示する場合別スレッドでやらないとエクスプローラーが止まっtえ鬱陶しいので注意）
                    if ((Control.ModifierKeys & Keys.Shift) != Keys.Shift)
                    {
                        OpenStateFile(fileName[index++]); // シフトが押されてなければ新規読み込み
                    }

                }
                else
                {
                    OpenStateFile(fileName[index++]);
                }


                //dgvChars.Columns[0].HeaderCell.SortGlyphDirection = SortOrder.None;
                //dgvChars.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.None;
                for (int i = 0; i < dgvChars.Columns.Count; i++)
                {
                    dgvChars.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
                }


                setEgvCharsSlotColor();
                setEgvCharsNameColor();
                setEgvCharsTextsColor();
                if (dgvChars.Rows.Count <= 0)
                {
                    btnCharsDelete.Enabled = false;
                }

                // 色付けを何度も行って無駄だが、
                // コードを簡単にすることでバグのリスクを減らすほうが有益だと判断
                while (index < fileName.Length)
                {
                    AddStateFile(fileName[index++]);
                }



            }
            else
            {
                // 特に処理はなし
            }

        }

        private void dgvChars_DragEnter(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(typeof(int)) && (e.AllowedEffect == DragDropEffects.Move))
            {
                e.Effect = DragDropEffects.Move;
                return;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileName =
                    (string[])e.Data.GetData(DataFormats.FileDrop, false);
                for (int i = 0; i < fileName.Length; i++)
                {

                    if (!System.Text.RegularExpressions.Regex.IsMatch(fileName[i], @"\.[lr]st$", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        e.Effect = DragDropEffects.None;
                        return;
                    }
                }
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void dgvChars_MouseMove(object sender, MouseEventArgs e)
        {
            // キャラのドラッグ中の操作
            if (dragPrevIndexes != null && dragPrevIndexes.Length > 0 && dragStartIndexes != null && dragStartIndexes.Length == dragPrevIndexes.Length && dragHoldRelIndex >= 0)
            {
                // 現在のマウスの位置
                DataGridView.HitTestInfo hitc = dgvChars.HitTest(e.X, e.Y);


                // 現在の位置が、有効なセル上を選択している場合
                if (hitc.Type == DataGridViewHitTestType.Cell
                    && (dgvChars.NewRowIndex == -1
                        || dgvChars.NewRowIndex != hitc.RowIndex))
                {
                    if (dragPrevIndexes[dragHoldRelIndex] != hitc.RowIndex)
                    {
                        slideChar(dragPrevIndexes, hitc.RowIndex); // ここで dragPrevIndexes  の調整もしてしまう
                                                                   //dragPrevIndex = hitc.RowIndex;

                        this.Cursor = Cursors.NoMoveVert;
                    }
                }
                else
                {
                    if (dragPrevIndexes[dragHoldRelIndex] != dragStartIndexes[dragHoldRelIndex])
                    {
                        //slideChar(dragPrevIndexes, dragStartIndexes); // ここで dragPrevIndexes  の調整もしてしまう。オーバーロードで実装
                        //dragPrevIndex = dragStartIndex;

                        //this.Cursor = Cursors.No;
                        this.Cursor = Cursors.Default;
                    }
                }
            }

            // else があってもいいけどひとつ目の条件から無くてもいい。
            if (mouseDownPoint != System.Drawing.Point.Empty && dgvChars.SelectedRows.Count > 0)
            {
                //ドラッグとしないマウスの移動範囲を取得する
                System.Drawing.Rectangle moveRect = new System.Drawing.Rectangle(
                    mouseDownPoint.X - SystemInformation.DragSize.Width / 2,
                    mouseDownPoint.Y - SystemInformation.DragSize.Height / 2,
                    SystemInformation.DragSize.Width,
                    SystemInformation.DragSize.Height);
                //ドラッグとする移動範囲を超えたか調べる
                if (!moveRect.Contains(e.X, e.Y))
                {

                    DataGridView.HitTestInfo hit = dgvChars.HitTest(mouseDownPoint.X, mouseDownPoint.Y);

                    // ドラッグ元としての指定位置が、有効なセル上を選択している場合
                    if (hit.Type == DataGridViewHitTestType.Cell
                        && (dgvChars.NewRowIndex == -1
                            || dgvChars.NewRowIndex != hit.RowIndex))
                    {
                        // ドラッグ元の行
                        int ind = hit.RowIndex;



                        // 編集状態であれば解除
                        gbChars.Focus();
                        //dgvChars.Rows[dgvChars.SelectedRows[0].Index].Cells[0].Selected = true;

                        // ドラッグスタート位置を記憶
                        // は、マウスをクリックしたときにやらないと既に解除されていて意味が無い。
                        //dragStartIndex = dragPrevIndex = ind;
                        //dragStartIndexes = GetSelectedCharsIndexesArray();
                        Array.Sort(dragStartIndexes); // でもソートは必要。ここでやるのがいいでしょう。

                        // 掴んでいるのが選択したファイルのうち上から何番目であるかを
                        // ゼロオリジンで記憶
                        dragHoldRelIndex = -1;
                        for (int i = 0; i < dragStartIndexes.Length; i++)
                        {
                            if (dragStartIndexes[i] == ind)
                            {
                                dragHoldRelIndex = i;
                                break;
                            }
                        }

                        // これは想定外。でもありそう。
                        if (dragHoldRelIndex < 0)
                        {
                            return;
                        }


                        // 選択されているものを
                        // 掴んでいる前後に寄せ集める
                        for (int dir = -1; dir <= 1; dir += 2)
                        {
                            for (int i = dragHoldRelIndex + dir; i >= 0 && i < dragStartIndexes.Length; i += dir)
                            {
                                var dstIndex = ind + i - dragHoldRelIndex;
                                if (dir * (dstIndex - dragStartIndexes[i]) < 0)
                                {
                                    var temp = dlcData.Chars[dragStartIndexes[i]];
                                    dlcData.Chars[dragStartIndexes[i]] = dlcData.Chars[dstIndex];
                                    dlcData.Chars[dstIndex] = temp;
                                }
                            }
                        }

                        // dlcData の変更を表示に反映
                        for (int i = dragStartIndexes[0]; i <= dragStartIndexes[dragStartIndexes.Length - 1]; i++)
                        {
                            dgvChars.Rows[i].Cells[0].Value = GetCharNamesJpn(dlcData.Chars[i].ID, false);// Program.CharNamesJpn[dlcData.Chars[i].ID];
                            dgvChars.Rows[i].Cells[1].Value = dlcData.Chars[i].CostumeSlot.ToString();
                            dgvChars.Rows[i].Cells[2].Value = dlcData.Chars[i].AddTexsCount.ToString();
                            //dgvChars.Rows[i].Cells[3].Value = dlcData.Chars[i].Comment;
                            showComment(i);
                        }



                        // 選択は連続するが、今後の仕様変更を想定して選択されたものは全て覚えておく
                        dragPrevIndexes = new int[dragStartIndexes.Length];
                        for (int i = 0; i < dragPrevIndexes.Length; i++)
                        {
                            dragPrevIndexes[i] = ind + i - dragHoldRelIndex;
                        }

                        // コイツが邪魔をするので黙らせる。
                        dgvChars_SelectionChanged_RepairingSelection = true;


                        // ドラッグ中、DGV の選択機能は全くあてにならないのでその表示が見えないようにする
                        //dgvChars.RowsDefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
                        //dgvChars.RowsDefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;


                        // 色づけて擬似的に選択させる
                        // 手動ソート中ということを表すためにちょっとくらい色が違ってもいいのでは。。
                        ColoringLikeSelection();

                        /*
                        // 実際にはここの時点で dragPrevIndexes が選択状態になっていないのでそうする
                        // dragPrevIndexes の算出が複雑になったり不要になったりする仕様変更を考慮して上とは別に書いておく
                        dgvChars.ClearSelection();
                        for (int i = 0; i < dragPrevIndexes.Length; i++)
                        {
                            dgvChars.Rows[dragPrevIndexes[i]].Selected = true;
                        }
                        */



                        this.Cursor = Cursors.NoMoveVert;

                        //MessageBox.Show(dragStartIndexes.Length.ToString());

                        // ちょっとドラッグイベントを使うのをやめてみる
                        //DoDragDrop(ind, DragDropEffects.Move);
                    }
                    // ドラッグ元の指定位置が、有効なセル上を選択していない場合
                    else
                    {
                        // 指定行は、ドラッグ&ドロップの対象ではないので、処理を終了
                        return;
                    }


                    mouseDownPoint = System.Drawing.Point.Empty;

                }
            }
        }

        private void dgvChars_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDownPoint = System.Drawing.Point.Empty;

            // 現在のマウスの位置
            DataGridView.HitTestInfo hitc = dgvChars.HitTest(e.X, e.Y);


            if (hitc.Type == DataGridViewHitTestType.Cell)
            {
                dgvChars.CurrentCell = dgvChars.Rows[hitc.RowIndex].Cells[hitc.ColumnIndex];
            }

            // ドラッグが終わった時の動作
            if (dragPrevIndexes != null && dragStartIndexes != null)
            {


                // 現在の位置が、有効なセル上を選択している場合
                if (hitc.Type == DataGridViewHitTestType.Cell
                    && (dgvChars.NewRowIndex == -1
                        || dgvChars.NewRowIndex != hitc.RowIndex))
                {
                    if (dragPrevIndexes[dragHoldRelIndex] != hitc.RowIndex)
                    {
                        slideChar(dragPrevIndexes, hitc.RowIndex);
                    }
                }
                else
                {
                    //if (dragPrevIndexes[dragHoldRelIndex] != dragStartIndexes[dragHoldRelIndex])
                    //{
                    //    slideChar(dragPrevIndexes, dragStartIndexes);
                    //}
                }

                // 擬似選択状態を本当の選択状態にコピー
                for (int i = 0; i < dgvChars.Rows.Count; i++)
                {
                    dgvChars.Rows[i].Selected = (Array.IndexOf(dragPrevIndexes, i) >= 0);
                }


                dragPrevIndexes = dragStartIndexes = null;
                dragHoldRelIndex = -1;

                this.Cursor = Cursors.Default;




                // 選択表示を普通の方法に戻す
                for (int i = 0; i < dgvChars.Rows.Count; i++)
                {
                    var row = dgvChars.Rows[i];
                    for (int j = 0; j < row.Cells.Count; j++)
                    {
                        var stl = row.Cells[j].Style;
                        stl.SelectionBackColor = System.Drawing.Color.Empty;
                        stl.SelectionForeColor = System.Drawing.Color.Empty;
                        stl.BackColor = System.Drawing.Color.Empty;
                        stl.ForeColor = System.Drawing.Color.Empty;
                    }
                }

                // キーの下をカレントに
                if (hitc.Type == DataGridViewHitTestType.Cell)
                {
                    dgvChars.CurrentCell = dgvChars.Rows[hitc.RowIndex].Cells[hitc.ColumnIndex];
                }

                // コイツを復活させる。
                dgvChars_SelectionChanged_RepairingSelection = false;

                // 色を再描画
                setEgvCharsSlotColor();
                //MessageBox.Show("2");
                setEgvCharsNameColor();
                setEgvCharsTextsColor();
                //MessageBox.Show("1");

                dgvChars.Focus();

            }

            // ドラッグが起こらずにマウスが離された場合選択をそこだけにする
            else if (e.Button == MouseButtons.Left)
            {
                if (dgvChars_MouseDown_dontSelectOne)
                {
                    dgvChars_MouseDown_dontSelectOne = false;
                }
                else if (hitc.Type == DataGridViewHitTestType.Cell)
                {
                    // 選択をオフからのオンにするのが悪いのか、これだとインナーの編集状態が解除されてしまう
                    // コメントは解除されないのが謎だけど
                    //dgvChars.ClearSelection();
                    //dgvChars.Rows[hitc.RowIndex].Selected = true;

                    for (int i = 0; i < dgvChars.Rows.Count; i++)
                    {
                        dgvChars.Rows[i].Selected = (hitc.RowIndex == i);
                    }

                    dgvChars.CurrentCell = dgvChars.Rows[hitc.RowIndex].Cells[hitc.ColumnIndex];
                }

            }

            // 何にしてもキーが離されたらこれをヌルにする。
            // じゃないとショートカットキーとかが無効になったまま
            dragStartIndexes = null;


            // マウスの左ボタンが押されている場合
            var eb = e.Button;
            if (e.Button == MouseButtons.Left)
            {

                // MouseDownイベント発生時の (x,y)座標を取得
                DataGridView.HitTestInfo hit = dgvChars.HitTest(e.X, e.Y);
                if (hit.RowIndex < 0)
                {
                    // クリックした場所がコメントじゃないカラムヘッダだった場合、その列が移動できないようにする
                    // マウス UP イベントで解除
                    if (hit.Type == DataGridViewHitTestType.ColumnHeader)
                    {
                        if (hit.ColumnIndex < 3)
                        {
                            dgvChars.AllowUserToOrderColumns = true;

                        }
                        else
                        {
                            //MessageBox.Show("ここで並びを直す");
                            SedCommentColumnOrder();
                        }
                    }
                }
            }
        }

        private void リストの追加読み込みToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // btnOpenState_Click のほぼそのままコピー
            try
            {


                var pt = GetOwnOFolderrExistingParent(LoadIniString("InitialDirectory", "LST"));
                if (pt != "") openFileDialogState.InitialDirectory = pt;
                openFileDialogState.FileName = Path.GetFileName(saveFileDialogState.FileName);
                if (openFileDialogState.ShowDialog() == DialogResult.OK)
                {
                    SaveIniString("InitialDirectory", "LST", Path.GetDirectoryName(openFileDialogState.FileName));
                    //dgvChars.Columns[0].HeaderCell.SortGlyphDirection = SortOrder.None;
                    //dgvChars.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.None;
                    for (int i = 0; i < dgvChars.Columns.Count; i++)
                    {
                        dgvChars.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
                    }

                    if (openFileDialogState.FilterIndex == 1)
                    {
                        //OpenStateFile(openFileDialogState.FileName);
                        AddStateFile(openFileDialogState.FileName);
                    }
                    else
                    {
                        var bf = new BinaryFormatter();
                        using (var fs = new FileStream(openFileDialogState.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            //dlcData = (DLCData)bf.Deserialize(fs);
                            dlcData.Chars.AddRange(((DLCData)bf.Deserialize(fs)).Chars);
                        }

                        newDlc = true;
                        string cufpath = tbSavePath.Text;
                        string culbcm = tbBCMVer.Text;
                        ClearMainUI();
                        tbSavePath.Text = cufpath;
                        btnSave.Text = Program.dicLanguage["SaveDLC"];
                        btnCmpSave.Enabled = btnSave.Enabled = true;
                        btnSaveState.Enabled = true;
                        btnCharsAdd.Enabled = true;
                        //clmCos.ReadOnly = false;
                        clmInner.ReadOnly = false;
                        tbBCMVer.Text = culbcm;

                        for (int i = 0; i < dlcData.Chars.Count; i++)
                        {
                            dgvChars.Rows.Add();
                            dgvChars.Rows[i].Cells[0].Value = GetCharNamesJpn(dlcData.Chars[i].ID);// Program.CharNamesJpn[dlcData.Chars[i].ID];
                            dgvChars.Rows[i].Cells[1].Value = dlcData.Chars[i].CostumeSlot.ToString();
                            dgvChars.Rows[i].Cells[2].Value = dlcData.Chars[i].AddTexsCount.ToString();
                            //dlcData.Chars[i].Comment = "";// GetComment()";
                            //dgvChars.Rows[i].Cells[3].Value = dlcData.Chars[i].Comment;
                            showComment(i);
                        }
                        if (dlcData.Chars.Count > 0)
                        {
                            dgvChars.Rows[0].Selected = true;
                        }

                    }
                    saveFileDialogState.FileName = openFileDialogState.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void AddStateFile(string fileName)
        {
            // OpenStateFile のほぼのそのままコピー
            try
            {
                string curDLCPath = "";
                string curLstPath = "";
                string curBCMVar = "";
                try
                {
                    curDLCPath = tbSavePath.Text;
                    curLstPath = tbListPath.Text;
                    curBCMVar = tbBCMVer.Text;
                }
                catch { }
                newDlc = true;
                ClearMainUI();
                tbSavePath.Text = curDLCPath;
                tbListPath.Text = curLstPath;
                tbListPath.Select(tbListPath.Text.Length, 0);
                tbListPath.ScrollToCaret();
                tbBCMVer.Text = curBCMVar;
                // dlcData = Program.OpenState(fileName);
                int cuoCount = dlcData.Chars.Count;
                dlcData.Chars.AddRange(Program.OpenState(fileName).Chars);

                MakeColumnsFromDLCData(dlcData);
                //btnSave.Text = Program.dicLanguage["SaveDLC"];
                setBtnSave();
                btnCmpSave.Enabled = btnSave.Enabled = true;
                btnSaveState.Enabled = true;
                btnCharsAdd.Enabled = true;
                btnHStylesAdd.Enabled = true;
                //clmCos.ReadOnly = false;
                clmInner.ReadOnly = false;
                if (dlcData.Chars.Count > 0)
                {
                    //tbSavePath.Text = dlcData.SavePath;
                    //tbBCMVer.Text = dlcData.BcmVer.ToString();
                    for (int i = 0; i < dlcData.Chars.Count; i++)
                    {
                        dgvChars.Rows.Add();
                        dgvChars.Rows[i].Cells[0].Value = GetCharNamesJpn(dlcData.Chars[i].ID);// Program.CharNamesJpn[dlcData.Chars[i].ID];
                        dgvChars.Rows[i].Cells[1].Value = dlcData.Chars[i].CostumeSlot.ToString();
                        dgvChars.Rows[i].Cells[2].Value = dlcData.Chars[i].AddTexsCount.ToString();
                        //dlcData.Chars[i].Comment = "";// GetComment()";
                        //dgvChars.Rows[i].Cells[3].Value = dlcData.Chars[i].Comment;
                        showComment(i);
                    }
                    dgvChars.ClearSelection();
                    if (cuoCount < dlcData.Chars.Count)
                    {
                        for (int i = cuoCount; i < dlcData.Chars.Count; i++)
                        {
                            dgvChars.Rows[i].Selected = true;
                        }
                    }
                    else
                    {
                        dgvChars.Rows[dlcData.Chars.Count - 1].Selected = true;
                    }

                }

                setEgvCharsSlotColor();
                setEgvCharsNameColor();
                setEgvCharsTextsColor();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Program.dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void コピーToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dgvChars.SelectedRows.Count <= 0)
            {
                MessageBox.Show("コスチューム非選択時にスロット変更の操作が行われました。これは想定されない動作です。作者へご報告頂けると幸いです。");
                return;
            }


            // 選択キャラのインデックスを取得
            var SortedSlectedCharIndexes = GetSelectedCharsIndexesArray();
            if (SortedSlectedCharIndexes.Length <= 0)
            {
                return;
            }
            Array.Sort(SortedSlectedCharIndexes);

            var newChars = new Character[SortedSlectedCharIndexes.Length];
            var curChars = dlcData.Chars;
            for (int j = 0; j < newChars.Length; j++)
            {
                var curChar = curChars[SortedSlectedCharIndexes[j]];

                newChars[j] = new Character();
                newChars[j].ID = curChar.ID;
                newChars[j].Female = curChar.Female;
                newChars[j].CostumeSlot = curChar.CostumeSlot; // これもコピーしてしまおう
                newChars[j].Comment = curChar.Comment;
                newChars[j].AddTexsCount = curChar.AddTexsCount;
                for (int i = 0; i < curChar.HStyles.Count; i++)
                {
                    newChars[j].HStyles.Add(new Hairstyle(curChar.HStyles[i].Hair, curChar.HStyles[i].Face));
                }
                for (int i = 0; i < curChar.Files.Length; i++)
                {
                    newChars[j].Files[i] = curChar.Files[i];
                }
            }

            int index = SortedSlectedCharIndexes[0];
            for (int i = newChars.Length - 1; i >= 0; i--)
            {
                dlcData.Chars.Insert(index + 0 + 0, newChars[i]);

                dgvChars.Rows.Insert(index + 0 + 0);
                dgvChars.Rows[index + 0 + 0].Cells[0].Value = GetCharNamesJpn(newChars[i].ID);// Program.CharNamesJpn[newChars[i].ID];
                dgvChars.Rows[index + 0 + 0].Cells[1].Value = newChars[i].CostumeSlot.ToString();
                dgvChars.Rows[index + 0 + 0].Cells[2].Value = newChars[i].AddTexsCount.ToString();
                //dgvChars.Rows[index + 0 + 0].Cells[3].Value = newChar.Comment;
                showComment(index + 0 + 0);
                //dgvChars.Rows[index + 0 + 0].Selected = true;
            }
            // Insert 時によくわからない選択がされるので全部終わってから選択
            dgvChars.ClearSelection();
            for (int i = 0; i < newChars.Length; i++)
            {
                dgvChars.Rows[index + 0 + i].Selected = true;
            }

            

            setEgvCharsSlotColor();

            //MessageBox.Show("2");
            setEgvCharsNameColor();

            //MessageBox.Show("3");
            setEgvCharsTextsColor();

            //MessageBox.Show("4");
        }

        private void setEgvCharsSlotColor()
        {
            Program.SlotTable<int> slotCount = new Program.SlotTable<int>(0);
            for (int i = 0; i < dgvChars.Rows.Count; i++)
            {
                slotCount[dlcData.Chars[i]]++;
            }
            //Program.SlotTable<bool> NotComIniSlotTable = GetNotComIniSlotTable();
            Program.SlotTable<bool> UsableSlotTable = GetUsableSlotTable();
            Program.SlotTable<string> ComIniSlotCommentTable = GetComIniSlotCommentTable();
            Program.SlotTable<bool> NotComIniSlotTable = new Program.SlotTable<bool>(true);
            for (int i = 0; i < ComIniSlotCommentTable.Count(); i++)
            {
                for (int j = 0; j < ComIniSlotCommentTable[i].Length; j++)
                {
                    var ComIniSlotComment = ComIniSlotCommentTable[i, j];
                    NotComIniSlotTable[i, j] = (ComIniSlotCommentTable[i, j] == "");
                }
            }


            for (int i = 0; i < dgvChars.Rows.Count; i++)
            {
                dgvChars.Rows[i].Cells[1].Style.BackColor = getSlotBackColor(slotCount, NotComIniSlotTable, UsableSlotTable, dlcData.Chars[i]);
                dgvChars.Rows[i].Cells[1].Style.SelectionBackColor = getSlotSelectionBackColor(slotCount, NotComIniSlotTable, UsableSlotTable, dlcData.Chars[i]);

                /*
                switch (slotCount[dlcData.Chars[i]])
                {
                    case default(int): // = 0
                        dgvChars.Rows[i].Cells[1].Style.BackColor = System.Drawing.Color.Red;
                        dgvChars.Rows[i].Cells[1].Style.SelectionBackColor = System.Drawing.Color.Red;
                        //dgvChars.Rows[i].Cells[1].Style.SelectionForeColor = System.Drawing.Color.Black;
                        break;

                    case 1:
                        if (!NotComIniSlotTable[dlcData.Chars[i]])
                        {
                            dgvChars[1, i].Style.BackColor = System.Drawing.Color.PaleTurquoise;
                            dgvChars[1, i].Style.SelectionBackColor = System.Drawing.Color.DarkBlue;
                            //dgvChars[1, i].Style.SelectionForeColor = System.Drawing.Color.Black;
                        }
                        else if(!UsableSlotTable[dlcData.Chars[i]])
                        {
                            dgvChars[1, i].Style.BackColor = System.Drawing.Color.Orange;
                            dgvChars[1, i].Style.SelectionBackColor = System.Drawing.Color.Chocolate;
                            //dgvChars[1, i].Style.BackColor = System.Drawing.Color.Red;
                        }
                        else
                        {
                            dgvChars[1, i].Style.BackColor = System.Drawing.Color.Empty;
                            dgvChars[1, i].Style.SelectionBackColor = System.Drawing.Color.Empty;
                            //dgvChars[1, i].Style.SelectionForeColor = System.Drawing.Color.Empty;
                        }
                        break;

                    default:
                        dgvChars[1, i].Style.BackColor = System.Drawing.Color.LightGray;
                        dgvChars[1, i].Style.SelectionBackColor = System.Drawing.Color.DimGray;
                        //dgvChars[1, i].Style.SelectionForeColor = System.Drawing.Color.Empty;
                        break;
                }
                */
            }
        }

        private System.Drawing.Color getSlotBackColor(Program.SlotTable<int> slotCount, Program.SlotTable<bool> NotComIniSlotTable, Program.SlotTable<bool> UsableSlotTable, Character Char)
        {

            switch (slotCount[Char])
            {
                case default(int): // = 0
                    return System.Drawing.Color.Red;
                //return System.Drawing.Color.PaleTurquoise;

                case 1:
                    if (!NotComIniSlotTable[Char])
                    {
                        return System.Drawing.Color.PaleTurquoise;
                    }
                    else if (!UsableSlotTable[Char])
                    {
                        return System.Drawing.Color.Orange;
                    }
                    else
                    {
                        return System.Drawing.Color.Empty;
                    }

                default:
                    return System.Drawing.Color.LightGray;
            }
        }

        private System.Drawing.Color getSlotSelectionBackColor(Program.SlotTable<int> slotCount, Program.SlotTable<bool> NotComIniSlotTable, Program.SlotTable<bool> UsableSlotTable, Character Char)
        {

            switch (slotCount[Char])
            {
                case default(int): // = 0
                    return System.Drawing.Color.Red;

                case 1:
                    if (!NotComIniSlotTable[Char])
                    {
                        return System.Drawing.Color.DarkBlue; ;
                    }
                    else if (!UsableSlotTable[Char])
                    {
                        return System.Drawing.Color.Chocolate;
                    }
                    else
                    {
                        return System.Drawing.Color.Empty;
                    }

                default:
                    return System.Drawing.Color.DimGray;
            }
        }


        private void setEgvCharsTextsColor()
        {

            for (int i = 0; i < dgvChars.Rows.Count; i++)
            {
                if (dlcData.Chars[i].AddTexsCount <= getAvailableTextsCount(dlcData.Chars[i]))
                {
                    dgvChars[2, i].Style.BackColor = System.Drawing.Color.Empty;
                    dgvChars[2, i].Style.SelectionBackColor = System.Drawing.Color.Empty;
                }
                else
                {
                    dgvChars[2, i].Style.BackColor = System.Drawing.Color.LightGray;
                    dgvChars[2, i].Style.SelectionBackColor = System.Drawing.Color.DimGray;
                }
            }



        }

        private void setEgvCharsNameColor()
        {
            bool changed = false;
            int selectedrow;
            if (dgvChars.SelectedRows.Count == 1)
            {
                selectedrow = dgvChars.SelectedRows[0].Index;
            }
            else
            {
                selectedrow = -1;
            }
            /*
            int selectedrow = -1;
            try
            {
                selectedrow = dgvChars.SelectedRows[0].Index;
            }
            catch { }
            */
            for (int i = 0; i < dgvChars.Rows.Count; i++)
            {
                if ((!newDlc) || CheckCharFile(dlcData.Chars[i]))
                {
                    if (dgvChars[0, i].Style.BackColor != System.Drawing.Color.Empty)
                    {
                        changed = (changed || selectedrow == i);
                        dgvChars[0, i].Style.BackColor = System.Drawing.Color.Empty;
                        dgvChars[0, i].Style.SelectionBackColor = System.Drawing.Color.Empty;
                    }

                }
                else
                {
                    if (dgvChars[0, i].Style.BackColor != System.Drawing.Color.LightGray)
                    {
                        changed = (changed || selectedrow == i);
                        dgvChars[0, i].Style.BackColor = System.Drawing.Color.LightGray;
                        dgvChars[0, i].Style.SelectionBackColor = System.Drawing.Color.DimGray;
                    }
                }
            }
            if (changed)
            {
                try
                {
                    bool[] selecteds = new bool[dgvFiles.Rows.Count];
                    for (int i = 0; i < selecteds.Length; i++)
                    {
                        selecteds[i] = dgvFiles.Rows[i].Selected;
                    }
                    ShowFiles(selectedrow);
                    for (int i = 0; i < selecteds.Length; i++)
                    {
                        dgvFiles.Rows[i].Selected = selecteds[i];
                    }
                }
                catch { }

            }
        }

        private bool CheckCharFile(Character Char)
        {
            string[] Files = Char.Files;
            bool TMC = false, TMCL = false, C = false, P = false;

            //public static readonly string[] FileOrder = new string[12] { ".TMC", ".TMCL", ".---C", "1.--H", "1.--HL", "2.--H", "2.--HL", "3.--H", "3.--HL", "4.--H", "4.--HL", ".--P" };

            cbTMC.Enabled = cbTMCL.Enabled = cbC.Enabled = cbP.Enabled = cb1H.Enabled = cb2H.Enabled = cb3H.Enabled = cb4H.Enabled = true;

            if (Files[0] == null)
            {
            }
            else if (File.Exists(Files[0]))
            {
                TMC = true;
            }
            cbTMC.Update();
            if (Files[1] == null)
            {
            }
            else if (File.Exists(Files[1]))
            {
                TMCL = true;
            }
            if (Files[2] == null)
            {
            }
            else if (File.Exists(Files[2]))
            {
                C = true;
            }
            if (Files[11] == null)
            {
            }
            else if (File.Exists(Files[11]))
            {
                P = true;
            }

            /*
            for (int j = 0; j < Files.Length; j++)
            {
                if (Files[j] != null)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(Files[j], @"\.tmc$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                        && System.IO.File.Exists(Files[j]))
                    {
                        TMC = true;
                    }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(Files[j], @"\.tmcl$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                        && System.IO.File.Exists(Files[j]))
                    {
                        TMCL = true;
                    }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(Files[j], @"\.---c$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                        && System.IO.File.Exists(Files[j]))
                    {
                        C = true;
                    }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(Files[j], @"\.--p$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                        && System.IO.File.Exists(Files[j]))
                    {
                        P = true;
                    }
                }
            }
            */

            return TMC && TMCL && C && P;
        }

        private void setFileCheckbox()
        {

            if (dgvChars.SelectedRows.Count != 1)
            {
                MessageBox.Show("コスチューム非選択時または複数選択時にチェックボックス設定メソッドが呼び出されました。これは想定されない動作です。作者へご報告頂けると幸いです。");
                return;
            }

            cbTMC.Enabled = cbTMCL.Enabled = cbC.Enabled = cbP.Enabled = cb1H.Enabled = cb2H.Enabled = cb3H.Enabled = cb4H.Enabled = true;

            string[] Files;
            try
            {
                Files = dlcData.Chars[dgvChars.SelectedRows[0].Index].Files;
            }
            catch
            {
                return;
            }

            if (Files[0] == null)
            {
                cbTMC.CheckState = CheckState.Unchecked;
            }
            else if (File.Exists(Files[0]))
            {
                cbTMC.CheckState = CheckState.Checked;
            }
            else
            {
                cbTMC.CheckState = CheckState.Indeterminate;
            }
            cbTMC.Update();
            if (Files[1] == null)
            {
                cbTMCL.CheckState = CheckState.Unchecked;
            }
            else if (File.Exists(Files[1]))
            {
                cbTMCL.CheckState = CheckState.Checked;
            }
            else
            {
                cbTMCL.CheckState = CheckState.Indeterminate;
            }
            if (Files[2] == null)
            {
                cbC.CheckState = CheckState.Unchecked;
            }
            else if (File.Exists(Files[2]))
            {
                cbC.CheckState = CheckState.Checked;
            }
            else
            {
                cbC.CheckState = CheckState.Indeterminate;
            }
            if (Files[11] == null)
            {
                cbP.CheckState = CheckState.Unchecked;
            }
            else if (File.Exists(Files[11]))
            {
                cbP.CheckState = CheckState.Checked;
            }
            else
            {
                cbP.CheckState = CheckState.Indeterminate;
            }

            CheckState[] H = new CheckState[4];
            for (int i = 3; i <= 9; i += 2)
            {
                if (Files[i] == null || Files[i + 1] == null)
                {
                    H[(i - 3) / 2] = CheckState.Unchecked;
                }
                else if (File.Exists(Files[i]) && File.Exists(Files[i + 1]))
                {
                    H[(i - 3) / 2] = CheckState.Checked;
                }
                else
                {
                    H[(i - 3) / 2] = CheckState.Indeterminate;
                }
            }
            cb1H.CheckState = H[0];
            cb2H.CheckState = H[1];
            cb3H.CheckState = H[2];
            cb4H.CheckState = H[3];
        }

        private byte getAvailableTextsCount(Character Char)
        {
            string[] Files = Char.Files;

            // この数え方は不適切
            /*
            byte countH = 0, countHL = 0;
            for (int j = 0; j < Files.Length; j++)
            {
                if (Files[j] != null)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(Files[j], @"\.--H$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                        && System.IO.File.Exists(Files[j]))
                    {
                        countH++;
                    }
                    else if (System.Text.RegularExpressions.Regex.IsMatch(Files[j], @"\.--HL$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                        && System.IO.File.Exists(Files[j]))
                    {
                        countHL++;
                    }
                }
            }
            int HCount = (countH < countHL ? countH : countHL);
            */

            // これを参考に。
            // public static readonly string[] FileOrder = new string[12] { ".TMC", ".TMCL", ".---C", "1.--H", "1.--HL", "2.--H", "2.--HL", "3.--H", "3.--HL", "4.--H", "4.--HL", ".--P" };
            int HCount = 0;
            if (Files[3] != null && Files[4] != null && File.Exists(Files[3]) && File.Exists(Files[4]))
            {
                HCount++;

                if (Files[5] != null && Files[6] != null && File.Exists(Files[5]) && File.Exists(Files[6]))
                {
                    HCount++;

                    if (Files[7] != null && Files[8] != null && File.Exists(Files[7]) && File.Exists(Files[8]))
                    {
                        HCount++;

                        if (Files[9] != null && Files[10] != null && File.Exists(Files[9]) && File.Exists(Files[10]))
                        {
                            HCount++;
                        }
                    }
                }
            }
            return (byte)(HCount > 1 ? HCount : 1);
        }





        private void OpenWithExplorer(string path)
        {
            try // path が間違っている可能性を考慮
            {
                if (File.Exists(path))
                {
                    System.Diagnostics.Process.Start("EXPLORER.EXE", @"/select,""" + path + @"""");
                }
                else
                {
                    while (!String.IsNullOrEmpty(Path.GetDirectoryName(path)))
                    {
                        if (System.IO.Directory.Exists(path))
                        {
                            System.Diagnostics.Process.Start("EXPLORER.EXE", path);
                            return;
                        }
                        path = Path.GetDirectoryName(path);
                    }
                }
            }
            catch { }
        }


        private void dgvChars_KeyDown(object sender, KeyEventArgs e)
        {
            if (dragStartIndexes != null) return;

            /*
            // テスト用のコード
            if (e.KeyCode == Keys.T && e.Control)
            {
                //MessageBox.Show("hero");

                
                //DataTableを作成する
                System.Data.DataTable dt = new System.Data.DataTable();
                //文字列型のWeek列を追加する
                dt.Columns.Add("Week", typeof(string));
                //Week列に日曜日～土曜日のデータを追加する
                dt.Rows.Add("0");
                dt.Rows.Add("1");
                dt.Rows.Add("2");
                //DataGridViewにデータソースを設定する
                dgvChars.DataSource = dt;
                

                //DataGridViewComboBoxColumnを作成
                DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
                //ComboBoxのリストに表示する項目を設定する
                column.Items.Add("0");
                column.Items.Add("1");
                column.Items.Add("2");
                column.Items.Add("3");
                column.Items.Add("4");
                column.Items.Add("5");
                column.Items.Add("6");

                //DataGridView1に現在存在しているWeek列と
                //　今作成したDataGridViewComboBoxColumnを入れ替える
                //表示する列の名前を設定する
                column.DataPropertyName = dgvChars.Columns[2].DataPropertyName;
                //以下のようにしても同じ
                //column.DataPropertyName = "Week";
                //現在Week列が存在している位置に挿入する
                dgvChars.Columns[2].set
                //今までのWeek列を削除する
                dgvChars.Columns.RemoveAt(2);
                //挿入した列の名前を「Week」とする
                column.Name = "Dtl";
                

            return;

            }
            */



            var kc = e.KeyCode;
            if (kc == Keys.C && CKeyUp && e.Control)
            {
                CKeyUp = false;
                DLCData dlcData2 = new DLCData();

                try // dlcData が作られる前に呼び出されることもある
                {
                    //dlcData2.SavePath = dlcData.SavePath; 意図がわからないけど、この式の右辺は必要になるまで設定されないらしい
                    dlcData2.SavePath = tbSavePath.Text;
                    dlcData2.BcmVer = dlcData.BcmVer;
                    dlcData2.skipRead = dlcData.skipRead;
                }
                catch
                {
                    return;
                }

                if (dgvChars.SelectedRows.Count > 0)
                {
                    // 選択中のものをコピー
                    var SortedSelectedIndexes = GetSelectedCharsIndexesArray();
                    Array.Sort(SortedSelectedIndexes);
                    for (int i = 0; i < SortedSelectedIndexes.Length; i++)
                    {
                        dlcData2.Chars.Add(dlcData.Chars[SortedSelectedIndexes[i]]);
                    }
                    Clipboard.SetText(Program.dlcData2string(dlcData2));

                }
            }
            else if (kc == Keys.V && VKeyUp && e.Control)
            {
                VKeyUp = false;

                if (Clipboard.GetDataObject().GetDataPresent(DataFormats.FileDrop))
                {
                    string[] paths = (string[])Clipboard.GetDataObject().GetData(DataFormats.FileDrop);
                    if (AddFiles(paths, true))
                    {
                        //var charselectedrows = GetSelectedCharsIndexesArray();
                        int CharSelected = GetFirstSelectedCharIndex();
                        if (CharSelected < 0)
                        {
                            return;
                        }
                        /*
                        int CharSelected;
                        try
                        {
                            CharSelected = dgvChars.SelectedRows[0].Index;
                        }
                        catch
                        {
                            return;
                        }
                        */
                        for (int i = 0; i < dlcData.Chars[CharSelected].Files.Length /* = 12 */; i++)
                        {
                            dlcData.Chars[CharSelected].Files[i] = null;
                        }

                        clikedForm = "dgvFiles";
                        貼り付けCtrlVToolStripMenuItem_Click(null, null);
                    }
                }
                else if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
                {



                    if (PasteHStyles(true))
                    {

                        int CharSelected = GetFirstSelectedCharIndex();
                        if (CharSelected < 0)
                        {
                            return;
                        }
                        /*
                        int CharSelected;
                        try
                        {
                            CharSelected = dgvChars.SelectedRows[0].Index;
                        }
                        catch
                        {
                            return;
                        }
                        */
                        dlcData.Chars[CharSelected].HStyles.Clear();

                        // 直後にペーストが控えているのでこれは不要
                        //ShowHairstyles(CharSelected);

                        // 髪型の貼り付けは現在の表示を見て選択位置に挿入しようとする
                        // なのでその前に整合性を取っておく必要がある。
                        ShowHairstyles(CharSelected);


                        // その後貼り付け処理
                        clikedForm = "dgvHStyles";
                        貼り付けCtrlVToolStripMenuItem_Click(null, null);
                    }
                    else
                    {
                        if (dgvChars.SelectedRows.Count <= 0)
                        {
                            return;
                        }


                        string str = Clipboard.GetText();

                        DLCData dlcData2 = Program.string2dlcData(str);

                        if (dlcData2 != null && dlcData2.Chars.Count > 0)
                        {
                            // 今選択されている場所。挿入に使う
                            int index = GetFirstSelectedCharIndex();
                            if (index < 0)
                            {
                                index = dgvChars.Rows.Count;
                            }
                            int index0 = index;


                            var dlcData3 = new DLCData();
                            dlcData3.Chars.AddRange(dlcData.Chars);
                            dlcData3.Chars.AddRange(dlcData2.Chars);
                            MakeColumnsFromDLCData(dlcData3);

                            for (int j = 0; j < dlcData2.Chars.Count; j++)
                            {
                                Character curChar;
                                Character newChar;
                                try
                                {
                                    curChar = dlcData2.Chars[j];
                                    newChar = new Character();
                                    newChar.ID = curChar.ID;
                                    newChar.Female = curChar.Female;
                                    newChar.CostumeSlot = curChar.CostumeSlot; // これもコピーしてしまおう
                                    newChar.Comment = curChar.Comment;
                                    newChar.AddTexsCount = curChar.AddTexsCount;
                                    for (int i = 0; i < curChar.HStyles.Count; i++)
                                    {
                                        newChar.HStyles.Add(new Hairstyle(curChar.HStyles[i].Hair, curChar.HStyles[i].Face));
                                    }
                                    for (int i = 0; i < curChar.Files.Length /* = 12 */; i++)
                                    {
                                        newChar.Files[i] = curChar.Files[i];
                                    }
                                }
                                catch
                                {
                                    continue;
                                }

                                // 挿入。今はこっち
                                // ボタンでのコピーだけ index + 1 だけどあっちは index == Length を許してないから間違いではない。
                                dlcData.Chars.Insert(index, newChar);
                                dgvChars.Rows.Insert(index);
                                dgvChars.Rows[index].Cells[0].Value = GetCharNamesJpn(newChar.ID);// Program.CharNamesJpn[newChar.ID];
                                dgvChars.Rows[index].Cells[1].Value = newChar.CostumeSlot.ToString();
                                dgvChars.Rows[index].Cells[2].Value = newChar.AddTexsCount.ToString();
                                //dgvChars.Rows[index].Cells[3].Value = newChar.Comment;
                                showComment(index);
                                // dgvChars.Rows[index].Selected = true; これは最後でいいですね

                                // 複数貼り付けることを考えるとこれを付けるか挿入の順番を逆にしないと結果が逆転する
                                index++;

                                /* これは最後に付け加える処理
                                dlcData.Chars.Add(newChar);

                                dgvChars.Rows.Add();
                                dgvChars.Rows[dgvChars.Rows.Count - 1].Cells[0].Value = GetCharNamesJpn(newChar.ID);// Program.CharNamesJpn[newChar.ID];
                                dgvChars.Rows[dgvChars.Rows.Count - 1].Cells[1].Value = newChar.CostumeSlot.ToString();
                                dgvChars.Rows[dgvChars.Rows.Count - 1].Cells[2].Value = newChar.AddTexsCount.ToString();
                                //dgvChars.Rows[dgvChars.Rows.Count - 1].Cells[3].Value = newChar.Comment;
                                showComment(dgvChars.Rows.Count - 1);
                                dgvChars.Rows[dgvChars.Rows.Count - 1].Selected = true;
                                */
                            }

                            //dgvChars.Columns[0].HeaderCell.SortGlyphDirection = SortOrder.None;
                            //dgvChars.Columns[1].HeaderCell.SortGlyphDirection = SortOrder.None;
                            for (int i = 0; i < dgvChars.Columns.Count; i++)
                            {
                                dgvChars.Columns[i].HeaderCell.SortGlyphDirection = SortOrder.None;
                            }


                            setEgvCharsSlotColor();
                            setEgvCharsNameColor();
                            setEgvCharsTextsColor();

                            dgvChars.ClearSelection();
                            for (int i = 0; i < dlcData2.Chars.Count; i++)
                            {
                                dgvChars.Rows[index0 + i].Selected = true;
                            }

                            /*
                            // 選択するのは一番上で。
                            try // 一つも読み込まれる前だとここで落ちる
                            {
                                dgvChars.Rows[index0].Selected = true;
                            }
                            catch
                            {
                                return;
                            }
                            */

                        }
                    }
                }
            }
            else if (kc == Keys.Delete && DeleteKeyUp)
            {
                DeleteKeyUp = false;

                btnCharsDelete_Click(null, null);


            }
        }

        private void dgvHStyles_KeyDown(object sender, KeyEventArgs e)
        {

            if (dragStartIndexes != null) return;

            if (e.KeyCode == Keys.Delete && DeleteKeyUp)
            {
                btnHStylesDelete_Click(null, null);
                DeleteKeyUp = false;
            }
            else if (e.KeyCode == Keys.C && CKeyUp && e.Control)
            {
                CKeyUp = false;
                CopyHStyles();
            }
            else if (e.KeyCode == Keys.V && VKeyUp && e.Control)
            {
                VKeyUp = false;
                PasteHStyles(false);
            }
        }

        /*
        // common, initial 情報を完全にファイルで管理する更新にともなって使われなくなる。
        private Program.SlotTable<bool> GetNotComIniSlotTable() // Dictionary とかを使ったほうがいいのだろうけどどうせここは計算時間の主要項じゃないし。
        {

            Program.SlotTable<bool> NotComIniSlotTable = new Program.SlotTable<bool>(true);

            // chara_common, chara_initial は直に書く。その方が楽だしトラブルも減りそうだし何より高速になる。

            NotComIniSlotTable[0, 0] = NotComIniSlotTable[0, 1] = NotComIniSlotTable[0, 2] = false; // ザック
            NotComIniSlotTable[1, 3] = NotComIniSlotTable[1, 5] = NotComIniSlotTable[1, 8] = false; // ティナ
            NotComIniSlotTable[2, 1] = NotComIniSlotTable[2, 2] = false; // ジャン・リー
            NotComIniSlotTable[3, 1] = NotComIniSlotTable[3, 2] = NotComIniSlotTable[3, 3] = false; // アイン
            NotComIniSlotTable[4, 1] = NotComIniSlotTable[4, 2] = NotComIniSlotTable[4, 3] = false; // リュウ・ハヤブサ
            NotComIniSlotTable[5, 3] = NotComIniSlotTable[5, 4] = NotComIniSlotTable[5, 5] = NotComIniSlotTable[5, 6] = false; // かすみ
            NotComIniSlotTable[6, 1] = NotComIniSlotTable[6, 2] = false; // ゲン・フー
            NotComIniSlotTable[7, 3] = NotComIniSlotTable[7, 5] = NotComIniSlotTable[7, 8] = false; // エレナ
            NotComIniSlotTable[8, 0] = NotComIniSlotTable[8, 1] = NotComIniSlotTable[8, 2] = NotComIniSlotTable[8, 3] = false; // レオン
            NotComIniSlotTable[9, 0] = NotComIniSlotTable[9, 1] = NotComIniSlotTable[9, 2] = false; // バース
            NotComIniSlotTable[10, 3] = NotComIniSlotTable[10, 5] = NotComIniSlotTable[10, 6] = false; // こころ
            NotComIniSlotTable[11, 1] = NotComIniSlotTable[11, 2] = false; // ハヤテ
            NotComIniSlotTable[12, 3] = NotComIniSlotTable[12, 5] = NotComIniSlotTable[12, 8] = false; // レイファン
            NotComIniSlotTable[13, 3] = NotComIniSlotTable[13, 4] = NotComIniSlotTable[13, 5] = NotComIniSlotTable[13, 6] = false; // あやね
            NotComIniSlotTable[14, 0] = NotComIniSlotTable[14, 1] = NotComIniSlotTable[14, 2] = false; // エリオット
            NotComIniSlotTable[15, 3] = NotComIniSlotTable[15, 5] = NotComIniSlotTable[15, 8] = false; // リサ
            // Alpha-152
            NotComIniSlotTable[19, 1] = NotComIniSlotTable[19, 2] = false; // ブラッド・ウォン
            NotComIniSlotTable[20, 3] = NotComIniSlotTable[20, 5] = NotComIniSlotTable[20, 6] = false; // クリスティ
            NotComIniSlotTable[21, 3] = NotComIniSlotTable[21, 5] = NotComIniSlotTable[21, 6] = false; // ヒトミ
            NotComIniSlotTable[24, 0] = NotComIniSlotTable[24, 1] = NotComIniSlotTable[24, 2] = false; // バイマン
            NotComIniSlotTable[29, 0] = NotComIniSlotTable[29, 1] = NotComIniSlotTable[29, 2] = false; // リグ
            NotComIniSlotTable[30, 3] = NotComIniSlotTable[30, 5] = NotComIniSlotTable[30, 6] = false; // ミラ
            NotComIniSlotTable[31, 1] = NotComIniSlotTable[31, 2] = false; // アキラ
            NotComIniSlotTable[32, 1] = NotComIniSlotTable[32, 2] = false; // サラ
            NotComIniSlotTable[33, 1] = NotComIniSlotTable[33, 2] = false; // パイ・チェン
            NotComIniSlotTable[39, 3] = NotComIniSlotTable[39, 4] = NotComIniSlotTable[39, 5] = NotComIniSlotTable[39, 6] = NotComIniSlotTable[39, 7] = NotComIniSlotTable[39, 8] = NotComIniSlotTable[39, 10] = false; // 紅葉
            NotComIniSlotTable[40, 3] = NotComIniSlotTable[40, 5] = NotComIniSlotTable[40, 6] = NotComIniSlotTable[40, 7] = false; // レイチェル
            NotComIniSlotTable[41, 1] = NotComIniSlotTable[41, 2] = NotComIniSlotTable[41, 3] = false; // ジャッキー
            // マリー・ローズ
            // PHASE-4
            // 女天狗
            // ほのか
            // 雷道

            return NotComIniSlotTable;
        }
        */
        private Program.SlotTable<bool> GetUsableSlotTable()
        {

            Program.SlotTable<bool> UsableSlotTable = new Program.SlotTable<bool>(true);

            // 想定される書式のパスが指定されているか
            string SavePath = tbSavePath.Text;
            string ParentPath;
            if (System.Text.RegularExpressions.Regex.IsMatch(SavePath, @"\\\d+$"))
            {
                ParentPath = Path.GetDirectoryName(SavePath);
            }
            else
            {
                return UsableSlotTable;
            }

            // 兄弟 DLC をすべて調べる
            //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\\(\d+)([^\\]*)$"); // 1234 - DLC Name\1234.bcm を許す記述
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\\(\d+)$"); // 1234 - DLC Name\1234.bcm を許さない記述
            string[] brothers = Directory.GetDirectories(ParentPath);
            for (int i = 0; i < brothers.Length; i++)
            {

                if (brothers[i] != SavePath && regex.IsMatch(brothers[i]))
                {
                    //string brotherBCM = regex.Replace(brothers[i], @"\$1$2\$1.bcm"); // 1234 - DLC Name\1234.bcm を許す記述
                    string brotherBCM = regex.Replace(brothers[i], @"\$1\$1.bcm"); // 1234 - DLC Name\1234.bcm を許さない記述
                    DLCData dlcData2;
                    //MessageBox.Show(brotherBCM);
                    try
                    {
                        dlcData2 = Program.OpenBCM_超原始的修正(brotherBCM);
                    }
                    catch
                    {
                        continue;
                    }
                    //MessageBox.Show(brothers[i] + "\n" + dlcData2.Chars.Count.ToString());
                    for (int j = 0; j < dlcData2.Chars.Count; j++)
                    {
                        UsableSlotTable[dlcData2.Chars[j]] = false;
                    }
                }
            }


            return UsableSlotTable;

        }

        // よく考えると全キャラ分取得するのは無駄な気もするが、
        // ボトルネックはおそらくファイル読み込みであって、
        // その部分はキャラを限定してもほとんど速くならないと思うから
        // このままでいいことにしよう。
        private Program.SlotTable<string> GetComIniSlotCommentTable()
        {
            //Program.SlotTable<string> ComIniSlotCommentTable = new Program.SlotTable<string>("common,initial");
            Program.SlotTable < string > ComIniSlotCommentTable = new Program.SlotTable<string>("");

            // Lists フォルダを読んでおく（GetSlotOwnerTable のほぼコピー）
            string[] listpaths = null;
            try
            {
                listpaths = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"Lists"));
            }
            catch
            {
                return ComIniSlotCommentTable; // この関数ではリスト以外に情報源がない

            }
            System.Collections.Generic.List<DLCData> LCands = null;
            System.Collections.Generic.List<string> LCandPaths = null;
            if (listpaths != null)
            {
                LCands = new System.Collections.Generic.List<DLCData>();
                LCandPaths = new System.Collections.Generic.List<string>();

                //System.Text.RegularExpressions.Regex checkComIni = new System.Text.RegularExpressions.Regex(@"chara_common|chara_initial");
                System.Text.RegularExpressions.Regex checkComIni = new System.Text.RegularExpressions.Regex(@"default(?:\.[^\\]*)?\.[lr]st$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                for (int k = 0; k < listpaths.Length; k++)
                {
                    //MessageBox.Show(Path.GetFileName(listpaths[k]) + ", " + checkComIni.IsMatch(Path.GetFileName(listpaths[k])));

                    if (checkComIni.IsMatch(Path.GetFileName(listpaths[k]))) // ここも違う
                    {
                        try
                        {
                            // 順序を逆転させないこと！
                            LCands.Add(Program.OpenState(listpaths[k], false));
                            LCandPaths.Add(listpaths[k]);
                        }
                        catch { }
                    }
                }
            }



            // リストが何もなかったら何も出来ない
            if (LCandPaths.Count == 0)
            {
                return ComIniSlotCommentTable;
            }

            // listpaths 更新日時降順にバブルソート
            for (int i = 0; i < LCandPaths.Count; i++)
            {
                for (int j = 1; j < LCands.Count - i; j++)
                {
                    if (File.GetLastWriteTime(LCandPaths[j - 1]) < File.GetLastWriteTime(LCandPaths[j]))
                    {
                        var temp = LCandPaths[j];
                        LCandPaths[j] = LCandPaths[j - 1];
                        LCandPaths[j - 1] = temp;
                        var temp2 = LCands[j];
                        LCands[j] = LCands[j - 1];
                        LCands[j - 1] = temp2;
                    }
                }
            }


            // listdata を先頭のものに設定して残りの Char を AddRage する
            /*
            DLCData listdata = LCands[0];
            for (int i = 1; i < LCands.Count; i++)
            {
                listdata.Chars.AddRange(LCands[i].Chars);
            }
            */

            // 先頭の listdata のみを用いる
            DLCData listdata = LCands[0];
            string defaultTitle = Path.GetFileNameWithoutExtension(LCandPaths[0]);
            if (defaultTitle.Length >= "default.".Length)
            {
                defaultTitle = defaultTitle.Substring("default.".Length);
            }
            else
            {
                defaultTitle = "common,initial";
            }
            //MessageBox.Show(defaultTitle);

            for (int i = 0; i < listdata.Chars.Count; i++)
            {
                if (ComIniSlotCommentTable[listdata.Chars[i]] == "common/initial" && listdata.Chars[i].Comment != "")
                {
                    string name = listdata.Chars[i].Comment;
                    while (name.Substring(name.Length - 1) == ",")
                    {
                        name = name.Substring(0, name.Length - 1);
                    }
                    ComIniSlotCommentTable[listdata.Chars[i]] = name;
                }
                else
                {
                    ComIniSlotCommentTable[listdata.Chars[i]] = defaultTitle;
                }
            }

            /*

            // 各 com/ini のキャラに対して
            for(int i = 0; i < NotComIniSlotTable.Count(); i++)
            {
                for(int j = 0; j < NotComIniSlotTable[i].Length; j++)
                {
                    if(NotComIniSlotTable[i,j])
                    {
                        
                    }
                }
            }

            // リストファイル内のコメントから設定する
            for (int k = 0; k < listdata.Chars.Count; k++)
            {
                if (dlcData2.Chars[j].CostumeSlot == listdata.Chars[k].CostumeSlot && dlcData2.Chars[j].ID == listdata.Chars[k].ID && listdata.Chars[k].Comment != "")
                {
                    name = listdata.Chars[k].Comment;
                    break;
                }
            }

            if (SlotOwnerTable[dlcData2.Chars[j]] == "")
            {
                SlotOwnerTable[dlcData2.Chars[j]] = name;
            }
            else
            {
                SlotOwnerTable[dlcData2.Chars[j]] += ", " + name;
            }
            */

            return ComIniSlotCommentTable;
        }

        // よく考えると全キャラ分取得するのは無駄な気もするが、
        // ボトルネックはおそらくファイル読み込みであって、
        // その部分はキャラを限定してもほとんど速くならないと思うから
        // このままでいいことにしよう。
        private Program.SlotTable<string> GetSlotOwnerTable()
        {
            Program.SlotTable<string> SlotOwnerTable = new Program.SlotTable<string>("");

            // 想定される書式のパスが指定されているか
            string SavePath = tbSavePath.Text;
            string ParentPath;
            if (System.Text.RegularExpressions.Regex.IsMatch(SavePath, @"\\\d+$"))
            {
                ParentPath = Path.GetDirectoryName(SavePath);
            }
            else
            {
                return SlotOwnerTable;
            }

            // 兄弟 DLC を取得
            string[] brothers;
            try
            {
                brothers = Directory.GetDirectories(ParentPath);
            }
            catch
            {
                return SlotOwnerTable;
            }

            // Lists フォルダを読んでおく
            string[] listpaths = null;
            try
            {
                listpaths = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"Lists"));
            }
            catch { }
            System.Collections.Generic.List<DLCData> LCands0 = null;
            System.Collections.Generic.List<string> LCandPaths0 = null;
            if (listpaths != null)
            {
                LCands0 = new System.Collections.Generic.List<DLCData>();
                LCandPaths0 = new System.Collections.Generic.List<string>();

                System.Text.RegularExpressions.Regex checkComIni = new System.Text.RegularExpressions.Regex(@"default(?:\.[^\\]*)?\.[lr]st$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                for (int k = 0; k < listpaths.Length; k++)
                {
                    if (!checkComIni.IsMatch(Path.GetFileName(listpaths[k])))
                    {
                        try
                        {
                            // 順序を逆転させないこと！
                            LCands0.Add(Program.OpenState(listpaths[k], false));
                            LCandPaths0.Add(listpaths[k]);
                        }
                        catch { }
                    }
                }
            }

            // 兄弟 DLC をすべて調べる
            //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\\(\d+)([^\\]*)$"); // 1234 - DLC Name\1234.bcm を許す記述
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\\(\d+)$"); // 1234 - DLC Name\1234.bcm を許さない記述
            for (int i = 0; i < brothers.Length; i++)
            {
                var LCands = new System.Collections.Generic.List<DLCData>();
                System.Collections.Generic.List<string> LCandPaths = new System.Collections.Generic.List<string>();
                if (LCands0 != null && LCandPaths0 != null)
                {
                    LCands.AddRange(LCands0);
                    LCandPaths.AddRange(LCandPaths0);
                }

                //MessageBox.Show(((brothers[i] != SavePath) + ", " + ( regex.IsMatch(brothers[i]))));

                if (brothers[i] != SavePath && regex.IsMatch(brothers[i]))
                {


                    //string brotherBCM = regex.Replace(brothers[i], @"\$1$2\$1.bcm"); // 1234 - DLC Name\1234.bcm を許す記述
                    string brotherBCM = regex.Replace(brothers[i], @"\$1\$1.bcm"); // 1234 - DLC Name\1234.bcm を許さない記述
                    DLCData dlcData2;
                    //MessageBox.Show(brotherBCM);
                    try
                    {
                        dlcData2 = Program.OpenBCM_超原始的修正(brotherBCM);
                    }
                    catch
                    {
                        continue;
                    }
                    //MessageBox.Show(brothers[i] + "\n" + dlcData2.Chars.Count.ToString());

                    for (int j = 0; j < dlcData2.Chars.Count; j++)
                    {
                        string name;
                        string listpath = "";
                        string[] lsts = Directory.GetFiles(brothers[i], "*.lst");
                        string[] rsts = Directory.GetFiles(brothers[i], "*.rst");


                        //新しい配列を作る
                        string[] lists = new string[lsts.Length + rsts.Length];
                        //マージする配列のデータをコピーする
                        Array.Copy(lsts, lists, lsts.Length);
                        Array.Copy(rsts, 0, lists, lsts.Length, rsts.Length);


                        //MessageBox.Show(lists.Length.ToString());
                        if (lists.Length <= 0)
                        {
                            name = Path.GetFileName(brothers[i]);
                        }
                        else
                        {
                            var last = File.GetLastWriteTime(lists[0]);
                            name = Path.GetFileNameWithoutExtension(lists[0]);
                            for (int k = 1; k < lists.Length; k++)
                            {
                                var last2 = File.GetLastWriteTime(lists[k]);
                                if (last2 > last)
                                {
                                    last = last2;
                                    listpath = lists[k];
                                    name = Path.GetFileNameWithoutExtension(listpath);
                                }
                            }
                        }

                        //  Lists フォルダから name と listpath を作る
                        DLCData listdata = null;

                        if (listpath == "" && listpaths != null)
                        {

                            // break のためだけの while 文
                            bool found;

                            while (true)
                            {
                                // パスがドンピシャのものを探す
                                found = false;
                                for (int k = 0; k < LCands.Count; k++)
                                {
                                    if (LCands[k].SavePath == brothers[i])
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (found)
                                {
                                    for (int k = 0; k < LCands.Count; k++)
                                    {
                                        if (LCands[k].SavePath != brothers[i])
                                        {
                                            LCands.RemoveAt(k);
                                            LCandPaths.RemoveAt(k);
                                            k--;
                                        }
                                    }
                                    break;
                                }


                                // リストファイルの path のファイル名が brothers[i] のプレフィックスに一致するものを探す
                                // 同時に最大一致文字数を取得

                                int maxlength = 0;
                                found = false;
                                for (int k = 0; k < LCands.Count; k++)
                                {
                                    string listpathfilename = Path.GetFileName(LCands[k].SavePath);
                                    string br = Path.GetFileName(brothers[i]);
                                    //MessageBox.Show(brothers[i] + ", \n" + Path.GetFileName(LCands[k].SavePath) + "\n" + maxlength);

                                    if (listpathfilename.Length > maxlength && listpathfilename == br.Substring(0, Math.Min(listpathfilename.Length, br.Length)))
                                    {
                                        //MessageBox.Show(brothers[i] + ", \n" + Path.GetFileName(LCands[k].SavePath) + "\n" + k);
                                        found = true;
                                        maxlength = listpathfilename.Length;
                                        //break;
                                    }
                                }




                                if (found)
                                {
                                    for (int k = 0; k < LCands.Count; k++)
                                    {
                                        string listpathfilename = Path.GetFileName(LCands[k].SavePath);
                                        string br = Path.GetFileName(brothers[i]);
                                        //MessageBox.Show(listpathfilename + "\n" + br + "\n" + k.ToString() + "\n" + LCandPaths[k]);
                                        if (!(listpathfilename.Length == maxlength && listpathfilename == br.Substring(0, Math.Min(listpathfilename.Length, br.Length))))
                                        {
                                            LCands.RemoveAt(k);
                                            LCandPaths.RemoveAt(k);
                                            k--;
                                        }
                                    }

                                    break;
                                }

                                break;
                            }

                            // もし何かしら見つかっていたらその中で更新日時が最も新しい物を設定
                            if (found && LCands.Count > 0)
                            {
                                listpath = LCandPaths[0];
                                name = Path.GetFileNameWithoutExtension(listpath);
                                listdata = LCands[0];
                                var last = File.GetLastWriteTime(listpath);
                                for (int k = 1; k < LCands.Count; k++)
                                {
                                    var last2 = File.GetLastWriteTime(LCandPaths[k]);
                                    if (last2 > last)
                                    {
                                        listpath = LCandPaths[k];
                                        name = Path.GetFileNameWithoutExtension(listpath);
                                        listdata = LCands[k];
                                        last = last2;
                                    }
                                }
                            }



                        }



                        // リストファイル内のコメントから設定する
                        if (listpath != "")
                        {
                            if (listdata == null)
                            {
                                listdata = Program.OpenState(listpath, false);
                            }

                            for (int k = 0; k < listdata.Chars.Count; k++)
                            {
                                if (dlcData2.Chars[j].CostumeSlot == listdata.Chars[k].CostumeSlot && dlcData2.Chars[j].ID == listdata.Chars[k].ID && listdata.Chars[k].Comment != "")
                                {
                                    name = listdata.Chars[k].Comment;
                                    while (name.Substring(name.Length - 1) == ",")
                                    {
                                        name = name.Substring(0, name.Length - 1);
                                    }
                                    break;
                                }
                            }
                        }

                        if (SlotOwnerTable[dlcData2.Chars[j]] == "")
                        {
                            SlotOwnerTable[dlcData2.Chars[j]] = name;
                        }
                        else
                        {
                            SlotOwnerTable[dlcData2.Chars[j]] += ", " + name;
                        }
                    }
                }
            }


            return SlotOwnerTable;
        }


        public void SetCharNames()
        {
            bool LoadLanguage = false;
            try
            {
                // リストファイルを読み込み、「追加」のところに追加する

                catMale.DropDownItems.Clear();
                catFemale.DropDownItems.Clear();
                var CharInfoPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"CharactersInfo");
                System.Text.RegularExpressions.Regex regexIgnore = new System.Text.RegularExpressions.Regex(@"^\s*\/\/|^\s*$");
                System.Text.RegularExpressions.Regex regexRead = new System.Text.RegularExpressions.Regex(@"^([A-Z0-9]+)\s*=\s*(\d+)\s*,\s*(Male|Female)\s*,\s*(\d+)\s*$");
                using (var sr = new StreamReader(CharInfoPath))
                {
                    Program.CharNames = new System.Collections.Generic.Dictionary<byte, string>();
                    Program.NumOfSlots = new System.Collections.Generic.Dictionary<byte, byte>();
                    Program.FemaleIDs = new System.Collections.Generic.List<byte>();

                    int linecount = 0;
                    while (!sr.EndOfStream)
                    {
                        linecount++;
                        var line = sr.ReadLine();
                        if (regexIgnore.IsMatch(line))
                        {
                            continue;
                        }
                        var mc = regexRead.Matches(line);
                        if (mc.Count == 0)
                        {
                            TranslateInitialUI(false);
                            LoadLanguage = true;
                            System.Text.RegularExpressions.Regex regexReplace = new System.Text.RegularExpressions.Regex(@"(.+)");
                            var msg = regexReplace.Replace(
                                Path.Combine(Path.GetFileName(Path.GetDirectoryName(CharInfoPath)), Path.GetFileName(CharInfoPath)), Program.dicLanguage["FormatOfLineInXIsIncorrect"]) + "\n\n" +
                                linecount + ": " + line;
                                
                            
                            throw new FormatException(msg);
                        }
                        else
                        {
                            var m = mc[0]; // インデクサが1オリジンなので注意
                            var Name = m.Groups[1].Value;
                            var ID = byte.Parse(m.Groups[2].Value);
                            Program.CharNames[ID] = Name;
                            Program.NumOfSlots[ID] = byte.Parse(m.Groups[4].Value);
                            if (m.Groups[3].Value == "Male")
                            {
                                catMale.DropDownItems.Add(Name);
                                catMale.DropDownItems[catMale.DropDownItems.Count - 1].Click += AddCharacter;
                            }
                            else
                            {
                                Program.FemaleIDs.Add(ID);
                                catFemale.DropDownItems.Add(Name);
                                catFemale.DropDownItems[catFemale.DropDownItems.Count - 1].Click += AddCharacter;
                            }

                        }
                    }
                }


                // 念のためソート
                Program.FemaleIDs.Sort();
                //MessageBox.Show(Program.FemaleIDs[0] + ", " + Program.FemaleIDs[1] + ", " + Program.FemaleIDs[2] + ", " + Program.FemaleIDs[3]);

                // 表示用の追加
                Program.CharNamesJpn = new System.Collections.Generic.Dictionary<byte, string>(Program.CharNames);
            }
            catch (Exception e)
            {
               if(!LoadLanguage)
                {
                    TranslateInitialUI(false);
                }
                
                MessageBox.Show(e.Message, Program.dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                // プログラムを終了
                Environment.Exit(0);
            }
        }

        private void TranslateInitialUI(bool withCharNames)
        {
            string prtDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string lngSubPath = @"Languages\" + Application.CurrentCulture + ".lng";
            string folderPath = Path.Combine(prtDir, "Languages");
            string lngPath = Path.Combine(prtDir, lngSubPath);
            string lngPathDefault = Path.Combine(prtDir, @"Languages\default.lng");

            // en-US.lng を更新したらここに貼り付ける
            string enUS = @"Save=Save
NewDLC=New DLC
Notice=Notice
Error=Error
OpenBCM=Open BCM
SaveBCM=Save BCM
OverwriteBCM=Overwrite
SaveDLC=Save DLC
OverwriteDLC=Overwrite
SaveCompressedDLC=Save Cmp.
OverwriteCompressedDLC=Ovwt Cmp.
ExtractFiles=Extract TMC
Characters=Characters
Hairstyles=Hairstyles
CopyC=Copy (Ctrl+C)
PasteV=Paste (Ctrl+V)
DeleteD=Delete (Del)
ClearPasteVAfterCharSelection=Clear and paste (Ctrl+V after character selection)
Delete=Delete
Add=Add
Type=Type
Hair=Hr
Face=Fc
Files=Files
Character=Character
Index=Idx
Detail=Dtl
Comment=Comment
Mail_=Mail:
Femail_=Femail:
AddCommentIntoRight=Add comment column into the right
DeleteComment=Delete comment column
MakeACopyCV=Make a copy (Ctrl+C, Ctrl+V)
LoadState=Load state
SaveState=Save state
OverwriteState=Overwrite
SaveListInDLC=Save same name file in DLC folder
AddStateSD=Add state (Shift+Drop)
OriginalStateData=State data for original DLC Tool
FileNameMustBeDigits=File name must be digits.
DoYouStartDOA5=Do you start DEAD OR ALIVE 5 Last Round?
OverwritingKillsSkipedItem=Skipped item(s) will be lost with the overwriting.
SetDestinationToSave=Set destination to save.
SavedBCM=BCM was saved.
SavedDLC=DLC was saved.
SavedPartialDLC=DLC was saved but several items were skipped.
NotSavedDLC=DLC was not saved because there is no available item.
SavedState=State was saved.
DecreaseIndex=Enter smaller value for DLC name.
AddCharacter=Add Characters.
SkippedNItems=$1 unsupported item(s) was skipped.
NotFoundNameX=Unable to found $1 from the database.
NotFoundSomeOfFilesXOfCharY=Unable to found some of $1 files for $2.
SkippedBadNameX=$1 was skipped because of its unsupported name.
IDXIsUnknown=ID ""$1"" is unknown.
ProblemMayBeSolvedByEditingX =The Problem may be solved by editing ""$1.""
NeedDAT =This program requires at least one ""DAT\*.dat"" file.
NotPureDLCFolder = Save path has unknown items. DLC is not saved.
FormatOfLineInXIsIncorrect=The format of following line in ""$1"" is not correct.
ZACK =ZACK
TINA=TINA
JANNLEE=JANNLEE
EIN=EIN
HAYABUSA=HAYABUSA
KASUMI=KASUMI
GENFU=GENFU
HELENA=HELENA
LEON=LEON
BASS=BASS
KOKORO=KOKORO
HAYATE=HAYATE
LEIFANG=LEIFANG
AYANE=AYANE
ELIOT=ELIOT
LISA=LISA
ALPHA152=ALPHA152
BRAD=BRAD
CHRISTIE=CHRISTIE
HITOMI=HITOMI
BAYMAN=BAYMAN
RIG=RIG
MILA=MILA
AKIRA=AKIRA
SARAH=SARAH
PAI=PAI
MOMIJI=MOMIJI
RACHEL=RACHEL
JACKY=JACKY
MARIE=MARIE
PHASE4=PHASE4
NYOTENGU=NYOTENGU
HONOKA=HONOKA
RAIDOU=RAIDOU
";

            if (!File.Exists(lngPath) && !File.Exists(lngPathDefault))
            {
                try // 書き込みできないストレージから起動された時のため
                {
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    using (StreamWriter sw = new StreamWriter(lngPath, false))
                    {
                        sw.Write(enUS);
                        sw.Close();
                    }
                    MessageBox.Show("Created \"" + lngSubPath + "\" as a sample language file.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch { }
            }
            try
            {
                Program.OpenTranslation(lngPathDefault);
            }
            catch
            {
                try
                {
                    Program.OpenTranslation(lngPath);
                }
                catch { }
            }

            // 不足分は英語で補完しつつ初期画面を翻訳
            string from, def;
            // BCMを開く
            from = "OpenBCM"; def = "Open BCM";
            if (Program.dicLanguage.ContainsKey(from)) this.btnOpenBCM.Text = Program.dicLanguage[from]; else this.btnOpenBCM.Text = Program.dicLanguage[from] = def;
            // 保存
            from = "Save"; def = "Save";
            if (Program.dicLanguage.ContainsKey(from)) btnSave.Text = Program.dicLanguage[from]; else btnSave.Text = Program.dicLanguage[from] = def;
            // 新規作成
            from = "NewDLC"; def = "New DLC";
            if (Program.dicLanguage.ContainsKey(from)) this.btnNewDLC.Text = Program.dicLanguage[from]; else this.btnNewDLC.Text = Program.dicLanguage[from] = def;
            // キャラクター
            from = "Characters"; def = "Characters";
            if (Program.dicLanguage.ContainsKey(from)) this.gbChars.Text = Program.dicLanguage[from]; else this.gbChars.Text = Program.dicLanguage[from] = def;
            // 髪型
            from = "Hairstyles"; def = "Hairstyles";
            if (Program.dicLanguage.ContainsKey(from)) this.gbHairs2.Text = Program.dicLanguage[from]; else this.gbHairs2.Text = Program.dicLanguage[from] = def;
            // 削除
            from = "Delete"; def = "Delete";
            if (Program.dicLanguage.ContainsKey(from)) this.btnHStylesDelete.Text = Program.dicLanguage[from]; else this.btnHStylesDelete.Text = Program.dicLanguage[from] = def;
            // 追加
            from = "Add"; def = "Add";
            if (Program.dicLanguage.ContainsKey(from)) this.btnHStylesAdd.Text = Program.dicLanguage[from]; else this.btnHStylesAdd.Text = Program.dicLanguage[from] = def;
            // タイプ
            from = "Type"; def = "Type";
            if (Program.dicLanguage.ContainsKey(from)) this.clmType.HeaderText = Program.dicLanguage[from]; else this.clmType.HeaderText = Program.dicLanguage[from] = def;
            // 髪
            from = "Hair"; def = "Hr";
            if (Program.dicLanguage.ContainsKey(from)) this.clmHair.HeaderText = Program.dicLanguage[from]; else this.clmHair.HeaderText = Program.dicLanguage[from] = def;
            // 顔
            from = "Face"; def = "Fc";
            if (Program.dicLanguage.ContainsKey(from)) this.clmFace.HeaderText = Program.dicLanguage[from]; else this.clmFace.HeaderText = Program.dicLanguage[from] = def;
            // 削除（二回目）
            from = "Delete";
            this.btnCharsDelete.Text = Program.dicLanguage[from];
            // 追加（二回目）
            from = "Add";
            this.btnCharsAdd.Text = Program.dicLanguage[from];
            // ファイル
            from = "Files"; def = "Files";
            if (Program.dicLanguage.ContainsKey(from)) this.gbFiles.Text = Program.dicLanguage[from]; else this.gbFiles.Text = Program.dicLanguage[from] = def;
            // 削除（三回目）
            from = "Delete";
            this.btnFilesDelete.Text = Program.dicLanguage[from];
            // 追加（三回目）
            from = "Add";
            this.btnFilesAdd.Text = Program.dicLanguage[from];
            // 名前
            from = "Character"; def = "Character";
            if (Program.dicLanguage.ContainsKey(from)) this.clmName.HeaderText = Program.dicLanguage[from]; else this.clmName.HeaderText = Program.dicLanguage[from] = def;
            // ｽﾛｯﾄ
            from = "Index"; def = "Idx";
            if (Program.dicLanguage.ContainsKey(from)) this.clmCos.HeaderText = Program.dicLanguage[from]; else this.clmCos.HeaderText = Program.dicLanguage[from] = def;
            // ｲﾝﾅｰ
            from = "Detail"; def = "Dtl";
            if (Program.dicLanguage.ContainsKey(from)) this.clmInner.HeaderText = Program.dicLanguage[from]; else this.clmInner.HeaderText = Program.dicLanguage[from] = def;
            // コメント
            from = "Comment"; def = "Comment";
            if (Program.dicLanguage.ContainsKey(from)) { } else Program.dicLanguage[from] = def;
            this.clmComment.HeaderText = Program.dicLanguage[from] + " 1";
            // 男 :
            from = "Mail_"; def = "Mail:";
            if (Program.dicLanguage.ContainsKey(from)) this.catMale.Text = Program.dicLanguage[from]; else this.catMale.Text = Program.dicLanguage[from] = def;
            // 女 :
            from = "Femail_"; def = "Femail:";
            if (Program.dicLanguage.ContainsKey(from)) this.catFemale.Text = Program.dicLanguage[from]; else this.catFemale.Text = Program.dicLanguage[from] = def;
            // 右にコメント列を追加
            from = "AddCommentIntoRight"; def = "Add comment column into the right";
            if (Program.dicLanguage.ContainsKey(from)) this.smiAddComColumn.Text = Program.dicLanguage[from]; else this.smiAddComColumn.Text = Program.dicLanguage[from] = def;
            // コメント列を削除
            from = "DeleteComment"; def = "Delete comment column";
            if (Program.dicLanguage.ContainsKey(from)) this.smiDelComColumn.Text = Program.dicLanguage[from]; else this.smiDelComColumn.Text = Program.dicLanguage[from] = def;
            // 選択データのコピー
            from = "MakeACopyCV"; def = "Make a copy (Ctrl+C, Ctrl+V)";
            if (Program.dicLanguage.ContainsKey(from)) this.コピーToolStripMenuItem.Text = Program.dicLanguage[from]; else this.コピーToolStripMenuItem.Text = Program.dicLanguage[from] = def;
            // リストを開く
            from = "LoadState"; def = "Load state";
            if (Program.dicLanguage.ContainsKey(from)) this.btnOpenState.Text = Program.dicLanguage[from]; else this.btnOpenState.Text = Program.dicLanguage[from] = def;
            // リストを保存
            from = "SaveState"; def = "Save state";
            if (Program.dicLanguage.ContainsKey(from)) this.btnSaveState.Text = Program.dicLanguage[from]; else this.btnSaveState.Text = Program.dicLanguage[from] = def;
            // 同じ名前でDLCフォルダにも保存
            from = "SaveListInDLC"; def = "Save same name file in DLC folder";
            if (Program.dicLanguage.ContainsKey(from)) this.cbSaveListInDLC.Text = Program.dicLanguage[from]; else this.cbSaveListInDLC.Text = Program.dicLanguage[from] = def;
            // リストの追加読込
            from = "AddStateSD"; def = "Add state (Shift+Drop)";
            if (Program.dicLanguage.ContainsKey(from)) this.リストの追加読み込みToolStripMenuItem.Text = Program.dicLanguage[from]; else this.リストの追加読み込みToolStripMenuItem.Text = Program.dicLanguage[from] = def;
            // コピー (Ctrl+C)
            from = "CopyC"; def = "Copy (Ctrl+C)";
            if (Program.dicLanguage.ContainsKey(from)) コピーCtrlCToolStripMenuItem.Text = Program.dicLanguage[from]; else コピーCtrlCToolStripMenuItem.Text = Program.dicLanguage[from] = def;
            // 貼り付け (Ctrl+V)
            from = "PasteV"; def = "Paste (Ctrl+V)";
            if (Program.dicLanguage.ContainsKey(from)) 貼り付けCtrlVToolStripMenuItem.Text = Program.dicLanguage[from]; else 貼り付けCtrlVToolStripMenuItem.Text = Program.dicLanguage[from] = def;
            // 削除 (Del)
            from = "DeleteD"; def = "Delete (Del)";
            if (Program.dicLanguage.ContainsKey(from)) 削除DeleteToolStripMenuItem.Text = Program.dicLanguage[from]; else 削除DeleteToolStripMenuItem.Text = Program.dicLanguage[from] = def;
            // ｸﾘｱ+貼り付け (服装選択後 Ctrl+V)
            from = "ClearPasteVAfterCharSelection"; def = "Clear and paste (Ctrl+V after character selection)";
            if (Program.dicLanguage.ContainsKey(from)) ClearPasteToolStripMenuItem.Text = Program.dicLanguage[from]; else ClearPasteToolStripMenuItem.Text = Program.dicLanguage[from] = def;
            // 圧縮して保存
            from = "SaveCompressedDLC"; def = "Save Cmp.";
            if (Program.dicLanguage.ContainsKey(from)) btnCmpSave.Text = Program.dicLanguage[from]; else btnCmpSave.Text = Program.dicLanguage[from] = def;

            // 元ツールのステートデータ
            from = "OriginalStateData"; def = "State data for original DLC Tool";
            if (Program.dicLanguage.ContainsKey(from)) { } else Program.dicLanguage[from] = def; ;
            this.openFileDialogState.Filter = "*.lst;*.rst|*.lst;*.rst|" + Program.dicLanguage[from] + "|*.*";
            this.saveFileDialogState.Filter = "*.lst|*.lst|" + Program.dicLanguage[from] + "|*.*";


            // 0.10.3 日本語版 ほげほげば～じょん
            //from = "ProgramName"; def = "hogehoge (based on Japanized version 0.10.3)";
            //if (! Program.dicLanguage.ContainsKey(from))
            //{
            //    Program.dicLanguage[from] = def;
            //}
            //
            //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^.*?([0-9.\-]*)$");
            //string version = regex.Replace(this.Text, "$1");
            //this.Text = "DLC Tool " + Program.dicLanguage[from] + " " + version;



            // 初期画面に現れない部分の補間

            // BCMを保存
            from = "SaveBCM"; def = "Save BCM";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // ファイル展開
            from = "ExtractFiles"; def = "Extract TMC";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // 注意
            from = "Notice"; def = "Notice";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // エラー
            from = "Error"; def = "Error";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // ファイル名は数字のみ使用できます
            from = "FileNameMustBeDigits"; def = "File name must be digits.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // DEAD OR ALIVE 5 Last Round を起動しますか？
            from = "DoYouStartDOA5"; def = "Do you start DEAD OR ALIVE 5 Last Round?";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // 上書き保存するとスキップしたデータを失うので注意して下さい
            from = "OverwritingKillsSkipedItem"; def = "Skipped item(s) will be lost with the overwriting.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // 保存先を指定して下さい
            from = "SetDestinationToSave"; def = "Set destination to save.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // BCMを保存しました
            from = "SavedBCM"; def = "BCM was saved.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // 現在の状態を保存しました
            from = "SavedState"; def = "State was saved.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // DLCの数字を小さくして下さい
            from = "DecreaseDLCNum"; def = "Enter smaller value for DLC name.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;

            // DLC書き出し
            from = "SaveDLC"; def = "Save DLC";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;

            // DLC上書き
            from = "OverwriteDLC"; def = "Overwrite";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;


            // 圧縮して保存
            from = "SaveCompressedDLC"; def = "Save Cmp.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;

            // 圧縮して上書
            from = "OverwriteCompressedDLC"; def = "Ovwt Cmp.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;

            // BCM上書き
            from = "OverwriteBCM"; def = "Overwrite";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;

            // 指定されたパス以下に未知のファイル・フォルダが存在するためDLCを書き出せませんでした。
            from = "NotPureDLCFolder"; def = "Save path has unknown items. DLC is not saved.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;

            // リスト上書き
            from = "OverwriteState"; def = "Overwrite";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;

            // 同時に保存するリストファイルの名前を正しく入力してください。
            //from = "InputCorrectListFileName"; def = "Input correct list file name.";
            //if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;

            // キャラクターを追加して下さい
            from = "AddCharacter"; def = "Add Characters.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // DLCを書出しました
            from = "SavedDLC"; def = "DLC was saved.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // DLCを書出しましたが一部の項目は無視されました
            from = "SavedPartialDLC"; def = "DLC was saved but several items were skipped.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // 出力可能な項目が一つもありません
            from = "NotSavedDLC"; def = "DLC was not saved because there is no available item.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // 未対応のデータ $1 個の読み込みをスキップしました
            from = "SkippedNItems"; def = "$1 unsupported item(s) was skipped.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;

            // $1の名前はデータベースに存在しません
            from = "NotFoundNameX"; def = "Unable to found $1 from the database.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // $2の$1ファイルのいずれかが見つかりません
            from = "NotFoundSomeOfFilesXOfCharY"; def = "Unable to found some of $1 files for $2.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // $1の名前に問題があるため読込はスキップされました
            from = "SkippedBadNameX"; def = "$1 was skipped because of its unsupported name.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // 未知の ID "$1" が検出されました
            from = "IDXIsUnknown"; def = @" ID ""$1"" is unknown.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // "$1" を修正することで問題が解決するかもしれません
            from = "ProblemMayBeSolvedByEditingX"; def = @"The Problem may be solved by editing ""$1.""";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // "DAT\*.dat" ファイルが少なくとも一つ必要です
            from = "NeedDAT"; def = @"This program requires at least one ""DAT\*.dat"" file.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;
            // "$1" 内の以下の行のフォーマットが正しくありません
            from = "FormatOfLineInXIsIncorrect"; def = @"The format of following line in ""$1"" is not correct.";
            if (!Program.dicLanguage.ContainsKey(from)) Program.dicLanguage[from] = def;

            // キャラクター名
            if (withCharNames)
            {
                foreach (var element in Program.CharNames)
                {
                    from = def = element.Value;
                    if (Program.dicLanguage.ContainsKey(from)) Program.CharNamesJpn[element.Key] = Program.dicLanguage[from]; else Program.CharNamesJpn[element.Key] = Program.dicLanguage[from] = def;
                }

                foreach (ToolStripItem element in catMale.DropDownItems)
                {
                    from = def = element.Text;
                    if (Program.dicLanguage.ContainsKey(from)) element.Text = Program.dicLanguage[from];
                }
                foreach (ToolStripItem element in catFemale.DropDownItems)
                {
                    from = def = element.Text;
                    if (Program.dicLanguage.ContainsKey(from)) element.Text = Program.dicLanguage[from];
                }
            }


        }



        private void dgvHStyles_MouseDown(object sender, MouseEventArgs e)
        {

            if (dgvChars.SelectedRows.Count != 1)
            {
                return;
            }

            // 左クリック
            if (e.Button == MouseButtons.Left && dgvHStyles.Rows.Count > 0)
            {

                int CharSelected;
                try
                {
                    CharSelected = dgvChars.SelectedRows[0].Index;
                }
                catch
                {
                    return;
                }


                // MouseDownイベント発生時の (x,y)座標を取得
                DataGridView.HitTestInfo hit = dgvHStyles.HitTest(e.X, e.Y);
                int index = hit.RowIndex;
                if (index >= 0 && dlcData.Chars[CharSelected].HStyles.Count > 1)
                {
                    btnHStylesDelete.Enabled = true;
                }
                else
                {

                    // なにもないところがクリックされたら選択を解除

                    for (int i = 0; i < dgvHStyles.Rows.Count; i++)
                    {
                        dgvHStyles.Rows[i].Selected = false;
                    }
                    btnHStylesDelete.Enabled = false;
                }
            }
            // 右クリック
            else if (e.Button == MouseButtons.Right && dgvHStyles.Rows.Count > 0)
            {




                // MouseDownイベント発生時の (x,y)座標を取得
                DataGridView.HitTestInfo hit = dgvHStyles.HitTest(e.X, e.Y);
                int index = hit.RowIndex;
                if (index >= 0)
                {

                    // 該当行が選択されていなければそれだけを選択状態にする
                    if (!dgvHStyles.Rows[index].Selected)
                    {
                        for (int i = 0; i < dgvHStyles.Rows.Count; i++)
                        {
                            dgvHStyles.Rows[i].Selected = (i == index);
                        }

                        int CharSelected;
                        try
                        {
                            CharSelected = dgvChars.SelectedRows[0].Index;
                        }
                        catch
                        {
                            return;
                        }
                        if (dlcData.Chars[CharSelected].HStyles.Count > 1)
                        {
                            btnHStylesDelete.Enabled = true;
                        }
                    }
                }
                else
                {
                    // なにもないところがクリックされたら選択を解除

                    for (int i = 0; i < dgvHStyles.Rows.Count; i++)
                    {
                        dgvHStyles.Rows[i].Selected = false;
                    }
                    btnHStylesDelete.Enabled = false;
                }



                clikedForm = "dgvHStyles";

                //コピーCtrlCToolStripMenuItem.Enabled = 削除DeleteToolStripMenuItem.Enabled = btnHStylesDelete.Enabled;
                // 髪型に関しては、残り１つだと選択されていてもデリートボタンが向こうになるのでこの様な手の抜き方はダメ
                try
                {
                    コピーCtrlCToolStripMenuItem.Enabled = (dgvHStyles.SelectedRows.Count > 0);
                }
                catch
                {
                    コピーCtrlCToolStripMenuItem.Enabled = false;
                }

                削除DeleteToolStripMenuItem.Enabled = btnHStylesDelete.Enabled;

                ClearPasteToolStripMenuItem.Enabled = true;
                bool canpaste = PasteHStyles(true); // テストモード
                貼り付けCtrlVToolStripMenuItem.Enabled = (canpaste && btnHStylesAdd.Enabled); // 髪型マックスなら貼り付けられない
                ClearPasteToolStripMenuItem.Enabled = canpaste;

                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private void 削除DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 髪型にフォーカスがあれば
            if (clikedForm == "dgvHStyles")
            {
                if (btnHStylesDelete.Enabled)
                {
                    btnHStylesDelete_Click(null, null);
                }
            }
            // ファイルにフォーカスがあれば
            else if (clikedForm == "dgvFiles")
            {
                if (btnFilesDelete.Enabled)
                {
                    btnFilesDelete_Click(null, null);
                }
            }
        }

        private void 貼り付けCtrlVToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // 髪型にフォーカスがあれば
            if (clikedForm == "dgvHStyles")
            {
                PasteHStyles(false);
            }
            // ファイルにフォーカスがあれば
            else if (clikedForm == "dgvFiles")
            {

                string[] paths = (string[])Clipboard.GetDataObject().GetData(DataFormats.FileDrop);
                AddFiles(paths, false);
            }
        }

        private void コピーCtrlCToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // 髪型にフォーカスがあれば
            if (clikedForm == "dgvHStyles")
            {
                CopyHStyles();
            }
            // ファイルにフォーカスがあれば
            else if (clikedForm == "dgvFiles")
            {

                //コピーするファイルのパスをStringCollectionに追加する
                System.Collections.Specialized.StringCollection files =
                    new System.Collections.Specialized.StringCollection();

                if (dgvFiles.SelectedRows.Count <= 0)
                {
                    return;
                }

                for (int i = 0; i < dgvFiles.SelectedRows.Count; i++)
                {
                    files.Add(dgvFiles.SelectedRows[i].Cells[0].Value.ToString());
                }
                //クリップボードにコピーする
                Clipboard.SetFileDropList(files);
                //MessageBox.Show(files[0]);
            }
        }

        private void CopyHStyles()
        {

            if (dgvChars.SelectedRows.Count != 1)
            {
                return;
            }

            if (dgvHStyles.SelectedRows.Count <= 0)
            {
                return;
            }

            // SelectedRows は自動ではソートされない
            int[] inds = new int[dgvHStyles.SelectedRows.Count];
            Character Char = dlcData.Chars[dgvChars.SelectedRows[0].Index];
            for (int i = 0; i < dgvHStyles.SelectedRows.Count; i++)
            {
                inds[i] = dgvHStyles.SelectedRows[i].Index;
            }
            Array.Sort(inds);


            string str = "";
            for (int i = 0; i < dgvHStyles.SelectedRows.Count; i++)
            {
                var line = Char.HStyles[inds[i]];
                str += line.Hair + "\t" + line.Face + "\n";
            }
            Clipboard.SetText(str);
        }

        private bool PasteHStyles(bool testMode) // テストモードでは髪型の空きは考慮しない。その方が書きやすいし削除＋貼り付けで使うのに都合も良い
        {

            if (dgvChars.SelectedRows.Count != 1)
            {
                return false;
            }

            bool pasted = false;

            if (!btnHStylesAdd.Enabled && !testMode) // テストモードなら髪型の数は気にしない
            {
                return false;
            }

            if (!Clipboard.GetDataObject().GetDataPresent(DataFormats.Text))
            {
                return false;
            }

            string[] lines = Clipboard.GetText().Split('\n');


            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^(\d)\t(\d)\r?$");
            int index;
            try
            {
                index = dgvHStyles.SelectedRows[0].Index;
            }
            catch
            {
                index = dgvHStyles.Rows.Count;
            }

            int addcount = 0;
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                if (regex.IsMatch(lines[i]))
                {
                    // try はしなくていいと思う
                    byte Hair = byte.Parse(regex.Replace(lines[i], "$1"));
                    byte Face = byte.Parse(regex.Replace(lines[i], "$2"));

                    pasted = true;

                    if (!testMode)
                    {
                        addcount++;
                        dlcData.Chars[dgvChars.SelectedRows[0].Index].HStyles.Insert(index, new Hairstyle(Hair, Face));



                        //if (dgvHStyles.Rows.Count >= 8)
                        if (dlcData.Chars[dgvChars.SelectedRows[0].Index].HStyles.Count >= 8)
                        {
                            btnHStylesAdd.Enabled = false;
                            break;
                        }
                    }
                }
            }

            if (addcount > 0)
            {

                ShowHairstyles(dgvChars.SelectedRows[0].Index);
                btnHStylesDelete.Enabled = true; // ShowHairstyles よりも後じゃないとダメ
                for (int i = 0; i < dgvHStyles.Rows.Count; i++)
                {
                    dgvHStyles.Rows[i].Selected = false;
                }
                for (int i = index; i < index + addcount; i++)
                {
                    dgvHStyles.Rows[i].Selected = true;
                }
            }
            //MessageBox.Show("a");

            return pasted;

        }


        private void dgvChars_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Delete)
            {
                DeleteKeyUp = true;
            }
            else if (e.KeyCode == Keys.C)
            {
                CKeyUp = true;
            }
            else if (e.KeyCode == Keys.V)
            {
                VKeyUp = true;
            }
        }

        private void dgvHStyles_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Delete)
            {
                DeleteKeyUp = true;
            }
            else if (e.KeyCode == Keys.C)
            {
                CKeyUp = true;
            }
            else if (e.KeyCode == Keys.V)
            {
                VKeyUp = true;
            }
        }


        private void ClearPasteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (dgvChars.SelectedRows.Count != 1)
            {
                return;
            }

            // まず現在のものを削除
            if (clikedForm == "dgvHStyles")
            {
                // ポップアップ呼び出しの時点でチェックすることにした
                /*
                // 貼り付けられるものがなければ何もしない
                if(!PasteHStyles(true)) // テストモード
                {
                    return;
                }
                */


                int CharSelected;
                try
                {
                    CharSelected = dgvChars.SelectedRows[0].Index;
                }
                catch
                {
                    return;
                }
                dlcData.Chars[CharSelected].HStyles.Clear();

                // 直後にペーストが控えているのでこれは不要
                //ShowHairstyles(CharSelected);

                // 髪型の貼り付けは現在の表示を見て選択位置に挿入しようとする
                // なのでその前に整合性を取っておく必要がある。
                ShowHairstyles(CharSelected);

            }
            else if (clikedForm == "dgvFiles")
            {
                // ポップアップ呼び出しの時点でチェックすることにした
                /*
                // 貼り付けられるものがなければ何もしない
                string[] paths = (string[])Clipboard.GetDataObject().GetData(DataFormats.FileDrop);
                if(!AddFiles(paths, true)) // テストモード
                {
                    return;
                }
                */

                int CharSelected;
                try
                {
                    CharSelected = dgvChars.SelectedRows[0].Index;
                }
                catch
                {
                    return;
                }
                for (int i = 0; i < dlcData.Chars[CharSelected].Files.Length /* = 12 */; i++)
                {
                    dlcData.Chars[CharSelected].Files[i] = null;
                }
                // 直後にペーストが控えているのでこれは不要
                //setEgvCharsSlotColor(); // 流石にこれは不要
                //setEgvCharsNameColor();
                //setEgvCharsTextsColor();
                //
                //ShowFiles(dgvChars.SelectedRows[0].Index);
            }
            // その後貼り付け処理
            貼り付けCtrlVToolStripMenuItem_Click(null, null);
        }



        public void SetProgressBar(bool pbStart, int pbMaximum = 0)
        {
            if (pbStart)
            {
                progressBar.Maximum = pbMaximum;
                progressBar.Visible = true;
            }
            else
            {
                progressBar.Visible = false;
                progressBar.Value = 0;
            }
        }
        public void IncrementProgressBar()
        {
            progressBar.Increment(1);
        }

        private void dgvChars_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                dgvChars.ImeMode = System.Windows.Forms.ImeMode.Disable;
            }
            else if (e.ColumnIndex >= 3)
            {
                dgvChars.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            }
        }

        private void SetDATList()
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\.[dD][aA][tT]$");
            string datPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"DAT");
            try
            {
                DATs = Directory.GetFiles(datPath, "*.dat");//Directory.GetDirectories(ParentPath);
            }
            catch
            {
                MessageBox.Show(Program.dicLanguage["NeedDAT"], Program.dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);

                // プログラムを終了
                Environment.Exit(0);
            }
            if (DATs.Length <= 0)
            {
                MessageBox.Show(Program.dicLanguage["NeedDAT"], Program.dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);

                // プログラムを終了
                Environment.Exit(0);
            }


            System.Collections.Generic.List<string> defDATs = new System.Collections.Generic.List<string>();
            System.Collections.Generic.List<string> nodefDATs = new System.Collections.Generic.List<string>();
            for (int i = 0; i < DATs.Length; i++)
            {
                if (Path.GetFileName(DATs[i]).Substring(0, "default.".Length) == "default.")
                {
                    defDATs.Add(DATs[i]);
                }
                else
                {
                    nodefDATs.Add(DATs[i]);
                }
            }
            defDATs.Sort();
            nodefDATs.Sort();
            for (int i = 0; i < defDATs.Count; i++)
            {
                DATs[i] = defDATs[i];
                cbDAT.Items.Add(Path.GetFileName(Path.GetFileName(DATs[i]).Substring("default.".Length)));
            }
            for (int i = 0; i < nodefDATs.Count; i++)
            {
                DATs[i + defDATs.Count] = nodefDATs[i];
                cbDAT.Items.Add(Path.GetFileName(Path.GetFileName(nodefDATs[i])));
            }

            // 読み取り専用（テキストボックスは編集不可）にする
            cbDAT.DropDownStyle = ComboBoxStyle.DropDownList;

            cbDAT.SelectedIndex = 0; // 先頭の項目を選択
        }

        private void cbDAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            DatSelectedIndex = cbDAT.SelectedIndex;
        }

        public static string tbListPath_Text_static = "";
        public static string tbSavePath_Text_static = "";
        private void tbListPath_TextChanged(object sender, EventArgs e)
        {
            tbListPath_Text_static = tbListPath.Text;

            //MessageBox.Show(newDlc.ToString());
            if (newDlc && tbListPath.Text != "") // なんでこうしたんだったか思い出せない → いや当たり前でしょう。BCM モードではリストを保存しないのだから。
            //if (tbListPath.Text != "")
            {
                if (File.Exists(tbListPath.Text + ".lst"))
                {
                    btnSaveState.Text = Program.dicLanguage["OverwriteState"];
                    tbListPath.BackColor = System.Drawing.Color.Empty;
                    cbSaveListInDLC.Enabled = true;
                }
                else
                {
                    btnSaveState.Text = Program.dicLanguage["SaveState"];

                    string name;
                    try
                    {
                        name = Path.GetFileName(tbListPath.Text);
                    }
                    catch
                    {
                        name = "";
                    }
                    bool GoodName = (name != "" && name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0);
                    bool GoodFolder = false;
                    try
                    {
                        GoodFolder = Directory.Exists(Path.GetDirectoryName(tbListPath.Text));
                    }
                    catch { }
                    if (GoodName && GoodFolder)
                    {
                        tbListPath.BackColor = System.Drawing.Color.Empty;
                        cbSaveListInDLC.Enabled = true;
                    }
                    else if (GoodName)
                    {
                        tbListPath.BackColor = System.Drawing.Color.LightGray;
                        cbSaveListInDLC.Enabled = true;
                    }
                    else
                    {
                        tbListPath.BackColor = System.Drawing.Color.LightGray;
                        //cbSaveListInDLC.Enabled = false;
                        //cbSaveListInDLC.Checked = false;
                    }
                }
            }
            else
            {
                btnSaveState.Text = Program.dicLanguage["SaveState"];
                tbListPath.BackColor = System.Drawing.Color.Empty;
                //cbSaveListInDLC.Enabled = false;
                //cbSaveListInDLC.Checked = false;
            }
        }

        private void SelectFile(int Index, bool selectone)
        {
            // ctrl または shift が押されていると selectone を無視し、またトグルモードになる
            bool ctsh = (((Control.ModifierKeys & Keys.Shift) == Keys.Shift || (Control.ModifierKeys & Keys.Control) == Keys.Control));

            string suf = FileOrder[Index];
            for (int i = 0; i < dgvFiles.Rows.Count; i++)
            {
                string str = dgvFiles.Rows[i].Cells[0].Value.ToString();
                if (str.Substring(str.Length - suf.Length, suf.Length) == suf)
                {
                    if (ctsh)
                    {
                        dgvFiles.Rows[i].Selected = !dgvFiles.Rows[i].Selected;
                    }
                    else
                    {
                        dgvFiles.Rows[i].Selected = true;
                    }
                }
                else if (selectone && !ctsh)
                {
                    dgvFiles.Rows[i].Selected = false;
                }
            }
        }

        // public static readonly string[] FileOrder = new string[12] { ".TMC", ".TMCL", ".---C", "1.--H", "1.--HL", "2.--H", "2.--HL", "3.--H", "3.--HL", "4.--H", "4.--HL", ".--P" };

        private void cbC_Click(object sender, EventArgs e)
        {
            SelectFile(2, true);
        }

        private void cbP_Click(object sender, EventArgs e)
        {
            SelectFile(11, true);
        }

        private void cbTMC_Click(object sender, EventArgs e)
        {
            SelectFile(0, true);
        }

        private void cbTMCL_Click(object sender, EventArgs e)
        {
            SelectFile(1, true);
        }

        private void cb1H_Click(object sender, EventArgs e)
        {
            SelectFile(3, true);
            SelectFile(4, false);
        }

        private void cb2H_Click(object sender, EventArgs e)
        {
            SelectFile(5, true);
            SelectFile(6, false);
        }

        private void cb3H_Click(object sender, EventArgs e)
        {
            SelectFile(7, true);
            SelectFile(8, false);
        }

        private void cb4H_Click(object sender, EventArgs e)
        {
            SelectFile(9, true);
            SelectFile(10, false);
        }

        private void tbSavePath_TextChanged(object sender, EventArgs e)
        {
            tbSavePath_Text_static = tbSavePath.Text;

            try // Path 系の関数は Path でないものを入力するとエラーを返す
            {
                if ((!newDlc) || tbSavePath.Text == "" || (System.Text.RegularExpressions.Regex.IsMatch(tbSavePath.Text, @"\\\d+$") && Directory.Exists(Path.GetDirectoryName(tbSavePath.Text))))
                {
                    tbSavePath.BackColor = System.Drawing.Color.Empty;
                }
                else
                {
                    tbSavePath.BackColor = System.Drawing.Color.LightGray;
                }
            }
            catch
            {
                tbSavePath.BackColor = System.Drawing.Color.LightGray;
            }

            setBtnSave();
        }

        private void showComment(int Index)
        {
            string com = dlcData.Chars[Index].Comment;

            /*
            int[] d2i = new int[dgvChars.Columns.Count];
            for(int j = 3; j < dgvChars.Columns.Count; j++)
            {
                d2i[dgvChars.Columns[j].DisplayIndex] = j;
            }
            */

            // token 的に書いていく。空をスキップしたりはしない。最後には全部を突っ込む
            //int start = 0;
            int i;
            for (i = 3; i < dgvChars.Rows[Index].Cells.Count; i++)
            {
                int len = com.IndexOf(',');
                if (len >= 0)
                {
                    string substr = com.Substring(0, len);
                    dgvChars.Rows[Index].Cells[i].Value = substr;
                    com = com.Substring(len + 1);
                }
                else
                {
                    string substr = com;
                    dgvChars.Rows[Index].Cells[i].Value = substr;
                    com = "";
                    i++;
                    break;
                }
            }

            // 最後まで到達していなかったら空文字で埋める
            for (; i < dgvChars.Rows[Index].Cells.Count; i++)
            {
                dgvChars.Rows[Index].Cells[i].Value = "";
            }
        }

        private void smiAddComColumn_Click(object sender, EventArgs e)
        {
            // クリックされたカラム番号の取得
            //clikedForm = "dgvColumn" + e.ColumnIndex.ToString();
            int clicledIndex;
            try
            {
                clicledIndex = int.Parse(clikedForm.Substring("dgvColumn".Length));
            }
            catch
            {
                return;
            }
            AddComColumn(clicledIndex, false);
        }

        private void AddComColumn(int clicledIndex, bool ForInit)
        {
            /*
            // display から 普通のインデックスへ
            int[] d2i = new int[dgvChars.Columns.Count];
            for (int j = 3; j < dgvChars.Columns.Count; j++)
            {
                d2i[dgvChars.Columns[j].DisplayIndex] = j;
            }
            */


            //DataGridViewTextBoxColumn列を作成する
            DataGridViewTextBoxColumn textColumn = new DataGridViewTextBoxColumn();

            //名前とヘッダーを設定する
            //textColumn.Name = "Column1";
            //if (clicledIndex == 2)
            {
                //    textColumn.HeaderText = Program.dicLanguage["Comment"];
            }
            //else
            {
                textColumn.HeaderText = Program.dicLanguage["Comment"] + " " + (clicledIndex - 1).ToString();
            }
            textColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            textColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            textColumn.FillWeight = 100;
            textColumn.Name = "clmComment";
            textColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            /*
            // リサイズ時、サイズの増減は一番右が請け負うことにする
            for (int i = 3; i < dgvChars.Columns.Count - 1; i++)
            {
                dgvChars.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            */

            //列を追加する
            dgvChars.Columns.Insert(clicledIndex + 1, textColumn);


            // リサイズ時、サイズの増減は一番右が請け負うことにする
            for (int i = 3; i < dgvChars.Columns.Count - 1; i++)
            {
                dgvChars.Columns[i].Resizable = DataGridViewTriState.True;
                //dgvChars.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            dgvChars.Columns[dgvChars.Columns.Count - 1].Resizable = DataGridViewTriState.False;
            //dgvChars.Columns[dgvChars.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;


            // カラム名を修正
            for (int i = clicledIndex + 2; i < dgvChars.Columns.Count; i++)
            {
                dgvChars.Columns[i].HeaderText = Program.dicLanguage["Comment"] + " " + (i - 2).ToString();
            }

            if (ForInit)
            {
                return;
            }

            // 現在のコメントの列数
            int ColCount = dgvChars.Columns.Count - 3;

            // 追加したコメント列、コメント1つ目を 0 としていくつ目か
            int ColAddIndex = clicledIndex - 2;

            // Comment 文字列の処理して再描画
            for (int i = 0; i < dlcData.Chars.Count; i++)
            {
                string com = dlcData.Chars[i].Comment;


                // ColAddIndex 個目のカンマがあればその前にカンマを挿入
                // カンマが見つからなければ最後にカンマを追加してそれを見つけたものとする。
                // ColAddIndex == 0 なら文字列の最初にカンマ
                int comma = 0;
                int start = 0;
                for (int j = 0; j < ColAddIndex; j++)
                {
                    comma = com.IndexOf(',', start);
                    if (comma >= 0)
                    {
                        start = comma + 1;
                    }
                    else
                    {
                        comma = com.Length;
                        if (j < ColAddIndex - 1)
                        {
                            com += ",";
                            start = comma + 1;
                        }

                    }

                }
                if (comma >= 0)
                {
                    dlcData.Chars[i].Comment = com.Insert(comma, ",");
                }
                else
                {
                    dlcData.Chars[i].Comment = com; // 末尾のコンマを取ったかもしれないので
                }



                showComment(i);
            }
        }

        private void smiDelComColumn_Click(object sender, EventArgs e)
        {

            // クリックされたカラム番号の取得
            //clikedForm = "dgvColumn" + e.ColumnIndex.ToString();
            int clicledIndex;
            try
            {
                clicledIndex = int.Parse(clikedForm.Substring("dgvColumn".Length));
            }
            catch
            {
                return;
            }

            // その列を削除
            dgvChars.Columns.RemoveAt(clicledIndex);



            // カラム名を修正
            for (int i = clicledIndex; i < dgvChars.Columns.Count; i++)
            {
                //if (i == 3)
                {
                    //    dgvChars.Columns[i].HeaderText = Program.dicLanguage["Comment"];
                }
                //else
                {
                    dgvChars.Columns[i].HeaderText = Program.dicLanguage["Comment"] + " " + (i - 2).ToString();
                }
            }

            // 現在のコメントの列数（削除後）
            int ColCount = dgvChars.Columns.Count - 3;

            // 削除したコメント列、コメント1つ目を 0 としていくつ目だったか
            int ColAddIndex = clicledIndex - 3;

            for (int i = 0; i < dlcData.Chars.Count; i++)
            {
                string com = dlcData.Chars[i].Comment;


                if (ColAddIndex == 0)
                {
                    int cp = dlcData.Chars[i].Comment.IndexOf(",");
                    if (cp >= 0)
                    {
                        dlcData.Chars[i].Comment = com.Substring(cp + 1);
                    }
                    else
                    {
                        dlcData.Chars[i].Comment = "";
                    }
                }
                else
                {
                    // ColAddIndex 番目（1オリジン）のコンマの位置を取得
                    // ColAddIndex = 0 なら行頭を得る
                    // コンマが足りなかったら負になる
                    int comma = 0;
                    int start = 0;
                    for (int j = 0; j < ColAddIndex; j++)
                    {
                        comma = com.IndexOf(',', start);
                        if (comma < 0)
                        {
                            break;
                        }
                        start = comma + 1;
                    }

                    if (comma < 0)
                    {
                        dlcData.Chars[i].Comment = com;
                    }
                    else
                    {
                        int next = com.IndexOf(',', comma + 1);
                        if (next < 0)
                        {
                            dlcData.Chars[i].Comment = com.Substring(0, comma);
                        }
                        else
                        {
                            dlcData.Chars[i].Comment = com.Substring(0, comma) + com.Substring(next);
                        }
                    }

                    showComment(i);
                }

            }
        }

        private void SedCommentColumnOrder()
        {

            // 編集状態を終了する
            gbChars.Focus();

            // DisplayIndex と Index に差が出ていれば直す
            // 一箇所ずれているだけと仮定
            int from = -1, to = -1;
            for (int i = 3; i < dgvChars.Columns.Count; i++)
            {
                int i2 = dgvChars.Columns[i].DisplayIndex;
                if (i2 == i + 1)
                {
                    to = i;
                    for (int j = i + 1; j < dgvChars.Columns.Count; j++)
                    {
                        if (dgvChars.Columns[j].DisplayIndex == i)
                        {
                            from = j;
                            break;
                        }
                    }
                    break;
                }
                else if (i2 > i + 1)
                {
                    from = i;
                    to = i2;
                    break;
                }

            }

            if (from < 3)
            {
                return;
            }


            // 先に dlcData を更新しておく
            int[] d2i = new int[dgvChars.Columns.Count];
            for (int j = 3; j < dgvChars.Columns.Count; j++)
            {
                d2i[dgvChars.Columns[j].DisplayIndex] = j;
            }
            for (int idx = 0; idx < dgvChars.Rows.Count; idx++)
            {

                // Char に格納する書式を書き直す
                string s = "";
                for (int i = 3; i < dgvChars.Columns.Count; i++)
                {
                    s += dgvChars.Rows[idx].Cells[d2i[i]].Value;
                    if (i < dgvChars.Columns.Count - 1)
                    {
                        s += ",";
                    }
                }
                dlcData.Chars[idx].Comment = s; //SetComment()
            }

            //MessageBox.Show("a");

            if (from >= 3)
            {
                var dirtemp = dgvChars.Columns[from].HeaderCell.SortGlyphDirection;

                var temp = dgvChars.Columns[from];
                dgvChars.Columns.RemoveAt(from);
                dgvChars.Columns.Insert(to, temp);

                dgvChars.Columns[to].HeaderCell.SortGlyphDirection = dirtemp;

                //MessageBox.Show(from + ", " + to);
            }

            // 列のコピーは列の要素まではコピーしてくれないみたいなので。
            for (int i = 0; i < dgvChars.Rows.Count; i++)
            {
                showComment(i);
            }


            for (int i = 3; i < dgvChars.Columns.Count; i++)
            {
                // ヘッダを書き直す
                dgvChars.Columns[i].HeaderText = Program.dicLanguage["Comment"] + " " + (i - 2);
            }

            /*
            var moved = e.Column;
            var oldIndex = moved.Index;
            var newIndex = moved.DisplayIndex;
            //MessageBox.Show(dgvChars.Columns[3].DisplayIndex.ToString() + ", " + dgvChars.Columns[4].DisplayIndex.ToString());
            moved.DisplayIndex = oldIndex;
            //moved.Index = oldIndex;
            //MessageBox.Show(dgvChars.Columns[3].DisplayIndex.ToString() + ", " + dgvChars.Columns[4].DisplayIndex.ToString());
            //dgvChars.Columns.RemoveAt(oldIndex);
            //dgvChars.Columns.Insert(newIndex, moved);
            */

            /*
            // コメントを表示順で格納する配列
            DataGridViewColumn[] commentColumns = new DataGridViewColumn[dgvChars.Columns.Count - 3];

            // 格納
            for(int i = 3; i < dgvChars.Columns.Count; i++)
            {
                commentColumns[dgvChars.Columns[i].DisplayIndex - 3] = dgvChars.Columns[i];
            }

            // その順を普通のインデックスにも適用
            for (int i = 3; i < dgvChars.Columns.Count; i++)
            {
                dgvChars.Columns.Insert()
            }
            */


            //MessageBox.Show((string)dgvChars.Rows[0].Cells[3].Value + ", " + dgvChars.Rows[0].Cells[4].Value);
        }

        private void tbSavePath_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                string bcm = Path.Combine(tbSavePath.Text, Path.GetFileName(tbSavePath.Text) + ".bcm");
                OpenWithApplication("bcm", bcm);
            }
            catch { }

            /*
            try
            {
                OpenWithExplorer(tbSavePath.Text);
            }
            catch { }
            */
        }

        private void tbListPath_DoubleClick(object sender, EventArgs e)
        {

            try
            {
                OpenWithApplication("lst", tbListPath.Text + ".lst");



                /*
                if (File.Exists(tbListPath.Text + ".rst") && !File.Exists(tbListPath.Text + ".lst"))
                {
                    OpenWithExplorer(tbListPath.Text + ".rst");
                }
                else
                {
                    OpenWithExplorer(tbListPath.Text + ".lst");
                }
                */
            }
            catch { }
        }

        private bool DirectoryIsPureDLC(string path)
        {

            // しばらくはこの形で残しておく
            return true;

            /* 暫定的な対応 */
            /*
            string prtDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string kksPath = Path.Combine(prtDir, @"ホゾンデキール");
            if(File.Exists(kksPath))
            {
                return true;
            }
            */

            try
            {
                // 現在のリストファイルのフォルダ
                string curlistfolder;
                try
                {
                    curlistfolder = Path.GetDirectoryName(tbListPath.Text);
                }
                catch
                {
                    curlistfolder = "";
                }

                // path の末尾の \ マークに一貫性を持たせる
                path = Path.GetDirectoryName(Path.Combine(path, "a"));

                // 現在のリストフォルダと同じフォルダに保存しようとしている場合
                // 無条件に却下

                if (path.ToLower() == curlistfolder.ToLower())
                {
                    return false;
                }

                // そのパスが存在しなければ危険はない
                if (!Directory.Exists(path))
                {
                    return true;
                }

                // ファイルを全て取得
                string[] files = Directory.GetFiles(path);

                string foldername = Path.GetFileNameWithoutExtension(path);

                // 想定されないファイルが無いかチェック
                for (int i = 0; i < files.Length; i++)
                {
                    string ext = Path.GetExtension(files[i]).ToLower();
                    if (ext == ".rst")
                    {
                        continue; // rst ファイルは配布用なので仮に失っても再 DLC すればよく損害は軽微と判断
                    }
                    else if (ext == ".lst")
                    {
                        // lst ファイルは中身をみて判断
                        DLCData lstdata = Program.OpenState(files[i]);
                        if (Path.GetDirectoryName(Path.Combine(lstdata.SavePath, "a")).ToLower() == path.ToLower())
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (ext != ".bcm")
                    {
                        return false;
                    }

                    string name = Path.GetFileNameWithoutExtension(files[i]).ToLower();

                    if (name.Substring(name.Length - 1) == "g")
                    {
                        name = name.Substring(0, name.Length - 1);
                    }


                    if (name.Length <= foldername.Length && foldername.Substring(0, name.Length) == name)
                    {
                        continue;
                    }
                    else
                    {
                        return false;
                    }
                }

                // フォルダを全て取得
                string[] folders = Directory.GetDirectories(path);

                // 想定されないフォルダがないかチェック
                if (folders.Length > 1)
                {
                    return false;
                }
                else if (folders.Length == 0)
                {
                    return true; // これ以上危険はない
                }
                else if (Path.GetFileNameWithoutExtension(folders[0]) != "data")
                {
                    return false;
                }

                // data フォルダの中身
                string[] datapath = Directory.GetFiles(folders[0]);

                for (int i = 0; i < datapath.Length; i++)
                {
                    // 拡張子チェック
                    string ext = Path.GetExtension(datapath[i]).ToLower();
                    if (ext != ".bin" && ext != ".blp" && ext != ".lnk")
                    {
                        return false;
                    }

                    // 名前チェック
                    string name = Path.GetFileNameWithoutExtension(datapath[i]).ToLower();
                    bool found = false;
                    for (int j = 0; j < files.Length; j++)
                    {
                        if (Path.GetExtension(files[j]).ToLower() == ".bcm" && name == Path.GetFileNameWithoutExtension(files[j]).ToLower())
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        return false;
                    }
                }

                // 余計なフォルダがないか
                if (Directory.GetDirectories(folders[0]).Length > 0)
                {
                    return false;
                }

                // ここまで来たら安全
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void setBtnSave()
        {
            if (newDlc)
            {
                try
                {
                    if (Directory.Exists(tbSavePath.Text))
                    {
                        btnSave.Text = Program.dicLanguage["OverwriteDLC"];
                        btnCmpSave.Text = Program.dicLanguage["OverwriteCompressedDLC"];
                    }
                    else
                    {
                        btnSave.Text = Program.dicLanguage["SaveDLC"];
                        btnCmpSave.Text = Program.dicLanguage["SaveCompressedDLC"];
                    }
                }
                catch
                {
                    btnSave.Text = Program.dicLanguage["SaveDLC"];
                }
            }
            else
            {

                try
                {
                    if (File.Exists(tbSavePath.Text))
                    {
                        btnSave.Text = Program.dicLanguage["OverwriteBCM"];
                    }
                    else
                    {
                        btnSave.Text = Program.dicLanguage["SaveBCM"];
                    }
                }
                catch
                {
                    btnSave.Text = Program.dicLanguage["SaveBCM"];
                }

                btnCmpSave.Enabled = (GetBinPath(true) != ""); // この場合は TMC を抽出機能
            }
        }

        // 拡張子のないパス scPath を渡されたとき、
        // そのパスに適当な拡張子を付加したファイルを探索し、
        // 最初に見つかったものに param を付け加えたコマンドを実行する
        // scPath はダブルコーテーションを含んではならない
        // param には必要に応じてダブルコーテーションを付加しなければならない
        // param は空文字でもよい
        // コマンを実行した場合のみ true を返す。
        // この関数はいかなる場合もエラーを投げない。
        // test == true の場合は、実際に実行せず実行可能かどうかのみ返す
        private bool OpenWithShortcut(string scPath, string param, bool test)
        {
            string[] exts = new string[] { ".lnk", ".lnk.lnk", ".bat", ".vbs", ".js", "wsh" };
            // .lnk.lnk は多くの環境でショートカットファイルの拡張子 .lnk が表示されないことを知らないユーザーへの配慮

            try
            {
                string foundScPath = "";
                for (int i = 0; i < exts.Length; i++)
                {
                    string scPathWithExt = scPath + exts[i];
                    if (File.Exists(scPathWithExt))
                    {
                        foundScPath = scPathWithExt;
                        break;
                    }
                }

                if (foundScPath == "")
                {
                    return false;
                }
                else
                {
                    if (!test)
                    {


                        // 念のためカレントディレクトリを動かしておく
                        string sCD = System.Environment.CurrentDirectory;
                        System.Environment.CurrentDirectory = Path.GetDirectoryName(foundScPath);


                        if (param != "")
                        {
                            System.Diagnostics.Process.Start("\"" + foundScPath + "\"", param);
                        }
                        else
                        {
                            System.Diagnostics.Process.Start("\"" + foundScPath + "\"");
                        }

                        // カレントディレクトリを元に戻す
                        System.Environment.CurrentDirectory = sCD;
                    }

                    return true;
                }
            }
            catch { }

            return false;
        }


        private void OpenWithApplication(string scName, string param)
        {
            string path = Path.Combine(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"Applications"), scName);

            if (File.Exists(param))
            {
                if (!OpenWithShortcut(path, "\"" + param + "\"", false))
                {
                    OpenWithExplorer(param);
                }
            }
            else
            {
                OpenWithExplorer(param);
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {

            if (dgvChars.SelectedRows.Count == 1) // 起動直後などは SelectedRows がなかったりするので。
            {
                int index = dgvChars.SelectedRows[0].Index;
                RedrawFiles(index, false);
            }

            /*
            System.Drawing.Point pt = new System.Drawing.Point(tbListPath.Location.X +  tbListPath.Width, tbListPath.Location.Y);
            var pos = tbListPath.GetCharIndexFromPosition(pt);
            int tail = ;
            //if (pos >= tail - 1) pos = tail;
            */
            tbListPath.Select(tbListPath.Text.Length, 0);
            tbListPath.ScrollToCaret();
        }

        private void dgvFiles_MouseDown(object sender, MouseEventArgs e)
        {

            if (dgvChars.SelectedRows.Count != 1)
            {
                return;
            }

            // 左クリック
            if ((e.Button == MouseButtons.Left && btnCharsAdd.Enabled))
            {

                // MouseDownイベント発生時の (x,y)座標を取得
                int index = dgvFiles.HitTest(e.X, e.Y).RowIndex;
                if (index < 0)
                {

                    for (int i = 0; i < dgvFiles.Rows.Count; i++)
                    {
                        dgvFiles.Rows[i].Selected = false;
                    }
                    if (btnFilesDelete.Enabled) // ちらつき抑え
                    {
                        btnFilesDelete.Enabled = false;
                    }
                }
            }
            // 右クリック
            else if (e.Button == MouseButtons.Right && btnCharsAdd.Enabled)
            {

                // MouseDownイベント発生時の (x,y)座標を取得
                int index = dgvFiles.HitTest(e.X, e.Y).RowIndex;
                //if (index < 0) return;
                if (index >= 0)
                {
                    //string str = dgvFiles.Rows[index].Cells[0].Value.ToString();
                    /*
                    bool onSelected = false;
                    try
                    {
                        onSelected = (dgvFiles.SelectedRows.Count > 0);
                    }
                    catch { }
                    */
                    if (!dgvFiles.Rows[index].Cells[0].Selected)
                    {

                        for (int i = 0; i < dgvFiles.Rows.Count; i++)
                        {
                            dgvFiles.Rows[i].Selected = (i == index);
                        }
                    }
                    /*
                    else
                    {

                        for (int i = 0; i < dgvFiles.Rows.Count; i++)
                        {
                            dgvFiles.Rows[i].Selected = (i == index);
                        }
                    }
                    */
                }
                else
                {

                    for (int i = 0; i < dgvFiles.Rows.Count; i++)
                    {
                        dgvFiles.Rows[i].Selected = false;
                    }
                    if (btnFilesDelete.Enabled) // ちらつき抑え
                    {
                        btnFilesDelete.Enabled = false;
                    }

                }


                clikedForm = "dgvFiles";
                コピーCtrlCToolStripMenuItem.Enabled = btnFilesDelete.Enabled;
                削除DeleteToolStripMenuItem.Enabled = btnFilesDelete.Enabled;
                ClearPasteToolStripMenuItem.Enabled = (dgvFiles.Rows.Count > 0);

                string[] paths = (string[])Clipboard.GetDataObject().GetData(DataFormats.FileDrop);
                bool canpaste = AddFiles(paths, true); // テストモード
                貼り付けCtrlVToolStripMenuItem.Enabled = canpaste;
                ClearPasteToolStripMenuItem.Enabled = canpaste;
                contextMenuStrip1.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

        private void dgvFiles_SelectionChanged(object sender, EventArgs e)
        {
            btnFilesDelete.Enabled = (dgvFiles.SelectedRows.Count > 0);
        }

        private void dgvFiles_KeyDown(object sender, KeyEventArgs e)
        {

            if (dragStartIndexes != null) return;


            if (e.KeyCode == Keys.C && CKeyUp && e.Control)
            {
                CKeyUp = false;

                //コピーするファイルのパスをStringCollectionに追加する
                System.Collections.Specialized.StringCollection files =
                    new System.Collections.Specialized.StringCollection();

                if (dgvFiles.SelectedRows.Count <= 0)
                {
                    return;
                }

                for (int i = 0; i < dgvFiles.SelectedRows.Count; i++)
                {
                    files.Add(dgvFiles.SelectedRows[i].Cells[0].Value.ToString());
                }
                //クリップボードにコピーする
                Clipboard.SetFileDropList(files);

            }
            else if (e.KeyCode == Keys.V && VKeyUp && e.Control && Clipboard.GetDataObject().GetDataPresent(DataFormats.FileDrop))
            {
                VKeyUp = false;
                string[] paths = (string[])Clipboard.GetDataObject().GetData(DataFormats.FileDrop);
                AddFiles(paths, false);
                /* AddFiles の中でやることにした
                //setEgvCharsSlotColor(); // 流石にこれは不要
                setEgvCharsNameColor();
                setEgvCharsTextsColor();
                */

            }
            else if (e.KeyCode == Keys.Delete && DeleteKeyUp)
            {
                btnFilesDelete_Click(null, null);
                DeleteKeyUp = false;
            }

            // これは ListBox だった頃は必要だった
            /*
            else if (e.KeyCode == Keys.A && DeleteKeyUp)
            {
                int FileCount;
                try
                {
                    FileCount = dgvFiles.SelectedRows.Count;
                }
                catch
                {
                    return;
                }
                for (int i = 0; i < FileCount; i++)
                {
                    dgvFiles.Rows[i].Selected = true;
                }
            }
            */
        }

        private void dgvFiles_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Delete)
            {
                DeleteKeyUp = true;
            }
            else if (e.KeyCode == Keys.C)
            {
                CKeyUp = true;
            }
            else if (e.KeyCode == Keys.V)
            {
                VKeyUp = true;
            }
        }

        private void dgvFiles_DoubleClick(object sender, EventArgs e)
        {

            if (dgvFiles.SelectedRows.Count == 1)
            {
                string path = dgvFiles.SelectedRows[0].Cells[0].Value.ToString();
                OpenWithApplication(Path.GetExtension(path).Substring(1), path);
            }
        }


        string GetOwnOFolderrExistingParent(string path)
        {
            try
            {
                /*
                if (File.Exists(path))
                {
                    return path;
                }
                */


                while (!String.IsNullOrEmpty(path) && !Directory.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                }

                if (String.IsNullOrEmpty(path))
                {
                    return "";
                }
                else
                {
                    return path;
                }
            }
            catch
            {
                return "";
            }
        }
        

        private void btnExtractFiles()
        {
            // ダイアログを開く前に一回チェック
            var loadPath = GetBinPath(true);
            if(loadPath == "")
            {
                setBtnSave();
                var e = new FileNotFoundException(null,GetBinPath(false));
                MessageBox.Show(e.Message, Program.dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string pt;
            try
            {
                pt = GetOwnOFolderrExistingParent(Path.GetDirectoryName(LoadIniString("InitialDirectory", "DATA")));
            }
            catch
            {
                pt = "";
            }
            if (pt != "") saveFileDialogTMC.InitialDirectory = pt;
            var fn = Path.GetFileName(Path.GetFileNameWithoutExtension(loadPath));

            // 入力後にパスの修正を行わない代わりに " (2)" を末尾につける
            var defaultFilePath = Path.Combine(pt, fn);
            for (var i = 2; Directory.Exists(defaultFilePath) || File.Exists(defaultFilePath) ; i++)
            {
                defaultFilePath = Path.Combine(pt, fn + " (" + i + ")");
            }
            fn = Path.GetFileName(defaultFilePath);

            saveFileDialogTMC.FileName = fn;
            if (saveFileDialogTMC.ShowDialog() == DialogResult.OK)
            {

                string savePath = saveFileDialogTMC.FileName;

                // 安全性などを考えて、現段階では“あえて”パスの修正は行わない

                // フォルダ名が重なっていた場合に重複を避ける処理
                /*
                var defpath = Path.Combine(pt, fn);
                if (savePath == Path.Combine(defpath, fn))
                {
                    savePath = defpath;
                }
                */

                // フォルダ内のファイルを選択した場合にフォルダを選択する処理
                /*
                for (int i = 0; i < FileOrder.Length; i++)
                {
                    if (savePath.EndsWith(MainForm.FileOrder[i]))
                    {
                        savePath = Path.GetDirectoryName(savePath);
                        break;
                    }
                }
                */

                SaveIniString("InitialDirectory", "DATA", savePath);
                ExtractFiles(savePath, loadPath);

            }
        }

        private void ExtractFiles(string savePath, string loadPath)
        {
            try
            {
                var archData = Archive_Tool.Program.ParseArchive(loadPath, DAT); // loadPath == "" でもエラーを投げてくれない
                if (archData == null)
                {
                    if (!File.Exists(loadPath))
                    {
                        throw new FileNotFoundException(null, loadPath);
                    }
                    else if (!File.Exists(DAT))
                    {
                        throw new FileNotFoundException(null, DAT);
                    }
                    else
                    {
                        throw new ArgumentNullException("archData");
                    }
                }

                var targetArchData = new System.Collections.Generic.List<Archive_Tool.ArchiveFile>();

                var Chars = dlcData.Chars;


                tbSavePath.Text = Path.GetDirectoryName(Path.GetDirectoryName(loadPath));
                newDlc = true;
                ClearMainUI();
                dlcData = new DLCData();
                setBtnSave();
                btnCmpSave.Enabled = btnSave.Enabled = true;
                btnSaveState.Enabled = true;
                btnCharsAdd.Enabled = true;
                clmInner.ReadOnly = false;
                dlcData.BcmVer = 9;
                tbBCMVer.Text = "9";
                cbSaveListInDLC.Enabled = true;
                cbSaveListInDLC.Checked = true;

                if(File.Exists(savePath))
                {
                    File.Delete(savePath);
                }
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                var userFlags = new bool[] { false, false, false, false };

                // 主要な計算は明らかに抽出処理なのでこれは不要というか、無いほうが良い
                // と思ったが、処理中はリストの更新は行われずチェックボックスだけがチラついたのでやっぱりやる
                var dgvChars_SelectionChanged_RepairingSelection_current = dgvChars_SelectionChanged_RepairingSelection;
                dgvChars_SelectionChanged_RepairingSelection = true;

                // スクロールバーが変に動くのでファイルの抽出だけは先にしておく
                SetProgressBar(true, Chars.Count);
                for (var i = 0; i < Chars.Count; i++)
                {
                    IncrementProgressBar();

                    var chr = Chars[i];

                    var decName = GetDecName(chr);
                    for (var j = 0; j < archData.Length; j++)
                    {
                        var archFile = archData[j];
                        if (archFile.Name.StartsWith(decName))
                        {
                            var dataFilePath = Path.Combine(savePath, archFile.Name);
                            var result = Archive_Tool.Program.ExtractFile(archFile, dataFilePath, userFlags);
                            if(result != null)
                            {
                                throw new Exception(result.Item1);
                            }
                        }
                    }
                }


                for (var i = 0; i < Chars.Count; i++)
                {
                    //IncrementProgressBar();

                    var chr = Chars[i];
                    

                    // これは一番下に追加する処理
                    dlcData.Chars.Add(chr);
                    dgvChars.Rows.Add();
                    dgvChars.Rows[dgvChars.Rows.Count - 1].Cells[0].Value = GetCharNamesJpn(chr.ID);// Program.CharNamesJpn[chr.ID];
                    dgvChars.Rows[dgvChars.Rows.Count - 1].Cells[1].Value = chr.CostumeSlot.ToString();
                    dgvChars.Rows[dgvChars.Rows.Count - 1].Cells[2].Value = chr.AddTexsCount.ToString();
                    showComment(dgvChars.Rows.Count - 1);
                    dgvChars.ClearSelection(); //AddFiles のために必要
                    dgvChars.Rows[dgvChars.Rows.Count - 1].Selected = true; //AddFiles のために必要

                    var decName = GetDecName(chr);
                    var fileNameList = new System.Collections.Generic.List<string>();
                    for(var j = 0; j < archData.Length; j++)
                    {
                        var archFile = archData[j];
                        if(archFile.Name.StartsWith(decName) )
                        {
                            var dataFilePath = Path.Combine(savePath, archFile.Name);
                            fileNameList.Add(dataFilePath);
                        }
                    }
                    var fileNames = new string[fileNameList.Count];
                    for(int j = 0; j < fileNames.Length; j++)
                    {
                        fileNames[j] = fileNameList[j];
                    }
                    AddFiles(fileNames, false, false);
                }

                // 主要な計算は明らかに抽出処理なのでこれは不要というか、無いほうが良い
                // と思ったが、処理中はリストの更新は行われずチェックボックスだけがチラついたのでやっぱりやる
                dgvChars_SelectionChanged_RepairingSelection = dgvChars_SelectionChanged_RepairingSelection_current;

                //1つめを選択。この時点で描画モードはオンになってないとダメ
                dgvChars.ClearSelection(); //AddFiles のために必要
                if(dgvChars.Rows.Count > 0) dgvChars.Rows[0].Selected = true; //AddFiles のために必要


                //コピーじゃないのでこれは必要
                btnCharsDelete.Enabled = true;
                btnFilesAdd.Enabled = true;

                setEgvCharsSlotColor(); // これらも追加
                setEgvCharsNameColor();
                setEgvCharsTextsColor(); // デフォルト機能により初めから問題があることも多々


                SetProgressBar(false, 0);

                return;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, Program.dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetBinPath(bool checkExistence)
        {
            try
            {
                string fileName = tbSavePath.Text;
                if (Path.GetExtension(fileName).ToLower() != ".bcm")
                {
                    return "";
                }

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\\(\d+)\.bcm$",
                   System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                if (regex.IsMatch(fileName))
                {
                    fileName = regex.Replace(fileName, "\\data\\$1.bin");
                }
                else
                {
                    return "";
                }

                if ((!checkExistence) || File.Exists(fileName))
                {
                    return fileName;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }

        private string GetDecName(Character chr)
        {
            // オリジナルを参考にしてるから問題無いでしょう
            return Program.CharNames[chr.ID] + "_DLCU_" + (chr.CostumeSlot + 1).ToString("D3");
        }
        
        private string GetCharNamesJpn(byte ID)
        {
            return GetCharNamesJpn(ID, true);
        }
        private string GetCharNamesJpn(byte ID, bool ShowError)
        {
            try
            {
                return Program.CharNamesJpn[ID];
            }
            catch(System.Collections.Generic.KeyNotFoundException e)
            {
                if (ShowError)
                {

                    var CharInfoPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"CharactersInfo");
                    var subPath = Path.Combine(Path.GetFileName(Path.GetDirectoryName(CharInfoPath)), Path.GetFileName(CharInfoPath));

                    System.Text.RegularExpressions.Regex regexReplace = new System.Text.RegularExpressions.Regex(@"(.+)");
                    var msg = regexReplace.Replace(ID.ToString(), Program.dicLanguage["IDXIsUnknown"]) + "\n" +
                         regexReplace.Replace(subPath, Program.dicLanguage["ProblemMayBeSolvedByEditingX"]);

                    // アウトオブレンジな ID があった場合のエラー処理をすべてやるのは大変なのでプログラムを終わっちゃおう

                    MessageBox.Show(msg, Program.dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                    return "";
                    //throw new System.Collections.Generic.KeyNotFoundException(msg);
                }
                else
                {
                    return "Unknown";
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }

}
