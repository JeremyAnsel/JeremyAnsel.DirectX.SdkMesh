using JeremyAnsel.DirectX.D3D11;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshSubset
    {
        internal SdkMeshSubset(SdkMeshRawSubset rawSubset)
        {
            this.Name = rawSubset.Name;
            this.MaterialIndex = rawSubset.MaterialIndex;
            this.PrimitiveTopology = GetPrimitiveTopology(rawSubset.PrimitiveType);
            this.IndexStart = (int)rawSubset.IndexStart;
            this.IndexCount = (int)rawSubset.IndexCount;
            this.VertexStart = (int)rawSubset.VertexStart;
            this.VertexCount = (int)rawSubset.VertexCount;
        }

        public string Name { get; private set; }

        public int MaterialIndex { get; private set; }

        public D3D11PrimitiveTopology PrimitiveTopology { get; private set; }

        public int IndexStart { get; private set; }

        public int IndexCount { get; private set; }

        public int VertexStart { get; private set; }

        public int VertexCount { get; private set; }

        private static D3D11PrimitiveTopology GetPrimitiveTopology(SdkMeshPrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case SdkMeshPrimitiveType.TriangleList:
                    return D3D11PrimitiveTopology.TriangleList;

                case SdkMeshPrimitiveType.TriangleStrip:
                    return D3D11PrimitiveTopology.TriangleStrip;

                case SdkMeshPrimitiveType.LineList:
                    return D3D11PrimitiveTopology.LineList;

                case SdkMeshPrimitiveType.LineStrip:
                    return D3D11PrimitiveTopology.LineStrip;

                case SdkMeshPrimitiveType.PointList:
                    return D3D11PrimitiveTopology.PointList;

                case SdkMeshPrimitiveType.TriangleListAdj:
                    return D3D11PrimitiveTopology.TriangleListAdj;

                case SdkMeshPrimitiveType.TriangleStripAdj:
                    return D3D11PrimitiveTopology.TriangleStripAdj;

                case SdkMeshPrimitiveType.LineListAdj:
                    return D3D11PrimitiveTopology.LineListAdj;

                case SdkMeshPrimitiveType.LineStripAdj:
                    return D3D11PrimitiveTopology.LineStripAdj;

                case SdkMeshPrimitiveType.QuadPatchList:
                    return D3D11PrimitiveTopology.TriangleList;

                case SdkMeshPrimitiveType.TrianglePatchList:
                    return D3D11PrimitiveTopology.TriangleList;

                default:
                    return D3D11PrimitiveTopology.TriangleList;
            }
        }
    }
}
