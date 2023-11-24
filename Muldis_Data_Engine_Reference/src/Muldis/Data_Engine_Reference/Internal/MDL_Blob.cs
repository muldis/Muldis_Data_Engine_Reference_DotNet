namespace Muldis.Data_Engine_Reference.Internal;

internal class MDL_Blob : MDL_NQA
{
    internal readonly Byte[] octet_members;

    internal MDL_Blob(Memory memory, Byte[] octet_members)
        : base(memory, Well_Known_Base_Type.MDL_Blob)
    {
        this.octet_members = octet_members;
    }
}
