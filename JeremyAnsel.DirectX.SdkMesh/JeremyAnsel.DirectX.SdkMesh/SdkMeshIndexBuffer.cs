using JeremyAnsel.DirectX.D3D11;
using JeremyAnsel.DirectX.Dxgi;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshIndexBuffer
    {
        internal SdkMeshIndexBuffer(D3D11Device device, SdkMeshRawFile rawFile, SdkMeshRawMesh rawMesh)
        {
            int index = rawMesh.IndexBuffer;
            SdkMeshRawIndexBufferHeader header = rawFile.IndexBufferHeaders[index];
            byte[] bytes = rawFile.IndexBufferBytes[index];

            this.NumIndices = (int)header.NumIndices;
            this.SizeBytes = (uint)header.SizeBytes;

            switch (header.IndexType)
            {
                case SdkMeshIndexType.IndexType16Bit:
                    this.IndexFormat = DxgiFormat.R16UInt;
                    break;

                case SdkMeshIndexType.IndexType32Bit:
                    this.IndexFormat = DxgiFormat.R32UInt;
                    break;

                default:
                    this.IndexFormat = DxgiFormat.R16UInt;
                    break;
            }

            var desc = new D3D11BufferDesc((uint)header.SizeBytes, D3D11BindOptions.IndexBuffer);
            var data = new D3D11SubResourceData(bytes, 0, 0);
            this.Buffer = device.CreateBuffer(desc, data);
        }

        public int NumIndices { get; private set; }

        public uint SizeBytes { get; private set; }

        public DxgiFormat IndexFormat { get; private set; }

        public D3D11Buffer Buffer { get; private set; }

        public void Release()
        {
            this.Buffer.Release();
            this.Buffer = null;
        }
    }
}
