using System;
using System.Collections.Generic;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    public enum D3DDeclMethod
    {
        Default = 0,
        PartialU = 1,
        PartialV = 2,
        CrossUV = 3,
        UV = 4,
        Lookup = 5,
        LookupPreSampled = 6
    }
}
