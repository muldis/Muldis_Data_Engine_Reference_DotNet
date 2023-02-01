using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Muldis.Data_Engine_Reference.Core;

namespace Muldis.Data_Engine_Reference.Core.Plain_Text;

// Muldis.Data_Engine_Reference.Core.Plain_Text.Parser
// Provides common implementation code for all other *_Parser
// classes where they don't have reason to differ.
// Or it would if there was more than one *_Parser, which there isn't.

internal abstract class Parser
{
}

// Muldis.Data_Engine_Reference.Core.Plain_Text.Standard_Parser
// Provides utility pure functions that accept Muldis D Plain Text (MDPT)
// source code, either a complete "parsing unit" that might comprise a
// foo.mdpt disk file or appropriate portions of such, usually input as
// a "Text" value, and derives by parsing it the actual Muldis D "value"
// that the source code denotes; typically the result is a value of the
// "Package" type, which is the "native" form of Muldis D source code
// and a "standard compilation unit".
// The input "Text" source code is expected to conform to the formal
// specification "Muldis_D_Plain_Text 'https://muldis.com' '0.300.0'"
// and would typically either be hand-written by users or be generated
// by code such as the X or Y classes below.
// This class is completely deterministic and its exact output Muldis D
// Package/etc values are determined entirely by its input Text/etc values.
// Standard_Parser only accepts input in the "foundational" flavor of
// Muldis_D_Plain_Text, the same flavor that Standard_Generator outputs;
// it also expressly does not retain any
// parsing meta-data as new decorations on the output code, such as
// which exact numeric or string formats or whitespace was used.
// Similarly, Standard_Parser has zero configuration options.
// The more complicated reference implementation of a parser for the full
// Muldis_D_Plain_Text grammar is in turn written in the "foundational"
// subset of Muldis D, thus an executor for the full language is
// bootstrapped by an executor taking just the subset.
// The full reference parser can create decorations to support perfect
// round-tripping from plain text source to identical plain-text source.

internal class Standard_Parser : Parser
{
    internal MD_Any MDPT_Parsing_Unit_MD_Text_to_MD_Any(MD_Any parsing_unit)
    {
        // TODO: Everything.
        return parsing_unit.Memory.Well_Known_Excuses["No_Reason"];
    }
}

// Muldis.Data_Engine_Reference.Core.Plain_Text.Generator
// Provides common implementation code for all other *_Generator
// classes where they don't have reason to differ.
// In fact, presently all inheriting classes actually have identical
// output, which is deterministic and valid Standard_Parser input;
// this is subject to change later.

internal abstract class Generator
{
    protected abstract String Any_Selector(MD_Any value, String indent);

    protected String Any_Selector_Foundation_Dispatch(MD_Any value, String indent)
    {
        switch (value.MD_MSBT)
        {
            case MD_Well_Known_Base_Type.MD_Boolean:
                return Boolean_Literal(value);
            case MD_Well_Known_Base_Type.MD_Integer:
                return Integer_Literal(value);
            case MD_Well_Known_Base_Type.MD_Fraction:
                return Fraction_Literal(value);
            case MD_Well_Known_Base_Type.MD_Bits:
                return Bits_Literal(value);
            case MD_Well_Known_Base_Type.MD_Blob:
                return Blob_Literal(value);
            case MD_Well_Known_Base_Type.MD_Text:
                return Text_Literal(value);
            case MD_Well_Known_Base_Type.MD_Array:
                return Array_Selector(value, indent);
            case MD_Well_Known_Base_Type.MD_Set:
                return Set_Selector(value, indent);
            case MD_Well_Known_Base_Type.MD_Bag:
                return Bag_Selector(value, indent);
            case MD_Well_Known_Base_Type.MD_Tuple:
                return Tuple_Selector(value, indent);
            case MD_Well_Known_Base_Type.MD_Article:
                return Article_Selector(value, indent);
            case MD_Well_Known_Base_Type.MD_Variable:
                // We display something useful for debugging purposes, but no
                // (transient) MD_Variable can actually be rendered as Muldis D Plain Text.
                return "`Some IMD_Variable value is here.`";
            case MD_Well_Known_Base_Type.MD_Process:
                // We display something useful for debugging purposes, but no
                // (transient) MD_Process can actually be rendered as Muldis D Plain Text.
                return "`Some IMD_Process value is here.`";
            case MD_Well_Known_Base_Type.MD_Stream:
                // We display something useful for debugging purposes, but no
                // (transient) MD_Stream can actually be rendered as Muldis D Plain Text.
                return "`Some IMD_Stream value is here.`";
            case MD_Well_Known_Base_Type.MD_External:
                // We display something useful for debugging purposes, but no
                // (transient) MD_External can actually be rendered as Muldis D Plain Text.
                return "`Some IMD_External value is here.`";
            case MD_Well_Known_Base_Type.MD_Excuse:
                return Excuse_Selector(value, indent);
            default:
                return "DIE UN-HANDLED FOUNDATION TYPE"
                    + " [" + value.MD_MSBT.ToString() + "]";
        }
    }

    private String Boolean_Literal(MD_Any value)
    {
        return value.MD_Boolean().Value ? "True" : "False";
    }

    private String Integer_Literal(MD_Any value)
    {
        return value.MD_Integer().ToString();
    }

    private String Integer_Literal(Int64 value)
    {
        return value.ToString();
    }

    private String Fraction_Literal(MD_Any value)
    {
        MD_Fraction_Struct fa = value.MD_Fraction();
        // Iff the Fraction value can be exactly expressed as a
        // non-terminating decimal (includes all Fraction that can be
        // non-terminating binary/octal/hex), express in that format;
        // otherwise, express as a coprime numerator/denominator pair.
        // Note, Is_Terminating_Decimal() will ensure As_Pair is
        // coprime iff As_Decimal is null.
        if (fa.Is_Terminating_Decimal())
        {
            if (fa.As_Decimal != null)
            {
                String dec_digits = fa.As_Decimal.ToString();
                // When a .NET Decimal is selected using a .NET Decimal
                // literal having trailing zeroes, those trailing
                // zeroes will persist in the .ToString() result
                // (but any leading zeroes do not persist);
                // we need to detect and eliminate any of those,
                // except for a single trailing zero after the radix
                // point when the value is an integer.
                dec_digits = dec_digits.TrimEnd(new Char[] {'0'});
                return dec_digits.Substring(dec_digits.Length-1,1) == "."
                    ? dec_digits + "0" : dec_digits;
            }
            Int32 dec_scale = fa.Pair_Decimal_Denominator_Scale();
            if (dec_scale == 0)
            {
                // We have an integer expressed as a Fraction.
                return fa.As_Pair.Numerator.ToString() + ".0";
            }
            BigInteger dec_denominator = BigInteger.Pow(10,dec_scale);
            BigInteger dec_numerator = fa.As_Pair.Numerator
                * BigInteger.Divide(dec_denominator, fa.As_Pair.Denominator);
            String numerator_digits = dec_numerator.ToString();
            Int32 left_size = numerator_digits.Length - dec_scale;
            return numerator_digits.Substring(0, left_size)
                + "." + numerator_digits.Substring(left_size);
        }
        return fa.As_Pair.Numerator.ToString() + "/" + fa.As_Pair.Denominator.ToString();
    }

    private String Bits_Literal(MD_Any value)
    {
        if (Object.ReferenceEquals(value, value.Memory.MD_Bits_C0))
        {
            return @"\~?''";
        }
        System.Collections.IEnumerator e = value.MD_Bits().GetEnumerator();
        List<Boolean> list = new List<Boolean>();
        while (e.MoveNext())
        {
            list.Add((Boolean)e.Current);
        }
        return @"\~?'0b" + String.Concat(
                Enumerable.Select(list, m => m ? "1" : "0")
            ) + "'";
    }

    private String Blob_Literal(MD_Any value)
    {
        if (Object.ReferenceEquals(value, value.Memory.MD_Blob_C0))
        {
            return @"\~+''";
        }
        return @"\~+'0x" + String.Concat(
                Enumerable.Select(value.MD_Blob(), m => m.ToString("X2"))
            ) + "'";
    }

    private String Text_Literal(MD_Any value)
    {
        if (Object.ReferenceEquals(value, value.Memory.MD_Text_C0))
        {
            return "''";
        }
        return "'" + Quoted_Name_Or_Text_Segment_Content(
            value.MD_Text().Code_Point_Members) + "'";
    }

    private String Quoted_Name_Or_Text_Segment_Content(String value)
    {
        if (value == "" || !Regex.IsMatch(value,
            "[\"\'`\\\u0000-\u001F\u0080-\u009F]"))
        {
            return value;
        }
        StringBuilder sb = new StringBuilder(value.Length);
        sb.Append(@"\");
        for (Int32 i = 0; i < value.Length; i++)
        {
            Char c = value[i];
            switch ((Int32)c)
            {
                case 0x22:
                    sb.Append(@"\q");
                    break;
                case 0x27:
                    sb.Append(@"\a");
                    break;
                case 0x60:
                    sb.Append(@"\g");
                    break;
                case 0x5C:
                    sb.Append(@"\b");
                    break;
                case 0x9:
                    sb.Append(@"\t");
                    break;
                case 0xA:
                    sb.Append(@"\n");
                    break;
                case 0xD:
                    sb.Append(@"\r");
                    break;
                default:
                    if (c <= 0x1F || (c >= 0x80 && c <= 0x9F))
                    {
                        sb.Append(@"\c<" + Integer_Literal((Int64)c) + ">");
                    }
                    else
                    {
                        sb.Append(c);
                    }
                    break;
            }
        }
        return sb.ToString();
    }

    private String Attr_Name(String value)
    {
        if (value.Length == 1 && ((Char)value[0]) <= 0x1F)
        {
            // Format as an ordered attribute name, an Integer in 0..31.
            return Integer_Literal((Int64)(Char)value[0]);
        }
        // Else, format as a non-ordered attribute name.
        if (Regex.IsMatch(value, @"\A[A-Za-z_][A-Za-z_0-9]*\z"))
        {
            // Format as a bareword alphanumeric name.
            return value;
        }
        // Else, format as a quoted name.
        if (value == "")
        {
            return "\"\"";
        }
        return "\"" + Quoted_Name_Or_Text_Segment_Content(value) + "\"";
    }

    private String Heading_Literal(MD_Any value)
    {
        Memory m = value.Memory;
        Dictionary<String,MD_Any> attrs = value.MD_Tuple();
        if (attrs.Count == 1)
        {
            return @"\" + Attr_Name(attrs.First().Key);
        }
        return Object.ReferenceEquals(value, m.MD_Tuple_D0) ? "()"
            : @"\@("
                + String.Concat(Enumerable.Select(
                        Enumerable.OrderBy(attrs, a => a.Key),
                        a => Attr_Name(a.Key) + ","))
                + ")";
    }

    private String Array_Selector(MD_Any value, String indent)
    {
        String mei = indent + "\u0009";
        Memory m = value.Memory;
        return Object.ReferenceEquals(value, m.MD_Array_C0) ? "[]"
            : "[\u000A" + Array_Selector__node__tree(
                value.MD_Array(), mei) + indent + "]";
    }

    private String Array_Selector__node__tree(MD_Array_Struct node, String indent)
    {
        // Note: We always display consecutive duplicates in repeating
        // value format rather than value-count format in order to keep
        // everything simple and reliable in the 99% common case; we
        // assume that it would only be a rare edge case to have a
        // sparse array that we would want to output in that manner.
        // TODO: Rendering a Singular with a higher Multiplicity than
        // an Int32 can hold will throw a runtime exception; we can
        // deal with that later if an actual use case runs afowl of it.
        switch (node.Local_Symbolic_Type)
        {
            case Symbolic_Array_Type.None:
                return "";
            case Symbolic_Array_Type.Singular:
                return String.Concat(Enumerable.Repeat(
                    indent + Any_Selector(node.Local_Singular_Members().Member, indent) + ",\u000A",
                    (Int32)node.Local_Singular_Members().Multiplicity));
            case Symbolic_Array_Type.Arrayed:
                return String.Concat(Enumerable.Select(
                    node.Local_Arrayed_Members(),
                    m => indent + Any_Selector(m, indent) + ",\u000A"));
            case Symbolic_Array_Type.Catenated:
                return Array_Selector__node__tree(node.Tree_Catenated_Members().A0, indent)
                    + Array_Selector__node__tree(node.Tree_Catenated_Members().A1, indent);
            default:
                throw new NotImplementedException();
        }
    }

    private String Set_Selector(MD_Any value, String indent)
    {
        String mei = indent + "\u0009";
        value.Memory.Bag__Collapse(bag: value, want_indexed: true);
        MD_Bag_Struct node = value.MD_Bag();
        switch (node.Local_Symbolic_Type)
        {
            case Symbolic_Bag_Type.None:
                return "{}";
            case Symbolic_Bag_Type.Indexed:
                return "{\u000A" + String.Concat(Enumerable.Select(
                    Enumerable.OrderBy(node.Local_Indexed_Members(), m => m.Key),
                    m => mei + Any_Selector(m.Key, mei) + ",\u000A"
                )) + indent + "}";
            default:
                throw new NotImplementedException();
        }
    }

    private String Bag_Selector(MD_Any value, String indent)
    {
        String mei = indent + "\u0009";
        value.Memory.Bag__Collapse(bag: value, want_indexed: true);
        MD_Bag_Struct node = value.MD_Bag();
        switch (node.Local_Symbolic_Type)
        {
            case Symbolic_Bag_Type.None:
                return "{0:0}";
            case Symbolic_Bag_Type.Indexed:
                return "{\u000A" + String.Concat(Enumerable.Select(
                    Enumerable.OrderBy(node.Local_Indexed_Members(), m => m.Key),
                    m => mei + Any_Selector(m.Key, mei) + " : "
                        + Integer_Literal(m.Value.Multiplicity) + ",\u000A"
                )) + indent + "}";
            default:
                throw new NotImplementedException();
        }
    }

    private String Tuple_Selector(MD_Any value, String indent)
    {
        if (value.Member_Status_in_WKT(MD_Well_Known_Type.Heading) == true)
        {
            return Heading_Literal(value);
        }
        String ati = indent + "\u0009";
        Memory m = value.Memory;
        Dictionary<String,MD_Any> attrs = value.MD_Tuple();
        return Object.ReferenceEquals(value, m.MD_Tuple_D0) ? "()"
            : "(\u000A"
                + String.Concat(Enumerable.Select(
                        Enumerable.OrderBy(attrs, a => a.Key),
                        a => ati + Attr_Name(a.Key) + " : "
                            + Any_Selector(a.Value, ati) + ",\u000A"))
                + indent + ")";
    }

    private String Article_Selector(MD_Any value, String indent)
    {
        return "(" + Any_Selector(value.MD_Article().Label, indent) + " : "
            + Any_Selector(value.MD_Article().Attrs, indent) + ")";
    }

    private String Excuse_Selector(MD_Any value, String indent)
    {
        String ati = indent + "\u0009";
        Memory m = value.Memory;
        Dictionary<String,MD_Any> attrs = value.MD_Excuse();
        if (attrs.Count == 0)
        {
            return @"\!()";
        }
        if (attrs.Count == 1 && attrs.ContainsKey("\u0000")
            && attrs["\u0000"].MD_MSBT == MD_Well_Known_Base_Type.MD_Tuple
            && attrs["\u0000"].Member_Status_in_WKT(MD_Well_Known_Type.Attr_Name) == true)
        {
            return @"\!" + Attr_Name(attrs["\u0000"].MD_Tuple().First().Key);
        }
        return @"\!" + "(\u000A"
            + String.Concat(Enumerable.Select(
                    Enumerable.OrderBy(attrs, a => a.Key),
                    a => ati + Attr_Name(a.Key) + " : "
                        + Any_Selector(a.Value, ati) + ",\u000A"))
            + indent + ")";
    }
}

// Muldis.Data_Engine_Reference.Core.Plain_Text.Standard_Generator
// Provides utility pure functions that accept any Muldis D "Package"
// value, which is a native Muldis D "standard compilation unit", and
// derive a "Text" value that is this "Package" encoded in compliance
// with the "Muldis_D_Plain_Text 'https://muldis.com' '0.300.0'"
// formal specification.  This outputs of this generator are intended
// for external use, whether for storage in foo.mdpt disk files or
// other places, viewing by users, and reading by other programs, as a
// means of interchange with any other system conforming to the spec.
// This generator actually handles any Muldis D "value", not just a "Package".
// This class is completely deterministic and its exact output Muldis D
// Text/etc values are determined entirely by its input Package/etc values.
// This generator is expressly kept as simple as possible such that it
// has zero configuration options and only does simple pretty-printing;
// it is only meant so that the Muldis_D_Foundation has a competent
// serialization capability built-in whose result is easy enough to
// read and that is easy to diff changes for.
// This generator makes only the "foundational" flavor of
// Muldis_D_Plain_Text and provides a literal serialization, including
// that all annotation and decoration values are output verbatim, with
// no interpretation and with nothing left out.  Round-tripping will
// only work as intended with a parser that doesn't produce decorations.
// Typically it is up to bootstrapped higher-level libraries written in
// Muldis D to provide other generating options that are better
// pretty-printed or provide any configuration or that serialize with
// non-foundational Muldis_D_Plain_Text or interpret decorations.
// While this generator's output includes a "parsing unit predicate" on
// request (its optionality is the sole configuration option), that
// does not include any preceeding "shebang line", which is up to the
// caller where such is desired.

internal class Standard_Generator : Generator
{
    internal MD_Any MD_Any_to_MD_Text_MDPT_Parsing_Unit(MD_Any value)
    {
        return value.Memory.MD_Text(
            "Muldis_D Plain_Text 'https://muldis.com' '0.300.0'\u000A"
            + "meta foundational\u000A"
            + Any_Selector(value, "") + "\u000A",
            false
        );
    }

    internal MD_Any MD_Text_MDPT_Parsing_Unit_Predicate(Memory memory)
    {
        return memory.MD_Text(
            "Muldis_D Plain_Text 'https://muldis.com' '0.300.0'\u000A"
            + "meta foundational\u000A",
            false
        );
    }

    internal MD_Any MD_Any_to_MD_Text_MDPT_Parsing_Unit_Subject(MD_Any value)
    {
        String s = Any_Selector(value, "") + "\u000A";
        return value.Memory.MD_Text(
            s,
            (value.Memory.Test_Dot_Net_String(s)
                == Core.Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
        );
    }

    protected override String Any_Selector(MD_Any value, String indent)
    {
        return Any_Selector_Foundation_Dispatch(value, indent);
    }
}

// Muldis.Data_Engine_Reference.Core.Plain_Text.Identity_Generator
// Provides utility pure functions that accept any Muldis D "value"
// and derive a .NET String that uniquely identifies it.
// This class is deterministic and guarantees that iff 2 MD_Any are
// logically considered to be the "same" Muldis D value then they will
// map to exactly the same .NET String value, and moreover, that iff 2
// MD_Any are logically considered to NOT be the "same" Muldis D value,
// they are guaranteed to map to distinct .NET String values.
// The outputs of this generator are intended for internal use only,
// where the outputs are transient and only intended to be used within
// the same in-memory Muldis.Data_Engine_Reference VM instance that generated them.
// The intended use of this class is to produce normalized identity
// values for .NET collection indexes, Dictionary keys for example, or
// otherwise support the means of primary/last resort for set-like
// operations like duplicate elimination, relational joins, and more.
// The outputs of this class may possibly conform to the
// Muldis_D_Plain_Text specification and be parseable to yield the
// original input values, but that is not guaranteed; even if that is
// the case, the outputs might be considerably less "pretty" as a
// trade-off to make the generating faster and less error-prone.
// A normal side effect of using Identity_Generator on a MD_Any/etc
// value is to update a cache therein to hold the serialization result.

internal class Identity_Generator : Generator
{
    internal String MD_Any_to_Identity_String(MD_Any value)
    {
        return Any_Selector(value, "");
    }

    protected override String Any_Selector(MD_Any value, String indent)
    {
        if (value.Cached_MD_Any_Identity == null)
        {
            value.Cached_MD_Any_Identity
                = Any_Selector_Foundation_Dispatch(value, indent);
        }
        return value.Cached_MD_Any_Identity;
    }
}

// Muldis.Data_Engine_Reference.Core.Plain_Text.Preview_Generator
// Provides utility pure functions that accept any Muldis D "value"
// and derive a .NET String that provides a "preview quick look"
// serialization of that value.  The intended use of this class is to
// underlie a .NET ToString() override for all .NET values representing
// Muldis D values so that debuggers including MS Visual Studio can
// assist programmers at easily determining what the current logical
// values of their program variables are, which would otherwise be
// quite labour intensive for humans inspeding MD_Any object fields.
// This class is explicitly NOT guaranteed to be deterministic and so
// different runs of its functions on what are logically the same
// Muldis D values might produce different String outputs; reasons for
// that include not sorting collection elements and truncating results.
// Given its purpose in aiding debugging, it is intended that this
// class will NOT populate any calculation caching fields in MD_Any/etc
// objects, to help avoid heisenbugs.

internal class Preview_Generator : Generator
{
    internal String MD_Any_To_Preview_String(MD_Any value)
    {
        return Any_Selector(value, "");
    }

    protected override String Any_Selector(MD_Any value, String indent)
    {
        return Any_Selector_Foundation_Dispatch(value, indent);
    }
}
