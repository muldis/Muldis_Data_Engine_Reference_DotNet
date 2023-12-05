namespace Muldis.Data_Engine_Reference;

internal class MDER_Bag : MDER_Any
{
    internal MDER_Bag_Struct tree_root_node;

    internal MDER_Bag(Memory memory, MDER_Bag_Struct tree_root_node)
        : base(memory, Well_Known_Base_Type.MDER_Bag)
    {
        this.tree_root_node = tree_root_node;
    }
}
