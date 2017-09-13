using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Muldis.D.Ref_Eng.Core
{
    // Muldis.D.Ref_Eng.Core.MD_MSBT
    // Enumerates Muldis D base types that are considered well-known to
    // this Muldis D language implementation, and that in particular have
    // their own dedicated handling code or formats in the implementation.
    // Every one of these types is either disjoint from or a proper subtype
    // or proper supertype of each of the others of these types, and in
    // particular many of these are proper subtypes of Capsule; however,
    // every Muldis D value is considered to have a best fit into exactly
    // one of these types and that value is expected to be normalized
    // towards its best fit storage format.

    internal enum MD_Well_Known_Base_Type
    {
        MD_Boolean,
        MD_Integer,
        MD_Bits,
        MD_Blob,
        MD_Text,
        MD_Array,
        MD_Bag,
        MD_Tuple,
        MD_Capsule,
        MD_Variable,
        MD_Process,
        MD_Stream,
        MD_External,
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

        // Muldis D most specific well known base data type (MSBT) this
        // "value" is a member of.  Determines interpreting Details field.
        // Some of these types have their own subset of specialized
        // representation formats for the sake of optimization.
        internal MD_Well_Known_Base_Type MD_MSBT { get; set; }

        // Details of this Muldis D "value", in one of several possible
        // specialized representation formats depending on the data type.
        // Iff MSBT is MD_Boolean, this field holds a Nullable<Boolean>.
        // Iff MSBT is MD_Integer, this field holds a BigInteger.
            // While we conceptually could special case smaller integers with
            // additional fields for performance, we won't, mainly to keep
            // things simpler, and because BigInteger special-cases internally.
        // Iff MSBT is MD_Bits, this field holds a BitArray.
            // Consider a MD_Bits_Struct if we want symbolic like MD_Array.
        // Iff MSBT is MD_Blob, this field holds a Byte[].
            // Consider a MD_Blob_Struct if we want symbolic like MD_Array.
        // Iff MSBT is MD_Text, this field holds a MD_Text_Struct.
        // Iff MSBT is MD_Array, this field holds a MD_Array_Struct.
        // Iff MSBT is MD_Bag, this field holds a MD_Bag_Struct.
        // Iff MSBT is MD_Tuple, this field holds a MD_Tuple_Struct.
        // Iff MSBT is MD_Capsule, this field holds a MD_Capsule_Struct.
        // Iff MSBT is MD_Variable, this field holds a MD_Variable_Struct.
        // Iff MSBT is MD_Process, this field holds a MD_Process_Struct.
        // Iff MSBT is MD_Stream, this field holds a MD_Stream_Struct.
        // Iff MSBT is MD_External, this field holds a MD_External_Struct.
        internal Object Details { get; set; }

        // Set of well-known Muldis D types that this value is known to be
        // a member of.  This is calculated semi-lazily as needed.
        // This set excludes on purpose the subset of well-known types that
        // should be trivial to test membership of by other means; in
        // particular it excludes {Any,None}, the MD_Well_Known_Base_Type;
        // types not excluded are more work to test.
        internal HashSet<MD_Well_Known_Type> Cached_WKT { get; set; }

        // Normalized serialization of the Muldis D "value" that its host
        // MD_Any_Struct represents.  This is calculated lazily if needed,
        // typically when the "value" is a member of an indexed collection.
        // The serialization format either is or resembles a Muldis D Plain Text
        // literal for selecting the value, in the form of character strings
        // whose character codepoints are typically in the 0..127 range.
        internal String Cached_MD_Any_Identity { get; set; }

        internal Nullable<Boolean> MD_Boolean()
        {
            return (Nullable<Boolean>)Details;
        }

        internal BigInteger MD_Integer()
        {
            return (BigInteger)Details;
        }

        internal BitArray MD_Bits()
        {
            return (BitArray)Details;
        }

        internal Byte[] MD_Blob()
        {
            return (Byte[])Details;
        }

        internal MD_Text_Struct MD_Text()
        {
            return (MD_Text_Struct)Details;
        }

        internal MD_Array_Struct MD_Array()
        {
            return (MD_Array_Struct)Details;
        }

        internal MD_Bag_Struct MD_Bag()
        {
            return (MD_Bag_Struct)Details;
        }

        internal MD_Tuple_Struct MD_Tuple()
        {
            return (MD_Tuple_Struct)Details;
        }

        internal MD_Capsule_Struct MD_Capsule()
        {
            return (MD_Capsule_Struct)Details;
        }

        internal MD_Variable_Struct MD_Variable()
        {
            return (MD_Variable_Struct)Details;
        }

        internal MD_Process_Struct MD_Process()
        {
            return (MD_Process_Struct)Details;
        }

        internal MD_Stream_Struct MD_Stream()
        {
            return (MD_Stream_Struct)Details;
        }

        internal MD_External_Struct MD_External()
        {
            return (MD_External_Struct)Details;
        }
    }

    // Muldis.D.Ref_Eng.Core.MD_Text_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Any is representing a MD_Text,
    // an MD_Text_Struct is used by it to hold the MD_Text-specific details.

    internal class MD_Text_Struct
    {
        // Represents a Muldis D Text value where each member value is
        // a Muldis D Integer in the range {0..0xD7FF,0xE000..0x10FFFF}.
        // A .Net String is the simplest storage representation for that
        // type which doesn't internally use trees for sharing or multipliers.
        // This is the canonical storage type for a regular character string.
        // Each logical member represents a single Unicode standard character
        // codepoint from either the Basic Multilingual Plane (BMP), which
        // is those member values in the range {0..0xD7FF,0xE000..0xFFFF},
        // or from either of the 16 supplementary planes, which is those
        // member values in the range {0x10000..0x10FFFF}.
        // Codepoint_Members is represented using a standard .Net
        // String value for simplicity but a String has a different native
        // concept of components; it is formally an array of .Net Char
        // each of which is either a whole BMP codepoint or half of a
        // non-BMP codepoint; a non-BMP codepoint is represented by a pair
        // of consecutive Char with numeric values in {0xD800..0xDFFF};
        // therefore, the native "length" of a String only matches the
        // "length" of the Muldis D Text when all codepoints are in the BMP.
        // While it is possible for a .Net String to contain an isolated
        // "surrogate" Char outside of a proper "surrogate pair", both
        // Muldis.DBP and Muldis.D.Ref_Eng forbid such a malformed String
        // from either being used internally or being passed in by the API.
        internal String Codepoint_Members { get; set; }

        // Nullable Boolean
        // This is true iff we know that at least 1 Codepoint member is NOT
        // in the Basic Multilingual Plane (BMP); this is false iff we know
        // that there is no such Codepoint member.  That is, with respect
        // to a .Net String, this is true iff we know the String has at least
        // 1 "surrogate pair".  We cache this knowledge because a .Net String
        // with any non-BMP Char is more complicated to count the members
        // of or access members by ordinal position or do some other stuff.
        // This field is always defined as a side-effect of Muldis.D.Ref_Eng
        // forbidding a malformed String and so they are always tested at
        // the borders, that test also revealing if a String has non-BMP chars.
        internal Boolean Has_Any_Non_BMP { get; set; }

        // Cached count of codepoint members of the Muldis D Text.
        internal Nullable<Int64> Cached_Member_Count { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.Symbolic_Array_Type
    // Enumerates the various ways that a MD_Array collection can be defined
    // symbolically in terms of other collections.
    // None means the collection simply has zero members.

    internal enum Symbolic_Array_Type
    {
        None,
        Arrayed,
        Catenated,
    }

    // Muldis.D.Ref_Eng.Core.MD_Array_Struct
    // TODO: Refactor MD_Array_Struct to be more like MD_Bag_Struct.
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

        // LST determines how to interpret most of the other fields.
        // Iff LST is None, this node is explicitly a leaf node defining zero members.
        // Iff LST is Arrayed, Local_Arrayed_Members, combined with
        // Local_Multiplicity, defines all of the Array members.
        // Iff LST is Catenated, this Array's members are defined as
        // the catenation of Pred_Members and Succ_Members.
        // TODO: Refactor MD_Array_Struct to be more like MD_Bag_Struct.
        internal Symbolic_Array_Type Local_Symbolic_Type { get; set; }

        // Iff this is zero, then there are zero "local" members;
        // iff this is 1, then the "local" members are as Local_*_Members
        // specifies with no repeats;
        // iff this is >1, then the local members have that many occurrances.
        internal Int64 Local_Multiplicity { get; set; }

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

        // This field is used iff LST is Arrayed.
        // A List<MD_Any> is the simplest storage representation for that
        // type which doesn't internally use trees for sharing or multipliers.
        internal List<MD_Any> Local_Arrayed_Members { get; set; }

        // This field is used iff LST is Catenated.
        internal MD_Array_Struct Pred_Members { get; set; }

        // This field is used iff LST is Catenated.
        internal MD_Array_Struct Succ_Members { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.Symbolic_Bag_Type
    // Enumerates the various ways that a MD_Bag collection can be defined
    // symbolically in terms of other collections.
    // None means the collection simply has zero members.

    internal enum Symbolic_Bag_Type
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
        internal Symbolic_Bag_Type Local_Symbolic_Type { get; set; }

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
