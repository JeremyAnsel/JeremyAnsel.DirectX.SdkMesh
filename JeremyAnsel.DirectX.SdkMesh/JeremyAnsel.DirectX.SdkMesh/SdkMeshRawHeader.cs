using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshRawHeader
    {
        public int Version { get; private set; }

        public bool IsBigEndian { get; private set; }

        public long HeaderSize { get; private set; }

        public long NonBufferDataSize { get; private set; }

        public long BufferDataSize { get; private set; }

        public int NumVertexBuffers { get; private set; }

        public int NumIndexBuffers { get; private set; }

        public int NumMeshes { get; private set; }

        public int NumTotalSubsets { get; private set; }

        public int NumFrames { get; private set; }

        public int NumMaterials { get; private set; }

        public long VertexStreamHeadersOffset { get; private set; }

        public long IndexStreamHeadersOffset { get; private set; }

        public long MeshDataOffset { get; private set; }

        public long SubsetDataOffset { get; private set; }

        public long FrameDataOffset { get; private set; }

        public long MaterialDataOffset { get; private set; }

        internal static SdkMeshRawHeader Read(BinaryReader reader)
        {
            var header = new SdkMeshRawHeader
            {
                Version = reader.ReadInt32(),
                IsBigEndian = reader.ReadInt32() != 0,
                HeaderSize = reader.ReadInt64(),
                NonBufferDataSize = reader.ReadInt64(),
                BufferDataSize = reader.ReadInt64(),
                NumVertexBuffers = reader.ReadInt32(),
                NumIndexBuffers = reader.ReadInt32(),
                NumMeshes = reader.ReadInt32(),
                NumTotalSubsets = reader.ReadInt32(),
                NumFrames = reader.ReadInt32(),
                NumMaterials = reader.ReadInt32(),
                VertexStreamHeadersOffset = reader.ReadInt64(),
                IndexStreamHeadersOffset = reader.ReadInt64(),
                MeshDataOffset = reader.ReadInt64(),
                SubsetDataOffset = reader.ReadInt64(),
                FrameDataOffset = reader.ReadInt64(),
                MaterialDataOffset = reader.ReadInt64()
            };

            return header;
        }
    }
}
