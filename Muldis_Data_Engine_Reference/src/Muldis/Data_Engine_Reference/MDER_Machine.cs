using System.Collections;
using System.Numerics;

namespace Muldis.Data_Engine_Reference;

public class MDER_Machine
{
    private readonly Internal_Executor __executor;

    // About this virtual machine memory pool where Muldis Data Language values
    // and variables live, which exploits the "flyweight pattern" for
    // efficiency in both performance and memory usage.
    // Conceptually, memory pool is a singleton, in that typically only one
    // would be used per virtual machine; in practice, only the set of all
    // VM values/vars/processes/etc that would interact must share one.
    // Virtual machine might have multiple pools, say, for separate handling of
    // entities that are short-lived versus longer-lived.
    // ? Note that .NET Core lacks System.Runtime.Caching or similar built-in
    // so for any situations we might have used such, we roll our own.
    // Some caches are logically just HashSets, but we need the ability to
    // fetch the actual cached objects which are the set members so we can
    // reuse them, not just know they exist, so Dictionaries are used instead.
    // Note this is called "interning" in a variety of programming languages;
    // see https://en.wikipedia.org/wiki/String_interning for more on that.

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

    public MDER_Machine()
    {
        this.__executor = new Internal_Executor(this);

        this.MDER_0iIGNORANCE = new MDER_Ignorance(this);

        this.MDER_0bFALSE = new MDER_False(this);
        this.MDER_0bTRUE = new MDER_True(this);

        this.integers = new Dictionary<Int32, MDER_Integer>();
        for (Int32 i = -1; i <= 1; i++)
        {
            MDER_Integer v = this.MDER_Integer(i);
        }

        this.MDER_Fraction_0 = new MDER_Fraction(this, 0.0M,
            this.integers[0].as_BigInteger(), this.integers[1].as_BigInteger());

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
            new Internal_MDER_Discrete_Struct {
                local_symbolic_type = Internal_Symbolic_Discrete_Type.None,
                cached_tree_member_count = 0,
                cached_tree_all_unique = true,
                cached_tree_relational = true,
            }
        );

        this.MDER_Set_C0 = new MDER_Set(this,
            new Internal_MDER_Discrete_Struct {
                local_symbolic_type = Internal_Symbolic_Discrete_Type.None,
                cached_tree_member_count = 0,
                cached_tree_all_unique = true,
                cached_tree_relational = true,
            }
        );

        this.MDER_Bag_C0 = new MDER_Bag(this,
            new Internal_MDER_Discrete_Struct {
                local_symbolic_type = Internal_Symbolic_Discrete_Type.None,
                cached_tree_member_count = 0,
                cached_tree_all_unique = true,
                cached_tree_relational = true,
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
                new Internal_MDER_Discrete_Struct {
                    local_symbolic_type = Internal_Symbolic_Discrete_Type.Singular,
                    members = new Internal_Multiplied_Member(MDER_Tuple_D0),
                    cached_tree_member_count = 1,
                    cached_tree_all_unique = true,
                    cached_tree_relational = true,
                }
            )
        );

        this.MDER_Relation_D0C0 = new MDER_Relation(this,
            MDER_Heading_D0, MDER_Set_C0);

        this.MDER_Relation_D0C1 = new MDER_Relation(this,
            MDER_Heading_D0,
            new MDER_Set(this,
                new Internal_MDER_Discrete_Struct {
                    local_symbolic_type = Internal_Symbolic_Discrete_Type.Singular,
                    members = new Internal_Multiplied_Member(MDER_Tuple_D0),
                    cached_tree_member_count = 1,
                    cached_tree_all_unique = true,
                    cached_tree_relational = true,
                }
            )
        );

        this.MDER_Tuple_Bag_D0C0 = new MDER_Tuple_Bag(this,
            MDER_Heading_D0, MDER_Bag_C0);

        this.MDER_Tuple_Bag_D0C1 = new MDER_Tuple_Bag(this,
            MDER_Heading_D0,
            new MDER_Bag(this,
                new Internal_MDER_Discrete_Struct {
                    local_symbolic_type = Internal_Symbolic_Discrete_Type.Singular,
                    members = new Internal_Multiplied_Member(MDER_Tuple_D0),
                    cached_tree_member_count = 1,
                    cached_tree_all_unique = true,
                    cached_tree_relational = true,
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

    internal Internal_Executor _executor()
    {
        return this.__executor;
    }


    internal MDER_Ignorance MDER_Ignorance()
    {
        return this.MDER_0iIGNORANCE;
    }

    internal MDER_Boolean MDER_Boolean(Boolean topic)
    {
        return topic ? MDER_0bTRUE : MDER_0bFALSE;
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
                new Internal_MDER_Discrete_Struct {
                    local_symbolic_type = Internal_Symbolic_Discrete_Type.Singular,
                    members = new Internal_Multiplied_Member(members[0]),
                    cached_tree_member_count = 1,
                    cached_tree_all_unique = true,
                    cached_tree_relational = (members[0]._WKBT()
                        == Internal_Well_Known_Base_Type.MDER_Tuple),
                }
            );
        }
        return new MDER_Array(this,
            new Internal_MDER_Discrete_Struct {
                local_symbolic_type = Internal_Symbolic_Discrete_Type.Arrayed,
                members = members,
            }
        );
    }

    internal MDER_Set MDER_Set(List<Internal_Multiplied_Member> members)
    {
        if (members.Count == 0)
        {
            return MDER_Set_C0;
        }
        return new MDER_Set(this,
            new Internal_MDER_Discrete_Struct {
                local_symbolic_type = Internal_Symbolic_Discrete_Type.Unique,
                members = new Internal_MDER_Discrete_Struct {
                    local_symbolic_type = Internal_Symbolic_Discrete_Type.Arrayed_MM,
                    members = members,
                },
                cached_tree_all_unique = true,
            }
        );
    }

    internal MDER_Bag MDER_Bag(List<Internal_Multiplied_Member> members)
    {
        if (members.Count == 0)
        {
            return MDER_Bag_C0;
        }
        return new MDER_Bag(this,
            new Internal_MDER_Discrete_Struct {
                local_symbolic_type = Internal_Symbolic_Discrete_Type.Arrayed_MM,
                members = members,
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
            if (body.Discrete__count() == 1)
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
            if (body.Discrete__maybe_Pick_Arbitrary_Member() is not null)
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

    internal MDER_Excuse Simple_MDER_Excuse(String topic)
    {
        if (well_known_excuses.ContainsKey(topic))
        {
            return well_known_excuses[topic];
        }
        return new MDER_Excuse(this, this.MDER_Attr_Name(topic), this.MDER_Tuple_D0);
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

    public MDER_Any MDER_evaluate(MDER_Any function, MDER_Any? args = null)
    {
        if (function is null)
        {
            throw new ArgumentNullException("function");
        }
        return this.__executor.Evaluates(function, args);
    }

    public void MDER_perform(MDER_Any procedure, MDER_Any? args = null)
    {
        if (procedure is null)
        {
            throw new ArgumentNullException("procedure");
        }
        this.__executor.Performs(procedure, args);
    }

    public MDER_Any MDER_current(MDER_Variable variable)
    {
        if (variable is null)
        {
            throw new ArgumentNullException("variable");
        }
        return variable.current_value;
    }

    public void MDER_assign(MDER_Variable variable, MDER_Any new_current)
    {
        if (variable is null)
        {
            throw new ArgumentNullException("variable");
        }
        if (new_current is null)
        {
            throw new ArgumentNullException("new_current");
        }
        variable.current_value = new_current;
    }

    public MDER_Any MDER_import(Object topic)
    {
        return import__tree(topic);
    }

    public MDER_Any MDER_import_qualified(KeyValuePair<String, Object?> topic)
    {
        return import__tree_qualified(topic);
    }

    private MDER_Any import__tree(Object? topic)
    {
        if (topic is MDER_Any)
        {
            return (MDER_Any)topic;
        }
        if (topic is not null && (topic.GetType().FullName ?? "").StartsWith("System.Collections.Generic.KeyValuePair`"))
        {
            return import__tree_qualified((KeyValuePair<String, Object?>)topic);
        }
        return import__tree_unqualified(topic);
    }

    private MDER_Any import__tree_qualified(KeyValuePair<String, Object?> topic)
    {
        Object? v = topic.Value;
        // Note that .NET guarantees the .Key is never null.
        if (v is null)
        {
            if (String.Equals(topic.Key, "Ignorance"))
            {
                return this.MDER_Ignorance();
            }
            throw new ArgumentNullException
            (
                paramName: "topic",
                message: "Can't select MDER_Any with a KeyValuePair operand"
                    + " with a null Value property (except with [Ignorance] Key)."
            );
        }
        switch (topic.Key)
        {
            case "Boolean":
                if (v is Boolean)
                {
                    return this.MDER_Boolean((Boolean)v);
                }
                break;
            case "Integer":
                if (v is Int32)
                {
                    return this.MDER_Integer((Int32)v);
                }
                if (v is BigInteger)
                {
                    return this.MDER_Integer((BigInteger)v);
                }
                break;
            case "Fraction":
                if (v is Decimal)
                {
                    return this.MDER_Fraction((Decimal)v);
                }
                if ((v.GetType().FullName ?? "").StartsWith("System.Collections.Generic.KeyValuePair`"))
                {
                    Object numerator   = ((KeyValuePair<Object, Object>)v).Key;
                    Object denominator = ((KeyValuePair<Object, Object>)v).Value;
                    // Note that .NET guarantees the .Key is never null.
                    if (denominator is null)
                    {
                        throw new ArgumentNullException
                        (
                            paramName: "topic",
                            message: "Can't select MDER_Fraction with a null denominator."
                        );
                    }
                    if (numerator is Int32 && denominator is Int32)
                    {
                        if (((Int32)denominator) == 0)
                        {
                            throw new ArgumentException
                            (
                                paramName: "topic",
                                message: "Can't select MDER_Fraction with a denominator of zero."
                            );
                        }
                        return this.MDER_Fraction((Int32)numerator, (Int32)denominator);
                    }
                    if (numerator is BigInteger && denominator is BigInteger)
                    {
                        if (((BigInteger)denominator) == 0)
                        {
                            throw new ArgumentException
                            (
                                paramName: "topic",
                                message: "Can't select MDER_Fraction with a denominator of zero."
                            );
                        }
                        return this.MDER_Fraction((BigInteger)numerator, (BigInteger)denominator);
                    }
                    if (numerator is MDER_Any
                        && ((MDER_Any)numerator)._WKBT() == Internal_Well_Known_Base_Type.MDER_Integer
                        && denominator is MDER_Any
                        && ((MDER_Any)denominator)._WKBT() == Internal_Well_Known_Base_Type.MDER_Integer)
                    {
                        if (((MDER_Integer)((MDER_Any)denominator)).as_BigInteger() == 0)
                        {
                            throw new ArgumentException
                            (
                                paramName: "topic",
                                message: "Can't select MDER_Fraction with a denominator of zero."
                            );
                        }
                        return this.MDER_Fraction(
                            ((MDER_Integer)((MDER_Any)numerator  )).as_BigInteger(),
                            ((MDER_Integer)((MDER_Any)denominator)).as_BigInteger()
                        );
                    }
                }
                break;
            case "Bits":
                if (v is BitArray)
                {
                    // BitArrays are mutable so clone argument to protect our internals.
                    return this.MDER_Bits(new BitArray((BitArray)v));
                }
                break;
            case "Blob":
                if (v is Byte[])
                {
                    // Arrays are mutable so clone argument to protect our internals.
                    return this.MDER_Blob(((Byte[])v).ToArray());
                }
                break;
            case "Text":
                if (v is String)
                {
                    Internal_Dot_Net_String_Unicode_Test_Result tr = this.__executor.Test_Dot_Net_String((String)v);
                    if (tr == Internal_Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                    {
                        throw new ArgumentException
                        (
                            paramName: "topic",
                            message: "Can't select MDER_Text with a malformed .NET String."
                        );
                    }
                    return this.MDER_Text(
                        (String)v,
                        (tr == Internal_Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
                    );
                }
                break;
            case "Array":
                if ((v.GetType().FullName ?? "").StartsWith("System.Collections.Generic.List`"))
                {
                    return this.MDER_Array(
                        new List<MDER_Any>(((List<Object>)v).Select(
                            m => import__tree(m)
                        ))
                    );
                }
                break;
            case "Set":
                if ((v.GetType().FullName ?? "").StartsWith("System.Collections.Generic.List`"))
                {
                    return this.MDER_Set(
                        new List<Internal_Multiplied_Member>(((List<Object>)v).Select(
                            m => new Internal_Multiplied_Member(import__tree(m))
                        ))
                    );
                }
                break;
            case "Bag":
                if ((v.GetType().FullName ?? "").StartsWith("System.Collections.Generic.List`"))
                {
                    return this.MDER_Bag(
                        new List<Internal_Multiplied_Member>(((List<Object>)v).Select(
                            m => new Internal_Multiplied_Member(import__tree(m))
                        ))
                    );
                }
                break;
            case "Heading":
                if (v is Object[])
                {
                    HashSet<String> attr_names = new HashSet<String>();
                    for (Int32 i = 0; i < ((Object[])v).Length; i++)
                    {
                        if (((Object[])v)[0] is not null && (Boolean)((Object[])v)[0])
                        {
                            attr_names.Add(Char.ConvertFromUtf32(i));
                        }
                    }
                    return this.MDER_Heading(attr_names);
                }
                if ((v.GetType().FullName ?? "").StartsWith("System.Collections.Generic.HashSet`"))
                {
                    HashSet<String> attr_names = (HashSet<String>)v;
                    foreach (String atnm in attr_names)
                    {
                        if (this.__executor.Test_Dot_Net_String(atnm)
                            == Internal_Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "topic",
                                message: "Can't select MDER_Heading with attribute"
                                    + " name that is a malformed .NET String."
                            );
                        }
                    }
                    return this.MDER_Heading(new HashSet<String>(attr_names));
                }
                break;
            case "Tuple":
                if (v is Object[])
                {
                    Dictionary<String, Object> attrs = new Dictionary<String, Object>();
                    for (Int32 i = 0; i < ((Object[])v).Length; i++)
                    {
                        attrs.Add(Char.ConvertFromUtf32(i), ((Object[])v)[i]);
                    }
                    return this.MDER_Tuple(
                        new Dictionary<String, MDER_Any>(attrs.ToDictionary(
                            a => a.Key, a => import__tree(a.Value)))
                    );
                }
                if ((v.GetType().FullName ?? "").StartsWith("System.Collections.Generic.Dictionary`"))
                {
                    Dictionary<String, Object> attrs = (Dictionary<String, Object>)v;
                    foreach (String atnm in attrs.Keys)
                    {
                        if (this.__executor.Test_Dot_Net_String(atnm)
                            == Internal_Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "topic",
                                message: "Can't select MDER_Tuple with attribute"
                                    + " name that is a malformed .NET String."
                            );
                        }
                    }
                    return this.MDER_Tuple(
                        new Dictionary<String, MDER_Any>(attrs.ToDictionary(
                            a => a.Key, a => import__tree(a.Value)))
                    );
                }
                break;
            case "Tuple_Array":
                if (v is MDER_Any && ((MDER_Any)v)._WKBT() == Internal_Well_Known_Base_Type.MDER_Heading)
                {
                    MDER_Heading hv = (MDER_Heading)((MDER_Any)v);
                    MDER_Array bv = this.MDER_Array_C0;
                    return this.MDER_Tuple_Array(hv, bv);
                }
                if (v is MDER_Any && ((MDER_Any)v)._WKBT() == Internal_Well_Known_Base_Type.MDER_Array)
                {
                    MDER_Array bv = (MDER_Array)((MDER_Any)v);
                    if (!bv.Discrete__Is_Relational())
                    {
                        throw new ArgumentException
                        (
                            paramName: "topic",
                            message: "Can't select MDER_Tuple_Array from a MDER_Array whose"
                                + " members aren't all MDER_Tuple with a common heading."
                        );
                    }
                    MDER_Heading? hv = this.__executor.Tuple__maybe_Heading((MDER_Tuple?)bv.Discrete__maybe_Pick_Arbitrary_Member());
                    if (hv is null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "topic",
                            message: "Can't select MDER_Tuple_Array from empty MDER_Array."
                        );
                    }
                    return this.MDER_Tuple_Array(hv, bv);
                }
                break;
            case "Relation":
                if (v is MDER_Any && ((MDER_Any)v)._WKBT() == Internal_Well_Known_Base_Type.MDER_Heading)
                {
                    MDER_Heading hv = (MDER_Heading)((MDER_Any)v);
                    MDER_Set bv = this.MDER_Set_C0;
                    return this.MDER_Relation(hv, bv);
                }
                if (v is MDER_Any && ((MDER_Any)v)._WKBT() == Internal_Well_Known_Base_Type.MDER_Set)
                {
                    MDER_Set bv = (MDER_Set)((MDER_Any)v);
                    if (!bv.Discrete__Is_Relational())
                    {
                        throw new ArgumentException
                        (
                            paramName: "topic",
                            message: "Can't select MDER_Relation from a MDER_Set whose"
                                + " members aren't all MDER_Tuple with a common heading."
                        );
                    }
                    MDER_Heading? hv = this.__executor.Tuple__maybe_Heading((MDER_Tuple?)bv.Discrete__maybe_Pick_Arbitrary_Member());
                    if (hv is null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "topic",
                            message: "Can't select MDER_Relation from empty MDER_Set."
                        );
                    }
                    return this.MDER_Relation(hv, bv);
                }
                break;
            case "Tuple_Bag":
                if (v is MDER_Any && ((MDER_Any)v)._WKBT() == Internal_Well_Known_Base_Type.MDER_Heading)
                {
                    MDER_Heading hv = (MDER_Heading)((MDER_Any)v);
                    MDER_Bag bv = this.MDER_Bag_C0;
                    return this.MDER_Tuple_Bag(hv, bv);
                }
                if (v is MDER_Any && ((MDER_Any)v)._WKBT() == Internal_Well_Known_Base_Type.MDER_Bag)
                {
                    MDER_Bag bv = (MDER_Bag)((MDER_Any)v);
                    if (!bv.Discrete__Is_Relational())
                    {
                        throw new ArgumentException
                        (
                            paramName: "topic",
                            message: "Can't select MDER_Tuple_Bag from a MDER_Bag whose"
                                + " members aren't all MDER_Tuple with a common heading."
                        );
                    }
                    MDER_Heading? hv = this.__executor.Tuple__maybe_Heading((MDER_Tuple?)bv.Discrete__maybe_Pick_Arbitrary_Member());
                    if (hv is null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "topic",
                            message: "Can't select MDER_Tuple_Bag from empty MDER_Bag."
                        );
                    }
                    return this.MDER_Tuple_Bag(hv, bv);
                }
                break;
            case "Article":
                if ((v.GetType().FullName ?? "").StartsWith("System.Collections.Generic.KeyValuePair`"))
                {
                    Object label = ((KeyValuePair<Object, Object>)v).Key;
                    Object attrs = ((KeyValuePair<Object, Object>)v).Value;
                    // Note that .NET guarantees the .Key is never null.
                    if (attrs is null)
                    {
                        throw new ArgumentNullException
                        (
                            paramName: "topic",
                            message: "Can't select MDER_Article with a null Article attrs."
                        );
                    }
                    MDER_Any attrs_cv = import__tree(attrs);
                    if (attrs_cv._WKBT() != Internal_Well_Known_Base_Type.MDER_Tuple)
                    {
                        throw new ArgumentException
                        (
                            paramName: "topic",
                            message: "Can't select MDER_Article with an Article attrs"
                                + " that doesn't evaluate as a MDER_Tuple."
                        );
                    }
                    MDER_Tuple attrs_cv_as_MDER_Tuple = (MDER_Tuple)attrs_cv;
                    if (label is String)
                    {
                        if (this.__executor.Test_Dot_Net_String((String)label)
                            == Internal_Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "topic",
                                message: "Can't select MDER_Article with label"
                                    + " that is a malformed .NET String."
                            );
                        }
                        return this.MDER_Article(
                            this.MDER_Attr_Name((String)label),
                            attrs_cv_as_MDER_Tuple
                        );
                    }
                    if (label is String[])
                    {
                        foreach (String s in ((String[])label))
                        {
                            if (this.__executor.Test_Dot_Net_String(s)
                                == Internal_Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                            {
                                throw new ArgumentException
                                (
                                    paramName: "topic",
                                    message: "Can't select MDER_Article with label"
                                        + " that includes a malformed .NET String."
                                );
                            }
                        }
                        return this.MDER_Article(
                            this.MDER_Array(new List<MDER_Any>(((String[])label).Select(
                                m => this.MDER_Attr_Name(m)
                            ))),
                            attrs_cv_as_MDER_Tuple
                        );
                    }
                    if (label is MDER_Any)
                    {
                        return this.MDER_Article(import__tree(label), attrs_cv_as_MDER_Tuple);
                    }
                }
                break;
            case "Excuse":
                if (v is String)
                {
                    if (this.__executor.Test_Dot_Net_String((String)v)
                        == Internal_Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                    {
                        throw new ArgumentException
                        (
                            paramName: "topic",
                            message: "Can't select MDER_Excuse with label"
                                + " that is a malformed .NET String."
                        );
                    }
                    return this.Simple_MDER_Excuse((String)v);
                }
                throw new NotImplementedException();
            case "New_Variable":
                if (v is MDER_Any)
                {
                    return this.New_MDER_Variable(
                        ((MDER_Any)v));
                }
                break;
            case "New_Process":
                if (v is null)
                {
                    return this.New_MDER_Process();
                }
                break;
            case "New_Stream":
                if (v is null)
                {
                    return this.New_MDER_Stream();
                }
                break;
            case "New_External":
                return this.New_MDER_External(v);
            case "Nesting":
                if (v is String[])
                {
                    foreach (String s in ((String[])v))
                    {
                        if (this.__executor.Test_Dot_Net_String(s)
                            == Internal_Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "topic",
                                message: "Can't select MDER_Attr_Name_List with"
                                    + " member that is a malformed .NET String."
                            );
                        }
                    }
                    return this.MDER_Array(new List<MDER_Any>(((String[])v).Select(
                        m => this.MDER_Attr_Name(m)
                    )));
                }
                break;
            default:
                throw new NotImplementedException(
                    "Unhandled MDER value type ["+topic.Key+"]+[" + (v.GetType().FullName ?? "(GetType.FullName() is null)") + "].");
        }
        // Duplicated as Visual Studio says otherwise not all code paths return a value.
        throw new NotImplementedException(
            "Unhandled MDER value type ["+topic.Key+"]+[" + (v.GetType().FullName ?? "(GetType.FullName() is null)") + "].");
    }

    private MDER_Any import__tree_unqualified(Object? topic)
    {
        if (topic is null)
        {
            return this.MDER_Ignorance();
        }
        if (topic is Boolean)
        {
            return this.MDER_Boolean((Boolean)topic);
        }
        if (topic is Int32)
        {
            return this.MDER_Integer((Int32)topic);
        }
        if (topic is BigInteger)
        {
            return this.MDER_Integer((BigInteger)topic);
        }
        if (topic is Decimal)
        {
            return this.MDER_Fraction((Decimal)topic);
        }
        if (topic is BitArray)
        {
            // BitArrays are mutable so clone argument to protect our internals.
            return this.MDER_Bits(new BitArray((BitArray)topic));
        }
        if (topic is Byte[])
        {
            // Arrays are mutable so clone argument to protect our internals.
            return this.MDER_Blob(((Byte[])topic).ToArray());
        }
        if (topic is String)
        {
            Internal_Dot_Net_String_Unicode_Test_Result tr = this.__executor.Test_Dot_Net_String((String)topic);
            if (tr == Internal_Dot_Net_String_Unicode_Test_Result.Is_Malformed)
            {
                throw new ArgumentException
                (
                    paramName: "topic",
                    message: "Can't select MDER_Text with a malformed .NET String."
                );
            }
            return this.MDER_Text(
                (String)topic,
                (tr == Internal_Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
            );
        }
        throw new NotImplementedException("Unhandled MDER value type [" + (topic.GetType().FullName ?? "(GetType.FullName() is null)") + "].");
    }

    public Object? MDER_export(MDER_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException("topic");
        }
        switch (topic._WKBT())
        {
            case Internal_Well_Known_Base_Type.MDER_Ignorance:
                return null;
            case Internal_Well_Known_Base_Type.MDER_False:
                return false;
            case Internal_Well_Known_Base_Type.MDER_True:
                return true;
            case Internal_Well_Known_Base_Type.MDER_Integer:
                return ((MDER_Integer)topic).as_BigInteger();
            case Internal_Well_Known_Base_Type.MDER_External:
                return ((MDER_External)topic).external_value;
            default:
                return MDER_export_qualified(topic);
        }
    }

    public KeyValuePair<String, Object?> MDER_export_qualified(MDER_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException("topic");
        }
        switch (topic._WKBT())
        {
            case Internal_Well_Known_Base_Type.MDER_Ignorance:
                return new KeyValuePair<String, Object?>("Ignorance", null);
            case Internal_Well_Known_Base_Type.MDER_False:
                return new KeyValuePair<String, Object?>("Boolean", false);
            case Internal_Well_Known_Base_Type.MDER_True:
                return new KeyValuePair<String, Object?>("Boolean", true);
            case Internal_Well_Known_Base_Type.MDER_Integer:
                return new KeyValuePair<String, Object?>("Integer",
                    ((MDER_Integer)topic).as_BigInteger());
            case Internal_Well_Known_Base_Type.MDER_Fraction:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Bits:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Blob:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Text:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Array:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Set:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Bag:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Heading:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Tuple:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Article:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Excuse:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Variable:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Process:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Stream:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_External:
                return new KeyValuePair<String, Object?>("New_External",
                    ((MDER_External)topic).external_value);
            default:
                throw new NotImplementedException();
        }
    }
}
