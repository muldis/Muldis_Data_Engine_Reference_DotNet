using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Muldis.Data_Engine_Reference.Internal;

namespace Muldis.Data_Engine_Reference.Internal.Plain_Text;

// Muldis.Data_Engine_Reference.Internal.Plain_Text.Parser
// Provides common implementation code for all other *_Parser
// classes where they don't have reason to differ.
// Or it would if there was more than one *_Parser, which there isn't.

internal abstract class Parser
{
}

// TODO: Some of this text has obsolete notions and it should be rewritten.
// Muldis.Data_Engine_Reference.Internal.Plain_Text.Standard_Parser
// Provides utility pure functions that accept Muldis Data Language Plain Text (MDPT)
// source code, either a complete "parsing unit" that might comprise a
// foo.mdpt disk file or appropriate portions of such, usually input as
// a "Text" value, and derives by parsing it the actual Muldis Data Language "value"
// that the source code denotes; typically the result is a value of the
// "Package" type, which is the "native" form of Muldis Data Language source code
// and a "standard compilation unit".
// The input "Text" source code is expected to conform to the formal
// specification "Muldis_Object_Notation_Plain_Text 'https://muldis.com' '0.300.0'"
// and would typically either be hand-written by users or be generated
// by code such as the X or Y classes below.
// This class is completely deterministic and its exact output Muldis Data Language
// Package/etc values are determined entirely by its input Text/etc values.
// Standard_Parser only accepts input in the "foundational" flavor of
// Muldis_Object_Notation_Plain_Text, the same flavor that Standard_Generator outputs;
// it also expressly does not retain any
// parsing meta-data as new decorations on the output code, such as
// which exact numeric or string formats or whitespace was used.
// Similarly, Standard_Parser has zero configuration options.
// The more complicated reference implementation of a parser for the full
// Muldis_Object_Notation_Plain_Text grammar is in turn written in the "foundational"
// subset of Muldis Data Language, thus an executor for the full language is
// bootstrapped by an executor taking just the subset.
// The full reference parser can create decorations to support perfect
// round-tripping from plain text source to identical plain-text source.

internal class Standard_Parser : Parser
{
    internal MDL_Any MDPT_Parsing_Unit_MD_Text_to_MD_Any(MDL_Any parsing_unit)
    {
        // TODO: Everything.
        return parsing_unit.Memory.Well_Known_Excuses["No_Reason"];
    }
}

// Muldis.Data_Engine_Reference.Internal.Plain_Text.Generator
// Provides common implementation code for all other *_Generator
// classes where they don't have reason to differ.
// In fact, presently all inheriting classes actually have identical
// output, which is deterministic and valid Standard_Parser input;
// this is subject to change later.

internal abstract class Generator
{
    protected abstract String Any_Selector(MDL_Any value, String indent);

    protected String Any_Selector_Foundation_Dispatch(MDL_Any value, String indent)
    {
        switch (value.WKBT)
        {
            case Well_Known_Base_Type.MDL_Boolean:
                return Boolean_Literal(value);
            case Well_Known_Base_Type.MDL_Integer:
                return Integer_Literal(value);
            case Well_Known_Base_Type.MDL_Fraction:
                return Fraction_Literal(value);
            case Well_Known_Base_Type.MDL_Bits:
                return Bits_Literal(value);
            case Well_Known_Base_Type.MDL_Blob:
                return Blob_Literal(value);
            case Well_Known_Base_Type.MDL_Text:
                return Text_Literal(value);
            case Well_Known_Base_Type.MDL_Array:
                return Array_Selector(value, indent);
            case Well_Known_Base_Type.MDL_Set:
                return Set_Selector(value, indent);
            case Well_Known_Base_Type.MDL_Bag:
                return Bag_Selector(value, indent);
            case Well_Known_Base_Type.MDL_Tuple:
                return Tuple_Selector(value, indent);
            case Well_Known_Base_Type.MDL_Article:
                return Article_Selector(value, indent);
            case Well_Known_Base_Type.MDL_Variable:
                // We display something useful for debugging purposes, but no
                // (transient) MDL_Variable can actually be rendered as Muldis Data Language Plain Text.
                return "`Some IMD_Variable value is here.`";
            case Well_Known_Base_Type.MDL_Process:
                // We display something useful for debugging purposes, but no
                // (transient) MDL_Process can actually be rendered as Muldis Data Language Plain Text.
                return "`Some IMD_Process value is here.`";
            case Well_Known_Base_Type.MDL_Stream:
                // We display something useful for debugging purposes, but no
                // (transient) MDL_Stream can actually be rendered as Muldis Data Language Plain Text.
                return "`Some IMD_Stream value is here.`";
            case Well_Known_Base_Type.MDL_External:
                // We display something useful for debugging purposes, but no
                // (transient) MDL_External can actually be rendered as Muldis Data Language Plain Text.
                return "`Some IMD_External value is here.`";
            case Well_Known_Base_Type.MDL_Excuse:
                return Excuse_Selector(value, indent);
            default:
                return "DIE UN-HANDLED FOUNDATION TYPE"
                    + " [" + value.WKBT.ToString() + "]";
        }
    }

    private String Boolean_Literal(MDL_Any value)
    {
        return value.MDL_Boolean().Value ? "0bTRUE" : "0bFALSE";
    }

    private String Integer_Literal(MDL_Any value)
    {
        return value.MDL_Integer().ToString();
    }

    private String Integer_Literal(Int64 value)
    {
        return value.ToString();
    }

    private String Fraction_Literal(MDL_Any value)
    {
        MDL_Fraction_Struct fa = value.MDL_Fraction();
        // Iff the Fraction value can be exactly expressed as a
        // non-terminating decimal (includes all Fraction that can be
        // non-terminating binary/octal/hex), express in that format;
        // otherwise, express as a coprime numerator/denominator pair.
        // Note, Is_Terminating_Decimal() will ensure As_Pair is
        // coprime iff As_Decimal is null.
        if (fa.Is_Terminating_Decimal())
        {
            if (fa.As_Decimal is not null)
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

    private String Bits_Literal(MDL_Any value)
    {
        if (Object.ReferenceEquals(value, value.Memory.MDL_Bits_C0))
        {
            return "0bb";
        }
        System.Collections.IEnumerator e = value.MDL_Bits().GetEnumerator();
        List<Boolean> list = new List<Boolean>();
        while (e.MoveNext())
        {
            list.Add((Boolean)e.Current);
        }
        return "0bb" + String.Concat(
                Enumerable.Select(list, m => m ? "1" : "0")
            );
    }

    private String Blob_Literal(MDL_Any value)
    {
        if (Object.ReferenceEquals(value, value.Memory.MDL_Blob_C0))
        {
            return "0xx";
        }
        return "0xx" + String.Concat(
                Enumerable.Select(value.MDL_Blob(), m => m.ToString("X2"))
            );
    }

    private String Text_Literal(MDL_Any value)
    {
        if (Object.ReferenceEquals(value, value.Memory.MDL_Text_C0))
        {
            return "\"\"";
        }
        return Nonempty_Text_Literal(value.MDL_Text().Code_Point_Members);
    }

    private String Nonempty_Text_Literal(String value)
    {
        if (value.Length == 1 && ((Char)value[0]) <= 0x1F)
        {
            // Format as a code-point-text.
            return "0t" + ((Int32)(Char)value[0]);
        }
        // Else, format as a nonquoted-alphanumeric-text.
        if (Regex.IsMatch(value, @"\A[A-Za-z_][A-Za-z_0-9]*\z"))
        {
            return value;
        }
        // Else, format as a quoted text.
        return "\"" + Quoted_Text_Segment_Content(value) + "\"";
    }

    private String Quoted_Text_Segment_Content(String value)
    {
        if (value == "" || !Regex.IsMatch(value,
            "[\u0000-\u001F\"\\`\u0080-\u009F]"))
        {
            return value;
        }
        StringBuilder sb = new StringBuilder(value.Length);
        for (Int32 i = 0; i < value.Length; i++)
        {
            Char c = value[i];
            switch ((Int32)c)
            {
                case 0x7:
                    sb.Append(@"\a");
                    break;
                case 0x8:
                    sb.Append(@"\b");
                    break;
                case 0x9:
                    sb.Append(@"\t");
                    break;
                case 0xA:
                    sb.Append(@"\n");
                    break;
                case 0xB:
                    sb.Append(@"\v");
                    break;
                case 0xC:
                    sb.Append(@"\f");
                    break;
                case 0xD:
                    sb.Append(@"\r");
                    break;
                case 0x1B:
                    sb.Append(@"\e");
                    break;
                case 0x22:
                    sb.Append(@"\q");
                    break;
                case 0x5C:
                    sb.Append(@"\k");
                    break;
                case 0x60:
                    sb.Append(@"\g");
                    break;
                default:
                    if (c <= 0x1F || (c >= 0x80 && c <= 0x9F))
                    {
                        sb.Append(@"\(0t" + c.ToString() + ")");
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
        if (value == "")
        {
            return "\"\"";
        }
        return Nonempty_Text_Literal(value);
    }

    private String Heading_Literal(MDL_Any value)
    {
        Memory m = value.Memory;
        Dictionary<String,MDL_Any> attrs = value.MDL_Tuple();
        return Object.ReferenceEquals(value, m.MDL_Tuple_D0) ? "(Heading:{})"
            : "(Heading:{"
                + String.Concat(Enumerable.Select(
                        Enumerable.OrderBy(attrs, a => a.Key),
                        a => Attr_Name(a.Key) + ","))
                + "})";
    }

    private String Array_Selector(MDL_Any value, String indent)
    {
        String mei = indent + "\u0009";
        Memory m = value.Memory;
        return Object.ReferenceEquals(value, m.MDL_Array_C0) ? "(Array:[])"
            : "(Array:[\u000A" + Array_Selector__node__tree(
                value.MDL_Array(), mei) + indent + "])";
    }

    private String Array_Selector__node__tree(MDL_Array_Struct node, String indent)
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

    private String Set_Selector(MDL_Any value, String indent)
    {
        String mei = indent + "\u0009";
        value.Memory.Bag__Collapse(bag: value, want_indexed: true);
        MDL_Bag_Struct node = value.MDL_Bag();
        switch (node.Local_Symbolic_Type)
        {
            case Symbolic_Bag_Type.None:
                return "(Set:[])";
            case Symbolic_Bag_Type.Indexed:
                return "(Set:[\u000A" + String.Concat(Enumerable.Select(
                    Enumerable.OrderBy(node.Local_Indexed_Members(), m => m.Key),
                    m => mei + Any_Selector(m.Key, mei) + ",\u000A"
                )) + indent + "])";
            default:
                throw new NotImplementedException();
        }
    }

    private String Bag_Selector(MDL_Any value, String indent)
    {
        String mei = indent + "\u0009";
        value.Memory.Bag__Collapse(bag: value, want_indexed: true);
        MDL_Bag_Struct node = value.MDL_Bag();
        switch (node.Local_Symbolic_Type)
        {
            case Symbolic_Bag_Type.None:
                return "(Bag:[])";
            case Symbolic_Bag_Type.Indexed:
                return "(Bag:[\u000A" + String.Concat(Enumerable.Select(
                    Enumerable.OrderBy(node.Local_Indexed_Members(), m => m.Key),
                    m => mei + Any_Selector(m.Key, mei) + " : "
                        + Integer_Literal(m.Value.Multiplicity) + ",\u000A"
                )) + indent + "])";
            default:
                throw new NotImplementedException();
        }
    }

    private String Tuple_Selector(MDL_Any value, String indent)
    {
        if (value.Member_Status_in_WKT(MDL_Well_Known_Type.Heading) == true)
        {
            return Heading_Literal(value);
        }
        String ati = indent + "\u0009";
        Memory m = value.Memory;
        Dictionary<String,MDL_Any> attrs = value.MDL_Tuple();
        return Object.ReferenceEquals(value, m.MDL_Tuple_D0) ? "(Tuple:{})"
            : "(Tuple:{\u000A"
                + String.Concat(Enumerable.Select(
                        Enumerable.OrderBy(attrs, a => a.Key),
                        a => ati + Attr_Name(a.Key) + " : "
                            + Any_Selector(a.Value, ati) + ",\u000A"))
                + indent + "})";
    }

    private String Article_Selector(MDL_Any value, String indent)
    {
        // TODO: Change Article so represented as Nesting+Kit pair.
        return "(Article:("
            + Any_Selector(value.MDL_Article().Label, indent)
            + " : "
            + Any_Selector(value.MDL_Article().Attrs, indent)
            + "))";
    }

    private String Excuse_Selector(MDL_Any value, String indent)
    {
        // TODO: Change Excuse so represented as Nesting+Kit pair.
        String ati = indent + "\u0009";
        Memory m = value.Memory;
        Dictionary<String,MDL_Any> attrs = value.MDL_Excuse();
        if (attrs.Count == 0)
        {
            return "0iIGNORANCE";
        }
        if (attrs.Count == 1 && attrs.ContainsKey("\u0000")
            && attrs["\u0000"].WKBT == Well_Known_Base_Type.MDL_Tuple
            && attrs["\u0000"].Member_Status_in_WKT(MDL_Well_Known_Type.Attr_Name) == true)
        {
            return "(Excuse:(" + Attr_Name(attrs["\u0000"].MDL_Tuple().First().Key) + " : {}))";
        }
        return "(Excuse:(::\"\" : {\u000A"
            + String.Concat(Enumerable.Select(
                    Enumerable.OrderBy(attrs, a => a.Key),
                    a => ati + Attr_Name(a.Key) + " : "
                        + Any_Selector(a.Value, ati) + ",\u000A"))
            + indent + "}))";
    }
}

// TODO: Some of this text has obsolete notions and it should be rewritten.
// Muldis.Data_Engine_Reference.Internal.Plain_Text.Standard_Generator
// Provides utility pure functions that accept any Muldis Data Language "Package"
// value, which is a native Muldis Data Language "standard compilation unit", and
// derive a "Text" value that is this "Package" encoded in compliance
// with the "Muldis_Object_Notation_Plain_Text 'https://muldis.com' '0.300.0'"
// formal specification.  This outputs of this generator are intended
// for external use, whether for storage in foo.mdpt disk files or
// other places, viewing by users, and reading by other programs, as a
// means of interchange with any other system conforming to the spec.
// This generator actually handles any Muldis Data Language "value", not just a "Package".
// This class is completely deterministic and its exact output Muldis Data Language
// Text/etc values are determined entirely by its input Package/etc values.
// This generator is expressly kept as simple as possible such that it
// has zero configuration options and only does simple pretty-printing;
// it is only meant so that the Muldis_D_Foundation has a competent
// serialization capability built-in whose result is easy enough to
// read and that is easy to diff changes for.
// This generator makes only the "foundational" flavor of
// Muldis_Object_Notation_Plain_Text and provides a literal serialization, including
// that all annotation and decoration values are output verbatim, with
// no interpretation and with nothing left out.  Round-tripping will
// only work as intended with a parser that doesn't produce decorations.
// Typically it is up to bootstrapped higher-level libraries written in
// Muldis Data Language to provide other generating options that are better
// pretty-printed or provide any configuration or that serialize with
// non-foundational Muldis_Object_Notation_Plain_Text or interpret decorations.
// While this generator's output includes a "parsing unit predicate" on
// request (its optionality is the sole configuration option), that
// does not include any preceeding "shebang line", which is up to the
// caller where such is desired.

internal class Standard_Generator : Generator
{
    internal MDL_Any MDL_Any_to_MD_Text_MDPT_Parsing_Unit(MDL_Any value)
    {
        return value.Memory.MDL_Text(
            "(Muldis_Object_Notation_Syntax:([Plain_Text,"
            + " \"https://muldis.com\", \"0.300.0\"]:\u000A"
            + Any_Selector(value, "") + "))\u000A",
            false
        );
    }

    protected override String Any_Selector(MDL_Any value, String indent)
    {
        return Any_Selector_Foundation_Dispatch(value, indent);
    }
}

// Muldis.Data_Engine_Reference.Internal.Plain_Text.Identity_Generator
// Provides utility pure functions that accept any Muldis Data Language "value"
// and derive a .NET String that uniquely identifies it.
// This class is deterministic and guarantees that iff 2 MDL_Any are
// logically considered to be the "same" Muldis Data Language value then they will
// map to exactly the same .NET String value, and moreover, that iff 2
// MDL_Any are logically considered to NOT be the "same" Muldis Data Language value,
// they are guaranteed to map to distinct .NET String values.
// The outputs of this generator are intended for internal use only,
// where the outputs are transient and only intended to be used within
// the same in-memory Muldis.Data_Engine_Reference VM instance that generated them.
// The intended use of this class is to produce normalized identity
// values for .NET collection indexes, Dictionary keys for example, or
// otherwise support the means of primary/last resort for set-like
// operations like duplicate elimination, relational joins, and more.
// The outputs of this class may possibly conform to the
// Muldis_Object_Notation_Plain_Text specification and be parseable to yield the
// original input values, but that is not guaranteed; even if that is
// the case, the outputs might be considerably less "pretty" as a
// trade-off to make the generating faster and less error-prone.
// A normal side effect of using Identity_Generator on a MDL_Any/etc
// value is to update a cache therein to hold the serialization result.

internal class Identity_Generator : Generator
{
    internal String MDL_Any_to_Identity_String(MDL_Any value)
    {
        return Any_Selector(value, "");
    }

    protected override String Any_Selector(MDL_Any value, String indent)
    {
        if (value.Cached_MD_Any_Identity is null)
        {
            value.Cached_MD_Any_Identity
                = Any_Selector_Foundation_Dispatch(value, indent);
        }
        return value.Cached_MD_Any_Identity;
    }
}

// Muldis.Data_Engine_Reference.Internal.Plain_Text.Preview_Generator
// Provides utility pure functions that accept any Muldis Data Language "value"
// and derive a .NET String that provides a "preview quick look"
// serialization of that value.  The intended use of this class is to
// underlie a .NET ToString() override for all .NET values representing
// Muldis Data Language values so that debuggers including MS Visual Studio can
// assist programmers at easily determining what the current logical
// values of their program variables are, which would otherwise be
// quite labour intensive for humans inspeding MDL_Any object fields.
// This class is explicitly NOT guaranteed to be deterministic and so
// different runs of its functions on what are logically the same
// Muldis Data Language values might produce different String outputs; reasons for
// that include not sorting collection elements and truncating results.
// Given its purpose in aiding debugging, it is intended that this
// class will NOT populate any calculation caching fields in MDL_Any/etc
// objects, to help avoid heisenbugs.

internal class Preview_Generator : Generator
{
    internal String MDL_Any_To_Preview_String(MDL_Any value)
    {
        return Any_Selector(value, "");
    }

    protected override String Any_Selector(MDL_Any value, String indent)
    {
        return Any_Selector_Foundation_Dispatch(value, indent);
    }
}
