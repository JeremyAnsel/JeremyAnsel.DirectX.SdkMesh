using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshRawSubset
    {
        private const int MaxSubsetName = 100;

        public string Name { get; private set; }

        public int MaterialIndex { get; private set; }

        public SdkMeshPrimitiveType PrimitiveType { get; private set; }

        public long IndexStart { get; private set; }

        public long IndexCount { get; private set; }

        public long VertexStart { get; private set; }

        public long VertexCount { get; private set; }

        internal static SdkMeshRawSubset Read(BinaryReader reader)
        {
            var subset = new SdkMeshRawSubset
            {
                Name = Encoding.UTF8.GetString(reader.ReadBytes(MaxSubsetName).TakeWhile(t => t != 0).ToArray()),
                MaterialIndex = reader.ReadInt32(),
                PrimitiveType = (SdkMeshPrimitiveType)reader.ReadInt64(),
                IndexStart = reader.ReadInt64(),
                IndexCount = reader.ReadInt64(),
                VertexStart = reader.ReadInt64(),
                VertexCount = reader.ReadInt64()
            };

            return subset;
        }
    }
}
