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