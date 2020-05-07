using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class D3DVertexElement
    {
        public short StreamIndex { get; private set; }

        public short Offset { get; private set; }

        public D3DDeclType Type { get; private set; }

        public D3DDeclMethod Method { get; private set; }

        public D3DDeclUsage Usage { get; private set; }

        public byte UsageIndex { get; private set; }

        internal static D3DVertexElement Read(BinaryReader reader)
        {
            var element = new D3DVertexElement
            {
                StreamIndex = reader.ReadInt16(),
                Offset = reader.ReadInt16(),
                Type = (D3DDeclType)reader.ReadByte(),
                Method = (D3DDeclMethod)reader.ReadByte(),
                Usage = (D3DDeclUsage)reader.ReadByte(),
                UsageIndex = reader.ReadByte()
            };

            return element;
        }
    }
}
