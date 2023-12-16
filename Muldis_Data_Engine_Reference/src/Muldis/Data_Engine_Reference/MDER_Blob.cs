namespace Muldis.Data_Engine_Reference;

public class MDER_Blob : MDER_Any
{
    // A value of the .NET array class Byte[] is mutable.
    // It should be cloned as needed for protection of our internals.
    private readonly Byte[] __octet_members_as_array_of_Byte;

    internal MDER_Blob(MDER_Machine machine,
        Byte[] octet_members_as_array_of_Byte)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Blob)
    {
        this.__octet_members_as_array_of_Byte
            = octet_members_as_array_of_Byte;
    }

    public Byte[] octet_members_as_array_of_Byte()
    {
        // An array is mutable so clone to protect our internals.
        return (Byte[])this.__octet_members_as_array_of_Byte.Clone();
    }

    internal Byte[] _octet_members_as_array_of_Byte()
    {
        return this.__octet_members_as_array_of_Byte;
    }

    internal String _as_MUON_Blob_artifact()
    {
        return "0xx" + String.Concat(Enumerable.Select(
            this.__octet_members_as_array_of_Byte, m => m.ToString("X2")));
    }

    internal Boolean _MDER_Blob__same(MDER_Blob topic_1)
    {
        MDER_Blob topic_0 = this;
        return Enumerable.SequenceEqual(
            topic_0.__octet_members_as_array_of_Byte,
            topic_1.__octet_members_as_array_of_Byte);
    }

    internal Int32 _MDER_Blob__octet_count()
    {
        return this.__octet_members_as_array_of_Byte.Length;
    }

    internal MDER_Integer? _MDER_Blob__octet_maybe_at(Int32 ord_pos)
    {
        return (ord_pos >= this.__octet_members_as_array_of_Byte.Length) ? null
            : this.machine().MDER_Integer(
                this.__octet_members_as_array_of_Byte[ord_pos]);
    }
}
