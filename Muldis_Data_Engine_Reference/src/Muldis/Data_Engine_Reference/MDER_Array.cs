namespace Muldis.Data_Engine_Reference;

internal class MDER_Array : MDER_Any
{
    internal MDER_Array_Struct tree_root_node;

    internal MDER_Array(Internal_Memory memory, MDER_Array_Struct tree_root_node)
        : base(memory, Internal_Well_Known_Base_Type.MDER_Array)
    {
        this.tree_root_node = tree_root_node;
    }
}
