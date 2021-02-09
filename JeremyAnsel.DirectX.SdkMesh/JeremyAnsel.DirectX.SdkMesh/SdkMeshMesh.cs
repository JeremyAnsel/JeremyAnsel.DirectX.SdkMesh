using JeremyAnsel.DirectX.D3D11;
using JeremyAnsel.DirectX.DXMath;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshMesh
    {
        internal SdkMeshMesh(D3D11Device device, SdkMeshRawFile rawFile, SdkMeshRawMesh rawMesh)
        {
            this.Name = rawMesh.Name;
            this.ComputeBoundingBox(rawFile, rawMesh);

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

        private void ComputeBoundingBox(SdkMeshRawFile rawFile, SdkMeshRawMesh rawMesh)
        {
            //this.BoundingBoxCenter = rawMesh.BoundingBoxCenter;
            //this.BoundingBoxExtents = rawMesh.BoundingBoxExtents;

            XMVector lower = XMVector.Replicate(float.MaxValue);
            XMVector upper = XMVector.Replicate(float.MinValue);

            int indsize;
            if (rawFile.IndexBufferHeaders[rawMesh.IndexBuffer].IndexType == SdkMeshIndexType.IndexType32Bit)
            {
                indsize = 4;
            }
            else
            {
                indsize = 2;
            }

            for (int subset = 0; subset < rawMesh.NumSubsets; subset++)
            {
                SdkMeshRawSubset pSubset = rawFile.Subsets[rawMesh.SubsetsIndices[subset]];

                SdkMeshPrimitiveType primType = pSubset.PrimitiveType;
                Debug.Assert(primType == SdkMeshPrimitiveType.TriangleList, "Only triangle lists are handled.");

                if (primType != SdkMeshPrimitiveType.TriangleList)
                {
                    continue;
                }

                int indexCount = (int)pSubset.IndexCount;
                int indexStart = (int)pSubset.IndexStart;
                byte[] ind = rawFile.IndexBufferBytes[rawMesh.IndexBuffer];
                byte[] verts = rawFile.VertexBufferBytes[rawMesh.VertexBuffers[0]];
                int stride = (int)rawFile.VertexBufferHeaders[rawMesh.VertexBuffers[0]].StrideBytes;

                for (int vertind = indexStart; vertind < indexStart + indexCount; vertind++)
                {
                    int current_ind;

                    if (indsize == 2)
                    {
                        current_ind = BitConverter.ToInt16(ind, vertind * 2);
                    }
                    else
                    {
                        current_ind = BitConverter.ToInt32(ind, vertind * 4);
                    }

                    float x = BitConverter.ToSingle(verts, stride * current_ind);
                    float y = BitConverter.ToSingle(verts, stride * current_ind + 4);
                    float z = BitConverter.ToSingle(verts, stride * current_ind + 8);

                    lower = XMVector.Min(new XMVector(x, y, z, 1.0f), lower);
                    upper = XMVector.Max(new XMVector(x, y, z, 1.0f), upper);
                }
            }

            XMVector half = upper - lower;
            half *= 0.5f;

            this.BoundingBoxCenter = lower + half;
            this.BoundingBoxExtents = half;
        }
    }
}
