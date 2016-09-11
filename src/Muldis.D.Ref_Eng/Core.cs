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

    public enum MD_Array_Member_Type
    {
        MD_None,
        MD_Any,
        MD_Integer,
        MD_Unicode_Code,
        MD_Octet
    }

    public class Array_Node
    {
        public MD_Array_Member_Type Type_Each_Local_Member { get; set; }
        public MD_Value[] Local_Any_Members { get; set; }
        public System.Numerics.BigInteger[] Local_Integer_Members { get; set; }
        public uint[] Local_Unicode_Code_Members { get; set; }
        public byte[] Local_Octet_Members { get; set; }
        public System.Numerics.BigInteger Local_Multiplicity { get; set; }
        public Array_Node Pred_Members { get; set; }
        public Array_Node Succ_Members { get; set; }
        public System.Numerics.BigInteger Member_Count { get; set; }
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
                MD_Array = new Array_Node()
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
