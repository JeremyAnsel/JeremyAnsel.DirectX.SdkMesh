using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshRawAnimFile
    {
        private const int FileVersion = 101;

        public SdkMeshRawAnimHeader Header { get; private set; }

        public IList<SdkMeshRawAnimFrameData> AnimationFrames { get; } = new List<SdkMeshRawAnimFrameData>();

        public static SdkMeshRawAnimFile FromFile(string fileName)
        {
            using (FileStream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return FromStream(stream);
            }
        }

        public static SdkMeshRawAnimFile FromStream(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var file = new SdkMeshRawAnimFile();

            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                file.Header = SdkMeshRawAnimHeader.Read(reader);

                if (file.Header.Version != FileVersion)
                {
                    throw new InvalidDataException();
                }

                if (stream.Position != file.Header.AnimationDataOffset)
                {
                    throw new InvalidDataException();
                }

                stream.Seek(file.Header.AnimationDataOffset, SeekOrigin.Begin);
                for (int frameIndex = 0; frameIndex < file.Header.NumFrames; frameIndex++)
                {
                    SdkMeshRawAnimFrameData frame = SdkMeshRawAnimFrameData.Read(reader);
                    file.AnimationFrames.Add(frame);
                }

                foreach (SdkMeshRawAnimFrameData frame in file.AnimationFrames)
                {
                    if (stream.Position != file.Header.AnimationDataOffset + frame.DataOffset)
                    {
                        throw new InvalidDataException();
                    }

                    stream.Seek(file.Header.AnimationDataOffset + frame.DataOffset, SeekOrigin.Begin);
                    for (int keyIndex = 0; keyIndex < file.Header.NumAnimationKeys; keyIndex++)
                    {
                        SdkMeshRawAnimData data = SdkMeshRawAnimData.Read(reader);
                        frame.AnimationKeys.Add(data);
                    }
                }

                if (stream.Position != file.Header.AnimationDataOffset + file.Header.AnimationDataSize)
                {
                    throw new InvalidDataException();
                }
            }

            return file;
        }
    }
}
