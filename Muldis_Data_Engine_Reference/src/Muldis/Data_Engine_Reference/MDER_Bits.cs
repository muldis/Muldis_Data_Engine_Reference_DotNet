using System.Collections;

namespace Muldis.Data_Engine_Reference;

public class MDER_Bits : MDER_Any
{
    private readonly BitArray __bit_members;

    internal MDER_Bits(MDER_Machine machine, BitArray bit_members)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Bits)
    {
        this.__bit_members = bit_members;
    }

    internal BitArray _bit_members()
    {
        return this.__bit_members;
    }

    internal String _as_MUON_Bits_artifact()
    {
        if (Object.ReferenceEquals(this, this.machine().MDER_Bits_C0))
        {
            return "0bb";
        }
        System.Collections.IEnumerator e = this.__bit_members.GetEnumerator();
        List<Boolean> list = new List<Boolean>();
        while (e.MoveNext())
        {
            list.Add((Boolean)e.Current);
        }
        return "0bb" + String.Concat(
                Enumerable.Select(list, m => m ? "1" : "0")
            );
    }

    internal Boolean _MDER_Bits__same(MDER_Bits value_1)
    {
        MDER_Bits value_0 = this;
        return Enumerable.SequenceEqual(
            _BitArray_to_List(value_0.__bit_members),
            _BitArray_to_List(value_1.__bit_members));
    }

    private List<Boolean> _BitArray_to_List(BitArray value)
    {
        System.Collections.IEnumerator e = value.GetEnumerator();
        List<Boolean> list = new List<Boolean>();
        while (e.MoveNext())
        {
            list.Add((Boolean)e.Current);
        }
        return list;
    }
}
