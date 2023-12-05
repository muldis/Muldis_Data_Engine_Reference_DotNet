using System.Collections;
using System.Numerics;

namespace Muldis.Data_Engine_Reference;

// Muldis.Data_Engine_Reference.Internal_Memory
// Provides a virtual machine memory pool where Muldis Data Language values
// and variables live, which exploits the "flyweight pattern" for
// efficiency in both performance and memory usage.
// Conceptually, Internal_Memory is a singleton, in that typically only one
// would be used per virtual machine; in practice, only the set of all
// VM values/vars/processes/etc that would interact must share one.
// Internal_Memory might have multiple pools, say, for separate handling of
// entities that are short-lived versus longer-lived.
// ? Note that .NET Core lacks System.Runtime.Caching or similar built-in
// so for any situations we might have used such, we roll our own.
// Some caches are logically just HashSets, but we need the ability to
// fetch the actual cached objects which are the set members so we can
// reuse them, not just know they exist, so Dictionaries are used instead.
// Note this is called "interning" in a variety of programming languages;
// see https://en.wikipedia.org/wiki/String_interning for more on that.

internal class Internal_Memory
{
    private readonly MDER_Machine __machine;

    // MDER_Ignorance 0iIGNORANCE (type default value) value.
    private readonly MDER_Ignorance MDER_0iIGNORANCE;

    // MDER_Boolean 0bFALSE (type default value) and 0bTRUE values.
    private readonly MDER_False MDER_0bFALSE;
    private readonly MDER_True MDER_0bTRUE;

    // MDER_Integer value cache.
    // Seeded with {-1,0,1}.
    // Limited to 10K entries each in range -2B..2B (signed 32-bit integer).
    // The MDER_Integer 0 is the type default value.
    private readonly Dictionary<Int32, MDER_Integer> integers;

    // MDER_Fraction 0.0 (type default value).
    private readonly MDER_Fraction MDER_Fraction_0;

    // MDER_Bits with no members (type default value).
    internal readonly MDER_Bits MDER_Bits_C0;

    // MDER_Blob with no members (type default value).
    internal readonly MDER_Blob MDER_Blob_C0;

    // MDER_Text value cache.
    // Seeded with empty string and well-known attribute names.
    // Limited to 10K entries each not more than 200 code points length.
    // The MDER_Text empty string is the type default value.
    private readonly Dictionary<String, MDER_Text> texts;

    // MDER_Array with no members (type default value).
    internal readonly MDER_Array MDER_Array_C0;

    // MDER_Set with no members (type default value).
    internal readonly MDER_Set MDER_Set_C0;

    // MDER_Bag with no members (type default value).
    internal readonly MDER_Bag MDER_Bag_C0;

    // MDER_Heading value cache.
    // Limited to 10K entries of shorter size.
    // The MDER_Heading nullary (zero attribute) is the type default value.
    private readonly Dictionary<MDER_Heading, MDER_Heading> headings;
    internal readonly MDER_Heading MDER_Heading_D0;
    internal readonly MDER_Heading MDER_Heading_0;
    internal readonly MDER_Heading MDER_Heading_1;
    internal readonly MDER_Heading MDER_Heading_2;
    internal readonly MDER_Heading MDER_Heading_0_1;
    internal readonly MDER_Heading MDER_Heading_0_2;
    internal readonly MDER_Heading MDER_Heading_1_2;
    internal readonly MDER_Heading MDER_Heading_0_1_2;

    // MDER_Tuple with no attributes (type default value).
    internal readonly MDER_Tuple MDER_Tuple_D0;

    // MDER_Tuple_Array with no attributes and no members
    // (type default value), or with no attributes and 1 member.
    internal readonly MDER_Tuple_Array MDER_Tuple_Array_D0C0;
    internal readonly MDER_Tuple_Array MDER_Tuple_Array_D0C1;

    // MDER_Relation with no attributes and no members
    // (type default value), or with no attributes and 1 member.
    internal readonly MDER_Relation MDER_Relation_D0C0;
    internal readonly MDER_Relation MDER_Relation_D0C1;

    // MDER_Tuple_Bag with no attributes and no members
    // (type default value), or with no attributes and 1 member.
    internal readonly MDER_Tuple_Bag MDER_Tuple_Bag_D0C0;
    internal readonly MDER_Tuple_Bag MDER_Tuple_Bag_D0C1;

    // MDER_Article with False label and no attributes
    // (type default value but not actually useful in practice).
    private readonly MDER_Article false_nullary_article;

    // All well known MDER_Excuse values.
    internal readonly Dictionary<String, MDER_Excuse> well_known_excuses;

    internal Internal_Memory(MDER_Machine machine)
    {
        this.__machine = machine;

        this.MDER_0iIGNORANCE = new MDER_Ignorance(this.__machine);

        this.MDER_0bFALSE = new MDER_False(this.__machine);
        this.MDER_0bTRUE = new MDER_True(this.__machine);

        this.integers = new Dictionary<Int32, MDER_Integer>();
        for (Int32 i = -1; i <= 1; i++)
        {
            MDER_Integer v = this.MDER_Integer(i);
        }

        this.MDER_Fraction_0 = new MDER_Fraction(this.__machine, 0.0M,
            this.integers[0].as_BigInteger, this.integers[1].as_BigInteger);

        this.MDER_Bits_C0 = new MDER_Bits(this.__machine, new BitArray(0));

        this.MDER_Blob_C0 = new MDER_Blob(this.__machine, new Byte[] {});

        this.texts = new Dictionary<String, MDER_Text>();
        foreach (String s in new String[] {"", "\u0000", "\u0001", "\u0002"})
        {
            MDER_Text v = this.MDER_Text(s, false);
        }
        foreach (String s in Internal_Constants.Strings__Seeded_Non_Positional_Attr_Names())
        {
            MDER_Text v = this.MDER_Text(s, false);
        }

        this.MDER_Array_C0 = new MDER_Array(this.__machine,
            new Internal_MDER_Array_Struct {
                local_symbolic_type = Internal_Symbolic_Array_Type.None,
                cached_members_meta = new Internal_Cached_Members_Meta {
                    tree_member_count = 0,
                    tree_all_unique = true,
                    tree_relational = true,
                },
            }
        );

        this.MDER_Set_C0 = new MDER_Set(this.__machine,
            new Internal_MDER_Bag_Struct {
                local_symbolic_type = Internal_Symbolic_Bag_Type.None,
                cached_members_meta = new Internal_Cached_Members_Meta {
                    tree_member_count = 0,
                    tree_all_unique = true,
                    tree_relational = true,
                },
            }
        );

        this.MDER_Bag_C0 = new MDER_Bag(this.__machine,
            new Internal_MDER_Bag_Struct {
                local_symbolic_type = Internal_Symbolic_Bag_Type.None,
                cached_members_meta = new Internal_Cached_Members_Meta {
                    tree_member_count = 0,
                    tree_all_unique = true,
                    tree_relational = true,
                },
            }
        );

        this.headings = new Dictionary<MDER_Heading, MDER_Heading>();
        this.MDER_Heading_D0 = new MDER_Heading(this.__machine, new HashSet<String>());
        this.MDER_Heading_0 = new MDER_Heading(this.__machine, new HashSet<String>()
            {"\u0000"});
        this.MDER_Heading_1 = new MDER_Heading(this.__machine, new HashSet<String>()
            {"\u0001"});
        this.MDER_Heading_2 = new MDER_Heading(this.__machine, new HashSet<String>()
            {"\u0002"});
        this.MDER_Heading_0_1 = new MDER_Heading(this.__machine, new HashSet<String>()
            {"\u0000", "\u0001"});
        this.MDER_Heading_0_2 = new MDER_Heading(this.__machine, new HashSet<String>()
            {"\u0000", "\u0002"});
        this.MDER_Heading_1_2 = new MDER_Heading(this.__machine, new HashSet<String>()
            {"\u0001", "\u0002"});
        this.MDER_Heading_0_1_2 = new MDER_Heading(this.__machine, new HashSet<String>()
            {"\u0000", "\u0001", "\u0002"});

        this.MDER_Tuple_D0 = new MDER_Tuple(this.__machine, new Dictionary<String, MDER_Any>());

        this.false_nullary_article
            = new MDER_Article(this.__machine, this.MDER_0bFALSE, this.MDER_Tuple_D0);

        this.MDER_Tuple_Array_D0C0 = new MDER_Tuple_Array(this.__machine,
            MDER_Heading_D0, MDER_Array_C0);

        this.MDER_Tuple_Array_D0C1 = new MDER_Tuple_Array(this.__machine,
            MDER_Heading_D0,
            new MDER_Array(this.__machine,
                new Internal_MDER_Array_Struct {
                    local_symbolic_type = Internal_Symbolic_Array_Type.Singular,
                    members = new Internal_Multiplied_Member(MDER_Tuple_D0),
                    cached_members_meta = new Internal_Cached_Members_Meta {
                        tree_member_count = 1,
                        tree_all_unique = true,
                        tree_relational = true,
                    },
                }
            )
        );

        this.MDER_Relation_D0C0 = new MDER_Relation(this.__machine,
            MDER_Heading_D0, MDER_Set_C0);

        this.MDER_Relation_D0C1 = new MDER_Relation(this.__machine,
            MDER_Heading_D0,
            new MDER_Set(this.__machine,
                new Internal_MDER_Bag_Struct {
                    local_symbolic_type = Internal_Symbolic_Bag_Type.Singular,
                    members = new Internal_Multiplied_Member(MDER_Tuple_D0),
                    cached_members_meta = new Internal_Cached_Members_Meta {
                        tree_member_count = 1,
                        tree_all_unique = true,
                        tree_relational = true,
                    },
                }
            )
        );

        this.MDER_Tuple_Bag_D0C0 = new MDER_Tuple_Bag(this.__machine,
            MDER_Heading_D0, MDER_Bag_C0);

        this.MDER_Tuple_Bag_D0C1 = new MDER_Tuple_Bag(this.__machine,
            MDER_Heading_D0,
            new MDER_Bag(this.__machine,
                new Internal_MDER_Bag_Struct {
                    local_symbolic_type = Internal_Symbolic_Bag_Type.Singular,
                    members = new Internal_Multiplied_Member(MDER_Tuple_D0),
                    cached_members_meta = new Internal_Cached_Members_Meta {
                        tree_member_count = 1,
                        tree_all_unique = true,
                        tree_relational = true,
                    },
                }
            )
        );

        well_known_excuses = new Dictionary<String, MDER_Excuse>();
        foreach (String s in Internal_Constants.Strings__Well_Known_Excuses())
        {
            well_known_excuses.Add(s,
                new MDER_Excuse(this.__machine, this.MDER_Attr_Name(s), this.MDER_Tuple_D0));
        }
    }

    internal MDER_Machine machine()
    {
        return this.__machine;
    }

    internal MDER_Ignorance MDER_Ignorance()
    {
        return this.MDER_0iIGNORANCE;
    }

    internal MDER_Boolean MDER_Boolean(Boolean value)
    {
        return value ? MDER_0bTRUE : MDER_0bFALSE;
    }

    internal MDER_False MDER_False()
    {
        return this.MDER_0bFALSE;
    }

    internal MDER_True MDER_True()
    {
        return this.MDER_0bTRUE;
    }

    internal MDER_Integer MDER_Integer(BigInteger as_BigInteger)
    {
        Boolean may_cache = as_BigInteger >= -2000000000 && as_BigInteger <= 2000000000;
        if (may_cache && this.integers.ContainsKey((Int32)as_BigInteger))
        {
            return this.integers[(Int32)as_BigInteger];
        }
        MDER_Integer integer = new MDER_Integer(this.__machine, as_BigInteger);
        if (may_cache && this.integers.Count < 10000)
        {
            this.integers.Add((Int32)as_BigInteger, integer);
        }
        return integer;
    }

    internal MDER_Fraction MDER_Fraction(Decimal as_Decimal)
    {
        if (as_Decimal == 0)
        {
            return MDER_Fraction_0;
        }
        return new MDER_Fraction(this.__machine, as_Decimal);
    }

    internal MDER_Fraction MDER_Fraction(BigInteger numerator, BigInteger denominator)
    {
        // Note we assume our caller has already ensured denominator is not zero.
        if (numerator == 0)
        {
            return MDER_Fraction_0;
        }
        // We still have to ensure normalization such that denominator is positive.
        if (denominator < 0)
        {
            denominator = -denominator;
            numerator   = -numerator  ;
        }
        return new MDER_Fraction(this.__machine, numerator, denominator);
    }

    internal MDER_Bits MDER_Bits(BitArray bit_members)
    {
        if (bit_members.Length == 0)
        {
            return MDER_Bits_C0;
        }
        return new MDER_Bits(this.__machine, bit_members);
    }

    internal MDER_Blob MDER_Blob(Byte[] octet_members)
    {
        if (octet_members.Length == 0)
        {
            return MDER_Blob_C0;
        }
        return new MDER_Blob(this.__machine, octet_members);
    }

    internal MDER_Text MDER_Text(String code_point_members, Boolean has_any_non_BMP)
    {
        Boolean may_cache = code_point_members.Length <= 200;
        if (may_cache && this.texts.ContainsKey(code_point_members))
        {
            return this.texts[code_point_members];
        }
        MDER_Text text = new MDER_Text(this.__machine, code_point_members, has_any_non_BMP);
        if (may_cache && this.texts.Count < 10000)
        {
            this.texts.Add(code_point_members, text);
        }
        return text;
    }

    // Temporary wrapper.
    internal MDER_Text MDER_Attr_Name(String code_point_members)
    {
        return this.MDER_Text(code_point_members, false);
    }

    internal MDER_Array MDER_Array(List<MDER_Any> members)
    {
        if (members.Count == 0)
        {
            return MDER_Array_C0;
        }
        if (members.Count == 1)
        {
            return new MDER_Array(this.__machine,
                new Internal_MDER_Array_Struct {
                    local_symbolic_type = Internal_Symbolic_Array_Type.Singular,
                    members = new Internal_Multiplied_Member(members[0]),
                    cached_members_meta = new Internal_Cached_Members_Meta {
                        tree_member_count = 1,
                        tree_all_unique = true,
                        tree_relational = (members[0].WKBT
                            == Internal_Well_Known_Base_Type.MDER_Tuple),
                    },
                }
            );
        }
        return new MDER_Array(this.__machine,
            new Internal_MDER_Array_Struct {
                local_symbolic_type = Internal_Symbolic_Array_Type.Arrayed,
                members = members,
                cached_members_meta = new Internal_Cached_Members_Meta(),
            }
        );
    }

    internal MDER_Set MDER_Set(List<Internal_Multiplied_Member> members)
    {
        if (members.Count == 0)
        {
            return MDER_Set_C0;
        }
        return new MDER_Set(this.__machine,
            new Internal_MDER_Bag_Struct {
                local_symbolic_type = Internal_Symbolic_Bag_Type.Unique,
                members = new Internal_MDER_Bag_Struct {
                    local_symbolic_type = Internal_Symbolic_Bag_Type.Arrayed,
                    members = members,
                    cached_members_meta = new Internal_Cached_Members_Meta(),
                },
                cached_members_meta = new Internal_Cached_Members_Meta {
                    tree_all_unique = true,
                },
            }
        );
    }

    internal MDER_Bag MDER_Bag(List<Internal_Multiplied_Member> members)
    {
        if (members.Count == 0)
        {
            return MDER_Bag_C0;
        }
        return new MDER_Bag(this.__machine,
            new Internal_MDER_Bag_Struct {
                local_symbolic_type = Internal_Symbolic_Bag_Type.Arrayed,
                members = members,
                cached_members_meta = new Internal_Cached_Members_Meta(),
            }
        );
    }

    internal MDER_Heading MDER_Heading(HashSet<String> attr_names)
    {
        Boolean may_cache = attr_names.Count <= 30
            && Enumerable.All(attr_names, attr_name => attr_name.Length <= 200);
        MDER_Heading heading = new MDER_Heading(this.__machine, attr_names);
        if (may_cache && this.headings.ContainsKey(heading))
        {
            return this.headings[heading];
        }
        if (may_cache && this.headings.Count < 10000)
        {
            this.headings.Add(heading, heading);
        }
        return heading;
    }

    internal MDER_Tuple MDER_Tuple(Dictionary<String, MDER_Any> attrs)
    {
        if (attrs.Count == 0)
        {
            return MDER_Tuple_D0;
        }
        return new MDER_Tuple(this.__machine, attrs);
    }

    internal MDER_Tuple_Array MDER_Tuple_Array(MDER_Heading heading, MDER_Array body)
    {
        if (Object.ReferenceEquals(heading, MDER_Heading_D0))
        {
            if (Object.ReferenceEquals(body, MDER_Array_C0))
            {
                return MDER_Tuple_Array_D0C0;
            }
            if (this.__machine._executor().Array__count(body) == 1)
            {
                return MDER_Tuple_Array_D0C1;
            }
        }
        return new MDER_Tuple_Array(this.__machine, heading, body);
    }

    internal MDER_Relation MDER_Relation(MDER_Heading heading, MDER_Set body)
    {
        if (Object.ReferenceEquals(heading, MDER_Heading_D0))
        {
            if (Object.ReferenceEquals(body, MDER_Set_C0))
            {
                return MDER_Relation_D0C0;
            }
            if (this.__machine._executor().Set__Pick_Arbitrary_Member(body) is not null)
            {
                return MDER_Relation_D0C1;
            }
        }
        return new MDER_Relation(this.__machine, heading, body);
    }

    internal MDER_Tuple_Bag MDER_Tuple_Bag(MDER_Heading heading, MDER_Bag body)
    {
        if (Object.ReferenceEquals(heading, MDER_Heading_D0))
        {
            if (Object.ReferenceEquals(body, MDER_Bag_C0))
            {
                return MDER_Tuple_Bag_D0C0;
            }
            // TODO
            //if (this.executor.Bag__count(body) == 1)
            //{
            //    return MDER_Tuple_Bag_D0C1;
            //}
        }
        return new MDER_Tuple_Bag(this.__machine, heading, body);
    }

    internal MDER_Article MDER_Article(MDER_Any label, MDER_Tuple attrs)
    {
        if (Object.ReferenceEquals(label, MDER_0bFALSE)
            && Object.ReferenceEquals(attrs, MDER_Tuple_D0))
        {
            return this.false_nullary_article;
        }
        // TODO: If label corresponds to a Internal_Well_Known_Base_Type then
        // validate whether the label+attrs is actually a member of its
        // Muldis Data Language type, and if it is, return a MDER_Any using the most
        // specific well known base type for that MD value rather than
        // using the generic Article format, for normalization.
        // Note: We do NOT ever have to declare known-not-wkt for types
        // with their own storage formats (things we would never declare
        // known-is-wkt) because they're all tested to that level and
        // normalized at selection, therefore if we have a MDER_Article
        // extant whose label is say 'Text' we know it isn't a Text value, and so on.
        return new MDER_Article(this.__machine, label, attrs);
    }

    internal MDER_Excuse MDER_Excuse(MDER_Any label, MDER_Tuple attrs)
    {
        return new MDER_Excuse(this.__machine, label, attrs);
    }

    internal MDER_Excuse Simple_MDER_Excuse(String value)
    {
        if (well_known_excuses.ContainsKey(value))
        {
            return well_known_excuses[value];
        }
        return new MDER_Excuse(this.__machine, this.MDER_Attr_Name(value), this.MDER_Tuple_D0);
    }

    // TODO: Here or in Internal_Executor also have Article_attrs() etc functions
    // that take anything conceptually a Article and let users use it
    // as if it were a MDER_Article_Struct; these have the opposite
    // transformations as MDER_Article() above does.

    internal MDER_Variable New_MDER_Variable(MDER_Any initial_current_value)
    {
        return new MDER_Variable(this.__machine, initial_current_value);
    }

    internal MDER_Process New_MDER_Process()
    {
        return new MDER_Process(this.__machine);
    }

    internal MDER_Stream New_MDER_Stream()
    {
        return new MDER_Stream(this.__machine);
    }

    internal MDER_External New_MDER_External(Object external_value)
    {
        return new MDER_External(this.__machine, external_value);
    }
}
