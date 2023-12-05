namespace Muldis.Data_Engine_Reference;

internal class MDER_Set : MDER_Any
{
    internal Internal_MDER_Bag_Struct tree_root_node;

    internal MDER_Set(Internal_Memory memory, Internal_MDER_Bag_Struct tree_root_node)
        : base(memory, Internal_Well_Known_Base_Type.MDER_Set)
    {
        this.tree_root_node = tree_root_node;
    }
}
