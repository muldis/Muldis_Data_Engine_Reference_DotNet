namespace Muldis.Data_Engine_Reference;

internal class MDER_Set : MDER_Any
{
    internal Internal_MDER_Bag_Struct tree_root_node;

    internal MDER_Set(MDER_Machine machine, Internal_MDER_Bag_Struct tree_root_node)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Set)
    {
        this.tree_root_node = tree_root_node;
    }
}
