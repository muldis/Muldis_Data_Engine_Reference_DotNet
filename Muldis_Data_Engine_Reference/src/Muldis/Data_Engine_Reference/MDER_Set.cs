namespace Muldis.Data_Engine_Reference;

public sealed class MDER_Set : MDER_Discrete
{
    internal MDER_Set(MDER_Machine machine, Internal_MDER_Discrete_Struct tree_root_node)
        : base(machine, tree_root_node)
    {
    }

    internal String _as_MUON_Set_artifact(String indent)
    {
        String mei = indent + "\u0009";
        this.Discrete__Collapse(want_indexed: true);
        Internal_MDER_Discrete_Struct node = this.tree_root_node;
        switch (node.local_symbolic_type)
        {
            case Internal_Symbolic_Discrete_Type.None:
                return "(Set:[])";
            case Internal_Symbolic_Discrete_Type.Indexed:
                return "(Set:[\u000A" + String.Concat(Enumerable.Select(
                    Enumerable.OrderBy(node.Local_Indexed_Members(), m => m.Key),
                    m => mei + m.Key._as_MUON_Any_artifact(mei) + ",\u000A"
                )) + indent + "])";
            default:
                throw new NotImplementedException();
        }
    }

    internal Boolean _MDER_Set__same(MDER_Set topic_1)
    {
        MDER_Set topic_0 = this;
        topic_0.Discrete__Collapse(want_indexed: true);
        topic_1.Discrete__Collapse(want_indexed: true);
        return this.collapsed_Discrete_Struct__same(
            topic_0.tree_root_node, topic_0.tree_root_node);
    }

    private Boolean collapsed_Discrete_Struct__same(
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
