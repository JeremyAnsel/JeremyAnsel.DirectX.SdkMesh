using JeremyAnsel.DirectX.DXMath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshRawAnimData
    {
        public XMFloat3 Translation { get; private set; }

        public XMFloat4 Orientation { get; private set; }

        public XMFloat3 Scaling { get; private set; }

        internal static SdkMeshRawAnimData Read(BinaryReader reader)
        {
            var data = new SdkMeshRawAnimData
            {
                Translation = new XMFloat3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                Orientation = new XMFloat4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                Scaling = new XMFloat3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
            };

            return data;
        }
    }
}
