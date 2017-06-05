using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Muldis.D.Ref_Eng.Core
{
    internal class Executor
    {
        internal MD_Any Evaluates(MD_Any function, MD_Any args = null)
        {
            Memory m = function.AS.Memory;
            if (args == null)
            {
                args = m.MD_Tuple_D0;
            }
            if (args.AS.MD_Foundation_Type != MD_Foundation_Type.MD_Tuple)
            {
                throw new ArgumentException(m.Simple_MD_Excuse(
                    "X_Args_Not_Tuple").ToString());
            }
            switch (function.AS.MD_Foundation_Type)
            {
                case MD_Foundation_Type.MD_Array:
                    // Try to use as a Muldis D Absolute_Name value where
                    // that specifies the qualified name of a function to call.
                    MD_Any maybe_first = Array__maybe_at(function, 0);
                    if (maybe_first != null)
                    {
                        if (Object.ReferenceEquals(maybe_first, m.MD_Attr_Name("foundation")))
                        {
                            MD_Any maybe_second = Array__maybe_at(function, 1);
                            if (Array__count(function) == 2
                                && maybe_second != null
                                && maybe_second.AS.Cached_WKT.Contains(
                                    MD_Well_Known_Type.Attr_Name))
                            {
                                return evaluate_foundation_function(maybe_second, args);
                            }
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Malformed_Foundation_Function_Call").ToString());
                        }
                        throw new NotImplementedException();
                    }
                    throw new ArgumentException(m.Simple_MD_Excuse(
                        "X_Malformed_Function_Call").ToString());
                case MD_Foundation_Type.MD_Capsule:
                    MD_Any label = function.AS.MD_Capsule.Label;
                    MD_Any attrs = function.AS.MD_Capsule.Attrs;
                    if (Object.ReferenceEquals(label, m.MD_Attr_Name("Routine_Call")))
                    {
                        throw new NotImplementedException();
                    }
                    if (Object.ReferenceEquals(label, m.MD_Attr_Name("Function")))
                    {
                        throw new NotImplementedException();
                    }
                    if (Object.ReferenceEquals(label, m.MD_Attr_Name("Package")))
                    {
                        throw new NotImplementedException();
                    }
                    throw new ArgumentException(m.Simple_MD_Excuse(
                        "X_Malformed_Function_Call").ToString());
                default:
                    throw new ArgumentException(m.Simple_MD_Excuse(
                        "X_Malformed_Function_Call").ToString());
            }
        }

        internal void Performs(MD_Any procedure, MD_Any args = null)
        {
            Memory m = procedure.AS.Memory;
            if (args == null)
            {
                args = m.MD_Tuple_D0;
            }
            if (args.AS.MD_Foundation_Type != MD_Foundation_Type.MD_Tuple)
            {
                throw new ArgumentException(m.Simple_MD_Excuse("X_Args_Not_Tuple").ToString());
            }
            throw new NotImplementedException();
        }

        internal MD_Any evaluate_foundation_function(MD_Any func_name, MD_Any args)
        {
            Memory m = func_name.AS.Memory;
            String func_name_s
                = Object.ReferenceEquals(func_name, m.Attr_Name_0) ? "\u0000"
                : Object.ReferenceEquals(func_name, m.Attr_Name_1) ? "\u0001"
                : Object.ReferenceEquals(func_name, m.Attr_Name_2) ? "\u0002"
                : func_name.AS.MD_Tuple.Only_OA.Value.Key;

            // TYPE DEFINERS

            if (Constants.Strings__Foundation_Type_Definer_Function_Names().Contains(func_name_s))
            {
                if (!m.Tuple__Same_Heading(args, m.Attr_Name_0))
                {
                    throw new ArgumentException(m.Simple_MD_Excuse(
                        "X_Type_Definer_Function_Args_Not_Heading_0").ToString());
                }
                MD_Any v = args.AS.MD_Tuple.A0;
                switch (func_name_s)
                {
                    case "Any":
                        return m.MD_True;
                    case "None":
                        return m.MD_False;

                    // FOUNDATION TYPE DEFINERS

                    case "Boolean":
                        return m.MD_Boolean(
                            v.AS.MD_Foundation_Type == MD_Foundation_Type.MD_Boolean
                        );
                    case "Integer":
                        return m.MD_Boolean(
                            v.AS.MD_Foundation_Type == MD_Foundation_Type.MD_Integer
                        );
                    case "Array":
                        return m.MD_Boolean(
                            v.AS.MD_Foundation_Type == MD_Foundation_Type.MD_Array
                        );
                    case "Bag":
                        return m.MD_Boolean(
                            v.AS.MD_Foundation_Type == MD_Foundation_Type.MD_Bag
                        );
                    case "Tuple":
                        return m.MD_Boolean(
                            v.AS.MD_Foundation_Type == MD_Foundation_Type.MD_Tuple
                        );
                    case "Capsule":
                        return m.MD_Boolean(
                            v.AS.MD_Foundation_Type == MD_Foundation_Type.MD_Capsule
                        );
                    case "Handle":
                        return m.MD_Boolean(
                            v.AS.MD_Foundation_Type == MD_Foundation_Type.MD_Handle
                        );

                    // HANDLE SUBTYPE DEFINERS

                    case "Variable":
                        return m.MD_Boolean(
                            v.AS.MD_Foundation_Type == MD_Foundation_Type.MD_Handle
                                && v.AS.MD_Handle.MD_Handle_Type == MD_Handle_Type.MD_Variable
                        );
                    case "Process":
                        return m.MD_Boolean(
                            v.AS.MD_Foundation_Type == MD_Foundation_Type.MD_Handle
                                && v.AS.MD_Handle.MD_Handle_Type == MD_Handle_Type.MD_Process
                        );
                    case "Stream":
                        return m.MD_Boolean(
                            v.AS.MD_Foundation_Type == MD_Foundation_Type.MD_Handle
                                && v.AS.MD_Handle.MD_Handle_Type == MD_Handle_Type.MD_Stream
                        );
                    case "External":
                        return m.MD_Boolean(
                            v.AS.MD_Foundation_Type == MD_Foundation_Type.MD_Handle
                                && v.AS.MD_Handle.MD_Handle_Type == MD_Handle_Type.MD_External
                        );

                    // ARRAY SUBTYPE DEFINERS

                    // TUPLE SUBTYPE DEFINERS

                    // CAPSULE SUBTYPE DEFINERS

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
                MD_Any v = args.AS.MD_Tuple.A0;
                switch (func_name_s)
                {
                    case "Integer_opposite":
                        if (v.AS.MD_Foundation_Type != MD_Foundation_Type.MD_Integer)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Integer_opposite_Arg_0_Not_Integer").ToString());
                        }
                        return m.MD_Integer(-v.AS.MD_Integer);
                    case "Integer_modulus":
                        if (v.AS.MD_Foundation_Type != MD_Foundation_Type.MD_Integer)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Integer_modulus_Arg_0_Not_Integer").ToString());
                        }
                        return m.MD_Integer(BigInteger.Abs(v.AS.MD_Integer));
                    case "Integer_factorial":
                        if (v.AS.MD_Foundation_Type != MD_Foundation_Type.MD_Integer
                            || v.AS.MD_Integer < 0)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Integer_factorial_Arg_0_Not_Integer_NN").ToString());
                        }
                        // Note that System.Numerics.BigInteger doesn't come
                        // with a Factorial(n) so we have to do it ourselves.
                        return m.MD_Integer(Integer__factorial(v.AS.MD_Integer));
                    case "Array_count":
                        if (v.AS.MD_Foundation_Type != MD_Foundation_Type.MD_Array)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Array_count_Arg_0_Not_Array").ToString());
                        }
                        return m.MD_Integer(Array__count(v));
                    case "Bag_unique":
                        if (v.AS.MD_Foundation_Type != MD_Foundation_Type.MD_Bag)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Bag_unique_Arg_0_Not_Bag").ToString());
                        }
                        return new MD_Any { AS = new MD_Any_Struct {
                            Memory = m,
                            MD_Foundation_Type = MD_Foundation_Type.MD_Bag,
                            MD_Bag = new MD_Bag_Node {
                                Cached_Tree_All_Unique = true,
                                Local_Symbolic_Type = Symbolic_Value_Type.Unique,
                                Primary_Arg = v.AS.MD_Bag,
                            },
                            Cached_WKT = new HashSet<MD_Well_Known_Type>()
                                {MD_Well_Known_Type.Bag},
                        } };
                    case "Tuple_degree":
                        if (v.AS.MD_Foundation_Type != MD_Foundation_Type.MD_Tuple)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Tuple_degree_Arg_0_Not_Tuple").ToString());
                        }
                        return m.MD_Integer(v.AS.MD_Tuple.Degree);
                    case "Tuple_heading":
                        if (v.AS.MD_Foundation_Type != MD_Foundation_Type.MD_Tuple)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Tuple_heading_Arg_0_Not_Tuple").ToString());
                        }
                        return m.Tuple__Heading(v);
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
                MD_Any a0 = args.AS.MD_Tuple.A0;
                MD_Any a1 = args.AS.MD_Tuple.A1;
                switch (func_name_s)
                {
                    case "same":
                        return m.MD_Boolean(Any__same(a0, a1));
                    case "Integer_plus":
                        if (a0.AS.MD_Foundation_Type != MD_Foundation_Type.MD_Integer)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Integer_plus_Arg_0_Not_Integer").ToString());
                        }
                        if (a1.AS.MD_Foundation_Type != MD_Foundation_Type.MD_Integer)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Integer_plus_Arg_1_Not_Integer").ToString());
                        }
                        return m.MD_Integer(a0.AS.MD_Integer + a1.AS.MD_Integer);
                    case "Array_at":
                        if (a0.AS.MD_Foundation_Type != MD_Foundation_Type.MD_Array)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Array_at_Arg_0_Not_Array").ToString());
                        }
                        if (a1.AS.MD_Foundation_Type != MD_Foundation_Type.MD_Integer
                            || a1.AS.MD_Integer < 0)
                        {
                            throw new ArgumentException(m.Simple_MD_Excuse(
                                "X_Array_at_Arg_1_Not_Integer_NN").ToString());
                        }
                        MD_Any maybe_member = Array__maybe_at(a0, (Int64)a1.AS.MD_Integer);
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
                MD_Any v = args.AS.MD_Tuple.A0;
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
            if (a0.AS.MD_Foundation_Type != a1.AS.MD_Foundation_Type)
            {
                return false;
            }
            Boolean result;
            switch (a0.AS.MD_Foundation_Type)
            {
                case MD_Foundation_Type.MD_Boolean:
                    return a0.AS.MD_Boolean == a1.AS.MD_Boolean;
                case MD_Foundation_Type.MD_Integer:
                    result = a0.AS.MD_Integer == a1.AS.MD_Integer;
                    break;
                case MD_Foundation_Type.MD_Array:
                    MD_Array_Node n0 = a0.AS.MD_Array;
                    MD_Array_Node n1 = a1.AS.MD_Array;
                    if (n0.Tree_Widest_Type == Widest_Component_Type.None
                        && n1.Tree_Widest_Type == Widest_Component_Type.None)
                    {
                        // In theory we should never get here assuming that
                        // the empty Array is optimized to return a constant
                        // at selection time, but we will check anyway.
                        result = true;
                        break;
                    }
                    if (n0.Tree_Widest_Type == Widest_Component_Type.None
                        || n1.Tree_Widest_Type == Widest_Component_Type.None)
                    {
                        return false;
                    }
                    // Every one of the Arrays has at least 1 member.
                    if (n0.Pred_Members != null || n1.Pred_Members != null
                        || n0.Succ_Members != null || n1.Succ_Members != null
                        || n0.Tree_Widest_Type != n1.Tree_Widest_Type)
                    {
                        // For now skip the more complicated cases where
                        // the Array members are defined using > 1 node
                        // or where members use different representations.
                        //throw new NotImplementedException();
                        return false;  // TODO; meanwhile we compare like a reference type
                    }
                    switch (n0.Local_Widest_Type)
                    {
                        case Widest_Component_Type.Unrestricted:
                            result = Enumerable.SequenceEqual(
                                n0.Local_Unrestricted_Members,
                                n1.Local_Unrestricted_Members);
                            break;
                        case Widest_Component_Type.Bit:
                            //throw new NotImplementedException();
                            return false;  // TODO; meanwhile we compare like a reference type
                        case Widest_Component_Type.Octet:
                            result = Enumerable.SequenceEqual(
                                n0.Local_Octet_Members, n1.Local_Octet_Members);
                            break;
                        case Widest_Component_Type.Codepoint:
                            result = (n0.Local_Codepoint_Members
                                == n1.Local_Codepoint_Members);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case MD_Foundation_Type.MD_Bag:
                    MD_Bag_Node bn0 = a0.AS.MD_Bag;
                    MD_Bag_Node bn1 = a1.AS.MD_Bag;
                    if (bn0.Local_Symbolic_Type != bn1.Local_Symbolic_Type)
                    {
                        // For now skip the more complicated cases where
                        // the Bag root nodes use different representations.
                        //throw new NotImplementedException();
                        return false;  // TODO; meanwhile we compare like a reference type
                    }
                    switch (bn0.Local_Symbolic_Type)
                    {
                        case Symbolic_Value_Type.None:
                            // In theory we should never get here assuming that
                            // the empty Bag is optimized to return a constant
                            // at selection time, but we will check anyway.
                            result = true;
                            break;
                        case Symbolic_Value_Type.Singular:
                            result = ((bn0.Local_Singular_Members.Multiplicity
                                    == bn1.Local_Singular_Members.Multiplicity)
                                && Any__same(bn0.Local_Singular_Members.Member,
                                    bn1.Local_Singular_Members.Member));
                            break;
                        case Symbolic_Value_Type.Arrayed:
                        case Symbolic_Value_Type.Indexed:
                        case Symbolic_Value_Type.Unique:
                        case Symbolic_Value_Type.Insert_N:
                        case Symbolic_Value_Type.Remove_N:
                        case Symbolic_Value_Type.Member_Plus:
                        case Symbolic_Value_Type.Except:
                        case Symbolic_Value_Type.Intersect:
                        case Symbolic_Value_Type.Union:
                        case Symbolic_Value_Type.Exclusive:
                            // For now skip the more complicated cases where
                            // further processing is required in order to
                            // properly compare the 2 Bag for logical equality.
                            // That is, normalize trees into Indexed.
                            // This in practice skips most Bag, alas.
                            //throw new NotImplementedException();
                            return false;  // TODO; meanwhile we compare like a reference type
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case MD_Foundation_Type.MD_Tuple:
                    MD_Tuple_Struct ts0 = a0.AS.MD_Tuple;
                    MD_Tuple_Struct ts1 = a1.AS.MD_Tuple;
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
                case MD_Foundation_Type.MD_Capsule:
                    result = Any__same(a0.AS.MD_Capsule.Label, a1.AS.MD_Capsule.Label)
                          && Any__same(a0.AS.MD_Capsule.Attrs, a1.AS.MD_Capsule.Attrs);
                    break;
                case MD_Foundation_Type.MD_Handle:
                    // Every Muldis D Handle object is always distinct from every other one.
                    return false;
                default:
                    throw new NotImplementedException();
            }
            if (result)
            {
                // TODO: Make a more educated decision on which one to keep.
                a1.AS = a0.AS;
            }
            return result;
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

        private Int64 Array__count(MD_Any array)
        {
            return Array__node__count(array.AS.MD_Array);
        }

        private Int64 Array__node__count(MD_Array_Node node)
        {
            if (node.Cached_Tree_Member_Count == null)
            {
                node.Cached_Tree_Member_Count
                    = Array__node__local_member_count(node)
                        + (node.Pred_Members == null ? 0
                            : Array__node__count(node.Pred_Members))
                        + (node.Succ_Members == null ? 0
                            : Array__node__count(node.Succ_Members));
            }
            return (Int64)node.Cached_Tree_Member_Count;
        }

        private Int64 Array__node__local_member_count(MD_Array_Node node)
        {
            Int64 lm = node.Local_Multiplicity;
            switch (node.Local_Widest_Type)
            {
                case Widest_Component_Type.None:
                    return 0;
                case Widest_Component_Type.Unrestricted:
                    return lm * node.Local_Unrestricted_Members.Count;
                case Widest_Component_Type.Bit:
                    return lm * node.Local_Bit_Members.Length;
                case Widest_Component_Type.Octet:
                    return lm * node.Local_Octet_Members.Length;
                case Widest_Component_Type.Codepoint:
                    return lm * node.Local_Codepoint_Members.Length;
                default:
                    throw new NotImplementedException();
            }
        }

        private MD_Any Array__maybe_at(MD_Any array, Int64 ord_pos)
        {
            return Array__node__maybe_at(array.AS.MD_Array, ord_pos);
        }

        private MD_Any Array__node__maybe_at(MD_Array_Node node, Int64 ord_pos)
        {
            if (node.Cached_Tree_Member_Count == 0)
            {
                return null;
            }
            MD_Any maybe_member = null;
            Int64 maybe_last_ord_pos_seen = -1;
            if (node.Pred_Members != null)
            {
                maybe_member = Array__node__maybe_at(node.Pred_Members, ord_pos);
                if (maybe_member != null)
                {
                    return maybe_member;
                }
                maybe_last_ord_pos_seen += Array__node__count(node.Pred_Members);
                if (ord_pos <= maybe_last_ord_pos_seen)
                {
                    return null;
                }
            }
            if (node.Local_Widest_Type != Widest_Component_Type.None)
            {
                Int64 local_member_count = Array__node__local_member_count(node);
                if (ord_pos <= (maybe_last_ord_pos_seen + local_member_count))
                {
                    Int64 ord_pos_within_local = ord_pos - (1 + maybe_last_ord_pos_seen);
                    if (node.Local_Multiplicity > 1)
                    {
                        Int64 non_multiplied_local_count
                            = local_member_count / node.Local_Multiplicity;
                        ord_pos_within_local
                            = ord_pos_within_local % non_multiplied_local_count;
                    }
                    switch (node.Local_Widest_Type)
                    {
                        case Widest_Component_Type.Unrestricted:
                            return node.Local_Unrestricted_Members[(Int32)ord_pos_within_local];
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
                else
                {
                    maybe_last_ord_pos_seen += local_member_count;
                }
            }
            if (node.Succ_Members != null)
            {
                Int64 ord_pos_within_succ = ord_pos - (1 + maybe_last_ord_pos_seen);
                return Array__node__maybe_at(node.Succ_Members, ord_pos_within_succ);
            }
            return null;
        }
    }
}
