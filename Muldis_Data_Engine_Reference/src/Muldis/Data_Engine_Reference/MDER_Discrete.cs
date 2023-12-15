namespace Muldis.Data_Engine_Reference;

public abstract class MDER_Discrete : MDER_Any
{
    internal Internal_MDER_Discrete_Struct tree_root_node;

    internal MDER_Discrete(MDER_Machine machine, Internal_Well_Known_Base_Type WKBT,
        Internal_MDER_Discrete_Struct tree_root_node)
        : base(machine, WKBT)
    {
        this.tree_root_node = tree_root_node;
    }
}
