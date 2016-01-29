namespace DLC_Tool
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    internal static class Program
    {
        // これらは外部ファイルで指定するように仕様変更
        /*
        public static readonly Dictionary<byte, string> CharNames = new Dictionary<byte, string>()
        {
            { 0, "ZACK" }, { 1, "TINA" }, { 2, "JANNLEE" }, { 3, "EIN" }, { 4, "HAYABUSA" }, { 5, "KASUMI" }, { 6, "GENFU" }, { 7, "HELENA" }, { 8, "LEON" }, { 9, "BASS" },
            { 10, "KOKORO" }, { 11, "HAYATE" }, { 12, "LEIFANG" }, { 13, "AYANE" }, { 14, "ELIOT" }, { 15, "LISA" }, { 16, "ALPHA152" }, { 19, "BRAD" }, { 20, "CHRISTIE" },
            { 21, "HITOMI" }, { 24, "BAYMAN" }, { 29, "RIG" }, { 30, "MILA" }, { 31, "AKIRA" }, { 32, "SARAH" }, { 33, "PAI" }, { 39, "MOMIJI" }, { 40, "RACHEL" },
            { 41, "JACKY" }, { 42, "MARIE" }, { 43, "PHASE4" }, { 44, "NYOTENGU" }, { 45, "HONOKA" }, { 46, "RAIDOU" }
        };

        public static readonly Dictionary<byte, string> CharNamesJpn = new Dictionary<byte, string>()
        {
            { 0, "ザック" }, { 1, "ティナ" }, { 2, "ジャン・リー" }, { 3, "アイン" }, { 4, "リュウ・ハヤブサ" }, { 5, "かすみ" }, { 6, "ゲン・フー" }, { 7, "エレナ" }, { 8, "レオン" }, { 9, "バース" },
            { 10, "こころ" }, { 11, "ハヤテ" }, { 12, "レイファン" }, { 13, "あやね" }, { 14, "エリオット" }, { 15, "リサ" }, { 16, "Alpha-152" }, { 19, "ブラッド・ウォン" }, { 20, "クリスティ" },
            { 21, "ヒトミ" }, { 24, "バイマン" }, { 29, "リグ" }, { 30, "ミラ" }, { 31, "アキラ" }, { 32, "サラ" }, { 33, "パイ・チェン" }, { 39, "紅葉" }, { 40, "レイチェル" },
            { 41, "ジャッキー" }, { 42, "マリー・ローズ" }, { 43, "PHASE-4" }, { 44, "女天狗" }, { 45, "ほのか" }, { 46, "雷道" }
        };

        public static readonly Dictionary<byte, byte> NumOfSlots = new Dictionary<byte, byte>()
        {
            {0, 10}, // ザック
            {1, 40}, // ティナ
            {2, 10}, // ジャン・リー
            {3, 10}, // アイン
            {4, 10}, // リュウ・ハヤブサ
            {5, 40}, // かすみ
            {6, 10}, // ゲン・フー
            {7, 40}, // エレナ
            {8, 10}, // レオン
            {9, 10}, // バース
            {10, 40}, // こころ
            {11, 10}, // ハヤテ
            {12, 40}, // レイファン
            {13, 40}, // あやね
            {14, 10}, // エリオット
            {15, 40}, // リサ
            {16, 21}, // Alpha-152
            {19, 10}, // ブラッド・ウォン
            {20, 40}, // クリスティ
            {21, 40}, // ヒトミ
            {24, 10}, // バイマン
            {29, 10}, // リグ
            {30, 40}, // ミラ
            {31, 10}, // アキラ
            {32, 15}, // サラ
            {33, 15}, // パイ・チェン
            {39, 40}, // 紅葉
            {40, 40}, // レイチェル
            {41, 10}, // ジャッキー
            {42, 32}, // マリー・ローズ
            {43, 32}, // PHASE-4
            {44, 32}, // 女天狗
            {45, 32}, // ほのか
            {46, 10}  // 雷道
        };

            
        public static readonly List<byte> FemaleIDs = new List<byte> { 1, 5, 7, 10, 12, 13, 15, 16, 20, 21, 30, 32, 33, 39, 40, 42, 43, 44, 45 };
        */
        public static Dictionary<byte, string> CharNames;
        public static Dictionary<byte, string> CharNamesJpn;
        public static Dictionary<byte, byte> NumOfSlots;
        public static List<byte> FemaleIDs;

        public struct SlotTable<T>
        {
            private T[][] CharSlot;
            public const int Length = 47;
            public SlotTable( T default_value )
            {
                CharSlot = new T[Length][];
                for(byte i = 0; i < Length; i++)
                {
                    CharSlot[i] = new T[NumOfSlots.ContainsKey(i) ? NumOfSlots[i] : 0];
                }
                for(int i = 0; i < Length; i++)
                {
                    int Length2;
                    try
                    {
                        Length2 = CharSlot[i].Length;
                    }
                    catch
                    {
                        Length2 = 0;
                    }
                    for (int j = 0; j < Length2; j++)
                    {
                        CharSlot[i][j] = default_value;
                    }
                }
            }

            public T[] this[int i]
            {
                get
                {
                    return CharSlot[i];
                }
            }
            public T this[int i, int j]
            {
                get
                {
                    if (0 <= j && j < this[i].Length)
                    {
                        return this[i][j];
                    }
                    else
                    {
                        return default(T);
                    }
                }
                set
                {
                    if (0 <= j && j < this[i].Length)
                    {
                        this[i][j] = value;
                    }
                }
            }
            public T this[Character Char]
            {
                get
                {
                    return this[Char.ID, Char.CostumeSlot];
                }
                set
                {
                    this[Char.ID, Char.CostumeSlot] = value;
                }
            }

            public int Count() { return Length; }
        }

        public static Dictionary<string, string> dicLanguage = new Dictionary<string, string> { };


        private static List<string> decNames, encNames;

        public static DLCData OpenBCM(string fileName)
        {
            var dlcData = new DLCData();
            dlcData.skipRead = 0;
            using (var ms = new MemoryStream())
            {
                var br = new BinaryReader(ms);
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.CopyTo(br.BaseStream);
                }

                br.BaseStream.Position = 0;
                dlcData.BcmVer = br.ReadByte();
                br.BaseStream.Position += 1;
                byte entriesCount = br.ReadByte();
                br.BaseStream.Position += 5;
                for (int i = 0; i < entriesCount; i++)
                {
                    var charEntry = new Character();
                    charEntry.ID = br.ReadByte();
                    charEntry.CostumeSlot = br.ReadByte();
                    if (charEntry.CostumeSlot > 100)
                    {
                        dlcData.skipRead ++;
                        continue;
                    }
                    charEntry.AddTexsCount = br.ReadByte();
                    byte hStylesCount = br.ReadByte();
                    for (int j = 0; j < hStylesCount; j++)
                    {
                        charEntry.HStyles.Add(new Hairstyle(br.ReadByte(), br.ReadByte()));
                    }

                    dlcData.Chars.Add(charEntry);
                }
            }
            if (dlcData.skipRead > 0)
            {

                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\$1");
                MessageBox.Show(regex.Replace(dicLanguage["SkippedNItems"], dlcData.skipRead.ToString()) + "\r\n" + dicLanguage["OverwritingKillsSkipedData"], dicLanguage["Notice"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return dlcData;
        }

        public static DLCData OpenBCM_超原始的修正(string fileName)
        {
            var dlcData = new DLCData();
            dlcData.skipRead = 0;
            using (var ms = new MemoryStream())
            {
                var br = new BinaryReader(ms);
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.CopyTo(br.BaseStream);
                }

                br.BaseStream.Position = 0;
                dlcData.BcmVer = br.ReadByte();
                br.BaseStream.Position += 1;
                byte entriesCount = br.ReadByte();
                br.BaseStream.Position += 5;
                for (int i = 0; i < entriesCount; i++)
                {
                    var charEntry = new Character();
                    charEntry.ID = br.ReadByte();
                    charEntry.CostumeSlot = br.ReadByte();
                    if (charEntry.CostumeSlot > 100)
                    {
                        dlcData.skipRead++;
                        continue;
                    }
                    charEntry.AddTexsCount = br.ReadByte();
                    byte hStylesCount = br.ReadByte();
                    for (int j = 0; j < hStylesCount; j++)
                    {
                        charEntry.HStyles.Add(new Hairstyle(br.ReadByte(), br.ReadByte()));
                    }

                    dlcData.Chars.Add(charEntry);
                }
            }
            if (dlcData.skipRead > 0)
            {
                if ((System.Text.RegularExpressions.Regex.IsMatch(fileName, @"\b358142\b") && dlcData.skipRead == 12))
                {
                    // 何もしない
                }
                else
                {
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\$1");
                    MessageBox.Show(regex.Replace(dicLanguage["SkippedNItems"], dlcData.skipRead.ToString()) + "\r\n" + dicLanguage["OverwritingKillsSkipedData"], dicLanguage["Notice"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            return dlcData;
        }


        public static void SaveBCM(DLCData dlcData, string savePath, string dlcName)
        {
            ulong dlcNum;
            if (!ulong.TryParse(dlcName, out dlcNum))
            {
                throw new Exception(dicLanguage["FileNameMustBeDigits"]);
            }

            ulong checkSum = 0;
            for (int i = 0; i < dlcData.Chars.Count; i++)
            {
                checkSum += (ushort)(dlcData.Chars[i].ID * dlcData.Chars[i].CostumeSlot);
            }

            checkSum = ((dlcNum + 1) * (checkSum & 4095)) % ((dlcData.BcmVer * dlcNum) + 17);

            using (var ms = new MemoryStream())
            {
                var bw = new BinaryWriter(ms);
                bw.Write(dlcData.BcmVer);
                bw.BaseStream.Position += 1;
                bw.Write((byte)dlcData.Chars.Count);
                bw.BaseStream.Position += 1;
                bw.Write(checked((uint)checkSum));
                for (int i = 0; i < dlcData.Chars.Count; i++)
                {
                    bw.Write(dlcData.Chars[i].ID);
                    bw.Write(dlcData.Chars[i].CostumeSlot);
                    bw.Write(dlcData.Chars[i].AddTexsCount);
                    bw.Write((byte)dlcData.Chars[i].HStyles.Count);
                    for (int j = 0; j < dlcData.Chars[i].HStyles.Count; j++)
                    {
                        bw.Write(dlcData.Chars[i].HStyles[j].Hair);
                        bw.Write(dlcData.Chars[i].HStyles[j].Face);
                    }
                }

                bw.BaseStream.Position = 0;
                using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    bw.BaseStream.CopyTo(fs);
                }
            }
        }

        
        public static List<int> SaveBIN(DLCData dlcData, string savePath, string dlcName)
        {
            if (decNames == null)
            {
                string datname;
                string parent = Path.GetDirectoryName(Application.ExecutablePath);
                /*
                if (File.Exists(parent + @"\file5lr.dat"))
                {
                    datname = parent + @"\file5lr.dat";
                }
                else
                {
                    datname = parent + @"\dlc5lr.dat";
                }
                */
                datname = MainForm.DAT;

                using (var sr = new StreamReader(datname))
                {
                    decNames = new List<string>();
                    encNames = new List<string>();
                    while (!sr.EndOfStream)
                    {
                        string[] inLine = sr.ReadLine().Split('\t');
                        encNames.Add(inLine[0]);
                        decNames.Add(inLine[1]);
                    }
                }
            }

            var nameIndexes = new List<int>();
            var fileNames = new List<string>();
            for (int i = 0; i < dlcData.Chars.Count; i++)
            {
                if ((dlcData.Chars[i].Files[0] == null) || (dlcData.Chars[i].Files[1] == null)
                    || (dlcData.Chars[i].Files[2] == null) || (dlcData.Chars[i].Files[11] == null))
                {

                    System.Text.RegularExpressions.Regex regex1 = new System.Text.RegularExpressions.Regex(@"\$1");
                    System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@"\$2");
                    string mes = regex1.Replace(regex2.Replace(dicLanguage["NotFoundSomeOfFilesXOfCharY"], CharNames[dlcData.Chars[i].ID] + "[" + dlcData.Chars[i].CostumeSlot + "]"), "TMC/TMCL/---C/--P");

                    throw new Exception(mes);
                }

                string decTmcName = CharNames[dlcData.Chars[i].ID] + "_DLCU_" + (dlcData.Chars[i].CostumeSlot + 1).ToString("D3") + ".TMC";
                int nameIndex = decNames.IndexOf(decTmcName);
                if (nameIndex == -1)
                {
                    /*
                    throw new Exception(CharNames[dlcData.Chars[i].ID] + "[" + dlcData.Chars[i].CostumeSlot + "] の名前はデータベースに存在しません");
                    */

                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\$1");
                    string mes = regex.Replace(dicLanguage["NotFoundNameX"], CharNames[dlcData.Chars[i].ID] + "[" + dlcData.Chars[i].CostumeSlot + "]");

                }

                nameIndexes.Add(nameIndex);
                nameIndexes.Add(nameIndex + 1);
                nameIndexes.Add(nameIndex + 2);
                fileNames.Add(encNames[nameIndex]);
                fileNames.Add(encNames[nameIndex + 1]);
                fileNames.Add(encNames[nameIndex + 2]);
                if (dlcData.Chars[i].AddTexsCount > 1)
                {
                    for (int j = 0; j < dlcData.Chars[i].AddTexsCount; j++)
                    {
                        if ((dlcData.Chars[i].Files[3 + (j * 2)] == null) || (dlcData.Chars[i].Files[4 + (j * 2)] == null))
                        {
                            /*
                            throw new Exception(CharNames[dlcData.Chars[i].ID] + "["
                                + dlcData.Chars[i].CostumeSlot + "]の" + (j + 1).ToString("D3") + ".--H/--HLファイルのいずれかが見つかりません");
                                */


                            System.Text.RegularExpressions.Regex regex1 = new System.Text.RegularExpressions.Regex(@"\$1");
                            System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@"\$2");
                            string mes = regex1.Replace(regex2.Replace(dicLanguage["NotFoundSomeOfFilesXOfCharY"], CharNames[dlcData.Chars[i].ID] + "["
                                + dlcData.Chars[i].CostumeSlot + "]"), ".--H/--HL");


                            throw new Exception(mes);

                        }

                        nameIndexes.Add(nameIndex + 3 + (j * 2));
                        nameIndexes.Add(nameIndex + 4 + (j * 2));
                        fileNames.Add(encNames[nameIndex + 3 + (j * 2)]);
                        fileNames.Add(encNames[nameIndex + 4 + (j * 2)]);
                    }
                }

                nameIndex = decNames.IndexOf(Path.ChangeExtension(decTmcName, ".--P"), nameIndex + 3);
                nameIndexes.Add(nameIndex);
                fileNames.Add(encNames[nameIndex]);
            }

            int headerSize = 40 + (fileNames.Count * 12);
            using (var ms = new MemoryStream())
            {
                var bw = new BinaryWriter(ms, new ASCIIEncoding());
                bw.BaseStream.Position = headerSize;
                bw.Write(("_output/costume_pack_" + dlcName + char.MinValue + "order" + char.MinValue).ToCharArray());
                var namesOffs = new List<int>();
                foreach (string fileName in fileNames)
                {
                    namesOffs.Add((int)bw.BaseStream.Position);
                    bw.Write((@"/" + fileName + char.MinValue).ToCharArray());
                }

                bw.BaseStream.Position = 0;
                bw.Write(0x4F4D464C);
                bw.Write(1);
                bw.Write(fileNames.Count);
                bw.Write(0x20);
                bw.Write(0x28);
                bw.Write(headerSize);
                bw.Write(namesOffs[0]);
                bw.Write(0);
                bw.Write(0);
                bw.Write(headerSize);
                for (int i = 0; i < namesOffs.Count; i++)
                {
                    bw.Write(0);
                    bw.Write(i);
                    bw.Write(namesOffs[i]);
                }

                bw.BaseStream.Position = 0;
                using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    bw.BaseStream.CopyTo(fs);
                }
            }

            return nameIndexes;
        }

        public static List<uint> SaveLNK(DLCData dlcData, string savePath, int fileCount)
        {
            int headerSize = 32 + (fileCount * 32);
            if (headerSize % 2048 > 0)
            {
                headerSize = headerSize - (headerSize % 2048) + 2048;
            }

            var fileSizes = new List<uint>();
            var fileOffs = new List<uint>();
            using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                fs.Position = headerSize;
                for (int i = 0; i < dlcData.Chars.Count; i++)
                {
                    int orderLimit = 2;
                    if (dlcData.Chars[i].AddTexsCount > 1)
                    {
                        orderLimit += dlcData.Chars[i].AddTexsCount * 2;
                    }

                    for (int j = 0; j < 12; j++)
                    {
                        if ((j > orderLimit) && (j < 11))
                        {
                            continue;
                        }

                        fileOffs.Add((uint)fs.Position);
                        using (var fs2 = new FileStream(dlcData.Chars[i].Files[j], FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            fileSizes.Add((uint)fs2.Length);
                            fs2.CopyTo(fs);
                        }

                        if (fs.Position % 2048 > 0)
                        {
                            fs.Position = fs.Position - (fs.Position % 2048) + 2048;
                        }
                    }
                }

                var bw = new BinaryWriter(fs);
                if (fs.Length % 2048 > 0)
                {
                    bw.BaseStream.Position -= 1;
                    bw.Write((byte)0);
                }

                bw.BaseStream.Position = 0;
                bw.Write(0x4D444350);
                bw.Write(0);
                bw.Write(fileCount);
                bw.Write(0);
                bw.Write((uint)bw.BaseStream.Length);
                bw.Write(0);
                bw.Write(2048);
                bw.Write(0);
                for (int i = 0; i < fileCount; i++)
                {
                    bw.Write(fileOffs[i]);
                    bw.Write(0);
                    bw.Write(fileSizes[i]);
                    bw.Write(0);
                    bw.Write(fileSizes[i]);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(0);
                }
            }

            return fileSizes;
        }

        public static void SaveBLP(string savePath, List<int> nameIndexes, List<uint> fileSizes)
        {
            using (var ms = new MemoryStream())
            {
                ms.Position = 16;
                var bw = new BinaryWriter(ms);
                for (int i = 0; i < nameIndexes.Count; i++)
                {
                    bw.Write(nameIndexes[i]);
                    bw.Write(fileSizes[i]);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(0);
                }

                bw.BaseStream.Position = 0;
                bw.Write(0x46495031);
                bw.Write(nameIndexes.Count);
                bw.Write(16);
                bw.Write((int)bw.BaseStream.Length);
                bw.BaseStream.Position = 0;
                using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    bw.BaseStream.CopyTo(fs);
                }
            }
        }

        public static DLCData OpenState(string fileName) { return OpenState(fileName, true); }
        public static DLCData OpenState(string fileName, bool ShowBadNameNotice)
        {
            var dlcData = new DLCData();
            string group = "";
            string sTemp = "";
            int groupLine = 0;
            int charInd = -1;
            bool skipChar = false;
            Character charEntry = null;

            using (StreamReader sr = new StreamReader(fileName))
            {
                var BadNameList = new List<string>();
                while (sr.Peek() >= 0) 
                {
                    string s = sr.ReadLine();

                    if (s == "path")
                    {
                        group = "path";
                    }
                    else if (s == "character")
                    {
                        if (charInd != -1 && !skipChar)
                        {
                            dlcData.Chars.Add(charEntry);
                        }
                        group = "character";
                        groupLine = 0;
                        charInd = charInd + 1;
                        skipChar = false;
                    }
                    else if (s == "hairstyles" && !skipChar)
                    {
                        group = "hairstyles";
                        groupLine = 0;
                    }
                    else if (s == "files" && !skipChar)
                    {
                        if(charEntry.HStyles.Count == 0)
                        {
                            charEntry.HStyles.Add(new Hairstyle(1, 1));
                        }
                        else if (groupLine == 1 && group == "hairstyles")
                        {
                            charEntry.HStyles.Add(new Hairstyle(byte.Parse(sTemp), 1));
                        }
                        group = "files";
                    }
                    else if (group == "path")
                    {
                        dlcData.SavePath = s;
                    }
                    else if (group == "character" && !skipChar)
                    {
                        if (groupLine == 0)
                        {
                            foreach (byte key in CharNames.Keys)
                            {
                                if (CharNames[key] == s)
                                {
                                    charEntry = new Character();
                                    charEntry.ID = key;
                                    charEntry.CostumeSlot = 0;
                                    charEntry.AddTexsCount = 1;
                                    skipChar = false;
                                    if (FemaleIDs.Contains(key))
                                    {
                                        charEntry.Female = true;
                                    }
                                    break;
                                }
                                skipChar = true;
                            }
                            if (skipChar && ShowBadNameNotice)
                            {
                                /*
                                MessageBox.Show("[" + s + "]の名前に問題があるため読込はスキップされました", "注意", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                */

                                if(BadNameList.IndexOf(s) < 0)
                                {
                                    BadNameList.Add(s);

                                    var CharInfoPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"CharactersInfo");
                                    var subPath = Path.Combine(Path.GetFileName(Path.GetDirectoryName(CharInfoPath)), Path.GetFileName(CharInfoPath));

                                    System.Text.RegularExpressions.Regex regexReplace = new System.Text.RegularExpressions.Regex(@"(.+)");
                                    string mes = regexReplace.Replace("[" + s + "]", dicLanguage["SkippedBadNameX"]) + "\n" +
                                         regexReplace.Replace(subPath, dicLanguage["ProblemMayBeSolvedByEditingX"]);

                                    MessageBox.Show(mes, dicLanguage["Notice"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }

                            }
                        }
                        else if (groupLine == 1)
                        {
                            charEntry.CostumeSlot = byte.Parse(s);
                        }
                        else if (groupLine == 2)
                        {
                            charEntry.AddTexsCount = byte.Parse(s);
                        }
                        else if (groupLine == 3)
                        {
                            charEntry.Comment = s;
                        }
                        groupLine ++;
                    }
                    else if (group == "hairstyles" && !skipChar)
                    {
                        if (groupLine == 0)
                        {
                            sTemp = s;
                            groupLine = 1;
                        }
                        else if (groupLine == 1)
                        {
                            charEntry.HStyles.Add(new Hairstyle(byte.Parse(sTemp), byte.Parse(s)));
                            groupLine = 0;
                        }
                    }
                    else if (group == "files" && !skipChar)
                    {
                        for (int i = 0; i < MainForm.FileOrder.Length; i++)
                        {
                            if (s.EndsWith(MainForm.FileOrder[i], true, null))
                            {
                                string sCD = System.Environment.CurrentDirectory;
                                System.Environment.CurrentDirectory = System.IO.Path.GetDirectoryName(fileName);
                                charEntry.Files[i] = System.IO.Path.GetFullPath(s);
                                System.Environment.CurrentDirectory = sCD;
                                break;
                            }
                        }
                    }

                }

            }

            if (charEntry != null) // この if はほげほげで修正
            {
                dlcData.Chars.Add(charEntry);
            }
            dlcData.BcmVer = 9;

            return dlcData;
        }


        public static void OpenTranslation(string fileName)
        {

            using (StreamReader sr = new StreamReader(fileName))
            {
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^(\w+)=(.*)$");
                while (sr.Peek() >= 0)
                {
                    string s = sr.ReadLine();

                    if(regex.IsMatch(s))
                    {
                        string from = regex.Replace(s, "$1");
                        string to = regex.Replace(s, "$2");
                        dicLanguage[from] = to;
                    }
                }
            }
            
        }


        // コイツはエラーを投げる
        public static void SaveState(DLCData dlcData, string savePath)
        {
            // 確実に存在しない保存用フォルダパスの作成
            string saveTempPathBase = savePath + ".temp";
            string saveTempPath = saveTempPathBase;
            for (int i = 2; Directory.Exists(saveTempPath) || File.Exists(saveTempPath); i++)
            {
                saveTempPath = saveTempPathBase + i;
            }

            SaveState(dlcData, savePath, saveTempPath);
        }

        // コイツはエラーを投げる
        public static void SaveState(DLCData dlcData, string savePath, string saveTempPath)
        {
            try
            {
                SaveState_core(dlcData, saveTempPath);
            }
            catch (Exception e)
            {
                try
                {
                    if (File.Exists(saveTempPath))
                    {
                        File.Delete(saveTempPath);
                    }
                }
                catch { }

                throw e;
            }

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            File.Move(saveTempPath, savePath);
        }



        public static void SaveState_core(DLCData dlcData, string savePath)
        {
            string text = "";

            text += "path\r\n";
            if (dlcData.SavePath.EndsWith(".bcm"))
            {
                text += dlcData.SavePath.Substring(0, dlcData.SavePath.LastIndexOf("\\")) + "\r\n";
            }
            else
            {
                text += dlcData.SavePath + "\r\n";
            }

            for (int i = 0; i < dlcData.Chars.Count; i++)
            {
                text += "character\r\n";
                text += CharNames[dlcData.Chars[i].ID] + "\r\n";
                text += dlcData.Chars[i].CostumeSlot.ToString() + "\r\n";
                text += dlcData.Chars[i].AddTexsCount.ToString() + "\r\n";
                text += dlcData.Chars[i].Comment + "\r\n";

                text += "hairstyles\r\n";
                for (int j = 0; j < dlcData.Chars[i].HStyles.Count; j++)
                {
                    text += dlcData.Chars[i].HStyles[j].Hair.ToString() + "\r\n";
                    text += dlcData.Chars[i].HStyles[j].Face.ToString() + "\r\n";
                }

                text += "files\r\n";
                foreach (string fileName in dlcData.Chars[i].Files)
                {
                    if (fileName != null)
                    {
                        text += fileName + "\r\n";
                    }
                }
            }

            using (StreamWriter sw = new StreamWriter(savePath, false))
            {
                sw.Write(text);
                sw.Close();
            }
        }

        public static DLCData string2dlcData(string str)
        {
            // str を配列に変換
            string[] astr = str.Split('\n');
            int index = 0;

            // astr の先頭に改行があれば無視
            if(astr.Length > 0 && ( astr[0]=="\r" || astr[0] == ""))
            {
                index++;
            }
            var len = str.Length;
            var start = (str.Substring(0, 1) == "\n" ? 1 : 0);
            var suf = (str.Substring(len-1, 1) == "\n" ? "" : "\n");
            str = str.Substring(start) + suf;


            // あとはほとんど OpenState をそのままコピー

            var dlcData = new DLCData();
            string group = "";
            string sTemp = "";
            int groupLine = 0;
            int charInd = -1;
            bool skipChar = false;
            Character charEntry = null;

            var BadNameList = new List<string>();
            while (index < astr.Length)
            {
                string s = astr[index++];
                if (s.Length >= 1 && s.Substring(s.Length - 1) == "\r") 
                {
                    s = s.Substring(0, s.Length - 1);
                }

                if (s == "path")
                {
                    group = "path";
                }
                else if (s == "character")
                {
                    if (charInd != -1 && !skipChar)
                    {
                        dlcData.Chars.Add(charEntry);
                    }
                    group = "character";
                    groupLine = 0;
                    charInd = charInd + 1;
                    skipChar = false;
                }
                else if (s == "hairstyles" && !skipChar)
                {
                    group = "hairstyles";
                    groupLine = 0;
                }
                else if (s == "files" && !skipChar)
                {
                    if (charEntry.HStyles.Count == 0)
                    {
                        charEntry.HStyles.Add(new Hairstyle(1, 1));
                    }
                    else if (groupLine == 1 && group == "hairstyles")
                    {
                        charEntry.HStyles.Add(new Hairstyle(byte.Parse(sTemp), 1));
                    }
                    group = "files";
                }
                else if (group == "path")
                {
                    dlcData.SavePath = s;
                }
                else if (group == "character" && !skipChar)
                {
                    if (groupLine == 0)
                    {
                        foreach (byte key in CharNames.Keys)
                        {
                            if (CharNames[key] == s)
                            {
                                charEntry = new Character();
                                charEntry.ID = key;
                                charEntry.CostumeSlot = 0;
                                charEntry.AddTexsCount = 1;
                                skipChar = false;
                                if (FemaleIDs.Contains(key))
                                {
                                    charEntry.Female = true;
                                }
                                break;
                            }
                            skipChar = true;
                        }
                        if (skipChar && BadNameList.IndexOf(s) < 0)
                        {
                            /*
                            MessageBox.Show("[" + s + "]の名前に問題があるため読込はスキップされました", "注意", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            */
                            BadNameList.Add(s);

                            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"\$1");
                            string mes = regex.Replace(dicLanguage["SkippedBadNameX"], "[" + s + "]");

                            MessageBox.Show(mes, dicLanguage["Notice"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else if (groupLine == 1)
                    {
                        charEntry.CostumeSlot = byte.Parse(s);
                    }
                    else if (groupLine == 2)
                    {
                        charEntry.AddTexsCount = byte.Parse(s);
                    }
                    else if (groupLine == 3)
                    {
                        charEntry.Comment = s;
                    }
                    groupLine++;
                }
                else if (group == "hairstyles" && !skipChar)
                {
                    if (groupLine == 0)
                    {
                        sTemp = s;
                        groupLine = 1;
                    }
                    else if (groupLine == 1)
                    {
                        charEntry.HStyles.Add(new Hairstyle(byte.Parse(sTemp), byte.Parse(s)));
                        groupLine = 0;
                    }
                }
                else if (group == "files" && !skipChar)
                {
                    for (int i = 0; i < MainForm.FileOrder.Length; i++)
                    {
                        if (s.EndsWith(MainForm.FileOrder[i], true, null))
                        {
                            charEntry.Files[i] = s;
                            break;
                        }
                    }
                }

            }

            if (charEntry != null) // この if はほげほげで修正
            {
                dlcData.Chars.Add(charEntry);
            }
            dlcData.BcmVer = 9;

            return dlcData;
        }

        public static string dlcData2string(DLCData dlcData)
        {
            // ほぼ SaveState そのまま
            string text = "";

            text += "path\r\n";
            {

                if (dlcData.SavePath.EndsWith(".bcm"))
                {
                    text += dlcData.SavePath.Substring(0, dlcData.SavePath.LastIndexOf("\\")) + "\r\n";
                }
                else
                {
                    text += dlcData.SavePath + "\r\n";
                }
            }

            for (int i = 0; i < dlcData.Chars.Count; i++)
            {
                text += "character\r\n";
                text += CharNames[dlcData.Chars[i].ID] + "\r\n";
                text += dlcData.Chars[i].CostumeSlot.ToString() + "\r\n";
                text += dlcData.Chars[i].AddTexsCount.ToString() + "\r\n";
                text += dlcData.Chars[i].Comment + "\r\n";

                text += "hairstyles\r\n";
                for (int j = 0; j < dlcData.Chars[i].HStyles.Count; j++)
                {
                    text += dlcData.Chars[i].HStyles[j].Hair.ToString() + "\r\n";
                    text += dlcData.Chars[i].HStyles[j].Face.ToString() + "\r\n";
                }

                text += "files\r\n";
                foreach (string fileName in dlcData.Chars[i].Files)
                {
                    if (fileName != null)
                    {
                        text += fileName + "\r\n";
                    }
                }
            }

            /*
            using (StreamWriter sw = new StreamWriter(savePath, false))
            {
                sw.Write(text);
                sw.Close();
            }
            */
            return text;
        }

        /*  ここから DLC Tool 1.1 より */


        public class FileData
        {
            public FileData(string fileName, string name, uint flags, uint index)
            {
                this.FileName = fileName;
                this.Name = name;
                this.Flags = flags;
                this.Index = index;
            }

            public string FileName { get; set; }

            public string Name { get; set; }

            public uint Flags { get; set; }

            public uint Index { get; set; }

            public uint Size { get; set; }

            public uint DecompSize { get; set; }

            public uint LNKOffset { get; set; }

            public uint BINOffset { get; set; }
        }

        public static bool SaveDLC(DLCData dlcData, string savePath, string dlcName, bool compressDLC)
        {
            string saveTempPath = "";
            bool saveTempPathOK = false;
            try
            {
                // DLC がロックされている、書き込み権限がないなどの理由で書き出せない場合に積極的にエラーを出す
                // ここでチェックしなくても安全性には問題ないが出力失敗は早く分かったほうがユーザーに優しい
                if (!Directory.Exists(savePath))
                {
                    // 作って消す。作ったままだとエラーが起きたときに消すのがめんどくさい
                    //（元からあったのかプログラムが作ったのか判定しないといけないので）
                    Directory.CreateDirectory(savePath);
                    Directory.Delete(savePath);
                }
                else
                {
                    // 明確に把握できているもののみ移動

                    // bcm
                    string bcm = Path.Combine(savePath, dlcName + ".bcm");
                    if (File.Exists(bcm))
                    {
                        File.Move(bcm, bcm);
                    }
                    

                    // data フォルダ
                    string datafolder = Path.Combine(savePath, "data");
                    if (!Directory.Exists(datafolder))
                    {
                        // パス長の問題で savePath は作れても datafolder は作れないかもしれない
                        // って言い出すとすべてのファイルを一回作ってみないとダメか
                        // 別に安全性に関わる話じゃないのでそこまではやらなくていいでしょう
                        /*
                        Directory.CreateDirectory(datafolder);
                        Directory.Delete(datafolder);
                        */
                    }
                    else
                    {

                        // データファイル三つ
                        string[] exts = new string[] { ".lnk", ".bin", ".blp" };
                        for (int i = 0; i < exts.Length; i++)
                        {
                            string data = Path.Combine(datafolder, dlcName + exts[i]);

                            if (File.Exists(data))
                            {
                                File.Move(data, data);
                            }
                        }
                    }
                    
                }

                //MessageBox.Show("ファイルの書き込みはできそう");

                // 確実に存在しない保存用フォルダパスの作成
                string saveTempPathBase = savePath + "_temp";
                saveTempPath = saveTempPathBase;
                for (int i = 2; Directory.Exists(saveTempPath) || File.Exists(saveTempPath); i++)
                {
                    saveTempPath = saveTempPathBase + "_" + i;
                }

                saveTempPathOK = true;

                // 一旦そこに出力
                bool success = SaveDLC_core(dlcData, saveTempPath, dlcName, compressDLC);

                // 成功していれば、その中身を実際の保存フォルダへ移動
                if (success)
                {
                    if(!Directory.Exists(savePath))
                    {
                        Directory.Move(saveTempPath, savePath);
                    }
                    else
                    {
                        // 明確に把握できているもののみ移動

                        // bcm
                        string bcmTemp = Path.Combine(saveTempPath, dlcName + ".bcm");
                        string bcm = Path.Combine(savePath, dlcName + ".bcm");
                        if(File.Exists(bcm))
                        {
                            File.Delete(bcm);
                        }
                        File.Move(bcmTemp, bcm);

                        // data フォルダ
                        string datafolder = Path.Combine(savePath, "data");
                        string dataTempfolder = Path.Combine(saveTempPath, "data");
                        if (!Directory.Exists(datafolder))
                        {
                            Directory.CreateDirectory(datafolder);
                        }

                        // データファイル三つ
                        string[] exts = new string[] {".lnk", ".bin", ".blp"};
                        for(int i = 0; i < exts.Length; i++)
                        {
                            string data = Path.Combine(datafolder, dlcName + exts[i]);
                            string dataTemp = Path.Combine(dataTempfolder, dlcName + exts[i]);

                            if (File.Exists(data))
                            {
                                File.Delete(data);
                            }
                            File.Move(dataTemp, data);
                        }

                        // Temp Delete
                        Directory.Delete(dataTempfolder);
                        Directory.Delete(saveTempPath);
                    }



                }
                else
                {
                    // saveTempPath を削除。savePath に手を出してはいけない。
                    Directory.Delete(saveTempPath, true);
                }


                return success;
            }
            catch (Exception e)
            {
                // ファイルの移動時にエラーが起こるとここに移動
                if(saveTempPathOK)
                {
                    try
                    {
                        // saveTempPath を削除。savePath に手を出してはいけない。
                        Directory.Delete(saveTempPath, true);
                    }
                    catch { }
                }

                MessageBox.Show(e.Message, dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }
        }

        public static bool SaveDLC_core(DLCData dlcData, string savePath, string dlcName, bool compressDLC)
        {
            try
            {
                Directory.CreateDirectory(savePath + @"\data\");
                SaveBCM(dlcData, savePath + @"\" + dlcName + ".bcm", dlcName);
                List<FileData> fileData = CollectFileData(dlcData, compressDLC);
                SaveBIN_DLC_Tool_1_1(fileData, savePath + @"\data\" + dlcName + ".bin", dlcName);
                mainForm.SetProgressBar(true, fileData.Count); // ●●●●プログレスバー
                SaveLNK(fileData, savePath + @"\data\" + dlcName + ".lnk");
                mainForm.SetProgressBar(false); // ●●●●プログレスバー
                SaveBLP(fileData, savePath + @"\data\" + dlcName + ".blp");
                //MessageBox.Show("DLC saved.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception e)
            {
                // 確かにこの処理は危険
                // 【PC】デッドオアアライブ5LR MOD晒しスレ24 >>462 での指摘
                /*
                try
                {
                    Directory.Delete(savePath, true);
                }
                catch (Exception)
                {
                }
                */

                if (e is OverflowException)
                {
                    MessageBox.Show(dicLanguage["DecreaseIndex"], dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                }
                else
                {
                    MessageBox.Show(e.Message, dicLanguage["Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;

        }


        private static List<FileData> CollectFileData(DLCData dlcData, bool compressDLC)
        {
            var fileData = new List<FileData>();
            if (nameDB == null)
            {
                LoadDB();
            }

            for (int i = 0; i < dlcData.Chars.Count; i++)
            {
                string decName = CharNames[dlcData.Chars[i].ID] + "_DLCU_" + (dlcData.Chars[i].CostumeSlot + 1).ToString("D3");
                int nameLimit = 3;
                if (dlcData.Chars[i].AddTexsCount > 1)
                {
                    nameLimit += dlcData.Chars[i].AddTexsCount * 2;
                }

                for (int j = 0; j < 12; j++)
                {
                    if ((j < nameLimit) || (j == 11))
                    {
                        string fileName;
                        if (dlcData.Chars[i].Files[j] != null)
                        {
                            fileName = dlcData.Chars[i].Files[j];
                        }
                        else
                        {
                            if (j == 2)
                            {
                                fileName = "cStub";
                            }
                            else if (j == 11)
                            {
                                fileName = "pStub";
                            }
                            else
                            {
                                throw new Exception("Path for " + CharNames[dlcData.Chars[i].ID] + "[" + dlcData.Chars[i].CostumeSlot + "]'s \"" + CharacterFiles[j] + "\" file not found.");
                            }
                        }

                        NameEntry nameEntry;
                        if (nameDB.TryGetValue(decName + CharacterFiles[j], out nameEntry))
                        {
                            if (compressDLC && (nameEntry.Flags[0] != '0'))
                            {
                                fileData.Add(new FileData(fileName, nameEntry.Name, uint.Parse("4" + nameEntry.Flags.Substring(1, 7), NumberStyles.HexNumber),
                                    uint.Parse(nameEntry.Index, NumberStyles.HexNumber)));
                            }
                            else
                            {
                                fileData.Add(new FileData(fileName, nameEntry.Name, 0, uint.Parse(nameEntry.Index, NumberStyles.HexNumber)));
                            }
                        }
                        else
                        {
                            throw new Exception("Filename for " + CharNames[dlcData.Chars[i].ID] + "[" + dlcData.Chars[i].CostumeSlot + "]'s \"" + CharacterFiles[j] +
                                "\" file not found in the database. Possibly character's costume slot limit is reached.");
                        }
                    }
                }
            }

            return fileData;
        }

        private static void SaveBIN_DLC_Tool_1_1(List<FileData> fileData, string fileName, string dlcName)
        {
            int headerSize = 40 + (fileData.Count * 12);
            using (var ms = new MemoryStream())
            {
                var bw = new BinaryWriter(ms, Encoding.ASCII);
                bw.BaseStream.Position = headerSize;
                bw.Write(("_output/costume_pack_" + dlcName + char.MinValue + "order" + char.MinValue).ToCharArray());
                for (int i = 0; i < fileData.Count; i++)
                {
                    fileData[i].BINOffset = (uint)bw.BaseStream.Position;
                    bw.Write((@"/" + fileData[i].Name + char.MinValue).ToCharArray());
                }

                bw.BaseStream.Position = 0;
                bw.Write(0x4F4D464C);
                bw.Write(1);
                bw.Write(fileData.Count);
                bw.Write(0x20);
                bw.Write(0x28);
                bw.Write(headerSize);
                bw.Write(fileData[0].BINOffset);
                bw.Write(0);
                bw.Write(0);
                bw.Write(headerSize);
                for (int i = 0; i < fileData.Count; i++)
                {
                    bw.Write(0);
                    bw.Write(i);
                    bw.Write(fileData[i].BINOffset);
                }

                bw.BaseStream.Position = 0;
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    bw.BaseStream.CopyTo(fs);
                }
            }
        }


        private static void SaveLNK(List<FileData> fileData, string fileName)
        {
            int headerSize = 32 + (fileData.Count * 32);
            if (headerSize % 2048 > 0)
            {
                headerSize = headerSize - (headerSize % 2048) + 2048;
            }

            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                fs.Position = headerSize;
                for (int i = 0; i < fileData.Count; i++)
                {
                    fileData[i].LNKOffset = (uint)fs.Position;
                    if (fileData[i].FileName == "cStub")
                    {
                        using (var ms = new MemoryStream(DLC_Tool.Properties.Resources.cStub))
                        {
                            fileData[i].Size = (uint)ms.Length;
                            ms.CopyTo(fs);
                        }
                    }
                    else if (fileData[i].FileName == "pStub")
                    {
                        using (var ms = new MemoryStream(DLC_Tool.Properties.Resources.pStub))
                        {
                            fileData[i].Size = (uint)ms.Length;
                            ms.CopyTo(fs);
                        }
                    }
                    else
                    {
                        using (var fs2 = new FileStream(fileData[i].FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            if (fileData[i].Flags > 0)
                            {
                                fileData[i].DecompSize = (uint)fs2.Length;
                                WriteCompressedFile(fs, fs2);
                                fileData[i].Size = (uint)fs.Length - fileData[i].LNKOffset;
                            }
                            else
                            {
                                fileData[i].Size = (uint)fs2.Length;
                                fs2.CopyTo(fs);
                            }
                        }
                    }

                    mainForm.IncrementProgressBar(); // ●●●●
                    if (fs.Position % 2048 > 0)
                    {
                        fs.Position = fs.Position - (fs.Position % 2048) + 2048;
                    }
                }

                var bw = new BinaryWriter(fs);
                if (fs.Length % 2048 > 0)
                {
                    bw.BaseStream.Position -= 1;
                    bw.Write((byte)0);
                }

                bw.BaseStream.Position = 0;
                bw.Write(0x4D444350);
                bw.Write(0);
                bw.Write(fileData.Count);
                bw.Write(0);
                bw.Write((uint)bw.BaseStream.Length);
                bw.Write(0);
                bw.Write(2048);
                bw.Write(0);
                for (int i = 0; i < fileData.Count; i++)
                {
                    bw.Write(fileData[i].LNKOffset);
                    bw.Write(0);
                    bw.Write(fileData[i].Size);
                    bw.Write(0);
                    bw.Write(fileData[i].Size);
                    bw.Write(0);
                    bw.Write(0);
                    bw.Write(0);
                }
            }
        }

        private static void SaveBLP(List<FileData> fileData, string fileName)
        {
            using (var ms = new MemoryStream())
            {
                ms.Position = 16;
                var bw = new BinaryWriter(ms);
                for (int i = 0; i < fileData.Count; i++)
                {
                    bw.Write(fileData[i].Index);
                    bw.Write(fileData[i].Size);
                    bw.Write(fileData[i].Flags);
                    bw.Write(fileData[i].DecompSize);
                    bw.Write(0);
                }

                bw.BaseStream.Position = 0;
                bw.Write(0x46495031);
                bw.Write(fileData.Count);
                bw.Write(16);
                bw.Write((int)bw.BaseStream.Length);
                bw.BaseStream.Position = 0;
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    bw.BaseStream.CopyTo(fs);
                }
            }
        }

        private static void LoadDB()
        {
            //using (var sr = new StreamReader(Path.GetDirectoryName(Application.ExecutablePath) + @"\file5lr.dat", Encoding.ASCII))
            using (var sr = new StreamReader(MainForm.DAT, Encoding.ASCII))
            {
                nameDB = new Dictionary<string, NameEntry>();
                while (!sr.EndOfStream)
                {
                    string[] dbEntry = sr.ReadLine().Split('\t');
                    if (dbEntry[0] == "debug_patch0.bin")
                    {
                        break;
                    }
                    else if (!nameDB.ContainsKey(dbEntry[1]))
                    {
                        if ((dbEntry.Length > 3) && (dbEntry[1] != string.Empty))
                        {
                            nameDB.Add(dbEntry[1], new NameEntry(dbEntry[0], dbEntry[2], dbEntry[3].Split(',')[0]));
                        }
                    }
                }
            }
        }


        private static void WriteCompressedFile(FileStream fs, FileStream fs2)
        {
            var taskList = new List<Task<byte[]>>();
            while (true)
            {
                var inputBuffer = new byte[0x4000];
                int readCount = fs2.Read(inputBuffer, 0, 0x4000);
                if (readCount > 0)
                {
                    taskList.Add(Task<byte[]>.Factory.StartNew(() => CompressChunk(inputBuffer, readCount)));
                }
                else
                {
                    break;
                }
            }

            fs.Write(BitConverter.GetBytes((uint)fs2.Length), 0, 4);
            foreach (var taskResult in taskList)
            {
                if (taskResult.Status != TaskStatus.RanToCompletion)
                {
                    taskResult.Wait();
                }

                fs.Write(BitConverter.GetBytes(taskResult.Result.Length + 0x8000), 0, 4);
                fs.Write(taskResult.Result, 0, taskResult.Result.Length);
                if ((fs.Position - 4) % 16 > 0)
                {
                    fs.Position = fs.Position - ((fs.Position - 4) % 16) + 16;
                }
            }

            if ((fs.Length - 4) % 16 > 0)
            {
                fs.Position -= 1;
                fs.WriteByte((byte)0);
            }
        }


        private static byte[] CompressChunk(byte[] chunkBuffer, int chunkSize)
        {
            using (var ms = new MemoryStream())
            {
                var zs = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(ms);
                zs.IsStreamOwner = false;
                zs.Write(chunkBuffer, 0, chunkSize);
                zs.Close();
                return ms.ToArray();
            }
        }

        private static AppDomain currentDomain;
        private static MainForm mainForm;
        private static Dictionary<string, NameEntry> nameDB;

        private static readonly string[] CharacterFiles = new string[] { ".TMC", ".TMCL", ".---C", "_001.--H", "_001.--HL", "_002.--H", "_002.--HL", "_003.--H", "_003.--HL",
            "_004.--H", "_004.--HL", ".--P"};

        private static Assembly ResolveEventHandler(Object sender, ResolveEventArgs args)
        {
            string dllName = new AssemblyName(args.Name).Name + ".dll";
            if (dllName != "ICSharpCode.SharpZipLib.dll")
            {
                return null;
            }
            else
            {
                currentDomain.AssemblyResolve -= ResolveEventHandler;
                return Assembly.Load(DLC_Tool.Properties.Resources.ICSharpCode_SharpZipLib);
            }
        }

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += ResolveEventHandler;
            mainForm = new MainForm();
            Application.Run(mainForm);
        }


        /*  ここまで DLC Tool 1.1 より */




        /*
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        */
    }
}

//using System.Data.OleDb;

namespace Archive_Tool
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    internal static class Program
    {
        private static readonly byte[] FirstName = new byte[8] { 0x32, 0x52, 0x42, 0x36, 0x33, 0x40, 0x31, 0x53 };
        private static readonly byte[] FirstAsset = new byte[12] { 0x19, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x1A, 0x00, 0x00, 0x00 };
        private static readonly byte[] FirstData = new byte[8] { 0x14, 0x3D, 0x08, 0x00, 0x01, 0x00, 0x00, 0xE0 };
        private static AppDomain currentDomain;
        //private static MainForm mainForm;
        private static Dictionary<string, List<NameEntry>> nameDB;
        private static Dictionary<string, SwapEntry> swapDB;
        private static Dictionary<string, List<string>> indexDB;
        private static string lnkPath, blpPath, swapPath, lrPath;

        private static string DAT;

        public static void testEcho(string str)
        {
            MessageBox.Show(str);
        }

        public static ArchiveFile[] ParseArchive(string fileName, string DatPath)
        {
            DAT = DatPath;
            try
            {
                if (nameDB == null)
                {
                    LoadDB(true);
                }

                //mainForm.AddToLog("Info: Opening archive...");
                lnkPath = Path.ChangeExtension(fileName, "lnk");
                string[] nameEntries = ParseBIN(fileName);
                uint[,] lnkEntries = ParseLNK(lnkPath);
                blpPath = Path.ChangeExtension(fileName, "blp");
                int[] blpIndexes = null;
                uint[,] blpEntries = null;
                if (File.Exists(blpPath))
                {
                    try
                    {
                        blpEntries = ParseBLP(blpPath);
                        if (blpEntries != null)
                        {
                            blpIndexes = MatchIndexes(nameEntries, lnkEntries, blpEntries);
                        }
                    }
                    catch (Exception e)
                    {
                        blpPath = null;
                        //mainForm.AddToLog("Warning: Parsing of \"" + Path.GetFileName(blpPath) + "\" resulted in an error. " + e.Message, Color.DodgerBlue);
                    }
                }
                else
                {
                    blpPath = null;
                }

                string fileDir = Path.GetDirectoryName(fileName);
                if (fileDir != Path.GetDirectoryName(swapPath))
                {
                    swapPath = fileDir + @"\mod_at.config";
                    lrPath = fileDir + @"\mod.config";
                    if (!File.Exists(lrPath))
                    {
                        lrPath = null;
                    }

                    try
                    {
                        if (File.Exists(swapPath))
                        {
                            LoadSwapDB();
                        }
                        else
                        {
                            if (lrPath != null)
                            {
                                CreateSwapDB();
                                LoadSwapDB();
                            }
                            else
                            {
                                swapDB = null;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        swapDB = null;
                        //mainForm.AddToLog("Warning: Parsing of \"mod_at.config\" resulted in an error. File will be overwritten.", Color.DodgerBlue);
                    }
                }

                var archData = new ArchiveFile[nameEntries.Length];
                int unusedCount = 0;
                int unknownCount = 0;
                for (int i = 0; i < nameEntries.Length; i++)
                {
                    var archFile = new ArchiveFile();
                    archFile.EncryptedName = nameEntries[i];
                    List<NameEntry> nameList;
                    if (nameDB.TryGetValue(nameEntries[i], out nameList))
                    {
                        archFile.Flags = nameList[0].Flags;
                        if (nameList[0].Name != string.Empty)
                        {
                            archFile.Name = nameList[0].Name;
                        }
                        else
                        {
                            archFile.Name = nameEntries[i];
                        }

                        if (nameList[0].Indexes != null)
                        {
                            archFile.Index = nameList[0].Indexes[0];
                        }

                        if (nameList[0].Unused)
                        {
                            archFile.Unused = true;
                            unusedCount++;
                        }
                    }
                    else
                    {
                        archFile.Name = nameEntries[i];
                        archFile.Unknown = true;
                        unknownCount++;
                    }

                    if ((blpIndexes != null) && (blpIndexes[i] != -1))
                    {
                        archFile.BLPIndex = blpIndexes[i];
                        archFile.Index = blpEntries[blpIndexes[i], 0].ToString("X");
                        archFile.Flags = blpEntries[blpIndexes[i], 2].ToString("X8");
                        archFile.DecompSize = blpEntries[blpIndexes[i], 3].ToString("X");
                    }
                    else
                    {
                        archFile.BLPIndex = -1;
                        if (archFile.Flags == null)
                        {
                            archFile.Flags = "00000000";
                        }
                    }

                    SwapEntry swapEntry;
                    if ((swapDB != null) && swapDB.TryGetValue(archFile.EncryptedName, out swapEntry))
                    {
                        archFile.SwapName = swapEntry.SwapName;
                    }

                    archFile.LNKIndex = i;
                    archFile.LNKOffset = lnkEntries[i, 0];
                    archFile.Size = lnkEntries[i, 1];
                    archData[i] = archFile;
                }

                //mainForm.AddToLog("Info: Archive \"" + Path.GetFileNameWithoutExtension(fileName) + "\" is successfully opened.", Color.Green);
                if (unusedCount > 0)
                {
                    if (unknownCount > 0)
                    {
                        //mainForm.AddToLog("Info: Found " + unusedCount.ToString() + " unused and " + unknownCount.ToString() + " unknown entries.", Color.DodgerBlue);
                    }
                    else
                    {
                        //mainForm.AddToLog("Info: Found " + unusedCount.ToString() + " unused entries.", Color.DodgerBlue);
                    }
                }
                else if (unknownCount > 0)
                {
                    //mainForm.AddToLog("Info: Found " + unknownCount.ToString() + " unknown entries.", Color.DodgerBlue);
                }

                return archData;
            }
            catch (Exception e)
            {
                //mainForm.AddToLog("Error: " + e.Message, Color.Red);
                return null;
            }
        }

        public static void UpdateDB(string fileName)
        {
            try
            {
                //mainForm.AddToLog("Info: Updating DB...");
                string[,] gameDB = DumpGameDB(fileName);
                if (nameDB == null)
                {
                    LoadDB(false);
                }

                bool dbUpdated = false;
                var newDB = new List<string>(nameDB.Count);
                for (int i = 0; i < gameDB.GetLength(0); i++)
                {
                    List<NameEntry> nameList;
                    if (nameDB.TryGetValue(gameDB[i, 0], out nameList))
                    {
                        if (nameList[0].Indexes != null)
                        {
                            string currentIndex = i.ToString("X");
                            int indIndex = nameList[0].Indexes.IndexOf(currentIndex);
                            if (indIndex != -1)
                            {
                                if (indIndex == 0)
                                {
                                    newDB.Add(gameDB[i, 0] + "\t" + nameList[0].Name + "\t" + gameDB[i, 1] + "\t" + string.Join(",", nameList[0].Indexes));
                                    if (nameList[0].Unused || (gameDB[i, 1] != nameList[0].Flags))
                                    {
                                        dbUpdated = true;
                                    }
                                }
                                else
                                {
                                    nameList[0].Indexes.RemoveAt(indIndex);
                                    nameList[0].Indexes.Insert(0, currentIndex);
                                    newDB.Add(gameDB[i, 0] + "\t" + nameList[0].Name + "\t" + gameDB[i, 1] + "\t" + string.Join(",", nameList[0].Indexes));
                                    dbUpdated = true;
                                }
                            }
                            else
                            {
                                nameList[0].Indexes.Insert(0, currentIndex);
                                newDB.Add(gameDB[i, 0] + "\t" + nameList[0].Name + "\t" + gameDB[i, 1] + "\t" + string.Join(",", nameList[0].Indexes));
                                dbUpdated = true;
                            }
                        }
                        else
                        {
                            newDB.Add(gameDB[i, 0] + "\t" + nameList[0].Name + "\t" + gameDB[i, 1] + "\t" + i.ToString("X"));
                            dbUpdated = true;
                        }

                        if (nameList.Count == 1)
                        {
                            nameDB.Remove(gameDB[i, 0]);
                        }
                        else
                        {
                            nameList.RemoveAt(0);
                        }
                    }
                    else
                    {
                        newDB.Add(gameDB[i, 0] + "\t" + "\t" + gameDB[i, 1] + "\t" + i.ToString("X"));
                        dbUpdated = true;
                    }
                }

                if (dbUpdated)
                {
                    newDB.Add("end_flag" + "\t" + "\t" + "FFFFFFFF");
                    if (nameDB.Count > 0)
                    {
                        foreach (var nameList in nameDB.Values)
                        {
                            foreach (var nameEntry in nameList)
                            {
                                if (nameEntry.Indexes != null)
                                {
                                    newDB.Add(nameEntry.EncryptedName + "\t" + nameEntry.Name + "\t" + nameEntry.Flags + "\t" + string.Join(",", nameEntry.Indexes));
                                }
                                else
                                {
                                    newDB.Add(nameEntry.EncryptedName + "\t" + nameEntry.Name + "\t" + nameEntry.Flags);
                                }
                            }
                        }
                    }

                    File.WriteAllLines(DAT, newDB, Encoding.ASCII);
                    //mainForm.AddToLog("Info: DB updated.", Color.Green);
                }
                else
                {
                    //mainForm.AddToLog("Info: New information not found.");
                }
            }
            catch (Exception e)
            {
                //mainForm.AddToLog("Error: " + e.Message, Color.Red);
            }
            finally
            {
                nameDB = null;
            }
        }

        public static Tuple<string, Color> ExtractFile(ArchiveFile archFile, string savePath, bool[] userFlags)
        {
            try
            {
                Tuple<string, Color> extractResult = null;
                using (var fs = new FileStream(lnkPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.Position = archFile.LNKOffset;
                    using (var fs2 = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        if ((archFile.Flags[0] != '0') || userFlags[0] || userFlags[2])
                        {
                            var sizeBytes = new byte[4];
                            fs.Read(sizeBytes, 0, 4);
                            uint decompSize = BitConverter.ToUInt32(sizeBytes, 0);
                            fs.Position = archFile.LNKOffset;
                            if (((archFile.Flags[0] == 'E') || (archFile.Flags[0] == 'C') || userFlags[0]) && (!userFlags[1]))
                            {
                                var inputBuffer = new byte[archFile.Size];
                                fs.Read(inputBuffer, 0, (int)archFile.Size);
                                DecryptFile(inputBuffer, decompSize);
                                using (var ms = new MemoryStream(inputBuffer))
                                {
                                    if (!userFlags[3])
                                    {
                                        WriteDecompressedFile(ms, fs2, archFile.Size, decompSize);
                                        if (fs2.Length != decompSize)
                                        {
                                            extractResult = new Tuple<string, Color>("Warning: File \"" + archFile.Name + "\" has a wrong size. ", Color.DodgerBlue);
                                        }
                                    }
                                    else
                                    {
                                        WriteFile(ms, fs2, archFile.Size);
                                    }
                                }
                            }
                            else
                            {
                                if (!userFlags[3])
                                {
                                    WriteDecompressedFile(fs, fs2, archFile.Size, decompSize);
                                    if (fs2.Length != decompSize)
                                    {
                                        extractResult = new Tuple<string, Color>("Warning: File \"" + archFile.Name + "\" has a wrong size. ", Color.DodgerBlue);
                                    }
                                }
                                else
                                {
                                    WriteFile(fs, fs2, archFile.Size);
                                }
                            }
                        }
                        else
                        {
                            WriteFile(fs, fs2, archFile.Size);
                        }
                    }
                }

                return extractResult;
            }
            catch (Exception e)
            {
                return new Tuple<string, Color>("Error: File \"" + archFile.Name + "\" can't be extracted. " + e.Message, Color.Red);
            }
        }

        public static bool SwapFile(ArchiveFile archFile, string swapDir, string fileName)
        {
            try
            {
                string sourceName = Path.GetFileName(fileName);
                if (blpPath != null)
                {
                    if (archFile.BLPIndex == -1)
                    {
                        throw new Exception("BLP index for this file is unknown.");
                    }

                    uint fileOffset;
                    uint decompSize;
                    uint compSize = 0;
                    using (var fs = new FileStream(lnkPath, FileMode.Open, FileAccess.Write, FileShare.Read))
                    {
                        fs.Position = fs.Length;
                        fileOffset = (uint)fs.Position;
                        using (var fs2 = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            decompSize = (uint)fs2.Length;
                            if (archFile.Flags[0] != '0')
                            {
                                WriteCompressedFile(fs, fs2);
                                compSize = (uint)fs.Length - fileOffset;
                            }
                            else
                            {
                                fs2.CopyTo(fs);
                            }
                        }

                        if (fs.Position % 2048 > 0)
                        {
                            fs.Position = fs.Position - (fs.Position % 2048) + 2048;
                        }

                        var bw = new BinaryWriter(fs);
                        if (fs.Length % 2048 > 0)
                        {
                            bw.BaseStream.Position -= 1;
                            bw.Write((byte)0);
                        }

                        bw.BaseStream.Position = 0x10;
                        bw.Write((uint)bw.BaseStream.Length);
                        bw.BaseStream.Position = 0x20 + (archFile.LNKIndex * 0x20);
                        bw.Write(fileOffset);
                        bw.Write(0);
                        if (archFile.Flags[0] != '0')
                        {
                            bw.Write(compSize);
                            bw.Write(0);
                            bw.Write(compSize);
                        }
                        else
                        {
                            bw.Write(decompSize);
                            bw.Write(0);
                            bw.Write(decompSize);
                        }
                    }

                    using (var fs = new FileStream(blpPath, FileMode.Open, FileAccess.Write, FileShare.Read))
                    {
                        var bw = new BinaryWriter(fs);
                        bw.BaseStream.Position = 0x10 + (archFile.BLPIndex * 0x14) + 4;
                        if (archFile.Flags[0] != '0')
                        {
                            bw.Write(compSize);
                            if (archFile.Flags[0] != '4')
                            {
                                bw.BaseStream.Position += 3;
                                bw.Write((byte)0x40);
                            }
                            else
                            {
                                bw.BaseStream.Position += 4;
                            }

                            bw.Write(decompSize);
                        }
                        else
                        {
                            bw.Write(decompSize);
                            bw.Write(0);
                            bw.Write(0);
                        }
                    }

                    archFile.SwapName = sourceName;
                    if (swapDB == null)
                    {
                        CreateSwapDB();
                    }

                    ChangeSwapDB(new SwapEntry(archFile.EncryptedName, archFile.Index, archFile.Size.ToString("X"), archFile.Flags, archFile.DecompSize, archFile.LNKOffset.ToString("X"),
                        archFile.SwapName), false);

                    archFile.LNKOffset = fileOffset;
                    if (archFile.Flags[0] != '0')
                    {
                        archFile.Size = compSize;
                        archFile.Flags = "4" + archFile.Flags.Substring(1, 7);
                    }
                    else
                    {
                        archFile.Size = decompSize;
                    }
                }
                else
                {
                    uint fileSize;
                    string fileFlags;
                    string swapDirPath = Path.GetDirectoryName(lnkPath) + @"\" + swapDir;
                    Directory.CreateDirectory(swapDirPath);
                    if (File.Exists(swapDirPath + sourceName))
                    {
                        sourceName += "_";
                        for (int i = 2; i < 10000; i++)
                        {
                            if (!File.Exists(swapDirPath + sourceName + i.ToString()))
                            {
                                sourceName += i.ToString();
                                break;
                            }
                        }
                    }

                    if (archFile.Flags[0] != '0')
                    {
                        using (var fs = new FileStream(swapDirPath + sourceName, FileMode.Create, FileAccess.Write, FileShare.Read))
                        {
                            using (var fs2 = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                WriteCompressedFile(fs, fs2);
                                fileSize = (uint)fs2.Length;
                                fileFlags = "4" + archFile.Flags.Substring(1, 7);
                            }
                        }
                    }
                    else
                    {
                        File.Copy(fileName, swapDirPath + sourceName, true);
                        fileSize = (uint)new System.IO.FileInfo(fileName).Length;
                        fileFlags = archFile.Flags;
                    }

                    if (archFile.SwapName != null)
                    {
                        File.Delete(Path.GetDirectoryName(lnkPath) + @"\" + archFile.SwapName);
                    }

                    archFile.SwapName = swapDir + sourceName;
                    if (swapDB == null)
                    {
                        CreateSwapDB();
                    }

                    ChangeSwapDB(new SwapEntry(archFile.EncryptedName, archFile.Index, "0", fileFlags, fileSize.ToString("X"), "0", archFile.SwapName), false);
                }

                //mainForm.AddToLog("Info: File \"" + archFile.Name + "\" is swapped with \"" + sourceName + "\".", Color.Green);
                return true;
            }
            catch (Exception e)
            {
                //mainForm.AddToLog("Error: " + e.Message, Color.Red);
                return false;
            }
        }

        public static bool ResetSwap(ArchiveFile archFile)
        {
            try
            {
                SwapEntry swapEntry = swapDB[archFile.EncryptedName];
                if (blpPath != null)
                {
                    archFile.Size = uint.Parse(swapEntry.Size, NumberStyles.HexNumber);
                    archFile.Flags = swapEntry.Flags;
                    archFile.DecompSize = swapEntry.DecompSize;
                    archFile.LNKOffset = uint.Parse(swapEntry.LNKOffset, NumberStyles.HexNumber);
                    using (var fs = new FileStream(lnkPath, FileMode.Open, FileAccess.Write, FileShare.Read))
                    {
                        var bw = new BinaryWriter(fs);
                        bw.BaseStream.Position = 0x20 + (archFile.LNKIndex * 0x20);
                        bw.Write(archFile.LNKOffset);
                        bw.Write(0);
                        bw.Write(archFile.Size);
                        bw.Write(0);
                        bw.Write(archFile.Size);
                    }

                    using (var fs = new FileStream(blpPath, FileMode.Open, FileAccess.Write, FileShare.Read))
                    {
                        var bw = new BinaryWriter(fs);
                        bw.BaseStream.Position = 0x10 + (archFile.BLPIndex * 0x14) + 4;
                        bw.Write(archFile.Size);
                        bw.Write(uint.Parse(archFile.Flags, NumberStyles.HexNumber));
                        bw.Write(uint.Parse(archFile.DecompSize, NumberStyles.HexNumber));
                    }
                }
                else
                {
                    File.Delete(Path.GetDirectoryName(lnkPath) + @"\" + archFile.SwapName);
                    archFile.DecompSize = "0";
                }

                archFile.SwapName = null;
                ChangeSwapDB(swapEntry, true);
                //mainForm.AddToLog("Info: File \"" + archFile.Name + "\" has been reset to the default value.", Color.Green);
                return true;
            }
            catch (Exception e)
            {
                //mainForm.AddToLog("Error: " + e.Message, Color.Red);
                return false;
            }
        }

        private static void LoadDB(bool showInfo)
        {
            if (showInfo)
            {
                //mainForm.AddToLog("Info: Loading DB...");
            }

            using (var sr = new StreamReader(DAT, Encoding.ASCII))
            {
                nameDB = new Dictionary<string, List<NameEntry>>();
                indexDB = new Dictionary<string, List<string>>();
                bool endFlag = false;
                while (!sr.EndOfStream)
                {
                    string[] dbEntry = sr.ReadLine().Split('\t');
                    if (dbEntry[0] == "end_flag")
                    {
                        endFlag = true;
                        continue;
                    }

                    NameEntry nameEntry;
                    if (dbEntry.Length > 3)
                    {
                        var entryIndexes = new List<string>(dbEntry[3].Split(','));
                        nameEntry = new NameEntry(dbEntry[0], dbEntry[1], dbEntry[2], entryIndexes, endFlag);
                        foreach (var entryIndex in entryIndexes)
                        {
                            List<string> indexEntry;
                            if (!indexDB.TryGetValue(entryIndex, out indexEntry))
                            {
                                indexEntry = new List<string>();
                                indexEntry.Add(dbEntry[0]);
                                indexDB.Add(entryIndex, indexEntry);
                            }
                            else
                            {
                                if (indexEntry.IndexOf(dbEntry[0]) == -1)
                                {
                                    indexEntry.Add(dbEntry[0]);
                                }
                            }
                        }
                    }
                    else if (dbEntry.Length > 2)
                    {
                        nameEntry = new NameEntry(dbEntry[0], dbEntry[1], dbEntry[2], endFlag);
                    }
                    else
                    {
                        nameEntry = new NameEntry(dbEntry[0], dbEntry[1], "00000000", endFlag);
                    }

                    List<NameEntry> nameList;
                    if (!nameDB.TryGetValue(dbEntry[0], out nameList))
                    {
                        nameList = new List<NameEntry>();
                        nameList.Add(nameEntry);
                        nameDB.Add(dbEntry[0], nameList);
                    }
                    else
                    {
                        nameList.Add(nameEntry);
                    }
                }
            }
        }

        private static void LoadSwapDB()
        {
            bool indexMismatch = false;
            using (var sr = new StreamReader(swapPath, Encoding.ASCII))
            {
                swapDB = new Dictionary<string, SwapEntry>();
                if (blpPath != null)
                {
                    while (!sr.EndOfStream)
                    {
                        string[] dbEntry = sr.ReadLine().Split('\t');
                        var swapEntry = new SwapEntry(dbEntry[0], dbEntry[1], dbEntry[2], dbEntry[3], dbEntry[4], dbEntry[5], dbEntry[6]);
                        swapDB.Add(swapEntry.EncryptedName, swapEntry);
                    }
                }
                else
                {
                    while (!sr.EndOfStream)
                    {
                        string[] dbEntry = sr.ReadLine().Split('\t');
                        if (dbEntry[1] != nameDB[dbEntry[0]][0].Indexes[0])
                        {
                            if (!indexMismatch)
                            {
                                indexMismatch = true;
                            }
                        }

                        var swapEntry = new SwapEntry(dbEntry[0], dbEntry[1], dbEntry[2], dbEntry[3], dbEntry[4], dbEntry[5], dbEntry[6]);
                        swapDB.Add(swapEntry.EncryptedName, swapEntry);
                    }
                }
            }

            if (indexMismatch)
            {
                if (!File.Exists(Path.GetDirectoryName(lnkPath) + @"\dis_chk"))
                {
                    var dialogResult = MessageBox.Show("Swaps config file doesn't match the current DB version. Press \"Yes\" to update the config file. Press \"Cancel\" to disable this warning.",
                        "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (dialogResult == DialogResult.Yes)
                    {
                        foreach (var swapEntry in swapDB)
                        {
                            swapEntry.Value.Index = nameDB[swapEntry.Value.EncryptedName][0].Indexes[0];
                        }

                        RebuildSwapDB();
                    }
                    else if (dialogResult == DialogResult.Cancel)
                    {
                        File.Create(Path.GetDirectoryName(lnkPath) + @"\dis_chk").Dispose();
                    }
                }
            }
        }

        private static void CreateSwapDB()
        {
            using (var sw = new StreamWriter(Path.GetDirectoryName(lnkPath) + @"\mod_at.config", false, Encoding.ASCII))
            {
                swapDB = new Dictionary<string, SwapEntry>();
                if (lrPath != null)
                {
                    using (var sr = new StreamReader(lrPath, Encoding.ASCII))
                    {
                        while (!sr.EndOfStream)
                        {
                            string[] dbEntry = sr.ReadLine().Replace("0x", string.Empty).Split(':');
                            var swapEntry = new SwapEntry();
                            swapEntry.Index = dbEntry[0].ToUpperInvariant().TrimStart('0');
                            if (swapEntry.Index == string.Empty)
                            {
                                swapEntry.Index = "0";
                            }

                            swapEntry.EncryptedName = indexDB[swapEntry.Index][0];
                            swapEntry.Flags = dbEntry[1].ToUpperInvariant();
                            swapEntry.DecompSize = dbEntry[2].ToUpperInvariant();
                            swapEntry.SwapName = dbEntry[3].Replace('/', '\\');
                            swapEntry.Size = "0";
                            swapEntry.LNKOffset = "0";
                            swapDB.Add(swapEntry.EncryptedName, swapEntry);
                            sw.WriteLine(string.Join("\t", new string[] { swapEntry.EncryptedName, swapEntry.Index, swapEntry.Size, swapEntry.Flags, swapEntry.DecompSize,
                                swapEntry.LNKOffset, swapEntry.SwapName }));
                        }
                    }
                }
                else if (blpPath == null)
                {
                    lrPath = Path.GetDirectoryName(lnkPath) + @"\mod.config";
                    File.Create(lrPath).Dispose();
                }
            }
        }

        private static void ChangeSwapDB(SwapEntry inputEntry, bool deleteEntry)
        {
            if (!deleteEntry)
            {
                if (swapDB.ContainsKey(inputEntry.EncryptedName))
                {
                    swapDB[inputEntry.EncryptedName] = inputEntry;
                    RebuildSwapDB();
                }
                else
                {
                    swapDB.Add(inputEntry.EncryptedName, inputEntry);
                    using (var sw = new StreamWriter(swapPath, true, Encoding.ASCII))
                    {
                        sw.WriteLine(string.Join("\t", new string[] { inputEntry.EncryptedName, inputEntry.Index, inputEntry.Size, inputEntry.Flags, inputEntry.DecompSize,
                            inputEntry.LNKOffset, inputEntry.SwapName }));
                    }

                    if (lrPath != null)
                    {
                        using (var sw = new StreamWriter(lrPath, true, Encoding.ASCII))
                        {
                            sw.WriteLine(string.Join(":", new string[] { "0x" + inputEntry.Index.ToLowerInvariant(), "0x" + inputEntry.Flags.ToLowerInvariant(),
                                "0x" + inputEntry.DecompSize.ToLowerInvariant(), inputEntry.SwapName.Replace('\\', '/') }));
                        }
                    }
                }
            }
            else
            {
                swapDB.Remove(inputEntry.EncryptedName);
                RebuildSwapDB();
            }
        }

        private static void RebuildSwapDB()
        {
            using (var sw = new StreamWriter(swapPath, false, Encoding.ASCII))
            {
                foreach (var swapEntry in swapDB)
                {
                    sw.WriteLine(string.Join("\t", new string[] { swapEntry.Value.EncryptedName, swapEntry.Value.Index, swapEntry.Value.Size, swapEntry.Value.Flags,
                        swapEntry.Value.DecompSize, swapEntry.Value.LNKOffset, swapEntry.Value.SwapName }));
                }
            }

            if (lrPath != null)
            {
                using (var sw = new StreamWriter(lrPath, false, Encoding.ASCII))
                {
                    foreach (var swapEntry in swapDB)
                    {
                        sw.WriteLine(string.Join(":", new string[] { "0x" + swapEntry.Value.Index.ToLowerInvariant(), "0x" + swapEntry.Value.Flags.ToLowerInvariant(),
                                "0x" + swapEntry.Value.DecompSize.ToLowerInvariant(), swapEntry.Value.SwapName.Replace('\\', '/') }));
                    }
                }
            }
        }

        private static string[] ParseBIN(string fileName)
        {
            using (var ms = new MemoryStream())
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.CopyTo(ms);
                }

                ms.Position = 0;
                var br = new BinaryReader(ms);
                if (br.ReadInt32() != 0x4F4D464C)
                {
                    throw new Exception("Unsupported BIN file format.");
                }

                br.BaseStream.Position = 8;
                int nameCount = br.ReadInt32();
                var nameOffsets = new int[nameCount];
                br.BaseStream.Position = 0x30;
                nameOffsets[0] = br.ReadInt32() + 1;
                for (int i = 1; i < nameCount; i++)
                {
                    br.BaseStream.Position += 8;
                    nameOffsets[i] = br.ReadInt32() + 1;
                }

                var nameEntries = new string[nameCount];
                for (int i = 0; i < nameCount; i++)
                {
                    br.BaseStream.Position = nameOffsets[i];
                    var charsList = new List<char>();
                    int charByte = br.ReadByte();
                    while (charByte != 0)
                    {
                        charsList.Add((char)charByte);
                        charByte = ms.ReadByte();
                    }

                    nameEntries[i] = new string(charsList.ToArray());
                }

                return nameEntries;
            }
        }

        private static uint[,] ParseLNK(string fileName)
        {
            byte[] headerBuffer;
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fs.Position = 8;
                var byteBuffer = new byte[4];
                fs.Read(byteBuffer, 0, 4);
                int headerSize = (BitConverter.ToInt32(byteBuffer, 0) + 1) * 32;
                headerBuffer = new byte[headerSize];
                fs.Position = 0;
                fs.Read(headerBuffer, 0, headerSize);
            }

            using (var ms = new MemoryStream(headerBuffer))
            {
                var br = new BinaryReader(ms);
                br.BaseStream.Position = 8;
                int fileCount = br.ReadInt32();
                br.BaseStream.Position = 0x20;
                var lnkEntries = new uint[fileCount, 2];
                for (int i = 0; i < fileCount; i++)
                {
                    lnkEntries[i, 0] = br.ReadUInt32();
                    br.BaseStream.Position += 4;
                    lnkEntries[i, 1] = br.ReadUInt32();
                    br.BaseStream.Position += 20;
                }

                return lnkEntries;
            }
        }

        private static uint[,] ParseBLP(string fileName)
        {
            using (var ms = new MemoryStream())
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.CopyTo(ms);
                }

                ms.Position = 0;
                var br = new BinaryReader(ms);
                if (br.ReadInt32() == 0x46495031)
                {
                    int entryCount = br.ReadInt32();
                    var blpEntries = new uint[entryCount, 4];
                    br.BaseStream.Position = 0x10;
                    for (int i = 0; i < entryCount; i++)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            blpEntries[i, j] = br.ReadUInt32();
                        }

                        br.BaseStream.Position += 4;
                    }

                    return blpEntries;
                }
                else
                {
                    return null;
                }
            }
        }

        private static int[] MatchIndexes(string[] nameEntries, uint[,] lnkEntries, uint[,] blpEntries)
        {
            var blpIndexes = new int[nameEntries.Length];
            for (int i = 0; i < blpIndexes.Length; i++)
            {
                blpIndexes[i] = -1;
            }

            var dupIndexes = new List<int>();
            for (int i = 0; i < blpEntries.GetLength(0); i++)
            {
                List<string> indexNames;
                if (indexDB.TryGetValue(blpEntries[i, 0].ToString("X"), out indexNames))
                {
                    var foundIndexes = new List<int>();
                    foreach (var indexName in indexNames)
                    {
                        int foundIndex = Array.IndexOf<string>(nameEntries, indexName);
                        while (foundIndex != -1)
                        {
                            foundIndexes.Add(foundIndex);
                            foundIndex = Array.IndexOf<string>(nameEntries, indexName, ++foundIndex);
                        }
                    }

                    foreach (var foundIndex in foundIndexes)
                    {
                        if (blpEntries[i, 1] == lnkEntries[foundIndex, 1])
                        {
                            if (blpIndexes[foundIndex] == -1)
                            {
                                blpIndexes[foundIndex] = i;
                            }
                            else
                            {
                                dupIndexes.Add(foundIndex);
                            }
                        }
                    }
                }
            }

            if (dupIndexes.Count > 0)
            {
                foreach (var dupIndex in dupIndexes)
                {
                    blpIndexes[dupIndex] = -1;
                }
            }

            return blpIndexes;
        }

        private static string[,] DumpGameDB(string fileName)
        {
            using (var ms = new MemoryStream())
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    fs.CopyTo(ms);
                }

                var byteBuffer = ms.ToArray();
                int nameTableOffset = ByteSearch(byteBuffer, FirstName, 0, false);
                if (nameTableOffset < 0)
                {
                    throw new Exception("Name table not found in the game.exe.");
                }

                int assetTableOffset = ByteSearch(byteBuffer, FirstAsset, nameTableOffset, true) - 4;
                if (assetTableOffset < 0)
                {
                    throw new Exception("Asset table not found in the game.exe.");
                }

                int dataTableOffset = ByteSearch(byteBuffer, FirstData, assetTableOffset, false);
                if (dataTableOffset < 0)
                {
                    throw new Exception("Data table not found in the game.exe.");
                }

                var nameOffsets = new List<int>();
                var br = new BinaryReader(ms);
                br.BaseStream.Position = assetTableOffset;
                int nameStartOffset = br.ReadInt32();
                int offsetCorrection = nameStartOffset - nameTableOffset;
                while (nameStartOffset != 0)
                {
                    nameOffsets.Add(nameStartOffset - offsetCorrection);
                    br.BaseStream.Position += 12;
                    nameStartOffset = br.ReadInt32();
                }

                var gameDB = new string[nameOffsets.Count, 2];
                for (int i = 0; i < nameOffsets.Count; i++)
                {
                    br.BaseStream.Position = nameOffsets[i];
                    var charsList = new List<char>();
                    int charByte = br.ReadByte();
                    while (charByte != 0)
                    {
                        charsList.Add((char)charByte);
                        charByte = ms.ReadByte();
                    }

                    gameDB[i, 0] = new string(charsList.ToArray());
                }

                br.BaseStream.Position = dataTableOffset;
                for (int i = 0; i < nameOffsets.Count; i++)
                {
                    br.BaseStream.Position += 4;
                    gameDB[i, 1] = br.ReadUInt32().ToString("X8");
                }

                return gameDB;
            }
        }

        private static void DecryptFile(byte[] inputBuffer, uint decompSize)
        {
            byte[] tempKey = BitConverter.GetBytes((((decompSize + 0x3E7) * 7) / 0xB) + (decompSize % 0x11) + 0x1AC);
            var byteList = new List<byte>();
            for (int i = tempKey.Length - 1; i >= 0; i--)
            {
                if (tempKey[i] != 00)
                {
                    byteList.Add(tempKey[i]);
                }
            }

            byte[] cryptKey = byteList.ToArray();
            int cryptKeyPos = 0;
            //byte[] doaKey = Archive_Tool.Properties.Resources.doaKey;
            byte[] doaKey = DLC_Tool.Properties.Resources.doaKey;
            int doaKeyPos = 0;
            byte xorByte;
            for (int i = 4; i < inputBuffer.Length; i++)
            {
                if (cryptKeyPos == cryptKey.Length)
                {
                    cryptKeyPos = 0;
                }

                if (doaKeyPos == doaKey.Length)
                {
                    doaKeyPos = 0;
                }

                xorByte = (byte)(cryptKey[cryptKeyPos] ^ doaKey[doaKeyPos]);
                if ((inputBuffer[i] != 0) && (inputBuffer[i] != xorByte))
                {
                    inputBuffer[i] = (byte)(inputBuffer[i] ^ xorByte);
                }

                cryptKeyPos++;
                doaKeyPos++;
            }
        }

        private static void WriteDecompressedFile(Stream s, FileStream fs, uint fileSize, uint decompSize)
        {
            fs.SetLength(decompSize);
            long fileEndPos = s.Position + fileSize;
            var sizeBytes = new byte[4];
            int chunkSize;
            s.Position += 4;
            while (s.Position < fileEndPos)
            {
                s.Read(sizeBytes, 0, 4);
                chunkSize = BitConverter.ToInt32(sizeBytes, 0) - 0x8000;
                if (chunkSize > 0)
                {
                    var inputBuffer = new byte[chunkSize];
                    s.Read(inputBuffer, 0, chunkSize);
                    using (var iis = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(new MemoryStream(inputBuffer)))
                    {
                        iis.CopyTo(fs);
                    }
                }
                else
                {
                    chunkSize += 0x8000;
                    var inputBuffer = new byte[chunkSize];
                    s.Read(inputBuffer, 0, chunkSize);
                    fs.Write(inputBuffer, 0, chunkSize);
                }

                if ((s.Position - 4) % 16 != 0)
                {
                    s.Position = s.Position - ((s.Position - 4) % 16) + 16;
                }
            }
        }

        private static int ByteSearch(byte[] byteBuffer, byte[] searchPattern, int startOffset, bool customSearch)
        {
            bool patternFound = false;
            if (!customSearch)
            {
                for (int i = startOffset; i < byteBuffer.Length - searchPattern.Length; i++)
                {
                    if (byteBuffer[i] == searchPattern[0])
                    {
                        patternFound = true;
                        for (int j = 1; j < searchPattern.Length; j++)
                        {
                            if (byteBuffer[i + j] != searchPattern[j])
                            {
                                patternFound = false;
                                break;
                            }
                        }

                        if (patternFound)
                        {
                            return i;
                        }
                    }
                }
            }
            else
            {
                for (int i = startOffset; i < byteBuffer.Length - searchPattern.Length; i++)
                {
                    if (byteBuffer[i] == searchPattern[0])
                    {
                        patternFound = true;
                        for (int j = 1; j < 4; j++)
                        {
                            if (byteBuffer[i + j] != searchPattern[j])
                            {
                                patternFound = false;
                                break;
                            }
                        }

                        for (int j = 4; j < 8; j++)
                        {
                            if (byteBuffer[i + j + 4] != searchPattern[j])
                            {
                                patternFound = false;
                                break;
                            }
                        }

                        for (int j = 8; j < 12; j++)
                        {
                            if (byteBuffer[i + j + 8] != searchPattern[j])
                            {
                                patternFound = false;
                                break;
                            }
                        }

                        if (patternFound)
                        {
                            return i;
                        }
                    }
                }
            }

            return -1;
        }

        private static Assembly ResolveEventHandler(Object sender, ResolveEventArgs args)
        {
            string dllName = new AssemblyName(args.Name).Name + ".dll";
            if (dllName != "ICSharpCode.SharpZipLib.dll")
            {
                return null;
            }
            else
            {
                currentDomain.AssemblyResolve -= ResolveEventHandler;
                //return Assembly.Load(Archive_Tool.Properties.Resources.ICSharpCode_SharpZipLib);
                return Assembly.Load(DLC_Tool.Properties.Resources.ICSharpCode_SharpZipLib);
            }
        }

        private static void WriteCompressedFile(FileStream fs, FileStream fs2)
        {
            var taskList = new List<Task<byte[]>>();
            while (true)
            {
                var inputBuffer = new byte[0x4000];
                int readCount = fs2.Read(inputBuffer, 0, 0x4000);
                if (readCount > 0)
                {
                    taskList.Add(Task<byte[]>.Factory.StartNew(() => CompressChunk(inputBuffer, readCount)));
                }
                else
                {
                    break;
                }
            }

            fs.Write(BitConverter.GetBytes((uint)fs2.Length), 0, 4);
            foreach (var taskResult in taskList)
            {
                if (taskResult.Status != TaskStatus.RanToCompletion)
                {
                    taskResult.Wait();
                }

                fs.Write(BitConverter.GetBytes(taskResult.Result.Length + 0x8000), 0, 4);
                fs.Write(taskResult.Result, 0, taskResult.Result.Length);
                if ((fs.Position - 4) % 16 > 0)
                {
                    fs.Position = fs.Position - ((fs.Position - 4) % 16) + 16;
                }
            }

            if ((fs.Length - 4) % 16 > 0)
            {
                fs.Position -= 1;
                fs.WriteByte((byte)0);
            }
        }

        private static byte[] CompressChunk(byte[] chunkBuffer, int chunkSize)
        {
            using (var ms = new MemoryStream())
            {
                var zs = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.DeflaterOutputStream(ms);
                zs.IsStreamOwner = false;
                zs.Write(chunkBuffer, 0, chunkSize);
                zs.Close();
                return ms.ToArray();
            }
        }

        private static void WriteFile(Stream s, FileStream fs, uint fileSize)
        {
            fs.SetLength(fileSize);
            var inputBuffer = new byte[0x1000];
            uint chunksCount = fileSize / 0x1000;
            int lastChunkSize = (int)(fileSize % 0x1000);
            int readCount;
            for (int i = 0; i < chunksCount; i++)
            {
                readCount = s.Read(inputBuffer, 0, 0x1000);
                fs.Write(inputBuffer, 0, readCount);
            }

            if (lastChunkSize > 0)
            {
                readCount = s.Read(inputBuffer, 0, lastChunkSize);
                fs.Write(inputBuffer, 0, readCount);
            }
        }

        /*
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += ResolveEventHandler;
            mainForm = new MainForm();
            Application.Run(mainForm);
        }
        */
    }
}