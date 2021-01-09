﻿namespace R1Engine
{
    public class GBACrash_ObjDataUnkTable : R1Serializable
    {
        public uint Count { get; set; }
        public Entry[] Entries { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            Count = s.Serialize<uint>(Count, name: nameof(Count));
            Entries = s.SerializeObjectArray<Entry>(Entries, Count, name: nameof(Entries));
        }

        public class Entry : R1Serializable
        {
            public uint Uint_00 { get; set; }
            public uint Uint_04 { get; set; }

            public override void SerializeImpl(SerializerObject s)
            {
                Uint_00 = s.Serialize<uint>(Uint_00, name: nameof(Uint_00));
                Uint_04 = s.Serialize<uint>(Uint_04, name: nameof(Uint_04));
            }
        }
    }
}