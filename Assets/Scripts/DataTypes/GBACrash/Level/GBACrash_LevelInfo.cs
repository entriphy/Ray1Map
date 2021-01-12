﻿namespace R1Engine
{
    public class GBACrash_LevelInfo : R1Serializable
    {
        public GBACrash_BaseManager.LevInfo LevInfo { get; set; } // Set before serializing if it's the current level

        public uint LocIndex_LevelName { get; set; }
        public uint LevelTheme { get; set; } // For the level music among some other hard-coded checks
        public GBACrash_Time Time_Sapphire { get; set; }
        public GBACrash_Time Time_Gold { get; set; }
        public GBACrash_Time Time_Platinum { get; set; }
        public uint Uint_14 { get; set; }
        public uint Uint_18 { get; set; }
        public bool IsBossFight { get; set; }
        public Pointer LevelDataPointer { get; set; }

        // Serialized from pointers

        public GBACrash_LevelData LevelData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            LocIndex_LevelName = s.Serialize<uint>(LocIndex_LevelName, name: nameof(LocIndex_LevelName));
            s.Log($"LevelName: {s.Context.GetStoredObject<GBACrash_LocTable>(GBACrash_BaseManager.LocTableID).Strings[LocIndex_LevelName]}");
            LevelTheme = s.Serialize<uint>(LevelTheme, name: nameof(LevelTheme));

            Time_Sapphire = s.SerializeObject<GBACrash_Time>(Time_Sapphire, name: nameof(Time_Sapphire));
            Time_Gold = s.SerializeObject<GBACrash_Time>(Time_Gold, name: nameof(Time_Gold));
            Time_Platinum = s.SerializeObject<GBACrash_Time>(Time_Platinum, name: nameof(Time_Platinum));

            Uint_14 = s.Serialize<uint>(Uint_14, name: nameof(Uint_14));
            Uint_18 = s.Serialize<uint>(Uint_18, name: nameof(Uint_18));
            IsBossFight = s.Serialize<bool>(IsBossFight, name: nameof(IsBossFight));
            s.SerializeArray<byte>(new byte[3], 3, name: "Padding");
            LevelDataPointer = s.SerializePointer(LevelDataPointer, name: nameof(LevelDataPointer));

            LevelData = s.DoAt(LevelDataPointer, () => s.SerializeObject<GBACrash_LevelData>(LevelData, x => x.LevInfo = LevInfo, name: nameof(LevelData)));
        }
    }
}