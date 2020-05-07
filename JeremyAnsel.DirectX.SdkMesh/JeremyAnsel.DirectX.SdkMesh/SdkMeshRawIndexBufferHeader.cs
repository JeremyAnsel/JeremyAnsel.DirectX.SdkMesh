using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshRawIndexBufferHeader
    {
        public long NumIndices { get; private set; }

        public long SizeBytes { get; private set; }

        public SdkMeshIndexType IndexType { get; private set; }

        public long DataOffset { get; private set; }

        internal static SdkMeshRawIndexBufferHeader Read(BinaryReader reader)
        {
            var header = new SdkMeshRawIndexBufferHeader
            {
                NumIndices = reader.ReadInt64(),
                SizeBytes = reader.ReadInt64(),
                IndexType = (SdkMeshIndexType)reader.ReadInt64(),
                DataOffset = reader.ReadInt64()
            };

            return header;
        }
    }
}
