using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshRawVertexBufferHeader
    {
        private const int MaxVertexElements = 32;

        public long NumVertices { get; private set; }

        public long SizeBytes { get; private set; }

        public long StrideBytes { get; private set; }

        public IList<D3DVertexElement> Decl { get; } = new List<D3DVertexElement>();

        public long DataOffset { get; private set; }

        internal static SdkMeshRawVertexBufferHeader Read(BinaryReader reader)
        {
            var header = new SdkMeshRawVertexBufferHeader
            {
                NumVertices = reader.ReadInt64(),
                SizeBytes = reader.ReadInt64(),
                StrideBytes = reader.ReadInt64()
            };

            bool add = true;

            for (int i = 0; i < MaxVertexElements; i++)
            {
                D3DVertexElement element = D3DVertexElement.Read(reader);

                if (add && element.Type == D3DDeclType.Unused)
                {
                    add = false;
                }

                if (add)
                {
                    header.Decl.Add(element);
                }
            }

            header.DataOffset = reader.ReadInt64();

            return header;
        }
    }
}
