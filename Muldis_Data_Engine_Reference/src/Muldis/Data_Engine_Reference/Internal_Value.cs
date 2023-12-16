namespace Muldis.Data_Engine_Reference;

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

internal class Internal_MDER_Discrete_Struct
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

internal class Internal_MDER_Discrete_Pair
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

internal class Internal_Multiplied_Member
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
