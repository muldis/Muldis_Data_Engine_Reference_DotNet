using System.Numerics;

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
        private readonly MD_Any m_false;
        private readonly MD_Any m_true;

        // MD_Integer 0 (type default value) and 1 and -1 values.
        private readonly MD_Any m_zero;
        private readonly MD_Any m_one;
        private readonly MD_Any m_neg_one;

        // MD_Array with no members (type default value).
        private readonly MD_Any m_empty_array;

        // MD_Bag with no members (type default value).
        private readonly MD_Any m_empty_bag;

        // MD_Tuple with no attributes (type default value).
        private readonly MD_Any m_nullary_tuple;

        // MD_Capsule with False label and no attributes (type default value).
        private readonly MD_Any m_false_nullary_capsule;

        internal Memory ()
        {
            m_false = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Boolean,
                MD_Boolean = false,
            } };

            m_true = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Boolean,
                MD_Boolean = true,
            } };

            m_zero = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Integer,
                MD_Integer = 0,
            } };

            m_one = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Integer,
                MD_Integer = 1,
            } };

            m_neg_one = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Integer,
                MD_Integer = -1,
            } };

            m_empty_array = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Array,
                MD_Array = new MD_Array_Node {
                    Tree_Member_Count = 0,
                    Tree_Widest_Type = Widest_Component_Type.None,
                    Local_Multiplicity = 0,
                    Local_Widest_Type = Widest_Component_Type.None,
                }
            } };

            m_empty_bag = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Bag,
                MD_Bag = new MD_Bag_Node {
                    Tree_Member_Count = 0,
                    Local_Symbolic_Type = Symbolic_Value_Type.None,
                    Local_Member_Count = 0,
                }
            } };

            m_nullary_tuple = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                MD_Tuple = new MD_Tuple_Struct {
                    Degree = 0,
                }
            } };

            m_false_nullary_capsule = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Capsule,
                MD_Capsule = new MD_Capsule_Struct {
                    Label = m_false,
                    Attrs = m_nullary_tuple,
                }
            } };
        }

        internal MD_Any MD_Boolean (System.Boolean value)
        {
            return value ? m_true : m_false;
        }

        internal MD_Any MD_Integer (BigInteger value)
        {
            if (value == 0)
            {
                return m_zero;
            }
            else if (value == 1)
            {
                return m_one;
            }
            else if (value == -1)
            {
                return m_neg_one;
            }
            else
            {
                return new MD_Any { AS = new MD_Any_Struct {
                    Memory = this,
                    MD_Foundation_Type = MD_Foundation_Type.MD_Integer,
                    MD_Integer = value,
                } };
            }
        }
    }
}
