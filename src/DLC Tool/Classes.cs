namespace DLC_Tool
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class DLCData
    {
        public DLCData()
        {
            this.Chars = new List<Character>();
        }

        public string SavePath { get; set; }

        public byte BcmVer { get; set; }

        public byte skipRead { get; set; }

        public List<Character> Chars { get; private set; }
    }

    [Serializable]
    public class Character
    {
        public Character()
        {
            this.HStyles = new List<Hairstyle>();
            this.Files = new string[12];
        }

        public byte ID { get; set; }

        public bool Female { get; set; }

        public byte CostumeSlot { get; set; }

        public byte AddTexsCount { get; set; }

        public List<Hairstyle> HStyles { get; private set; }

        public string[] Files { get; private set; }

        // ほげほげの拡張
        public string Comment { get; set; } = "";
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