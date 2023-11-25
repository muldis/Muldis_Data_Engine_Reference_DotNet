namespace Muldis.Data_Engine_Reference.Internal;

internal class MDL_Excuse : MDL_NQA
{
    // The Muldis Data Language value that is the "label" of this MDL_Excuse value.
    // TODO: Change this to a MDL_Nesting.
    internal readonly MDL_Any label;

    // The Muldis Data Language value that is the "attributes" of this MDL_Excuse value.
    internal readonly MDL_Tuple attrs;

    internal MDL_Excuse(Memory memory, MDL_Any label, MDL_Tuple attrs)
        : base(memory, Well_Known_Base_Type.MDL_Excuse)
    {
        this.label = label;
        this.attrs = attrs;
    }
}
