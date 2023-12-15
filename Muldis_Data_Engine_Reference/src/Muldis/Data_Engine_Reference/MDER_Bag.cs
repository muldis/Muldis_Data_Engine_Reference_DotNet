namespace Muldis.Data_Engine_Reference;

public class MDER_Bag : MDER_Discrete
{
    internal MDER_Bag(MDER_Machine machine, Internal_MDER_Discrete_Struct tree_root_node)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Bag, tree_root_node)
    {
    }

    internal String _as_MUON_Bag_artifact(String indent)
    {
        String mei = indent + "\u0009";
        this.machine()._executor().Bag__Collapse(bag: this, want_indexed: true);
        Internal_MDER_Discrete_Struct node = this.tree_root_node;
        switch (node.local_symbolic_type)
        {
            case Internal_Symbolic_Discrete_Type.None:
                return "(Bag:[])";
            case Internal_Symbolic_Discrete_Type.Indexed:
                return "(Bag:[\u000A" + String.Concat(Enumerable.Select(
                    Enumerable.OrderBy(node.Local_Indexed_Members(), m => m.Key),
                    m => mei + m.Key._as_MUON_Any_artifact(mei) + " : "
                        + m.Value.multiplicity.ToString() + ",\u000A"
                )) + indent + "])";
            default:
                throw new NotImplementedException();
        }
    }

    internal Boolean _MDER_Bag__same(MDER_Bag value_1)
    {
        MDER_Bag value_0 = this;
        this.machine()._executor().Bag__Collapse(bag: value_0, want_indexed: true);
        this.machine()._executor().Bag__Collapse(bag: value_1, want_indexed: true);
        return this.collapsed_Bag_Struct__same(
            value_0.tree_root_node, value_0.tree_root_node);
    }

    private Boolean collapsed_Bag_Struct__same(
        Internal_MDER_Discrete_Struct node_0, Internal_MDER_Discrete_Struct node_1)
    {
        if (Object.ReferenceEquals(node_0, node_1))
        {
            return true;
        }
        if (node_0.local_symbolic_type == Internal_Symbolic_Discrete_Type.None
            && node_1.local_symbolic_type == Internal_Symbolic_Discrete_Type.None)
        {
            // In theory we should never get here assuming that
            // the empty Bag is optimized to return a constant
            // at selection time, but we will check anyway.
            return true;
        }
        if (node_0.local_symbolic_type == Internal_Symbolic_Discrete_Type.Indexed
            && node_1.local_symbolic_type == Internal_Symbolic_Discrete_Type.Indexed)
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
