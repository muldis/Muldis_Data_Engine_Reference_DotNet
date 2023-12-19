namespace Muldis.Data_Engine_Reference;

public sealed class MDER_Array : MDER_Discrete
{
    internal MDER_Array(MDER_Machine machine, Internal_MDER_Discrete_Struct tree_root_node)
        : base(machine, tree_root_node)
    {
    }

    internal override String _as_MUON_Plain_Text_artifact(String indent)
    {
        String mei = indent + "\u0009";
        return Object.ReferenceEquals(this, this.machine().MDER_Array_C0) ? "(Array:[])"
            : "(Array:[\u000A" + this._as_MUON_Array_artifact__node__tree(
                this.tree_root_node, mei) + indent + "])";
    }

    private String _as_MUON_Array_artifact__node__tree(Internal_MDER_Discrete_Struct node, String indent)
    {
        // Note: We always display consecutive duplicates in repeating
        // value format rather than value-count format in order to keep
        // everything simple and reliable in the 99% common case; we
        // assume that it would only be a rare edge case to have a
        // sparse array that we would want to output in that manner.
        // TODO: Rendering a Singular with a higher multiplicity than
        // an Int32 can hold will throw a runtime exception; we can
        // deal with that later if an actual use case runs afowl of it.
        switch (node.local_symbolic_type)
        {
            case Internal_Symbolic_Discrete_Type.None:
                return "";
            case Internal_Symbolic_Discrete_Type.Singular:
                return String.Concat(Enumerable.Repeat(
                    indent + node.Local_Singular_Members().member._as_MUON_Plain_Text_artifact(indent) + ",\u000A",
                    (Int32)node.Local_Singular_Members().multiplicity));
            case Internal_Symbolic_Discrete_Type.Arrayed:
                return String.Concat(Enumerable.Select(
                    node.Local_Arrayed_Members(),
                    m => indent + m._as_MUON_Plain_Text_artifact(indent) + ",\u000A"));
            case Internal_Symbolic_Discrete_Type.Catenated:
                return this._as_MUON_Array_artifact__node__tree(node.Tree_Catenated_Members().a0, indent)
                    + this._as_MUON_Array_artifact__node__tree(node.Tree_Catenated_Members().a1, indent);
            default:
                throw new NotImplementedException();
        }
    }

    internal Boolean _MDER_Array__same(MDER_Array topic_1)
    {
        MDER_Array topic_0 = this;
        topic_0.Discrete__Collapse();
        topic_1.Discrete__Collapse();
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
            // the empty Array is optimized to return a constant
            // at selection time, but we will check anyway.
            return true;
        }
        if (node_0.local_symbolic_type == Internal_Symbolic_Discrete_Type.Singular
            && node_1.local_symbolic_type == Internal_Symbolic_Discrete_Type.Singular)
        {
            return ((node_0.Local_Singular_Members().multiplicity
                    == node_1.Local_Singular_Members().multiplicity)
                && node_0.Local_Singular_Members().member._MDER_Any__same(
                    node_1.Local_Singular_Members().member));
        }
        if (node_0.local_symbolic_type == Internal_Symbolic_Discrete_Type.Arrayed
            && node_1.local_symbolic_type == Internal_Symbolic_Discrete_Type.Arrayed)
        {
            // This works because MDER_Any Equals() calls Any__Same().
            return Enumerable.SequenceEqual(
                node_0.Local_Arrayed_Members(),
                node_1.Local_Arrayed_Members());
        }
        Internal_MDER_Discrete_Struct n0_ = node_0;
        Internal_MDER_Discrete_Struct n1_ = node_1;
        if (n0_.local_symbolic_type == Internal_Symbolic_Discrete_Type.Arrayed
            && n1_.local_symbolic_type == Internal_Symbolic_Discrete_Type.Singular)
        {
             n1_ = node_0;
             n0_ = node_1;
        }
        if (n0_.local_symbolic_type == Internal_Symbolic_Discrete_Type.Singular
            && n1_.local_symbolic_type == Internal_Symbolic_Discrete_Type.Arrayed)
        {
            Internal_Multiplied_Member sm = n0_.Local_Singular_Members();
            List<MDER_Any> am = n1_.Local_Arrayed_Members();
            return sm.multiplicity == am.Count
                && Enumerable.All(am, m => m._MDER_Any__same(sm.member));
        }
        // We should never get here.
        throw new NotImplementedException();
    }

    internal MDER_Any? Array__maybe_at(Int64 ord_pos)
    {
        return Array__node__maybe_at(this.tree_root_node, ord_pos);
    }

    internal MDER_Any? Array__node__maybe_at(Internal_MDER_Discrete_Struct node, Int64 ord_pos)
    {
        if (node.cached_tree_member_count == 0)
        {
            return null;
        }
        Int64 maybe_last_ord_pos_seen = -1;
        switch (node.local_symbolic_type)
        {
            case Internal_Symbolic_Discrete_Type.None:
                return null;
            case Internal_Symbolic_Discrete_Type.Singular:
                if (ord_pos <= (maybe_last_ord_pos_seen + node.Local_Singular_Members().multiplicity))
                {
                    return node.Local_Singular_Members().member;
                }
                return null;
            case Internal_Symbolic_Discrete_Type.Arrayed:
                Int64 local_member_count = Discrete__node__tree_member_count(node);
                if (ord_pos <= (maybe_last_ord_pos_seen + local_member_count))
                {
                    Int64 ord_pos_within_local = ord_pos - (1 + maybe_last_ord_pos_seen);
                    return node.Local_Arrayed_Members()[(Int32)ord_pos_within_local];
                }
                return null;
            case Internal_Symbolic_Discrete_Type.Catenated:
                MDER_Any maybe_member = Array__node__maybe_at(node.Tree_Catenated_Members().a0, ord_pos)!;
                if (maybe_member is not null)
                {
                    return maybe_member;
                }
                maybe_last_ord_pos_seen += Discrete__node__tree_member_count(node.Tree_Catenated_Members().a0);
                if (ord_pos <= maybe_last_ord_pos_seen)
                {
                    return null;
                }
                Int64 ord_pos_within_succ = ord_pos - (1 + maybe_last_ord_pos_seen);
                return Array__node__maybe_at(node.Tree_Catenated_Members().a1, ord_pos_within_succ);
            default:
                throw new NotImplementedException();
        }
    }
}
