using JeremyAnsel.DirectX.D3D11;
using JeremyAnsel.DirectX.Dxgi;
using JeremyAnsel.DirectX.DXMath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshFile
    {
        private D3D11Device _d3dDevice;

        private D3D11DeviceContext _d3dDeviceContext;

        public string FilePath { get; private set; }

        public string FileDirectory { get; private set; }

        public IList<SdkMeshMaterial> Materials { get; } = new List<SdkMeshMaterial>();

        public IList<SdkMeshMesh> Meshes { get; } = new List<SdkMeshMesh>();

        public IList<SdkMeshFrame> Frames { get; } = new List<SdkMeshFrame>();

        public SdkMeshFrameTransformType AnimationFrameTransformType { get; private set; }

        public int AnimationKeysCount { get; private set; }

        public int AnimationFPS { get; private set; }

        public IList<SdkMeshAnimationFrame> AnimationFrames { get; } = new List<SdkMeshAnimationFrame>();

        public static SdkMeshFile FromFile(D3D11Device device, D3D11DeviceContext deviceContext, string fileName)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (deviceContext == null)
            {
                throw new ArgumentNullException(nameof(deviceContext));
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var file = new SdkMeshFile
            {
                _d3dDevice = device,
                _d3dDeviceContext = deviceContext,
                FilePath = fileName,
                FileDirectory = Path.GetDirectoryName(fileName)
            };

            SdkMeshRawFile rawFile = SdkMeshRawFile.FromFile(file.FilePath);

            foreach (SdkMeshRawMaterial rawMaterial in rawFile.Materials)
            {
                var material = new SdkMeshMaterial(file._d3dDevice, file._d3dDeviceContext, file.FileDirectory, rawMaterial);
                file.Materials.Add(material);
            }

            foreach (SdkMeshRawMesh rawMesh in rawFile.Meshes)
            {
                var mesh = new SdkMeshMesh(file._d3dDevice, rawFile, rawMesh);
                file.Meshes.Add(mesh);
            }

            foreach (SdkMeshRawFrame rawFrame in rawFile.Frames)
            {
                var frame = new SdkMeshFrame(rawFrame);
                file.Frames.Add(frame);
            }

            string animFilePath = file.FilePath + "_anim";
            if (File.Exists(animFilePath))
            {
                SdkMeshRawAnimFile rawAnimFile = SdkMeshRawAnimFile.FromFile(animFilePath);

                file.AnimationFrameTransformType = rawAnimFile.Header.FrameTransformType;
                file.AnimationKeysCount = rawAnimFile.Header.NumAnimationKeys;
                file.AnimationFPS = rawAnimFile.Header.AnimationFPS;

                foreach (SdkMeshRawAnimFrameData rawFrame in rawAnimFile.AnimationFrames)
                {
                    var frame = new SdkMeshAnimationFrame(rawFrame);
                    file.AnimationFrames.Add(frame);
                }

                for (int index = 0; index < file.AnimationFrames.Count; index++)
                {
                    SdkMeshFrame frame = file.FindFrame(file.AnimationFrames[index].FrameName);

                    if (frame != null)
                    {
                        frame.UpdateAnimationFrameIndex(index);
                    }
                }
            }

            return file;
        }

        public void Release()
        {
            foreach (SdkMeshMaterial material in this.Materials)
            {
                material.Release();
            }

            this.Materials.Clear();

            foreach (SdkMeshMesh mesh in this.Meshes)
            {
                mesh.Release();
            }

            this.Meshes.Clear();
        }

        public SdkMeshFrame FindFrame(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (SdkMeshFrame frame in this.Frames)
            {
                if (string.Equals(frame.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return frame;
                }
            }

            return null;
        }

        public XMMatrix GetMeshInfluenceMatrix(int meshIndex, int influanceIndex)
        {
            int frameIndex = this.Meshes[meshIndex].FrameInfluencesIndices[influanceIndex];
            return this.Frames[frameIndex].TransformedFrameMatrix;
        }

        public XMMatrix GetWorldMatrix(int frameIndex)
        {
            return this.Frames[frameIndex].WorldPoseFrameMatrix;
        }

        public XMMatrix GetInfluenceMatrix(int frameIndex)
        {
            return this.Frames[frameIndex].TransformedFrameMatrix;
        }

        public int GetAnimationKeyFromTime(double time)
        {
            if (this.AnimationKeysCount == 0)
            {
                return 0;
            }

            int tick = ((int)(this.AnimationFPS * time) % (this.AnimationKeysCount - 1)) + 1;

            return tick;
        }

        public bool GetAnimationProperties(out int numKeys, out float frameTime)
        {
            if (this.AnimationKeysCount == 0)
            {
                numKeys = 0;
                frameTime = 0;
                return false;
            }

            numKeys = this.AnimationKeysCount;
            frameTime = 1.0f / AnimationFPS;

            return true;
        }

        public void TransformBindPose(XMMatrix world)
        {
            if (this.Frames.Count >= 1)
            {
                this.Frames[0].TransformBindPoseFrame(this, world);
            }
        }

        public void TransformMesh(XMMatrix world, double time)
        {
            if (this.AnimationKeysCount == 0)
            {
                return;
            }

            if (this.AnimationFrameTransformType == SdkMeshFrameTransformType.Relative)
            {
                if (this.Frames.Count >= 1)
                {
                    this.Frames[0].TransformFrame(this, world, time);
                }

                // For each frame, move the transform to the bind pose, then
                // move it to the final position
                foreach (SdkMeshFrame frame in this.Frames)
                {
                    XMMatrix m;
                    m = frame.BindPoseFrameMatrix;
                    XMMatrix mInvBindPose = m.Inverse();
                    m = frame.TransformedFrameMatrix;
                    XMMatrix mFinal = mInvBindPose * m;
                    frame.TransformedFrameMatrix = mFinal;
                }
            }
            else if (this.AnimationFrameTransformType == SdkMeshFrameTransformType.Absolute)
            {
                foreach (SdkMeshFrame frame in this.Frames)
                {
                    frame.TransformFrameAbsolute(this, time);
                }
            }
        }

        private void RenderMesh(int meshIndex, int diffuseSlot, int normalSlot, int specularSlot)
        {
            if (meshIndex < 0 || meshIndex >= this.Meshes.Count)
            {
                return;
            }

            SdkMeshMesh mesh = this.Meshes[meshIndex];

            if (mesh.VertexBuffers.Length > D3D11Constants.InputAssemblerVertexInputResourceSlotCount)
            {
                return;
            }

            D3D11Buffer[] vb = new D3D11Buffer[mesh.VertexBuffers.Length];
            uint[] strides = new uint[mesh.VertexBuffers.Length];
            uint[] offsets = new uint[mesh.VertexBuffers.Length];

            for (int i = 0; i < mesh.VertexBuffers.Length; i++)
            {
                vb[i] = mesh.VertexBuffers[i].Buffer;
                strides[i] = mesh.VertexBuffers[i].StrideBytes;
                offsets[i] = 0;
            }

            D3D11Buffer ib = mesh.IndexBuffer.Buffer;
            DxgiFormat ibFormat = mesh.IndexBuffer.IndexFormat;

            this._d3dDeviceContext.InputAssemblerSetVertexBuffers(0, vb, strides, offsets);
            this._d3dDeviceContext.InputAssemblerSetIndexBuffer(ib, ibFormat, 0);


            foreach (SdkMeshSubset subset in mesh.Subsets)
            {
                this._d3dDeviceContext.InputAssemblerSetPrimitiveTopology(subset.PrimitiveTopology);

                SdkMeshMaterial material = this.Materials[subset.MaterialIndex];

                if (diffuseSlot != -1 && material.DiffuseTextureView != null)
                {
                    this._d3dDeviceContext.PixelShaderSetShaderResources((uint)diffuseSlot, new[] { material.DiffuseTextureView });
                }

                if (normalSlot != -1 && material.NormalTextureView != null)
                {
                    this._d3dDeviceContext.PixelShaderSetShaderResources((uint)normalSlot, new[] { material.NormalTextureView });
                }

                if (specularSlot != -1 && material.SpecularTextureView != null)
                {
                    this._d3dDeviceContext.PixelShaderSetShaderResources((uint)specularSlot, new[] { material.SpecularTextureView });
                }

                this._d3dDeviceContext.DrawIndexed((uint)subset.IndexCount, (uint)subset.IndexStart, subset.VertexStart);
            }
        }

        private void RenderFrame(int frameIndex, int diffuseSlot, int normalSlot, int specularSlot)
        {
            if (frameIndex < 0 || frameIndex >= this.Frames.Count)
            {
                return;
            }

            SdkMeshFrame frame = this.Frames[frameIndex];

            if (frame.MeshIndex != -1)
            {
                this.RenderMesh(frame.MeshIndex, diffuseSlot, normalSlot, specularSlot);
            }

            // Render our children
            if (frame.ChildFrameIndex != -1)
            {
                this.RenderFrame(frame.ChildFrameIndex, diffuseSlot, normalSlot, specularSlot);
            }

            // Render our siblings
            if (frame.SiblingFrameIndex != -1)
            {
                this.RenderFrame(frame.SiblingFrameIndex, diffuseSlot, normalSlot, specularSlot);
            }
        }

        public void Render(int diffuseSlot, int normalSlot, int specularSlot)
        {
            this.RenderFrame(0, diffuseSlot, normalSlot, specularSlot);
        }
    }
}
