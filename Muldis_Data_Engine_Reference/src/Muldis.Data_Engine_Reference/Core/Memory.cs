using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Muldis.ReferenceEngine.Core
{
    // Muldis.ReferenceEngine.Core.Memory
    // Provides a virtual machine memory pool where Muldis D values and
    // variables live, which exploits the "flyweight pattern" for
    // efficiency in both performance and memory usage.
    // Conceptually, Memory is a singleton, in that typically only one
    // would be used per virtual machine; in practice, only the set of all
    // VM values/vars/processes/etc that would interact must share one.
    // Memory might have multiple pools, say, for separate handling of
    // entities that are short-lived versus longer-lived.
    // Note that .NET Core lacks System.Runtime.Caching or similar built-in
    // so for any situations we might have used such, we roll our own.
    // Some caches are logically just HashSets, but we need the ability to
    // fetch the actual cached objects which are the set members so we can
    // reuse them, not just know they exist, so Dictionaries are used instead.
    // Note this is called "interning" in a variety of programming languages;
    // see https://en.wikipedia.org/wiki/String_interning for more on that.

    internal class Memory
    {
        internal readonly Plain_Text.Identity_Generator Identity_Generator;

        internal readonly Plain_Text.Preview_Generator Preview_Generator;

        internal readonly Executor Executor;

        // MD_Boolean False (type default value) and True values.
        internal readonly MD_Any MD_False;
        internal readonly MD_Any MD_True;

        // MD_Integer value cache.
        // Seeded with {-1,0,1}, limited to 10K entries in range 2B..2B.
        // The MD_Integer 0 is the type default value.
        private readonly Dictionary<Int32,MD_Any> m_integers;

        // MD_Fraction 0.0 (type default value).
        internal readonly MD_Any MD_Fraction_0;

        // MD_Bits with no members (type default value).
        internal readonly MD_Any MD_Bits_C0;

        // MD_Blob with no members (type default value).
        internal readonly MD_Any MD_Blob_C0;

        // MD_Text with no members (type default value).
        internal readonly MD_Any MD_Text_C0;

        // MD_Array with no members (type default value).
        internal readonly MD_Any MD_Array_C0;

        // MD_Set with no members (type default value).
        internal readonly MD_Any MD_Set_C0;

        // MD_Bag with no members (type default value).
        internal readonly MD_Any MD_Bag_C0;

        // MD_Tuple with no attributes (type default value).
        internal readonly MD_Any MD_Tuple_D0;
        // Cache of MD_Tuple subtype Heading values (all Tuple attribute
        // assets are False), limited to 10K entries of shorter size.
        private readonly Dictionary<MD_Any,MD_Any> m_heading_tuples;
        // Cache of MD_Tuple and Heading subtype Attr_Name values.
        // The set of values in here is a proper subset of m_heading_tuples.
        private readonly Dictionary<String,MD_Any> m_attr_name_tuples;
        internal readonly MD_Any Attr_Name_0;
        internal readonly MD_Any Attr_Name_1;
        internal readonly MD_Any Attr_Name_2;
        internal readonly MD_Any Heading_0_1;
        internal readonly MD_Any Heading_0_2;
        internal readonly MD_Any Heading_1_2;
        internal readonly MD_Any Heading_0_1_2;

        // MD_Tuple_Array with no attributes and no members
        // (type default value), or with no attributes and 1 member.
        internal readonly MD_Any MD_Tuple_Array_D0C0;
        internal readonly MD_Any MD_Tuple_Array_D0C1;

        // MD_Relation with no attributes and no members
        // (type default value), or with no attributes and 1 member.
        internal readonly MD_Any MD_Relation_D0C0;
        internal readonly MD_Any MD_Relation_D0C1;

        // MD_Tuple_Bag with no attributes and no members
        // (type default value), or with no attributes and 1 member.
        internal readonly MD_Any MD_Tuple_Bag_D0C0;
        internal readonly MD_Any MD_Tuple_Bag_D0C1;

        // MD_Article with False label and no attributes
        // (type default value but not actually useful in practice).
        private readonly MD_Any m_false_nullary_article;

        // All well known MD_Excuse values.
        internal readonly Dictionary<String,MD_Any> Well_Known_Excuses;

        internal Memory()
        {
            Identity_Generator = new Plain_Text.Identity_Generator();

            Preview_Generator = new Plain_Text.Preview_Generator();

            Executor = new Executor(this);

            MD_False = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Boolean,
                Details = false,
            };

            MD_True = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Boolean,
                Details = true,
            };

            m_integers = new Dictionary<Int32,MD_Any>();
            for (Int32 i = -1; i <= 1; i++)
            {
                MD_Any v = MD_Integer(i);
            }

            MD_Fraction_0 = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Fraction,
                Details = new MD_Fraction_Struct {
                    As_Decimal = 0.0M,
                    As_Pair = new MD_Fraction_Pair {
                        Numerator = m_integers[0].MD_Integer(),
                        Denominator = m_integers[1].MD_Integer(),
                        Cached_Is_Coprime = true,
                    },
                },
            };

            MD_Bits_C0 = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Bits,
                Details = new BitArray(0),
            };

            MD_Blob_C0 = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Blob,
                Details = new Byte[] {},
            };

            MD_Text_C0 = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Text,
                Details = new MD_Text_Struct {
                    Code_Point_Members = "",
                    Has_Any_Non_BMP = false,
                    Cached_Member_Count = 0,
                },
            };

            MD_Array_C0 = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Array,
                Details = new MD_Array_Struct {
                    Local_Symbolic_Type = Symbolic_Array_Type.None,
                    Cached_Members_Meta = new Cached_Members_Meta {
                        Tree_Member_Count = 0,
                        Tree_All_Unique = true,
                        Tree_Relational = true,
                    },
                },
            };

            MD_Set_C0 = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Set,
                Details = new MD_Bag_Struct {
                    Local_Symbolic_Type = Symbolic_Bag_Type.None,
                    Cached_Members_Meta = new Cached_Members_Meta {
                        Tree_Member_Count = 0,
                        Tree_All_Unique = true,
                        Tree_Relational = true,
                    },
                },
            };

            MD_Bag_C0 = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Bag,
                Details = new MD_Bag_Struct {
                    Local_Symbolic_Type = Symbolic_Bag_Type.None,
                    Cached_Members_Meta = new Cached_Members_Meta {
                        Tree_Member_Count = 0,
                        Tree_All_Unique = true,
                        Tree_Relational = true,
                    },
                },
            };

            MD_Tuple_D0 = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Tuple,
                Details = new Dictionary<String, MD_Any>(),
                Cached_WKT_Statuses = new Dictionary<MD_Well_Known_Type,Boolean>()
                    {{MD_Well_Known_Type.Heading, true}},
            };

            m_attr_name_tuples = new Dictionary<String,MD_Any>()
            {
                {"\u0000", new MD_Any {
                    Memory = this,
                    MD_MSBT = MD_Well_Known_Base_Type.MD_Tuple,
                    Details = new Dictionary<String,MD_Any>() {{"\u0000", MD_True}},
                    Cached_WKT_Statuses = new Dictionary<MD_Well_Known_Type,Boolean>()
                        {{MD_Well_Known_Type.Heading, true}, {MD_Well_Known_Type.Attr_Name, true}},
                } },
                {"\u0001", new MD_Any {
                    Memory = this,
                    MD_MSBT = MD_Well_Known_Base_Type.MD_Tuple,
                    Details = new Dictionary<String,MD_Any>() {{"\u0001", MD_True}},
                    Cached_WKT_Statuses = new Dictionary<MD_Well_Known_Type,Boolean>()
                        {{MD_Well_Known_Type.Heading, true}, {MD_Well_Known_Type.Attr_Name, true}},
                } },
                {"\u0002", new MD_Any {
                    Memory = this,
                    MD_MSBT = MD_Well_Known_Base_Type.MD_Tuple,
                    Details = new Dictionary<String,MD_Any>() {{"\u0002", MD_True}},
                    Cached_WKT_Statuses = new Dictionary<MD_Well_Known_Type,Boolean>()
                        {{MD_Well_Known_Type.Heading, true}, {MD_Well_Known_Type.Attr_Name, true}},
                } },
            };
            Attr_Name_0 = m_attr_name_tuples["\u0000"];
            Attr_Name_1 = m_attr_name_tuples["\u0001"];
            Attr_Name_2 = m_attr_name_tuples["\u0002"];

            Heading_0_1 = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Tuple,
                Details = new Dictionary<String,MD_Any>()
                    {{"\u0000", MD_True}, {"\u0001", MD_True}},
                Cached_WKT_Statuses = new Dictionary<MD_Well_Known_Type,Boolean>()
                    {{MD_Well_Known_Type.Heading, true}},
            };
            Heading_0_2 = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Tuple,
                Details = new Dictionary<String,MD_Any>()
                    {{"\u0000", MD_True}, {"\u0002", MD_True}},
                Cached_WKT_Statuses = new Dictionary<MD_Well_Known_Type,Boolean>()
                    {{MD_Well_Known_Type.Heading, true}},
            };
            Heading_1_2 = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Tuple,
                Details = new Dictionary<String,MD_Any>()
                    {{"\u0001", MD_True}, {"\u0002", MD_True}},
                Cached_WKT_Statuses = new Dictionary<MD_Well_Known_Type,Boolean>()
                    {{MD_Well_Known_Type.Heading, true}},
            };

            Heading_0_1_2 = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Tuple,
                Details = new Dictionary<String,MD_Any>()
                    {{"\u0000", MD_True}, {"\u0001", MD_True}, {"\u0002", MD_True}},
                Cached_WKT_Statuses = new Dictionary<MD_Well_Known_Type,Boolean>()
                    {{MD_Well_Known_Type.Heading, true}},
            };

            m_heading_tuples = new Dictionary<MD_Any,MD_Any>()
            {
                {MD_Tuple_D0, MD_Tuple_D0},
                {Attr_Name_0, Attr_Name_0},
                {Attr_Name_1, Attr_Name_1},
                {Attr_Name_2, Attr_Name_2},
                {Heading_0_1, Heading_0_1},
                {Heading_0_2, Heading_0_2},
                {Heading_1_2, Heading_1_2},
                {Heading_0_1_2, Heading_0_1_2},
            };

            foreach (String s in Constants.Strings__Seeded_Non_Positional_Attr_Names())
            {
                MD_Any an = MD_Attr_Name(s);
            }

            m_false_nullary_article = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Article,
                Details = new MD_Article_Struct {
                    Label = MD_False,
                    Attrs = MD_Tuple_D0,
                },
            };

            MD_Tuple_Array_D0C0 = MD_Article(
                MD_Attr_Name("Tuple_Array"),
                MD_Tuple(new Dictionary<String,MD_Any>()
                    {{"heading", MD_Tuple_D0}, {"body", MD_Array_C0}})
            );
            MD_Tuple_Array_D0C0.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Tuple_Array, true);

            MD_Tuple_Array_D0C1 = MD_Article(
                MD_Attr_Name("Tuple_Array"),
                MD_Tuple(
                    new Dictionary<String,MD_Any>()
                    {
                        {"heading", MD_Tuple_D0},
                        {"body", new MD_Any {
                            Memory = this,
                            MD_MSBT = MD_Well_Known_Base_Type.MD_Array,
                            Details = new MD_Array_Struct {
                                Local_Symbolic_Type = Symbolic_Array_Type.Singular,
                                Members = new Multiplied_Member(MD_Tuple_D0),
                                Cached_Members_Meta = new Cached_Members_Meta {
                                    Tree_Member_Count = 1,
                                    Tree_All_Unique = true,
                                    Tree_Relational = true,
                                },
                            },
                        }},
                    }
                )
            );
            MD_Tuple_Array_D0C1.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Tuple_Array, true);

            MD_Relation_D0C0 = MD_Article(
                MD_Attr_Name("Relation"),
                MD_Tuple(new Dictionary<String,MD_Any>()
                    {{"heading", MD_Tuple_D0}, {"body", MD_Set_C0}})
            );
            MD_Relation_D0C0.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Relation, true);

            MD_Relation_D0C1 = MD_Article(
                MD_Attr_Name("Relation"),
                MD_Tuple(
                    new Dictionary<String,MD_Any>()
                    {
                        {"heading", MD_Tuple_D0},
                        {"body", new MD_Any {
                            Memory = this,
                            MD_MSBT = MD_Well_Known_Base_Type.MD_Set,
                            Details = new MD_Bag_Struct {
                                Local_Symbolic_Type
                                    = Symbolic_Bag_Type.Singular,
                                Members = new Multiplied_Member(MD_Tuple_D0),
                                Cached_Members_Meta = new Cached_Members_Meta {
                                    Tree_Member_Count = 1,
                                    Tree_All_Unique = true,
                                    Tree_Relational = true,
                                },
                            },
                        }},
                    }
                )
            );
            MD_Relation_D0C1.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Relation, true);

            MD_Tuple_Bag_D0C0 = MD_Article(
                MD_Attr_Name("Tuple_Bag"),
                MD_Tuple(new Dictionary<String,MD_Any>()
                    {{"heading", MD_Tuple_D0}, {"body", MD_Bag_C0}})
            );
            MD_Tuple_Bag_D0C0.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Tuple_Bag, true);

            MD_Tuple_Bag_D0C1 = MD_Article(
                MD_Attr_Name("Tuple_Bag"),
                MD_Tuple(
                    new Dictionary<String,MD_Any>()
                    {
                        {"heading", MD_Tuple_D0},
                        {"body", new MD_Any {
                            Memory = this,
                            MD_MSBT = MD_Well_Known_Base_Type.MD_Bag,
                            Details = new MD_Bag_Struct {
                                Local_Symbolic_Type
                                    = Symbolic_Bag_Type.Singular,
                                Members = new Multiplied_Member(MD_Tuple_D0),
                                Cached_Members_Meta = new Cached_Members_Meta {
                                    Tree_Member_Count = 1,
                                    Tree_All_Unique = true,
                                    Tree_Relational = true,
                                },
                            },
                        }},
                    }
                )
            );
            MD_Tuple_Bag_D0C1.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Tuple_Bag, true);

            Well_Known_Excuses = new Dictionary<String,MD_Any>();
            foreach (String s in Constants.Strings__Well_Known_Excuses())
            {
                Well_Known_Excuses.Add(
                    s,
                    new MD_Any {
                        Memory = this,
                        MD_MSBT = MD_Well_Known_Base_Type.MD_Excuse,
                        Details = new Dictionary<String,MD_Any>() {{"\u0000", MD_Attr_Name(s)}},
                    }
                );
            }
        }

        internal MD_Any MD_Boolean(Boolean value)
        {
            return value ? MD_True : MD_False;
        }

        internal MD_Any MD_Integer(BigInteger value)
        {
            Boolean may_cache = value >= -2000000000 && value <= 2000000000;
            if (may_cache && m_integers.ContainsKey((Int32)value))
            {
                return m_integers[(Int32)value];
            }
            MD_Any integer = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Integer,
                Details = value,
            };
            if (may_cache && m_integers.Count < 10000)
            {
                m_integers.Add((Int32)value, integer);
            }
            return integer;
        }

        internal MD_Any MD_Fraction(BigInteger numerator, BigInteger denominator)
        {
            // Note we assume our caller has already ensured denominator is not zero.
            if (numerator == 0)
            {
                return MD_Fraction_0;
            }
            // We still have to ensure normalization such that denominator is positive.
            if (denominator < 0)
            {
                denominator = -denominator;
                numerator   = -numerator  ;
            }
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Fraction,
                Details = new MD_Fraction_Struct {
                    As_Pair = new MD_Fraction_Pair {
                        Numerator = numerator,
                        Denominator = denominator,
                    },
                },
            };
        }

        internal MD_Any MD_Fraction(Decimal value)
        {
            if (value == 0)
            {
                return MD_Fraction_0;
            }
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Fraction,
                Details = new MD_Fraction_Struct {
                    As_Decimal = value,
                },
            };
        }

        internal MD_Any MD_Bits(BitArray members)
        {
            if (members.Length == 0)
            {
                return MD_Bits_C0;
            }
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Bits,
                Details = members,
            };
        }

        internal MD_Any MD_Blob(Byte[] members)
        {
            if (members.Length == 0)
            {
                return MD_Blob_C0;
            }
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Blob,
                Details = members,
            };
        }

        internal MD_Any MD_Text(String members, Boolean has_any_non_BMP)
        {
            if (members == "")
            {
                return MD_Text_C0;
            }
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Text,
                Details = new MD_Text_Struct {
                    Code_Point_Members = members,
                    Has_Any_Non_BMP = has_any_non_BMP,
                },
            };
        }

        internal MD_Any MD_Array(List<MD_Any> members)
        {
            if (members.Count == 0)
            {
                return MD_Array_C0;
            }
            if (members.Count == 1)
            {
                return new MD_Any {
                    Memory = this,
                    MD_MSBT = MD_Well_Known_Base_Type.MD_Array,
                    Details = new MD_Array_Struct {
                        Local_Symbolic_Type = Symbolic_Array_Type.Singular,
                        Members = new Multiplied_Member(members[0]),
                        Cached_Members_Meta = new Cached_Members_Meta {
                            Tree_Member_Count = 1,
                            Tree_All_Unique = true,
                            Tree_Relational = (members[0].MD_MSBT
                                == MD_Well_Known_Base_Type.MD_Tuple),
                        },
                    },
                };
            }
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Array,
                Details = new MD_Array_Struct {
                    Local_Symbolic_Type = Symbolic_Array_Type.Arrayed,
                    Members = members,
                    Cached_Members_Meta = new Cached_Members_Meta(),
                },
            };
        }

        internal MD_Any MD_Set(List<Multiplied_Member> members)
        {
            if (members.Count == 0)
            {
                return MD_Set_C0;
            }
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Set,
                Details = new MD_Bag_Struct {
                    Local_Symbolic_Type = Symbolic_Bag_Type.Unique,
                    Members = new MD_Bag_Struct {
                        Local_Symbolic_Type = Symbolic_Bag_Type.Arrayed,
                        Members = members,
                        Cached_Members_Meta = new Cached_Members_Meta(),
                    },
                    Cached_Members_Meta = new Cached_Members_Meta {
                        Tree_All_Unique = true,
                    },
                },
            };
        }

        internal MD_Any MD_Bag(List<Multiplied_Member> members)
        {
            if (members.Count == 0)
            {
                return MD_Bag_C0;
            }
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Bag,
                Details = new MD_Bag_Struct {
                    Local_Symbolic_Type = Symbolic_Bag_Type.Arrayed,
                    Members = members,
                    Cached_Members_Meta = new Cached_Members_Meta(),
                },
            };
        }

        internal MD_Any MD_Tuple(Dictionary<String,MD_Any> attrs)
        {
            if (attrs.Count == 0)
            {
                return MD_Tuple_D0;
            }
            if (attrs.Count == 1)
            {
                KeyValuePair<String, MD_Any> only_attr = attrs.First();
                if (Object.ReferenceEquals(only_attr.Value, MD_True)
                    && only_attr.Key.Length <= 200
                    && m_attr_name_tuples.ContainsKey(only_attr.Key))
                {
                    return m_attr_name_tuples[only_attr.Key];
                }
            }
            MD_Any tuple = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Tuple,
                Details = attrs,
            };
            if (attrs.Count == 1)
            {
                KeyValuePair<String, MD_Any> only_attr = attrs.First();
                if (Object.ReferenceEquals(only_attr.Value, MD_True))
                {
                    tuple.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Heading, true);
                    tuple.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Attr_Name, true);
                    if (only_attr.Key.Length <= 200 && m_heading_tuples.Count < 10000)
                    {
                        m_heading_tuples.Add(tuple, tuple);
                        m_attr_name_tuples.Add(only_attr.Key, tuple);
                    }
                }
                return tuple;
            }
            // We only get here if the tuple degree >= 2.
            if (Enumerable.All(attrs, attr => Object.ReferenceEquals(attr.Value, MD_True)))
            {
                tuple.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Heading, true);
                if (attrs.Count <= 30 && Enumerable.All(attrs, attr => attr.Key.Length <= 200))
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

        internal MD_Any MD_Tuple_Array(MD_Any heading, MD_Any body)
        {
            if (Object.ReferenceEquals(heading, MD_Tuple_D0))
            {
                if (Object.ReferenceEquals(body, MD_Array_C0))
                {
                    return MD_Tuple_Array_D0C0;
                }
                if (Executor.Array__count(body) == 1)
                {
                    return MD_Tuple_Array_D0C1;
                }
            }
            MD_Any tuple_array = MD_Article(
                MD_Attr_Name("Tuple_Array"),
                MD_Tuple(new Dictionary<String,MD_Any>()
                    {{"heading", heading}, {"body", body}})
            );
            tuple_array.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Tuple_Array, true);
            return tuple_array;
        }

        internal MD_Any MD_Relation(MD_Any heading, MD_Any body)
        {
            if (Object.ReferenceEquals(heading, MD_Tuple_D0))
            {
                if (Object.ReferenceEquals(body, MD_Set_C0))
                {
                    return MD_Relation_D0C0;
                }
                if (Set__Pick_Arbitrary_Member(body) != null)
                {
                    return MD_Relation_D0C1;
                }
            }
            MD_Any relation = MD_Article(
                MD_Attr_Name("Relation"),
                MD_Tuple(new Dictionary<String,MD_Any>()
                    {{"heading", heading}, {"body", body}})
            );
            relation.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Relation, true);
            return relation;
        }

        internal MD_Any MD_Tuple_Bag(MD_Any heading, MD_Any body)
        {
            if (Object.ReferenceEquals(heading, MD_Tuple_D0))
            {
                if (Object.ReferenceEquals(body, MD_Bag_C0))
                {
                    return MD_Tuple_Bag_D0C0;
                }
                // TODO
                //if (Executor.Bag__count(body) == 1)
                //{
                //    return MD_Tuple_Bag_D0C1;
                //}
            }
            MD_Any tuple_bag = MD_Article(
                MD_Attr_Name("Tuple_Bag"),
                MD_Tuple(new Dictionary<String,MD_Any>()
                    {{"heading", heading}, {"body", body}})
            );
            tuple_bag.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Tuple_Bag, true);
            return tuple_bag;
        }

        internal MD_Any MD_Article(MD_Any label, MD_Any attrs)
        {
            if (Object.ReferenceEquals(label, MD_False)
                && Object.ReferenceEquals(attrs, MD_Tuple_D0))
            {
                return m_false_nullary_article;
            }
            // TODO: If label corresponds to a MD_Well_Known_Base_Type then
            // validate whether the label+attrs is actually a member of its
            // Muldis D type, and if it is, return a MD_Any using the most
            // specific well known base type for that MD value rather than
            // using the generic Article format, for normalization.
            // Note: We do NOT ever have to declare known-not-wkt for types
            // with their own storage formats (things we would never declare
            // known-is-wkt) because they're all tested to that level and
            // normalized at selection, therefore if we have a MD_Article
            // extant whose label is say 'Text' we know it isn't a Text value, and so on.
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Article,
                Details = new MD_Article_Struct {
                    Label = label,
                    Attrs = attrs,
                },
            };
        }

        // TODO: Here or in Executor also have Article_attrs() etc functions
        // that take anything conceptually a Article and let users use it
        // as if it were a MD_Article_Struct; these have the opposite
        // transformations as MD_Article() above does.

        internal MD_Any New_MD_Variable(MD_Any initial_current_value)
        {
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Variable,
                Details = initial_current_value,
            };
        }

        internal MD_Any New_MD_Process()
        {
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Process,
                Details = null,
            };
        }

        internal MD_Any New_MD_Stream()
        {
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Stream,
                Details = null,
            };
        }

        internal MD_Any New_MD_External(Object value)
        {
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_External,
                Details = value,
            };
        }

        internal MD_Any MD_Excuse(MD_Any attrs)
        {
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Excuse,
                Details = attrs.Details,
            };
        }

        internal MD_Any Simple_MD_Excuse(String value)
        {
            if (Well_Known_Excuses.ContainsKey(value))
            {
                return Well_Known_Excuses[value];
            }
            return new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Excuse,
                Details = new Dictionary<String,MD_Any>() {{"\u0000", MD_Attr_Name(value)}},
            };
        }

        internal Dot_Net_String_Unicode_Test_Result Test_Dot_Net_String(String value)
        {
            Dot_Net_String_Unicode_Test_Result ok_result
                = Dot_Net_String_Unicode_Test_Result.Valid_Is_All_BMP;
            for (Int32 i = 0; i < value.Length; i++)
            {
                if (Char.IsSurrogate(value[i]))
                {
                    if ((i+1) < value.Length
                        && Char.IsSurrogatePair(value[i], value[i+1]))
                    {
                        ok_result = Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP;
                        i++;
                    }
                    else
                    {
                        return Dot_Net_String_Unicode_Test_Result.Is_Malformed;
                    }
                }
            }
            return ok_result;
        }

        internal MD_Any MD_Attr_Name(String value)
        {
            if (value.Length <= 200 && m_attr_name_tuples.ContainsKey(value))
            {
                return m_attr_name_tuples[value];
            }
            MD_Any tuple = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Tuple,
                Details = new Dictionary<String,MD_Any>() {{value, MD_True}},
                Cached_WKT_Statuses = new Dictionary<MD_Well_Known_Type,Boolean>()
                    {{MD_Well_Known_Type.Heading, true}, {MD_Well_Known_Type.Attr_Name, true}},
            };
            if (value.Length <= 200 && m_heading_tuples.Count < 10000)
            {
                m_heading_tuples.Add(tuple, tuple);
                m_attr_name_tuples.Add(value, tuple);
            }
            return tuple;
        }

        internal void Array__Collapse(MD_Any array)
        {
            array.Details = Array__Collapsed_Struct(array.MD_Array());
        }

        private MD_Array_Struct Array__Collapsed_Struct(MD_Array_Struct node)
        {
            switch (node.Local_Symbolic_Type)
            {
                case Symbolic_Array_Type.None:
                    // Node is already collapsed.
                    // In theory we should never get here assuming that any
                    // operations which would knowingly result in the empty
                    // Array are optimized to return MD_Array_C0 directly.
                    return MD_Array_C0.MD_Array();
                case Symbolic_Array_Type.Singular:
                case Symbolic_Array_Type.Arrayed:
                    // Node is already collapsed.
                    return node;
                case Symbolic_Array_Type.Catenated:
                    MD_Array_Struct n0 = Array__Collapsed_Struct(node.Tree_Catenated_Members().A0);
                    MD_Array_Struct n1 = Array__Collapsed_Struct(node.Tree_Catenated_Members().A1);
                    if (n0.Local_Symbolic_Type == Symbolic_Array_Type.None)
                    {
                        return n1;
                    }
                    if (n1.Local_Symbolic_Type == Symbolic_Array_Type.None)
                    {
                        return n0;
                    }
                    // Both child nodes have at least 1 member, so we will
                    // use Arrayed format for merger even if the input
                    // members are the same value.
                    // This will die if Multiplicity greater than an Int32.
                    return new MD_Array_Struct {
                        Local_Symbolic_Type = Symbolic_Array_Type.Arrayed,
                        Members = Enumerable.Concat(
                            (n0.Local_Symbolic_Type == Symbolic_Array_Type.Singular
                                ? Enumerable.Repeat(n0.Local_Singular_Members().Member,
                                    (Int32)n0.Local_Singular_Members().Multiplicity)
                                : n0.Local_Arrayed_Members()),
                            (n1.Local_Symbolic_Type == Symbolic_Array_Type.Singular
                                ? Enumerable.Repeat(n1.Local_Singular_Members().Member,
                                    (Int32)n1.Local_Singular_Members().Multiplicity)
                                : n1.Local_Arrayed_Members())
                        ),
                        Cached_Members_Meta = new Cached_Members_Meta(),
                            // TODO: Merge existing source meta where efficient,
                            // Tree_Member_Count and Tree_Relational in particular.
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        internal void Bag__Collapse(MD_Any bag, Boolean want_indexed = false)
        {
            bag.Details = Bag__Collapsed_Struct(bag.MD_Bag(), want_indexed);
        }

        private MD_Bag_Struct Bag__Collapsed_Struct(MD_Bag_Struct node,
            Boolean want_indexed = false)
        {
            // Note: want_indexed only causes Indexed result when Singular
            // or Arrayed otherwise would be used; None still also used.
            switch (node.Local_Symbolic_Type)
            {
                case Symbolic_Bag_Type.None:
                    // Node is already collapsed.
                    // In theory we should never get here assuming that any
                    // operations which would knowingly result in the empty
                    // Bag are optimized to return MD_Bag_C0 directly.
                    return MD_Bag_C0.MD_Bag();
                case Symbolic_Bag_Type.Singular:
                    if (!want_indexed)
                    {
                        // Node is already collapsed.
                        return node;
                    }
                    Multiplied_Member lsm = node.Local_Singular_Members();
                    return new MD_Bag_Struct {
                        Local_Symbolic_Type = Symbolic_Bag_Type.Indexed,
                        Members = new Dictionary<MD_Any,Multiplied_Member>()
                            {{lsm.Member, lsm}},
                        Cached_Members_Meta = new Cached_Members_Meta {
                            Tree_Member_Count = lsm.Multiplicity,
                            Tree_All_Unique = (lsm.Multiplicity == 1),
                            Tree_Relational = (lsm.Member.MD_MSBT
                                == MD_Well_Known_Base_Type.MD_Tuple),
                        },
                    };
                case Symbolic_Bag_Type.Arrayed:
                    if (!want_indexed)
                    {
                        // Node is already collapsed.
                        return node;
                    }
                    List<Multiplied_Member> ary_src_list = node.Local_Arrayed_Members();
                    Dictionary<MD_Any,Multiplied_Member> ary_res_dict
                        = new Dictionary<MD_Any,Multiplied_Member>();
                    foreach (Multiplied_Member m in ary_src_list)
                    {
                        if (!ary_res_dict.ContainsKey(m.Member))
                        {
                            ary_res_dict.Add(m.Member, m.Clone());
                        }
                        else
                        {
                            ary_res_dict[m.Member].Multiplicity ++;
                        }
                    }
                    return new MD_Bag_Struct {
                        Local_Symbolic_Type = Symbolic_Bag_Type.Indexed,
                        Members = ary_res_dict,
                        Cached_Members_Meta = new Cached_Members_Meta {
                            Tree_Member_Count = ary_src_list.Count,
                            Tree_All_Unique = node.Cached_Members_Meta.Tree_All_Unique,
                            Tree_Relational = node.Cached_Members_Meta.Tree_Relational,
                        },
                    };
                case Symbolic_Bag_Type.Indexed:
                    // Node is already collapsed.
                    return node;
                case Symbolic_Bag_Type.Unique:
                    MD_Bag_Struct uni_pa = Bag__Collapsed_Struct(
                        node: node.Tree_Unique_Members(), want_indexed: true);
                    if (uni_pa.Local_Symbolic_Type == Symbolic_Bag_Type.None)
                    {
                        return uni_pa;
                    }
                    Dictionary<MD_Any,Multiplied_Member> uni_src_dict
                        = uni_pa.Local_Indexed_Members();
                    return new MD_Bag_Struct {
                        Local_Symbolic_Type = Symbolic_Bag_Type.Indexed,
                        Members = uni_src_dict.ToDictionary(
                            m => m.Key, m => new Multiplied_Member(m.Key, 1)),
                        Cached_Members_Meta = new Cached_Members_Meta {
                            Tree_Member_Count = uni_src_dict.Count,
                            Tree_All_Unique = true,
                            Tree_Relational = uni_pa.Cached_Members_Meta.Tree_Relational,
                        },
                    };
                case Symbolic_Bag_Type.Summed:
                    MD_Bag_Struct n0 = Bag__Collapsed_Struct(
                        node: node.Tree_Summed_Members().A0, want_indexed: true);
                    MD_Bag_Struct n1 = Bag__Collapsed_Struct(
                        node: node.Tree_Summed_Members().A1, want_indexed: true);
                    if (n0.Local_Symbolic_Type == Symbolic_Bag_Type.None)
                    {
                        return n1;
                    }
                    if (n1.Local_Symbolic_Type == Symbolic_Bag_Type.None)
                    {
                        return n0;
                    }
                    Dictionary<MD_Any,Multiplied_Member> n0_src_dict
                        = n0.Local_Indexed_Members();
                    Dictionary<MD_Any,Multiplied_Member> n1_src_dict
                        = n1.Local_Indexed_Members();
                    Dictionary<MD_Any,Multiplied_Member> res_dict
                        = new Dictionary<MD_Any,Multiplied_Member>(n0_src_dict);
                    foreach (Multiplied_Member m in n1_src_dict.Values)
                    {
                        if (!res_dict.ContainsKey(m.Member))
                        {
                            res_dict.Add(m.Member, m);
                        }
                        else
                        {
                            res_dict[m.Member] = new Multiplied_Member(m.Member,
                                res_dict[m.Member].Multiplicity + m.Multiplicity);
                        }
                    }
                    return new MD_Bag_Struct {
                        Local_Symbolic_Type = Symbolic_Bag_Type.Indexed,
                        Members = res_dict,
                        Cached_Members_Meta = new Cached_Members_Meta(),
                            // TODO: Merge existing source meta where efficient,
                            // Tree_Member_Count and Tree_Relational in particular.
                    };
                default:
                    throw new NotImplementedException();
            }
        }

        internal MD_Any Array__Pick_Arbitrary_Member(MD_Any array)
        {
            return Array__Pick_Arbitrary_Node_Member(array.MD_Array());
        }

        private MD_Any Array__Pick_Arbitrary_Node_Member(MD_Array_Struct node)
        {
            switch (node.Local_Symbolic_Type)
            {
                case Symbolic_Array_Type.None:
                    return null;
                case Symbolic_Array_Type.Singular:
                    return node.Local_Singular_Members().Member;
                case Symbolic_Array_Type.Arrayed:
                    return node.Local_Arrayed_Members()[0];
                case Symbolic_Array_Type.Catenated:
                    return Array__Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().A0)
                        ?? Array__Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().A1);
                default:
                    throw new NotImplementedException();
            }
        }

        internal Boolean Array__Is_Relational(MD_Any array)
        {
            return Array__Tree_Relational(array.MD_Array());
        }

        private Boolean Array__Tree_Relational(MD_Array_Struct node)
        {
            if (node.Cached_Members_Meta.Tree_Relational == null)
            {
                Boolean tr = true;
                switch (node.Local_Symbolic_Type)
                {
                    case Symbolic_Array_Type.None:
                        tr = true;
                        break;
                    case Symbolic_Array_Type.Singular:
                        tr = node.Local_Singular_Members().Member.MD_MSBT
                            == MD_Well_Known_Base_Type.MD_Tuple;
                        break;
                    case Symbolic_Array_Type.Arrayed:
                        MD_Any m0 = Array__Pick_Arbitrary_Node_Member(node);
                        tr = m0.MD_MSBT == MD_Well_Known_Base_Type.MD_Tuple
                            && Enumerable.All(
                                node.Local_Arrayed_Members(),
                                m => m.MD_MSBT == MD_Well_Known_Base_Type.MD_Tuple
                                    && Tuple__Same_Heading(m, m0)
                            );
                        break;
                    case Symbolic_Array_Type.Catenated:
                        MD_Any pm0 = Array__Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().A0);
                        MD_Any sm0 = Array__Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().A1);
                        tr = Array__Tree_Relational(node.Tree_Catenated_Members().A0)
                            && Array__Tree_Relational(node.Tree_Catenated_Members().A1)
                            && (pm0 == null || sm0 == null || Tuple__Same_Heading(pm0, sm0));
                        break;
                    default:
                        throw new NotImplementedException();
                }
                node.Cached_Members_Meta.Tree_Relational = tr;
            }
            return (Boolean)node.Cached_Members_Meta.Tree_Relational;
        }

        internal MD_Any Set__Pick_Arbitrary_Member(MD_Any set)
        {
            return Bag__Pick_Arbitrary_Member(
                set.MD_Article().Attrs.MD_Tuple().First().Value);
        }

        internal Boolean Set__Is_Relational(MD_Any set)
        {
            return Bag__Is_Relational(
                set.MD_Article().Attrs.MD_Tuple().First().Value);
        }

        internal MD_Any Bag__Pick_Arbitrary_Member(MD_Any bag)
        {
            return Bag__Pick_Arbitrary_Node_Member(bag.MD_Bag());
        }

        private MD_Any Bag__Pick_Arbitrary_Node_Member(MD_Bag_Struct node)
        {
            if (node.Cached_Members_Meta.Tree_Member_Count == 0)
            {
                return null;
            }
            switch (node.Local_Symbolic_Type)
            {
                case Symbolic_Bag_Type.None:
                    return null;
                case Symbolic_Bag_Type.Singular:
                    return node.Local_Singular_Members().Member;
                case Symbolic_Bag_Type.Arrayed:
                    return node.Local_Arrayed_Members()[0].Member;
                case Symbolic_Bag_Type.Indexed:
                    return node.Local_Indexed_Members().First().Value.Member;
                case Symbolic_Bag_Type.Unique:
                    return Bag__Pick_Arbitrary_Node_Member(node.Tree_Unique_Members());
                case Symbolic_Bag_Type.Summed:
                    return Bag__Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().A0)
                        ?? Bag__Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().A1);
                default:
                    throw new NotImplementedException();
            }
        }

        internal Boolean Bag__Is_Relational(MD_Any bag)
        {
            return Bag__Tree_Relational(bag.MD_Bag());
        }

        private Boolean Bag__Tree_Relational(MD_Bag_Struct node)
        {
            if (node.Cached_Members_Meta.Tree_Relational == null)
            {
                Boolean tr = true;
                switch (node.Local_Symbolic_Type)
                {
                    case Symbolic_Bag_Type.None:
                        tr = true;
                        break;
                    case Symbolic_Bag_Type.Singular:
                        tr = node.Local_Singular_Members().Member.MD_MSBT
                            == MD_Well_Known_Base_Type.MD_Tuple;
                        break;
                    case Symbolic_Bag_Type.Arrayed:
                        MD_Any m0 = Bag__Pick_Arbitrary_Node_Member(node);
                        tr = m0.MD_MSBT == MD_Well_Known_Base_Type.MD_Tuple
                            && Enumerable.All(
                                node.Local_Arrayed_Members(),
                                m => m.Member.MD_MSBT == MD_Well_Known_Base_Type.MD_Tuple
                                    && Tuple__Same_Heading(m.Member, m0)
                            );
                        break;
                    case Symbolic_Bag_Type.Indexed:
                        MD_Any im0 = Bag__Pick_Arbitrary_Node_Member(node);
                        tr = im0.MD_MSBT == MD_Well_Known_Base_Type.MD_Tuple
                            && Enumerable.All(
                                node.Local_Indexed_Members().Values,
                                m => m.Member.MD_MSBT == MD_Well_Known_Base_Type.MD_Tuple
                                    && Tuple__Same_Heading(m.Member, im0)
                            );
                        break;
                    case Symbolic_Bag_Type.Unique:
                        tr = Bag__Tree_Relational(node.Tree_Unique_Members());
                        break;
                    case Symbolic_Bag_Type.Summed:
                        tr = Bag__Tree_Relational(node.Tree_Summed_Members().A0)
                            && Bag__Tree_Relational(node.Tree_Summed_Members().A1);
                        MD_Any pam0 = Bag__Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().A0);
                        MD_Any eam0 = Bag__Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().A1);
                        if (pam0 != null && eam0 != null)
                        {
                            tr = tr && Tuple__Same_Heading(pam0, eam0);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                node.Cached_Members_Meta.Tree_Relational = tr;
            }
            return (Boolean)node.Cached_Members_Meta.Tree_Relational;
        }

        internal MD_Any Tuple__Heading(MD_Any tuple)
        {
            if (tuple.Member_Status_in_WKT(MD_Well_Known_Type.Heading) == true)
            {
                return tuple;
            }
            // If we get here, the Tuple/Heading degree is guaranteed > 0.
            Dictionary<String,MD_Any> attrs = tuple.MD_Tuple();
            if (attrs.Count == 1)
            {
                if (attrs.ContainsKey("\u0000"))
                {
                    return Attr_Name_0;
                }
                if (attrs.ContainsKey("\u0001"))
                {
                    return Attr_Name_1;
                }
                if (attrs.ContainsKey("\u0002"))
                {
                    return Attr_Name_2;
                }
                if (attrs.First().Key.Length <= 200
                    && m_attr_name_tuples.ContainsKey(attrs.First().Key))
                {
                    return m_attr_name_tuples[attrs.First().Key];
                }
            }
            MD_Any heading = new MD_Any {
                Memory = this,
                MD_MSBT = MD_Well_Known_Base_Type.MD_Tuple,
                Details = new Dictionary<String,MD_Any>(
                    attrs.ToDictionary(a => a.Key, a => MD_True)),
                Cached_WKT_Statuses = new Dictionary<MD_Well_Known_Type,Boolean>()
                    {{MD_Well_Known_Type.Heading, true}},
            };
            if (attrs.Count == 1)
            {
                heading.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Attr_Name, true);
                if (attrs.First().Key.Length <= 200 && m_heading_tuples.Count < 10000)
                {
                    m_heading_tuples.Add(heading, heading);
                    m_attr_name_tuples.Add(attrs.First().Key, heading);
                }
                return heading;
            }
            // We only get here if the tuple degree >= 2.
            if (attrs.Count <= 30 && Enumerable.All(attrs, attr => attr.Key.Length <= 200))
            {
                if (m_heading_tuples.ContainsKey(heading))
                {
                    return m_heading_tuples[heading];
                }
                if (m_heading_tuples.Count < 10000)
                {
                    m_heading_tuples.Add(heading, heading);
                }
            }
            return heading;
        }

        internal Boolean Tuple__Same_Heading(MD_Any t1, MD_Any t2)
        {
            if (Object.ReferenceEquals(t1,t2))
            {
                return true;
            }
            Dictionary<String,MD_Any> attrs1 = t1.MD_Tuple();
            Dictionary<String,MD_Any> attrs2 = t2.MD_Tuple();
            return (attrs1.Count == attrs2.Count)
                && Enumerable.All(attrs1, attr => attrs2.ContainsKey(attr.Key));
        }

        internal MD_Any MD_Text_from_UTF_8_MD_Blob(MD_Any value)
        {
            Byte[] octets = value.MD_Blob();
            UTF8Encoding enc = new UTF8Encoding
            (
                encoderShouldEmitUTF8Identifier: false,
                throwOnInvalidBytes: true
            );
            try
            {
                String s = enc.GetString(octets);
                Dot_Net_String_Unicode_Test_Result tr = Test_Dot_Net_String(s);
                if (tr == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                {
                    return Simple_MD_Excuse("X_Unicode_Blob_Not_UTF_8");
                }
                return MD_Text(
                    s,
                    (tr == Core.Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
                );
            }
            catch
            {
                return Simple_MD_Excuse("X_Unicode_Blob_Not_UTF_8");
            }
        }
    }
}
