namespace Muldis.Data_Engine_Reference;

public class MDER_Blob : MDER_Any
{
    internal readonly Byte[] octet_members;

    internal MDER_Blob(MDER_Machine machine, Byte[] octet_members)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Blob)
    {
        this.octet_members = octet_members;
    }
}
