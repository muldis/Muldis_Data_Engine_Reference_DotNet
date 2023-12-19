namespace Muldis.Data_Engine_Reference;

public abstract class MDER_Discrete : MDER_Any
{
    // The MDER_Machine VM that this MDER_Any "value" lives in.
    private readonly MDER_Machine __machine;

    // Surrogate identity for this MDER_Any with a simpler representation.
    private String? __cached_identity_as_String;

    internal Internal_MDER_Discrete_Struct tree_root_node;

    internal MDER_Discrete(MDER_Machine machine,
        Internal_MDER_Discrete_Struct tree_root_node)
    {
        this.__machine = machine;
        this.tree_root_node = tree_root_node;
    }

    public override MDER_Machine machine()
    {
        return this.__machine;
    }

    internal override String _identity_as_String()
    {
        if (this.__cached_identity_as_String is null)
        {
            this.__cached_identity_as_String
                = this._as_MUON_Plain_Text_artifact("");
        }
        return this.__cached_identity_as_String;
    }

    internal override String _preview_as_String()
    {
        return this._as_MUON_Plain_Text_artifact("");
    }

    internal Int64 Discrete__count()
    {
        return Discrete__node__tree_member_count(this.tree_root_node);
    }

    internal Int64 Discrete__node__tree_member_count(Internal_MDER_Discrete_Struct node)
    {
        if (node.cached_tree_member_count is null)
        {
            switch (node.local_symbolic_type)
            {
                case Internal_Symbolic_Discrete_Type.None:
                    node.cached_tree_member_count = 0;
                    break;
                case Internal_Symbolic_Discrete_Type.Singular:
                    node.cached_tree_member_count
                        = node.Local_Singular_Members().multiplicity;
                    break;
                case Internal_Symbolic_Discrete_Type.Arrayed:
                    node.cached_tree_member_count
                        = node.Local_Arrayed_Members().Count;
                    break;
                case Internal_Symbolic_Discrete_Type.Catenated:
                    node.cached_tree_member_count
                        = Discrete__node__tree_member_count(node.Tree_Catenated_Members().a0)
                        + Discrete__node__tree_member_count(node.Tree_Catenated_Members().a1);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        return (Int64)node.cached_tree_member_count;
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
                    cached_tree_member_count = lsm.multiplicity,
                    cached_tree_all_unique = (lsm.multiplicity == 1),
                    cached_tree_relational = (lsm.member is MDER_Tuple),
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
                    cached_tree_member_count = ary_src_list.Count,
                    cached_tree_all_unique = node.cached_tree_all_unique,
                    cached_tree_relational = node.cached_tree_relational,
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
                        // TODO: Merge existing source meta where efficient,
                        // cached_tree_member_count and cached_tree_relational in particular.
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
                    cached_tree_member_count = uni_src_dict.Count,
                    cached_tree_all_unique = true,
                    cached_tree_relational = uni_pa.cached_tree_relational,
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
                        // TODO: Merge existing source meta where efficient,
                        // cached_tree_member_count and cached_tree_relational in particular.
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
        if (node.cached_tree_member_count == 0)
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
        if (node.cached_tree_relational is null)
        {
            Boolean tr = true;
            switch (node.local_symbolic_type)
            {
                case Internal_Symbolic_Discrete_Type.None:
                    tr = true;
                    break;
                case Internal_Symbolic_Discrete_Type.Singular:
                    tr = node.Local_Singular_Members().member is MDER_Tuple;
                    break;
                case Internal_Symbolic_Discrete_Type.Arrayed:
                    MDER_Any m0 = Discrete__maybe_Pick_Arbitrary_Node_Member(node)!;
                    tr = m0 is MDER_Tuple
                        && Enumerable.All(
                            node.Local_Arrayed_Members(),
                            m => m is MDER_Tuple
                                && Tuple__Same_Heading((MDER_Tuple)m, (MDER_Tuple)m0)
                        );
                    break;
                case Internal_Symbolic_Discrete_Type.Arrayed_MM:
                    MDER_Any m0_ = Discrete__maybe_Pick_Arbitrary_Node_Member(node)!;
                    tr = m0_ is MDER_Tuple
                        && Enumerable.All(
                            node.Local_Arrayed_MM_Members(),
                            m => m.member is MDER_Tuple
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
                    tr = im0 is MDER_Tuple
                        && Enumerable.All(
                            node.Local_Indexed_Members().Values,
                            m => m.member is MDER_Tuple
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
            node.cached_tree_relational = tr;
        }
        return (Boolean)node.cached_tree_relational;
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

// Muldis.Data_Engine_Reference.Internal_Symbolic_Discrete_Type
// Enumerates the various ways that a MDER_Array collection can be defined
// symbolically in terms of other collections.
// None means the collection simply has zero members.

internal enum Internal_Symbolic_Discrete_Type
{
    None,
    Singular,
    Arrayed,
    Arrayed_MM,
    Catenated,
    Indexed,
    Unique,
    Summed,
}

// Muldis.Data_Engine_Reference.Internal_MDER_Discrete_Struct
// When a Muldis.Data_Engine_Reference.MDER_Any is representing a MDER_Array,
// a Internal_MDER_Discrete_Struct is used by it to hold the MDER_Array-specific details.
// Also used for MDER_Set and MDER_Bag, so any MDER_Array reference generally should be
// read as either MDER_Set or MDER_Bag or MDER_Array.
// The "tree" is actually a uni-directional graph as multiple nodes can
// cite the same other conceptually immutable nodes as their children.
// MDER_Array members are only directly stored in leaf nodes of the graph.
// It is important to note that the same logical Muldis Data Language Array value
// may be represented by several different representation formats here,
// Note that MDER_Array is the most complicated Muldis Data Language Foundation type to
// implement while at the same time is the least critical to the
// internals; it is mainly just used for user data.

internal sealed class Internal_MDER_Discrete_Struct
{
    // Local Symbolic Type (LST) determines the role this node plays in
    // the tree.  Determines interpreting "members" field.
    internal Internal_Symbolic_Discrete_Type local_symbolic_type;

    // Members of this Muldis Data Language Array, in one of several possible
    // specialized representation formats depending on the data type.
    // Iff LST is None, this field is simply null.
        // This is a leaf node explicitly defining zero Array members.
        // Guarantees the Array has exactly zero members.
    // Iff LST is Singular, this field holds a Internal_Multiplied_Member.
        // This is a leaf node defining 1..N Array members, every one of
        // which is the same value, which can be of any type;
        // Singular is the basis for compactly representing either a
        // single-element Array or a repeating portion of a sparse Array.
        // Guarantees the Array has at least 1 member and that all
        // Array members are the same value.
    // Iff LST is Arrayed, this field holds a List<MDER_Any>.
        // This is a leaf node defining 2..N Array members, each of which
        // can be of any type; this is the most common format.
        // Guarantees the Array has at least 2 members but does not
        // guarantee that at least 2 members have distinct values.
    // Iff LST is Arrayed_MM, this field holds a List<Internal_Multiplied_Member>.
        // This is a leaf node defining 2..N Array members, each of which
        // can be of any type; this is the most common format for a Array
        // that is just storing members without any operations.
        // Guarantees the Array has at least 2 members but does not
        // guarantee that at least 2 members have distinct values.
    // Iff LST is Catenated, this field holds a Internal_MDER_Discrete_Pair.
        // This is a non-leaf node with 2 direct child Array nodes;
        // this Array's members are defined as the catenation of
        // the pair's a0 and a1 nodes in that order.
        // Makes no guarantees that the Array is none/singular/otherwise.
    // Iff LST is Indexed, this field holds a Dictionary<MDER_Any, Internal_Multiplied_Member>.
        // This is a leaf node defining 2..N Array members, each of which
        // can be of any type; this is the most common format for a Array
        // that has had some searches or operations performed on it.
        // The Dictionary has one key-asset pair for each distinct Muldis Data Language
        // "value", all of which are indexed by __cached_MDER_Any_identity.
        // Guarantees the Array has at least 1 member.
    // Iff LST is Unique, this field holds a Internal_MDER_Discrete_Struct.
        // This is a non-leaf node with 1 direct child Array node;
        // this Array's members are defined as the child's unique members.
        // Makes no guarantees that the Array is none/singular/otherwise.
    // Iff LST is Summed, this field holds a Internal_MDER_Discrete_Pair.
        // This is a non-leaf node with 2 direct child Array nodes;
        // this Array's members are defined as the catenation of
        // the pair's a0 and a1 nodes in that order.
        // Makes no guarantees that the Array is none/singular/otherwise.
    internal Object? members;

    // Nullable Integer
    // Cached count of members of the Muldis Data Language Array/Bag represented by
    // this tree node including those defined by it and child nodes.
    internal Int64? cached_tree_member_count;

    // Nullable Boolean
    // This is true iff we know that no 2 members of the Muldis Data Language
    // Array/Bag represented by this tree node (including child nodes)
    // are the same value, and false iff we know that at least 2
    // members are the same value.
    internal Boolean? cached_tree_all_unique;

    // Nullable Boolean
    // This is true iff we know that the Muldis Data Language Array/Bag represented
    // by this tree node (as with cached_tree_all_unique) has no member that
    // is not a Muldis Data Language Tuple and has no 2 members that do not have
    // the same heading; this is false iff we know that any member is
    // not a Tuple or that any 2 members do not have the same heading.
    internal Boolean? cached_tree_relational;

    internal Internal_Multiplied_Member Local_Singular_Members()
    {
        return (Internal_Multiplied_Member)this.members!;
    }

    internal List<MDER_Any> Local_Arrayed_Members()
    {
        return (List<MDER_Any>)this.members!;
    }

    internal List<Internal_Multiplied_Member> Local_Arrayed_MM_Members()
    {
        return (List<Internal_Multiplied_Member>)this.members!;
    }

    internal Internal_MDER_Discrete_Pair Tree_Catenated_Members()
    {
        return (Internal_MDER_Discrete_Pair)this.members!;
    }

    internal Dictionary<MDER_Any, Internal_Multiplied_Member> Local_Indexed_Members()
    {
        return (Dictionary<MDER_Any, Internal_Multiplied_Member>)this.members!;
    }

    internal Internal_MDER_Discrete_Struct Tree_Unique_Members()
    {
        return (Internal_MDER_Discrete_Struct)this.members!;
    }

    internal Internal_MDER_Discrete_Pair Tree_Summed_Members()
    {
        return (Internal_MDER_Discrete_Pair)this.members!;
    }
}

// Muldis.Data_Engine_Reference.Internal_MDER_Discrete_Pair
// Represents an ordered pair of MDER_Array, typically corresponding in
// order to the "0" and "1" conceptually-ordered arg/attr to a function.

internal sealed class Internal_MDER_Discrete_Pair
{
    // This is the first conceptually-ordered arg/attr.
    internal Internal_MDER_Discrete_Struct a0;

    // This is the second conceptually-ordered arg/attr.
    internal Internal_MDER_Discrete_Struct a1;

    internal Internal_MDER_Discrete_Pair(Internal_MDER_Discrete_Struct a0, Internal_MDER_Discrete_Struct a1)
    {
        this.a0 = a0;
        this.a1 = a1;
    }
}

// Muldis.Data_Engine_Reference.Internal_Multiplied_Member
// Represents a multiset of 1..N members of a collection where every
// member is the same Muldis Data Language value.

internal sealed class Internal_Multiplied_Member
{
    // The Muldis Data Language value that every member of this multiset is.
    internal MDER_Any member;

    // The count of members of this multiset.
    internal Int64 multiplicity;

    internal Internal_Multiplied_Member(MDER_Any member, Int64 multiplicity = 1)
    {
        this.member       = member;
        this.multiplicity = multiplicity;
    }

    internal Internal_Multiplied_Member Clone()
    {
        return (Internal_Multiplied_Member)this.MemberwiseClone();
    }
}
