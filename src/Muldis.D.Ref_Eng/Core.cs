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
        MD_Reference,
        MD_External
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
        Int8u,
        Int32u
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

    // Muldis.D.Ref_Eng.Core.Array_Int8u
    // Represents a Muldis D Array value where each member value is
    // a Muldis D Integer in the range 0..255.
    // An Array_Int8u is the simplest storage representation for that
    // type which doesn't internally use trees for sharing or multipliers.

    public class Array_Int8u
    {
        public System.Collections.Generic.List<System.Byte> Members { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.Array_Int32u
    // Represents a Muldis D Array value where each member value is
    // a Muldis D Integer in the range 0..0xFFFFFFFF.
    // An Array_Int32u is the simplest storage representation for that
    // type which doesn't internally use trees for sharing or multipliers.

    public class Array_Int32u
    {
        public System.Collections.Generic.List<System.UInt32> Members { get; set; }
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
        public System.UInt64 Tree_Member_Count { get; set; }

        // Cached indication of the widest Local_Widest_Type in the tree,
        // and thus of the over-all type of the tree.
        public Widest_Component_Type Tree_Widest_Type { get; set; }

        // Iff this is zero, then there are zero "local" members;
        // iff this is 1, then the "local" members are as Local_*_Members
        // specifies with no repeats;
        // iff this is >1, then the local members have that many occurrances.
        public System.UInt64 Local_Multiplicity { get; set; }

        // LWT indicates which of the Local_*_Members this node is using.
        // This is None iff Local_Multiplicity is zero.
        public Widest_Component_Type Local_Widest_Type { get; set; }

        // Iff LWT is Unrestricted, this field is the payload.
        public Array_MD_Value Local_Unrestricted_Members { get; set; }

        // Iff LWT is Int8u, this field is the payload.
        public Array_Int8u Local_Int8u_Members { get; set; }

        // Iff LWT is Int32u, this field is the payload.
        public Array_Int32u Local_Int32u_Members { get; set; }

        // Iff there is at least 1 predecessor member of the "local" ones,
        // this subtree says what they are.
        public Array_Node Pred_Members { get; set; }

        // Iff there is at least 1 successor member of the "local" ones,
        // this subtree says what they are.
        public Array_Node Succ_Members { get; set; }
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
        private readonly MD_Value _false;
        private readonly MD_Value _true;
        private readonly MD_Value _zero;
        private readonly MD_Value _one;
        private readonly MD_Value _neg_one;
        private readonly MD_Value _empty_array;

        public Memory ()
        {
            _false = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Boolean,
                MD_Boolean = false
            } );

            _true = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Boolean,
                MD_Boolean = true
            } );

            _zero = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Integer,
                MD_Integer = 0
            } );

            _one = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Integer,
                MD_Integer = 1
            } );

            _neg_one = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Integer,
                MD_Integer = -1
            } );

            _empty_array = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Array,
                MD_Array = new Array_Node {
                    Tree_Member_Count = 0,
                    Tree_Widest_Type = Widest_Component_Type.None,
                    Local_Multiplicity = 0,
                    Local_Widest_Type = Widest_Component_Type.None
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
                    MD_Integer = value
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
