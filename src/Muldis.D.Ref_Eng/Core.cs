namespace Muldis.D.Ref_Eng.Core
{
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

        public Memory ()
        {
            _false = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.Boolean,
                Boolean = false
            } );

            _true = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.Boolean,
                Boolean = true
            } );

            _zero = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.Integer,
                BigInteger = 0
            } );

            _one = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.Integer,
                BigInteger = 1
            } );

            _neg_one = _v( new Value_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.Integer,
                BigInteger = -1
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
            if (value.VS.MD_Foundation_Type != MD_Foundation_Type.Boolean)
            {
                throw new System.ArgumentException
                (
                    paramName: "value",
                    message: "MD_Value is not a Muldis D Boolean"
                );
            }
            return value.VS.Boolean;
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
                    MD_Foundation_Type = MD_Foundation_Type.Integer,
                    BigInteger = value
                } );
            }
        }

        public System.Numerics.BigInteger MD_Integer_as_BigInteger (MD_Value value)
        {
            if (value.VS.MD_Foundation_Type != MD_Foundation_Type.Integer)
            {
                throw new System.ArgumentException
                (
                    paramName: "value",
                    message: "MD_Value is not a Muldis D Integer"
                );
            }
            return value.VS.BigInteger;
        }
    }

    // Muldis.D.Ref_Eng.Core.MD_Value
    // Represents a Muldis D "value", which is an individual constant that,
    // from a user's perspective, is immutable; internally one may be
    // mutated for the purpose of sharing memory per "flyweight".
    // Specifically, this class represents a "value handle", but the name
    // doesn't have "handle" in it for brevity and abstraction purposes.
    // The use of distinct "handle" and "struct" objects is implementing a
    // low-hanging fruit for "flyweight"; any time it is discovered that
    // two "handle" represent the same Muldis D value but have distinct
    // "struct" objects, both "handle" are made to use the same "struct";
    // similarly, proving equality of two "value" can often short-circuit.
    public class MD_Value
    {
        public Value_Struct VS { get; set; }
    }

    // Muldis.D.Ref_Eng.Core.MD_Foundation_Type
    // Used in a Value_Struct as the latter's primary field, which says how
    // to interpret the rest of the fields.  MD_Foundation_Type maps 1:1
    // with the set of Muldis D Foundation data types, which are mutually
    // exclusive.  Some of these types have their own subset of specialized
    // representation formats for the sake of optimization.
    public enum MD_Foundation_Type
    {
        Boolean,
        Integer,
        Array,
        Bag,
        Tuple,
        Capsule,
        Reference,
        External
    };

    // Muldis.D.Ref_Eng.Core.Value_Struct
    // The actual details of a Muldis D "value" representation.
    public class Value_Struct
    {
        // Memory pool this Muldis D "value" lives in.
        public Memory Memory { get; set; }

        public MD_Foundation_Type MD_Foundation_Type { get; set; }

        public System.Boolean Boolean { get; set; }

        // Note: BigInteger was introduced in .Net 4.0 apparently.
        // Note: BigInteger internally uses a signed int to store 32-bit
        // values and a bit array for larger-magnitude integers, or
        // otherwise it is designed to special case small numbers.
        public System.Numerics.BigInteger BigInteger { get; set; }
    }
}
