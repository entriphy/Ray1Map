﻿namespace R1Engine
{
    public class GBA_R3_Data : R1Serializable
    {
        public GBA_R3_OffsetTable UiOffsetTable { get; set; }

        public GBA_R3_LevelBlock LevelBlock { get; set; }

        /// <summary>
        /// Handles the data serialization
        /// </summary>
        /// <param name="s">The serializer object</param>
        public override void SerializeImpl(SerializerObject s)
        {
            // Get the pointer table
            var pointerTable = PointerTables.GetGBAR3PointerTable(s.GameSettings.GameModeSelection, Offset.file);

            // Serialize the offset table
            s.DoAt(pointerTable[GBA_R3_Pointer.UiOffsetTable], () => UiOffsetTable = s.SerializeObject<GBA_R3_OffsetTable>(UiOffsetTable, name: nameof(UiOffsetTable)));

            // Serialize the level block for the current level
            LevelBlock = s.DoAt(UiOffsetTable.GetPointer(s.Context.Settings.Level, true), () => s.SerializeObject<GBA_R3_LevelBlock>(LevelBlock, name: nameof(LevelBlock)));

        }
    }
}