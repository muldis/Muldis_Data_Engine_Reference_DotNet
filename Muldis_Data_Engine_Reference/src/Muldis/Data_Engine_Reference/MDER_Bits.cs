using System.Collections;

namespace Muldis.Data_Engine_Reference;

internal class MDL_Bits : MDL_Any
{
    internal readonly BitArray bit_members;

    internal MDL_Bits(Memory memory, BitArray bit_members)
        : base(memory, Well_Known_Base_Type.MDL_Bits)
    {
        this.bit_members = bit_members;
    }
}
