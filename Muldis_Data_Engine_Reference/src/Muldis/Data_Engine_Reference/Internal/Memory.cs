using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Muldis.Data_Engine_Reference.Internal;

// Muldis.Data_Engine_Reference.Internal.Memory
// Provides a virtual machine memory pool where Muldis Data Language values and
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
    internal readonly Identity_Generator Identity_Generator;

    internal readonly Preview_Generator Preview_Generator;

    internal readonly Executor Executor;

    // MDL_Boolean False (type default value) and True values.
    internal readonly MDL_Any MDL_False;
    internal readonly MDL_Any MDL_True;

    // MDL_Integer value cache.
    // Seeded with {-1,0,1}, limited to 10K entries in range 2B..2B.
    // The MDL_Integer 0 is the type default value.
    private readonly Dictionary<Int32,MDL_Any> integers;

    // MDL_Fraction 0.0 (type default value).
    internal readonly MDL_Any MDL_Fraction_0;

    // MDL_Bits with no members (type default value).
    internal readonly MDL_Any MDL_Bits_C0;

    // MDL_Blob with no members (type default value).
    internal readonly MDL_Any MDL_Blob_C0;

    // MDL_Text with no members (type default value).
    internal readonly MDL_Any MDL_Text_C0;

    // MDL_Array with no members (type default value).
    internal readonly MDL_Any MDL_Array_C0;

    // MDL_Set with no members (type default value).
    internal readonly MDL_Any MDL_Set_C0;

    // MDL_Bag with no members (type default value).
    internal readonly MDL_Any MDL_Bag_C0;

    // MDL_Tuple with no attributes (type default value).
    internal readonly MDL_Any MDL_Tuple_D0;
    // Cache of MDL_Tuple subtype Heading values (all Tuple attribute
    // assets are False), limited to 10K entries of shorter size.
    private readonly Dictionary<MDL_Any,MDL_Any> heading_tuples;
    // Cache of MDL_Tuple and Heading subtype Attr_Name values.
    // The set of values in here is a proper subset of this.heading_tuples.
    private readonly Dictionary<String,MDL_Any> attr_name_tuples;
    internal readonly MDL_Any Attr_Name_0;
    internal readonly MDL_Any Attr_Name_1;
    internal readonly MDL_Any Attr_Name_2;
    internal readonly MDL_Any Heading_0_1;
    internal readonly MDL_Any Heading_0_2;
    internal readonly MDL_Any Heading_1_2;
    internal readonly MDL_Any Heading_0_1_2;

    // MDL_Tuple_Array with no attributes and no members
    // (type default value), or with no attributes and 1 member.
    internal readonly MDL_Any MDL_Tuple_Array_D0C0;
    internal readonly MDL_Any MDL_Tuple_Array_D0C1;

    // MDL_Relation with no attributes and no members
    // (type default value), or with no attributes and 1 member.
    internal readonly MDL_Any MDL_Relation_D0C0;
    internal readonly MDL_Any MDL_Relation_D0C1;

    // MDL_Tuple_Bag with no attributes and no members
    // (type default value), or with no attributes and 1 member.
    internal readonly MDL_Any MDL_Tuple_Bag_D0C0;
    internal readonly MDL_Any MDL_Tuple_Bag_D0C1;

    // MDL_Article with False label and no attributes
    // (type default value but not actually useful in practice).
    private readonly MDL_Any false_nullary_article;

    // All well known MDL_Excuse values.
    internal readonly Dictionary<String,MDL_Any> Well_Known_Excuses;

    internal Memory()
    {
        Identity_Generator = new Identity_Generator();

        Preview_Generator = new Preview_Generator();

        Executor = new Executor(this);

        MDL_False = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Boolean,
            Details = false,
        };

        MDL_True = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Boolean,
            Details = true,
        };

        this.integers = new Dictionary<Int32,MDL_Any>();
        for (Int32 i = -1; i <= 1; i++)
        {
            MDL_Any v = MDL_Integer(i);
        }

        MDL_Fraction_0 = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Fraction,
            Details = new MDL_Fraction_Struct {
                As_Decimal = 0.0M,
                As_Pair = new MDL_Fraction_Pair {
                    Numerator = this.integers[0].MDL_Integer(),
                    Denominator = this.integers[1].MDL_Integer(),
                    Cached_Is_Coprime = true,
                },
            },
        };

        MDL_Bits_C0 = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Bits,
            Details = new BitArray(0),
        };

        MDL_Blob_C0 = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Blob,
            Details = new Byte[] {},
        };

        MDL_Text_C0 = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Text,
            Details = new MDL_Text_Struct {
                Code_Point_Members = "",
                Has_Any_Non_BMP = false,
                Cached_Member_Count = 0,
            },
        };

        MDL_Array_C0 = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Array,
            Details = new MDL_Array_Struct {
                Local_Symbolic_Type = Symbolic_Array_Type.None,
                Cached_Members_Meta = new Cached_Members_Meta {
                    Tree_Member_Count = 0,
                    Tree_All_Unique = true,
                    Tree_Relational = true,
                },
            },
        };

        MDL_Set_C0 = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Set,
            Details = new MDL_Bag_Struct {
                Local_Symbolic_Type = Symbolic_Bag_Type.None,
                Cached_Members_Meta = new Cached_Members_Meta {
                    Tree_Member_Count = 0,
                    Tree_All_Unique = true,
                    Tree_Relational = true,
                },
            },
        };

        MDL_Bag_C0 = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Bag,
            Details = new MDL_Bag_Struct {
                Local_Symbolic_Type = Symbolic_Bag_Type.None,
                Cached_Members_Meta = new Cached_Members_Meta {
                    Tree_Member_Count = 0,
                    Tree_All_Unique = true,
                    Tree_Relational = true,
                },
            },
        };

        MDL_Tuple_D0 = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Tuple,
            Details = new Dictionary<String, MDL_Any>(),
            cached_WKT_statuses = new Dictionary<Well_Known_Type,Boolean>()
                {{Well_Known_Type.Heading, true}},
        };

        this.attr_name_tuples = new Dictionary<String,MDL_Any>()
        {
            {"\u0000", new MDL_Any {
                Memory = this,
                WKBT = Well_Known_Base_Type.MDL_Tuple,
                Details = new Dictionary<String,MDL_Any>() {{"\u0000", MDL_True}},
                cached_WKT_statuses = new Dictionary<Well_Known_Type,Boolean>()
                    {{Well_Known_Type.Heading, true}, {Well_Known_Type.Attr_Name, true}},
            } },
            {"\u0001", new MDL_Any {
                Memory = this,
                WKBT = Well_Known_Base_Type.MDL_Tuple,
                Details = new Dictionary<String,MDL_Any>() {{"\u0001", MDL_True}},
                cached_WKT_statuses = new Dictionary<Well_Known_Type,Boolean>()
                    {{Well_Known_Type.Heading, true}, {Well_Known_Type.Attr_Name, true}},
            } },
            {"\u0002", new MDL_Any {
                Memory = this,
                WKBT = Well_Known_Base_Type.MDL_Tuple,
                Details = new Dictionary<String,MDL_Any>() {{"\u0002", MDL_True}},
                cached_WKT_statuses = new Dictionary<Well_Known_Type,Boolean>()
                    {{Well_Known_Type.Heading, true}, {Well_Known_Type.Attr_Name, true}},
            } },
        };
        Attr_Name_0 = this.attr_name_tuples["\u0000"];
        Attr_Name_1 = this.attr_name_tuples["\u0001"];
        Attr_Name_2 = this.attr_name_tuples["\u0002"];

        Heading_0_1 = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Tuple,
            Details = new Dictionary<String,MDL_Any>()
                {{"\u0000", MDL_True}, {"\u0001", MDL_True}},
            cached_WKT_statuses = new Dictionary<Well_Known_Type,Boolean>()
                {{Well_Known_Type.Heading, true}},
        };
        Heading_0_2 = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Tuple,
            Details = new Dictionary<String,MDL_Any>()
                {{"\u0000", MDL_True}, {"\u0002", MDL_True}},
            cached_WKT_statuses = new Dictionary<Well_Known_Type,Boolean>()
                {{Well_Known_Type.Heading, true}},
        };
        Heading_1_2 = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Tuple,
            Details = new Dictionary<String,MDL_Any>()
                {{"\u0001", MDL_True}, {"\u0002", MDL_True}},
            cached_WKT_statuses = new Dictionary<Well_Known_Type,Boolean>()
                {{Well_Known_Type.Heading, true}},
        };

        Heading_0_1_2 = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Tuple,
            Details = new Dictionary<String,MDL_Any>()
                {{"\u0000", MDL_True}, {"\u0001", MDL_True}, {"\u0002", MDL_True}},
            cached_WKT_statuses = new Dictionary<Well_Known_Type,Boolean>()
                {{Well_Known_Type.Heading, true}},
        };

        this.heading_tuples = new Dictionary<MDL_Any,MDL_Any>()
        {
            {MDL_Tuple_D0, MDL_Tuple_D0},
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
            MDL_Any an = MDL_Attr_Name(s);
        }

        this.false_nullary_article = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Article,
            Details = new MDL_Article_Struct {
                Label = MDL_False,
                Attrs = MDL_Tuple_D0,
            },
        };

        MDL_Tuple_Array_D0C0 = MDL_Article(
            MDL_Attr_Name("Tuple_Array"),
            MDL_Tuple(new Dictionary<String,MDL_Any>()
                {{"heading", MDL_Tuple_D0}, {"body", MDL_Array_C0}})
        );
        MDL_Tuple_Array_D0C0.declare_member_status_in_WKT(Well_Known_Type.Tuple_Array, true);

        MDL_Tuple_Array_D0C1 = MDL_Article(
            MDL_Attr_Name("Tuple_Array"),
            MDL_Tuple(
                new Dictionary<String,MDL_Any>()
                {
                    {"heading", MDL_Tuple_D0},
                    {"body", new MDL_Any {
                        Memory = this,
                        WKBT = Well_Known_Base_Type.MDL_Array,
                        Details = new MDL_Array_Struct {
                            Local_Symbolic_Type = Symbolic_Array_Type.Singular,
                            Members = new Multiplied_Member(MDL_Tuple_D0),
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
        MDL_Tuple_Array_D0C1.declare_member_status_in_WKT(Well_Known_Type.Tuple_Array, true);

        MDL_Relation_D0C0 = MDL_Article(
            MDL_Attr_Name("Relation"),
            MDL_Tuple(new Dictionary<String,MDL_Any>()
                {{"heading", MDL_Tuple_D0}, {"body", MDL_Set_C0}})
        );
        MDL_Relation_D0C0.declare_member_status_in_WKT(Well_Known_Type.Relation, true);

        MDL_Relation_D0C1 = MDL_Article(
            MDL_Attr_Name("Relation"),
            MDL_Tuple(
                new Dictionary<String,MDL_Any>()
                {
                    {"heading", MDL_Tuple_D0},
                    {"body", new MDL_Any {
                        Memory = this,
                        WKBT = Well_Known_Base_Type.MDL_Set,
                        Details = new MDL_Bag_Struct {
                            Local_Symbolic_Type
                                = Symbolic_Bag_Type.Singular,
                            Members = new Multiplied_Member(MDL_Tuple_D0),
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
        MDL_Relation_D0C1.declare_member_status_in_WKT(Well_Known_Type.Relation, true);

        MDL_Tuple_Bag_D0C0 = MDL_Article(
            MDL_Attr_Name("Tuple_Bag"),
            MDL_Tuple(new Dictionary<String,MDL_Any>()
                {{"heading", MDL_Tuple_D0}, {"body", MDL_Bag_C0}})
        );
        MDL_Tuple_Bag_D0C0.declare_member_status_in_WKT(Well_Known_Type.Tuple_Bag, true);

        MDL_Tuple_Bag_D0C1 = MDL_Article(
            MDL_Attr_Name("Tuple_Bag"),
            MDL_Tuple(
                new Dictionary<String,MDL_Any>()
                {
                    {"heading", MDL_Tuple_D0},
                    {"body", new MDL_Any {
                        Memory = this,
                        WKBT = Well_Known_Base_Type.MDL_Bag,
                        Details = new MDL_Bag_Struct {
                            Local_Symbolic_Type
                                = Symbolic_Bag_Type.Singular,
                            Members = new Multiplied_Member(MDL_Tuple_D0),
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
        MDL_Tuple_Bag_D0C1.declare_member_status_in_WKT(Well_Known_Type.Tuple_Bag, true);

        Well_Known_Excuses = new Dictionary<String,MDL_Any>();
        foreach (String s in Constants.Strings__Well_Known_Excuses())
        {
            Well_Known_Excuses.Add(
                s,
                new MDL_Any {
                    Memory = this,
                    WKBT = Well_Known_Base_Type.MDL_Excuse,
                    Details = new Dictionary<String,MDL_Any>() {{"\u0000", MDL_Attr_Name(s)}},
                }
            );
        }
    }

    internal MDL_Any MDL_Boolean(Boolean value)
    {
        return value ? MDL_True : MDL_False;
    }

    internal MDL_Any MDL_Integer(BigInteger value)
    {
        Boolean may_cache = value >= -2000000000 && value <= 2000000000;
        if (may_cache && this.integers.ContainsKey((Int32)value))
        {
            return this.integers[(Int32)value];
        }
        MDL_Any integer = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Integer,
            Details = value,
        };
        if (may_cache && this.integers.Count < 10000)
        {
            this.integers.Add((Int32)value, integer);
        }
        return integer;
    }

    internal MDL_Any MDL_Fraction(BigInteger numerator, BigInteger denominator)
    {
        // Note we assume our caller has already ensured denominator is not zero.
        if (numerator == 0)
        {
            return MDL_Fraction_0;
        }
        // We still have to ensure normalization such that denominator is positive.
        if (denominator < 0)
        {
            denominator = -denominator;
            numerator   = -numerator  ;
        }
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Fraction,
            Details = new MDL_Fraction_Struct {
                As_Pair = new MDL_Fraction_Pair {
                    Numerator = numerator,
                    Denominator = denominator,
                },
            },
        };
    }

    internal MDL_Any MDL_Fraction(Decimal value)
    {
        if (value == 0)
        {
            return MDL_Fraction_0;
        }
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Fraction,
            Details = new MDL_Fraction_Struct {
                As_Decimal = value,
            },
        };
    }

    internal MDL_Any MDL_Bits(BitArray members)
    {
        if (members.Length == 0)
        {
            return MDL_Bits_C0;
        }
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Bits,
            Details = members,
        };
    }

    internal MDL_Any MDL_Blob(Byte[] members)
    {
        if (members.Length == 0)
        {
            return MDL_Blob_C0;
        }
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Blob,
            Details = members,
        };
    }

    internal MDL_Any MDL_Text(String members, Boolean has_any_non_BMP)
    {
        if (members == "")
        {
            return MDL_Text_C0;
        }
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Text,
            Details = new MDL_Text_Struct {
                Code_Point_Members = members,
                Has_Any_Non_BMP = has_any_non_BMP,
            },
        };
    }

    internal MDL_Any MDL_Array(List<MDL_Any> members)
    {
        if (members.Count == 0)
        {
            return MDL_Array_C0;
        }
        if (members.Count == 1)
        {
            return new MDL_Any {
                Memory = this,
                WKBT = Well_Known_Base_Type.MDL_Array,
                Details = new MDL_Array_Struct {
                    Local_Symbolic_Type = Symbolic_Array_Type.Singular,
                    Members = new Multiplied_Member(members[0]),
                    Cached_Members_Meta = new Cached_Members_Meta {
                        Tree_Member_Count = 1,
                        Tree_All_Unique = true,
                        Tree_Relational = (members[0].WKBT
                            == Well_Known_Base_Type.MDL_Tuple),
                    },
                },
            };
        }
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Array,
            Details = new MDL_Array_Struct {
                Local_Symbolic_Type = Symbolic_Array_Type.Arrayed,
                Members = members,
                Cached_Members_Meta = new Cached_Members_Meta(),
            },
        };
    }

    internal MDL_Any MDL_Set(List<Multiplied_Member> members)
    {
        if (members.Count == 0)
        {
            return MDL_Set_C0;
        }
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Set,
            Details = new MDL_Bag_Struct {
                Local_Symbolic_Type = Symbolic_Bag_Type.Unique,
                Members = new MDL_Bag_Struct {
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

    internal MDL_Any MDL_Bag(List<Multiplied_Member> members)
    {
        if (members.Count == 0)
        {
            return MDL_Bag_C0;
        }
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Bag,
            Details = new MDL_Bag_Struct {
                Local_Symbolic_Type = Symbolic_Bag_Type.Arrayed,
                Members = members,
                Cached_Members_Meta = new Cached_Members_Meta(),
            },
        };
    }

    internal MDL_Any MDL_Tuple(Dictionary<String,MDL_Any> attrs)
    {
        if (attrs.Count == 0)
        {
            return MDL_Tuple_D0;
        }
        if (attrs.Count == 1)
        {
            KeyValuePair<String, MDL_Any> only_attr = attrs.First();
            if (Object.ReferenceEquals(only_attr.Value, MDL_True)
                && only_attr.Key.Length <= 200
                && this.attr_name_tuples.ContainsKey(only_attr.Key))
            {
                return this.attr_name_tuples[only_attr.Key];
            }
        }
        MDL_Any tuple = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Tuple,
            Details = attrs,
        };
        if (attrs.Count == 1)
        {
            KeyValuePair<String, MDL_Any> only_attr = attrs.First();
            if (Object.ReferenceEquals(only_attr.Value, MDL_True))
            {
                tuple.declare_member_status_in_WKT(Well_Known_Type.Heading, true);
                tuple.declare_member_status_in_WKT(Well_Known_Type.Attr_Name, true);
                if (only_attr.Key.Length <= 200 && this.heading_tuples.Count < 10000)
                {
                    this.heading_tuples.Add(tuple, tuple);
                    this.attr_name_tuples.Add(only_attr.Key, tuple);
                }
            }
            return tuple;
        }
        // We only get here if the tuple degree >= 2.
        if (Enumerable.All(attrs, attr => Object.ReferenceEquals(attr.Value, MDL_True)))
        {
            tuple.declare_member_status_in_WKT(Well_Known_Type.Heading, true);
            if (attrs.Count <= 30 && Enumerable.All(attrs, attr => attr.Key.Length <= 200))
            {
                if (this.heading_tuples.ContainsKey(tuple))
                {
                    return this.heading_tuples[tuple];
                }
                if (this.heading_tuples.Count < 10000)
                {
                    this.heading_tuples.Add(tuple, tuple);
                }
            }
        }
        return tuple;
    }

    internal MDL_Any MDL_Tuple_Array(MDL_Any heading, MDL_Any body)
    {
        if (Object.ReferenceEquals(heading, MDL_Tuple_D0))
        {
            if (Object.ReferenceEquals(body, MDL_Array_C0))
            {
                return MDL_Tuple_Array_D0C0;
            }
            if (Executor.Array__count(body) == 1)
            {
                return MDL_Tuple_Array_D0C1;
            }
        }
        MDL_Any tuple_array = MDL_Article(
            MDL_Attr_Name("Tuple_Array"),
            MDL_Tuple(new Dictionary<String,MDL_Any>()
                {{"heading", heading}, {"body", body}})
        );
        tuple_array.declare_member_status_in_WKT(Well_Known_Type.Tuple_Array, true);
        return tuple_array;
    }

    internal MDL_Any MDL_Relation(MDL_Any heading, MDL_Any body)
    {
        if (Object.ReferenceEquals(heading, MDL_Tuple_D0))
        {
            if (Object.ReferenceEquals(body, MDL_Set_C0))
            {
                return MDL_Relation_D0C0;
            }
            if (Set__Pick_Arbitrary_Member(body) is not null)
            {
                return MDL_Relation_D0C1;
            }
        }
        MDL_Any relation = MDL_Article(
            MDL_Attr_Name("Relation"),
            MDL_Tuple(new Dictionary<String,MDL_Any>()
                {{"heading", heading}, {"body", body}})
        );
        relation.declare_member_status_in_WKT(Well_Known_Type.Relation, true);
        return relation;
    }

    internal MDL_Any MDL_Tuple_Bag(MDL_Any heading, MDL_Any body)
    {
        if (Object.ReferenceEquals(heading, MDL_Tuple_D0))
        {
            if (Object.ReferenceEquals(body, MDL_Bag_C0))
            {
                return MDL_Tuple_Bag_D0C0;
            }
            // TODO
            //if (Executor.Bag__count(body) == 1)
            //{
            //    return MDL_Tuple_Bag_D0C1;
            //}
        }
        MDL_Any tuple_bag = MDL_Article(
            MDL_Attr_Name("Tuple_Bag"),
            MDL_Tuple(new Dictionary<String,MDL_Any>()
                {{"heading", heading}, {"body", body}})
        );
        tuple_bag.declare_member_status_in_WKT(Well_Known_Type.Tuple_Bag, true);
        return tuple_bag;
    }

    internal MDL_Any MDL_Article(MDL_Any label, MDL_Any attrs)
    {
        if (Object.ReferenceEquals(label, MDL_False)
            && Object.ReferenceEquals(attrs, MDL_Tuple_D0))
        {
            return this.false_nullary_article;
        }
        // TODO: If label corresponds to a Well_Known_Base_Type then
        // validate whether the label+attrs is actually a member of its
        // Muldis Data Language type, and if it is, return a MDL_Any using the most
        // specific well known base type for that MD value rather than
        // using the generic Article format, for normalization.
        // Note: We do NOT ever have to declare known-not-wkt for types
        // with their own storage formats (things we would never declare
        // known-is-wkt) because they're all tested to that level and
        // normalized at selection, therefore if we have a MDL_Article
        // extant whose label is say 'Text' we know it isn't a Text value, and so on.
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Article,
            Details = new MDL_Article_Struct {
                Label = label,
                Attrs = attrs,
            },
        };
    }

    // TODO: Here or in Executor also have Article_attrs() etc functions
    // that take anything conceptually a Article and let users use it
    // as if it were a MDL_Article_Struct; these have the opposite
    // transformations as MDL_Article() above does.

    internal MDL_Any New_MD_Variable(MDL_Any initial_current_value)
    {
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Variable,
            Details = initial_current_value,
        };
    }

    internal MDL_Any New_MD_Process()
    {
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Process,
            Details = null,
        };
    }

    internal MDL_Any New_MD_Stream()
    {
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Stream,
            Details = null,
        };
    }

    internal MDL_Any New_MD_External(Object value)
    {
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_External,
            Details = value,
        };
    }

    internal MDL_Any MDL_Excuse(MDL_Any attrs)
    {
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Excuse,
            Details = attrs.Details,
        };
    }

    internal MDL_Any Simple_MD_Excuse(String value)
    {
        if (Well_Known_Excuses.ContainsKey(value))
        {
            return Well_Known_Excuses[value];
        }
        return new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Excuse,
            Details = new Dictionary<String,MDL_Any>() {{"\u0000", MDL_Attr_Name(value)}},
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

    internal MDL_Any MDL_Attr_Name(String value)
    {
        if (value.Length <= 200 && this.attr_name_tuples.ContainsKey(value))
        {
            return this.attr_name_tuples[value];
        }
        MDL_Any tuple = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Tuple,
            Details = new Dictionary<String,MDL_Any>() {{value, MDL_True}},
            cached_WKT_statuses = new Dictionary<Well_Known_Type,Boolean>()
                {{Well_Known_Type.Heading, true}, {Well_Known_Type.Attr_Name, true}},
        };
        if (value.Length <= 200 && this.heading_tuples.Count < 10000)
        {
            this.heading_tuples.Add(tuple, tuple);
            this.attr_name_tuples.Add(value, tuple);
        }
        return tuple;
    }

    internal void Array__Collapse(MDL_Any array)
    {
        array.Details = Array__Collapsed_Struct(array.MDL_Array());
    }

    private MDL_Array_Struct Array__Collapsed_Struct(MDL_Array_Struct node)
    {
        switch (node.Local_Symbolic_Type)
        {
            case Symbolic_Array_Type.None:
                // Node is already collapsed.
                // In theory we should never get here assuming that any
                // operations which would knowingly result in the empty
                // Array are optimized to return MDL_Array_C0 directly.
                return MDL_Array_C0.MDL_Array();
            case Symbolic_Array_Type.Singular:
            case Symbolic_Array_Type.Arrayed:
                // Node is already collapsed.
                return node;
            case Symbolic_Array_Type.Catenated:
                MDL_Array_Struct n0 = Array__Collapsed_Struct(node.Tree_Catenated_Members().A0);
                MDL_Array_Struct n1 = Array__Collapsed_Struct(node.Tree_Catenated_Members().A1);
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
                return new MDL_Array_Struct {
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

    internal void Bag__Collapse(MDL_Any bag, Boolean want_indexed = false)
    {
        bag.Details = Bag__Collapsed_Struct(bag.MDL_Bag(), want_indexed);
    }

    private MDL_Bag_Struct Bag__Collapsed_Struct(MDL_Bag_Struct node,
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
                // Bag are optimized to return MDL_Bag_C0 directly.
                return MDL_Bag_C0.MDL_Bag();
            case Symbolic_Bag_Type.Singular:
                if (!want_indexed)
                {
                    // Node is already collapsed.
                    return node;
                }
                Multiplied_Member lsm = node.Local_Singular_Members();
                return new MDL_Bag_Struct {
                    Local_Symbolic_Type = Symbolic_Bag_Type.Indexed,
                    Members = new Dictionary<MDL_Any,Multiplied_Member>()
                        {{lsm.Member, lsm}},
                    Cached_Members_Meta = new Cached_Members_Meta {
                        Tree_Member_Count = lsm.Multiplicity,
                        Tree_All_Unique = (lsm.Multiplicity == 1),
                        Tree_Relational = (lsm.Member.WKBT
                            == Well_Known_Base_Type.MDL_Tuple),
                    },
                };
            case Symbolic_Bag_Type.Arrayed:
                if (!want_indexed)
                {
                    // Node is already collapsed.
                    return node;
                }
                List<Multiplied_Member> ary_src_list = node.Local_Arrayed_Members();
                Dictionary<MDL_Any,Multiplied_Member> ary_res_dict
                    = new Dictionary<MDL_Any,Multiplied_Member>();
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
                return new MDL_Bag_Struct {
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
                MDL_Bag_Struct uni_pa = Bag__Collapsed_Struct(
                    node: node.Tree_Unique_Members(), want_indexed: true);
                if (uni_pa.Local_Symbolic_Type == Symbolic_Bag_Type.None)
                {
                    return uni_pa;
                }
                Dictionary<MDL_Any,Multiplied_Member> uni_src_dict
                    = uni_pa.Local_Indexed_Members();
                return new MDL_Bag_Struct {
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
                MDL_Bag_Struct n0 = Bag__Collapsed_Struct(
                    node: node.Tree_Summed_Members().A0, want_indexed: true);
                MDL_Bag_Struct n1 = Bag__Collapsed_Struct(
                    node: node.Tree_Summed_Members().A1, want_indexed: true);
                if (n0.Local_Symbolic_Type == Symbolic_Bag_Type.None)
                {
                    return n1;
                }
                if (n1.Local_Symbolic_Type == Symbolic_Bag_Type.None)
                {
                    return n0;
                }
                Dictionary<MDL_Any,Multiplied_Member> n0_src_dict
                    = n0.Local_Indexed_Members();
                Dictionary<MDL_Any,Multiplied_Member> n1_src_dict
                    = n1.Local_Indexed_Members();
                Dictionary<MDL_Any,Multiplied_Member> res_dict
                    = new Dictionary<MDL_Any,Multiplied_Member>(n0_src_dict);
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
                return new MDL_Bag_Struct {
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

    internal MDL_Any Array__Pick_Arbitrary_Member(MDL_Any array)
    {
        return Array__Pick_Arbitrary_Node_Member(array.MDL_Array());
    }

    private MDL_Any Array__Pick_Arbitrary_Node_Member(MDL_Array_Struct node)
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

    internal Boolean Array__Is_Relational(MDL_Any array)
    {
        return Array__Tree_Relational(array.MDL_Array());
    }

    private Boolean Array__Tree_Relational(MDL_Array_Struct node)
    {
        if (node.Cached_Members_Meta.Tree_Relational is null)
        {
            Boolean tr = true;
            switch (node.Local_Symbolic_Type)
            {
                case Symbolic_Array_Type.None:
                    tr = true;
                    break;
                case Symbolic_Array_Type.Singular:
                    tr = node.Local_Singular_Members().Member.WKBT
                        == Well_Known_Base_Type.MDL_Tuple;
                    break;
                case Symbolic_Array_Type.Arrayed:
                    MDL_Any m0 = Array__Pick_Arbitrary_Node_Member(node);
                    tr = m0.WKBT == Well_Known_Base_Type.MDL_Tuple
                        && Enumerable.All(
                            node.Local_Arrayed_Members(),
                            m => m.WKBT == Well_Known_Base_Type.MDL_Tuple
                                && Tuple__Same_Heading(m, m0)
                        );
                    break;
                case Symbolic_Array_Type.Catenated:
                    MDL_Any pm0 = Array__Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().A0);
                    MDL_Any sm0 = Array__Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().A1);
                    tr = Array__Tree_Relational(node.Tree_Catenated_Members().A0)
                        && Array__Tree_Relational(node.Tree_Catenated_Members().A1)
                        && (pm0 is null || sm0 is null || Tuple__Same_Heading(pm0, sm0));
                    break;
                default:
                    throw new NotImplementedException();
            }
            node.Cached_Members_Meta.Tree_Relational = tr;
        }
        return (Boolean)node.Cached_Members_Meta.Tree_Relational;
    }

    internal MDL_Any Set__Pick_Arbitrary_Member(MDL_Any set)
    {
        return Bag__Pick_Arbitrary_Member(
            set.MDL_Article().Attrs.MDL_Tuple().First().Value);
    }

    internal Boolean Set__Is_Relational(MDL_Any set)
    {
        return Bag__Is_Relational(
            set.MDL_Article().Attrs.MDL_Tuple().First().Value);
    }

    internal MDL_Any Bag__Pick_Arbitrary_Member(MDL_Any bag)
    {
        return Bag__Pick_Arbitrary_Node_Member(bag.MDL_Bag());
    }

    private MDL_Any Bag__Pick_Arbitrary_Node_Member(MDL_Bag_Struct node)
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

    internal Boolean Bag__Is_Relational(MDL_Any bag)
    {
        return Bag__Tree_Relational(bag.MDL_Bag());
    }

    private Boolean Bag__Tree_Relational(MDL_Bag_Struct node)
    {
        if (node.Cached_Members_Meta.Tree_Relational is null)
        {
            Boolean tr = true;
            switch (node.Local_Symbolic_Type)
            {
                case Symbolic_Bag_Type.None:
                    tr = true;
                    break;
                case Symbolic_Bag_Type.Singular:
                    tr = node.Local_Singular_Members().Member.WKBT
                        == Well_Known_Base_Type.MDL_Tuple;
                    break;
                case Symbolic_Bag_Type.Arrayed:
                    MDL_Any m0 = Bag__Pick_Arbitrary_Node_Member(node);
                    tr = m0.WKBT == Well_Known_Base_Type.MDL_Tuple
                        && Enumerable.All(
                            node.Local_Arrayed_Members(),
                            m => m.Member.WKBT == Well_Known_Base_Type.MDL_Tuple
                                && Tuple__Same_Heading(m.Member, m0)
                        );
                    break;
                case Symbolic_Bag_Type.Indexed:
                    MDL_Any im0 = Bag__Pick_Arbitrary_Node_Member(node);
                    tr = im0.WKBT == Well_Known_Base_Type.MDL_Tuple
                        && Enumerable.All(
                            node.Local_Indexed_Members().Values,
                            m => m.Member.WKBT == Well_Known_Base_Type.MDL_Tuple
                                && Tuple__Same_Heading(m.Member, im0)
                        );
                    break;
                case Symbolic_Bag_Type.Unique:
                    tr = Bag__Tree_Relational(node.Tree_Unique_Members());
                    break;
                case Symbolic_Bag_Type.Summed:
                    tr = Bag__Tree_Relational(node.Tree_Summed_Members().A0)
                        && Bag__Tree_Relational(node.Tree_Summed_Members().A1);
                    MDL_Any pam0 = Bag__Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().A0);
                    MDL_Any eam0 = Bag__Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().A1);
                    if (pam0 is not null && eam0 is not null)
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

    internal MDL_Any Tuple__Heading(MDL_Any tuple)
    {
        if (tuple.member_status_in_WKT(Well_Known_Type.Heading) == true)
        {
            return tuple;
        }
        // If we get here, the Tuple/Heading degree is guaranteed > 0.
        Dictionary<String,MDL_Any> attrs = tuple.MDL_Tuple();
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
                && this.attr_name_tuples.ContainsKey(attrs.First().Key))
            {
                return this.attr_name_tuples[attrs.First().Key];
            }
        }
        MDL_Any heading = new MDL_Any {
            Memory = this,
            WKBT = Well_Known_Base_Type.MDL_Tuple,
            Details = new Dictionary<String,MDL_Any>(
                attrs.ToDictionary(a => a.Key, a => MDL_True)),
            cached_WKT_statuses = new Dictionary<Well_Known_Type,Boolean>()
                {{Well_Known_Type.Heading, true}},
        };
        if (attrs.Count == 1)
        {
            heading.declare_member_status_in_WKT(Well_Known_Type.Attr_Name, true);
            if (attrs.First().Key.Length <= 200 && this.heading_tuples.Count < 10000)
            {
                this.heading_tuples.Add(heading, heading);
                this.attr_name_tuples.Add(attrs.First().Key, heading);
            }
            return heading;
        }
        // We only get here if the tuple degree >= 2.
        if (attrs.Count <= 30 && Enumerable.All(attrs, attr => attr.Key.Length <= 200))
        {
            if (this.heading_tuples.ContainsKey(heading))
            {
                return this.heading_tuples[heading];
            }
            if (this.heading_tuples.Count < 10000)
            {
                this.heading_tuples.Add(heading, heading);
            }
        }
        return heading;
    }

    internal Boolean Tuple__Same_Heading(MDL_Any t1, MDL_Any t2)
    {
        if (Object.ReferenceEquals(t1,t2))
        {
            return true;
        }
        Dictionary<String,MDL_Any> attrs1 = t1.MDL_Tuple();
        Dictionary<String,MDL_Any> attrs2 = t2.MDL_Tuple();
        return (attrs1.Count == attrs2.Count)
            && Enumerable.All(attrs1, attr => attrs2.ContainsKey(attr.Key));
    }

    internal MDL_Any MDL_Text_from_UTF_8_MD_Blob(MDL_Any value)
    {
        Byte[] octets = value.MDL_Blob();
        UTF8Encoding enc = new UTF8Encoding
        (
            encoderShouldEmitUTF8Identifier: false,
            throwOnInvalidBytes: true
        );
        try
        {
            String s = enc.GetString(octets);
            Dot_Net_String_Unicode_Test_Result tr = Test_Dot_Net_String(s);
            if (tr == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
            {
                return Simple_MD_Excuse("X_Unicode_Blob_Not_UTF_8");
            }
            return MDL_Text(
                s,
                (tr == Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
            );
        }
        catch
        {
            return Simple_MD_Excuse("X_Unicode_Blob_Not_UTF_8");
        }
    }
}
