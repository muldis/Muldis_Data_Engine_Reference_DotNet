namespace Muldis.Data_Engine_Reference;

public class MDER_Blob : MDER_Any
{
    internal readonly Byte[] octet_members;

    internal MDER_Blob(MDER_Machine machine, Byte[] octet_members)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Blob)
    {
        this.octet_members = octet_members;
    }

    internal String _as_MUON_Blob_artifact()
    {
        if (Object.ReferenceEquals(this, this.machine().MDER_Blob_C0))
        {
            return "0xx";
        }
        return "0xx" + String.Concat(
                Enumerable.Select(this.octet_members, m => m.ToString("X2"))
            );
    }
}
