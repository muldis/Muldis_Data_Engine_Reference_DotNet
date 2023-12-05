using System.Collections;
using System.Numerics;
using System.Text;

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
    internal readonly Internal_Identity_Generator identity_generator;
    internal readonly Internal_Preview_Generator preview_generator;

    internal readonly Internal_Executor executor;

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

    internal Internal_Memory()
    {
        this.identity_generator = new Internal_Identity_Generator();
        this.preview_generator = new Internal_Preview_Generator();

        this.executor = new Internal_Executor(this);

        this.MDER_0iIGNORANCE = new MDER_Ignorance(this);

        this.MDER_0bFALSE = new MDER_False(this);
        this.MDER_0bTRUE = new MDER_True(this);

        this.integers = new Dictionary<Int32, MDER_Integer>();
        for (Int32 i = -1; i <= 1; i++)
        {
            MDER_Integer v = this.MDER_Integer(i);
        }

        this.MDER_Fraction_0 = new MDER_Fraction(this, 0.0M,
            this.integers[0].as_BigInteger, this.integers[1].as_BigInteger);

        this.MDER_Bits_C0 = new MDER_Bits(this, new BitArray(0));

        this.MDER_Blob_C0 = new MDER_Blob(this, new Byte[] {});

        this.texts = new Dictionary<String, MDER_Text>();
        foreach (String s in new String[] {"", "\u0000", "\u0001", "\u0002"})
        {
            MDER_Text v = this.MDER_Text(s, false);
        }
        foreach (String s in Internal_Constants.Strings__Seeded_Non_Positional_Attr_Names())
        {
            MDER_Text v = this.MDER_Text(s, false);
        }

        this.MDER_Array_C0 = new MDER_Array(this,
            new MDER_Array_Struct {
                local_symbolic_type = Symbolic_Array_Type.None,
                cached_members_meta = new Cached_Members_Meta {
                    tree_member_count = 0,
                    tree_all_unique = true,
                    tree_relational = true,
                },
            }
        );

        this.MDER_Set_C0 = new MDER_Set(this,
            new MDER_Bag_Struct {
                local_symbolic_type = Symbolic_Bag_Type.None,
                cached_members_meta = new Cached_Members_Meta {
                    tree_member_count = 0,
                    tree_all_unique = true,
                    tree_relational = true,
                },
            }
        );

        this.MDER_Bag_C0 = new MDER_Bag(this,
            new MDER_Bag_Struct {
                local_symbolic_type = Symbolic_Bag_Type.None,
                cached_members_meta = new Cached_Members_Meta {
                    tree_member_count = 0,
                    tree_all_unique = true,
                    tree_relational = true,
                },
            }
        );

        this.headings = new Dictionary<MDER_Heading, MDER_Heading>();
        this.MDER_Heading_D0 = new MDER_Heading(this, new HashSet<String>());
        this.MDER_Heading_0 = new MDER_Heading(this, new HashSet<String>()
            {"\u0000"});
        this.MDER_Heading_1 = new MDER_Heading(this, new HashSet<String>()
            {"\u0001"});
        this.MDER_Heading_2 = new MDER_Heading(this, new HashSet<String>()
            {"\u0002"});
        this.MDER_Heading_0_1 = new MDER_Heading(this, new HashSet<String>()
            {"\u0000", "\u0001"});
        this.MDER_Heading_0_2 = new MDER_Heading(this, new HashSet<String>()
            {"\u0000", "\u0002"});
        this.MDER_Heading_1_2 = new MDER_Heading(this, new HashSet<String>()
            {"\u0001", "\u0002"});
        this.MDER_Heading_0_1_2 = new MDER_Heading(this, new HashSet<String>()
            {"\u0000", "\u0001", "\u0002"});

        this.MDER_Tuple_D0 = new MDER_Tuple(this, new Dictionary<String, MDER_Any>());

        this.false_nullary_article
            = new MDER_Article(this, this.MDER_0bFALSE, this.MDER_Tuple_D0);

        this.MDER_Tuple_Array_D0C0 = new MDER_Tuple_Array(this,
            MDER_Heading_D0, MDER_Array_C0);

        this.MDER_Tuple_Array_D0C1 = new MDER_Tuple_Array(this,
            MDER_Heading_D0,
            new MDER_Array(this,
                new MDER_Array_Struct {
                    local_symbolic_type = Symbolic_Array_Type.Singular,
                    members = new Multiplied_Member(MDER_Tuple_D0),
                    cached_members_meta = new Cached_Members_Meta {
                        tree_member_count = 1,
                        tree_all_unique = true,
                        tree_relational = true,
                    },
                }
            )
        );

        this.MDER_Relation_D0C0 = new MDER_Relation(this,
            MDER_Heading_D0, MDER_Set_C0);

        this.MDER_Relation_D0C1 = new MDER_Relation(this,
            MDER_Heading_D0,
            new MDER_Set(this,
                new MDER_Bag_Struct {
                    local_symbolic_type = Symbolic_Bag_Type.Singular,
                    members = new Multiplied_Member(MDER_Tuple_D0),
                    cached_members_meta = new Cached_Members_Meta {
                        tree_member_count = 1,
                        tree_all_unique = true,
                        tree_relational = true,
                    },
                }
            )
        );

        this.MDER_Tuple_Bag_D0C0 = new MDER_Tuple_Bag(this,
            MDER_Heading_D0, MDER_Bag_C0);

        this.MDER_Tuple_Bag_D0C1 = new MDER_Tuple_Bag(this,
            MDER_Heading_D0,
            new MDER_Bag(this,
                new MDER_Bag_Struct {
                    local_symbolic_type = Symbolic_Bag_Type.Singular,
                    members = new Multiplied_Member(MDER_Tuple_D0),
                    cached_members_meta = new Cached_Members_Meta {
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
                new MDER_Excuse(this, this.MDER_Attr_Name(s), this.MDER_Tuple_D0));
        }
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
        MDER_Integer integer = new MDER_Integer(this, as_BigInteger);
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
        return new MDER_Fraction(this, as_Decimal);
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
        return new MDER_Fraction(this, numerator, denominator);
    }

    internal MDER_Bits MDER_Bits(BitArray bit_members)
    {
        if (bit_members.Length == 0)
        {
            return MDER_Bits_C0;
        }
        return new MDER_Bits(this, bit_members);
    }

    internal MDER_Blob MDER_Blob(Byte[] octet_members)
    {
        if (octet_members.Length == 0)
        {
            return MDER_Blob_C0;
        }
        return new MDER_Blob(this, octet_members);
    }

    internal MDER_Text MDER_Text(String code_point_members, Boolean has_any_non_BMP)
    {
        Boolean may_cache = code_point_members.Length <= 200;
        if (may_cache && this.texts.ContainsKey(code_point_members))
        {
            return this.texts[code_point_members];
        }
        MDER_Text text = new MDER_Text(this, code_point_members, has_any_non_BMP);
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
            return new MDER_Array(this,
                new MDER_Array_Struct {
                    local_symbolic_type = Symbolic_Array_Type.Singular,
                    members = new Multiplied_Member(members[0]),
                    cached_members_meta = new Cached_Members_Meta {
                        tree_member_count = 1,
                        tree_all_unique = true,
                        tree_relational = (members[0].WKBT
                            == Internal_Well_Known_Base_Type.MDER_Tuple),
                    },
                }
            );
        }
        return new MDER_Array(this,
            new MDER_Array_Struct {
                local_symbolic_type = Symbolic_Array_Type.Arrayed,
                members = members,
                cached_members_meta = new Cached_Members_Meta(),
            }
        );
    }

    internal MDER_Set MDER_Set(List<Multiplied_Member> members)
    {
        if (members.Count == 0)
        {
            return MDER_Set_C0;
        }
        return new MDER_Set(this,
            new MDER_Bag_Struct {
                local_symbolic_type = Symbolic_Bag_Type.Unique,
                members = new MDER_Bag_Struct {
                    local_symbolic_type = Symbolic_Bag_Type.Arrayed,
                    members = members,
                    cached_members_meta = new Cached_Members_Meta(),
                },
                cached_members_meta = new Cached_Members_Meta {
                    tree_all_unique = true,
                },
            }
        );
    }

    internal MDER_Bag MDER_Bag(List<Multiplied_Member> members)
    {
        if (members.Count == 0)
        {
            return MDER_Bag_C0;
        }
        return new MDER_Bag(this,
            new MDER_Bag_Struct {
                local_symbolic_type = Symbolic_Bag_Type.Arrayed,
                members = members,
                cached_members_meta = new Cached_Members_Meta(),
            }
        );
    }

    internal MDER_Heading MDER_Heading(HashSet<String> attr_names)
    {
        Boolean may_cache = attr_names.Count <= 30
            && Enumerable.All(attr_names, attr_name => attr_name.Length <= 200);
        MDER_Heading heading = new MDER_Heading(this, attr_names);
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
        return new MDER_Tuple(this, attrs);
    }

    internal MDER_Tuple_Array MDER_Tuple_Array(MDER_Heading heading, MDER_Array body)
    {
        if (Object.ReferenceEquals(heading, MDER_Heading_D0))
        {
            if (Object.ReferenceEquals(body, MDER_Array_C0))
            {
                return MDER_Tuple_Array_D0C0;
            }
            if (this.executor.Array__count(body) == 1)
            {
                return MDER_Tuple_Array_D0C1;
            }
        }
        return new MDER_Tuple_Array(this, heading, body);
    }

    internal MDER_Relation MDER_Relation(MDER_Heading heading, MDER_Set body)
    {
        if (Object.ReferenceEquals(heading, MDER_Heading_D0))
        {
            if (Object.ReferenceEquals(body, MDER_Set_C0))
            {
                return MDER_Relation_D0C0;
            }
            if (Set__Pick_Arbitrary_Member(body) is not null)
            {
                return MDER_Relation_D0C1;
            }
        }
        return new MDER_Relation(this, heading, body);
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
        return new MDER_Tuple_Bag(this, heading, body);
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
        return new MDER_Article(this, label, attrs);
    }

    internal MDER_Excuse MDER_Excuse(MDER_Any label, MDER_Tuple attrs)
    {
        return new MDER_Excuse(this, label, attrs);
    }

    internal MDER_Excuse Simple_MDER_Excuse(String value)
    {
        if (well_known_excuses.ContainsKey(value))
        {
            return well_known_excuses[value];
        }
        return new MDER_Excuse(this, this.MDER_Attr_Name(value), this.MDER_Tuple_D0);
    }

    // TODO: Here or in Internal_Executor also have Article_attrs() etc functions
    // that take anything conceptually a Article and let users use it
    // as if it were a MDER_Article_Struct; these have the opposite
    // transformations as MDER_Article() above does.

    internal MDER_Variable New_MDER_Variable(MDER_Any initial_current_value)
    {
        return new MDER_Variable(this, initial_current_value);
    }

    internal MDER_Process New_MDER_Process()
    {
        return new MDER_Process(this);
    }

    internal MDER_Stream New_MDER_Stream()
    {
        return new MDER_Stream(this);
    }

    internal MDER_External New_MDER_External(Object external_value)
    {
        return new MDER_External(this, external_value);
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

    internal void Array__Collapse(MDER_Array array)
    {
        array.tree_root_node = Array__Collapsed_Struct(array.tree_root_node);
    }

    private MDER_Array_Struct Array__Collapsed_Struct(MDER_Array_Struct node)
    {
        switch (node.local_symbolic_type)
        {
            case Symbolic_Array_Type.None:
                // Node is already collapsed.
                // In theory we should never get here assuming that any
                // operations which would knowingly result in the empty
                // Array are optimized to return MDER_Array_C0 directly.
                return MDER_Array_C0.tree_root_node;
            case Symbolic_Array_Type.Singular:
            case Symbolic_Array_Type.Arrayed:
                // Node is already collapsed.
                return node;
            case Symbolic_Array_Type.Catenated:
                MDER_Array_Struct n0 = Array__Collapsed_Struct(node.Tree_Catenated_Members().a0);
                MDER_Array_Struct n1 = Array__Collapsed_Struct(node.Tree_Catenated_Members().a1);
                if (n0.local_symbolic_type == Symbolic_Array_Type.None)
                {
                    return n1;
                }
                if (n1.local_symbolic_type == Symbolic_Array_Type.None)
                {
                    return n0;
                }
                // Both child nodes have at least 1 member, so we will
                // use Arrayed format for merger even if the input
                // members are the same value.
                // This will die if multiplicity greater than an Int32.
                return new MDER_Array_Struct {
                    local_symbolic_type = Symbolic_Array_Type.Arrayed,
                    members = Enumerable.Concat(
                        (n0.local_symbolic_type == Symbolic_Array_Type.Singular
                            ? Enumerable.Repeat(n0.Local_Singular_Members().member,
                                (Int32)n0.Local_Singular_Members().multiplicity)
                            : n0.Local_Arrayed_Members()),
                        (n1.local_symbolic_type == Symbolic_Array_Type.Singular
                            ? Enumerable.Repeat(n1.Local_Singular_Members().member,
                                (Int32)n1.Local_Singular_Members().multiplicity)
                            : n1.Local_Arrayed_Members())
                    ),
                    cached_members_meta = new Cached_Members_Meta(),
                        // TODO: Merge existing source meta where efficient,
                        // tree_member_count and tree_relational in particular.
                };
            default:
                throw new NotImplementedException();
        }
    }

    internal void Set__Collapse(MDER_Set set, Boolean want_indexed = false)
    {
        set.tree_root_node = Bag__Collapsed_Struct(set.tree_root_node, want_indexed);
    }

    internal void Bag__Collapse(MDER_Bag bag, Boolean want_indexed = false)
    {
        bag.tree_root_node = Bag__Collapsed_Struct(bag.tree_root_node, want_indexed);
    }

    private MDER_Bag_Struct Bag__Collapsed_Struct(MDER_Bag_Struct node,
        Boolean want_indexed = false)
    {
        // Note: want_indexed only causes Indexed result when Singular
        // or Arrayed otherwise would be used; None still also used.
        switch (node.local_symbolic_type)
        {
            case Symbolic_Bag_Type.None:
                // Node is already collapsed.
                // In theory we should never get here assuming that any
                // operations which would knowingly result in the empty
                // Bag are optimized to return MDER_Bag_C0 directly.
                return MDER_Bag_C0.tree_root_node;
            case Symbolic_Bag_Type.Singular:
                if (!want_indexed)
                {
                    // Node is already collapsed.
                    return node;
                }
                Multiplied_Member lsm = node.Local_Singular_Members();
                return new MDER_Bag_Struct {
                    local_symbolic_type = Symbolic_Bag_Type.Indexed,
                    members = new Dictionary<MDER_Any, Multiplied_Member>()
                        {{lsm.member, lsm}},
                    cached_members_meta = new Cached_Members_Meta {
                        tree_member_count = lsm.multiplicity,
                        tree_all_unique = (lsm.multiplicity == 1),
                        tree_relational = (lsm.member.WKBT
                            == Internal_Well_Known_Base_Type.MDER_Tuple),
                    },
                };
            case Symbolic_Bag_Type.Arrayed:
                if (!want_indexed)
                {
                    // Node is already collapsed.
                    return node;
                }
                List<Multiplied_Member> ary_src_list = node.Local_Arrayed_Members();
                Dictionary<MDER_Any, Multiplied_Member> ary_res_dict
                    = new Dictionary<MDER_Any, Multiplied_Member>();
                foreach (Multiplied_Member m in ary_src_list)
                {
                    if (!ary_res_dict.ContainsKey(m.member))
                    {
                        ary_res_dict.Add(m.member, m.Clone());
                    }
                    else
                    {
                        ary_res_dict[m.member].multiplicity ++;
                    }
                }
                return new MDER_Bag_Struct {
                    local_symbolic_type = Symbolic_Bag_Type.Indexed,
                    members = ary_res_dict,
                    cached_members_meta = new Cached_Members_Meta {
                        tree_member_count = ary_src_list.Count,
                        tree_all_unique = node.cached_members_meta.tree_all_unique,
                        tree_relational = node.cached_members_meta.tree_relational,
                    },
                };
            case Symbolic_Bag_Type.Indexed:
                // Node is already collapsed.
                return node;
            case Symbolic_Bag_Type.Unique:
                MDER_Bag_Struct uni_pa = Bag__Collapsed_Struct(
                    node: node.Tree_Unique_Members(), want_indexed: true);
                if (uni_pa.local_symbolic_type == Symbolic_Bag_Type.None)
                {
                    return uni_pa;
                }
                Dictionary<MDER_Any, Multiplied_Member> uni_src_dict
                    = uni_pa.Local_Indexed_Members();
                return new MDER_Bag_Struct {
                    local_symbolic_type = Symbolic_Bag_Type.Indexed,
                    members = uni_src_dict.ToDictionary(
                        m => m.Key, m => new Multiplied_Member(m.Key, 1)),
                    cached_members_meta = new Cached_Members_Meta {
                        tree_member_count = uni_src_dict.Count,
                        tree_all_unique = true,
                        tree_relational = uni_pa.cached_members_meta.tree_relational,
                    },
                };
            case Symbolic_Bag_Type.Summed:
                MDER_Bag_Struct n0 = Bag__Collapsed_Struct(
                    node: node.Tree_Summed_Members().a0, want_indexed: true);
                MDER_Bag_Struct n1 = Bag__Collapsed_Struct(
                    node: node.Tree_Summed_Members().a1, want_indexed: true);
                if (n0.local_symbolic_type == Symbolic_Bag_Type.None)
                {
                    return n1;
                }
                if (n1.local_symbolic_type == Symbolic_Bag_Type.None)
                {
                    return n0;
                }
                Dictionary<MDER_Any, Multiplied_Member> n0_src_dict
                    = n0.Local_Indexed_Members();
                Dictionary<MDER_Any, Multiplied_Member> n1_src_dict
                    = n1.Local_Indexed_Members();
                Dictionary<MDER_Any, Multiplied_Member> res_dict
                    = new Dictionary<MDER_Any, Multiplied_Member>(n0_src_dict);
                foreach (Multiplied_Member m in n1_src_dict.Values)
                {
                    if (!res_dict.ContainsKey(m.member))
                    {
                        res_dict.Add(m.member, m);
                    }
                    else
                    {
                        res_dict[m.member] = new Multiplied_Member(m.member,
                            res_dict[m.member].multiplicity + m.multiplicity);
                    }
                }
                return new MDER_Bag_Struct {
                    local_symbolic_type = Symbolic_Bag_Type.Indexed,
                    members = res_dict,
                    cached_members_meta = new Cached_Members_Meta(),
                        // TODO: Merge existing source meta where efficient,
                        // tree_member_count and tree_relational in particular.
                };
            default:
                throw new NotImplementedException();
        }
    }

    internal MDER_Any Array__Pick_Arbitrary_Member(MDER_Array array)
    {
        return Array__Pick_Arbitrary_Node_Member(array.tree_root_node);
    }

    private MDER_Any Array__Pick_Arbitrary_Node_Member(MDER_Array_Struct node)
    {
        switch (node.local_symbolic_type)
        {
            case Symbolic_Array_Type.None:
                return null;
            case Symbolic_Array_Type.Singular:
                return node.Local_Singular_Members().member;
            case Symbolic_Array_Type.Arrayed:
                return node.Local_Arrayed_Members()[0];
            case Symbolic_Array_Type.Catenated:
                return Array__Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().a0)
                    ?? Array__Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().a1);
            default:
                throw new NotImplementedException();
        }
    }

    internal Boolean Array__Is_Relational(MDER_Array array)
    {
        return Array__Tree_Relational(array.tree_root_node);
    }

    private Boolean Array__Tree_Relational(MDER_Array_Struct node)
    {
        if (node.cached_members_meta.tree_relational is null)
        {
            Boolean tr = true;
            switch (node.local_symbolic_type)
            {
                case Symbolic_Array_Type.None:
                    tr = true;
                    break;
                case Symbolic_Array_Type.Singular:
                    tr = node.Local_Singular_Members().member.WKBT
                        == Internal_Well_Known_Base_Type.MDER_Tuple;
                    break;
                case Symbolic_Array_Type.Arrayed:
                    MDER_Any m0 = Array__Pick_Arbitrary_Node_Member(node);
                    tr = m0.WKBT == Internal_Well_Known_Base_Type.MDER_Tuple
                        && Enumerable.All(
                            node.Local_Arrayed_Members(),
                            m => m.WKBT == Internal_Well_Known_Base_Type.MDER_Tuple
                                && Tuple__Same_Heading((MDER_Tuple)m, (MDER_Tuple)m0)
                        );
                    break;
                case Symbolic_Array_Type.Catenated:
                    MDER_Any pm0 = Array__Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().a0);
                    MDER_Any sm0 = Array__Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().a1);
                    tr = Array__Tree_Relational(node.Tree_Catenated_Members().a0)
                        && Array__Tree_Relational(node.Tree_Catenated_Members().a1)
                        && (pm0 is null || sm0 is null || Tuple__Same_Heading((MDER_Tuple)pm0, (MDER_Tuple)sm0));
                    break;
                default:
                    throw new NotImplementedException();
            }
            node.cached_members_meta.tree_relational = tr;
        }
        return (Boolean)node.cached_members_meta.tree_relational;
    }

    internal MDER_Any Set__Pick_Arbitrary_Member(MDER_Set set)
    {
        return Bag__Pick_Arbitrary_Node_Member(set.tree_root_node);
    }

    internal Boolean Set__Is_Relational(MDER_Set set)
    {
        return Bag__Tree_Relational(set.tree_root_node);
    }

    internal MDER_Any Bag__Pick_Arbitrary_Member(MDER_Bag bag)
    {
        return Bag__Pick_Arbitrary_Node_Member(bag.tree_root_node);
    }

    private MDER_Any Bag__Pick_Arbitrary_Node_Member(MDER_Bag_Struct node)
    {
        if (node.cached_members_meta.tree_member_count == 0)
        {
            return null;
        }
        switch (node.local_symbolic_type)
        {
            case Symbolic_Bag_Type.None:
                return null;
            case Symbolic_Bag_Type.Singular:
                return node.Local_Singular_Members().member;
            case Symbolic_Bag_Type.Arrayed:
                return node.Local_Arrayed_Members()[0].member;
            case Symbolic_Bag_Type.Indexed:
                return node.Local_Indexed_Members().First().Value.member;
            case Symbolic_Bag_Type.Unique:
                return Bag__Pick_Arbitrary_Node_Member(node.Tree_Unique_Members());
            case Symbolic_Bag_Type.Summed:
                return Bag__Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().a0)
                    ?? Bag__Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().a1);
            default:
                throw new NotImplementedException();
        }
    }

    internal Boolean Bag__Is_Relational(MDER_Bag bag)
    {
        return Bag__Tree_Relational(bag.tree_root_node);
    }

    private Boolean Bag__Tree_Relational(MDER_Bag_Struct node)
    {
        if (node.cached_members_meta.tree_relational is null)
        {
            Boolean tr = true;
            switch (node.local_symbolic_type)
            {
                case Symbolic_Bag_Type.None:
                    tr = true;
                    break;
                case Symbolic_Bag_Type.Singular:
                    tr = node.Local_Singular_Members().member.WKBT
                        == Internal_Well_Known_Base_Type.MDER_Tuple;
                    break;
                case Symbolic_Bag_Type.Arrayed:
                    MDER_Any m0 = Bag__Pick_Arbitrary_Node_Member(node);
                    tr = m0.WKBT == Internal_Well_Known_Base_Type.MDER_Tuple
                        && Enumerable.All(
                            node.Local_Arrayed_Members(),
                            m => m.member.WKBT == Internal_Well_Known_Base_Type.MDER_Tuple
                                && Tuple__Same_Heading((MDER_Tuple)m.member, (MDER_Tuple)m0)
                        );
                    break;
                case Symbolic_Bag_Type.Indexed:
                    MDER_Any im0 = Bag__Pick_Arbitrary_Node_Member(node);
                    tr = im0.WKBT == Internal_Well_Known_Base_Type.MDER_Tuple
                        && Enumerable.All(
                            node.Local_Indexed_Members().Values,
                            m => m.member.WKBT == Internal_Well_Known_Base_Type.MDER_Tuple
                                && Tuple__Same_Heading((MDER_Tuple)m.member, (MDER_Tuple)im0)
                        );
                    break;
                case Symbolic_Bag_Type.Unique:
                    tr = Bag__Tree_Relational(node.Tree_Unique_Members());
                    break;
                case Symbolic_Bag_Type.Summed:
                    tr = Bag__Tree_Relational(node.Tree_Summed_Members().a0)
                        && Bag__Tree_Relational(node.Tree_Summed_Members().a1);
                    MDER_Tuple pam0 = (MDER_Tuple)Bag__Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().a0);
                    MDER_Tuple eam0 = (MDER_Tuple)Bag__Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().a1);
                    if (pam0 is not null && eam0 is not null)
                    {
                        tr = tr && Tuple__Same_Heading(pam0, eam0);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            node.cached_members_meta.tree_relational = tr;
        }
        return (Boolean)node.cached_members_meta.tree_relational;
    }

    internal MDER_Heading Tuple__Heading(MDER_Tuple tuple)
    {
        return this.MDER_Heading(new HashSet<String>(tuple.attrs.Keys));
    }

    internal Boolean Tuple__Same_Heading(MDER_Tuple t1, MDER_Tuple t2)
    {
        if (Object.ReferenceEquals(t1,t2))
        {
            return true;
        }
        Dictionary<String, MDER_Any> attrs1 = t1.attrs;
        Dictionary<String, MDER_Any> attrs2 = t2.attrs;
        return (attrs1.Count == attrs2.Count)
            && Enumerable.All(attrs1, attr => attrs2.ContainsKey(attr.Key));
    }

    internal Boolean Tuple__Has_Heading(MDER_Tuple t, MDER_Heading h)
    {
        Dictionary<String, MDER_Any> attrs = t.attrs;
        HashSet<String> attr_names = h.attr_names;
        return (attrs.Count == attr_names.Count)
            && Enumerable.All(attr_names, attr_name => attrs.ContainsKey(attr_name));
    }

    internal MDER_Any MDER_Text_from_UTF_8_MDER_Blob(MDER_Blob value)
    {
        Byte[] octets = value.octet_members;
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
                return Simple_MDER_Excuse("X_Unicode_Blob_Not_UTF_8");
            }
            return this.MDER_Text(
                s,
                (tr == Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
            );
        }
        catch
        {
            return Simple_MDER_Excuse("X_Unicode_Blob_Not_UTF_8");
        }
    }
}
