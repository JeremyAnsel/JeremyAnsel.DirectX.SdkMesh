using JeremyAnsel.DirectX.DXMath;
using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public sealed class SdkMeshAnimationKey
    {
        internal SdkMeshAnimationKey(SdkMeshRawAnimData rawData)
        {
            this.Translation = rawData.Translation;
            this.Orientation = rawData.Orientation;
            this.Scaling = rawData.Scaling;
        }

        public XMFloat3 Translation { get; private set; }

        public XMFloat4 Orientation { get; private set; }

        public XMFloat3 Scaling { get; private set; }
    }
}
