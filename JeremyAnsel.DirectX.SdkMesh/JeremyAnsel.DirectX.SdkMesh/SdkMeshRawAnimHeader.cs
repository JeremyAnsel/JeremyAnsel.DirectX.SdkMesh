using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshRawAnimHeader
    {
        public int Version { get; private set; }

        public bool IsBigEndian { get; private set; }

        public SdkMeshFrameTransformType FrameTransformType { get; private set; }

        public int NumFrames { get; private set; }

        public int NumAnimationKeys { get; private set; }

        public int AnimationFPS { get; private set; }

        public long AnimationDataSize { get; private set; }

        public long AnimationDataOffset { get; private set; }

        internal static SdkMeshRawAnimHeader Read(BinaryReader reader)
        {
            var header = new SdkMeshRawAnimHeader
            {
                Version = reader.ReadInt32(),
                IsBigEndian = reader.ReadInt32() != 0,
                FrameTransformType = (SdkMeshFrameTransformType)reader.ReadInt32(),
                NumFrames = reader.ReadInt32(),
                NumAnimationKeys = reader.ReadInt32(),
                AnimationFPS = reader.ReadInt32(),
                AnimationDataSize = reader.ReadInt64(),
                AnimationDataOffset = reader.ReadInt64()
            };

            return header;
        }
    }
}
