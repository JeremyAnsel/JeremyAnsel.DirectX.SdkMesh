using JeremyAnsel.DirectX.DXMath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshRawMesh
    {
        private const int MaxMeshName = 100;

        private const int MaxVertexStreams = 16;

        public string? Name { get; private set; }

        public byte NumVertexBuffers { get; private set; }

        public IList<int> VertexBuffers { get; } = new List<int>();

        public int IndexBuffer { get; private set; }

        public int NumSubsets { get; private set; }

        public int NumFrameInfluences { get; private set; }

        public XMFloat3 BoundingBoxCenter { get; private set; }

        public XMFloat3 BoundingBoxExtents { get; private set; }

        public long SubsetOffset { get; private set; }

        public long FrameInfluenceOffset { get; private set; }

        public IList<int> SubsetsIndices { get; } = new List<int>();

        public IList<int> FrameInfluencesIndices { get; } = new List<int>();

        internal static SdkMeshRawMesh Read(BinaryReader reader)
        {
            var mesh = new SdkMeshRawMesh
            {
                Name = Encoding.UTF8.GetString(reader.ReadBytes(MaxMeshName).TakeWhile(t => t != 0).ToArray()),
                NumVertexBuffers = (byte)reader.ReadInt32()
            };

            for (int i = 0; i < mesh.NumVertexBuffers; i++)
            {
                mesh.VertexBuffers.Add(reader.ReadInt32());
            }

            for (int i = mesh.NumVertexBuffers; i < MaxVertexStreams; i++)
            {
                reader.ReadInt32();
            }

            mesh.IndexBuffer = reader.ReadInt32();
            mesh.NumSubsets = reader.ReadInt32();
            mesh.NumFrameInfluences = reader.ReadInt32();

            mesh.BoundingBoxCenter = new XMFloat3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            mesh.BoundingBoxExtents = new XMFloat3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            reader.ReadInt32();

            mesh.SubsetOffset = reader.ReadInt64();
            mesh.FrameInfluenceOffset = reader.ReadInt64();

            return mesh;
        }
    }
}
