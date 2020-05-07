using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JeremyAnsel.DirectX.SdkMesh
{
    [SuppressMessage("Naming", "CA1707:Les identificateurs ne doivent pas contenir de traits de soulignement", Justification = "Reviewed.")]
    public enum D3DDeclType
    {
        Float1 = 0,
        Float2 = 1,
        Float3 = 2,
        Float4 = 3,
        D3DColor = 4,
        UByte4 = 5,
        Short2 = 6,
        Short4 = 7,
        UByte4N = 8,
        Short2N = 9,
        Short4N = 10,
        UShort2N = 11,
        UShort4N = 12,
        UDec3 = 13,
        Dec3N = 14,
        Float16_2 = 15,
        Float16_4 = 16,
        Unused = 17
    }
}
