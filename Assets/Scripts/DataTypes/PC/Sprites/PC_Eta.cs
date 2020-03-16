﻿namespace R1Engine
{
    /// <summary>
    /// ETA data for PC
    /// </summary>
    public class PC_Eta : IBinarySerializable
    {
        /// <summary>
        /// The right speed
        /// </summary>
        public sbyte RightSpeed { get; set; }
        
        /// <summary>
        /// The left speed
        /// </summary>
        public sbyte LeftSpeed { get; set; }
        
        /// <summary>
        /// The animation index
        /// </summary>
        public byte AnimationIndex { get; set; }
        
        /// <summary>
        /// The etat value
        /// </summary>
        public byte Etat { get; set; }
        
        /// <summary>
        /// The sub-etat value
        /// </summary>
        public byte SubEtat { get; set; }
        
        /// <summary>
        /// The amount of frames to skip in the animation each second, or 0 for it to not animate
        /// </summary>
        public byte AnimationSpeed { get; set; }
        
        /// <summary>
        /// The sound index
        /// </summary>
        public byte SoundIndex { get; set; }
        
        /// <summary>
        /// The interaction type
        /// </summary>
        public byte InteractionType { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public void Serialize(BinarySerializer serializer)
        {
            serializer.Serialize(nameof(RightSpeed));
            serializer.Serialize(nameof(LeftSpeed));
            serializer.Serialize(nameof(AnimationIndex));
            serializer.Serialize(nameof(Etat));
            serializer.Serialize(nameof(SubEtat));
            serializer.Serialize(nameof(AnimationSpeed));
            serializer.Serialize(nameof(SoundIndex));
            serializer.Serialize(nameof(InteractionType));
        }
    }
}