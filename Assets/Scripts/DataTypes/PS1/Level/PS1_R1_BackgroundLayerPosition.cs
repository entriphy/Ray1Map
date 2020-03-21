﻿using System.IO;

namespace R1Engine
{
    /// <summary>
    /// Background later position data for Rayman 1 (PS1)
    /// </summary>
    public class PS1_R1_BackgroundLayerPosition : R1Serializable
    {
        /// <summary>
        /// The layer x position
        /// </summary>
        public ushort XPosition { get; set; }
        
        /// <summary>
        /// The later y position
        /// </summary>
        public ushort YPosition { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            XPosition = s.Serialize(XPosition, name: "XPosition");
            YPosition = s.Serialize(YPosition, name: "YPosition");
        }
    }
}