﻿namespace R1Engine
{
    public class GBAVV_ROM_NitroKart : GBAVV_BaseROM
    {
        // Helpers
        public GBAVV_NitroKart_LevelInfo CurrentLevelInfo => LevelInfos[Context.Settings.Level];

        // Nitro Kart
        public GBAVV_NitroKart_HubWorldPortal[][] HubWorldPortals { get; set; }
        public GBAVV_NitroKart_LevelInfo[] LevelInfos { get; set; }
        public GBAVV_NitroKart_ObjTypeData[] ObjTypeData { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            // Serialize ROM header
            base.SerializeImpl(s);

            // Get the pointer table
            var pointerTable = PointerTables.GBAVV_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Get the graphics pointers
            var graphicsDataPointers = s.GameSettings.GetGameManagerOfType<GBAVV_NitroKart_Manager>().GraphicsDataPointers;

            // Serialize graphics
            if (Map2D_Graphics == null)
                Map2D_Graphics = new GBAVV_Graphics[graphicsDataPointers.Length];

            for (int i = 0; i < graphicsDataPointers.Length; i++)
                Map2D_Graphics[i] = s.DoAt(new Pointer(graphicsDataPointers[i], Offset.file), () => s.SerializeObject<GBAVV_Graphics>(Map2D_Graphics[i], name: $"{nameof(Map2D_Graphics)}[{i}]"));

            // Serialize scripts
            SerializeScripts(s);

            // Serialize level meta data
            s.DoAt(pointerTable[GBAVV_Pointer.NitroKart_HubWorldPortals], () =>
            {
                if (HubWorldPortals == null)
                    HubWorldPortals = new GBAVV_NitroKart_HubWorldPortal[5][];

                for (int i = 0; i < HubWorldPortals.Length; i++)
                    HubWorldPortals[i] = s.SerializeObjectArray<GBAVV_NitroKart_HubWorldPortal>(HubWorldPortals[i], 9, name: $"{nameof(HubWorldPortals)}[{i}]");
            });

            // Serialize level infos
            s.DoAt(pointerTable[GBAVV_Pointer.NitroKart_LevelInfos], () =>
            {
                if (LevelInfos == null)
                    LevelInfos = new GBAVV_NitroKart_LevelInfo[26];

                for (int i = 0; i < LevelInfos.Length; i++)
                    LevelInfos[i] = s.SerializeObject<GBAVV_NitroKart_LevelInfo>(LevelInfos[i], x => x.SerializeData = i == s.GameSettings.Level, name: $"{nameof(LevelInfos)}[{i}]");
            });

            var objTypesDataPointers = s.GameSettings.GetGameManagerOfType<GBAVV_NitroKart_Manager>().ObjTypesDataPointers;

            // Serialize object type data
            if (ObjTypeData == null)
                ObjTypeData = new GBAVV_NitroKart_ObjTypeData[objTypesDataPointers.Length];

            for (int i = 0; i < objTypesDataPointers.Length; i++)
                ObjTypeData[i] = s.DoAt(objTypesDataPointers[i] == null ? null : new Pointer(objTypesDataPointers[i].Value, Offset.file), () => s.SerializeObject<GBAVV_NitroKart_ObjTypeData>(ObjTypeData[i], name: $"{nameof(ObjTypeData)}[{i}]"));
        }
    }
}