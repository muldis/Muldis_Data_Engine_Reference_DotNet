using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Muldis.D.Ref_Eng.Core;

namespace Muldis.D.Ref_Eng.Core.Plain_Text
{
    internal abstract class Parser
    {
    }

    // Muldis.D.Ref_Eng.Core.Plain_Text.Standard_Parser
    // Provides utility pure functions that accept Muldis D Plain Text (MDPT)
    // source code, either a complete "parsing unit" that might comprise a
    // foo.mdpt disk file or appropriate portions of such, usually input as
    // a "Text" value, and derives by parsing it the actual Muldis D "value"
    // that the source code denotes; typically the result is a value of the
    // "Package" type, which is the "native" form of Muldis D source code
    // and a "standard compilation unit".
    // The input "Text" source code is expected to conform to the formal
    // specification "Muldis_D_Plain_Text 'http://muldis.com' '0.201.0.-9'"
    // and would typically either be hand-written by users or be generated
    // by code such as the X or Y classes below.
    // This class is completely deterministic and its exact output Muldis D
    // Package/etc values are determined entirely by its input Text/etc values.

    internal class Standard_Parser : Parser
    {
        internal MD_Any MDPT_Parsing_Unit_MD_Text_to_MD_Any(
            MD_Any parsing_unit)
        {
            // TODO: Everything.
            return parsing_unit.AS.Memory.Well_Known_Excuses["No_Reason"];
        }
    }

    internal abstract class Generator
    {
        protected abstract String Any_Selector(MD_Any value);

        protected String Any_Selector_Foundation_Dispatch(MD_Any value)
        {
            switch (value.AS.MD_Foundation_Type)
            {
                case MD_Foundation_Type.MD_Boolean:
                    return Boolean_Literal(value);
                case MD_Foundation_Type.MD_Integer:
                    return Integer_Literal(value);
                case MD_Foundation_Type.MD_Array:
                    return Array_Selector(value);
                case MD_Foundation_Type.MD_Bag:
                    return Bag_Selector(value);
                case MD_Foundation_Type.MD_Tuple:
                    return Tuple_Selector(value);
                case MD_Foundation_Type.MD_Capsule:
                    if (value.AS.Cached_WKT.Contains(MD_Well_Known_Type.Fraction))
                    {
                        return Fraction_Literal(value);
                    }
                    if (value.AS.Cached_WKT.Contains(MD_Well_Known_Type.Bits))
                    {
                        return Bits_Literal(value);
                    }
                    if (value.AS.Cached_WKT.Contains(MD_Well_Known_Type.Blob))
                    {
                        return Blob_Literal(value);
                    }
                    if (value.AS.Cached_WKT.Contains(MD_Well_Known_Type.Text))
                    {
                        return Text_Literal(value);
                    }
                    if (value.AS.Cached_WKT.Contains(MD_Well_Known_Type.Set))
                    {
                        return Set_Selector(value);
                    }
                    return Capsule_Selector(value);
                case MD_Foundation_Type.MD_Handle:
                    // We display something useful for debugging purposes, but no
                    // (transient) MD_Handle can actually be rendered as Muldis D Plain Text.
                    switch (value.AS.MD_Handle.MD_Handle_Type)
                    {
                        case MD_Handle_Type.MD_Variable:
                            return "`Some IMD_Variable value is here.`";
                        case MD_Handle_Type.MD_Process:
                            return "`Some IMD_Process value is here.`";
                        case MD_Handle_Type.MD_Stream:
                            return "`Some IMD_Stream value is here.`";
                        case MD_Handle_Type.MD_External:
                            return "`Some IMD_External value is here.`";
                        default:
                            return "DIE UN-HANDLED HANDLE TYPE"
                                + " [" + value.AS.MD_Handle.MD_Handle_Type.ToString() + "]";
                    }
                default:
                    return "DIE UN-HANDLED FOUNDATION TYPE"
                        + " [" + value.AS.MD_Foundation_Type.ToString() + "]";
            }
        }

        private String Boolean_Literal(MD_Any value)
        {
            return value.AS.MD_Boolean ? "True" : "False";
        }

        private String Integer_Literal(MD_Any value)
        {
            // TODO: Standard will need ad-hoc customizability eg base 10 vs 16 vs etc.
            return value.AS.MD_Integer.ToString();
        }

        private String Integer_Literal(Int32 value)
        {
            // TODO: Standard will need ad-hoc customizability eg base 10 vs 16 vs etc.
            return value.ToString();
        }

        private String Fraction_Literal(MD_Any value)
        {
            // TODO: Standard will need ad-hoc customizability eg base 10 vs 16 vs etc.
            MD_Tuple_Struct ca = value.AS.MD_Capsule.Attrs.AS.MD_Tuple;
            return ca.Multi_OA["numerator"].AS.MD_Integer.ToString()
                + "/" + ca.Multi_OA["denominator"].AS.MD_Integer.ToString();
        }

        private String Bits_Literal(MD_Any value)
        {
            Memory m = value.AS.Memory;
            MD_Any array = value.AS.MD_Capsule.Attrs.AS.MD_Tuple.Only_OA.Value.Value;
            return Object.ReferenceEquals(array, m.MD_Array_C0) ? @"\~?''"
                : @"\~?'0b" + Bits_Literal__node__tree(array.AS.MD_Array) + "'";
        }

        private String Bits_Literal__node__tree(MD_Array_Node node)
        {
            String pred = (node.Pred_Members == null ? ""
                : Bits_Literal__node__tree(node.Pred_Members));
            String curr = Bits_Literal__node__local(node);
            String succ = (node.Succ_Members == null ? ""
                : Bits_Literal__node__tree(node.Succ_Members));
            return pred + curr + succ;
        }

        private String Bits_Literal__node__local(MD_Array_Node node)
        {
            switch (node.Local_Widest_Type)
            {
                case Widest_Component_Type.None:
                    return "";
                case Widest_Component_Type.Unrestricted:
                    throw new NotImplementedException();
                case Widest_Component_Type.Bit:
                    System.Collections.IEnumerator e
                        = node.Local_Bit_Members.GetEnumerator();
                    List<Boolean> list = new List<Boolean>();
                    while (e.MoveNext())
                    {
                        list.Add((Boolean)e.Current);
                    }
                    return String.Concat(Enumerable.Select(
                        list, m => m ? "1" : "0"));
                case Widest_Component_Type.Octet:
                    throw new NotImplementedException();
                case Widest_Component_Type.Codepoint:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        private String Blob_Literal(MD_Any value)
        {
            Memory m = value.AS.Memory;
            MD_Any array = value.AS.MD_Capsule.Attrs.AS.MD_Tuple.Only_OA.Value.Value;
            return Object.ReferenceEquals(array, m.MD_Array_C0) ? @"\~+''"
                : @"\~+'0x" + Blob_Literal__node__tree(array.AS.MD_Array) + "'";
        }

        private String Blob_Literal__node__tree(MD_Array_Node node)
        {
            String pred = (node.Pred_Members == null ? ""
                : Blob_Literal__node__tree(node.Pred_Members));
            String curr = Blob_Literal__node__local(node);
            String succ = (node.Succ_Members == null ? ""
                : Blob_Literal__node__tree(node.Succ_Members));
            return pred + curr + succ;
        }

        private String Blob_Literal__node__local(MD_Array_Node node)
        {
            switch (node.Local_Widest_Type)
            {
                case Widest_Component_Type.None:
                    return "";
                case Widest_Component_Type.Unrestricted:
                    throw new NotImplementedException();
                case Widest_Component_Type.Bit:
                    throw new NotImplementedException();
                case Widest_Component_Type.Octet:
                    return String.Concat(Enumerable.Select(
                        node.Local_Octet_Members,
                        m => m.ToString("X2")));
                case Widest_Component_Type.Codepoint:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }

        private String Text_Literal(MD_Any value)
        {
            Memory m = value.AS.Memory;
            MD_Any array = value.AS.MD_Capsule.Attrs.AS.MD_Tuple.Only_OA.Value.Value;
            return Object.ReferenceEquals(array, m.MD_Array_C0) ? "''"
                : "'" + Quoted_Name_Or_Text_Segment_Content(
                    Text_Literal__node__tree(array.AS.MD_Array)) + "'";
        }

        private String Text_Literal__node__tree(MD_Array_Node node)
        {
            String pred = (node.Pred_Members == null ? ""
                : Text_Literal__node__tree(node.Pred_Members));
            String curr = Text_Literal__node__local(node);
            String succ = (node.Succ_Members == null ? ""
                : Text_Literal__node__tree(node.Succ_Members));
            return pred + curr + succ;
        }

        private String Text_Literal__node__local(MD_Array_Node node)
        {
            switch (node.Local_Widest_Type)
            {
                case Widest_Component_Type.None:
                    return "";
                case Widest_Component_Type.Unrestricted:
                    throw new NotImplementedException();
                case Widest_Component_Type.Bit:
                    throw new NotImplementedException();
                case Widest_Component_Type.Octet:
                    throw new NotImplementedException();
                case Widest_Component_Type.Codepoint:
                    return node.Local_Codepoint_Members;
                default:
                    throw new NotImplementedException();
            }
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
                            sb.Append(@"\c<" + Integer_Literal((Int32)c) + ">");
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

        private String Heading_Literal(MD_Any value)
        {
            // TODO: Quote attribute names where appropriate.
            Memory m = value.AS.Memory;
            MD_Tuple_Struct ts = value.AS.MD_Tuple;
            if (ts.Degree == 1)
            {
                return Object.ReferenceEquals(value, m.Attr_Name_0) ? @"\0"
                     : Object.ReferenceEquals(value, m.Attr_Name_1) ? @"\1"
                     : Object.ReferenceEquals(value, m.Attr_Name_2) ? @"\2"
                     : @"\" + ts.Only_OA.Value.Key;
            }
            return Object.ReferenceEquals(value, m.MD_Tuple_D0) ? "()"
                : @"\@("
                    + (ts.A0 == null ? "" : "0,")
                    + (ts.A1 == null ? "" : "1,")
                    + (ts.A2 == null ? "" : "2,")
                    + (ts.Only_OA == null ? ""
                        : ts.Only_OA.Value.Key + ",")
                    + (ts.Multi_OA == null ? ""
                        : String.Concat(Enumerable.Select(
                            Enumerable.OrderBy(ts.Multi_OA, a => a.Key),
                            a => a.Key + ",")))
                    + ")";
        }

        private String Array_Selector(MD_Any value)
        {
            Memory m = value.AS.Memory;
            return Object.ReferenceEquals(value, m.MD_Array_C0) ? "[]"
                : "[" + Array_Selector__node__tree(value.AS.MD_Array) + "]";
        }

        private String Array_Selector__node__tree(MD_Array_Node node)
        {
            String pred = (node.Pred_Members == null ? ""
                : Array_Selector__node__tree(node.Pred_Members));
            String curr = Array_Selector__node__local(node);
            String succ = (node.Succ_Members == null ? ""
                : Array_Selector__node__tree(node.Succ_Members));
            return pred + (pred != "" && curr != "" ? ", " : "")
                + curr + (curr != "" && succ != "" ? ", " : "") + succ;
        }

        private String Array_Selector__node__local(MD_Array_Node node)
        {
            switch (node.Local_Widest_Type)
            {
                case Widest_Component_Type.None:
                    return "";
                case Widest_Component_Type.Unrestricted:
                    return String.Concat(Enumerable.Select(
                        node.Local_Unrestricted_Members,
                        m => Any_Selector(m) + ", "));
                case Widest_Component_Type.Bit:
                    System.Collections.IEnumerator e
                        = node.Local_Bit_Members.GetEnumerator();
                    List<Boolean> list = new List<Boolean>();
                    while (e.MoveNext())
                    {
                        list.Add((Boolean)e.Current);
                    }
                    return String.Concat(Enumerable.Select(
                        list, m => Integer_Literal(m ? 1 : 0) + ", "));
                case Widest_Component_Type.Octet:
                    return String.Concat(Enumerable.Select(
                        node.Local_Octet_Members,
                        m => Integer_Literal(m) + ", "));
                case Widest_Component_Type.Codepoint:
                    String s = node.Local_Codepoint_Members;
                    List<Int32> cpa = new List<Int32>(s.Length);
                    for (Int32 i = 0; i < s.Length; i++)
                    {
                        if ((i+1) < s.Length
                            && Char.IsSurrogatePair(s[i], s[i+1]))
                        {
                            cpa.Add(Char.ConvertToUtf32(s[i], s[i+1]));
                            i++;
                        }
                        else
                        {
                            cpa.Add(s[i]);
                        }
                    }
                    return String.Concat(Enumerable.Select(
                        cpa, m => Integer_Literal(m) + ", "));
                default:
                    throw new NotImplementedException();
            }
        }

        private String Set_Selector(MD_Any value)
        {
            MD_Tuple_Struct ca = value.AS.MD_Capsule.Attrs.AS.MD_Tuple;
            return "a not yet specified IMD_Set value";
        }

        private String Bag_Selector(MD_Any value)
        {
            return "a not yet specified IMD_Bag value";
        }

        private String Tuple_Selector(MD_Any value)
        {
            // TODO: Quote attribute names where appropriate.
            if (value.AS.Cached_WKT.Contains(MD_Well_Known_Type.Heading))
            {
                return Heading_Literal(value);
            }
            Memory m = value.AS.Memory;
            MD_Tuple_Struct ts = value.AS.MD_Tuple;
            return Object.ReferenceEquals(value, m.MD_Tuple_D0) ? "()"
                : "("
                    + (ts.A0 == null ? "" : "0 : " + Any_Selector(ts.A0) + ", ")
                    + (ts.A1 == null ? "" : "1 : " + Any_Selector(ts.A1) + ", ")
                    + (ts.A2 == null ? "" : "2 : " + Any_Selector(ts.A2) + ", ")
                    + (ts.Only_OA == null ? ""
                        : ts.Only_OA.Value.Key + " : "
                            + Any_Selector(ts.Only_OA.Value.Value) + ", ")
                    + (ts.Multi_OA == null ? ""
                        : String.Concat(Enumerable.Select(
                            Enumerable.OrderBy(ts.Multi_OA, a => a.Key),
                            a => a.Key + " : " + Any_Selector(a.Value) + ", ")))
                    + ")";
        }

        private String Capsule_Selector(MD_Any value)
        {
            // TODO: Dig for cases where we have a well-known subtype not marked as such.
            return "a not yet specified IMD_Capsule value";
        }
    }

    // Muldis.D.Ref_Eng.Core.Plain_Text.Standard_Generator
    // Provides utility pure functions that accept any Muldis D "Package"
    // value, which is a native Muldis D "standard compilation unit", and
    // derive a "Text" value that is this "Package" encoded in compliance
    // with the "Muldis_D_Plain_Text 'http://muldis.com' '0.201.0.-9'"
    // formal specification.  This outputs of this generator are intended
    // for external use, whether for storage in foo.mdpt disk files or
    // other places, viewing by users, and reading by other programs, as a
    // means of interchange with any other system conforming to the spec.
    // This generator will output all code annotations attached to the
    // regular semantic code and will also respect any relevant attached
    // code decorations where possible, or it otherwise has some
    // configuration options for pretty-printing the output code strings.
    // This class is completely deterministic and its exact output Muldis D
    // Text/etc values are determined entirely by its input Package/etc values.

    internal class Standard_Generator : Generator
    {
        internal MD_Any MD_Package_to_MDPT_MD_Text(MD_Any package)
        {
            // TODO: Various customizability.
            return package.AS.Memory.MD_Text(Any_Selector(package));
        }

        protected override String Any_Selector(MD_Any value)
        {
            return Any_Selector_Foundation_Dispatch(value);
        }
    }

    // Muldis.D.Ref_Eng.Core.Plain_Text.Identity_Generator
    // Provides utility pure functions that accept any Muldis D "value"
    // and derive a .Net String that uniquely identifies it.
    // This class is deterministic and guarantees that iff 2 MD_Any are
    // logically considered to be the "same" Muldis D value then they will
    // map to exactly the same .Net String value, and moreover, that iff 2
    // MD_Any are logically considered to NOT be the "same" Muldis D value,
    // they are guaranteed to map to distinct .Net String values.
    // The outputs of this generator are intended for internal use only,
    // where the outputs are transient and only intended to be used within
    // the same in-memory Muldis.D.Ref_Eng VM instance that generated them.
    // The intended use of this class is to produce normalized identity
    // values for .Net collection indexes, Dictionary keys for example, or
    // otherwise support the means of primary/last resort for set-like
    // operations like duplicate elimination, relational joins, and more.
    // The outputs of this class may possibly conform to the
    // Muldis_D_Plain_Text specification and be parseable to yield the
    // original input values, but that is not guaranteed; even if that is
    // the case, the outputs would be considerably less "pretty" as a
    // trade-off to make the generating faster and less error-prone.
    // A normal side effect of using Identity_Generator on a MD_Any/etc
    // value is to update a cache therein to hold the serialization result.

    internal class Identity_Generator : Generator
    {
        internal String MD_Any_to_Identity_String(MD_Any value)
        {
            return Any_Selector(value);
        }

        protected override String Any_Selector(MD_Any value)
        {
            if (value.AS.Cached_MD_Any_Identity == null)
            {
                value.AS.Cached_MD_Any_Identity
                    = Any_Selector_Foundation_Dispatch(value);
            }
            return value.AS.Cached_MD_Any_Identity;
        }
    }

    // Muldis.D.Ref_Eng.Core.Plain_Text.Preview_Generator
    // Provides utility pure functions that accept any Muldis D "value"
    // and derive a .Net String that provides a "preview quick look"
    // serialization of that value.  The intended use of this class is to
    // underlie a .Net ToString() override for all .Net values representing
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
            return Any_Selector(value);
        }

        protected override String Any_Selector(MD_Any value)
        {
            return Any_Selector_Foundation_Dispatch(value);
        }
    }
}
