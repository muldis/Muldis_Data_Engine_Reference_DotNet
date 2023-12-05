using System.Collections;

namespace Muldis.Data_Engine_Reference;

internal class MDER_Bits : MDER_Any
{
    internal readonly BitArray bit_members;

    internal MDER_Bits(Internal_Memory memory, BitArray bit_members)
        : base(memory, Internal_Well_Known_Base_Type.MDER_Bits)
    {
        this.bit_members = bit_members;
    }
}
