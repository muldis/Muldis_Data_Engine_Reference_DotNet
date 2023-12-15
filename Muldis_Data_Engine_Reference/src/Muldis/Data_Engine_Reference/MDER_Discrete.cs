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

    internal Int64 Discrete__count()
    {
        return Discrete__node__tree_member_count(this.tree_root_node);
    }

    internal Int64 Discrete__node__tree_member_count(Internal_MDER_Discrete_Struct node)
    {
        if (node.cached_members_meta!.tree_member_count is null)
        {
            switch (node.local_symbolic_type)
            {
                case Internal_Symbolic_Discrete_Type.None:
                    node.cached_members_meta.tree_member_count = 0;
                    break;
                case Internal_Symbolic_Discrete_Type.Singular:
                    node.cached_members_meta.tree_member_count
                        = node.Local_Singular_Members().multiplicity;
                    break;
                case Internal_Symbolic_Discrete_Type.Arrayed:
                    node.cached_members_meta.tree_member_count
                        = node.Local_Arrayed_Members().Count;
                    break;
                case Internal_Symbolic_Discrete_Type.Catenated:
                    node.cached_members_meta.tree_member_count
                        = Discrete__node__tree_member_count(node.Tree_Catenated_Members().a0)
                        + Discrete__node__tree_member_count(node.Tree_Catenated_Members().a1);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        return (Int64)node.cached_members_meta.tree_member_count;
    }

    internal void Discrete__Collapse(Boolean want_indexed = false)
    {
        this.tree_root_node = Discrete__Collapsed_Struct(this.tree_root_node, want_indexed);
    }

    internal Internal_MDER_Discrete_Struct Discrete__Collapsed_Struct(
        Internal_MDER_Discrete_Struct node, Boolean want_indexed = false)
    {
        // Note: want_indexed only causes Indexed result when Singular
        // or Arrayed_MM otherwise would be used; None still also used.
        switch (node.local_symbolic_type)
        {
            case Internal_Symbolic_Discrete_Type.None:
                // Node is already collapsed.
                // In theory we should never get here assuming that any
                // operations which would knowingly result in the empty
                // Array are optimized to return MDER_Array_C0 directly.
                return this.machine().MDER_Array_C0.tree_root_node;
            case Internal_Symbolic_Discrete_Type.Singular:
                if (!want_indexed)
                {
                    // Node is already collapsed.
                    return node;
                }
                Internal_Multiplied_Member lsm = node.Local_Singular_Members();
                return new Internal_MDER_Discrete_Struct {
                    local_symbolic_type = Internal_Symbolic_Discrete_Type.Indexed,
                    members = new Dictionary<MDER_Any, Internal_Multiplied_Member>()
                        {{lsm.member, lsm}},
                    cached_members_meta = new Internal_Cached_Members_Meta {
                        tree_member_count = lsm.multiplicity,
                        tree_all_unique = (lsm.multiplicity == 1),
                        tree_relational = (lsm.member._WKBT()
                            == Internal_Well_Known_Base_Type.MDER_Tuple),
                    },
                };
            case Internal_Symbolic_Discrete_Type.Arrayed:
                // Node is already collapsed.
                return node;
            case Internal_Symbolic_Discrete_Type.Arrayed_MM:
                if (!want_indexed)
                {
                    // Node is already collapsed.
                    return node;
                }
                List<Internal_Multiplied_Member> ary_src_list = node.Local_Arrayed_MM_Members();
                Dictionary<MDER_Any, Internal_Multiplied_Member> ary_res_dict
                    = new Dictionary<MDER_Any, Internal_Multiplied_Member>();
                foreach (Internal_Multiplied_Member m in ary_src_list)
                {
                    if (!ary_res_dict.ContainsKey(m.member))
                    {
                        ary_res_dict.Add(m.member, m.Clone());
                    }
                    else
                    {
                        ary_res_dict[m.member].multiplicity ++;
                    }
                }
                return new Internal_MDER_Discrete_Struct {
                    local_symbolic_type = Internal_Symbolic_Discrete_Type.Indexed,
                    members = ary_res_dict,
                    cached_members_meta = new Internal_Cached_Members_Meta {
                        tree_member_count = ary_src_list.Count,
                        tree_all_unique = node.cached_members_meta!.tree_all_unique,
                        tree_relational = node.cached_members_meta.tree_relational,
                    },
                };
            case Internal_Symbolic_Discrete_Type.Catenated:
                Internal_MDER_Discrete_Struct n0 = Discrete__Collapsed_Struct(node.Tree_Catenated_Members().a0);
                Internal_MDER_Discrete_Struct n1 = Discrete__Collapsed_Struct(node.Tree_Catenated_Members().a1);
                if (n0.local_symbolic_type == Internal_Symbolic_Discrete_Type.None)
                {
                    return n1;
                }
                if (n1.local_symbolic_type == Internal_Symbolic_Discrete_Type.None)
                {
                    return n0;
                }
                // Both child nodes have at least 1 member, so we will
                // use Arrayed format for merger even if the input
                // members are the same value.
                // This will die if multiplicity greater than an Int32.
                return new Internal_MDER_Discrete_Struct {
                    local_symbolic_type = Internal_Symbolic_Discrete_Type.Arrayed,
                    members = Enumerable.Concat(
                        (n0.local_symbolic_type == Internal_Symbolic_Discrete_Type.Singular
                            ? Enumerable.Repeat(n0.Local_Singular_Members().member,
                                (Int32)n0.Local_Singular_Members().multiplicity)
                            : n0.Local_Arrayed_Members()),
                        (n1.local_symbolic_type == Internal_Symbolic_Discrete_Type.Singular
                            ? Enumerable.Repeat(n1.Local_Singular_Members().member,
                                (Int32)n1.Local_Singular_Members().multiplicity)
                            : n1.Local_Arrayed_Members())
                    ),
                    cached_members_meta = new Internal_Cached_Members_Meta(),
                        // TODO: Merge existing source meta where efficient,
                        // tree_member_count and tree_relational in particular.
                };
            case Internal_Symbolic_Discrete_Type.Indexed:
                // Node is already collapsed.
                return node;
            case Internal_Symbolic_Discrete_Type.Unique:
                Internal_MDER_Discrete_Struct uni_pa = Discrete__Collapsed_Struct(
                    node: node.Tree_Unique_Members(), want_indexed: true);
                if (uni_pa.local_symbolic_type == Internal_Symbolic_Discrete_Type.None)
                {
                    return uni_pa;
                }
                Dictionary<MDER_Any, Internal_Multiplied_Member> uni_src_dict
                    = uni_pa.Local_Indexed_Members();
                return new Internal_MDER_Discrete_Struct {
                    local_symbolic_type = Internal_Symbolic_Discrete_Type.Indexed,
                    members = uni_src_dict.ToDictionary(
                        m => m.Key, m => new Internal_Multiplied_Member(m.Key, 1)),
                    cached_members_meta = new Internal_Cached_Members_Meta {
                        tree_member_count = uni_src_dict.Count,
                        tree_all_unique = true,
                        tree_relational = uni_pa.cached_members_meta!.tree_relational,
                    },
                };
            case Internal_Symbolic_Discrete_Type.Summed:
                Internal_MDER_Discrete_Struct n0_ = Discrete__Collapsed_Struct(
                    node: node.Tree_Summed_Members().a0, want_indexed: true);
                Internal_MDER_Discrete_Struct n1_ = Discrete__Collapsed_Struct(
                    node: node.Tree_Summed_Members().a1, want_indexed: true);
                if (n0_.local_symbolic_type == Internal_Symbolic_Discrete_Type.None)
                {
                    return n1_;
                }
                if (n1_.local_symbolic_type == Internal_Symbolic_Discrete_Type.None)
                {
                    return n0_;
                }
                Dictionary<MDER_Any, Internal_Multiplied_Member> n0_src_dict
                    = n0_.Local_Indexed_Members();
                Dictionary<MDER_Any, Internal_Multiplied_Member> n1_src_dict
                    = n1_.Local_Indexed_Members();
                Dictionary<MDER_Any, Internal_Multiplied_Member> res_dict
                    = new Dictionary<MDER_Any, Internal_Multiplied_Member>(n0_src_dict);
                foreach (Internal_Multiplied_Member m in n1_src_dict.Values)
                {
                    if (!res_dict.ContainsKey(m.member))
                    {
                        res_dict.Add(m.member, m);
                    }
                    else
                    {
                        res_dict[m.member] = new Internal_Multiplied_Member(m.member,
                            res_dict[m.member].multiplicity + m.multiplicity);
                    }
                }
                return new Internal_MDER_Discrete_Struct {
                    local_symbolic_type = Internal_Symbolic_Discrete_Type.Indexed,
                    members = res_dict,
                    cached_members_meta = new Internal_Cached_Members_Meta(),
                        // TODO: Merge existing source meta where efficient,
                        // tree_member_count and tree_relational in particular.
                };
            default:
                throw new NotImplementedException();
        }
    }

    internal MDER_Any? Discrete__maybe_Pick_Arbitrary_Member()
    {
        return Discrete__maybe_Pick_Arbitrary_Node_Member(this.tree_root_node);
    }

    internal MDER_Any? Discrete__maybe_Pick_Arbitrary_Node_Member(Internal_MDER_Discrete_Struct node)
    {
        if (node.cached_members_meta!.tree_member_count == 0)
        {
            return null;
        }
        switch (node.local_symbolic_type)
        {
            case Internal_Symbolic_Discrete_Type.None:
                return null;
            case Internal_Symbolic_Discrete_Type.Singular:
                return node.Local_Singular_Members().member;
            case Internal_Symbolic_Discrete_Type.Arrayed:
                return node.Local_Arrayed_Members()[0];
            case Internal_Symbolic_Discrete_Type.Arrayed_MM:
                return node.Local_Arrayed_MM_Members()[0].member;
            case Internal_Symbolic_Discrete_Type.Catenated:
                return Discrete__maybe_Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().a0)
                    ?? Discrete__maybe_Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().a1);
            case Internal_Symbolic_Discrete_Type.Indexed:
                return node.Local_Indexed_Members().First().Value.member;
            case Internal_Symbolic_Discrete_Type.Unique:
                return Discrete__maybe_Pick_Arbitrary_Node_Member(node.Tree_Unique_Members());
            case Internal_Symbolic_Discrete_Type.Summed:
                return Discrete__maybe_Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().a0)
                    ?? Discrete__maybe_Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().a1);
            default:
                throw new NotImplementedException();
        }
    }

    internal Boolean Discrete__Is_Relational()
    {
        return Discrete__Tree_Relational(this.tree_root_node);
    }

    internal Boolean Discrete__Tree_Relational(Internal_MDER_Discrete_Struct node)
    {
        if (node.cached_members_meta!.tree_relational is null)
        {
            Boolean tr = true;
            switch (node.local_symbolic_type)
            {
                case Internal_Symbolic_Discrete_Type.None:
                    tr = true;
                    break;
                case Internal_Symbolic_Discrete_Type.Singular:
                    tr = node.Local_Singular_Members().member._WKBT()
                        == Internal_Well_Known_Base_Type.MDER_Tuple;
                    break;
                case Internal_Symbolic_Discrete_Type.Arrayed:
                    MDER_Any m0 = Discrete__maybe_Pick_Arbitrary_Node_Member(node)!;
                    tr = m0._WKBT() == Internal_Well_Known_Base_Type.MDER_Tuple
                        && Enumerable.All(
                            node.Local_Arrayed_Members(),
                            m => m._WKBT() == Internal_Well_Known_Base_Type.MDER_Tuple
                                && Tuple__Same_Heading((MDER_Tuple)m, (MDER_Tuple)m0)
                        );
                    break;
                case Internal_Symbolic_Discrete_Type.Arrayed_MM:
                    MDER_Any m0_ = Discrete__maybe_Pick_Arbitrary_Node_Member(node)!;
                    tr = m0_._WKBT() == Internal_Well_Known_Base_Type.MDER_Tuple
                        && Enumerable.All(
                            node.Local_Arrayed_MM_Members(),
                            m => m.member._WKBT() == Internal_Well_Known_Base_Type.MDER_Tuple
                                && Tuple__Same_Heading((MDER_Tuple)m.member, (MDER_Tuple)m0_)
                        );
                    break;
                case Internal_Symbolic_Discrete_Type.Catenated:
                    MDER_Any pm0 = Discrete__maybe_Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().a0)!;
                    MDER_Any sm0 = Discrete__maybe_Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().a1)!;
                    tr = Discrete__Tree_Relational(node.Tree_Catenated_Members().a0)
                        && Discrete__Tree_Relational(node.Tree_Catenated_Members().a1)
                        && (pm0 is null || sm0 is null || Tuple__Same_Heading((MDER_Tuple)pm0, (MDER_Tuple)sm0));
                    break;
                case Internal_Symbolic_Discrete_Type.Indexed:
                    MDER_Any im0 = Discrete__maybe_Pick_Arbitrary_Node_Member(node)!;
                    tr = im0._WKBT() == Internal_Well_Known_Base_Type.MDER_Tuple
                        && Enumerable.All(
                            node.Local_Indexed_Members().Values,
                            m => m.member._WKBT() == Internal_Well_Known_Base_Type.MDER_Tuple
                                && Tuple__Same_Heading((MDER_Tuple)m.member, (MDER_Tuple)im0)
                        );
                    break;
                case Internal_Symbolic_Discrete_Type.Unique:
                    tr = Discrete__Tree_Relational(node.Tree_Unique_Members());
                    break;
                case Internal_Symbolic_Discrete_Type.Summed:
                    tr = Discrete__Tree_Relational(node.Tree_Summed_Members().a0)
                        && Discrete__Tree_Relational(node.Tree_Summed_Members().a1);
                    MDER_Tuple pam0 = (MDER_Tuple)Discrete__maybe_Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().a0)!;
                    MDER_Tuple eam0 = (MDER_Tuple)Discrete__maybe_Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().a1)!;
                    if (pam0 is not null && eam0 is not null)
                    {
                        tr = tr && Tuple__Same_Heading(pam0, eam0);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            node.cached_members_meta.tree_relational = tr;
        }
        return (Boolean)node.cached_members_meta.tree_relational;
    }

    internal Boolean Tuple__Same_Heading(MDER_Tuple t1, MDER_Tuple t2)
    {
        if (Object.ReferenceEquals(t1, t2))
        {
            return true;
        }
        Dictionary<String, MDER_Any> attrs1 = t1.attrs;
        Dictionary<String, MDER_Any> attrs2 = t2.attrs;
        return (attrs1.Count == attrs2.Count)
            && Enumerable.All(attrs1, attr => attrs2.ContainsKey(attr.Key));
    }
}
