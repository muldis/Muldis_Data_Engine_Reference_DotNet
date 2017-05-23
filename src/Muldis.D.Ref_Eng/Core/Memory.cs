using System;
using System.Collections.Generic;
using System.Linq;
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
    // Note that .Net Core lacks System.Runtime.Caching or similar built-in
    // so for any situations we might have used such, we roll our own.
    // Some caches are logically just HashSets, but we need the ability to
    // fetch the actual cached objects which are the set members so we can
    // reuse them, not just know they exist, so Dictionaries are used instead.
    internal class Memory
    {
        // MD_Boolean False (type default value) and True values.
        private readonly MD_Any m_false;
        private readonly MD_Any m_true;

        // MD_Integer value cache.
        // Seeded with {-1,0,1}, limited to 10K entries in range 2B..2B.
        // The MD_Integer 0 is the type default value.
        private readonly Dictionary<Int32,MD_Any> m_integers;

        // MD_Array with no members (type default value).
        private readonly MD_Any m_empty_array;

        // MD_Bag with no members (type default value).
        private readonly MD_Any m_empty_bag;

        // MD_Tuple with no attributes (type default value).
        private readonly MD_Any m_nullary_tuple;
        // Cache of MD_Tuple subtype Heading values (all Tuple attribute
        // assets are False), limited to 10K entries of shorter size.
        private readonly Dictionary<MD_Any,MD_Any> m_heading_tuples;
        // Cache of MD_Tuple and Heading subtype Attr_Name values.
        // The set of values in here is a proper subset of m_nullary_tuple.
        private readonly Dictionary<Codepoint_Array,MD_Any> m_attr_name_tuples;

        // MD_Capsule with False label and no attributes (type default value).
        private readonly MD_Any m_false_nullary_capsule;

        internal Memory()
        {
            m_false = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Boolean,
                MD_Boolean = false,
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Boolean},
            } };

            m_true = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Boolean,
                MD_Boolean = true,
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Boolean},
            } };

            m_integers = new Dictionary<Int32,MD_Any>();
            for (Int32 i = -1; i <= 1; i++)
            {
                MD_Any v = MD_Integer(i);
            }

            m_empty_array = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Array,
                MD_Array = new MD_Array_Node {
                    Cached_Tree_Member_Count = 0,
                    Tree_Widest_Type = Widest_Component_Type.None,
                    Local_Multiplicity = 0,
                    Local_Widest_Type = Widest_Component_Type.None,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Array, MD_Well_Known_Type.String},
            } };

            m_empty_bag = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Bag,
                MD_Bag = new MD_Bag_Node {
                    Cached_Tree_Member_Count = 0,
                    Local_Symbolic_Type = Symbolic_Value_Type.None,
                    Cached_Local_Member_Count = 0,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Bag},
            } };

            m_nullary_tuple = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                MD_Tuple = new MD_Tuple_Struct {
                    Degree = 0,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Tuple, MD_Well_Known_Type.Heading},
            } };

            m_heading_tuples = new Dictionary<MD_Any,MD_Any>()
            {
                {m_nullary_tuple, m_nullary_tuple},
            };

            m_attr_name_tuples = new Dictionary<Codepoint_Array,MD_Any>();

            m_false_nullary_capsule = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Capsule,
                MD_Capsule = new MD_Capsule_Struct {
                    Label = m_false,
                    Attrs = m_nullary_tuple,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Capsule},
            } };
        }

        internal MD_Any MD_Boolean(Boolean value)
        {
            return value ? m_true : m_false;
        }

        internal MD_Any MD_Integer(BigInteger value)
        {
            Boolean may_cache = value >= -2000000000 && value <= 2000000000;
            if (may_cache && m_integers.ContainsKey((Int32)value))
            {
                return m_integers[(Int32)value];
            }
            MD_Any v = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Integer,
                MD_Integer = value,
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Integer},
            } };
            if (may_cache && m_integers.Count < 10000)
            {
                m_integers.Add((Int32)value, v);
            }
            return v;
        }

        internal MD_Any MD_Array(List<MD_Any> members)
        {
            if (members.Count == 0)
            {
                return m_empty_array;
            }
            MD_Any array = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Array,
                MD_Array = new MD_Array_Node {
                    Tree_Widest_Type = Widest_Component_Type.Unrestricted,
                    Local_Multiplicity = 1,
                    Local_Widest_Type = Widest_Component_Type.Unrestricted,
                    Local_Unrestricted_Members = members,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Array},
            } };
            return array;
        }

        internal MD_Any Octet_MD_String(Byte[] members)
        {
            if (members.Length == 0)
            {
                return m_empty_array;
            }
            MD_Any array = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Array,
                MD_Array = new MD_Array_Node {
                    Tree_Widest_Type = Widest_Component_Type.Octet,
                    Local_Multiplicity = 1,
                    Local_Widest_Type = Widest_Component_Type.Octet,
                    Local_Octet_Members = members,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Array, MD_Well_Known_Type.String},
            } };
            return array;
        }

        internal MD_Any Codepoint_MD_String(Codepoint_Array members)
        {
            if (!members.Has_Any_Members())
            {
                return m_empty_array;
            }
            MD_Any array = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Array,
                MD_Array = new MD_Array_Node {
                    Tree_Widest_Type = Widest_Component_Type.Codepoint,
                    Local_Multiplicity = 1,
                    Local_Widest_Type = Widest_Component_Type.Codepoint,
                    Local_Codepoint_Members = members,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Array, MD_Well_Known_Type.String},
            } };
            return array;
        }

        internal MD_Any MD_Bag(List<Multiplied_Member> members)
        {
            if (members.Count == 0)
            {
                return m_empty_bag;
            }
            MD_Any bag = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Bag,
                MD_Bag = new MD_Bag_Node {
                    Local_Symbolic_Type = Symbolic_Value_Type.Arrayed,
                    Local_Arrayed_Members = members,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Bag},
            } };
            return bag;
        }

        internal MD_Any MD_Tuple(Dictionary<Codepoint_Array,MD_Any> attrs)
        {
            if (attrs.Count == 0)
            {
                return m_nullary_tuple;
            }
            if (attrs.Count == 1)
            {
                KeyValuePair<Codepoint_Array,MD_Any> only_attr = Enumerable.Single(attrs);
                if (Object.ReferenceEquals(only_attr.Value, m_false)
                    && only_attr.Key.May_Cache()
                    && m_attr_name_tuples.ContainsKey(only_attr.Key))
                {
                    return m_attr_name_tuples[only_attr.Key];
                }
            }
            MD_Any tuple = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                MD_Tuple = new MD_Tuple_Struct {
                    Degree = attrs.Count,
                    Other_Attrs = attrs,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Tuple},
            } };
            if (attrs.Count == 1)
            {
                KeyValuePair<Codepoint_Array,MD_Any> only_attr = Enumerable.Single(attrs);
                if (Object.ReferenceEquals(only_attr.Value, m_false))
                {
                    tuple.AS.Cached_WKT.Add(MD_Well_Known_Type.Heading);
                    tuple.AS.Cached_WKT.Add(MD_Well_Known_Type.Attr_Name);
                    if (only_attr.Key.May_Cache() && m_heading_tuples.Count < 10000)
                    {
                        m_heading_tuples.Add(tuple, tuple);
                        m_attr_name_tuples.Add(only_attr.Key, tuple);
                    }
                }
                return tuple;
            }
            // We only get here if the tuple degree >= 2.
            if (Enumerable.All(attrs, attr => Object.ReferenceEquals(attr.Value, m_false)))
            {
                tuple.AS.Cached_WKT.Add(MD_Well_Known_Type.Heading);
                if (attrs.Count <= 30
                    && Enumerable.All(attrs, attr => attr.Key.May_Cache()))
                {
                    if (m_heading_tuples.ContainsKey(tuple))
                    {
                        return m_heading_tuples[tuple];
                    }
                    if (m_heading_tuples.Count < 10000)
                    {
                        m_heading_tuples.Add(tuple, tuple);
                    }
                }
            }
            return tuple;
        }

        internal MD_Any MD_Capsule(MD_Any label, MD_Any attrs)
        {
            if (Object.ReferenceEquals(label, m_false)
                && Object.ReferenceEquals(attrs, m_nullary_tuple))
            {
                return m_false_nullary_capsule;
            }
            return new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Capsule,
                MD_Capsule = new MD_Capsule_Struct {
                    Label = label,
                    Attrs = attrs,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Capsule},
            } };
        }

        internal MD_Any MD_Variable(MD_Any current_value)
        {
            return new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Handle,
                MD_Handle = new MD_Handle_Struct {
                    MD_Handle_Type = MD_Handle_Type.MD_Variable,
                    MD_Variable = new MD_Variable_Struct {
                        Current_Value = current_value,
                    },
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Handle, MD_Well_Known_Type.Variable},
            } };
        }

        internal MD_Any MD_Process()
        {
            return new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Handle,
                MD_Handle = new MD_Handle_Struct {
                    MD_Handle_Type = MD_Handle_Type.MD_Process,
                    MD_Process = new MD_Process_Struct {},
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Handle, MD_Well_Known_Type.Process},
            } };
        }

        internal MD_Any MD_Stream()
        {
            return new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Handle,
                MD_Handle = new MD_Handle_Struct {
                    MD_Handle_Type = MD_Handle_Type.MD_Stream,
                    MD_Stream = new MD_Stream_Struct {},
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Handle, MD_Well_Known_Type.Stream},
            } };
        }

        internal MD_Any MD_External(Object value)
        {
            return new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Handle,
                MD_Handle = new MD_Handle_Struct {
                    MD_Handle_Type = MD_Handle_Type.MD_External,
                    MD_External = new MD_External_Struct {
                        Value = value,
                    },
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Handle, MD_Well_Known_Type.External},
            } };
        }

        internal MD_Any MD_Excuse(MD_Any attrs)
        {
            return new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Capsule,
                MD_Capsule = new MD_Capsule_Struct {
                    Label = MD_Attr_Name(new Codepoint_Array("Excuse")),
                    Attrs = attrs,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Capsule, MD_Well_Known_Type.Excuse},
            } };
        }

        internal MD_Any MD_Attr_Name(Codepoint_Array value)
        {
            if (value.May_Cache() && m_attr_name_tuples.ContainsKey(value))
            {
                return m_attr_name_tuples[value];
            }
            MD_Any tuple = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                MD_Tuple = new MD_Tuple_Struct {
                    Degree = 1,
                    Other_Attrs = new Dictionary<Codepoint_Array,MD_Any>()
                    {
                        {value, m_false},
                    }
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                {
                    MD_Well_Known_Type.Tuple,
                    MD_Well_Known_Type.Heading,
                    MD_Well_Known_Type.Attr_Name,
                },
            } };
            if (value.May_Cache() && m_heading_tuples.Count < 10000)
            {
                m_heading_tuples.Add(tuple, tuple);
                m_attr_name_tuples.Add(value, tuple);
            }
            return tuple;
        }

        internal Codepoint_Array MD_Attr_Name_as_Codepoint_Array(MD_Any value)
        {
            if (!value.AS.Cached_WKT.Contains(MD_Well_Known_Type.Attr_Name))
            {
                throw new ArgumentException
                (
                    paramName: "value",
                    message: "MD_Any is not a Muldis D Attr_Name."
                );
            }
            return Enumerable.Single(value.AS.MD_Tuple.Other_Attrs).Key;
        }
    }
}
