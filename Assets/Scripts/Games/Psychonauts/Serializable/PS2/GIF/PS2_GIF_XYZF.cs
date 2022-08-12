﻿using BinarySerializer;

namespace Ray1Map.Psychonauts
{
    public class PS2_GIF_XYZF : BinarySerializable
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float F { get; set; }

        public override void SerializeImpl(SerializerObject s)
        {
            X = s.Serialize<float>(X, name: nameof(X));
            Y = s.Serialize<float>(Y, name: nameof(Y));
            Z = s.Serialize<float>(Z, name: nameof(Z));
			F = s.Serialize<float>(F, name: nameof(F));
        }
    }
}