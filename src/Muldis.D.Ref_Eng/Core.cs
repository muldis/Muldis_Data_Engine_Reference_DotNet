namespace Muldis.D.Ref_Eng.Core
{
    // Muldis.D.Ref_Eng.Core.MD_Foundation_Type
    // Enumerates the Muldis D Foundation data types, which are mutually
    // exclusive; every Muldis D value is a member of exactly one of these.

    public enum MD_Foundation_Type
    {
        MD_Boolean,
        MD_Integer,
        MD_Array,
        MD_Bag,
        MD_Tuple,
        MD_Capsule,
        MD_Handle,
    };

    // Muldis.D.Ref_Eng.Core.MD_Value
    // Represents a Muldis D "value", which is an individual constant that
    // is not fixed in time or space.  Every Muldis D value is unique,
    // eternal, and immutable; it has no address and can not be updated.
    // Several distinct MD_Value objects may denote the same Muldis D
    // "value"; however, any time that two MD_Value are discovered to
    // denote the same Muldis D value, any references to one may be safely
    // replaced by references to the other.
    // As a primary aid in implementing the "flyweight pattern", a MD_Value
    // object is actually a lightweight "value handle" pointing to a
    // separate "value struct" object with its components; this allows us
    // to get as close as easily possible to replacing references to one
    // MD_Value with another where the underlying virtual machine or
    // garbage collector doesn't natively provide that ability.
    // Similarly, proving equality of two "value" can often short-circuit.
    // While MD_Value are immutable from a user's perspective, their
    // components may in fact mutate for memory sharing or consolidating.
    // Iff a Muldis D "value" is a "Handle" then it references something
    // that possibly can mutate, such as a Muldis D "variable".

    public class MD_Value
    {
        public Value_Struct VS { get; set; }
    }

    public class Value_Struct
    {
        // Memory pool this Muldis D "value" lives in.
        public Memory Memory { get; set; }

        // Muldis D Foundation data type (MDFT) this "value" is a member of.
        // This field determines how to interpret most of the other fields.
        // Some of these types have their own subset of specialized
        // representation formats for the sake of optimization.
        public MD_Foundation_Type MD_Foundation_Type { get; set; }

        // Iff MDFT is MD_Boolean, this field is the payload.
        public System.Boolean MD_Boolean { get; set; }

        // Iff MDFT is MD_Integer, this field is the payload.
        // While we conceptually could special case smaller integers with
        // additional fields for performance, we won't, mainly to keep
        // things simpler, and because BigInteger special-cases internally.
        public System.Numerics.BigInteger MD_Integer { get; set; }

        // Iff MDFT is MD_Array, this field is the payload.
        public Array_Node MD_Array { get; set; }

        // Iff MDFT is MD_Bag, this field is the payload.
        public Bag_Node MD_Bag { get; set; }

        // Iff MDFT is MD_Tuple, this field is the payload.
        public Tuple_Struct MD_Tuple { get; set; }

        // Iff MDFT is MD_Capsule, this field is the payload.
        public Capsule_Struct MD_Capsule { get; set; }

        // Iff MDFT is MD_Handle, this field is the payload.
        public Handle_Struct MD_Handle { get; set; }

        // Normalized serialization of the Muldis D "value" that its host
        // Value_Struct represents.  This is calculated lazily if needed,
        // typically when the "value" is a member of an indexed collection.
        public MD_Value_Identity Cached_MD_Value_Identity { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.Widest_Component_Type
    // Enumerates the levels of restriction that a collection's elements
    // would conform to, which can help set an optimized storage strategy.
    // Unrestricted means any Muldis D value at all may be a component.
    // None means no component is allowed; it is for empty collections.

    public enum Widest_Component_Type
    {
        None,
        Unrestricted,
        Octet,
        Codepoint,
    }

    // Muldis.D.Ref_Eng.Core.Array_MD_Value
    // Represents a Muldis D Array value where each member value is
    // unrestricted in allowed type.
    // An Array_MD_Value is the simplest storage representation for that
    // type which doesn't internally use trees for sharing or multipliers.

    public class Array_MD_Value
    {
        public System.Collections.Generic.List<MD_Value> Members { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.Array_Octet
    // Represents a Muldis D Array value where each member value is
    // a Muldis D Integer in the range 0..255.
    // An Array_Octet is the simplest storage representation for that
    // type which doesn't internally use trees for sharing or multipliers.
    // This is the canonical storage type for a regular octet (byte) string.

    public class Array_Octet
    {
        public System.Collections.Generic.List<System.Byte> Members { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.Array_Codepoint
    // Represents a Muldis D Array value where each member value is
    // a Muldis D Integer in the range 0..0x7FFFFFFF.
    // An Array_Codepoint is the simplest storage representation for that
    // type which doesn't internally use trees for sharing or multipliers.
    // This is the canonical storage type for a regular character string.

    public class Array_Codepoint
    {
        public System.Collections.Generic.List<System.Int32> Members { get; set; }
        // TODO: Consider caching alternate formats, such as C# string or
        // UTF-8 octets as byte[], to avoid unnecessary conversions;
        // however those alternatives will only work for Unicode proper,
        // and not supersets that we otherwise support.
    }

    // Muldis.D.Ref_Eng.Core.Array_Node
    // When a Muldis.D.Ref_Eng.Core.MD_Value is representing a MD_Array,
    // an Array_Node is used by it to hold the MD_Array-specific details.
    // It takes the form of a tree of its own kind to aid in reusability
    // of common substrings of members of distinct MD_Array values;
    // the actual members of the MD_Array value are, in order, any members
    // specified by Pred_Members, then any Local_*_Members, then
    // Succ_Members.  An Array_Node also uses run-length encoding to
    // optimize storage, such that the actual "local" members of a node are
    // defined as the sequence in Local_*_Members repeated the number of
    // times specified by Local_Multiplicity.
    // The "tree" is actually a uni-directional graph as multiple nodes can
    // cite the same other conceptually immutable nodes as their children.

    public class Array_Node
    {
        // Cached count of members of the Muldis D Array represented by
        // this tree node including those defined by it and child nodes.
        // Equals count(Local_*_Members) x Local_Multiplicity
        // + Pred_Members.Tree_Member_Count + Succ_Members.Tree_Member_Count.
        public System.Int64 Tree_Member_Count { get; set; }

        // Cached indication of the widest Local_Widest_Type in the tree,
        // and thus of the over-all type of the tree.
        public Widest_Component_Type Tree_Widest_Type { get; set; }

        // Iff this is zero, then there are zero "local" members;
        // iff this is 1, then the "local" members are as Local_*_Members
        // specifies with no repeats;
        // iff this is >1, then the local members have that many occurrances.
        public System.Int64 Local_Multiplicity { get; set; }

        // LWT indicates which of the Local_*_Members this node is using.
        // This is None iff Local_Multiplicity is zero.
        public Widest_Component_Type Local_Widest_Type { get; set; }

        // Iff LWT is Unrestricted, this field is the payload.
        public Array_MD_Value Local_Unrestricted_Members { get; set; }

        // Iff LWT is Octet, this field is the payload.
        public Array_Octet Local_Octet_Members { get; set; }

        // Iff LWT is Codepoint, this field is the payload.
        public Array_Codepoint Local_Codepoint_Members { get; set; }

        // Iff there is at least 1 predecessor member of the "local" ones,
        // this subtree says what they are.
        public Array_Node Pred_Members { get; set; }

        // Iff there is at least 1 successor member of the "local" ones,
        // this subtree says what they are.
        public Array_Node Succ_Members { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.Symbolic_Value_Type
    // Enumerates the various ways that a collection can be defined
    // symbolically in terms of other collections.
    // None means the collection simply has zero members.

    public enum Symbolic_Value_Type
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

    public class Multiplied_Member
    {
        // The Muldis D value that every member of this multiset is.
        public MD_Value Member { get; set; }

        // The count of members of this multiset.
        public System.Int64 Multiplicity { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.Bag_Node
    // When a Muldis.D.Ref_Eng.Core.MD_Value is representing a MD_Bag,
    // a Bag_Node is used by it to hold the MD_Bag-specific details.
    // The "tree" is actually a uni-directional graph as multiple nodes can
    // cite the same other conceptually immutable nodes as their children.
    // Note that MD_Bag is the most complicated Muldis D Foundation type to
    // implement while at the same time is the least critical to the
    // internals; it is mainly just used for user data.

    public class Bag_Node
    {
        // Cached count of members of the Muldis D Bag represented by
        // this tree node including those defined by it and child nodes.
        public System.Int64 Tree_Member_Count { get; set; }

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
        public Symbolic_Value_Type Local_Symbolic_Type { get; set; }

        // Cached count of members defined by the Local_*_Members fields as
        // they are defined in isolation, meaning it is positive (or zero)
        // even when LST is Remove_N.
        public System.Int64 Local_Member_Count { get; set; }

        // This field is used iff LST is one of {Singular, Insert_N, Remove_N}.
        public Multiplied_Member Local_Singular_Members { get; set; }

        // This field is used iff LST is Arrayed.
        public System.Collections.Generic.List<Multiplied_Member> Local_Arrayed_Members { get; set; }

        // This field is used iff LST is Indexed.
        // The Dictionary has one key-asset pair for each distinct Muldis D
        // "value", all of which are indexed by Cached_MD_Value_Identity.
        public System.Collections.Generic
            .Dictionary<MD_Value_Identity,Multiplied_Member>
            Local_Indexed_Members { get; set; }

        // This field is used iff LST is one of {Unique, Insert_N, Remove_N,
        // Member_Plus, Except, Intersect, Union, Exclusive}.
        public Bag_Node Primary_Arg { get; set; }

        // This field is used iff LST is one of
        // {Member_Plus, Except, Intersect, Union, Exclusive}.
        public Bag_Node Extra_Arg { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.Tuple_Attr_Name
    // Represents a Muldis D Tuple attribute name which is a string where
    // each member is a Muldis D Integer in the range 0..0x7FFFFFFF.
    // The associated Tuple_Attr_Name_Comparer class exists to simplify
    // using Tuple_Attr_Name objects as C# Dictionary keys.

    public class Tuple_Attr_Name
    {
        public System.Collections.Generic.List<System.Int32> Members { get; set; }

        public System.Int32 Cached_HashCode { get; set; } = -1;
    }

    public class Tuple_Attr_Name_Comparer
        : System.Collections.Generic.EqualityComparer<Tuple_Attr_Name>
    {
        public override System.Boolean Equals(Tuple_Attr_Name v1, Tuple_Attr_Name v2)
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
            if (System.Object.ReferenceEquals(v1, v2))
            {
                return true;
            }
            if (v1.Members.Count != v2.Members.Count)
            {
                return false;
            }
            if (v1.Members.Count == 0)
            {
                return true;
            }
            return System.Linq.Enumerable.SequenceEqual(v1.Members, v2.Members);
        }

        public override System.Int32 GetHashCode(Tuple_Attr_Name v)
        {
            if (v == null)
            {
                // Would we ever get here?
                return 0;
            }
            if (v.Cached_HashCode == -1)
            {
                // We are assuming that all XOR operations on valid
                // character codepoints would have a zero sign bit,
                // and so -1 would never be a result of any XORing.
                v.Cached_HashCode = System.Linq.Enumerable
                    .Aggregate(v.Members, 0, (m1, m2) => m1 ^ m2)
                    .GetHashCode();
            }
            return v.Cached_HashCode;
        }
    }

    // Muldis.D.Ref_Eng.Core.Tuple_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Value is representing a MD_Tuple,
    // a Tuple_Struct is used by it to hold the MD_Tuple-specific details.
    // For efficiency, a few most-commonly used Muldis D Tuple attribute
    // names have their own corresponding Tuple_Struct fields, while the
    // long tail of remaining possible but less often used names don't.
    // For example, all routine argument lists of degree 0..3 with just
    // conceptually-ordered arguments are fully covered with said few,
    // including nearly all routines of the Muldis D Standard Library.

    public class Tuple_Struct
    {
        // Cached count of attributes of the Muldis D Tuple.
        public System.Int32 Degree { get; set; }

        // Iff Muldis D Tuple has attr named [], this field has its asset.
        // In a Package materials namespace, this name has special meaning.
        // TODO: Probably fold this into Other_Attrs considering its use cases
        // including it likely only used in same Tuple as other named attrs.
        public MD_Value Attr_Named_Empty_Str { get; set; }

        // Iff Muldis D Tuple has attr named [0], this field has its asset.
        // This is the canonical name of a first conceptually-ordered attr.
        public MD_Value Attr_Named_0 { get; set; }

        // Iff Muldis D Tuple has attr named [1], this field has its asset.
        // This is the canonical name of a second conceptually-ordered attr.
        public MD_Value Attr_Named_1 { get; set; }

        // Iff Muldis D Tuple has attr named [2], this field has its asset.
        // This is the canonical name of a third conceptually-ordered attr.
        public MD_Value Attr_Named_2 { get; set; }

        // Iff Muldis D Tuple has at least 1 attribute with some other name
        // than the ones handled above, those other attrs are represented
        // by this field as a set of name-asset pairs.
        public System.Collections.Generic.Dictionary<Tuple_Attr_Name,MD_Value>
            Other_Attrs { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.Capsule_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Value is representing a MD_Capsule,
    // a Capsule_Struct is used by it to hold the MD_Capsule-specific details.

    public class Capsule_Struct
    {
        // The Muldis D value that is the "label" of this MD_Capsule value.
        public MD_Value Label { get; set; }

        // The Muldis D value that is the "attributes" of this MD_Capsule value.
        public MD_Value Attrs { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.MD_Handle_Type
    // Enumerates the Muldis D Handle data types, which are for now mutually
    // exclusive; every Muldis D Handle value is a member of exactly one of
    // these, though that is subject to change in the future.

    public enum MD_Handle_Type
    {
        MD_Variable,
        MD_Process,
        MD_Stream,
        MD_External,
    };

    // Muldis.D.Ref_Eng.Core.Handle_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Value is representing a MD_Handle,
    // a Reference_Struct is used by it to hold the MD_Handle-specific details.

    public class Handle_Struct
    {
        // Muldis D Handle data type (MDHT) this Handle "value" is a member of.
        // This field determines how to interpret most of the other fields.
        // Some of these types have their own subset of specialized
        // representation formats for the sake of optimization.
        public MD_Handle_Type MD_Handle_Type { get; set; }

        // Iff MDHT is MD_Variable, this field is the payload.
        public Variable_Struct MD_Variable { get; set; }

        // TODO: MD_Process, MD_Stream.

        // Iff MDHT is MD_External, this field is the payload.
        public External_Struct MD_External { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.Variable_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Value is representing a MD_Variable,
    // a Variable_Struct is used by it to hold the MD_Variable-specific details.
    // Represents a Muldis D "variable", which is a container for an
    // appearance of a value.  A Muldis D variable can be created,
    // destroyed, copied, and mutated.  A variable's fundamental identity
    // is its address, its identity does not vary with what value appears there.

    public class Variable_Struct
    {
        public MD_Value Current_Value { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.External_Struct
    // When a Muldis.D.Ref_Eng.Core.MD_Value is representing a MD_External,
    // a External_Struct is used by it to hold the MD_External-specific details.

    public class External_Struct
    {
        // The entity that is defined and managed externally to the Muldis
        // D language environment, which the MD_External value is an opaque
        // and transient reference to.
        public System.Object Value { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.MD_Value_Identity
    // Normalized serialization of a Muldis D "value".  This is calculated
    // lazily if needed, typically when the "value" is a member of an
    // indexed collection.
    // The serialization format either is or resembles a Muldis D Plain Text
    // literal for selecting the value, in the form of character strings
    // whose character codepoints are typically in the 0..127 range.

    public class MD_Value_Identity
    {
        public System.Collections.Generic.List<System.Int32> Members { get; set; }

        public System.Int32 Cached_HashCode { get; set; } = -1;
    }

    public class MD_Value_Identity_Comparer
        : System.Collections.Generic.EqualityComparer<MD_Value_Identity>
    {
        public override System.Boolean Equals(MD_Value_Identity v1, MD_Value_Identity v2)
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
            if (System.Object.ReferenceEquals(v1, v2))
            {
                return true;
            }
            if (v1.Members.Count != v2.Members.Count)
            {
                return false;
            }
            if (v1.Members.Count == 0)
            {
                return true;
            }
            return System.Linq.Enumerable.SequenceEqual(v1.Members, v2.Members);
        }

        // When hashing the string we merge each 4 consecutive characters by
        // catenation of each one's lower 8 bits so we can use millions of
        // hash buckets rather than just about 64 buckets (#letters+digits);
        // however it doesn't resist degeneration of hash-based collections
        // such as when many strings used as keys have just 8-character
        // repetitions in the form ABCDABCD and so all resolve to bucket 0.

        public override System.Int32 GetHashCode(MD_Value_Identity v)
        {
            if (v == null)
            {
                // Would we ever get here?
                return 0;
            }
            if (v.Cached_HashCode == -1)
            {
                // We are assuming that all XOR operations on valid
                // character codepoints would have a zero sign bit,
                // and so -1 would never be a result of any XORing.
                v.Cached_HashCode = 0;
                System.Int32[] members = v.Members.ToArray();
                for (System.Int32 i = 0; i < members.Length; i += 4)
                {
                    System.Int32 chunk_size = System.Math.Min(4, members.Length - i);
                    System.Int32 m1 =                  16777216 * (members[i  ] % 128);
                    System.Int32 m2 = (chunk_size <= 2) ? 65536 * (members[i+1] % 256) : 0;
                    System.Int32 m3 = (chunk_size <= 3) ?   256 * (members[i+2] % 256) : 0;
                    System.Int32 m4 = (chunk_size <= 4) ?          members[i+3] % 256  : 0;
                    v.Cached_HashCode = v.Cached_HashCode ^ (m1 + m2 + m3 + m4);
                }
            }
            return v.Cached_HashCode;
        }
    }

    // Muldis.D.Ref_Eng.Core.Memory
    // Provides a virtual machine memory pool where Muldis D values and
    // variables live, which exploits the "flyweight pattern" for
    // efficiency in both performance and memory usage.
    // Conceptually, Memory is a singleton, in that typically only one
    // would be used per virtual machine; in practice, only the set of all
    // VM values/vars/processes/etc that would interact must share one.
    // Memory might have multiple pools, say, for separate handling of
    // entities that are short-lived versus longer-lived.
    public class Memory
    {
        // MD_Boolean False (type default value) and True values.
        private readonly MD_Value _false;
        private readonly MD_Value _true;

        // MD_Integer 0 (type default value) and 1 and -1 values.
        private readonly MD_Value _zero;
        private readonly MD_Value _one;
        private readonly MD_Value _neg_one;

        // MD_Array with no members (type default value).
        private readonly MD_Value _empty_array;

        // MD_Bag with no members (type default value).
        private readonly MD_Value _empty_bag;

        // MD_Tuple with no attributes (type default value).
        private readonly MD_Value _nullary_tuple;

        // MD_Capsule with False label and no attributes (type default value).
        private readonly MD_Value _false_nullary_capsule;

        public Memory ()
        {
            _false = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Boolean,
                MD_Boolean = false,
            } );

            _true = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Boolean,
                MD_Boolean = true,
            } );

            _zero = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Integer,
                MD_Integer = 0,
            } );

            _one = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Integer,
                MD_Integer = 1,
            } );

            _neg_one = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Integer,
                MD_Integer = -1,
            } );

            _empty_array = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Array,
                MD_Array = new Array_Node {
                    Tree_Member_Count = 0,
                    Tree_Widest_Type = Widest_Component_Type.None,
                    Local_Multiplicity = 0,
                    Local_Widest_Type = Widest_Component_Type.None,
                }
            } );

            _empty_bag = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Bag,
                MD_Bag = new Bag_Node {
                    Tree_Member_Count = 0,
                    Local_Symbolic_Type = Symbolic_Value_Type.None,
                    Local_Member_Count = 0,
                }
            } );

            _nullary_tuple = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                MD_Tuple = new Tuple_Struct {
                    Degree = 0,
                }
            } );

            _false_nullary_capsule = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Capsule,
                MD_Capsule = new Capsule_Struct {
                    Label = _false,
                    Attrs = _nullary_tuple,
                }
            } );
        }

        private MD_Value _v (Value_Struct vs)
        {
            return new MD_Value { VS = vs };
        }

        public MD_Value MD_Boolean (System.Boolean value)
        {
            return value ? _true : _false;
        }

        public System.Boolean MD_Boolean_as_Boolean (MD_Value value)
        {
            if (value.VS.MD_Foundation_Type != MD_Foundation_Type.MD_Boolean)
            {
                throw new System.ArgumentException
                (
                    paramName: "value",
                    message: "MD_Value is not a Muldis D Boolean"
                );
            }
            return value.VS.MD_Boolean;
        }

        public MD_Value MD_Integer (System.Numerics.BigInteger value)
        {
            if (value == 0)
            {
                return _zero;
            }
            else if (value == 1)
            {
                return _one;
            }
            else if (value == -1)
            {
                return _neg_one;
            }
            else
            {
                return _v( new Value_Struct {
                    Memory = this,
                    MD_Foundation_Type = MD_Foundation_Type.MD_Integer,
                    MD_Integer = value,
                } );
            }
        }

        public System.Numerics.BigInteger MD_Integer_as_BigInteger (MD_Value value)
        {
            if (value.VS.MD_Foundation_Type != MD_Foundation_Type.MD_Integer)
            {
                throw new System.ArgumentException
                (
                    paramName: "value",
                    message: "MD_Value is not a Muldis D Integer"
                );
            }
            return value.VS.MD_Integer;
        }
    }
}
