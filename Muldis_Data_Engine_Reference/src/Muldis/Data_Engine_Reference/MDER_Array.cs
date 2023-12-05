namespace Muldis.Data_Engine_Reference.Internal;

internal class MDL_Array : MDL_Any
{
    internal MDL_Array_Struct tree_root_node;

    internal MDL_Array(Memory memory, MDL_Array_Struct tree_root_node)
        : base(memory, Well_Known_Base_Type.MDL_Array)
    {
        this.tree_root_node = tree_root_node;
    }
}
