using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Muldis.D.Ref_Eng.Core
{
    // Muldis.D.Ref_Eng.Core.MD_Foundation_Type
    // Enumerates the Muldis D Foundation data types, which are mutually
    // exclusive; every Muldis D value is a member of exactly one of these.

    internal enum MD_Foundation_Type
    {
        MD_Boolean,
        MD_Integer,
        MD_Array,
        MD_Bag,
        MD_Tuple,
        MD_Capsule,
        MD_Handle,
    };

    // Muldis.D.Ref_Eng.Core.MD_Well_Known_Type
    // Enumerates Muldis D types that are considered well-known to this
    // Muldis D language implementation.  Typically these are defined in
    // the Muldis D Foundation language spec even if most of their related
    // operators are not.  Typically each one is either equal to one of the
    // 7 Muldis D Foundation types or it is a Capsule subtype whose label
    // is a simple unqualified Attr_Name.  Typically a given Muldis D value
    // would be a member of several of these types rather than just one.
    // The Muldis D "Any" type is omitted as it implicitly always applies;
    // this omission is subject to be changed.

    internal enum MD_Well_Known_Type
    {
        Excuse,
        Boolean,
        Integer,
        Fraction,
        Bits,
        Blob,
        Text,
        Text__Unicode,
        Text__ASCII,
        Array,
        String,
        Set,
        Bag,
        Tuple,
        Tuple_Array,
        Relation,
        Tuple_Bag,
        Interval,
        Capsule,
        Handle,
        Variable,
        Process,
        Stream,
        External,
        Package,
        Function,
        Procedure,
        Signature,
        Signature__Conjunction,
        Signature__Disjunction,
        Signature__Tuple_Attrs_Match_Simple,
        Signature__Tuple_Attrs_Match,
        Signature__Capsule_Match,
        Expression,
        Literal,
        Args,
        Evaluates,
        Array_Selector,
        Set_Selector,
        Bag_Selector,
        Tuple_Selector,
        Capsule_Selector,
        If_Then_Else_Expr,
        And_Then,
        Or_Else,
        Given_When_Default_Expr,
        Guard,
        Factorization,
        Expansion,
        Vars,
        New,
        Current,
        Statement,
        Declare,
        Performs,
        If_Then_Else_Stmt,
        Given_When_Default_Stmt,
        Block,
        Leave,
        Iterate,
        Heading,
        Attr_Name,
        Attr_Name_List,
        Local_Name,
        Absolute_Name,
        Routine_Call,
        Function_Call,
        Function_Call_But_0,
        Function_Call_But_0_1,
        Procedure_Call,
    };

    // Muldis.D.Ref_Eng.Core.MD_Any
    // Represents a Muldis D "value", which is an individual constant that
    // is not fixed in time or space.  Every Muldis D value is unique,
    // eternal, and immutable; it has no address and can not be updated.
    // Several distinct MD_Any objects may denote the same Muldis D
    // "value"; however, any time that two MD_Any are discovered to
    // denote the same Muldis D value, any references to one may be safely
    // replaced by references to the other.
    // As a primary aid in implementing the "flyweight pattern", a MD_Any
    // object is actually a lightweight "value handle" pointing to a
    // separate "value struct" object with its components; this allows us
    // to get as close as easily possible to replacing references to one
    // MD_Any with another where the underlying virtual machine or
    // garbage collector doesn't natively provide that ability.
    // Similarly, proving equality of two "value" can often short-circuit.
    // While MD_Any are immutable from a user's perspective, their
    // components may in fact mutate for memory sharing or consolidating.
    // Iff a Muldis D "value" is a "Handle" then it references something
    // that possibly can mutate, such as a Muldis D "variable".

    internal class MD_Any
    {
        internal MD_Any_Struct AS { get; set; }
    }

    internal class MD_Any_Comparer : EqualityComparer<MD_Any>
    {
        public override Boolean Equals(MD_Any v1, MD_Any v2)
        {
            if (v1 == null && v2 == null)
            {
                // Would we ever get here?
                return true;
            }
            if (v1 == null || v2 == null)
            {
                return false;
            }
            if (Object.ReferenceEquals(v1, v2))
            {
                return true;
            }
            return false;  // TODO; meanwhile we compare like a reference type
        }

        public override Int32 GetHashCode(MD_Any v)
        {
            if (v == null)
            {
                // Would we ever get here?
                return 0;
            }
            if (v.AS.Cached_MD_Any_Identity == null)
            {
                v.AS.Cached_MD_Any_Identity = new Codepoint_Array(""); // TODO, fix this
            }
            return v.AS.Cached_MD_Any_Identity.GetHashCode();
        }
    }

    internal class MD_Any_Struct
    {
        // Memory pool this Muldis D "value" lives in.
        internal Memory Memory { get; set; }

        // Muldis D Foundation data type (MDFT) this "value" is a member of.
        // This field determines how to interpret most of the other fields.
        // Some of these types have their own subset of specialized
        // representation formats for the sake of optimization.
        internal MD_Foundation_Type MD_Foundation_Type { get; set; }

        // Iff MDFT is MD_Boolean, this field is the payload.
        internal Boolean MD_Boolean { get; set; }

        // Iff MDFT is MD_Integer, this field is the payload.
        // While we conceptually could special case smaller integers with
        // additional fields for performance, we won't, mainly to keep
        // things simpler, and because BigInteger special-cases internally.
        internal BigInteger MD_Integer { get; set; }

        // Iff MDFT is MD_Array, this field is the payload.
        internal MD_Array_Node MD_Array { get; set; }

        // Iff MDFT is MD_Bag, this field is the payload.
        internal MD_Bag_Node MD_Bag { get; set; }

        // Iff MDFT is MD_Tuple, this field is the payload.
        internal MD_Tuple_Struct MD_Tuple { get; set; }

        // Iff MDFT is MD_Capsule, this field is the payload.
        internal MD_Capsule_Struct MD_Capsule { get; set; }

        // Iff MDFT is MD_Handle, this field is the payload.
        internal MD_Handle_Struct MD_Handle { get; set; }

        // Set of well-known Muldis D types that this value is known to be
        // a member of.  This is calculated semi-lazily as needed.
        internal HashSet<MD_Well_Known_Type> Cached_WKT { get; set; }

        // Normalized serialization of the Muldis D "value" that its host
        // MD_Any_Struct represents.  This is calculated lazily if needed,
        // typically when the "value" is a member of an indexed collection.
        // The serialization format either is or resembles a Muldis D Plain Text
        // literal for selecting the value, in the form of character strings
        // whose character codepoints are typically in the 0..127 range.
        internal Codepoint_Array Cached_MD_Any_Identity { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.Widest_Component_Type
    // Enumerates the levels of restriction that a collection's elements
    // would conform to, which can help set an optimized storage strategy.
    // Unrestricted means any Muldis D value at all may be a component.
    // None means no component is allowed; it is for empty collections.

    internal enum Widest_Component_Type
    {
        None,
        Unrestricted,
        Bit,
        Octet,
        Codepoint,
    }

    // Muldis.D.Ref_Eng.Core.MD_Array_Node
    // When a Muldis.D.Ref_Eng.Core.MD_Any is representing a MD_Array,
    // an MD_Array_Node is used by it to hold the MD_Array-specific details.
    // It takes the form of a tree of its own kind to aid in reusability
    // of common substrings of members of distinct MD_Array values;
    // the actual members of the MD_Array value are, in order, any members
    // specified by Pred_Members, then any Local_*_Members, then
    // Succ_Members.  A MD_Array_Node also uses run-length encoding to
    // optimize storage, such that the actual "local" members of a node are
    // defined as the sequence in Local_*_Members repeated the number of
    // times specified by Local_Multiplicity.
    // The "tree" is actually a uni-directional graph as multiple nodes can
    // cite the same other conceptually immutable nodes as their children.

    internal class MD_Array_Node
    {
        // Cached count of members of the Muldis D Array represented by
        // this tree node including those defined by it and child nodes.
        // Equals count(Local_*_Members) x Local_Multiplicity
        // + Pred_Members.Cached_Tree_Member_Count + Succ_Members.Cached_Tree_Member_Count.
        internal Nullable<Int64> Cached_Tree_Member_Count { get; set; }

        // Nullable Boolean
        // This is true iff we know that no 2 members of the Muldis D Array
        // represented by this tree node (including child nodes) are the
        // same value, and false iff we know that at least 2 members are
        // the same value.
        internal Nullable<Boolean> Cached_Tree_All_Unique { get; set; }

        // Cached indication of the widest Local_Widest_Type in the tree,
        // and thus of the over-all type of the tree.
        internal Widest_Component_Type Tree_Widest_Type { get; set; }

        // Iff this is zero, then there are zero "local" members;
        // iff this is 1, then the "local" members are as Local_*_Members
        // specifies with no repeats;
        // iff this is >1, then the local members have that many occurrances.
        internal Int64 Local_Multiplicity { get; set; }

        // LWT indicates which of the Local_*_Members this node is using.
        // This is None iff Local_Multiplicity is zero.
        internal Widest_Component_Type Local_Widest_Type { get; set; }

        // Iff LWT is Unrestricted, this field is the payload.
        // Represents a Muldis D Array value where each member value is
        // unrestricted in allowed type.
        // A List<MD_Any> is the simplest storage representation for that
        // type which doesn't internally use trees for sharing or multipliers.
        internal List<MD_Any> Local_Unrestricted_Members { get; set; }

        // Iff LWT is Bit, this field is the payload.
        // Represents a Muldis D Array value where each member value is
        // a Muldis D Integer in the range 0..1.
        // A BitArray is the simplest storage representation for that
        // type which doesn't internally use trees for sharing or multipliers.
        // This is the canonical storage type for a regular bit string.
        // TODO: Consider rolling this into Local_Octet_Members instead,
        // where one octet represents one bit.
        internal BitArray Local_Bit_Members { get; set; }

        // Iff LWT is Octet, this field is the payload.
        // Represents a Muldis D Array value where each member value is
        // a Muldis D Integer in the range 0..255.
        // A Byte[] is the simplest storage representation for that
        // type which doesn't internally use trees for sharing or multipliers.
        // This is the canonical storage type for a regular octet (byte) string.
        internal Byte[] Local_Octet_Members { get; set; }

        // Iff LWT is Codepoint, this field is the payload.
        // Represents a Muldis D Array value where each member value is
        // a Muldis D Integer in the range -0x80000000..0x7FFFFFFF.
        // A Codepoint_Array is the simplest storage representation for that
        // type which doesn't internally use trees for sharing or multipliers.
        // This is the canonical storage type for a regular character string.
        internal Codepoint_Array Local_Codepoint_Members { get; set; }

        // Iff there is at least 1 predecessor member of the "local" ones,
        // this subtree says what they are.
        internal MD_Array_Node Pred_Members { get; set; }

        // Iff there is at least 1 successor member of the "local" ones,
        // this subtree says what they are.
        internal MD_Array_Node Succ_Members { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.Symbolic_Value_Type
    // Enumerates the various ways that a collection can be defined
    // symbolically in terms of other collections.
    // None means the collection simply has zero members.

    internal enum Symbolic_Value_Type
    {
        None,
        Singular,
        Arrayed,
        Indexed,
        Unique,
        Insert_N,
        Remove_N,
        Member_Plus,
        Except,
        Intersect,
        Union,
        Exclusive,
    }

    // Muldis.D.Ref_Eng.Core.Multiplied_Member
    // Represents a multiset of 0..N members of a collection where every
    // member is the same Muldis D value.

    internal class Multiplied_Member
    {
        // The Muldis D value that every member of this multiset is.
        internal MD_Any Member { get; set; }

        // The count of members of this multiset.
        internal Int64 Multiplicity { get; set; }

        internal Multiplied_Member(MD_Any member, Int64 multiplicity)
        {
            Member       = member;
            Multiplicity = multiplicity;
        }

        internal Multiplied_Member(MD_Any member)
        {
            Member       = member;
            Multiplicity = 1;
        }
    }

    // Muldis.D.Ref_Eng.Core.MD_Bag_Node
    // When a Muldis.D.Ref_Eng.Core.MD_Any is representing a MD_Bag,
    // a MD_Bag_Node is used by it to hold the MD_Bag-specific details.
    // The "tree" is actually a uni-directional graph as multiple nodes can
    // cite the same other conceptually immutable nodes as their children.
    // Note that MD_Bag is the most complicated Muldis D Foundation type to
    // implement while at the same time is the least critical to the
    // internals; it is mainly just used for user data.

    internal class MD_Bag_Node
    {
        // Cached count of members of the Muldis D Bag represented by
        // this tree node including those defined by it and child nodes.
        internal Nullable<Int64> Cached_Tree_Member_Count { get; set; }

        // Nullable Boolean
        // This is true iff we know that no 2 members of the Muldis D Bag
        // represented by this tree node (including child nodes) are the
        // same value, and false iff we know that at least 2 members are
        // the same value.
        internal Nullable<Boolean> Cached_Tree_All_Unique { get; set; }

        // LST determines how to interpret most of the other fields.
        // Iff LST is Singular, Local_Singular_Members defines all of the Bag members.
        // Iff LST is Arrayed, Local_Arrayed_Members defines all of the Bag members.
        // Iff LST is Indexed, Local_Indexed_Members defines all of the Bag members.
        // Iff LST is Unique, this Bag's members are defined as
        // the unique members of Primary_Arg.
        // Iff LST is Insert_N, this Bag's members are defined as
        // the multiset sum of Primary_Arg and Local_Singular_Members.
        // Iff LST is Remove_N, this Bag's members are defined as
        // the multiset difference of Primary_Arg and Local_Singular_Members.
        // Iff LST is Member_Plus, this Bag's members are defined as
        // the multiset sum of Primary_Arg and Extra_Arg.
        // Iff LST is Except, this Bag's members are defined as
        // the multiset difference of Primary_Arg and Extra_Arg.
        // Iff LST is Intersect, this Bag's members are defined as
        // the multiset intersection of Primary_Arg and Extra_Arg.
        // Iff LST is Union, this Bag's members are defined as
        // the multiset union of Primary_Arg and Extra_Arg.
        // Iff LST is Exclusive, this Bag's members are defined as
        // the multiset symmetric difference of Primary_Arg and Extra_Arg.
        internal Symbolic_Value_Type Local_Symbolic_Type { get; set; }

        // Cached count of members defined by the Local_*_Members fields as
        // they are defined in isolation, meaning it is positive (or zero)
        // even when LST is Remove_N.
        internal Nullable<Int64> Cached_Local_Member_Count { get; set; }

        // Nullable Boolean
        // This is true iff we know that no 2 members of the Muldis D Bag
        // represented by this tree node (specifically the Local_*_Members
        // fields as they are defined in isolation) are the same value, and
        // false iff we know that at least 2 members are the same value.
        internal Nullable<Boolean> Cached_Local_All_Unique { get; set; }

        // This field is used iff LST is one of {Singular, Insert_N, Remove_N}.
        internal Multiplied_Member Local_Singular_Members { get; set; }

        // This field is used iff LST is Arrayed.
        internal List<Multiplied_Member> Local_Arrayed_Members { get; set; }

        // This field is used iff LST is Indexed.
        // The Dictionary has one key-asset pair for each distinct Muldis D
        // "value", all of which are indexed by Cached_MD_Any_Identity.
        internal Dictionary<MD_Any,Multiplied_Member>
            Local_Indexed_Members { get; set; }

        // This field is used iff LST is one of {Unique, Insert_N, Remove_N,
        // Member_Plus, Except, Intersect, Union, Exclusive}.
        internal MD_Bag_Node Primary_Arg { get; set; }

        // This field is used iff LST is one of
        // {Member_Plus, Except, Intersect, Union, Exclusive}.
        internal MD_Bag_Node Extra_Arg { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.MD_Tuple_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Any is representing a MD_Tuple,
    // a MD_Tuple_Struct is used by it to hold the MD_Tuple-specific details.
    // For efficiency, a few most-commonly used Muldis D Tuple attribute
    // names have their own corresponding MD_Tuple_Struct fields, while the
    // long tail of remaining possible but less often used names don't.
    // For example, all routine argument lists of degree 0..3 with just
    // conceptually-ordered arguments are fully covered with said few,
    // including nearly all routines of the Muldis D Standard Library.

    internal class MD_Tuple_Struct
    {
        // Cached count of attributes of the Muldis D Tuple.
        internal Int32 Degree { get; set; }

        // Iff Muldis D Tuple has attr named [0], this field has its asset.
        // This is the canonical name of a first conceptually-ordered attr.
        internal MD_Any A0 { get; set; }

        // Iff Muldis D Tuple has attr named [1], this field has its asset.
        // This is the canonical name of a second conceptually-ordered attr.
        internal MD_Any A1 { get; set; }

        // Iff Muldis D Tuple has attr named [2], this field has its asset.
        // This is the canonical name of a third conceptually-ordered attr.
        internal MD_Any A2 { get; set; }

        // Iff Muldis D Tuple has exactly 1 attribute with some other name
        // than the [N] ones handled above, this other attr is represented
        // by this field as a of name-asset pair.
        internal Nullable<KeyValuePair<Codepoint_Array,MD_Any>> Only_OA { get; set; }

        // Iff Muldis D Tuple has at least 2 attributes with some other name
        // than the [N] ones handled above, those other attrs are represented
        // by this field as a set of name-asset pairs.
        internal Dictionary<Codepoint_Array,MD_Any> Multi_OA { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.MD_Capsule_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Any is representing a MD_Capsule,
    // a MD_Capsule_Struct is used by it to hold the MD_Capsule-specific details.

    internal class MD_Capsule_Struct
    {
        // The Muldis D value that is the "label" of this MD_Capsule value.
        internal MD_Any Label { get; set; }

        // The Muldis D value that is the "attributes" of this MD_Capsule value.
        internal MD_Any Attrs { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.MD_Handle_Type
    // Enumerates the Muldis D Handle data types, which are for now mutually
    // exclusive; every Muldis D Handle value is a member of exactly one of
    // these, though that is subject to change in the future.

    internal enum MD_Handle_Type
    {
        MD_Variable,
        MD_Process,
        MD_Stream,
        MD_External,
    };

    // Muldis.D.Ref_Eng.Core.MD_Handle_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Any is representing a MD_Handle,
    // a MD_Reference_Struct is used by it to hold the MD_Handle-specific details.

    internal class MD_Handle_Struct
    {
        // Muldis D Handle data type (MDHT) this Handle "value" is a member of.
        // This field determines how to interpret most of the other fields.
        // Some of these types have their own subset of specialized
        // representation formats for the sake of optimization.
        internal MD_Handle_Type MD_Handle_Type { get; set; }

        // Iff MDHT is MD_Variable, this field is the payload.
        internal MD_Variable_Struct MD_Variable { get; set; }

        // Iff MDHT is MD_Process, this field is the payload.
        internal MD_Process_Struct MD_Process { get; set; }

        // Iff MDHT is MD_Stream, this field is the payload.
        internal MD_Stream_Struct MD_Stream { get; set; }

        // Iff MDHT is MD_External, this field is the payload.
        internal MD_External_Struct MD_External { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.MD_Variable_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Any is representing a MD_Variable,
    // a MD_Variable_Struct is used by it to hold the MD_Variable-specific details.
    // Represents a Muldis D "variable", which is a container for an
    // appearance of a value.  A Muldis D variable can be created,
    // destroyed, copied, and mutated.  A variable's fundamental identity
    // is its address, its identity does not vary with what value appears there.

    internal class MD_Variable_Struct
    {
        internal MD_Any Current_Value { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.MD_Process_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Any is representing a MD_Process,
    // a MD_Process_Struct is used by it to hold the MD_Process-specific details.

    internal class MD_Process_Struct
    {
    }

    // Muldis.D.Ref_Eng.Core.MD_Stream_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Any is representing a MD_Stream,
    // a MD_Stream_Struct is used by it to hold the MD_Stream-specific details.

    internal class MD_Stream_Struct
    {
    }

    // Muldis.D.Ref_Eng.Core.MD_External_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Any is representing a MD_External,
    // a MD_External_Struct is used by it to hold the MD_External-specific details.

    internal class MD_External_Struct
    {
        // The entity that is defined and managed externally to the Muldis
        // D language environment, which the MD_External value is an opaque
        // and transient reference to.
        internal Object Value { get; set; }
    }
}
