namespace DLC_Tool
{
    using System;
    using System.Collections.Generic;


    public class UndoBuffer<T> where T : ICloneable
    {
        public List<T> History = new List<T>();
        public List<bool> Saved = new List<bool>();
        private int NextIndex_Hiddne = 0;
        public int NextIndex
        {
            get { return NextIndex_Hiddne; }
            set { NextIndex_Hiddne = Math.Max(0, Math.Min(History.Count, value)); }
        }

        public UndoBuffer()
        {
        }
        
        public UndoBuffer(T First)
        {
            Update(First);
        }

        public bool Update(T Current)
        {
            if(NextIndex > 0 && Current.Equals(History[NextIndex - 1]))
            {
                return false;
            }

            History.RemoveRange(NextIndex, History.Count - NextIndex);
            History.Add((T)Current.Clone());

            Saved.RemoveRange(NextIndex, Saved.Count - NextIndex);
            Saved.Add(false);

            NextIndex++;

            return true;
        }

        public bool SetSaved()
        {
            if(NextIndex > 0)
            {
                Saved[NextIndex - 1] = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool GetSaved()
        {
            return NextIndex > 0 ? Saved[NextIndex - 1] : true;
        }

        public bool Undoable()
        {
            return NextIndex > 1;
        }

        public bool Redoable()
        {
            return NextIndex < History.Count;
        }

        public T Undo()
        {
            if(NextIndex > 1)
            {
                NextIndex--;
                return (T)History[NextIndex - 1].Clone();
            }/*
            else if(NextIndex == 1)
            {
                NextIndex = 0;
                return History[0];
            }*/
            else
            {
                throw new Exception("No history.");
            }
        }

        public T Redo()
        {
            if (NextIndex < History.Count)
            {
                NextIndex++;
                return (T)History[NextIndex - 1].Clone();
            }
            else
            {
                throw new Exception("No history.");
            }
        }
    }
    

    [Serializable]
    public class DLCData : ICloneable
    {
        public DLCData()
        {
            this.Chars = new List<Character>();
        }

        public string SavePath { get; set; }

        public byte BcmVer { get; set; }

        public byte skipRead { get; set; }

        public List<Character> Chars { get; private set; }

        // Equals オーバーライドの要請
        // ・副作用を持たない関数であること
        // ・例外をスローしないこと
        // ・同値関係であること
        // ・GetHashCode もオーバーライドすることが推奨される（多分集合系のジェネリックを使わないなら必要ない）
        // ・null 非許容の場合 null とは等しくならないこと・・・とあるけど、null 許容だってそうだよねぇ
        public override bool Equals(Object obj)
        {
            if(obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var dlcData2 = (DLCData)obj;

            
            if(!(dlcData2.SavePath == SavePath && dlcData2.BcmVer == BcmVer && dlcData2.skipRead == skipRead && dlcData2.Chars.Count == Chars.Count))
            {
                return false;
            }


            for (var i = 0; i < Chars.Count; i++)
            {
                if (!dlcData2.Chars[i].Equals(Chars[i]))
                {
                    return false;
                }
            }

            return true;
        }


        public override int GetHashCode()
        {
            var HashCode = SavePath.GetHashCode() ^ BcmVer.GetHashCode() ^ skipRead.GetHashCode();

            for (var i = 0; i < Chars.Count; i++)
            {
                HashCode ^= Chars[i].GetHashCode();
            }

            return HashCode;
        }



        public virtual object Clone()
        {
            var result = (DLCData)MemberwiseClone();
            result.Chars = new List<Character>();
            foreach(var chr in Chars)
            {
                result.Chars.Add((Character)chr.Clone());
            }

            return result;
        }

    }

    [Serializable]
    public class Character : ICloneable
    {
        public Character()
        {
            this.HStyles = new List<Hairstyle>();
            this.Files = new string[Program.CharacterFiles.Length];
        }

        public byte ID { get; set; }

        public bool Female { get; set; }

        public byte CostumeSlot { get; set; }

        public byte AddTexsCount { get; set; }

        public List<Hairstyle> HStyles { get; private set; }

        public string[] Files { get; private set; }

        // ほげほげの拡張
        public string Comment { get; set; } = "";

        public override bool Equals(Object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var chr = (Character)obj;
            
            if(!( chr.ID == ID && chr.Female == Female && chr.CostumeSlot == CostumeSlot && chr.AddTexsCount == AddTexsCount && chr.HStyles.Count == HStyles.Count && chr.Files.Length == Files.Length && chr.Comment == Comment))
            {
                return false;
            }
            
            for(var i = 0; i < HStyles.Count; i++)
            {
                if (!chr.HStyles[i].Equals(HStyles[i]))
                {
                    return false;
                }
            }


            for (var i = 0; i < Files.Length; i++)
            {
                if (chr.Files[i] != Files[i])
                {
                    return false;
                }
            }

            return true;
        }
        
        public override int GetHashCode()
        {
            var HashCode = ID.GetHashCode() ^ Female.GetHashCode() ^ CostumeSlot.GetHashCode() ^ AddTexsCount.GetHashCode() ^ Comment.GetHashCode();

            for (var i = 0; i < HStyles.Count; i++)
            {
                HashCode ^= HStyles[i].GetHashCode();
            }


            for (var i = 0; i < Files.Length; i++)
            {
                if(Files[i] != null)
                HashCode ^= Files[i].GetHashCode();
            }
            
            return HashCode;
        }


        public virtual object Clone()
        {
            var result = (Character)MemberwiseClone();

            result.HStyles = new List<Hairstyle>();
            foreach (var hs in HStyles)
            {
                result.HStyles.Add(new Hairstyle(hs.Hair, hs.Face));
            }

            result.Files = new string[Files.Length];
            Array.Copy(Files, result.Files, Files.Length);

            return result;
        }
    }

    [Serializable]
    public class Hairstyle
    {
        public Hairstyle(byte hair, byte face)
        {
            this.Hair = hair;
            this.Face = face;
        }

        public Hairstyle()
        {
        }

        public byte Hair { get; set; }

        public byte Face { get; set; }

        
        public override bool Equals(Object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var hs = (Hairstyle)obj;

            return hs.Hair == Hair && hs.Face == Face;
        }

        public override int GetHashCode()
        {
            return Hair.GetHashCode() ^ Face.GetHashCode();
        }
    }

    /* DLC Tool 1.1 より */
    public class NameEntry
    {
        public NameEntry(string name, string flags, string index)
        {
            this.Name = name;
            this.Flags = flags;
            this.Index = index;
        }

        public string Name { get; set; }

        public string Flags { get; set; }

        public string Index { get; set; }
    }
}

namespace Archive_Tool
{
    using System.Collections.Generic;

    public class ArchiveFile
    {
        public string Name { get; set; }

        public string EncryptedName { get; set; }

        public string SwapName { get; set; }

        public string Index { get; set; }

        public uint Size { get; set; }

        public string Flags { get; set; }

        public string DecompSize { get; set; }

        public int BLPIndex { get; set; }

        public int LNKIndex { get; set; }

        public uint LNKOffset { get; set; }

        public bool Unused { get; set; }

        public bool Unknown { get; set; }
    }

    public class NameEntry
    {
        public NameEntry(string encryptedName, string name, string flags, List<string> indexes, bool unused)
        {
            this.EncryptedName = encryptedName;
            this.Name = name;
            this.Flags = flags;
            this.Indexes = indexes;
            this.Unused = unused;
        }

        public NameEntry(string encryptedName, string name, string flags, bool unused)
        {
            this.EncryptedName = encryptedName;
            this.Name = name;
            this.Flags = flags;
            this.Unused = unused;
        }

        public string EncryptedName { get; set; }

        public string Name { get; set; }

        public string Flags { get; set; }

        public List<string> Indexes { get; set; }

        public bool Unused { get; set; }
    }

    public class SwapEntry
    {
        public SwapEntry(string encryptedName, string index, string size, string flags, string decompSize, string lnkOffset, string swapName)
        {
            this.EncryptedName = encryptedName;
            this.Index = index;
            this.Size = size;
            this.Flags = flags;
            this.DecompSize = decompSize;
            this.LNKOffset = lnkOffset;
            this.SwapName = swapName;
        }

        public SwapEntry()
        {
        }

        public string EncryptedName { get; set; }

        public string Index { get; set; }

        public string Size { get; set; }

        public string Flags { get; set; }

        public string DecompSize { get; set; }

        public string LNKOffset { get; set; }

        public string SwapName { get; set; }
    }
}