namespace Muldis.Data_Engine_Reference;

public class MDER_Array : MDER_Any
{
    internal Internal_MDER_Array_Struct tree_root_node;

    internal MDER_Array(MDER_Machine machine, Internal_MDER_Array_Struct tree_root_node)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Array)
    {
        this.tree_root_node = tree_root_node;
    }
}
