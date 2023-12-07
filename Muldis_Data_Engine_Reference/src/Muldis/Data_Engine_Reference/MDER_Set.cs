namespace Muldis.Data_Engine_Reference;

public class MDER_Set : MDER_Any
{
    internal Internal_MDER_Bag_Struct tree_root_node;

    internal MDER_Set(MDER_Machine machine, Internal_MDER_Bag_Struct tree_root_node)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Set)
    {
        this.tree_root_node = tree_root_node;
    }

    internal String _as_MUON_Set_artifact(String indent)
    {
        String mei = indent + "\u0009";
        this.machine()._executor().Set__Collapse(set: this, want_indexed: true);
        Internal_MDER_Bag_Struct node = this.tree_root_node;
        switch (node.local_symbolic_type)
        {
            case Internal_Symbolic_Bag_Type.None:
                return "(Set:[])";
            case Internal_Symbolic_Bag_Type.Indexed:
                return "(Set:[\u000A" + String.Concat(Enumerable.Select(
                    Enumerable.OrderBy(node.Local_Indexed_Members(), m => m.Key),
                    m => mei + m.Key._as_MUON_Any_artifact(mei) + ",\u000A"
                )) + indent + "])";
            default:
                throw new NotImplementedException();
        }
    }

    internal Boolean _MDER_Set__same(MDER_Set value_1)
    {
        MDER_Set value_0 = this;
        this.machine()._executor().Set__Collapse(set: value_0, want_indexed: true);
        this.machine()._executor().Set__Collapse(set: value_1, want_indexed: true);
        return this.collapsed_Set_Struct__same(
            value_0.tree_root_node, value_0.tree_root_node);
    }

    private Boolean collapsed_Set_Struct__same(
        Internal_MDER_Bag_Struct node_0, Internal_MDER_Bag_Struct node_1)
    {
        if (Object.ReferenceEquals(node_0, node_1))
        {
            return true;
        }
        if (node_0.local_symbolic_type == Internal_Symbolic_Bag_Type.None
            && node_1.local_symbolic_type == Internal_Symbolic_Bag_Type.None)
        {
            // In theory we should never get here assuming that
            // the empty Bag is optimized to return a constant
            // at selection time, but we will check anyway.
            return true;
        }
        if (node_0.local_symbolic_type == Internal_Symbolic_Bag_Type.Indexed
            && node_1.local_symbolic_type == Internal_Symbolic_Bag_Type.Indexed)
        {
            Dictionary<MDER_Any, Internal_Multiplied_Member> im0
                = node_0.Local_Indexed_Members();
            Dictionary<MDER_Any, Internal_Multiplied_Member> im1
                = node_1.Local_Indexed_Members();
            return im0.Count == im1.Count
                && Enumerable.All(
                    im0.Values,
                    m => im1.ContainsKey(m.member)
                        && im1[m.member].multiplicity == m.multiplicity
                );
        }
        // We should never get here.
        throw new NotImplementedException();
    }
}
