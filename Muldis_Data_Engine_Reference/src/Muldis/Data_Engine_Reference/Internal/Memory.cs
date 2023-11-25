using System.Collections;
using System.Numerics;
using System.Text;

namespace Muldis.Data_Engine_Reference.Internal;

// Muldis.Data_Engine_Reference.Internal.Memory
// Provides a virtual machine memory pool where Muldis Data Language values
// and variables live, which exploits the "flyweight pattern" for
// efficiency in both performance and memory usage.
// Conceptually, Memory is a singleton, in that typically only one
// would be used per virtual machine; in practice, only the set of all
// VM values/vars/processes/etc that would interact must share one.
// Memory might have multiple pools, say, for separate handling of
// entities that are short-lived versus longer-lived.
// ? Note that .NET Core lacks System.Runtime.Caching or similar built-in
// so for any situations we might have used such, we roll our own.
// Some caches are logically just HashSets, but we need the ability to
// fetch the actual cached objects which are the set members so we can
// reuse them, not just know they exist, so Dictionaries are used instead.
// Note this is called "interning" in a variety of programming languages;
// see https://en.wikipedia.org/wiki/String_interning for more on that.

internal class Memory
{
    internal readonly Identity_Generator identity_generator;
    internal readonly Preview_Generator preview_generator;

    internal readonly Executor executor;

    // MDL_Ignorance 0iIGNORANCE (type default value) value.
    private readonly MDL_Ignorance MDL_0iIGNORANCE;

    // MDL_Boolean 0bFALSE (type default value) and 0bTRUE values.
    private readonly MDL_False MDL_0bFALSE;
    private readonly MDL_True MDL_0bTRUE;

    // MDL_Integer value cache.
    // Seeded with {-1,0,1}.
    // Limited to 10K entries each in range -2B..2B (signed 32-bit integer).
    // The MDL_Integer 0 is the type default value.
    private readonly Dictionary<Int32, MDL_Integer> integers;

    // MDL_Fraction 0.0 (type default value).
    private readonly MDL_Fraction MDL_Fraction_0;

    // MDL_Bits with no members (type default value).
    internal readonly MDL_Bits MDL_Bits_C0;

    // MDL_Blob with no members (type default value).
    internal readonly MDL_Blob MDL_Blob_C0;

    // MDL_Text value cache.
    // Seeded with empty string and well-known attribute names.
    // Limited to 10K entries each not more than 200 code points length.
    // The MDL_Text empty string is the type default value.
    private readonly Dictionary<String, MDL_Text> texts;

    // MDL_Array with no members (type default value).
    internal readonly MDL_Array MDL_Array_C0;

    // MDL_Set with no members (type default value).
    internal readonly MDL_Set MDL_Set_C0;

    // MDL_Bag with no members (type default value).
    internal readonly MDL_Bag MDL_Bag_C0;

    // MDL_Heading value cache.
    // Limited to 10K entries of shorter size.
    // The MDL_Heading nullary (zero attribute) is the type default value.
    private readonly Dictionary<MDL_Heading, MDL_Heading> headings;
    internal readonly MDL_Heading MDL_Heading_D0;
    internal readonly MDL_Heading MDL_Heading_0;
    internal readonly MDL_Heading MDL_Heading_1;
    internal readonly MDL_Heading MDL_Heading_2;
    internal readonly MDL_Heading MDL_Heading_0_1;
    internal readonly MDL_Heading MDL_Heading_0_2;
    internal readonly MDL_Heading MDL_Heading_1_2;
    internal readonly MDL_Heading MDL_Heading_0_1_2;

    // MDL_Tuple with no attributes (type default value).
    internal readonly MDL_Tuple MDL_Tuple_D0;

    // MDL_Tuple_Array with no attributes and no members
    // (type default value), or with no attributes and 1 member.
    internal readonly MDL_Tuple_Array MDL_Tuple_Array_D0C0;
    internal readonly MDL_Tuple_Array MDL_Tuple_Array_D0C1;

    // MDL_Relation with no attributes and no members
    // (type default value), or with no attributes and 1 member.
    internal readonly MDL_Relation MDL_Relation_D0C0;
    internal readonly MDL_Relation MDL_Relation_D0C1;

    // MDL_Tuple_Bag with no attributes and no members
    // (type default value), or with no attributes and 1 member.
    internal readonly MDL_Tuple_Bag MDL_Tuple_Bag_D0C0;
    internal readonly MDL_Tuple_Bag MDL_Tuple_Bag_D0C1;

    // MDL_Article with False label and no attributes
    // (type default value but not actually useful in practice).
    private readonly MDL_Article false_nullary_article;

    // All well known MDL_Excuse values.
    internal readonly Dictionary<String, MDL_Any> well_known_excuses;

    internal Memory()
    {
        this.identity_generator = new Identity_Generator();
        this.preview_generator = new Preview_Generator();

        this.executor = new Executor(this);

        this.MDL_0iIGNORANCE = new MDL_Ignorance(this);

        this.MDL_0bFALSE = new MDL_False(this);
        this.MDL_0bTRUE = new MDL_True(this);

        this.integers = new Dictionary<Int32, MDL_Integer>();
        for (Int32 i = -1; i <= 1; i++)
        {
            MDL_Integer v = this.MDL_Integer(i);
        }

        this.MDL_Fraction_0 = new MDL_Fraction(this, 0.0M,
            this.integers[0].as_BigInteger, this.integers[1].as_BigInteger);

        this.MDL_Bits_C0 = new MDL_Bits(this, new BitArray(0));

        this.MDL_Blob_C0 = new MDL_Blob(this, new Byte[] {});

        this.texts = new Dictionary<String, MDL_Text>();
        foreach (String s in new String[] {"", "\u0000", "\u0001", "\u0002"})
        {
            MDL_Text v = this.MDL_Text(s, false);
        }
        foreach (String s in Constants.Strings__Seeded_Non_Positional_Attr_Names())
        {
            MDL_Text v = this.MDL_Text(s, false);
        }

        this.MDL_Array_C0 = new MDL_Array(this,
            new MDL_Array_Struct {
                local_symbolic_type = Symbolic_Array_Type.None,
                cached_members_meta = new Cached_Members_Meta {
                    tree_member_count = 0,
                    tree_all_unique = true,
                    tree_relational = true,
                },
            }
        );

        this.MDL_Set_C0 = new MDL_Set(this,
            new MDL_Bag_Struct {
                local_symbolic_type = Symbolic_Bag_Type.None,
                cached_members_meta = new Cached_Members_Meta {
                    tree_member_count = 0,
                    tree_all_unique = true,
                    tree_relational = true,
                },
            }
        );

        this.MDL_Bag_C0 = new MDL_Bag(this,
            new MDL_Bag_Struct {
                local_symbolic_type = Symbolic_Bag_Type.None,
                cached_members_meta = new Cached_Members_Meta {
                    tree_member_count = 0,
                    tree_all_unique = true,
                    tree_relational = true,
                },
            }
        );

        this.headings = new Dictionary<MDL_Heading, MDL_Heading>();
        this.MDL_Heading_D0 = new MDL_Heading(this, new HashSet<String>());
        this.MDL_Heading_0 = new MDL_Heading(this, new HashSet<String>()
            {"\u0000"});
        this.MDL_Heading_1 = new MDL_Heading(this, new HashSet<String>()
            {"\u0001"});
        this.MDL_Heading_2 = new MDL_Heading(this, new HashSet<String>()
            {"\u0002"});
        this.MDL_Heading_0_1 = new MDL_Heading(this, new HashSet<String>()
            {"\u0000", "\u0001"});
        this.MDL_Heading_0_2 = new MDL_Heading(this, new HashSet<String>()
            {"\u0000", "\u0002"});
        this.MDL_Heading_1_2 = new MDL_Heading(this, new HashSet<String>()
            {"\u0001", "\u0002"});
        this.MDL_Heading_0_1_2 = new MDL_Heading(this, new HashSet<String>()
            {"\u0000", "\u0001", "\u0002"});

        this.MDL_Tuple_D0 = new MDL_Tuple(this, new Dictionary<String, MDL_Any>());

        this.false_nullary_article
            = new MDL_Article(this, this.MDL_0bFALSE, this.MDL_Tuple_D0);

        this.MDL_Tuple_Array_D0C0 = new MDL_Tuple_Array(this,
            MDL_Heading_D0, MDL_Array_C0);

        this.MDL_Tuple_Array_D0C1 = new MDL_Tuple_Array(this,
            MDL_Heading_D0,
            new MDL_Array(this,
                new MDL_Array_Struct {
                    local_symbolic_type = Symbolic_Array_Type.Singular,
                    members = new Multiplied_Member(MDL_Tuple_D0),
                    cached_members_meta = new Cached_Members_Meta {
                        tree_member_count = 1,
                        tree_all_unique = true,
                        tree_relational = true,
                    },
                }
            )
        );

        this.MDL_Relation_D0C0 = new MDL_Relation(this,
            MDL_Heading_D0, MDL_Set_C0);

        this.MDL_Relation_D0C1 = new MDL_Relation(this,
            MDL_Heading_D0,
            new MDL_Set(this,
                new MDL_Bag_Struct {
                    local_symbolic_type = Symbolic_Bag_Type.Singular,
                    members = new Multiplied_Member(MDL_Tuple_D0),
                    cached_members_meta = new Cached_Members_Meta {
                        tree_member_count = 1,
                        tree_all_unique = true,
                        tree_relational = true,
                    },
                }
            )
        );

        this.MDL_Tuple_Bag_D0C0 = new MDL_Tuple_Bag(this,
            MDL_Heading_D0, MDL_Bag_C0);

        this.MDL_Tuple_Bag_D0C1 = new MDL_Tuple_Bag(this,
            MDL_Heading_D0,
            new MDL_Bag(this,
                new MDL_Bag_Struct {
                    local_symbolic_type = Symbolic_Bag_Type.Singular,
                    members = new Multiplied_Member(MDL_Tuple_D0),
                    cached_members_meta = new Cached_Members_Meta {
                        tree_member_count = 1,
                        tree_all_unique = true,
                        tree_relational = true,
                    },
                }
            )
        );

        well_known_excuses = new Dictionary<String, MDL_Any>();
        foreach (String s in Constants.Strings__Well_Known_Excuses())
        {
            well_known_excuses.Add(
                s,
                new MDL_Any {
                    memory = this,
                    WKBT = Well_Known_Base_Type.MDL_Excuse,
                    details = new Dictionary<String, MDL_Any>() {{"\u0000", this.MDL_Attr_Name(s)}},
                }
            );
        }
    }

    internal MDL_Ignorance MDL_Ignorance()
    {
        return this.MDL_0iIGNORANCE;
    }

    internal MDL_Boolean MDL_Boolean(Boolean value)
    {
        return value ? MDL_0bTRUE : MDL_0bFALSE;
    }

    internal MDL_False MDL_False()
    {
        return this.MDL_0bFALSE;
    }

    internal MDL_True MDL_True()
    {
        return this.MDL_0bTRUE;
    }

    internal MDL_Integer MDL_Integer(BigInteger as_BigInteger)
    {
        Boolean may_cache = as_BigInteger >= -2000000000 && as_BigInteger <= 2000000000;
        if (may_cache && this.integers.ContainsKey((Int32)as_BigInteger))
        {
            return this.integers[(Int32)as_BigInteger];
        }
        MDL_Integer integer = new MDL_Integer(this, as_BigInteger);
        if (may_cache && this.integers.Count < 10000)
        {
            this.integers.Add((Int32)as_BigInteger, integer);
        }
        return integer;
    }

    internal MDL_Fraction MDL_Fraction(Decimal as_Decimal)
    {
        if (as_Decimal == 0)
        {
            return MDL_Fraction_0;
        }
        return new MDL_Fraction(this, as_Decimal);
    }

    internal MDL_Fraction MDL_Fraction(BigInteger numerator, BigInteger denominator)
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
        return new MDL_Fraction(this, numerator, denominator);
    }

    internal MDL_Bits MDL_Bits(BitArray bit_members)
    {
        if (bit_members.Length == 0)
        {
            return MDL_Bits_C0;
        }
        return new MDL_Bits(this, bit_members);
    }

    internal MDL_Blob MDL_Blob(Byte[] octet_members)
    {
        if (octet_members.Length == 0)
        {
            return MDL_Blob_C0;
        }
        return new MDL_Blob(this, octet_members);
    }

    internal MDL_Text MDL_Text(String code_point_members, Boolean has_any_non_BMP)
    {
        Boolean may_cache = code_point_members.Length <= 200;
        if (may_cache && this.texts.ContainsKey(code_point_members))
        {
            return this.texts[code_point_members];
        }
        MDL_Text text = new MDL_Text(this, code_point_members, has_any_non_BMP);
        if (may_cache && this.texts.Count < 10000)
        {
            this.texts.Add(code_point_members, text);
        }
        return text;
    }

    // Temporary wrapper.
    internal MDL_Text MDL_Attr_Name(String code_point_members)
    {
        return this.MDL_Text(code_point_members, false);
    }

    internal MDL_Array MDL_Array(List<MDL_Any> members)
    {
        if (members.Count == 0)
        {
            return MDL_Array_C0;
        }
        if (members.Count == 1)
        {
            return new MDL_Array(this,
                new MDL_Array_Struct {
                    local_symbolic_type = Symbolic_Array_Type.Singular,
                    members = new Multiplied_Member(members[0]),
                    cached_members_meta = new Cached_Members_Meta {
                        tree_member_count = 1,
                        tree_all_unique = true,
                        tree_relational = (members[0].WKBT
                            == Well_Known_Base_Type.MDL_Tuple),
                    },
                }
            );
        }
        return new MDL_Array(this,
            new MDL_Array_Struct {
                local_symbolic_type = Symbolic_Array_Type.Arrayed,
                members = members,
                cached_members_meta = new Cached_Members_Meta(),
            }
        );
    }

    internal MDL_Set MDL_Set(List<Multiplied_Member> members)
    {
        if (members.Count == 0)
        {
            return MDL_Set_C0;
        }
        return new MDL_Set(this,
            new MDL_Bag_Struct {
                local_symbolic_type = Symbolic_Bag_Type.Unique,
                members = new MDL_Bag_Struct {
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

    internal MDL_Bag MDL_Bag(List<Multiplied_Member> members)
    {
        if (members.Count == 0)
        {
            return MDL_Bag_C0;
        }
        return new MDL_Bag(this,
            new MDL_Bag_Struct {
                local_symbolic_type = Symbolic_Bag_Type.Arrayed,
                members = members,
                cached_members_meta = new Cached_Members_Meta(),
            }
        );
    }

    internal MDL_Heading MDL_Heading(HashSet<String> attr_names)
    {
        Boolean may_cache = attr_names.Count <= 30
            && Enumerable.All(attr_names, attr_name => attr_name.Length <= 200);
        MDL_Heading heading = new MDL_Heading(this, attr_names);
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

    internal MDL_Tuple MDL_Tuple(Dictionary<String, MDL_Any> attrs)
    {
        if (attrs.Count == 0)
        {
            return MDL_Tuple_D0;
        }
        return new MDL_Tuple(this, attrs);
    }

    internal MDL_Tuple_Array MDL_Tuple_Array(MDL_Heading heading, MDL_Array body)
    {
        if (Object.ReferenceEquals(heading, MDL_Heading_D0))
        {
            if (Object.ReferenceEquals(body, MDL_Array_C0))
            {
                return MDL_Tuple_Array_D0C0;
            }
            if (this.executor.Array__count(body) == 1)
            {
                return MDL_Tuple_Array_D0C1;
            }
        }
        return new MDL_Tuple_Array(this, heading, body);
    }

    internal MDL_Relation MDL_Relation(MDL_Heading heading, MDL_Set body)
    {
        if (Object.ReferenceEquals(heading, MDL_Heading_D0))
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
        return new MDL_Relation(this, heading, body);
    }

    internal MDL_Tuple_Bag MDL_Tuple_Bag(MDL_Heading heading, MDL_Bag body)
    {
        if (Object.ReferenceEquals(heading, MDL_Heading_D0))
        {
            if (Object.ReferenceEquals(body, MDL_Bag_C0))
            {
                return MDL_Tuple_Bag_D0C0;
            }
            // TODO
            //if (this.executor.Bag__count(body) == 1)
            //{
            //    return MDL_Tuple_Bag_D0C1;
            //}
        }
        return new MDL_Tuple_Bag(this, heading, body);
    }

    internal MDL_Article MDL_Article(MDL_Any label, MDL_Tuple attrs)
    {
        if (Object.ReferenceEquals(label, MDL_0bFALSE)
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
        return new MDL_Article(this, label, attrs);
    }

    // TODO: Here or in Executor also have Article_attrs() etc functions
    // that take anything conceptually a Article and let users use it
    // as if it were a MDL_Article_Struct; these have the opposite
    // transformations as MDL_Article() above does.

    internal MDL_Variable New_MDL_Variable(MDL_Any initial_current_value)
    {
        return new MDL_Variable(this, initial_current_value);
    }

    internal MDL_Process New_MDL_Process()
    {
        return new MDL_Process(this);
    }

    internal MDL_Stream New_MDL_Stream()
    {
        return new MDL_Stream(this);
    }

    internal MDL_External New_MDL_External(Object external_value)
    {
        return new MDL_External(this, external_value);
    }

    internal MDL_Any MDL_Excuse(MDL_Any attrs)
    {
        return new MDL_Any {
            memory = this,
            WKBT = Well_Known_Base_Type.MDL_Excuse,
            details = attrs.details,
        };
    }

    internal MDL_Any Simple_MDL_Excuse(String value)
    {
        if (well_known_excuses.ContainsKey(value))
        {
            return well_known_excuses[value];
        }
        return new MDL_Any {
            memory = this,
            WKBT = Well_Known_Base_Type.MDL_Excuse,
            details = new Dictionary<String, MDL_Any>() {{"\u0000", this.MDL_Attr_Name(value)}},
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

    internal void Array__Collapse(MDL_Array array)
    {
        array.tree_root_node = Array__Collapsed_Struct(array.tree_root_node);
    }

    private MDL_Array_Struct Array__Collapsed_Struct(MDL_Array_Struct node)
    {
        switch (node.local_symbolic_type)
        {
            case Symbolic_Array_Type.None:
                // Node is already collapsed.
                // In theory we should never get here assuming that any
                // operations which would knowingly result in the empty
                // Array are optimized to return MDL_Array_C0 directly.
                return MDL_Array_C0.tree_root_node;
            case Symbolic_Array_Type.Singular:
            case Symbolic_Array_Type.Arrayed:
                // Node is already collapsed.
                return node;
            case Symbolic_Array_Type.Catenated:
                MDL_Array_Struct n0 = Array__Collapsed_Struct(node.Tree_Catenated_Members().a0);
                MDL_Array_Struct n1 = Array__Collapsed_Struct(node.Tree_Catenated_Members().a1);
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
                return new MDL_Array_Struct {
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

    internal void Set__Collapse(MDL_Set set, Boolean want_indexed = false)
    {
        set.tree_root_node = Bag__Collapsed_Struct(set.tree_root_node, want_indexed);
    }

    internal void Bag__Collapse(MDL_Bag bag, Boolean want_indexed = false)
    {
        bag.tree_root_node = Bag__Collapsed_Struct(bag.tree_root_node, want_indexed);
    }

    private MDL_Bag_Struct Bag__Collapsed_Struct(MDL_Bag_Struct node,
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
                // Bag are optimized to return MDL_Bag_C0 directly.
                return MDL_Bag_C0.tree_root_node;
            case Symbolic_Bag_Type.Singular:
                if (!want_indexed)
                {
                    // Node is already collapsed.
                    return node;
                }
                Multiplied_Member lsm = node.Local_Singular_Members();
                return new MDL_Bag_Struct {
                    local_symbolic_type = Symbolic_Bag_Type.Indexed,
                    members = new Dictionary<MDL_Any, Multiplied_Member>()
                        {{lsm.member, lsm}},
                    cached_members_meta = new Cached_Members_Meta {
                        tree_member_count = lsm.multiplicity,
                        tree_all_unique = (lsm.multiplicity == 1),
                        tree_relational = (lsm.member.WKBT
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
                Dictionary<MDL_Any, Multiplied_Member> ary_res_dict
                    = new Dictionary<MDL_Any, Multiplied_Member>();
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
                return new MDL_Bag_Struct {
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
                MDL_Bag_Struct uni_pa = Bag__Collapsed_Struct(
                    node: node.Tree_Unique_Members(), want_indexed: true);
                if (uni_pa.local_symbolic_type == Symbolic_Bag_Type.None)
                {
                    return uni_pa;
                }
                Dictionary<MDL_Any, Multiplied_Member> uni_src_dict
                    = uni_pa.Local_Indexed_Members();
                return new MDL_Bag_Struct {
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
                MDL_Bag_Struct n0 = Bag__Collapsed_Struct(
                    node: node.Tree_Summed_Members().a0, want_indexed: true);
                MDL_Bag_Struct n1 = Bag__Collapsed_Struct(
                    node: node.Tree_Summed_Members().a1, want_indexed: true);
                if (n0.local_symbolic_type == Symbolic_Bag_Type.None)
                {
                    return n1;
                }
                if (n1.local_symbolic_type == Symbolic_Bag_Type.None)
                {
                    return n0;
                }
                Dictionary<MDL_Any, Multiplied_Member> n0_src_dict
                    = n0.Local_Indexed_Members();
                Dictionary<MDL_Any, Multiplied_Member> n1_src_dict
                    = n1.Local_Indexed_Members();
                Dictionary<MDL_Any, Multiplied_Member> res_dict
                    = new Dictionary<MDL_Any, Multiplied_Member>(n0_src_dict);
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
                return new MDL_Bag_Struct {
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

    internal MDL_Any Array__Pick_Arbitrary_Member(MDL_Array array)
    {
        return Array__Pick_Arbitrary_Node_Member(array.tree_root_node);
    }

    private MDL_Any Array__Pick_Arbitrary_Node_Member(MDL_Array_Struct node)
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

    internal Boolean Array__Is_Relational(MDL_Array array)
    {
        return Array__Tree_Relational(array.tree_root_node);
    }

    private Boolean Array__Tree_Relational(MDL_Array_Struct node)
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
                        == Well_Known_Base_Type.MDL_Tuple;
                    break;
                case Symbolic_Array_Type.Arrayed:
                    MDL_Any m0 = Array__Pick_Arbitrary_Node_Member(node);
                    tr = m0.WKBT == Well_Known_Base_Type.MDL_Tuple
                        && Enumerable.All(
                            node.Local_Arrayed_Members(),
                            m => m.WKBT == Well_Known_Base_Type.MDL_Tuple
                                && Tuple__Same_Heading((MDL_Tuple)m, (MDL_Tuple)m0)
                        );
                    break;
                case Symbolic_Array_Type.Catenated:
                    MDL_Any pm0 = Array__Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().a0);
                    MDL_Any sm0 = Array__Pick_Arbitrary_Node_Member(node.Tree_Catenated_Members().a1);
                    tr = Array__Tree_Relational(node.Tree_Catenated_Members().a0)
                        && Array__Tree_Relational(node.Tree_Catenated_Members().a1)
                        && (pm0 is null || sm0 is null || Tuple__Same_Heading((MDL_Tuple)pm0, (MDL_Tuple)sm0));
                    break;
                default:
                    throw new NotImplementedException();
            }
            node.cached_members_meta.tree_relational = tr;
        }
        return (Boolean)node.cached_members_meta.tree_relational;
    }

    internal MDL_Any Set__Pick_Arbitrary_Member(MDL_Set set)
    {
        return Bag__Pick_Arbitrary_Node_Member(set.tree_root_node);
    }

    internal Boolean Set__Is_Relational(MDL_Set set)
    {
        return Bag__Tree_Relational(set.tree_root_node);
    }

    internal MDL_Any Bag__Pick_Arbitrary_Member(MDL_Bag bag)
    {
        return Bag__Pick_Arbitrary_Node_Member(bag.tree_root_node);
    }

    private MDL_Any Bag__Pick_Arbitrary_Node_Member(MDL_Bag_Struct node)
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

    internal Boolean Bag__Is_Relational(MDL_Bag bag)
    {
        return Bag__Tree_Relational(bag.tree_root_node);
    }

    private Boolean Bag__Tree_Relational(MDL_Bag_Struct node)
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
                        == Well_Known_Base_Type.MDL_Tuple;
                    break;
                case Symbolic_Bag_Type.Arrayed:
                    MDL_Any m0 = Bag__Pick_Arbitrary_Node_Member(node);
                    tr = m0.WKBT == Well_Known_Base_Type.MDL_Tuple
                        && Enumerable.All(
                            node.Local_Arrayed_Members(),
                            m => m.member.WKBT == Well_Known_Base_Type.MDL_Tuple
                                && Tuple__Same_Heading((MDL_Tuple)m.member, (MDL_Tuple)m0)
                        );
                    break;
                case Symbolic_Bag_Type.Indexed:
                    MDL_Any im0 = Bag__Pick_Arbitrary_Node_Member(node);
                    tr = im0.WKBT == Well_Known_Base_Type.MDL_Tuple
                        && Enumerable.All(
                            node.Local_Indexed_Members().Values,
                            m => m.member.WKBT == Well_Known_Base_Type.MDL_Tuple
                                && Tuple__Same_Heading((MDL_Tuple)m.member, (MDL_Tuple)im0)
                        );
                    break;
                case Symbolic_Bag_Type.Unique:
                    tr = Bag__Tree_Relational(node.Tree_Unique_Members());
                    break;
                case Symbolic_Bag_Type.Summed:
                    tr = Bag__Tree_Relational(node.Tree_Summed_Members().a0)
                        && Bag__Tree_Relational(node.Tree_Summed_Members().a1);
                    MDL_Tuple pam0 = (MDL_Tuple)Bag__Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().a0);
                    MDL_Tuple eam0 = (MDL_Tuple)Bag__Pick_Arbitrary_Node_Member(node.Tree_Summed_Members().a1);
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

    internal MDL_Heading Tuple__Heading(MDL_Tuple tuple)
    {
        return this.MDL_Heading(new HashSet<String>(tuple.attrs.Keys));
    }

    internal Boolean Tuple__Same_Heading(MDL_Tuple t1, MDL_Tuple t2)
    {
        if (Object.ReferenceEquals(t1,t2))
        {
            return true;
        }
        Dictionary<String, MDL_Any> attrs1 = t1.attrs;
        Dictionary<String, MDL_Any> attrs2 = t2.attrs;
        return (attrs1.Count == attrs2.Count)
            && Enumerable.All(attrs1, attr => attrs2.ContainsKey(attr.Key));
    }

    internal Boolean Tuple__Has_Heading(MDL_Tuple t, MDL_Heading h)
    {
        Dictionary<String, MDL_Any> attrs = t.attrs;
        HashSet<String> attr_names = h.attr_names;
        return (attrs.Count == attr_names.Count)
            && Enumerable.All(attr_names, attr_name => attrs.ContainsKey(attr_name));
    }

    internal MDL_Any MDL_Text_from_UTF_8_MDL_Blob(MDL_Blob value)
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
                return Simple_MDL_Excuse("X_Unicode_Blob_Not_UTF_8");
            }
            return this.MDL_Text(
                s,
                (tr == Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
            );
        }
        catch
        {
            return Simple_MDL_Excuse("X_Unicode_Blob_Not_UTF_8");
        }
    }
}
