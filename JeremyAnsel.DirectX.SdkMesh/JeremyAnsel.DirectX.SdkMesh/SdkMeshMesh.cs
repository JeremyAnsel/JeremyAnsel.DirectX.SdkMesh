using JeremyAnsel.DirectX.D3D11;
using JeremyAnsel.DirectX.DXMath;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshMesh
    {
        internal SdkMeshMesh(D3D11Device device, SdkMeshRawFile rawFile, SdkMeshRawMesh rawMesh)
        {
            this.Name = rawMesh.Name;
            this.BoundingBoxCenter = rawMesh.BoundingBoxCenter;
            this.BoundingBoxExtents = rawMesh.BoundingBoxExtents;

            this.VertexBuffers = new SdkMeshVertexBuffer[rawMesh.NumVertexBuffers];

            for (int i = 0; i < rawMesh.NumVertexBuffers; i++)
            {
                this.VertexBuffers[i] = new SdkMeshVertexBuffer(device, rawFile, rawMesh, i);
            }

            this.IndexBuffer = new SdkMeshIndexBuffer(device, rawFile, rawMesh);

            foreach (int index in rawMesh.SubsetsIndices)
            {
                SdkMeshSubset subset = new SdkMeshSubset(rawFile.Subsets[index]);
                this.Subsets.Add(subset);
            }

            foreach (int index in rawMesh.FrameInfluencesIndices)
            {
                this.FrameInfluencesIndices.Add(index);
            }
        }

        public string Name { get; private set; }

        public XMFloat3 BoundingBoxCenter { get; private set; }

        public XMFloat3 BoundingBoxExtents { get; private set; }

        [SuppressMessage("Performance", "CA1819:Les propriétés ne doivent pas retourner de tableaux", Justification = "Reviewed.")]
        public SdkMeshVertexBuffer[] VertexBuffers { get; private set; }

        public SdkMeshIndexBuffer IndexBuffer { get; private set; }

        public IList<SdkMeshSubset> Subsets { get; } = new List<SdkMeshSubset>();

        public IList<int> FrameInfluencesIndices { get; } = new List<int>();

        public void Release()
        {
            if (this.VertexBuffers != null)
            {
                for (int i = 0; i < this.VertexBuffers.Length; i++)
                {
                    this.VertexBuffers[i]?.Release();
                    this.VertexBuffers[i] = null;
                }

                this.VertexBuffers = null;
            }

            this.IndexBuffer?.Release();
            this.IndexBuffer = null;
        }
    }
}
