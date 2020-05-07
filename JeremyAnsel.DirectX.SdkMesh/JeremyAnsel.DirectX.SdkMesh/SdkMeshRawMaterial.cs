using JeremyAnsel.DirectX.DXMath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshRawMaterial
    {
        private const int MaxMaterialName = 100;

        private const int MaxMaterialPath = 260;

        private const int MaxTextureName = 260;

        public string Name { get; private set; }

        public string MaterialInstancePath { get; private set; }

        public string DiffuseTexture { get; private set; }

        public string NormalTexture { get; private set; }

        public string SpecularTexture { get; private set; }

        public XMFloat4 Diffuse { get; private set; }

        public XMFloat4 Ambient { get; private set; }

        public XMFloat4 Specular { get; private set; }

        public XMFloat4 Emissive { get; private set; }

        public float Power { get; private set; }

        public long DiffuseTextureOffset { get; private set; }

        public long NormalTextureOffset { get; private set; }

        public long SpecularTextureOffset { get; private set; }

        public long DiffuseTextureViewOffset { get; private set; }

        public long NormalTextureViewOffset { get; private set; }

        public long SpecularTextureViewOffset { get; private set; }

        internal static SdkMeshRawMaterial Read(BinaryReader reader)
        {
            var material = new SdkMeshRawMaterial
            {
                Name = Encoding.UTF8.GetString(reader.ReadBytes(MaxMaterialName).TakeWhile(t => t != 0).ToArray()),
                MaterialInstancePath = Encoding.UTF8.GetString(reader.ReadBytes(MaxMaterialPath).TakeWhile(t => t != 0).ToArray()),
                DiffuseTexture = Encoding.UTF8.GetString(reader.ReadBytes(MaxTextureName).TakeWhile(t => t != 0).ToArray()),
                NormalTexture = Encoding.UTF8.GetString(reader.ReadBytes(MaxTextureName).TakeWhile(t => t != 0).ToArray()),
                SpecularTexture = Encoding.UTF8.GetString(reader.ReadBytes(MaxTextureName).TakeWhile(t => t != 0).ToArray()),
                Diffuse = new XMFloat4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                Ambient = new XMFloat4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                Specular = new XMFloat4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                Emissive = new XMFloat4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
                Power = reader.ReadSingle(),
                DiffuseTextureOffset = reader.ReadInt64(),
                NormalTextureOffset = reader.ReadInt64(),
                SpecularTextureOffset = reader.ReadInt64(),
                DiffuseTextureViewOffset = reader.ReadInt64(),
                NormalTextureViewOffset = reader.ReadInt64(),
                SpecularTextureViewOffset = reader.ReadInt64()
            };

            return material;
        }
    }
}
