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
        private static readonly System.Numerics.BigInteger _BI_zero
            = new System.Numerics.BigInteger(0);

        private readonly Value _false;
        private readonly Value _true;
        private readonly Value _zero;
        private readonly Value _one;
        private readonly Value _neg_one;

        public Memory ()
        {
            _false = _v( new Value_Struct(
                Memory: this,
                Kind: Value_Struct_Kind.Boolean,
                Bool: false,
                BigInt: _BI_zero
            ) );

            _true = _v( new Value_Struct(
                Memory: this,
                Kind: Value_Struct_Kind.Boolean,
                Bool: true,
                BigInt: _BI_zero
            ) );

            _zero = _v( new Value_Struct(
                Memory: this,
                Kind: Value_Struct_Kind.Integer,
                Bool: false,
                BigInt: new System.Numerics.BigInteger(0)
            ) );

            _one = _v( new Value_Struct(
                Memory: this,
                Kind: Value_Struct_Kind.Integer,
                Bool: false,
                BigInt: new System.Numerics.BigInteger(1)
            ) );

            _neg_one = _v( new Value_Struct(
                Memory: this,
                Kind: Value_Struct_Kind.Integer,
                Bool: false,
                BigInt: new System.Numerics.BigInteger(-1)
            ) );
        }

        private Value _v (Value_Struct vs)
        {
            return new Value { VS = vs };
        }

        public Value v_Boolean (bool value)
        {
            return value ? _true : _false;
        }

        public bool v_Boolean_as_bool (Value value)
        {
            if (value.VS.Kind != Value_Struct_Kind.Boolean)
            {
                throw new System.ArgumentException
                (
                    paramName: "value",
                    message: "Value is not a Muldis D Boolean"
                );
            }
            return value.VS.Bool;
        }

        public Value v_Integer (System.Numerics.BigInteger value)
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
                return _v( new Value_Struct(
                    Memory: this,
                    Kind: Value_Struct_Kind.Integer,
                    Bool: false,
                    BigInt: value
                ) );
            }
        }

        public System.Numerics.BigInteger v_Integer_as_BigInteger (Value value)
        {
            if (value.VS.Kind != Value_Struct_Kind.Integer)
            {
                throw new System.ArgumentException
                (
                    paramName: "value",
                    message: "Value is not a Muldis D Integer"
                );
            }
            return value.VS.BigInt;
        }
    }

    // Muldis.D.Ref_Eng.Core.Value
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
    public class Value
    {
        public Value_Struct VS { get; set; }

        // Memory pool this Muldis D "value" lives in.
        public Memory Memory()
        {
            return VS.Memory;
        }
    }

    // Muldis.D.Ref_Eng.Core.Value_Struct_Kind
    // Used in a Value_Struct as the latter's primary field, which says how
    // to interpret the rest of the fields.  Value_Struct_Kind maps 1:1
    // with the set of Muldis D Foundation data types, which are mutually
    // exclusive.  Some of these types have their own subset of specialized
    // representation formats for the sake of optimization.
    public enum Value_Struct_Kind
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
        private readonly Memory _Memory;
        public Memory Memory { get { return _Memory; } }

        private readonly Value_Struct_Kind _Kind;
        public Value_Struct_Kind Kind { get { return _Kind; } }

        private readonly bool _Bool;
        public bool Bool { get { return _Bool; } }

        // Note: BigInteger was introduced in .Net 4.0 apparently.
        // Note: BigInteger internally uses a signed int to store 32-bit
        // values and a bit array for larger-magnitude integers, or
        // otherwise it is designed to special case small numbers.
        private readonly System.Numerics.BigInteger _BigInt;
        public System.Numerics.BigInteger BigInt { get { return _BigInt; } }

        public Value_Struct
        (
            Memory Memory,
            Value_Struct_Kind Kind,
            bool Bool,
            System.Numerics.BigInteger BigInt
        )
        {
            _Memory = Memory;
            _Kind = Kind;
            _Bool = Bool;
            _BigInt = BigInt;
        }
    }
}
