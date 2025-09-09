using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshRawFile
    {
        private const int FileVersion = 101;

        public SdkMeshRawHeader? Header { get; private set; }

        public IList<SdkMeshRawVertexBufferHeader> VertexBufferHeaders { get; } = new List<SdkMeshRawVertexBufferHeader>();

        public IList<byte[]> VertexBufferBytes { get; } = new List<byte[]>();

        public IList<SdkMeshRawIndexBufferHeader> IndexBufferHeaders { get; } = new List<SdkMeshRawIndexBufferHeader>();

        public IList<byte[]> IndexBufferBytes { get; } = new List<byte[]>();

        public IList<SdkMeshRawMesh> Meshes { get; } = new List<SdkMeshRawMesh>();

        public IList<SdkMeshRawSubset> Subsets { get; } = new List<SdkMeshRawSubset>();

        public IList<SdkMeshRawFrame> Frames { get; } = new List<SdkMeshRawFrame>();

        public IList<SdkMeshRawMaterial> Materials { get; } = new List<SdkMeshRawMaterial>();

        public static SdkMeshRawFile FromFile(string? fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return FromStream(stream);
            }
        }

        public static SdkMeshRawFile FromStream(Stream? stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var file = new SdkMeshRawFile();

            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                file.Header = SdkMeshRawHeader.Read(reader);

                if (file.Header.Version != FileVersion)
                {
                    throw new InvalidDataException();
                }

                if (stream.Position != file.Header.VertexStreamHeadersOffset)
                {
                    throw new InvalidDataException();
                }

                stream.Seek(file.Header.VertexStreamHeadersOffset, SeekOrigin.Begin);
                for (int i = 0; i < file.Header.NumVertexBuffers; i++)
                {
                    SdkMeshRawVertexBufferHeader header = SdkMeshRawVertexBufferHeader.Read(reader);
                    file.VertexBufferHeaders.Add(header);
                }

                if (stream.Position != file.Header.IndexStreamHeadersOffset)
                {
                    throw new InvalidDataException();
                }

                stream.Seek(file.Header.IndexStreamHeadersOffset, SeekOrigin.Begin);
                for (int i = 0; i < file.Header.NumIndexBuffers; i++)
                {
                    SdkMeshRawIndexBufferHeader header = SdkMeshRawIndexBufferHeader.Read(reader);
                    file.IndexBufferHeaders.Add(header);
                }

                if (stream.Position != file.Header.MeshDataOffset)
                {
                    throw new InvalidDataException();
                }

                stream.Seek(file.Header.MeshDataOffset, SeekOrigin.Begin);
                for (int i = 0; i < file.Header.NumMeshes; i++)
                {
                    SdkMeshRawMesh mesh = SdkMeshRawMesh.Read(reader);
                    file.Meshes.Add(mesh);
                }

                if (stream.Position != file.Header.SubsetDataOffset)
                {
                    throw new InvalidDataException();
                }

                stream.Seek(file.Header.SubsetDataOffset, SeekOrigin.Begin);
                for (int i = 0; i < file.Header.NumTotalSubsets; i++)
                {
                    SdkMeshRawSubset subset = SdkMeshRawSubset.Read(reader);
                    file.Subsets.Add(subset);
                }

                if (stream.Position != file.Header.FrameDataOffset)
                {
                    throw new InvalidDataException();
                }

                stream.Seek(file.Header.FrameDataOffset, SeekOrigin.Begin);
                for (int i = 0; i < file.Header.NumFrames; i++)
                {
                    SdkMeshRawFrame frame = SdkMeshRawFrame.Read(reader);
                    file.Frames.Add(frame);
                }

                if (stream.Position != file.Header.MaterialDataOffset)
                {
                    throw new InvalidDataException();
                }

                stream.Seek(file.Header.MaterialDataOffset, SeekOrigin.Begin);
                for (int i = 0; i < file.Header.NumMaterials; i++)
                {
                    SdkMeshRawMaterial material = SdkMeshRawMaterial.Read(reader);
                    file.Materials.Add(material);
                }

                foreach (SdkMeshRawMesh mesh in file.Meshes)
                {
                    if (stream.Position != mesh.SubsetOffset)
                    {
                        throw new InvalidDataException();
                    }

                    stream.Seek(mesh.SubsetOffset, SeekOrigin.Begin);
                    for (int i = 0; i < mesh.NumSubsets; i++)
                    {
                        int index = reader.ReadInt32();
                        mesh.SubsetsIndices.Add(index);
                    }

                    if (stream.Position != mesh.FrameInfluenceOffset)
                    {
                        throw new InvalidDataException();
                    }

                    stream.Seek(mesh.FrameInfluenceOffset, SeekOrigin.Begin);
                    for (int i = 0; i < mesh.NumFrameInfluences; i++)
                    {
                        int index = reader.ReadInt32();
                        mesh.FrameInfluencesIndices.Add(index);
                    }
                }

                if (stream.Position != file.Header.HeaderSize + file.Header.NonBufferDataSize)
                {
                    throw new InvalidDataException();
                }

                foreach (SdkMeshRawVertexBufferHeader buffer in file.VertexBufferHeaders)
                {
                    stream.Seek(buffer.DataOffset, SeekOrigin.Begin);

                    byte[] bytes = reader.ReadBytes((int)buffer.SizeBytes);
                    file.VertexBufferBytes.Add(bytes);
                }

                foreach (SdkMeshRawIndexBufferHeader buffer in file.IndexBufferHeaders)
                {
                    stream.Seek(buffer.DataOffset, SeekOrigin.Begin);

                    byte[] bytes = reader.ReadBytes((int)buffer.SizeBytes);
                    file.IndexBufferBytes.Add(bytes);
                }
            }

            return file;
        }
    }
}
