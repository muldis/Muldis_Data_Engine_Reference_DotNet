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

        internal Boolean Same(MD_Any value)
        {
            return AS.Memory.Executor.Any__same(this, value);
        }

        internal String MD_Any_Identity()
        {
            // This called function will test if AS.Cached_MD_Any_Identity
            // is null and assign it a value if so and use its value if not.
            return AS.Memory.Identity_Generator.MD_Any_to_Identity_String(this);
        }

        public override String ToString()
        {
            return AS.Memory.Preview_Generator.MD_Any_To_Preview_String(this);
        }
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
            return v1.Same(v2);
        }

        public override Int32 GetHashCode(MD_Any v)
        {
            if (v == null)
            {
                // Would we ever get here?
                return 0;
            }
            return v.MD_Any_Identity().GetHashCode();
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
        internal Nullable<Boolean> MD_Boolean { get; set; }

        // Iff MDFT is MD_Integer, this field is the payload.
        // While we conceptually could special case smaller integers with
        // additional fields for performance, we won't, mainly to keep
        // things simpler, and because BigInteger special-cases internally.
        internal BigInteger MD_Integer { get; set; }

        // Iff MDFT is MD_Array, this field is the payload.
        internal MD_Array_Struct MD_Array { get; set; }

        // Iff MDFT is MD_Bag, this field is the payload.
        internal MD_Bag_Struct MD_Bag { get; set; }

        // Iff MDFT is MD_Tuple, this field is the payload.
        internal MD_Tuple_Struct MD_Tuple { get; set; }

        // Iff MDFT is MD_Capsule, this field is the payload.
        internal MD_Capsule_Struct MD_Capsule { get; set; }

        // Iff MDFT is MD_Handle, this field is the payload.
        internal MD_Handle_Struct MD_Handle { get; set; }

        // Set of well-known Muldis D types that this value is known to be
        // a member of.  This is calculated semi-lazily as needed.
        // This set excludes on purpose the subset of well-known types that
        // should be trivial to test membership of by other means; in
        // particular it excludes {Any,None}, the 7 MD_Foundation_Type,
        // the 4 MD_Handle_Type; types not excluded are more work to test.
        internal HashSet<MD_Well_Known_Type> Cached_WKT { get; set; }

        // Normalized serialization of the Muldis D "value" that its host
        // MD_Any_Struct represents.  This is calculated lazily if needed,
        // typically when the "value" is a member of an indexed collection.
        // The serialization format either is or resembles a Muldis D Plain Text
        // literal for selecting the value, in the form of character strings
        // whose character codepoints are typically in the 0..127 range.
        internal String Cached_MD_Any_Identity { get; set; }
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

    // Muldis.D.Ref_Eng.Core.MD_Array_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Any is representing a MD_Array,
    // an MD_Array_Struct is used by it to hold the MD_Array-specific details.
    // It takes the form of a tree of its own kind to aid in reusability
    // of common substrings of members of distinct MD_Array values;
    // the actual members of the MD_Array value are, in order, any members
    // specified by Pred_Members, then any Local_*_Members, then
    // Succ_Members.  A MD_Array_Struct also uses run-length encoding to
    // optimize storage, such that the actual "local" members of a node are
    // defined as the sequence in Local_*_Members repeated the number of
    // times specified by Local_Multiplicity.
    // The "tree" is actually a uni-directional graph as multiple nodes can
    // cite the same other conceptually immutable nodes as their children.

    internal class MD_Array_Struct
    {
        // Cached count of members of the Muldis D Array represented by
        // this tree node including those defined by it and child nodes.
        // Equals Cached_Local_Member_Count
        // + Pred_Members.Cached_Tree_Member_Count + Succ_Members.Cached_Tree_Member_Count.
        internal Nullable<Int64> Cached_Tree_Member_Count { get; set; }

        // Nullable Boolean
        // This is true iff we know that no 2 members of the Muldis D Array
        // represented by this tree node (including child nodes) are the
        // same value, and false iff we know that at least 2 members are
        // the same value.
        internal Nullable<Boolean> Cached_Tree_All_Unique { get; set; }

        // Nullable Boolean
        // This is true iff we know that the Muldis D Array represented by
        // this tree node (as with Tree_All_Unique) has no member that
        // is not a Muldis D Tuple and has no 2 members that do not have
        // the same heading; this is false iff we know that any member is
        // not a Tuple or that any 2 members do not have the same heading.
        internal Nullable<Boolean> Cached_Tree_Relational { get; set; }

        // Tree_Widest_Type indicates the widest Local_Widest_Type in the
        // tree, and thus of the over-all type of the tree.
        // This can be calculated from other fields, but is always defined.
        internal Widest_Component_Type Tree_Widest_Type { get; set; }

        // Iff this is zero, then there are zero "local" members;
        // iff this is 1, then the "local" members are as Local_*_Members
        // specifies with no repeats;
        // iff this is >1, then the local members have that many occurrances.
        internal Int64 Local_Multiplicity { get; set; }

        // LWT indicates which of the Local_*_Members this node is using.
        // This is None iff Local_Multiplicity is zero.
        internal Widest_Component_Type Local_Widest_Type { get; set; }

        // Cached count of members defined by the Local_*_Members fields as
        // they are defined in isolation.
        // Equals count(Local_*_Members) x Local_Multiplicity.
        internal Nullable<Int64> Cached_Local_Member_Count { get; set; }

        // Nullable Boolean
        // This is true iff we know that no 2 members of the Muldis D Array
        // represented by this tree node (specifically the Local_*_Members
        // fields as they are defined in isolation) are the same value, and
        // false iff we know that at least 2 members are the same value.
        internal Nullable<Boolean> Cached_Local_All_Unique { get; set; }

        // Nullable Boolean
        // This is true iff we know that the Muldis D Array represented by
        // this tree node (as with Local_All_Unique) has no member that
        // is not a Muldis D Tuple and has no 2 members that do not have
        // the same heading; this is false iff we know that any member is
        // not a Tuple or that any 2 members do not have the same heading.
        internal Nullable<Boolean> Cached_Local_Relational { get; set; }

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
        // a Muldis D Integer in the range {0..0xD7FF,0xE000..0x10FFFF}.
        // A .Net String is the simplest storage representation for that
        // type which doesn't internally use trees for sharing or multipliers.
        // This is the canonical storage type for a regular character string.
        // Each logical member represents a single Unicode standard character
        // codepoint from either the Basic Multilingual Plane (BMP), which
        // is those member values in the range {0..0xD7FF,0xE000..0xFFFF},
        // or from either of the 16 supplementary planes, which is those
        // member values in the range {0x10000..0x10FFFF}.
        // Local_Codepoint_Members is represented using a standard .Net
        // String value for simplicity but a String has a different native
        // concept of components; it is formally an array of .Net Char
        // each of which is either a whole BMP codepoint or half of a
        // non-BMP codepoint; a non-BMP codepoint is represented by a pair
        // of consecutive Char with numeric values in {0xD800..0xDFFF};
        // therefore, the native "length" of a String only matches the
        // "length" of the Muldis D Array when all codepoints are in the BMP.
        // While it is possible for a .Net String to contain an isolated
        // "surrogate" Char outside of a proper "surrogate pair", both
        // Muldis.DBP and Muldis.D.Ref_Eng forbid such a malformed String
        // from either being used internally or being passed in by the API.
        internal String Local_Codepoint_Members { get; set; }

        // Nullable Boolean
        // This is true iff we know that at least 1 Codepoint member is NOT
        // in the Basic Multilingual Plane (BMP); this is false iff we know
        // that there is no such Codepoint member.  That is, with respect
        // to a .Net String, this is true iff we know the String has at least
        // 1 "surrogate pair".  We cache this knowledge because a .Net String
        // with any non-BMP Char is more complicated to count the members
        // of or access members by ordinal position or do some other stuff.
        internal Nullable<Boolean> Cached_Local_Any_Non_BMP { get; set; }

        // Iff there is at least 1 predecessor member of the "local" ones,
        // this subtree says what they are.
        internal MD_Array_Struct Pred_Members { get; set; }

        // Iff there is at least 1 successor member of the "local" ones,
        // this subtree says what they are.
        internal MD_Array_Struct Succ_Members { get; set; }
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

        internal Multiplied_Member(MD_Any member, Int64 multiplicity = 1)
        {
            Member       = member;
            Multiplicity = multiplicity;
        }

        internal Multiplied_Member Clone()
        {
            return (Multiplied_Member)this.MemberwiseClone();
        }
    }

    // Muldis.D.Ref_Eng.Core.MD_Bag_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Any is representing a MD_Bag,
    // a MD_Bag_Struct is used by it to hold the MD_Bag-specific details.
    // The "tree" is actually a uni-directional graph as multiple nodes can
    // cite the same other conceptually immutable nodes as their children.
    // Note that MD_Bag is the most complicated Muldis D Foundation type to
    // implement while at the same time is the least critical to the
    // internals; it is mainly just used for user data.

    internal class MD_Bag_Struct
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

        // Nullable Boolean
        // This is true iff we know that the Muldis D Bag represented by
        // this tree node (as with Tree_All_Unique) has no member that
        // is not a Muldis D Tuple and has no 2 members that do not have
        // the same heading; this is false iff we know that any member is
        // not a Tuple or that any 2 members do not have the same heading.
        internal Nullable<Boolean> Cached_Tree_Relational { get; set; }

        // LST determines how to interpret most of the other fields.
        // Iff LST is None, this node is explicitly a leaf node defining zero members.
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

        // Nullable Boolean
        // This is true iff we know that the Muldis D Bag represented by
        // this tree node (as with Local_All_Unique) has no member that
        // is not a Muldis D Tuple and has no 2 members that do not have
        // the same heading; this is false iff we know that any member is
        // not a Tuple or that any 2 members do not have the same heading.
        internal Nullable<Boolean> Cached_Local_Relational { get; set; }

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
        internal MD_Bag_Struct Primary_Arg { get; set; }

        // This field is used iff LST is one of
        // {Member_Plus, Except, Intersect, Union, Exclusive}.
        internal MD_Bag_Struct Extra_Arg { get; set; }
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
        // Count of attributes of the Muldis D Tuple.
        // This can be calculated from other fields, but is always defined.
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
        internal Nullable<KeyValuePair<String,MD_Any>> Only_OA { get; set; }

        // Iff Muldis D Tuple has at least 2 attributes with some other name
        // than the [N] ones handled above, those other attrs are represented
        // by this field as a set of name-asset pairs.
        internal Dictionary<String,MD_Any> Multi_OA { get; set; }
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
