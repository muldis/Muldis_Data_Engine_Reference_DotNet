using System.Collections;

namespace Muldis.Data_Engine_Reference;

public class MDER_Bits : MDER_Any
{
    internal readonly BitArray bit_members;

    internal MDER_Bits(MDER_Machine machine, BitArray bit_members)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Bits)
    {
        this.bit_members = bit_members;
    }
}
