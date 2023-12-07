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

    internal String _as_MUON_Bits_artifact()
    {
        if (Object.ReferenceEquals(this, this.machine().MDER_Bits_C0))
        {
            return "0bb";
        }
        System.Collections.IEnumerator e = this.bit_members.GetEnumerator();
        List<Boolean> list = new List<Boolean>();
        while (e.MoveNext())
        {
            list.Add((Boolean)e.Current);
        }
        return "0bb" + String.Concat(
                Enumerable.Select(list, m => m ? "1" : "0")
            );
    }
}
