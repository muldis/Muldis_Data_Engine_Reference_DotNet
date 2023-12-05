namespace Muldis.Data_Engine_Reference;

internal class MDER_Article : MDER_Any
{
    // The Muldis Data Language value that is the "label" of this MDER_Article value.
    // TODO: Change this to a MDER_Nesting.
    internal readonly MDER_Any label;

    // The Muldis Data Language value that is the "attributes" of this MDER_Article value.
    internal readonly MDER_Tuple attrs;

    internal MDER_Article(Internal_Memory memory, MDER_Any label, MDER_Tuple attrs)
        : base(memory, Internal_Well_Known_Base_Type.MDER_Article)
    {
        this.label = label;
        this.attrs = attrs;
    }
}
