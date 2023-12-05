namespace Muldis.Data_Engine_Reference.Internal;

internal class MDL_Bag : MDL_Any
{
    internal MDL_Bag_Struct tree_root_node;

    internal MDL_Bag(Memory memory, MDL_Bag_Struct tree_root_node)
        : base(memory, Well_Known_Base_Type.MDL_Bag)
    {
        this.tree_root_node = tree_root_node;
    }
}
