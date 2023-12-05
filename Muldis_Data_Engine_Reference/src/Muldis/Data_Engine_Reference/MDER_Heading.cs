namespace Muldis.Data_Engine_Reference;

internal class MDER_Heading : MDER_Any
{
    internal readonly HashSet<String> attr_names;

    internal MDER_Heading(Internal_Memory memory, HashSet<String> attr_names)
        : base(memory, Internal_Well_Known_Base_Type.MDER_Heading)
    {
        this.attr_names = attr_names;
    }
}
