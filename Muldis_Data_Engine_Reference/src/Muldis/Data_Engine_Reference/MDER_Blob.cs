namespace Muldis.Data_Engine_Reference;

internal class MDER_Blob : MDER_Any
{
    internal readonly Byte[] octet_members;

    internal MDER_Blob(Internal_Memory memory, Byte[] octet_members)
        : base(memory, Internal_Well_Known_Base_Type.MDER_Blob)
    {
        this.octet_members = octet_members;
    }
}
