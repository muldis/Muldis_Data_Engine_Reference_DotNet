using System.Collections;

namespace Muldis.Data_Engine_Reference;

public class MDER_Bits : MDER_Any
{
    // A value of the .NET class BitArray is mutable.
    // It should be cloned as needed for protection of our internals.
    private readonly BitArray __bit_members_as_BitArray;

    internal MDER_Bits(MDER_Machine machine,
        BitArray bit_members_as_BitArray)
        : base(machine)
    {
        this.__bit_members_as_BitArray = bit_members_as_BitArray;
    }

    public BitArray bit_members_as_BitArray()
    {
        // A BitArray is mutable so clone to protect our internals.
        return new BitArray(this.__bit_members_as_BitArray);
    }

    internal BitArray _bit_members_as_BitArray()
    {
        return this.__bit_members_as_BitArray;
    }

    internal String _as_MUON_Bits_artifact()
    {
        return "0bb" + String.Concat(Enumerable.Select(
            this._BitArray_to_List(this.__bit_members_as_BitArray),
            m => m ? "1" : "0"));
    }

    internal Boolean _MDER_Bits__same(MDER_Bits topic_1)
    {
        MDER_Bits topic_0 = this;
        return Enumerable.SequenceEqual(
            this._BitArray_to_List(topic_0.__bit_members_as_BitArray),
            this._BitArray_to_List(topic_1.__bit_members_as_BitArray));
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

    internal Int32 _MDER_Bits__bit_count()
    {
        return this.__bit_members_as_BitArray.Length;
    }

    internal MDER_Integer? _MDER_Bits__bit_maybe_at(Int32 ord_pos)
    {
        return (ord_pos >= this.__bit_members_as_BitArray.Length) ? null
            : this.machine().MDER_Integer(
                this.__bit_members_as_BitArray[ord_pos] ? 1 : 0);
    }
}
