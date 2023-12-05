using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Muldis.Data_Engine_Reference;

// Muldis.Data_Engine_Reference.Internal_MUON_Generator
// Provides common implementation code for all other *_Generator
// classes where they don't have reason to differ.

internal abstract class MUON_Generator
{
    protected abstract String as_Any_artifact(MDER_Any value, String indent);

    protected String Any_Selector_Foundation_Dispatch(MDER_Any value, String indent)
    {
        switch (value.WKBT)
        {
            case Well_Known_Base_Type.MDER_Ignorance:
                return "0iIGNORANCE";
            case Well_Known_Base_Type.MDER_False:
                return "0bFALSE";
            case Well_Known_Base_Type.MDER_True:
                return "0bTRUE";
            case Well_Known_Base_Type.MDER_Integer:
                return this.as_Integer_artifact((MDER_Integer)value);
            case Well_Known_Base_Type.MDER_Fraction:
                return this.as_Fraction_artifact((MDER_Fraction)value);
            case Well_Known_Base_Type.MDER_Bits:
                return this.as_Bits_artifact((MDER_Bits)value);
            case Well_Known_Base_Type.MDER_Blob:
                return this.as_Blob_artifact((MDER_Blob)value);
            case Well_Known_Base_Type.MDER_Text:
                return this.as_Text_artifact((MDER_Text)value);
            case Well_Known_Base_Type.MDER_Array:
                return this.as_Array_artifact((MDER_Array)value, indent);
            case Well_Known_Base_Type.MDER_Set:
                return this.as_Set_artifact((MDER_Set)value, indent);
            case Well_Known_Base_Type.MDER_Bag:
                return this.as_Bag_artifact((MDER_Bag)value, indent);
            case Well_Known_Base_Type.MDER_Heading:
                return this.as_Heading_artifact((MDER_Heading)value);
            case Well_Known_Base_Type.MDER_Tuple:
                return this.as_Tuple_artifact((MDER_Tuple)value, indent);
            case Well_Known_Base_Type.MDER_Tuple_Array:
                return this.as_Tuple_Array_artifact((MDER_Tuple_Array)value, indent);
            case Well_Known_Base_Type.MDER_Relation:
                return this.as_Relation_artifact((MDER_Relation)value, indent);
            case Well_Known_Base_Type.MDER_Tuple_Bag:
                return this.as_Tuple_Bag_artifact((MDER_Tuple_Bag)value, indent);
            case Well_Known_Base_Type.MDER_Article:
                return this.as_Article_artifact((MDER_Article)value, indent);
            case Well_Known_Base_Type.MDER_Excuse:
                return this.as_Excuse_artifact((MDER_Excuse)value, indent);
            case Well_Known_Base_Type.MDER_Variable:
                // We display something useful for debugging purposes, but no
                // (transient) MDER_Variable can actually be rendered as MUON.
                return "`Some MDER_Variable value is here.`";
            case Well_Known_Base_Type.MDER_Process:
                // We display something useful for debugging purposes, but no
                // (transient) MDER_Process can actually be rendered as MUON.
                return "`Some MDER_Process value is here.`";
            case Well_Known_Base_Type.MDER_Stream:
                // We display something useful for debugging purposes, but no
                // (transient) MDER_Stream can actually be rendered as MUON.
                return "`Some MDER_Stream value is here.`";
            case Well_Known_Base_Type.MDER_External:
                // We display something useful for debugging purposes, but no
                // (transient) MDER_External can actually be rendered as MUON.
                return "`Some MDER_External value is here.`";
            default:
                return "\\TODO_FIX_UN_HANDLED_TYPE\\\"" + value.WKBT.ToString() + "\"";
        }
    }

    private String as_Integer_artifact(MDER_Integer value)
    {
        return value.as_BigInteger.ToString();
    }

    private String as_Integer_artifact(Int64 value)
    {
        return value.ToString();
    }

    private String as_Fraction_artifact(MDER_Fraction fa)
    {
        // Iff the Fraction value can be exactly expressed as a
        // non-terminating decimal (includes all Fraction that can be
        // non-terminating binary/octal/hex), express in that format;
        // otherwise, express as a coprime numerator/denominator pair.
        // Note, is_terminating_decimal() will ensure as_pair is
        // coprime iff as_Decimal is null.
        if (fa.is_terminating_decimal())
        {
            if (fa.maybe_as_Decimal() is not null)
            {
                String dec_digits = fa.maybe_as_Decimal().ToString();
                // When a .NET Decimal is selected using a .NET Decimal
                // literal having trailing zeroes, those trailing
                // zeroes will persist in the .ToString() result
                // (but any leading zeroes do not persist);
                // we need to detect and eliminate any of those,
                // except for a single trailing zero after the radix
                // point when the value is an integer.
                dec_digits = dec_digits.TrimEnd(new Char[] {'0'});
                return String.Equals(dec_digits.Substring(dec_digits.Length-1,1), ".")
                    ? dec_digits + "0" : dec_digits;
            }
            Int32 dec_scale = fa.pair_decimal_denominator_scale();
            if (dec_scale == 0)
            {
                // We have an integer expressed as a Fraction.
                return fa.numerator().ToString() + ".0";
            }
            BigInteger dec_denominator = BigInteger.Pow(10,dec_scale);
            BigInteger dec_numerator = fa.numerator()
                * BigInteger.Divide(dec_denominator, fa.denominator());
            String numerator_digits = dec_numerator.ToString();
            Int32 left_size = numerator_digits.Length - dec_scale;
            return numerator_digits.Substring(0, left_size)
                + "." + numerator_digits.Substring(left_size);
        }
        return fa.numerator().ToString() + "/" + fa.denominator().ToString();
    }

    private String as_Bits_artifact(MDER_Bits value)
    {
        if (Object.ReferenceEquals(value, value.memory.MDER_Bits_C0))
        {
            return "0bb";
        }
        System.Collections.IEnumerator e = value.bit_members.GetEnumerator();
        List<Boolean> list = new List<Boolean>();
        while (e.MoveNext())
        {
            list.Add((Boolean)e.Current);
        }
        return "0bb" + String.Concat(
                Enumerable.Select(list, m => m ? "1" : "0")
            );
    }

    private String as_Blob_artifact(MDER_Blob value)
    {
        if (Object.ReferenceEquals(value, value.memory.MDER_Blob_C0))
        {
            return "0xx";
        }
        return "0xx" + String.Concat(
                Enumerable.Select(value.octet_members, m => m.ToString("X2"))
            );
    }

    private String as_Text_artifact(MDER_Text value)
    {
        return Text_Literal_from_String(value.code_point_members);
    }

    private String Text_Literal_from_String(String value)
    {
        if (String.Equals(value, ""))
        {
            return "\"\"";
        }
        if (value.Length == 1 && ((Int32)(Char)value[0]) <= 0x1F)
        {
            // Format as a code-point-text.
            return "0t" + ((Int32)(Char)value[0]);
        }
        if (Regex.IsMatch(value, @"\A[A-Za-z_][A-Za-z_0-9]*\z"))
        {
            // Format as a nonquoted-alphanumeric-text.
            return value;
        }
        // Else, format as a quoted text.
        return "\"" + quoted_text_segment_content(value) + "\"";
    }

    private String quoted_text_segment_content(String value)
    {
        if (String.Equals(value, "") || !Regex.IsMatch(value,
            "[\u0000-\u001F\"\\`\u007F-\u009F]"))
        {
            return value;
        }
        StringBuilder sb = new StringBuilder(value.Length);
        for (Int32 i = 0; i < value.Length; i++)
        {
            Int32 c = (Int32)(Char)value[i];
            switch (c)
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
                    if (c <= 0x1F || (c >= 0x7F && c <= 0x9F))
                    {
                        sb.Append(@"\(0t" + c.ToString() + ")");
                    }
                    else
                    {
                        sb.Append((Char)c);
                    }
                    break;
            }
        }
        return sb.ToString();
    }

    private String as_Heading_artifact(MDER_Heading value)
    {
        Memory m = value.memory;
        return Object.ReferenceEquals(value, m.MDER_Tuple_D0) ? "(Heading:{})"
            : "(Heading:{"
                + String.Concat(Enumerable.Select(
                        Enumerable.OrderBy(value.attr_names, a => a),
                        a => Text_Literal_from_String(a) + ","))
                + "})";
    }

    private String as_Array_artifact(MDER_Array value, String indent)
    {
        String mei = indent + "\u0009";
        Memory m = value.memory;
        return Object.ReferenceEquals(value, m.MDER_Array_C0) ? "(Array:[])"
            : "(Array:[\u000A" + Array_artifact__node__tree(
                value.tree_root_node, mei) + indent + "])";
    }

    private String Array_artifact__node__tree(MDER_Array_Struct node, String indent)
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
                    indent + as_Any_artifact(node.Local_Singular_Members().member, indent) + ",\u000A",
                    (Int32)node.Local_Singular_Members().multiplicity));
            case Symbolic_Array_Type.Arrayed:
                return String.Concat(Enumerable.Select(
                    node.Local_Arrayed_Members(),
                    m => indent + as_Any_artifact(m, indent) + ",\u000A"));
            case Symbolic_Array_Type.Catenated:
                return Array_artifact__node__tree(node.Tree_Catenated_Members().a0, indent)
                    + Array_artifact__node__tree(node.Tree_Catenated_Members().a1, indent);
            default:
                throw new NotImplementedException();
        }
    }

    private String as_Set_artifact(MDER_Set value, String indent)
    {
        String mei = indent + "\u0009";
        value.memory.Set__Collapse(set: value, want_indexed: true);
        MDER_Bag_Struct node = value.tree_root_node;
        switch (node.local_symbolic_type)
        {
            case Symbolic_Bag_Type.None:
                return "(Set:[])";
            case Symbolic_Bag_Type.Indexed:
                return "(Set:[\u000A" + String.Concat(Enumerable.Select(
                    Enumerable.OrderBy(node.Local_Indexed_Members(), m => m.Key),
                    m => mei + as_Any_artifact(m.Key, mei) + ",\u000A"
                )) + indent + "])";
            default:
                throw new NotImplementedException();
        }
    }

    private String as_Bag_artifact(MDER_Bag value, String indent)
    {
        String mei = indent + "\u0009";
        value.memory.Bag__Collapse(bag: value, want_indexed: true);
        MDER_Bag_Struct node = value.tree_root_node;
        switch (node.local_symbolic_type)
        {
            case Symbolic_Bag_Type.None:
                return "(Bag:[])";
            case Symbolic_Bag_Type.Indexed:
                return "(Bag:[\u000A" + String.Concat(Enumerable.Select(
                    Enumerable.OrderBy(node.Local_Indexed_Members(), m => m.Key),
                    m => mei + as_Any_artifact(m.Key, mei) + " : "
                        + as_Integer_artifact(m.Value.multiplicity) + ",\u000A"
                )) + indent + "])";
            default:
                throw new NotImplementedException();
        }
    }

    private String as_Tuple_artifact(MDER_Tuple value, String indent)
    {
        String ati = indent + "\u0009";
        Memory m = value.memory;
        return Object.ReferenceEquals(value, m.MDER_Tuple_D0) ? "(Tuple:{})"
            : "(Tuple:{\u000A"
                + String.Concat(Enumerable.Select(
                        Enumerable.OrderBy(value.attrs, a => a.Key),
                        a => ati + Text_Literal_from_String(a.Key) + " : "
                            + as_Any_artifact(a.Value, ati) + ",\u000A"))
                + indent + "})";
    }

    private String as_Tuple_Array_artifact(MDER_Tuple_Array value, String indent)
    {
        return "(Tuple_Array:("
            + as_Heading_artifact(value.heading)
            + " : "
            + as_Any_artifact(value.body, indent)
            + "))";
    }

    private String as_Relation_artifact(MDER_Relation value, String indent)
    {
        return "(Relation:("
            + as_Heading_artifact(value.heading)
            + " : "
            + as_Any_artifact(value.body, indent)
            + "))";
    }

    private String as_Tuple_Bag_artifact(MDER_Tuple_Bag value, String indent)
    {
        return "(Tuple_Bag:("
            + as_Heading_artifact(value.heading)
            + " : "
            + as_Any_artifact(value.body, indent)
            + "))";
    }

    private String as_Article_artifact(MDER_Article value, String indent)
    {
        // TODO: Change Article so represented as Nesting+Kit pair.
        return "(Article:("
            + as_Any_artifact(value.label, indent)
            + " : "
            + as_Any_artifact(value.attrs, indent)
            + "))";
    }

    private String as_Excuse_artifact(MDER_Excuse value, String indent)
    {
        // TODO: Change Excuse so represented as Nesting+Kit pair.
        return "(Excuse:("
            + as_Any_artifact(value.label, indent)
            + " : "
            + as_Any_artifact(value.attrs, indent)
            + "))";
    }
}
