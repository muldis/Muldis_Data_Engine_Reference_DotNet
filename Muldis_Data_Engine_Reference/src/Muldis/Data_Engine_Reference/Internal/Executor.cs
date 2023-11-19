using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Muldis.Data_Engine_Reference.Internal;

internal class Executor
{
    internal readonly Memory Memory;

    internal readonly Plain_Text.Standard_Generator Standard_Generator;

    internal Executor(Memory memory)
    {
        Memory = memory;

        Standard_Generator = new Plain_Text.Standard_Generator();
    }

    internal MDL_Any Evaluates(MDL_Any function, MDL_Any args = null)
    {
        Memory m = Memory;
        if (args is null)
        {
            args = m.MDL_Tuple_D0;
        }
        if (args.WKBT != Well_Known_Base_Type.MDL_Tuple)
        {
            throw new ArgumentException(m.Simple_MD_Excuse(
                "X_Args_Not_Tuple").ToString());
        }
        if (Is_Absolute_Name(function))
        {
            if (Object.ReferenceEquals(
                Array__maybe_at(function, 0), m.MDL_Attr_Name("foundation")))
            {
                return evaluate_foundation_function(
                    Array__maybe_at(function, 1), args);
            }
            throw new NotImplementedException();
        }
        if (Is_Function_Call(function))
        {
            throw new NotImplementedException();
        }
        if (Is_Function(function))
        {
            throw new NotImplementedException();
        }
        if (Is_Package(function))
        {
            throw new NotImplementedException();
        }
        throw new ArgumentException(m.Simple_MD_Excuse(
            "X_Malformed_Function_Call").ToString());
    }

    internal void Performs(MDL_Any procedure, MDL_Any args = null)
    {
        Memory m = Memory;
        if (args is null)
        {
            args = m.MDL_Tuple_D0;
        }
        if (args.WKBT != Well_Known_Base_Type.MDL_Tuple)
        {
            throw new ArgumentException(m.Simple_MD_Excuse("X_Args_Not_Tuple").ToString());
        }
        if (Is_Absolute_Name(procedure))
        {
            throw new NotImplementedException();
        }
        if (Is_Procedure_Call(procedure))
        {
            throw new NotImplementedException();
        }
        if (Is_Procedure(procedure))
        {
            throw new NotImplementedException();
        }
        if (Is_Package(procedure))
        {
            throw new NotImplementedException();
        }
        throw new ArgumentException(m.Simple_MD_Excuse(
            "X_Malformed_Procedure_Call").ToString());
    }

    internal Boolean Is_Excuse(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Fraction(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Bits(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Blob(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Text(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_String(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Set(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Tuple_Array(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Relation(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Tuple_Bag(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Interval(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Package(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Function(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Procedure(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Function_Call(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Procedure_Call(MDL_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Heading(MDL_Any value)
    {
        return value.Member_Status_in_WKT(MDL_Well_Known_Type.Heading) == true;
    }

    internal Boolean Is_Attr_Name(MDL_Any value)
    {
        return value.Member_Status_in_WKT(MDL_Well_Known_Type.Attr_Name) == true;
    }

    internal Boolean Is_Attr_Name_List(MDL_Any value)
    {
        if (value.Member_Status_in_WKT(MDL_Well_Known_Type.Attr_Name_List) == true)
        {
            return true;
        }
        Memory m = Memory;
        if (value.WKBT != Well_Known_Base_Type.MDL_Array)
        {
            return false;
        }
        // TODO: Replace this loop with an "All" higher order function call.
        Int64 count = Array__count(value);
        for (Int64 i = 0; i < count; i++)
        {
            if (!Is_Attr_Name(Array__maybe_at(value, i)))
            {
                return false;
            }
        }
        value.Declare_Member_Status_in_WKT(MDL_Well_Known_Type.Attr_Name_List, true);
        return true;
    }

    internal Boolean Is_Local_Name(MDL_Any value)
    {
        if (value.Member_Status_in_WKT(MDL_Well_Known_Type.Local_Name) == true)
        {
            return true;
        }
        if (!Is_Attr_Name_List(value))
        {
            return false;
        }
        Int64 count = Array__count(value);
        if (count == 0)
        {
            return false;
        }
        MDL_Any first = Array__maybe_at(value, 0);
        if ((Object.ReferenceEquals(first, Memory.MDL_Attr_Name("foundation")) && count == 2)
            || (Object.ReferenceEquals(first, Memory.MDL_Attr_Name("used")) && count >= 2)
            || (Object.ReferenceEquals(first, Memory.MDL_Attr_Name("package")) && count >= 1)
            || (Object.ReferenceEquals(first, Memory.MDL_Attr_Name("folder")) && count >= 1)
            || (Object.ReferenceEquals(first, Memory.MDL_Attr_Name("material")) && count == 1)
            || (Object.ReferenceEquals(first, Memory.MDL_Attr_Name("floating")) && count >= 2))
        {
            value.Declare_Member_Status_in_WKT(MDL_Well_Known_Type.Local_Name, true);
            return true;
        }
        return false;
    }

    internal Boolean Is_Absolute_Name(MDL_Any value)
    {
        if (value.Member_Status_in_WKT(MDL_Well_Known_Type.Absolute_Name) == true)
        {
            return true;
        }
        if (!Is_Local_Name(value))
        {
            return false;
        }
        MDL_Any first = Array__maybe_at(value, 0);
        if (Object.ReferenceEquals(first, Memory.MDL_Attr_Name("foundation"))
            || Object.ReferenceEquals(first, Memory.MDL_Attr_Name("used"))
            || Object.ReferenceEquals(first, Memory.MDL_Attr_Name("package")))
        {
            value.Declare_Member_Status_in_WKT(MDL_Well_Known_Type.Absolute_Name, true);
            return true;
        }
        return false;
    }

    internal MDL_Any evaluate_foundation_function(MDL_Any func_name, MDL_Any args)
    {
        Memory m = Memory;
        String func_name_s = func_name.MDL_Tuple().First().Key;

        // TYPE DEFINERS

        if (Constants.Strings__Foundation_Type_Definer_Function_Names().Contains(func_name_s))
        {
            if (!m.Tuple__Same_Heading(args, m.Attr_Name_0))
            {
                throw new ArgumentException(m.Simple_MD_Excuse(
                    "X_Type_Definer_Function_Args_Not_Heading_0").ToString());
            }
            MDL_Any v = args.MDL_Tuple()["\u0000"];
            switch (func_name_s)
            {
                case "Any":
                    return m.MDL_True;
                case "None":
                    return m.MDL_False;

                // FOUNDATION BASE TYPE DEFINERS

                case "Boolean":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_Boolean
                    );
                case "Integer":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_Integer
                    );
                case "Array":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_Array
                    );
                case "Bag":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_Bag
                    );
                case "Tuple":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_Tuple
                    );
                case "Article":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_Article
                    );
                case "Handle":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_Variable
                            || v.WKBT == Well_Known_Base_Type.MDL_Process
                            || v.WKBT == Well_Known_Base_Type.MDL_Stream
                            || v.WKBT == Well_Known_Base_Type.MDL_External
                    );

                // HANDLE SUBTYPE DEFINERS

                case "Variable":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_Variable
                    );
                case "Process":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_Process
                    );
                case "Stream":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_Stream
                    );
                case "External":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_External
                    );

                // ARRAY SUBTYPE DEFINERS

                // TUPLE SUBTYPE DEFINERS

                // ARTICLE SUBTYPE DEFINERS

                case "Blob":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_Blob
                    );

                case "Text":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_Text
                    );

                case "Excuse":
                    return m.MDL_Boolean(
                        v.WKBT == Well_Known_Base_Type.MDL_Excuse
                    );

                default:
                    throw new NotImplementedException();
            }
        }

        // NON TYPE DEFINER UNARY FUNCTIONS

        if (Constants.Strings__Foundation_NTD_Unary_Function_Names().Contains(func_name_s))
        {
            if (!m.Tuple__Same_Heading(args, m.Attr_Name_0))
            {
                throw new ArgumentException(m.Simple_MD_Excuse(
                    "X_Non_Type_Definer_Unary_Function_Args_Not_Heading_0").ToString());
            }
            MDL_Any v = args.MDL_Tuple()["\u0000"];
            switch (func_name_s)
            {
                case "Integer_opposite":
                    if (v.WKBT != Well_Known_Base_Type.MDL_Integer)
                    {
                        throw new ArgumentException(m.Simple_MD_Excuse(
                            "X_Integer_opposite_Arg_0_Not_Integer").ToString());
                    }
                    return m.MDL_Integer(-v.MDL_Integer());
                case "Integer_modulus":
                    if (v.WKBT != Well_Known_Base_Type.MDL_Integer)
                    {
                        throw new ArgumentException(m.Simple_MD_Excuse(
                            "X_Integer_modulus_Arg_0_Not_Integer").ToString());
                    }
                    return m.MDL_Integer(BigInteger.Abs(v.MDL_Integer()));
                case "Integer_factorial":
                    if (v.WKBT != Well_Known_Base_Type.MDL_Integer
                        || v.MDL_Integer() < 0)
                    {
                        throw new ArgumentException(m.Simple_MD_Excuse(
                            "X_Integer_factorial_Arg_0_Not_Integer_NN").ToString());
                    }
                    // Note that System.Numerics.BigInteger doesn't come
                    // with a Factorial(n) so we have to do it ourselves.
                    return m.MDL_Integer(Integer__factorial(v.MDL_Integer()));
                case "Array_count":
                    if (v.WKBT != Well_Known_Base_Type.MDL_Array)
                    {
                        throw new ArgumentException(m.Simple_MD_Excuse(
                            "X_Array_count_Arg_0_Not_Array").ToString());
                    }
                    return m.MDL_Integer(Array__count(v));
                case "Bag_unique":
                    if (v.WKBT != Well_Known_Base_Type.MDL_Bag)
                    {
                        throw new ArgumentException(m.Simple_MD_Excuse(
                            "X_Bag_unique_Arg_0_Not_Bag").ToString());
                    }
                    return new MDL_Any {
                        Memory = m,
                        WKBT = Well_Known_Base_Type.MDL_Bag,
                        Details = new MDL_Bag_Struct {
                            Local_Symbolic_Type = Symbolic_Bag_Type.Unique,
                            Members = v.MDL_Bag(),
                            Cached_Members_Meta = new Cached_Members_Meta {
                                Tree_All_Unique = true,
                            },
                        },
                    };
                case "Tuple_degree":
                    if (v.WKBT != Well_Known_Base_Type.MDL_Tuple)
                    {
                        throw new ArgumentException(m.Simple_MD_Excuse(
                            "X_Tuple_degree_Arg_0_Not_Tuple").ToString());
                    }
                    return m.MDL_Integer(v.MDL_Tuple().Count);
                case "Tuple_heading":
                    if (v.WKBT != Well_Known_Base_Type.MDL_Tuple)
                    {
                        throw new ArgumentException(m.Simple_MD_Excuse(
                            "X_Tuple_heading_Arg_0_Not_Tuple").ToString());
                    }
                    return m.Tuple__Heading(v);
                case "Text_from_UTF_8_Blob":
                    if (v.WKBT != Well_Known_Base_Type.MDL_Blob)
                    {
                        throw new ArgumentException(m.Simple_MD_Excuse(
                            "X_Text_from_UTF_8_Blob_Arg_0_Not_Blob").ToString());
                    }
                    return m.MDL_Text_from_UTF_8_MD_Blob(v);
                case "MDPT_Parsing_Unit_Text_to_Any":
                    if (v.WKBT != Well_Known_Base_Type.MDL_Text)
                    {
                        throw new ArgumentException(m.Simple_MD_Excuse(
                            "X_MDPT_Parsing_Unit_Text_to_Any_Arg_0_Not_Text").ToString());
                    }
                    // TODO: Everything.
                    return m.Well_Known_Excuses["No_Reason"];
                default:
                    throw new NotImplementedException();
            }
        }

        // BINARY FUNCTIONS

        if (Constants.Strings__Foundation_Binary_Function_Names().Contains(func_name_s))
        {
            if (!m.Tuple__Same_Heading(args, m.Heading_0_1))
            {
                throw new ArgumentException(m.Simple_MD_Excuse(
                    "X_Binary_Function_Args_Not_Heading_0_1").ToString());
            }
            MDL_Any a0 = args.MDL_Tuple()["\u0000"];
            MDL_Any a1 = args.MDL_Tuple()["\u0001"];
            switch (func_name_s)
            {
                case "same":
                    return m.MDL_Boolean(Any__same(a0, a1));
                case "Integer_plus":
                    if (a0.WKBT != Well_Known_Base_Type.MDL_Integer)
                    {
                        throw new ArgumentException(m.Simple_MD_Excuse(
                            "X_Integer_plus_Arg_0_Not_Integer").ToString());
                    }
                    if (a1.WKBT != Well_Known_Base_Type.MDL_Integer)
                    {
                        throw new ArgumentException(m.Simple_MD_Excuse(
                            "X_Integer_plus_Arg_1_Not_Integer").ToString());
                    }
                    return m.MDL_Integer(a0.MDL_Integer() + a1.MDL_Integer());
                case "Array_at":
                    if (a0.WKBT != Well_Known_Base_Type.MDL_Array)
                    {
                        throw new ArgumentException(m.Simple_MD_Excuse(
                            "X_Array_at_Arg_0_Not_Array").ToString());
                    }
                    if (a1.WKBT != Well_Known_Base_Type.MDL_Integer
                        || a1.MDL_Integer() < 0)
                    {
                        throw new ArgumentException(m.Simple_MD_Excuse(
                            "X_Array_at_Arg_1_Not_Integer_NN").ToString());
                    }
                    MDL_Any maybe_member = Array__maybe_at(a0, (Int64)a1.MDL_Integer());
                    if (maybe_member is null)
                    {
                        throw new ArgumentException(
                            m.Well_Known_Excuses["No_Such_Ord_Pos"].ToString());
                    }
                    return maybe_member;
                default:
                    throw new NotImplementedException();
            }
        }

        // TERNARY FUNCTIONS

        if (Constants.Strings__Foundation_Ternary_Function_Names().Contains(func_name_s))
        {
            if (!m.Tuple__Same_Heading(args, m.Heading_0_1_2))
            {
                throw new ArgumentException(m.Simple_MD_Excuse(
                    "X_Ternary_Function_Args_Not_Heading_0_1_2").ToString());
            }
            MDL_Any v = args.MDL_Tuple()["\u0000"];
            switch (func_name_s)
            {
                default:
                    throw new NotImplementedException();
            }
        }

        throw new NotImplementedException();
    }

    internal Boolean Any__same(MDL_Any a0, MDL_Any a1)
    {
        if (Object.ReferenceEquals(a0, a1))
        {
            return true;
        }
        if (a0.WKBT != a1.WKBT)
        {
            return false;
        }
        if (a0.Cached_MD_Any_Identity is not null
            && a1.Cached_MD_Any_Identity is not null)
        {
            if (a0.Cached_MD_Any_Identity == a1.Cached_MD_Any_Identity)
            {
                merge_two_same(a0, a1);
                return true;
            }
            return false;
        }
        Boolean result;
        switch (a0.WKBT)
        {
            case Well_Known_Base_Type.MDL_Boolean:
                return a0.MDL_Boolean() == a1.MDL_Boolean();
            case Well_Known_Base_Type.MDL_Integer:
                result = (a0.MDL_Integer() == a1.MDL_Integer());
                break;
            case Well_Known_Base_Type.MDL_Fraction:
                MDL_Fraction_Struct fs0 = a0.MDL_Fraction();
                MDL_Fraction_Struct fs1 = a1.MDL_Fraction();
                if (Object.ReferenceEquals(fs0, fs1))
                {
                    result = true;
                    break;
                }
                if (fs0.As_Decimal is not null && fs1.As_Decimal is not null)
                {
                    result = (fs0.As_Decimal == fs1.As_Decimal);
                    break;
                }
                fs0.Ensure_Pair();
                fs1.Ensure_Pair();
                if (fs0.As_Pair.Denominator == fs1.As_Pair.Denominator)
                {
                    result = (fs0.As_Pair.Numerator == fs1.As_Pair.Numerator);
                    break;
                }
                fs0.Ensure_Coprime();
                fs1.Ensure_Coprime();
                result = (fs0.As_Pair.Denominator == fs1.As_Pair.Denominator
                    && fs0.As_Pair.Numerator == fs1.As_Pair.Numerator);
                break;
            case Well_Known_Base_Type.MDL_Bits:
                result = Enumerable.SequenceEqual(
                    BitArray_to_List(a0.MDL_Bits()),
                    BitArray_to_List(a1.MDL_Bits()));
                break;
            case Well_Known_Base_Type.MDL_Blob:
                result = Enumerable.SequenceEqual(a0.MDL_Blob(), a1.MDL_Blob());
                break;
            case Well_Known_Base_Type.MDL_Text:
                result = (a0.MDL_Text().Code_Point_Members == a1.MDL_Text().Code_Point_Members);
                break;
            case Well_Known_Base_Type.MDL_Array:
                Memory.Array__Collapse(a0);
                Memory.Array__Collapse(a1);
                MDL_Array_Struct n0 = a0.MDL_Array();
                MDL_Array_Struct n1 = a1.MDL_Array();
                if (Object.ReferenceEquals(n0, n1))
                {
                    result = true;
                    break;
                }
                if (n0.Local_Symbolic_Type == Symbolic_Array_Type.None
                    && n1.Local_Symbolic_Type == Symbolic_Array_Type.None)
                {
                    // In theory we should never get here assuming that
                    // the empty Array is optimized to return a constant
                    // at selection time, but we will check anyway.
                    result = true;
                    break;
                }
                if (n0.Local_Symbolic_Type == Symbolic_Array_Type.Singular
                    && n1.Local_Symbolic_Type == Symbolic_Array_Type.Singular)
                {
                    result = ((n0.Local_Singular_Members().Multiplicity
                            == n1.Local_Singular_Members().Multiplicity)
                        && Any__same(n0.Local_Singular_Members().Member,
                            n1.Local_Singular_Members().Member));
                    break;
                }
                if (n0.Local_Symbolic_Type == Symbolic_Array_Type.Arrayed
                    && n1.Local_Symbolic_Type == Symbolic_Array_Type.Arrayed)
                {
                    // This works because MDL_Any Equals() calls Any__Same().
                    result = Enumerable.SequenceEqual(
                        n0.Local_Arrayed_Members(),
                        n1.Local_Arrayed_Members());
                    break;
                }
                MDL_Array_Struct n0_ = n0;
                MDL_Array_Struct n1_ = n1;
                if (n0_.Local_Symbolic_Type == Symbolic_Array_Type.Arrayed
                    && n1_.Local_Symbolic_Type == Symbolic_Array_Type.Singular)
                {
                     n1_ = n0;
                     n0_ = n1;
                }
                if (n0_.Local_Symbolic_Type == Symbolic_Array_Type.Singular
                    && n1_.Local_Symbolic_Type == Symbolic_Array_Type.Arrayed)
                {
                    Multiplied_Member sm = n0_.Local_Singular_Members();
                    List<MDL_Any> am = n1_.Local_Arrayed_Members();
                    result = sm.Multiplicity == am.Count
                        && Enumerable.All(am, m => Any__same(m, sm.Member));
                }
                // We should never get here.
                throw new NotImplementedException();
            case Well_Known_Base_Type.MDL_Set:
            case Well_Known_Base_Type.MDL_Bag:
                // MDL_Set and MDL_Bag have the same internal representation.
                Memory.Bag__Collapse(bag: a0, want_indexed: true);
                Memory.Bag__Collapse(bag: a1, want_indexed: true);
                MDL_Bag_Struct bn0 = a0.MDL_Bag();
                MDL_Bag_Struct bn1 = a1.MDL_Bag();
                if (Object.ReferenceEquals(bn0, bn1))
                {
                    result = true;
                    break;
                }
                if (bn0.Local_Symbolic_Type == Symbolic_Bag_Type.None
                    && bn1.Local_Symbolic_Type == Symbolic_Bag_Type.None)
                {
                    // In theory we should never get here assuming that
                    // the empty Array is optimized to return a constant
                    // at selection time, but we will check anyway.
                    result = true;
                    break;
                }
                if (bn0.Local_Symbolic_Type == Symbolic_Bag_Type.Indexed
                    && bn1.Local_Symbolic_Type == Symbolic_Bag_Type.Indexed)
                {
                    Dictionary<MDL_Any,Multiplied_Member> im0
                        = bn0.Local_Indexed_Members();
                    Dictionary<MDL_Any,Multiplied_Member> im1
                        = bn1.Local_Indexed_Members();
                    result = im0.Count == im1.Count
                        && Enumerable.All(
                            im0.Values,
                            m => im1.ContainsKey(m.Member)
                                && im1[m.Member].Multiplicity == m.Multiplicity
                        );
                    break;
                }
                // We should never get here.
                throw new NotImplementedException();
            case Well_Known_Base_Type.MDL_Tuple:
            case Well_Known_Base_Type.MDL_Excuse:
                // MDL_Tuple and MDL_Excuse have the same internal representation.
                Dictionary<String,MDL_Any> attrs0 = a0.MDL_Tuple();
                Dictionary<String,MDL_Any> attrs1 = a1.MDL_Tuple();
                // First test just that the Tuple headings are the same,
                // and only if they are, compare the attribute values.
                return (attrs0.Count == attrs1.Count)
                    && Enumerable.All(attrs0, attr => attrs1.ContainsKey(attr.Key))
                    && Enumerable.All(attrs0, attr => Any__same(attr.Value, attrs1[attr.Key]));
            case Well_Known_Base_Type.MDL_Article:
                result = Any__same(a0.MDL_Article().Label, a1.MDL_Article().Label)
                      && Any__same(a0.MDL_Article().Attrs, a1.MDL_Article().Attrs);
                break;
            case Well_Known_Base_Type.MDL_Variable:
            case Well_Known_Base_Type.MDL_Process:
            case Well_Known_Base_Type.MDL_Stream:
            case Well_Known_Base_Type.MDL_External:
                // Every Muldis Data Language Handle object is always distinct from every other one.
                return false;
            default:
                throw new NotImplementedException();
        }
        if (result)
        {
            merge_two_same(a0, a1);
        }
        return result;
    }

    internal List<Boolean> BitArray_to_List(BitArray value)
    {
        System.Collections.IEnumerator e = value.GetEnumerator();
        List<Boolean> list = new List<Boolean>();
        while (e.MoveNext())
        {
            list.Add((Boolean)e.Current);
        }
        return list;
    }

    internal void merge_two_same(MDL_Any a0, MDL_Any a1)
    {
        // This may be run on 2 MDL_Any only if it was already
        // determined they represent the same Muldis Data Language value; typically
        // it is invoked by Any__same() when it would result in true.
            // TODO: Make a more educated decision on which one to keep.
            // It should also have logic dispatching on WKBT and
            // do struct mergers as applicable eg copying identity info.
    }

    private BigInteger Integer__factorial(BigInteger value)
    {
        // Note that System.Numerics.BigInteger doesn't come
        // with a Factorial(n) so we have to do it ourselves.
        // Note we calculate this by iteration rather than recursion in
        // order to scale better ... unless the C# compiler is smart
        // enough to optimize tail recursion anyway.
        BigInteger result = 1;
        for (Int32 i = 2; i <= value; i++)
        {
            result *= i;
        }
        return result;
    }

    internal Int64 Bits__count(MDL_Any bits)
    {
        return bits.MDL_Bits().Length;
    }

    internal Int64 Blob__count(MDL_Any blob)
    {
        return blob.MDL_Blob().Length;
    }

    internal Int64 Text__count(MDL_Any text)
    {
        MDL_Text_Struct node = text.MDL_Text();
        if (node.Cached_Member_Count is null)
        {
            if (node.Has_Any_Non_BMP)
            {
                String s = node.Code_Point_Members;
                Int32 count = 0;
                for (Int32 i = 0; i < s.Length; i++)
                {
                    count++;
                    if ((i+1) < s.Length
                        && Char.IsSurrogatePair(s[i], s[i+1]))
                    {
                        i++;
                    }
                }
                node.Cached_Member_Count = count;
            }
            else
            {
                node.Cached_Member_Count = node.Code_Point_Members.Length;
            }
        }
        return (Int64)node.Cached_Member_Count;
    }

    internal Int64 Array__count(MDL_Any array)
    {
        return Array__node__tree_member_count(array.MDL_Array());
    }

    private Int64 Array__node__tree_member_count(MDL_Array_Struct node)
    {
        if (node.Cached_Members_Meta.Tree_Member_Count is null)
        {
            switch (node.Local_Symbolic_Type)
            {
                case Symbolic_Array_Type.None:
                    node.Cached_Members_Meta.Tree_Member_Count = 0;
                    break;
                case Symbolic_Array_Type.Singular:
                    node.Cached_Members_Meta.Tree_Member_Count
                        = node.Local_Singular_Members().Multiplicity;
                    break;
                case Symbolic_Array_Type.Arrayed:
                    node.Cached_Members_Meta.Tree_Member_Count
                        = node.Local_Arrayed_Members().Count;
                    break;
                case Symbolic_Array_Type.Catenated:
                    node.Cached_Members_Meta.Tree_Member_Count
                        = Array__node__tree_member_count(node.Tree_Catenated_Members().A0)
                        + Array__node__tree_member_count(node.Tree_Catenated_Members().A1);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        return (Int64)node.Cached_Members_Meta.Tree_Member_Count;
    }

    private MDL_Any Bits__maybe_at(MDL_Any bits, Int64 ord_pos)
    {
        return Memory.MDL_Integer(bits.MDL_Bits()[(Int32)ord_pos] ? 1 : 0);
    }

    private MDL_Any Blob__maybe_at(MDL_Any blob, Int64 ord_pos)
    {
        return Memory.MDL_Integer(blob.MDL_Blob()[(Int32)ord_pos]);
    }

    private MDL_Any Text__maybe_at(MDL_Any text, Int64 ord_pos)
    {
        MDL_Text_Struct node = text.MDL_Text();
        if (node.Has_Any_Non_BMP)
        {
            String s = node.Code_Point_Members;
            Int64 logical_i = 0;
            for (Int32 i = 0; i < s.Length; i++)
            {
                if (logical_i == ord_pos)
                {
                    if ((i+1) < s.Length
                        && Char.IsSurrogatePair(s[i], s[i+1]))
                    {
                        return Memory.MDL_Integer(
                            Char.ConvertToUtf32(s[i], s[i+1]));
                    }
                    return Memory.MDL_Integer(s[i]);
                }
                logical_i++;
                if ((i+1) < s.Length
                    && Char.IsSurrogatePair(s[i], s[i+1]))
                {
                    i++;
                }
            }
        }
        return Memory.MDL_Integer(
            node.Code_Point_Members[(Int32)ord_pos]);
    }

    private MDL_Any Array__maybe_at(MDL_Any array, Int64 ord_pos)
    {
        return Array__node__maybe_at(array.MDL_Array(), ord_pos);
    }

    private MDL_Any Array__node__maybe_at(MDL_Array_Struct node, Int64 ord_pos)
    {
        if (node.Cached_Members_Meta.Tree_Member_Count == 0)
        {
            return null;
        }
        Int64 maybe_last_ord_pos_seen = -1;
        switch (node.Local_Symbolic_Type)
        {
            case Symbolic_Array_Type.None:
                return null;
            case Symbolic_Array_Type.Singular:
                if (ord_pos <= (maybe_last_ord_pos_seen + node.Local_Singular_Members().Multiplicity))
                {
                    return node.Local_Singular_Members().Member;
                }
                return null;
            case Symbolic_Array_Type.Arrayed:
                Int64 local_member_count = Array__node__tree_member_count(node);
                if (ord_pos <= (maybe_last_ord_pos_seen + local_member_count))
                {
                    Int64 ord_pos_within_local = ord_pos - (1 + maybe_last_ord_pos_seen);
                    return node.Local_Arrayed_Members()[(Int32)ord_pos_within_local];
                }
                return null;
            case Symbolic_Array_Type.Catenated:
                MDL_Any maybe_member = Array__node__maybe_at(node.Tree_Catenated_Members().A0, ord_pos);
                if (maybe_member is not null)
                {
                    return maybe_member;
                }
                maybe_last_ord_pos_seen += Array__node__tree_member_count(node.Tree_Catenated_Members().A0);
                if (ord_pos <= maybe_last_ord_pos_seen)
                {
                    return null;
                }
                Int64 ord_pos_within_succ = ord_pos - (1 + maybe_last_ord_pos_seen);
                return Array__node__maybe_at(node.Tree_Catenated_Members().A1, ord_pos_within_succ);
            default:
                throw new NotImplementedException();
        }
    }
}
