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
    internal class Memory
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

        internal Memory ()
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

        internal MD_Value MD_Boolean (System.Boolean value)
        {
            return value ? _true : _false;
        }

        internal System.Boolean MD_Boolean_as_Boolean (MD_Value value)
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

        internal MD_Value MD_Integer (System.Numerics.BigInteger value)
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

        internal System.Numerics.BigInteger MD_Integer_as_BigInteger (MD_Value value)
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
