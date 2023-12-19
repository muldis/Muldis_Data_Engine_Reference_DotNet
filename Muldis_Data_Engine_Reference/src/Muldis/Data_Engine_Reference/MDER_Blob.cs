namespace Muldis.Data_Engine_Reference;

public sealed class MDER_Blob : MDER_Any
{
    // The MDER_Machine VM that this MDER_Any "value" lives in.
    private readonly MDER_Machine __machine;

    // Surrogate identity for this MDER_Any with a simpler representation.
    private String? __cached_identity_as_String;

    // A value of the .NET array class Byte[] is mutable.
    // It should be cloned as needed for protection of our internals.
    private readonly Byte[] __octet_members_as_array_of_Byte;

    internal MDER_Blob(MDER_Machine machine,
        Byte[] octet_members_as_array_of_Byte)
    {
        this.__machine = machine;
        this.__octet_members_as_array_of_Byte
            = octet_members_as_array_of_Byte;
    }

    public override MDER_Machine machine()
    {
        return this.__machine;
    }

    internal override String _identity_as_String()
    {
        if (this.__cached_identity_as_String is null)
        {
            this.__cached_identity_as_String
                = this._as_MUON_Plain_Text_artifact("");
        }
        return this.__cached_identity_as_String;
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

    internal override String _as_MUON_Plain_Text_artifact(String indent)
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
