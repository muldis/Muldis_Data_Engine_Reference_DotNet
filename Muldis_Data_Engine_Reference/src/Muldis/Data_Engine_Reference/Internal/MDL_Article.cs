namespace Muldis.Data_Engine_Reference.Internal;

internal class MDL_Article : MDL_NQA
{
    // The Muldis Data Language value that is the "label" of this MDL_Article value.
    // TODO: Change this to a MDL_Nesting.
    internal readonly MDL_Any label;

    // The Muldis Data Language value that is the "attributes" of this MDL_Article value.
    internal readonly MDL_Tuple attrs;

    internal MDL_Article(Memory memory, MDL_Any label, MDL_Tuple attrs)
        : base(memory, Well_Known_Base_Type.MDL_Article)
    {
        this.label = label;
        this.attrs = attrs;
    }
}
