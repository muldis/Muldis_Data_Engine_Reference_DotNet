using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Muldis.Data_Engine_Reference.Internal;

// Muldis.Data_Engine_Reference.Internal.MUON_Generator
// Provides common implementation code for all other *_Generator
// classes where they don't have reason to differ.

internal abstract class MUON_Generator
{
    protected abstract String Any_Selector(MDL_Any value, String indent);

    protected String Any_Selector_Foundation_Dispatch(MDL_Any value, String indent)
    {
        switch (value.WKBT)
        {
            case Well_Known_Base_Type.MDL_Boolean:
                return Boolean_Literal((MDL_Boolean)value);
            case Well_Known_Base_Type.MDL_Integer:
                return Integer_Literal((MDL_Integer)value);
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

    private String Boolean_Literal(MDL_Boolean value)
    {
        return value.as_Boolean.Value ? "0bTRUE" : "0bFALSE";
    }

    private String Integer_Literal(MDL_Integer value)
    {
        return value.as_BigInteger.ToString();
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
        // Note, Is_Terminating_Decimal() will ensure as_pair is
        // coprime iff as_Decimal is null.
        if (fa.Is_Terminating_Decimal())
        {
            if (fa.as_Decimal is not null)
            {
                String dec_digits = fa.as_Decimal.ToString();
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
                return fa.as_pair.numerator.ToString() + ".0";
            }
            BigInteger dec_denominator = BigInteger.Pow(10,dec_scale);
            BigInteger dec_numerator = fa.as_pair.numerator
                * BigInteger.Divide(dec_denominator, fa.as_pair.denominator);
            String numerator_digits = dec_numerator.ToString();
            Int32 left_size = numerator_digits.Length - dec_scale;
            return numerator_digits.Substring(0, left_size)
                + "." + numerator_digits.Substring(left_size);
        }
        return fa.as_pair.numerator.ToString() + "/" + fa.as_pair.denominator.ToString();
    }

    private String Bits_Literal(MDL_Any value)
    {
        if (Object.ReferenceEquals(value, value.memory.MDL_Bits_C0))
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
        if (Object.ReferenceEquals(value, value.memory.MDL_Blob_C0))
        {
            return "0xx";
        }
        return "0xx" + String.Concat(
                Enumerable.Select(value.MDL_Blob(), m => m.ToString("X2"))
            );
    }

    private String Text_Literal(MDL_Any value)
    {
        if (Object.ReferenceEquals(value, value.memory.MDL_Text_C0))
        {
            return "\"\"";
        }
        return Nonempty_Text_Literal(value.MDL_Text().code_point_members);
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
        Memory m = value.memory;
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
        Memory m = value.memory;
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
        // TODO: Rendering a Singular with a higher multiplicity than
        // an Int32 can hold will throw a runtime exception; we can
        // deal with that later if an actual use case runs afowl of it.
        switch (node.local_symbolic_type)
        {
            case Symbolic_Array_Type.None:
                return "";
            case Symbolic_Array_Type.Singular:
                return String.Concat(Enumerable.Repeat(
                    indent + Any_Selector(node.Local_Singular_Members().member, indent) + ",\u000A",
                    (Int32)node.Local_Singular_Members().multiplicity));
            case Symbolic_Array_Type.Arrayed:
                return String.Concat(Enumerable.Select(
                    node.Local_Arrayed_Members(),
                    m => indent + Any_Selector(m, indent) + ",\u000A"));
            case Symbolic_Array_Type.Catenated:
                return Array_Selector__node__tree(node.Tree_Catenated_Members().a0, indent)
                    + Array_Selector__node__tree(node.Tree_Catenated_Members().a1, indent);
            default:
                throw new NotImplementedException();
        }
    }

    private String Set_Selector(MDL_Any value, String indent)
    {
        String mei = indent + "\u0009";
        value.memory.Bag__Collapse(bag: value, want_indexed: true);
        MDL_Bag_Struct node = value.MDL_Bag();
        switch (node.local_symbolic_type)
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
        value.memory.Bag__Collapse(bag: value, want_indexed: true);
        MDL_Bag_Struct node = value.MDL_Bag();
        switch (node.local_symbolic_type)
        {
            case Symbolic_Bag_Type.None:
                return "(Bag:[])";
            case Symbolic_Bag_Type.Indexed:
                return "(Bag:[\u000A" + String.Concat(Enumerable.Select(
                    Enumerable.OrderBy(node.Local_Indexed_Members(), m => m.Key),
                    m => mei + Any_Selector(m.Key, mei) + " : "
                        + Integer_Literal(m.Value.multiplicity) + ",\u000A"
                )) + indent + "])";
            default:
                throw new NotImplementedException();
        }
    }

    private String Tuple_Selector(MDL_Any value, String indent)
    {
        if (value.member_status_in_WKT(Well_Known_Type.Heading) == true)
        {
            return Heading_Literal(value);
        }
        String ati = indent + "\u0009";
        Memory m = value.memory;
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
            + Any_Selector(value.MDL_Article().label, indent)
            + " : "
            + Any_Selector(value.MDL_Article().attrs, indent)
            + "))";
    }

    private String Excuse_Selector(MDL_Any value, String indent)
    {
        // TODO: Change Excuse so represented as Nesting+Kit pair.
        String ati = indent + "\u0009";
        Memory m = value.memory;
        Dictionary<String,MDL_Any> attrs = value.MDL_Excuse();
        if (attrs.Count == 0)
        {
            return "0iIGNORANCE";
        }
        if (attrs.Count == 1 && attrs.ContainsKey("\u0000")
            && attrs["\u0000"].WKBT == Well_Known_Base_Type.MDL_Tuple
            && attrs["\u0000"].member_status_in_WKT(Well_Known_Type.Attr_Name) == true)
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
