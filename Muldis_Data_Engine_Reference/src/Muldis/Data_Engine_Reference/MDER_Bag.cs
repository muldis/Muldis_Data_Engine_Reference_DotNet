namespace Muldis.Data_Engine_Reference;

public class MDER_Bag : MDER_Any
{
    internal Internal_MDER_Bag_Struct tree_root_node;

    internal MDER_Bag(MDER_Machine machine, Internal_MDER_Bag_Struct tree_root_node)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Bag)
    {
        this.tree_root_node = tree_root_node;
    }

    internal String _as_MUON_Bag_artifact(String indent)
    {
        String mei = indent + "\u0009";
        this.machine()._executor().Bag__Collapse(bag: this, want_indexed: true);
        Internal_MDER_Bag_Struct node = this.tree_root_node;
        switch (node.local_symbolic_type)
        {
            case Internal_Symbolic_Bag_Type.None:
                return "(Bag:[])";
            case Internal_Symbolic_Bag_Type.Indexed:
                return "(Bag:[\u000A" + String.Concat(Enumerable.Select(
                    Enumerable.OrderBy(node.Local_Indexed_Members(), m => m.Key),
                    m => mei + m.Key._as_MUON_Any_artifact(mei) + " : "
                        + m.Value.multiplicity.ToString() + ",\u000A"
                )) + indent + "])";
            default:
                throw new NotImplementedException();
        }
    }
}
