﻿using System.Collections.Generic;
using System.Linq;

namespace R1Engine
{
    public abstract class GBAVV_BaseROM : GBA_ROMBase
    {
        public bool SerializeFLC { get; set; } // Set before serializing

        public GBAVV_Script[] Scripts { get; set; }
        public GBAVV_Script[] HardCodedScripts { get; set; }
        public IEnumerable<GBAVV_Script> GetAllScripts => Scripts?.Concat(HardCodedScripts ?? new GBAVV_Script[0]) ?? new GBAVV_Script[0];
        public GBAVV_DialogScript[] DialogScripts { get; set; }
        public GBAVV_Graphics[] Map2D_Graphics { get; set; }

        protected void SerializeScripts(SerializerObject s)
        {
            // Get the pointer table
            var pointerTable = PointerTables.GBAVV_PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Serialize scripts
            var scriptPointers = ((GBAVV_BaseManager)s.GameSettings.GetGameManager).ScriptPointers;

            if (scriptPointers != null)
            {
                if (Scripts == null)
                    Scripts = new GBAVV_Script[scriptPointers.Length];

                for (int i = 0; i < scriptPointers.Length; i++)
                    Scripts[i] = s.DoAt(new Pointer(scriptPointers[i], Offset.file), () => s.SerializeObject<GBAVV_Script>(Scripts[i], x =>
                    {
                        x.SerializeFLC = SerializeFLC;
                        x.BaseFile = Offset.file;
                    }, name: $"{nameof(Scripts)}[{i}]"));

                if (s.GameSettings.GBAVV_IsFusion && HardCodedScripts == null)
                    HardCodedScripts = s.DoAtBytes(((GBAVV_Fusion_Manager)s.GameSettings.GetGameManager).HardCodedScripts, nameof(HardCodedScripts), () => s.SerializeObjectArrayUntil<GBAVV_Script>(HardCodedScripts, x => s.CurrentPointer.FileOffset >= s.CurrentLength, onPreSerialize: x =>
                    {
                        x.SerializeFLC = SerializeFLC;
                        x.BaseFile = Offset.file;
                    }, includeLastObj: true, name: nameof(HardCodedScripts)));

                DialogScripts = s.DoAt(pointerTable.TryGetItem(GBAVV_Pointer.Fusion_DialogScripts), () => s.SerializeObjectArray<GBAVV_DialogScript>(DialogScripts, ((GBAVV_Fusion_Manager)s.GameSettings.GetGameManager).DialogScriptsCount, name: nameof(DialogScripts)));
            }
        }
    }
}