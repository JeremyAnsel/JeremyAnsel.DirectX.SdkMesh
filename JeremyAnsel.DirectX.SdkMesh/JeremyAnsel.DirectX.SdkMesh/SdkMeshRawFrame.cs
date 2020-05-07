using JeremyAnsel.DirectX.DXMath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshRawFrame
    {
        private const int MaxFrameName = 100;

        public string Name { get; private set; }

        public int MeshIndex { get; private set; }

        public int ParentFrameIndex { get; private set; }

        public int ChildFrameIndex { get; private set; }

        public int SiblingFrameIndex { get; private set; }

        public XMFloat4X4 Matrix { get; private set; }

        public int AnimationDataIndex { get; private set; }

        internal static SdkMeshRawFrame Read(BinaryReader reader)
        {
            var frame = new SdkMeshRawFrame
            {
                Name = Encoding.UTF8.GetString(reader.ReadBytes(MaxFrameName).TakeWhile(t => t != 0).ToArray()),
                MeshIndex = reader.ReadInt32(),
                ParentFrameIndex = reader.ReadInt32(),
                ChildFrameIndex = reader.ReadInt32(),
                SiblingFrameIndex = reader.ReadInt32(),
                Matrix = new XMFloat4X4(
                    reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                    reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                    reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(),
                    reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                AnimationDataIndex = reader.ReadInt32()
            };

            return frame;
        }
    }
}
