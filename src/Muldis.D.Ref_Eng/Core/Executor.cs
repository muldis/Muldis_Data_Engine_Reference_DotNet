using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Muldis.D.Ref_Eng.Core
{
    internal class Executor
    {
        internal readonly Memory Memory;

        internal readonly Plain_Text.Standard_Parser Standard_Parser;

        internal readonly Plain_Text.Standard_Generator Standard_Generator;

        internal Executor(Memory memory)
        {
            Memory = memory;

            Standard_Parser = new Plain_Text.Standard_Parser();

            Standard_Generator = new Plain_Text.Standard_Generator();
        }

        internal MD_Any Evaluates(MD_Any function, MD_Any args = null)
        {
            Memory m = Memory;
            if (args == null)
            {
                args = m.MD_Tuple_D0;
            }
            if (args.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Tuple)
            {
                throw new ArgumentException(m.Simple_MD_Excuse(
                    "X_Args_Not_Tuple").ToString());
            }
            if (Is_Absolute_Name(function))
            {
                if (Object.ReferenceEquals(
                    Array__maybe_at(function, 0), m.MD_Attr_Name("foundation")))
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

        internal void Performs(MD_Any procedure, MD_Any args = null)
        {
            Memory m = Memory;
            if (args == null)
            {
                args = m.MD_Tuple_D0;
            }
            if (args.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Tuple)
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

        internal Boolean Is_Excuse(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Fraction(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Bits(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Blob(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Text(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_String(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Set(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Tuple_Array(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Relation(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Tuple_Bag(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Interval(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Package(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Function(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Procedure(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Function_Call(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Procedure_Call(MD_Any value)
        {
            throw new NotImplementedException();
        }

        internal Boolean Is_Heading(MD_Any value)
        {
            return value.AS.Member_Status_in_WKT(MD_Well_Known_Type.Heading) == true;
        }

        internal Boolean Is_Attr_Name(MD_Any value)
        {
            return value.AS.Member_Status_in_WKT(MD_Well_Known_Type.Attr_Name) == true;
        }

        internal Boolean Is_Attr_Name_List(MD_Any value)
        {
            if (value.AS.Member_Status_in_WKT(MD_Well_Known_Type.Attr_Name_List) == true)
            {
                return true;
            }
            Memory m = Memory;
            if (value.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Array)
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
            value.AS.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Attr_Name_List, true);
            return true;
        }

        internal Boolean Is_Local_Name(MD_Any value)
        {
            if (value.AS.Member_Status_in_WKT(MD_Well_Known_Type.Local_Name) == true)
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
            MD_Any first = Array__maybe_at(value, 0);
            if ((Object.ReferenceEquals(first, Memory.MD_Attr_Name("foundation")) && count == 2)
                || (Object.ReferenceEquals(first, Memory.MD_Attr_Name("used")) && count >= 2)
                || (Object.ReferenceEquals(first, Memory.MD_Attr_Name("package")) && count >= 1)
                || (Object.ReferenceEquals(first, Memory.MD_Attr_Name("folder")) && count >= 1)
                || (Object.ReferenceEquals(first, Memory.MD_Attr_Name("material")) && count == 1)
                || (Object.ReferenceEquals(first, Memory.MD_Attr_Name("floating")) && count >= 2))
            {
                value.AS.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Local_Name, true);
                return true;
            }
            return false;
        }

        internal Boolean Is_Absolute_Name(MD_Any value)
        {
            if (value.AS.Member_Status_in_WKT(MD_Well_Known_Type.Absolute_Name) == true)
            {
                return true;
            }
            if (!Is_Local_Name(value))
            {
                return false;
            }
            MD_Any first = Array__maybe_at(value, 0);
            if (Object.ReferenceEquals(first, Memory.MD_Attr_Name("foundation"))
                || Object.ReferenceEquals(first, Memory.MD_Attr_Name("used"))
                || Object.ReferenceEquals(first, Memory.MD_Attr_Name("package")))
            {
                value.AS.Declare_Member_Status_in_WKT(MD_Well_Known_Type.Absolute_Name, true);
                return true;
            }
            return false;
        }

        internal MD_Any evaluate_foundation_function(MD_Any func_name, MD_Any args)
        {
            Memory m = Memory;
            String func_name_s
                = Object.ReferenceEquals(func_name, m.Attr_Name_0) ? "\u0000"
                : Object.ReferenceEquals(func_name, m.Attr_Name_1) ? "\u0001"
                : Object.ReferenceEquals(func_name, m.Attr_Name_2) ? "\u0002"
                : func_name.AS.MD_Tuple().Only_OA.Value.Key;

            // TYPE DEFINERS

            if (Constants.Strings__Foundation_Type_Definer_Function_Names().Contains(func_name_s))
            {
                if (!m.Tuple__Same_Heading(args, m.Attr_Name_0))
                {
                    throw new ArgumentException(m.Simple_MD_Excuse(
                        "X_Type_Definer_Function_Args_Not_Heading_0").ToString());
                }
                MD_Any v = args.AS.MD_Tuple().A0;
                switch (func_name_s)
                {
                    case "Any":
                        return m.MD_True;
                    case "None":
                        return m.MD_False;

                    // FOUNDATION BASE TYPE DEFINERS

                    case "Boolean":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Boolean
                        );
                    case "Integer":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Integer
                        );
                    case "Array":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Array
                        );
                    case "Bag":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Bag
                        );
                    case "Tuple":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Tuple
                        );
                    case "Capsule":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Capsule
                        );
                    case "Handle":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Variable
                                || v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Process
                                || v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Stream
                                || v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_External
                        );

                    // HANDLE SUBTYPE DEFINERS

                    case "Variable":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Variable
                        );
                    case "Process":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Process
                        );
                    case "Stream":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Stream
                        );
                    case "External":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_External
                        );

                    // ARRAY SUBTYPE DEFINERS

                    // TUPLE SUBTYPE DEFINERS

                    // CAPSULE SUBTYPE DEFINERS

                    case "Blob":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Blob
                        );

                    case "Text":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Text
                        );

                    case "Excuse":
                        return m.MD_Boolean(
                            v.AS.MD_MSBT == MD_Well_Known_Base_Type.MD_Excuse
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
                MD_Any v = args.AS.MD_Tuple().A0;
                switch (func_name_s)
                {
                    case "Integer_opposite":
                        if (v.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Integer)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Integer_opposite_Arg_0_Not_Integer").ToString());
                        }
                        return m.MD_Integer(-v.AS.MD_Integer());
                    case "Integer_modulus":
                        if (v.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Integer)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Integer_modulus_Arg_0_Not_Integer").ToString());
                        }
                        return m.MD_Integer(BigInteger.Abs(v.AS.MD_Integer()));
                    case "Integer_factorial":
                        if (v.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Integer
                            || v.AS.MD_Integer() < 0)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Integer_factorial_Arg_0_Not_Integer_NN").ToString());
                        }
                        // Note that System.Numerics.BigInteger doesn't come
                        // with a Factorial(n) so we have to do it ourselves.
                        return m.MD_Integer(Integer__factorial(v.AS.MD_Integer()));
                    case "Array_count":
                        if (v.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Array)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Array_count_Arg_0_Not_Array").ToString());
                        }
                        return m.MD_Integer(Array__count(v));
                    case "Bag_unique":
                        if (v.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Bag)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Bag_unique_Arg_0_Not_Bag").ToString());
                        }
                        return new MD_Any { AS = new MD_Any_Struct {
                            Memory = m,
                            MD_MSBT = MD_Well_Known_Base_Type.MD_Bag,
                            Details = new MD_Bag_Struct {
                                Local_Symbolic_Type = Symbolic_Bag_Type.Unique,
                                Members = v.AS.MD_Bag(),
                                Cached_Members_Meta = new Cached_Members_Meta {
                                    Tree_All_Unique = true,
                                },
                            },
                        } };
                    case "Tuple_degree":
                        if (v.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Tuple)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Tuple_degree_Arg_0_Not_Tuple").ToString());
                        }
                        return m.MD_Integer(v.AS.MD_Tuple().Degree);
                    case "Tuple_heading":
                        if (v.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Tuple)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Tuple_heading_Arg_0_Not_Tuple").ToString());
                        }
                        return m.Tuple__Heading(v);
                    case "Text_from_UTF_8_Blob":
                        if (v.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Blob)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Text_from_UTF_8_Blob_Arg_0_Not_Blob").ToString());
                        }
                        return m.MD_Text_from_UTF_8_MD_Blob(v);
                    case "MDPT_Parsing_Unit_Text_to_Any":
                        if (v.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Text)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_MDPT_Parsing_Unit_Text_to_Any_Arg_0_Not_Text").ToString());
                        }
                        return this.Standard_Parser.MDPT_Parsing_Unit_MD_Text_to_MD_Any(v);
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
                MD_Any a0 = args.AS.MD_Tuple().A0;
                MD_Any a1 = args.AS.MD_Tuple().A1;
                switch (func_name_s)
                {
                    case "same":
                        return m.MD_Boolean(Any__same(a0, a1));
                    case "Integer_plus":
                        if (a0.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Integer)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Integer_plus_Arg_0_Not_Integer").ToString());
                        }
                        if (a1.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Integer)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Integer_plus_Arg_1_Not_Integer").ToString());
                        }
                        return m.MD_Integer(a0.AS.MD_Integer() + a1.AS.MD_Integer());
                    case "Array_at":
                        if (a0.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Array)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Array_at_Arg_0_Not_Array").ToString());
                        }
                        if (a1.AS.MD_MSBT != MD_Well_Known_Base_Type.MD_Integer
                            || a1.AS.MD_Integer() < 0)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Array_at_Arg_1_Not_Integer_NN").ToString());
                        }
                        MD_Any maybe_member = Array__maybe_at(a0, (Int64)a1.AS.MD_Integer());
                        if (maybe_member == null)
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
                MD_Any v = args.AS.MD_Tuple().A0;
                switch (func_name_s)
                {
                    default:
                        throw new NotImplementedException();
                }
            }

            throw new NotImplementedException();
        }

        internal Boolean Any__same(MD_Any a0, MD_Any a1)
        {
            if (Object.ReferenceEquals(a0, a1))
            {
                return true;
            }
            if (Object.ReferenceEquals(a0.AS, a1.AS))
            {
                return true;
            }
            if (a0.AS.MD_MSBT != a1.AS.MD_MSBT)
            {
                return false;
            }
            if (a0.AS.Cached_MD_Any_Identity != null
                && a1.AS.Cached_MD_Any_Identity != null)
            {
                if (a0.AS.Cached_MD_Any_Identity == a1.AS.Cached_MD_Any_Identity)
                {
                    merge_two_same(a0, a1);
                    return true;
                }
                return false;
            }
            Boolean result;
            switch (a0.AS.MD_MSBT)
            {
                case MD_Well_Known_Base_Type.MD_Boolean:
                    return a0.AS.MD_Boolean() == a1.AS.MD_Boolean();
                case MD_Well_Known_Base_Type.MD_Integer:
                    result = (a0.AS.MD_Integer() == a1.AS.MD_Integer());
                    break;
                case MD_Well_Known_Base_Type.MD_Fraction:
                    MD_Fraction_Struct fs0 = a0.AS.MD_Fraction();
                    MD_Fraction_Struct fs1 = a1.AS.MD_Fraction();
                    if (Object.ReferenceEquals(fs0, fs1))
                    {
                        result = true;
                        break;
                    }
                    if (fs0.As_Decimal != null && fs1.As_Decimal != null)
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
                case MD_Well_Known_Base_Type.MD_Bits:
                    result = Enumerable.SequenceEqual(
                        BitArray_to_List(a0.AS.MD_Bits()),
                        BitArray_to_List(a1.AS.MD_Bits()));
                    break;
                case MD_Well_Known_Base_Type.MD_Blob:
                    result = Enumerable.SequenceEqual(a0.AS.MD_Blob(), a1.AS.MD_Blob());
                    break;
                case MD_Well_Known_Base_Type.MD_Text:
                    result = (a0.AS.MD_Text().Codepoint_Members == a1.AS.MD_Text().Codepoint_Members);
                    break;
                case MD_Well_Known_Base_Type.MD_Array:
                    Memory.Array__Collapse(a0);
                    Memory.Array__Collapse(a1);
                    MD_Array_Struct n0 = a0.AS.MD_Array();
                    MD_Array_Struct n1 = a1.AS.MD_Array();
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
                        // This works because MD_Any Equals() calls Any__Same().
                        result = Enumerable.SequenceEqual(
                            n0.Local_Arrayed_Members(),
                            n1.Local_Arrayed_Members());
                        break;
                    }
                    MD_Array_Struct n0_ = n0;
                    MD_Array_Struct n1_ = n1;
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
                        List<MD_Any> am = n1_.Local_Arrayed_Members();
                        result = sm.Multiplicity == am.Count
                            && Enumerable.All(am, m => Any__same(m, sm.Member));
                    }
                    // We should never get here.
                    throw new NotImplementedException();
                case MD_Well_Known_Base_Type.MD_Set:
                case MD_Well_Known_Base_Type.MD_Bag:
                    // MD_Set and MD_Bag have the same internal representation.
                    Memory.Bag__Collapse(bag: a0, want_indexed: true);
                    Memory.Bag__Collapse(bag: a1, want_indexed: true);
                    MD_Bag_Struct bn0 = a0.AS.MD_Bag();
                    MD_Bag_Struct bn1 = a1.AS.MD_Bag();
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
                        Dictionary<MD_Any,Multiplied_Member> im0
                            = bn0.Local_Indexed_Members();
                        Dictionary<MD_Any,Multiplied_Member> im1
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
                case MD_Well_Known_Base_Type.MD_Tuple:
                case MD_Well_Known_Base_Type.MD_Excuse:
                    // MD_Tuple and MD_Excuse have the same internal representation.
                    MD_Tuple_Struct ts0 = a0.AS.MD_Tuple();
                    MD_Tuple_Struct ts1 = a1.AS.MD_Tuple();
                    // First test just that the Tuple headings are the same,
                    // and only if they are, compare the attribute values.
                    return (ts0.Degree == ts1.Degree)
                        && ((ts0.A0 == null) == (ts1.A0 == null))
                        && ((ts0.A1 == null) == (ts1.A1 == null))
                        && ((ts0.A2 == null) == (ts1.A2 == null))
                        && ((ts0.Only_OA == null && ts1.Only_OA == null)
                            || (ts0.Only_OA != null && ts1.Only_OA != null
                            && ts0.Only_OA.Value.Key == ts1.Only_OA.Value.Key))
                        && ((ts0.Multi_OA == null && ts1.Multi_OA == null)
                            || (ts0.Multi_OA != null && ts1.Multi_OA != null
                            && Enumerable.All(ts0.Multi_OA,
                                attr => ts1.Multi_OA.ContainsKey(attr.Key))))
                        && (ts0.A0 == null || Any__same(ts0.A0, ts1.A0))
                        && (ts0.A1 == null || Any__same(ts0.A1, ts1.A1))
                        && (ts0.A2 == null || Any__same(ts0.A2, ts1.A2))
                        && (ts0.Only_OA == null || Any__same(
                            ts0.Only_OA.Value.Value, ts1.Only_OA.Value.Value))
                        && (ts0.Multi_OA == null || Enumerable.All(ts0.Multi_OA,
                            attr => Any__same(attr.Value, ts1.Multi_OA[attr.Key])));
                case MD_Well_Known_Base_Type.MD_Capsule:
                    result = Any__same(a0.AS.MD_Capsule().Label, a1.AS.MD_Capsule().Label)
                          && Any__same(a0.AS.MD_Capsule().Attrs, a1.AS.MD_Capsule().Attrs);
                    break;
                case MD_Well_Known_Base_Type.MD_Variable:
                case MD_Well_Known_Base_Type.MD_Process:
                case MD_Well_Known_Base_Type.MD_Stream:
                case MD_Well_Known_Base_Type.MD_External:
                    // Every Muldis D Handle object is always distinct from every other one.
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

        internal void merge_two_same(MD_Any a0, MD_Any a1)
        {
            // This may be run on 2 MD_Any only if it was already
            // determined they represent the same Muldis D value; typically
            // it is invoked by Any__same() when it would result in true.
            a1.AS = a0.AS;
                // TODO: Make a more educated decision on which one to keep.
                // It should also have logic dispatching on MD_MSBT and
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

        internal Int64 Bits__count(MD_Any bits)
        {
            return bits.AS.MD_Bits().Length;
        }

        internal Int64 Blob__count(MD_Any blob)
        {
            return blob.AS.MD_Blob().Length;
        }

        internal Int64 Text__count(MD_Any text)
        {
            MD_Text_Struct node = text.AS.MD_Text();
            if (node.Cached_Member_Count == null)
            {
                if (node.Has_Any_Non_BMP)
                {
                    String s = node.Codepoint_Members;
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
                    node.Cached_Member_Count = node.Codepoint_Members.Length;
                }
            }
            return (Int64)node.Cached_Member_Count;
        }

        internal Int64 Array__count(MD_Any array)
        {
            return Array__node__tree_member_count(array.AS.MD_Array());
        }

        private Int64 Array__node__tree_member_count(MD_Array_Struct node)
        {
            if (node.Cached_Members_Meta.Tree_Member_Count == null)
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

        private MD_Any Bits__maybe_at(MD_Any bits, Int64 ord_pos)
        {
            return Memory.MD_Integer(bits.AS.MD_Bits()[(Int32)ord_pos] ? 1 : 0);
        }

        private MD_Any Blob__maybe_at(MD_Any blob, Int64 ord_pos)
        {
            return Memory.MD_Integer(blob.AS.MD_Blob()[(Int32)ord_pos]);
        }

        private MD_Any Text__maybe_at(MD_Any text, Int64 ord_pos)
        {
            MD_Text_Struct node = text.AS.MD_Text();
            if (node.Has_Any_Non_BMP)
            {
                String s = node.Codepoint_Members;
                Int64 logical_i = 0;
                for (Int32 i = 0; i < s.Length; i++)
                {
                    if (logical_i == ord_pos)
                    {
                        if ((i+1) < s.Length
                            && Char.IsSurrogatePair(s[i], s[i+1]))
                        {
                            return Memory.MD_Integer(
                                Char.ConvertToUtf32(s[i], s[i+1]));
                        }
                        return Memory.MD_Integer(s[i]);
                    }
                    logical_i++;
                    if ((i+1) < s.Length
                        && Char.IsSurrogatePair(s[i], s[i+1]))
                    {
                        i++;
                    }
                }
            }
            return Memory.MD_Integer(
                node.Codepoint_Members[(Int32)ord_pos]);
        }

        private MD_Any Array__maybe_at(MD_Any array, Int64 ord_pos)
        {
            return Array__node__maybe_at(array.AS.MD_Array(), ord_pos);
        }

        private MD_Any Array__node__maybe_at(MD_Array_Struct node, Int64 ord_pos)
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
                    MD_Any maybe_member = Array__node__maybe_at(node.Tree_Catenated_Members().A0, ord_pos);
                    if (maybe_member != null)
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
}
