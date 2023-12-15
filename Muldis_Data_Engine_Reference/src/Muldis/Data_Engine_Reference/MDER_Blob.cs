namespace Muldis.Data_Engine_Reference;

public class MDER_Blob : MDER_Any
{
    private readonly Byte[] __octet_members;

    internal MDER_Blob(MDER_Machine machine, Byte[] octet_members)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Blob)
    {
        this.__octet_members = octet_members;
    }

    internal Byte[] _octet_members()
    {
        return this.__octet_members;
    }

    internal String _as_MUON_Blob_artifact()
    {
        if (Object.ReferenceEquals(this, this.machine().MDER_Blob_C0))
        {
            return "0xx";
        }
        return "0xx" + String.Concat(
                Enumerable.Select(this.__octet_members, m => m.ToString("X2"))
            );
    }

    internal Boolean _MDER_Blob__same(MDER_Blob topic_1)
    {
        MDER_Blob topic_0 = this;
        return Enumerable.SequenceEqual(topic_0.__octet_members,
            topic_1.__octet_members);
    }
}
