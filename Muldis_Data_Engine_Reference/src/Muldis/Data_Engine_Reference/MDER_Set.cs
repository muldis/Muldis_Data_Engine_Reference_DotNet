namespace Muldis.Data_Engine_Reference;

internal class MDL_Set : MDL_Any
{
    internal MDL_Bag_Struct tree_root_node;

    internal MDL_Set(Memory memory, MDL_Bag_Struct tree_root_node)
        : base(memory, Well_Known_Base_Type.MDL_Set)
    {
        this.tree_root_node = tree_root_node;
    }
}
