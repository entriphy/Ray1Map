﻿namespace R1Engine
{
    using Type = PC_WorldFile.Type;

    /// <summary>
    /// DES item data for PC
    /// </summary>
    public class PC_DesItem : R1Serializable {
        public Type FileType { get; set; }

        /// <summary>
        /// Indicates if the sprite has some gradation and requires clearing
        /// </summary>
        public bool RequiresBackgroundClearing { get; set; }

        public byte[] Unknown1 { get; set; }

        /// <summary>
        /// The length of the image data
        /// </summary>
        public uint ImageDataLength { get; set; }

        /// <summary>
        /// The image data
        /// </summary>
        public byte[] ImageData { get; set; }

        // TODO: In kit and edu this comes before the image data
        /// <summary>
        /// The checksum for <see cref="ImageData"/>
        /// </summary>
        public byte ImageDataChecksum { get; set; }

        public uint Unknown2 { get; set; }

        /// <summary>
        /// The amount of image descriptors
        /// </summary>
        public ushort ImageDescriptorCount { get; set; }

        /// <summary>
        /// The image descriptors
        /// </summary>
        public PC_ImageDescriptor[] ImageDescriptors { get; set; }

        /// <summary>
        /// The amount of animation descriptors
        /// </summary>
        public byte AnimationDescriptorCount { get; set; }

        /// <summary>
        /// The animation descriptors
        /// </summary>
        public PC_AnimationDescriptor[] AnimationDescriptors { get; set; }

        /// <summary>
        /// Serializes the data
        /// </summary>
        /// <param name="serializer">The serializer</param>
        public override void SerializeImpl(SerializerObject s) {
            if (FileType == Type.World)
                RequiresBackgroundClearing = s.Serialize(RequiresBackgroundClearing, name: "RequiresBackgroundClearing");
            else
                RequiresBackgroundClearing = true;

            if (FileType == Type.AllFix)
                Unknown1 = s.SerializeArray<byte>(Unknown1, 12, name: "Unknown1");

            ImageDataLength = s.Serialize(ImageDataLength, name: "ImageDataLength");

            if (FileType == Type.World && (s.GameSettings.GameMode == GameMode.RayKit || s.GameSettings.GameMode == GameMode.RayEduPC))
            {
                ImageDataChecksum = s.Serialize(ImageDataChecksum, name: "ImageDataChecksum");
                ImageData = s.SerializeArray<byte>(ImageData, ImageDataLength, name: "ImageData");
            }
            else
            {
                ImageData = s.SerializeArray<byte>(ImageData, ImageDataLength, name: "ImageData");

                if (FileType != Type.BigRay)
                    ImageDataChecksum = s.Serialize(ImageDataChecksum, name: "ImageDataChecksum");
            }

            if (FileType == Type.AllFix)
                Unknown2 = s.Serialize(Unknown2, name: "Unknown2");

            ImageDescriptorCount = s.Serialize(ImageDescriptorCount, name: "ImageDescriptorCount");
            ImageDescriptors = s.SerializeObjectArray<PC_ImageDescriptor>(ImageDescriptors, ImageDescriptorCount, name: "ImageDescriptors");
            AnimationDescriptorCount = s.Serialize(AnimationDescriptorCount, name: "AnimationDescriptorCount");
            AnimationDescriptors = s.SerializeObjectArray<PC_AnimationDescriptor>(AnimationDescriptors, AnimationDescriptorCount, name: "AnimationDescriptors");
        }
    }
}