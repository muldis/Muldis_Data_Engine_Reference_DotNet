namespace Muldis.Data_Engine_Reference;

internal class MDL_Tuple : MDL_Any
{
    internal readonly Dictionary<String, MDL_Any> attrs;

    internal MDL_Tuple(Memory memory, Dictionary<String, MDL_Any> attrs)
        : base(memory, Well_Known_Base_Type.MDL_Tuple)
    {
        this.attrs = attrs;
    }
}
