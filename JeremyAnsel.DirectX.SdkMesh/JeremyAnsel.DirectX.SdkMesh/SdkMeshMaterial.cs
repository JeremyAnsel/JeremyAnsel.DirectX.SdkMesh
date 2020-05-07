﻿using JeremyAnsel.DirectX.D3D11;
using JeremyAnsel.DirectX.Dds;
using JeremyAnsel.DirectX.Dxgi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshMaterial
    {
        internal SdkMeshMaterial(D3D11Device device, D3D11DeviceContext deviceContext, string directory, SdkMeshRawMaterial rawMaterial)
        {
            this.Name = rawMaterial.Name;

            if (!string.IsNullOrEmpty(rawMaterial.DiffuseTexture))
            {
                this.DiffuseTextureName = Path.GetFileName(rawMaterial.DiffuseTexture);
                CreateTexture(device, deviceContext, Path.Combine(directory, Path.GetFileName(rawMaterial.DiffuseTexture)), out D3D11ShaderResourceView textureView);
                this.DiffuseTextureView = textureView;
            }

            if (!string.IsNullOrEmpty(rawMaterial.NormalTexture))
            {
                this.NormalTextureName = Path.GetFileName(rawMaterial.NormalTexture);
                CreateTexture(device, deviceContext, Path.Combine(directory, Path.GetFileName(rawMaterial.NormalTexture)), out D3D11ShaderResourceView textureView);
                this.NormalTextureView = textureView;
            }

            if (!string.IsNullOrEmpty(rawMaterial.SpecularTexture))
            {
                this.SpecularTextureName = Path.GetFileName(rawMaterial.SpecularTexture);
                CreateTexture(device, deviceContext, Path.Combine(directory, Path.GetFileName(rawMaterial.SpecularTexture)), out D3D11ShaderResourceView textureView);
                this.SpecularTextureView = textureView;
            }
        }

        public string Name { get; private set; }

        public string DiffuseTextureName { get; private set; }

        public string NormalTextureName { get; private set; }

        public string SpecularTextureName { get; private set; }

        public D3D11ShaderResourceView DiffuseTextureView { get; private set; }

        public D3D11ShaderResourceView NormalTextureView { get; private set; }

        public D3D11ShaderResourceView SpecularTextureView { get; private set; }

        public void Release()
        {
            this.DiffuseTextureView?.Dispose();
            this.DiffuseTextureView = null;

            this.NormalTextureView?.Dispose();
            this.NormalTextureView = null;

            this.SpecularTextureView?.Dispose();
            this.SpecularTextureView = null;
        }

        private static void CreateTexture(D3D11Device device, D3D11DeviceContext deviceContext, string fileName, out D3D11ShaderResourceView textureView)
        {
            if (!File.Exists(fileName))
            {
                textureView = null;
                return;
            }

            string ext = Path.GetExtension(fileName);

            if (string.Equals(ext, ".dds", StringComparison.OrdinalIgnoreCase))
            {
                DdsDirectX.CreateTexture(fileName, device, deviceContext, out textureView);
            }
            else if (string.Equals(ext, ".jpg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(ext, ".bmp", StringComparison.OrdinalIgnoreCase)
                || string.Equals(ext, ".png", StringComparison.OrdinalIgnoreCase)
                || string.Equals(ext, ".gif", StringComparison.OrdinalIgnoreCase))
            {
                CreateBitmapTexture(device, fileName, out textureView);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private static void CreateBitmapTexture(D3D11Device device, string fileName, out D3D11ShaderResourceView textureView)
        {
            int width;
            int height;
            byte[] bytes;

            using (var file = new Bitmap(fileName))
            {
                var rect = new Rectangle(0, 0, file.Width, file.Height);
                int length = file.Width * file.Height;

                width = file.Width;
                height = file.Height;
                bytes = new byte[length * 4];

                using (var bitmap = file.Clone(rect, PixelFormat.Format32bppArgb))
                {
                    BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);

                    try
                    {
                        Marshal.Copy(data.Scan0, bytes, 0, length * 4);
                    }
                    finally
                    {
                        bitmap.UnlockBits(data);
                    }
                }
            }

            D3D11SubResourceData[] textureSubResData = new[]
            {
                new D3D11SubResourceData(bytes, (uint)width * 4)
            };

            var textureDesc = new D3D11Texture2DDesc(DxgiFormat.B8G8R8A8UNorm, (uint)width, (uint)height, 1, 1);

            using (var texture = device.CreateTexture2D(textureDesc, textureSubResData))
            {
                var textureViewDesc = new D3D11ShaderResourceViewDesc
                {
                    Format = textureDesc.Format,
                    ViewDimension = D3D11SrvDimension.Texture2D,
                    Texture2D = new D3D11Texture2DSrv
                    {
                        MipLevels = textureDesc.MipLevels,
                        MostDetailedMip = 0
                    }
                };

                textureView = device.CreateShaderResourceView(texture, textureViewDesc);
            }
        }
    }
}
