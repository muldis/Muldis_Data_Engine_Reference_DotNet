namespace Muldis.Data_Engine_Reference.Internal;

internal class MDL_Heading : MDL_NQA
{
    internal readonly HashSet<String> attr_names;

    internal MDL_Heading(Memory memory, HashSet<String> attr_names)
        : base(memory, Well_Known_Base_Type.MDL_Heading)
    {
        this.attr_names = attr_names;
    }
}
