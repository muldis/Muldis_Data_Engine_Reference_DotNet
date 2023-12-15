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

    internal Boolean _MDER_Bits__same(MDER_Bits topic_1)
    {
        MDER_Bits topic_0 = this;
        return Enumerable.SequenceEqual(
            _BitArray_to_List(topic_0.__bit_members),
            _BitArray_to_List(topic_1.__bit_members));
    }

    private List<Boolean> _BitArray_to_List(BitArray topic)
    {
        System.Collections.IEnumerator e = topic.GetEnumerator();
        List<Boolean> list = new List<Boolean>();
        while (e.MoveNext())
        {
            list.Add((Boolean)e.Current);
        }
        return list;
    }

    internal Int64 Bits__count()
    {
        return this.__bit_members.Length;
    }

    internal MDER_Integer? Bits__maybe_at(Int64 ord_pos)
    {
        return (ord_pos >= this.__bit_members.Length) ? null
            : this.machine().MDER_Integer(this.__bit_members[(Int32)ord_pos] ? 1 : 0);
    }
}
