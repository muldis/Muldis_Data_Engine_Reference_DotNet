using System;
using System.Collections;
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
        // The set of values in here is a proper subset of MD_Tuple_D0.
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

        // MD_Capsule with False label and no attributes
        // (type default value but not actually useful in practice).
        private readonly MD_Any m_false_nullary_capsule;

        // All well known MD_Excuse values.
        internal readonly Dictionary<String,MD_Any> Well_Known_Excuses;

        internal Memory()
        {
            Identity_Generator = new Plain_Text.Identity_Generator();

            Preview_Generator = new Plain_Text.Preview_Generator();

            Executor = new Executor(this);

            MD_False = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Boolean,
                MD_Boolean = false,
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };

            MD_True = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Boolean,
                MD_Boolean = true,
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };

            m_integers = new Dictionary<Int32,MD_Any>();
            for (Int32 i = -1; i <= 1; i++)
            {
                MD_Any v = MD_Integer(i);
            }

            MD_Array_C0 = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Array,
                MD_Array = new MD_Array_Node {
                    Cached_Tree_Member_Count = 0,
                    Cached_Tree_All_Unique = true,
                    Cached_Tree_Relational = true,
                    Tree_Widest_Type = Widest_Component_Type.None,
                    Local_Multiplicity = 0,
                    Local_Widest_Type = Widest_Component_Type.None,
                    Cached_Local_Member_Count = 0,
                    Cached_Local_All_Unique = true,
                    Cached_Local_Relational = true,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };

            MD_Bag_C0 = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Bag,
                MD_Bag = new MD_Bag_Node {
                    Cached_Tree_Member_Count = 0,
                    Cached_Tree_All_Unique = true,
                    Cached_Tree_Relational = true,
                    Local_Symbolic_Type = Symbolic_Value_Type.None,
                    Cached_Local_Member_Count = 0,
                    Cached_Local_All_Unique = true,
                    Cached_Local_Relational = true,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };

            MD_Tuple_D0 = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                MD_Tuple = new MD_Tuple_Struct {
                    Degree = 0,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Heading},
            } };

            m_attr_name_tuples = new Dictionary<String,MD_Any>()
            {
                {"\u0000", new MD_Any { AS = new MD_Any_Struct {
                    Memory = this,
                    MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                    MD_Tuple = new MD_Tuple_Struct {
                        Degree = 1,
                        A0 = MD_True,
                    },
                    Cached_WKT = new HashSet<MD_Well_Known_Type>()
                        {MD_Well_Known_Type.Heading, MD_Well_Known_Type.Attr_Name},
                } } },
                {"\u0001", new MD_Any { AS = new MD_Any_Struct {
                    Memory = this,
                    MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                    MD_Tuple = new MD_Tuple_Struct {
                        Degree = 1,
                        A1 = MD_True,
                    },
                    Cached_WKT = new HashSet<MD_Well_Known_Type>()
                        {MD_Well_Known_Type.Heading, MD_Well_Known_Type.Attr_Name},
                } } },
                {"\u0002", new MD_Any { AS = new MD_Any_Struct {
                    Memory = this,
                    MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                    MD_Tuple = new MD_Tuple_Struct {
                        Degree = 1,
                        A2 = MD_True,
                    },
                    Cached_WKT = new HashSet<MD_Well_Known_Type>()
                        {MD_Well_Known_Type.Heading, MD_Well_Known_Type.Attr_Name},
                } } },
            };
            Attr_Name_0 = m_attr_name_tuples["\u0000"];
            Attr_Name_1 = m_attr_name_tuples["\u0001"];
            Attr_Name_2 = m_attr_name_tuples["\u0002"];

            Heading_0_1 = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                MD_Tuple = new MD_Tuple_Struct {
                    Degree = 2,
                    A0 = MD_True,
                    A1 = MD_True,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Heading},
            } };
            Heading_0_2 = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                MD_Tuple = new MD_Tuple_Struct {
                    Degree = 2,
                    A0 = MD_True,
                    A2 = MD_True,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Heading},
            } };
            Heading_1_2 = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                MD_Tuple = new MD_Tuple_Struct {
                    Degree = 2,
                    A1 = MD_True,
                    A2 = MD_True,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Heading},
            } };

            Heading_0_1_2 = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                MD_Tuple = new MD_Tuple_Struct {
                    Degree = 3,
                    A0 = MD_True,
                    A1 = MD_True,
                    A2 = MD_True,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Heading},
            } };

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

            m_false_nullary_capsule = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Capsule,
                MD_Capsule = new MD_Capsule_Struct {
                    Label = MD_False,
                    Attrs = MD_Tuple_D0,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };

            MD_Fraction_0 = MD_Capsule(
                MD_Attr_Name("Fraction"),
                MD_Tuple(
                    multi_oa: new Dictionary<String,MD_Any>()
                    {
                        {"numerator"  , MD_Integer(0)},
                        {"denominator", MD_Integer(1)},
                    }
                )
            );
            MD_Fraction_0.AS.Cached_WKT.Add(MD_Well_Known_Type.Fraction);

            MD_Bits_C0 = MD_Capsule(
                MD_Attr_Name("Bits"),
                MD_Tuple(
                    only_oa: new KeyValuePair<String,MD_Any>(
                        "bits", MD_Array_C0)
                )
            );
            MD_Bits_C0.AS.Cached_WKT.Add(MD_Well_Known_Type.Bits);

            MD_Blob_C0 = MD_Capsule(
                MD_Attr_Name("Blob"),
                MD_Tuple(
                    only_oa: new KeyValuePair<String,MD_Any>(
                        "octets", MD_Array_C0)
                )
            );
            MD_Blob_C0.AS.Cached_WKT.Add(MD_Well_Known_Type.Blob);

            MD_Text_C0 = MD_Capsule(
                MD_Attr_Name("Text"),
                MD_Tuple(
                    only_oa: new KeyValuePair<String,MD_Any>(
                        "unicode_codes", MD_Array_C0)
                )
            );
            MD_Text_C0.AS.Cached_WKT.Add(MD_Well_Known_Type.Text);

            MD_Set_C0 = MD_Capsule(
                MD_Attr_Name("Set"),
                MD_Tuple(
                    only_oa: new KeyValuePair<String,MD_Any>(
                        "members", MD_Bag_C0)
                )
            );
            MD_Set_C0.AS.Cached_WKT.Add(MD_Well_Known_Type.Set);

            MD_Tuple_Array_D0C0 = MD_Capsule(
                MD_Attr_Name("Tuple_Array"),
                MD_Tuple(
                    multi_oa: new Dictionary<String,MD_Any>()
                        {{"heading", MD_Tuple_D0}, {"body", MD_Array_C0}}
                )
            );
            MD_Tuple_Array_D0C0.AS.Cached_WKT.Add(MD_Well_Known_Type.Tuple_Array);

            MD_Tuple_Array_D0C1 = MD_Capsule(
                MD_Attr_Name("Tuple_Array"),
                MD_Tuple(
                    multi_oa: new Dictionary<String,MD_Any>()
                    {
                        {"heading", MD_Tuple_D0},
                        {"body", new MD_Any { AS = new MD_Any_Struct {
                            Memory = this,
                            MD_Foundation_Type = MD_Foundation_Type.MD_Array,
                            MD_Array = new MD_Array_Node {
                                Cached_Tree_Member_Count = 1,
                                Cached_Tree_All_Unique = true,
                                Cached_Tree_Relational = true,
                                Tree_Widest_Type
                                    = Widest_Component_Type.Unrestricted,
                                Local_Multiplicity = 1,
                                Local_Widest_Type
                                    = Widest_Component_Type.Unrestricted,
                                Cached_Local_Member_Count = 1,
                                Cached_Local_All_Unique = true,
                                Cached_Local_Relational = true,
                                Local_Unrestricted_Members
                                    = new List<MD_Any> {MD_Tuple_D0},
                            },
                            Cached_WKT = new HashSet<MD_Well_Known_Type>(),
                        } }},
                    }
                )
            );
            MD_Tuple_Array_D0C1.AS.Cached_WKT.Add(MD_Well_Known_Type.Tuple_Array);

            MD_Tuple_Bag_D0C0 = MD_Capsule(
                MD_Attr_Name("Tuple_Bag"),
                MD_Tuple(
                    multi_oa: new Dictionary<String,MD_Any>()
                        {{"heading", MD_Tuple_D0}, {"body", MD_Bag_C0}}
                )
            );
            MD_Tuple_Bag_D0C0.AS.Cached_WKT.Add(MD_Well_Known_Type.Tuple_Bag);

            MD_Tuple_Bag_D0C1 = MD_Capsule(
                MD_Attr_Name("Tuple_Bag"),
                MD_Tuple(
                    multi_oa: new Dictionary<String,MD_Any>()
                    {
                        {"heading", MD_Tuple_D0},
                        {"body", new MD_Any { AS = new MD_Any_Struct {
                            Memory = this,
                            MD_Foundation_Type = MD_Foundation_Type.MD_Bag,
                            MD_Bag = new MD_Bag_Node {
                                Cached_Tree_Member_Count = 1,
                                Cached_Tree_All_Unique = true,
                                Cached_Tree_Relational = true,
                                Local_Symbolic_Type
                                    = Symbolic_Value_Type.Singular,
                                Cached_Local_Member_Count = 1,
                                Cached_Local_All_Unique = true,
                                Cached_Local_Relational = true,
                                Local_Singular_Members
                                    = new Multiplied_Member(MD_Tuple_D0),
                            },
                            Cached_WKT = new HashSet<MD_Well_Known_Type>(),
                        } }},
                    }
                )
            );
            MD_Tuple_Bag_D0C1.AS.Cached_WKT.Add(MD_Well_Known_Type.Tuple_Bag);

            MD_Relation_D0C0 = MD_Capsule(
                MD_Attr_Name("Relation"),
                MD_Tuple(
                    multi_oa: new Dictionary<String,MD_Any>()
                        {{"heading", MD_Tuple_D0}, {"body", MD_Set_C0}}
                )
            );
            MD_Relation_D0C0.AS.Cached_WKT.Add(MD_Well_Known_Type.Relation);

            MD_Relation_D0C1 = MD_Capsule(
                MD_Attr_Name("Relation"),
                MD_Tuple(
                    multi_oa: new Dictionary<String,MD_Any>()
                    {
                        {"heading", MD_Tuple_D0},
                        {"body", MD_Set(MD_Tuple_Bag_D0C1.AS.MD_Capsule.Attrs
                            .AS.MD_Tuple.Multi_OA["body"])},
                    }
                )
            );
            MD_Relation_D0C1.AS.Cached_WKT.Add(MD_Well_Known_Type.Relation);

            Well_Known_Excuses = new Dictionary<String,MD_Any>();
            foreach (String s in Constants.Strings__Well_Known_Excuses())
            {
                Well_Known_Excuses.Add(s, MD_Excuse(MD_Tuple(MD_Attr_Name(s))));
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
            MD_Any integer = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Integer,
                MD_Integer = value,
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };
            if (may_cache && m_integers.Count < 10000)
            {
                m_integers.Add((Int32)value, integer);
            }
            return integer;
        }

        internal MD_Any MD_Fraction(BigInteger numerator, BigInteger denominator)
        {
            // Note we assume our caller has already ensured denominator is not zero.
            // Note that GreatestCommonDivisor() always has a non-negative result.
            BigInteger gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);
            if (gcd > 1)
            {
                // Make the numerator and denominator coprime.
                numerator   = (denominator > 0 ? numerator   : -numerator  ) / gcd;
                denominator = (denominator > 0 ? denominator : -denominator) / gcd;
            }
            if (numerator == 0 && denominator == 1)
            {
                return MD_Fraction_0;
            }
            MD_Any fraction = MD_Capsule(
                MD_Attr_Name("Fraction"),
                MD_Tuple(
                    multi_oa: new Dictionary<String,MD_Any>()
                    {
                        {"numerator"  , MD_Integer(numerator  )},
                        {"denominator", MD_Integer(denominator)},
                    }
                )
            );
            fraction.AS.Cached_WKT.Add(MD_Well_Known_Type.Fraction);
            return fraction;
        }

        internal MD_Any MD_Fraction(Decimal value)
        {
            Int32[] dec_bits = Decimal.GetBits(value);
            // https://msdn.microsoft.com/en-us/library/system.decimal.getbits(v=vs.110).aspx
            // The GetBits spec says that it returns 4 32-bit integers
            // representing the 128 bits of the Decimal itself; of these,
            // the first 3 integers' bits give the mantissa,
            // the 4th integer's bits give the sign and the scale factor.
            // "Bits 16 to 23 must contain an exponent between 0 and 28,
            // which indicates the power of 10 to divide the integer number."
            Int32 scale_factor_int = ((dec_bits[3] >> 16) & 0x7F);
            Decimal denominator_dec = 1M;
            // Decimal doesn't have exponentiation op so we do it manually.
            for (Int32 i = 1; i <= scale_factor_int; i++)
            {
                denominator_dec = denominator_dec * 10M;
            }
            Decimal numerator_dec = value * denominator_dec;
            return MD_Fraction(new BigInteger(numerator_dec),
                new BigInteger(denominator_dec));
        }

        internal MD_Any MD_Bits(BitArray members)
        {
            if (members.Length == 0)
            {
                return MD_Bits_C0;
            }
            MD_Any array = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Array,
                MD_Array = new MD_Array_Node {
                    Tree_Widest_Type = Widest_Component_Type.Bit,
                    Local_Multiplicity = 1,
                    Local_Widest_Type = Widest_Component_Type.Bit,
                    Local_Bit_Members = members,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };
            MD_Any bits = MD_Capsule(
                MD_Attr_Name("Bits"),
                MD_Tuple(
                    only_oa: new KeyValuePair<String,MD_Any>("bits", array)
                )
            );
            bits.AS.Cached_WKT.Add(MD_Well_Known_Type.Bits);
            return bits;
        }

        internal MD_Any MD_Blob(Byte[] members)
        {
            if (members.Length == 0)
            {
                return MD_Blob_C0;
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
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };
            MD_Any blob = MD_Capsule(
                MD_Attr_Name("Blob"),
                MD_Tuple(
                    only_oa: new KeyValuePair<String,MD_Any>("octets", array)
                )
            );
            blob.AS.Cached_WKT.Add(MD_Well_Known_Type.Blob);
            return blob;
        }

        internal MD_Any MD_Text(String members, Nullable<Boolean> has_any_non_BMP = null)
        {
            if (members == "")
            {
                return MD_Text_C0;
            }
            MD_Any array = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Array,
                MD_Array = new MD_Array_Node {
                    Tree_Widest_Type = Widest_Component_Type.Codepoint,
                    Local_Multiplicity = 1,
                    Local_Widest_Type = Widest_Component_Type.Codepoint,
                    Local_Codepoint_Members = members,
                    Cached_Local_Any_Non_BMP = has_any_non_BMP,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };
            MD_Any text = MD_Capsule(
                MD_Attr_Name("Text"),
                MD_Tuple(
                    only_oa: new KeyValuePair<String,MD_Any>(
                        "unicode_codes", array)
                )
            );
            text.AS.Cached_WKT.Add(MD_Well_Known_Type.Text);
            return text;
        }

        internal MD_Any MD_Array(List<MD_Any> members)
        {
            if (members.Count == 0)
            {
                return MD_Array_C0;
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
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };
            return array;
        }

        internal MD_Any MD_Set(MD_Any bag)
        {
            // We assume our caller has already made sure that the MD_Bag
            // we were given logically has no duplicate members, such as
            // because it was selected using with_unique=true.
            if (Object.ReferenceEquals(bag, MD_Bag_C0))
            {
                return MD_Set_C0;
            }
            MD_Any set = MD_Capsule(
                MD_Attr_Name("Set"),
                MD_Tuple(
                    only_oa: new KeyValuePair<String,MD_Any>("members", bag)
                )
            );
            set.AS.Cached_WKT.Add(MD_Well_Known_Type.Set);
            return set;
        }

        internal MD_Any MD_Bag(List<Multiplied_Member> members, Boolean with_unique = false)
        {
            if (members.Count == 0)
            {
                return MD_Bag_C0;
            }
            MD_Bag_Node arrayed_node = new MD_Bag_Node {
                Local_Symbolic_Type = Symbolic_Value_Type.Arrayed,
                Local_Arrayed_Members = members,
            };
            MD_Bag_Node root_node = arrayed_node;
            if (with_unique)
            {
                root_node = new MD_Bag_Node {
                    Cached_Tree_All_Unique = true,
                    Local_Symbolic_Type = Symbolic_Value_Type.Unique,
                    Primary_Arg = arrayed_node,
                };
            }
            MD_Any bag = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Bag,
                MD_Bag = root_node,
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };
            return bag;
        }

        internal MD_Any MD_Tuple(
            MD_Any a0 = null, MD_Any a1 = null, MD_Any a2 = null,
            Nullable<KeyValuePair<String,MD_Any>> only_oa = null,
            Dictionary<String,MD_Any> multi_oa = null)
        {
            Int32 degree = (a0 == null ? 0 : 1) + (a1 == null ? 0 : 1)
                + (a2 == null ? 0 : 1) + (only_oa == null ? 0 : 1)
                + (multi_oa == null ? 0 : multi_oa.Count);
            if (degree == 0)
            {
                return MD_Tuple_D0;
            }
            if (degree == 1)
            {
                if (a0 != null && Object.ReferenceEquals(a0, MD_True))
                {
                    return Attr_Name_0;
                }
                if (a1 != null && Object.ReferenceEquals(a1, MD_True))
                {
                    return Attr_Name_1;
                }
                if (a2 != null && Object.ReferenceEquals(a2, MD_True))
                {
                    return Attr_Name_2;
                }
                if (only_oa != null
                    && Object.ReferenceEquals(only_oa.Value.Value, MD_True)
                    && only_oa.Value.Key.Length <= 200
                    && m_attr_name_tuples.ContainsKey(only_oa.Value.Key))
                {
                    return m_attr_name_tuples[only_oa.Value.Key];
                }
            }
            MD_Any tuple = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                MD_Tuple = new MD_Tuple_Struct {
                    Degree = degree,
                    A0 = a0,
                    A1 = a1,
                    A2 = a2,
                    Only_OA = only_oa,
                    Multi_OA = multi_oa,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };
            if (degree == 1)
            {
                if (only_oa != null
                    && Object.ReferenceEquals(only_oa.Value.Value, MD_True))
                {
                    tuple.AS.Cached_WKT.Add(MD_Well_Known_Type.Heading);
                    tuple.AS.Cached_WKT.Add(MD_Well_Known_Type.Attr_Name);
                    if (only_oa.Value.Key.Length <= 200 && m_heading_tuples.Count < 10000)
                    {
                        m_heading_tuples.Add(tuple, tuple);
                        m_attr_name_tuples.Add(only_oa.Value.Key, tuple);
                    }
                }
                return tuple;
            }
            // We only get here if the tuple degree >= 2.
            if ((a0 == null || Object.ReferenceEquals(a0, MD_True))
                && (a1 == null || Object.ReferenceEquals(a1, MD_True))
                && (a2 == null || Object.ReferenceEquals(a2, MD_True))
                && (only_oa == null || Object.ReferenceEquals(
                    only_oa.Value.Value, MD_True))
                && (multi_oa == null || Enumerable.All(multi_oa,
                    attr => Object.ReferenceEquals(attr.Value, MD_True))))
            {
                tuple.AS.Cached_WKT.Add(MD_Well_Known_Type.Heading);
                if (degree <= 30
                    && (only_oa == null || only_oa.Value.Key.Length <= 200)
                    && (multi_oa == null || Enumerable.All(multi_oa,
                        attr => attr.Key.Length <= 200)))
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
            MD_Any tuple_array = MD_Capsule(
                MD_Attr_Name("Tuple_Array"),
                MD_Tuple(
                    multi_oa: new Dictionary<String,MD_Any>()
                        {{"heading", heading}, {"body", body}}
                )
            );
            tuple_array.AS.Cached_WKT.Add(MD_Well_Known_Type.Tuple_Array);
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
                if (Set__Pick_Random_Member(body) != null)
                {
                    return MD_Relation_D0C1;
                }
            }
            MD_Any relation = MD_Capsule(
                MD_Attr_Name("Relation"),
                MD_Tuple(
                    multi_oa: new Dictionary<String,MD_Any>()
                        {{"heading", heading}, {"body", body}}
                )
            );
            relation.AS.Cached_WKT.Add(MD_Well_Known_Type.Relation);
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
            MD_Any tuple_bag = MD_Capsule(
                MD_Attr_Name("Tuple_Bag"),
                MD_Tuple(
                    multi_oa: new Dictionary<String,MD_Any>()
                        {{"heading", heading}, {"body", body}}
                )
            );
            tuple_bag.AS.Cached_WKT.Add(MD_Well_Known_Type.Tuple_Bag);
            return tuple_bag;
        }

        internal MD_Any MD_Capsule(MD_Any label, MD_Any attrs)
        {
            if (Object.ReferenceEquals(label, MD_False)
                && Object.ReferenceEquals(attrs, MD_Tuple_D0))
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
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };
        }

        internal MD_Any New_MD_Variable(MD_Any initial_current_value)
        {
            return new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Handle,
                MD_Handle = new MD_Handle_Struct {
                    MD_Handle_Type = MD_Handle_Type.MD_Variable,
                    MD_Variable = new MD_Variable_Struct {
                        Current_Value = initial_current_value,
                    },
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };
        }

        internal MD_Any New_MD_Process()
        {
            return new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Handle,
                MD_Handle = new MD_Handle_Struct {
                    MD_Handle_Type = MD_Handle_Type.MD_Process,
                    MD_Process = new MD_Process_Struct {},
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };
        }

        internal MD_Any New_MD_Stream()
        {
            return new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Handle,
                MD_Handle = new MD_Handle_Struct {
                    MD_Handle_Type = MD_Handle_Type.MD_Stream,
                    MD_Stream = new MD_Stream_Struct {},
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };
        }

        internal MD_Any New_MD_External(Object value)
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
                Cached_WKT = new HashSet<MD_Well_Known_Type>(),
            } };
        }

        internal MD_Any MD_Excuse(MD_Any attrs)
        {
            return new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Capsule,
                MD_Capsule = new MD_Capsule_Struct {
                    Label = MD_Attr_Name("Excuse"),
                    Attrs = attrs,
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Excuse},
            } };
        }

        internal MD_Any Simple_MD_Excuse(String value)
        {
            if (Well_Known_Excuses.ContainsKey(value))
            {
                return Well_Known_Excuses[value];
            }
            return MD_Excuse(MD_Tuple(MD_Attr_Name(value)));
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
            MD_Any tuple = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                MD_Tuple = new MD_Tuple_Struct {
                    Degree = 1,
                    Only_OA = new KeyValuePair<String,MD_Any>(value, MD_True),
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Heading, MD_Well_Known_Type.Attr_Name},
            } };
            if (value.Length <= 200 && m_heading_tuples.Count < 10000)
            {
                m_heading_tuples.Add(tuple, tuple);
                m_attr_name_tuples.Add(value, tuple);
            }
            return tuple;
        }

        internal void Bag__Collapse(MD_Any bag, Boolean want_indexed = false)
        {
            bag.AS.MD_Bag = Bag__Collapsed_Node(bag.AS.MD_Bag, want_indexed);
        }

        private MD_Bag_Node Bag__Collapsed_Node(MD_Bag_Node node,
            Boolean want_indexed = false)
        {
            // Note: want_indexed only causes Indexed result when Arrayed
            // otherwise would be used; None/Singular still also used.
            switch (node.Local_Symbolic_Type)
            {
                case Symbolic_Value_Type.None:
                    // Node is already collapsed.
                    // In theory we should never get here assuming that any
                    // operations which would knowingly result in the empty
                    // Bag are optimized to return MD_Bag_C0 directly.
                    return MD_Bag_C0.AS.MD_Bag;
                case Symbolic_Value_Type.Singular:
                    // Node is already collapsed.
                    return node;
                case Symbolic_Value_Type.Arrayed:
                    if (!want_indexed)
                    {
                        // Node is already collapsed.
                        return node;
                    }
                    List<Multiplied_Member> ary_src_list = node.Local_Arrayed_Members;
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
                    return new MD_Bag_Node {
                        Cached_Tree_Member_Count = ary_src_list.Count,
                        Cached_Tree_All_Unique = node.Cached_Local_All_Unique,
                        Cached_Tree_Relational = node.Cached_Local_Relational,
                        Local_Symbolic_Type = Symbolic_Value_Type.Indexed,
                        Cached_Local_Member_Count = ary_src_list.Count,
                        Cached_Local_All_Unique = node.Cached_Local_All_Unique,
                        Cached_Local_Relational = node.Cached_Local_Relational,
                        Local_Indexed_Members = ary_res_dict,
                    };
                case Symbolic_Value_Type.Indexed:
                    // Node is already collapsed.
                    return node;
                case Symbolic_Value_Type.Unique:
                    MD_Bag_Node uni_pa = Bag__Collapsed_Node(
                        node: node.Primary_Arg, want_indexed: true);
                    if (uni_pa.Local_Symbolic_Type == Symbolic_Value_Type.None)
                    {
                        return uni_pa;
                    }
                    if (uni_pa.Local_Symbolic_Type == Symbolic_Value_Type.Singular)
                    {
                        return new MD_Bag_Node {
                            Cached_Tree_Member_Count = 1,
                            Cached_Tree_All_Unique = true,
                            Cached_Tree_Relational = uni_pa.Cached_Local_Relational,
                            Local_Symbolic_Type = Symbolic_Value_Type.Singular,
                            Cached_Local_Member_Count = 1,
                            Cached_Local_All_Unique = true,
                            Cached_Local_Relational = uni_pa.Cached_Local_Relational,
                            Local_Singular_Members = new Multiplied_Member(
                                uni_pa.Local_Singular_Members.Member),
                        };
                    }
                    Dictionary<MD_Any,Multiplied_Member> uni_src_dict
                        = uni_pa.Local_Indexed_Members;
                    return new MD_Bag_Node {
                        Cached_Tree_Member_Count = uni_src_dict.Count,
                        Cached_Tree_All_Unique = true,
                        Cached_Tree_Relational = uni_pa.Cached_Local_Relational,
                        Local_Symbolic_Type = Symbolic_Value_Type.Indexed,
                        Cached_Local_Member_Count = uni_src_dict.Count,
                        Cached_Local_All_Unique = true,
                        Cached_Local_Relational = uni_pa.Cached_Local_Relational,
                        Local_Indexed_Members = uni_src_dict.ToDictionary(
                            m => m.Key, m => new Multiplied_Member(m.Key, 1)),
                    };
                case Symbolic_Value_Type.Insert_N:
                    throw new NotImplementedException();
                case Symbolic_Value_Type.Remove_N:
                    throw new NotImplementedException();
                case Symbolic_Value_Type.Member_Plus:
                    throw new NotImplementedException();
                case Symbolic_Value_Type.Except:
                    throw new NotImplementedException();
                case Symbolic_Value_Type.Intersect:
                    throw new NotImplementedException();
                case Symbolic_Value_Type.Union:
                    throw new NotImplementedException();
                case Symbolic_Value_Type.Exclusive:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        internal MD_Any Array__Pick_Random_Member(MD_Any array)
        {
            return Array__Pick_Random_Node_Member(array.AS.MD_Array);
        }

        private MD_Any Array__Pick_Random_Node_Member(MD_Array_Node node)
        {
            if (node.Cached_Tree_Member_Count == 0)
            {
                return null;
            }
            switch (node.Local_Widest_Type)
            {
                case Widest_Component_Type.None:
                    return (node.Pred_Members == null ? null
                            : Array__Pick_Random_Node_Member(node.Pred_Members))
                        ?? (node.Succ_Members == null ? null
                            : Array__Pick_Random_Node_Member(node.Succ_Members));
                case Widest_Component_Type.Unrestricted:
                    return node.Local_Unrestricted_Members[0];
                case Widest_Component_Type.Bit:
                    throw new NotImplementedException();
                case Widest_Component_Type.Octet:
                    throw new NotImplementedException();
                case Widest_Component_Type.Codepoint:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        internal Boolean Array__Is_Relational(MD_Any array)
        {
            return Array__Tree_Relational(array.AS.MD_Array);
        }

        private Boolean Array__Tree_Relational(MD_Array_Node node)
        {
            if (node.Cached_Tree_Relational == null)
            {
                Boolean tr = Array__Local_Relational(node);
                MD_Any m0 = Array__Pick_Random_Node_Member(node);
                if (tr && node.Pred_Members != null)
                {
                    MD_Any pm0 = Array__Pick_Random_Node_Member(node.Pred_Members);
                    if (pm0 != null)
                    {
                        tr = tr && Array__Tree_Relational(node.Pred_Members);
                        if (tr && m0 != null)
                        {
                            tr = tr && Tuple__Same_Heading(pm0, m0);
                        }
                        m0 = pm0;
                    }
                }
                if (tr && node.Succ_Members != null)
                {
                    MD_Any sm0 = Array__Pick_Random_Node_Member(node.Succ_Members);
                    if (sm0 != null)
                    {
                        tr = tr && Array__Tree_Relational(node.Succ_Members);
                        if (tr && m0 != null)
                        {
                            tr = tr && Tuple__Same_Heading(sm0, m0);
                        }
                    }
                }
                node.Cached_Tree_Relational = tr;
            }
            return (Boolean)node.Cached_Tree_Relational;
        }

        private Boolean Array__Local_Relational(MD_Array_Node node)
        {
            if (node.Cached_Local_Relational == null)
            {
                switch (node.Local_Widest_Type)
                {
                    case Widest_Component_Type.None:
                        node.Cached_Local_Relational = true;
                        break;
                    case Widest_Component_Type.Unrestricted:
                        MD_Any m0 = Array__Pick_Random_Node_Member(node);
                        node.Cached_Local_Relational
                            = m0.AS.MD_Foundation_Type
                                == MD_Foundation_Type.MD_Tuple
                            && Enumerable.All(
                                node.Local_Unrestricted_Members,
                                m => m.AS.MD_Foundation_Type
                                        == MD_Foundation_Type.MD_Tuple
                                    && Tuple__Same_Heading(m, m0)
                            );
                        break;
                    case Widest_Component_Type.Bit:
                    case Widest_Component_Type.Octet:
                    case Widest_Component_Type.Codepoint:
                        node.Cached_Local_Relational = false;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return (Boolean)node.Cached_Local_Relational;
        }

        internal MD_Any Set__Pick_Random_Member(MD_Any set)
        {
            return Bag__Pick_Random_Member(
                set.AS.MD_Capsule.Attrs.AS.MD_Tuple.Only_OA.Value.Value);
        }

        internal Boolean Set__Is_Relational(MD_Any set)
        {
            return Bag__Is_Relational(
                set.AS.MD_Capsule.Attrs.AS.MD_Tuple.Only_OA.Value.Value);
        }

        internal MD_Any Bag__Pick_Random_Member(MD_Any bag)
        {
            return Bag__Pick_Random_Node_Member(bag.AS.MD_Bag);
        }

        private MD_Any Bag__Pick_Random_Node_Member(MD_Bag_Node node)
        {
            if (node.Cached_Tree_Member_Count == 0)
            {
                return null;
            }
            switch (node.Local_Symbolic_Type)
            {
                case Symbolic_Value_Type.None:
                    return null;
                case Symbolic_Value_Type.Singular:
                    return node.Local_Singular_Members.Member;
                case Symbolic_Value_Type.Arrayed:
                    return node.Local_Arrayed_Members.Count == 0 ? null
                        : node.Local_Arrayed_Members[0].Member;
                case Symbolic_Value_Type.Indexed:
                    throw new NotImplementedException();
                case Symbolic_Value_Type.Unique:
                    return Bag__Pick_Random_Node_Member(node.Primary_Arg);
                case Symbolic_Value_Type.Insert_N:
                    return node.Local_Singular_Members.Member;
                case Symbolic_Value_Type.Remove_N:
                    throw new NotImplementedException();
                case Symbolic_Value_Type.Member_Plus:
                    return Bag__Pick_Random_Node_Member(node.Primary_Arg)
                        ?? Bag__Pick_Random_Node_Member(node.Extra_Arg);
                case Symbolic_Value_Type.Except:
                    throw new NotImplementedException();
                case Symbolic_Value_Type.Intersect:
                    throw new NotImplementedException();
                case Symbolic_Value_Type.Union:
                    return Bag__Pick_Random_Node_Member(node.Primary_Arg)
                        ?? Bag__Pick_Random_Node_Member(node.Extra_Arg);
                case Symbolic_Value_Type.Exclusive:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        internal Boolean Bag__Is_Relational(MD_Any bag)
        {
            return Bag__Tree_Relational(bag.AS.MD_Bag);
        }

        private Boolean Bag__Tree_Relational(MD_Bag_Node node)
        {
            if (node.Cached_Tree_Relational == null)
            {
                Boolean tr = true;
                switch (node.Local_Symbolic_Type)
                {
                    case Symbolic_Value_Type.None:
                    case Symbolic_Value_Type.Singular:
                    case Symbolic_Value_Type.Arrayed:
                    case Symbolic_Value_Type.Indexed:
                        tr = Bag__Local_Relational(node);
                        break;
                    case Symbolic_Value_Type.Unique:
                        tr = Bag__Tree_Relational(node.Primary_Arg);
                        break;
                    case Symbolic_Value_Type.Insert_N:
                        tr = Bag__Local_Relational(node);
                        MD_Any lsm = node.Local_Singular_Members.Member;
                        MD_Any pm0 = Bag__Pick_Random_Node_Member(node.Primary_Arg);
                        if (pm0 != null)
                        {
                            tr = tr && Bag__Tree_Relational(node.Primary_Arg)
                                && Tuple__Same_Heading(pm0, lsm);
                        }
                        break;
                    case Symbolic_Value_Type.Member_Plus:
                    case Symbolic_Value_Type.Union:
                        tr = Bag__Tree_Relational(node.Primary_Arg)
                            && Bag__Tree_Relational(node.Extra_Arg);
                        MD_Any pam0 = Bag__Pick_Random_Node_Member(node.Primary_Arg);
                        MD_Any eam0 = Bag__Pick_Random_Node_Member(node.Extra_Arg);
                        if (pam0 != null && eam0 != null)
                        {
                            tr = tr && Tuple__Same_Heading(pam0, eam0);
                        }
                        break;
                    case Symbolic_Value_Type.Remove_N:
                    case Symbolic_Value_Type.Except:
                    case Symbolic_Value_Type.Intersect:
                    case Symbolic_Value_Type.Exclusive:
                        throw new NotImplementedException();
                    default:
                        throw new NotImplementedException();
                }
                node.Cached_Tree_Relational = tr;
            }
            return (Boolean)node.Cached_Tree_Relational;
        }

        private Boolean Bag__Local_Relational(MD_Bag_Node node)
        {
            if (node.Cached_Local_Relational == null)
            {
                switch (node.Local_Symbolic_Type)
                {
                    case Symbolic_Value_Type.None:
                    case Symbolic_Value_Type.Unique:
                    case Symbolic_Value_Type.Member_Plus:
                    case Symbolic_Value_Type.Except:
                    case Symbolic_Value_Type.Intersect:
                    case Symbolic_Value_Type.Union:
                    case Symbolic_Value_Type.Exclusive:
                        node.Cached_Local_Relational = true;
                        break;
                    case Symbolic_Value_Type.Singular:
                    case Symbolic_Value_Type.Insert_N:
                    case Symbolic_Value_Type.Remove_N:
                        node.Cached_Local_Relational
                            = node.Local_Singular_Members.Member.AS.MD_Foundation_Type
                                == MD_Foundation_Type.MD_Tuple;
                        break;
                    case Symbolic_Value_Type.Arrayed:
                        if (node.Local_Arrayed_Members.Count == 0)
                        {
                            node.Cached_Local_Relational = true;
                        }
                        else
                        {
                            MD_Any m0 = Bag__Pick_Random_Node_Member(node);
                            node.Cached_Local_Relational
                                = m0.AS.MD_Foundation_Type
                                    == MD_Foundation_Type.MD_Tuple
                                && Enumerable.All(
                                    node.Local_Arrayed_Members,
                                    m => m.Member.AS.MD_Foundation_Type
                                            == MD_Foundation_Type.MD_Tuple
                                        && Tuple__Same_Heading(m.Member, m0)
                                );
                        }
                        break;
                    case Symbolic_Value_Type.Indexed:
                        throw new NotImplementedException();
                    default:
                        throw new NotImplementedException();
                }
            }
            return (Boolean)node.Cached_Local_Relational;
        }

        internal MD_Any Tuple__Heading(MD_Any tuple)
        {
            if (tuple.AS.Cached_WKT.Contains(MD_Well_Known_Type.Heading))
            {
                return tuple;
            }
            // If we get here, the Tuple/Heading degree is guaranteed > 0.
            MD_Tuple_Struct ts = tuple.AS.MD_Tuple;
            if (ts.Degree == 1)
            {
                if (ts.A0 != null)
                {
                    return Attr_Name_0;
                }
                if (ts.A1 != null)
                {
                    return Attr_Name_1;
                }
                if (ts.A2 != null)
                {
                    return Attr_Name_2;
                }
                if (ts.Only_OA != null
                    && ts.Only_OA.Value.Key.Length <= 200
                    && m_attr_name_tuples.ContainsKey(ts.Only_OA.Value.Key))
                {
                    return m_attr_name_tuples[ts.Only_OA.Value.Key];
                }
            }
            MD_Any heading = new MD_Any { AS = new MD_Any_Struct {
                Memory = this,
                MD_Foundation_Type = MD_Foundation_Type.MD_Tuple,
                MD_Tuple = new MD_Tuple_Struct {
                    Degree = ts.Degree,
                    A0 = ts.A0 == null ? null : MD_True,
                    A1 = ts.A1 == null ? null : MD_True,
                    A2 = ts.A2 == null ? null : MD_True,
                    Only_OA = ts.Only_OA == null
                        ? (Nullable<KeyValuePair<String,MD_Any>>)null
                        : new KeyValuePair<String,MD_Any>(
                            ts.Only_OA.Value.Key, MD_True),
                    Multi_OA = ts.Multi_OA == null ? null
                        : new Dictionary<String,MD_Any>(
                            ts.Multi_OA.ToDictionary(a => a.Key, a => MD_True)),
                },
                Cached_WKT = new HashSet<MD_Well_Known_Type>()
                    {MD_Well_Known_Type.Heading},
            } };
            if (ts.Degree == 1)
            {
                if (ts.Only_OA != null)
                {
                    heading.AS.Cached_WKT.Add(MD_Well_Known_Type.Attr_Name);
                    if (ts.Only_OA.Value.Key.Length <= 200 && m_heading_tuples.Count < 10000)
                    {
                        m_heading_tuples.Add(heading, heading);
                        m_attr_name_tuples.Add(ts.Only_OA.Value.Key, heading);
                    }
                }
                return heading;
            }
            // We only get here if the tuple degree >= 2.
            if (ts.Degree <= 30
                && (ts.Only_OA == null || ts.Only_OA.Value.Key.Length <= 200)
                && (ts.Multi_OA == null || Enumerable.All(ts.Multi_OA,
                    attr => attr.Key.Length <= 200)))
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
            if (Object.ReferenceEquals(t1,t2)
                || Object.ReferenceEquals(t1.AS,t2.AS))
            {
                return true;
            }
            MD_Tuple_Struct ts1 = t1.AS.MD_Tuple;
            MD_Tuple_Struct ts2 = t2.AS.MD_Tuple;
            return (ts1.Degree == ts2.Degree)
                && ((ts1.A0 == null) == (ts2.A0 == null))
                && ((ts1.A1 == null) == (ts2.A1 == null))
                && ((ts1.A2 == null) == (ts2.A2 == null))
                && ((ts1.Only_OA == null && ts2.Only_OA == null)
                    || (ts1.Only_OA != null && ts2.Only_OA != null
                    && ts1.Only_OA.Value.Key == ts2.Only_OA.Value.Key))
                && ((ts1.Multi_OA == null && ts2.Multi_OA == null)
                    || (ts1.Multi_OA != null && ts2.Multi_OA != null
                    && Enumerable.All(ts1.Multi_OA,
                        attr => ts2.Multi_OA.ContainsKey(attr.Key))));
        }
    }
}
