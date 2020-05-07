using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshRawAnimFrameData
    {
        private const int MaxFrameName = 100;

        public string FrameName { get; private set; }

        public long DataOffset { get; private set; }

        public IList<SdkMeshRawAnimData> AnimationKeys { get; } = new List<SdkMeshRawAnimData>();

        internal static SdkMeshRawAnimFrameData Read(BinaryReader reader)
        {
            var frame = new SdkMeshRawAnimFrameData
            {
                FrameName = Encoding.UTF8.GetString(reader.ReadBytes(MaxFrameName).TakeWhile(t => t != 0).ToArray())
            };

            reader.ReadInt32();
            frame.DataOffset = reader.ReadInt64();

            return frame;
        }
    }
}
