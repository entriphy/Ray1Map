﻿using System;
using BinarySerializer;

public class UnityWindowBitSerializer : BitSerializerObject
{
    public UnityWindowBitSerializer(SerializerObject serializerObject, Pointer valueOffset, string logPrefix, long value) 
        : base(serializerObject, valueOffset, logPrefix, value) { }

    public override T SerializeBits<T>(T value, int length, SignedNumberRepresentation sign = SignedNumberRepresentation.Unsigned, string name = null) 
    {
        T t = SerializerObject.Serialize<T>(value, name);

        Position += length;

        return t;
    }

    public override T SerializeObject<T>(T obj, Action<T> onPreSerialize = null, string name = null)
    {
        throw new NotImplementedException();
    }
}