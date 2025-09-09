using JeremyAnsel.DirectX.D3D11;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshVertexBuffer
    {
        internal SdkMeshVertexBuffer(D3D11Device device, SdkMeshRawFile rawFile, SdkMeshRawMesh rawMesh, int i)
        {
            int index = rawMesh.VertexBuffers[i];
            SdkMeshRawVertexBufferHeader header = rawFile.VertexBufferHeaders[index];
            byte[] bytes = rawFile.VertexBufferBytes[index];

            this.NumVertices = (int)header.NumVertices;
            this.SizeBytes = (uint)header.SizeBytes;
            this.StrideBytes = (uint)header.StrideBytes;
            this.Decl = header.Decl.ToArray();

            var desc = new D3D11BufferDesc((uint)header.SizeBytes, D3D11BindOptions.VertexBuffer | D3D11BindOptions.ShaderResource);
            var data = new D3D11SubResourceData(bytes, 0, 0);
            this.Buffer = device.CreateBuffer(desc, data);
        }

        public int NumVertices { get; private set; }

        public uint SizeBytes { get; private set; }

        public uint StrideBytes { get; private set; }

        [SuppressMessage("Performance", "CA1819:Les propriétés ne doivent pas retourner de tableaux", Justification = "Reviewed.")]
        public D3DVertexElement[] Decl { get; private set; }

        public D3D11Buffer? Buffer { get; private set; }

        public void Release()
        {
            this.Buffer?.Release();
            this.Buffer = null;
        }
    }
}
