using System.Collections;
using System.Numerics;

namespace Muldis.Data_Engine_Reference;

internal class Internal_Executor
{
    internal readonly Internal_Memory memory;

    internal readonly Internal_Standard_Generator standard_generator;

    internal Internal_Executor(Internal_Memory memory)
    {
        this.memory = memory;

        this.standard_generator = new Internal_Standard_Generator();
    }

    internal MDER_Any Evaluates(MDER_Any function, MDER_Any args = null)
    {
        Internal_Memory m = this.memory;
        if (args is null)
        {
            args = m.MDER_Tuple_D0;
        }
        if (args.WKBT != Internal_Well_Known_Base_Type.MDER_Tuple)
        {
            throw new ArgumentException(m.Simple_MDER_Excuse(
                "X_Args_Not_Tuple").ToString());
        }
        MDER_Tuple args_as_Tuple = (MDER_Tuple)args;
        if (Is_Absolute_Name(function))
        {
            if (Object.ReferenceEquals(
                Array__maybe_at((MDER_Array)function, 0), m.MDER_Attr_Name("foundation")))
            {
                return evaluate_foundation_function(
                    (MDER_Text)Array__maybe_at((MDER_Array)function, 1), args_as_Tuple);
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
        throw new ArgumentException(m.Simple_MDER_Excuse(
            "X_Malformed_Function_Call").ToString());
    }

    internal void Performs(MDER_Any procedure, MDER_Any args = null)
    {
        Internal_Memory m = this.memory;
        if (args is null)
        {
            args = m.MDER_Tuple_D0;
        }
        if (args.WKBT != Internal_Well_Known_Base_Type.MDER_Tuple)
        {
            throw new ArgumentException(m.Simple_MDER_Excuse("X_Args_Not_Tuple").ToString());
        }
        MDER_Tuple args_as_Tuple = (MDER_Tuple)args;
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
        throw new ArgumentException(m.Simple_MDER_Excuse(
            "X_Malformed_Procedure_Call").ToString());
    }

    internal Boolean Is_Excuse(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Fraction(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Bits(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Blob(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Text(MDER_Any value)
    {
        return value.WKBT == Internal_Well_Known_Base_Type.MDER_Text;
    }

    internal Boolean Is_String(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Set(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Tuple_Array(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Relation(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Tuple_Bag(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Interval(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Package(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Function(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Procedure(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Function_Call(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Procedure_Call(MDER_Any value)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Heading(MDER_Any value)
    {
        return value.WKBT == Internal_Well_Known_Base_Type.MDER_Heading;
    }

    internal Boolean Is_Attr_Name_List(MDER_Any value)
    {
        Internal_Memory m = this.memory;
        if (value.WKBT != Internal_Well_Known_Base_Type.MDER_Array)
        {
            return false;
        }
        // TODO: Replace this loop with an "All" higher order function call.
        Int64 count = Array__count((MDER_Array)value);
        for (Int64 i = 0; i < count; i++)
        {
            if (!Is_Text(Array__maybe_at((MDER_Array)value, i)))
            {
                return false;
            }
        }
        return true;
    }

    internal Boolean Is_Local_Name(MDER_Any value)
    {
        if (!Is_Attr_Name_List(value))
        {
            return false;
        }
        Int64 count = Array__count((MDER_Array)value);
        if (count == 0)
        {
            return false;
        }
        MDER_Any first = Array__maybe_at((MDER_Array)value, 0);
        if ((Object.ReferenceEquals(first, this.memory.MDER_Attr_Name("foundation")) && count == 2)
            || (Object.ReferenceEquals(first, this.memory.MDER_Attr_Name("used")) && count >= 2)
            || (Object.ReferenceEquals(first, this.memory.MDER_Attr_Name("package")) && count >= 1)
            || (Object.ReferenceEquals(first, this.memory.MDER_Attr_Name("folder")) && count >= 1)
            || (Object.ReferenceEquals(first, this.memory.MDER_Attr_Name("material")) && count == 1)
            || (Object.ReferenceEquals(first, this.memory.MDER_Attr_Name("floating")) && count >= 2))
        {
            return true;
        }
        return false;
    }

    internal Boolean Is_Absolute_Name(MDER_Any value)
    {
        if (!Is_Local_Name(value))
        {
            return false;
        }
        MDER_Any first = Array__maybe_at((MDER_Array)value, 0);
        if (Object.ReferenceEquals(first, this.memory.MDER_Attr_Name("foundation"))
            || Object.ReferenceEquals(first, this.memory.MDER_Attr_Name("used"))
            || Object.ReferenceEquals(first, this.memory.MDER_Attr_Name("package")))
        {
            return true;
        }
        return false;
    }

    internal MDER_Any evaluate_foundation_function(MDER_Text func_name, MDER_Tuple args)
    {
        Internal_Memory m = this.memory;
        String func_name_s = func_name.code_point_members;

        // TYPE DEFINERS

        if (Internal_Constants.Strings__Foundation_Type_Definer_Function_Names().Contains(func_name_s))
        {
            if (!m.Tuple__Has_Heading(args, m.MDER_Heading_0))
            {
                throw new ArgumentException(m.Simple_MDER_Excuse(
                    "X_Type_Definer_Function_Args_Not_MDER_Heading_0").ToString());
            }
            MDER_Any v = args.attrs["\u0000"];
            switch (func_name_s)
            {
                case "Any":
                    return m.MDER_True();
                case "None":
                    return m.MDER_False();

                // FOUNDATION BASE TYPE DEFINERS

                case "Ignorance":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_Ignorance
                    );
                case "Boolean":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_False
                            || v.WKBT == Internal_Well_Known_Base_Type.MDER_True
                    );
                case "Integer":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_Integer
                    );
                case "Array":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_Array
                    );
                case "Bag":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_Bag
                    );
                case "Tuple":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_Tuple
                    );
                case "Article":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_Article
                    );
                case "Handle":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_Variable
                            || v.WKBT == Internal_Well_Known_Base_Type.MDER_Process
                            || v.WKBT == Internal_Well_Known_Base_Type.MDER_Stream
                            || v.WKBT == Internal_Well_Known_Base_Type.MDER_External
                    );

                // HANDLE SUBTYPE DEFINERS

                case "Variable":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_Variable
                    );
                case "Process":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_Process
                    );
                case "Stream":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_Stream
                    );
                case "External":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_External
                    );

                // ARRAY SUBTYPE DEFINERS

                // TUPLE SUBTYPE DEFINERS

                // ARTICLE SUBTYPE DEFINERS

                case "Blob":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_Blob
                    );

                case "Text":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_Text
                    );

                case "Excuse":
                    return m.MDER_Boolean(
                        v.WKBT == Internal_Well_Known_Base_Type.MDER_Excuse
                    );

                default:
                    throw new NotImplementedException();
            }
        }

        // NON TYPE DEFINER UNARY FUNCTIONS

        if (Internal_Constants.Strings__Foundation_NTD_Unary_Function_Names().Contains(func_name_s))
        {
            if (!m.Tuple__Has_Heading(args, m.MDER_Heading_0))
            {
                throw new ArgumentException(m.Simple_MDER_Excuse(
                    "X_Non_Type_Definer_Unary_Function_Args_Not_MDER_Heading_0").ToString());
            }
            MDER_Any v = args.attrs["\u0000"];
            switch (func_name_s)
            {
                case "Integer_opposite":
                    if (v.WKBT != Internal_Well_Known_Base_Type.MDER_Integer)
                    {
                        throw new ArgumentException(m.Simple_MDER_Excuse(
                            "X_Integer_opposite_Arg_0_Not_Integer").ToString());
                    }
                    return m.MDER_Integer(-((MDER_Integer)v).as_BigInteger);
                case "Integer_modulus":
                    if (v.WKBT != Internal_Well_Known_Base_Type.MDER_Integer)
                    {
                        throw new ArgumentException(m.Simple_MDER_Excuse(
                            "X_Integer_modulus_Arg_0_Not_Integer").ToString());
                    }
                    return m.MDER_Integer(BigInteger.Abs(((MDER_Integer)v).as_BigInteger));
                case "Integer_factorial":
                    if (v.WKBT != Internal_Well_Known_Base_Type.MDER_Integer
                        || ((MDER_Integer)v).as_BigInteger < 0)
                    {
                        throw new ArgumentException(m.Simple_MDER_Excuse(
                            "X_Integer_factorial_Arg_0_Not_Integer_NN").ToString());
                    }
                    // Note that System.Numerics.BigInteger doesn't come
                    // with a Factorial(n) so we have to do it ourselves.
                    return m.MDER_Integer(Integer__factorial(((MDER_Integer)v).as_BigInteger));
                case "Array_count":
                    if (v.WKBT != Internal_Well_Known_Base_Type.MDER_Array)
                    {
                        throw new ArgumentException(m.Simple_MDER_Excuse(
                            "X_Array_count_Arg_0_Not_Array").ToString());
                    }
                    return m.MDER_Integer(Array__count((MDER_Array)v));
                case "Bag_unique":
                    if (v.WKBT != Internal_Well_Known_Base_Type.MDER_Bag)
                    {
                        throw new ArgumentException(m.Simple_MDER_Excuse(
                            "X_Bag_unique_Arg_0_Not_Bag").ToString());
                    }
                    return new MDER_Bag(m,
                        new MDER_Bag_Struct {
                            local_symbolic_type = Symbolic_Bag_Type.Unique,
                            members = ((MDER_Bag)v).tree_root_node,
                            cached_members_meta = new Cached_Members_Meta {
                                tree_all_unique = true,
                            },
                        }
                    );
                case "Tuple_degree":
                    if (v.WKBT != Internal_Well_Known_Base_Type.MDER_Tuple)
                    {
                        throw new ArgumentException(m.Simple_MDER_Excuse(
                            "X_Tuple_degree_Arg_0_Not_Tuple").ToString());
                    }
                    return m.MDER_Integer(((MDER_Tuple)v).attrs.Count);
                case "Tuple_heading":
                    if (v.WKBT != Internal_Well_Known_Base_Type.MDER_Tuple)
                    {
                        throw new ArgumentException(m.Simple_MDER_Excuse(
                            "X_Tuple_heading_Arg_0_Not_Tuple").ToString());
                    }
                    return m.Tuple__Heading((MDER_Tuple)v);
                case "Text_from_UTF_8_Blob":
                    if (v.WKBT != Internal_Well_Known_Base_Type.MDER_Blob)
                    {
                        throw new ArgumentException(m.Simple_MDER_Excuse(
                            "X_Text_from_UTF_8_Blob_Arg_0_Not_Blob").ToString());
                    }
                    return m.MDER_Text_from_UTF_8_MDER_Blob((MDER_Blob)v);
                case "MDPT_Parsing_Unit_Text_to_Any":
                    if (v.WKBT != Internal_Well_Known_Base_Type.MDER_Text)
                    {
                        throw new ArgumentException(m.Simple_MDER_Excuse(
                            "X_MDPT_Parsing_Unit_Text_to_Any_Arg_0_Not_Text").ToString());
                    }
                    // TODO: Everything.
                    return m.MDER_Ignorance();
                default:
                    throw new NotImplementedException();
            }
        }

        // BINARY FUNCTIONS

        if (Internal_Constants.Strings__Foundation_Binary_Function_Names().Contains(func_name_s))
        {
            if (!m.Tuple__Has_Heading(args, m.MDER_Heading_0_1))
            {
                throw new ArgumentException(m.Simple_MDER_Excuse(
                    "X_Binary_Function_Args_Not_MDER_Heading_0_1").ToString());
            }
            MDER_Any a0 = args.attrs["\u0000"];
            MDER_Any a1 = args.attrs["\u0001"];
            switch (func_name_s)
            {
                case "same":
                    return m.MDER_Boolean(Any__same(a0, a1));
                case "Integer_plus":
                    if (a0.WKBT != Internal_Well_Known_Base_Type.MDER_Integer)
                    {
                        throw new ArgumentException(m.Simple_MDER_Excuse(
                            "X_Integer_plus_Arg_0_Not_Integer").ToString());
                    }
                    if (a1.WKBT != Internal_Well_Known_Base_Type.MDER_Integer)
                    {
                        throw new ArgumentException(m.Simple_MDER_Excuse(
                            "X_Integer_plus_Arg_1_Not_Integer").ToString());
                    }
                    return m.MDER_Integer(((MDER_Integer)a0).as_BigInteger + ((MDER_Integer)a1).as_BigInteger);
                case "Array_at":
                    if (a0.WKBT != Internal_Well_Known_Base_Type.MDER_Array)
                    {
                        throw new ArgumentException(m.Simple_MDER_Excuse(
                            "X_Array_at_Arg_0_Not_Array").ToString());
                    }
                    if (a1.WKBT != Internal_Well_Known_Base_Type.MDER_Integer
                        || ((MDER_Integer)a1).as_BigInteger < 0)
                    {
                        throw new ArgumentException(m.Simple_MDER_Excuse(
                            "X_Array_at_Arg_1_Not_Integer_NN").ToString());
                    }
                    MDER_Any maybe_member = Array__maybe_at((MDER_Array)a0, (Int64)((MDER_Integer)a1).as_BigInteger);
                    if (maybe_member is null)
                    {
                        throw new ArgumentException(
                            m.well_known_excuses["No_Such_Ord_Pos"].ToString());
                    }
                    return maybe_member;
                default:
                    throw new NotImplementedException();
            }
        }

        // TERNARY FUNCTIONS

        if (Internal_Constants.Strings__Foundation_Ternary_Function_Names().Contains(func_name_s))
        {
            if (!m.Tuple__Has_Heading(args, m.MDER_Heading_0_1_2))
            {
                throw new ArgumentException(m.Simple_MDER_Excuse(
                    "X_Ternary_Function_Args_Not_MDER_Heading_0_1_2").ToString());
            }
            MDER_Any v = args.attrs["\u0000"];
            switch (func_name_s)
            {
                default:
                    throw new NotImplementedException();
            }
        }

        throw new NotImplementedException();
    }

    internal Boolean Any__same(MDER_Any a0, MDER_Any a1)
    {
        if (Object.ReferenceEquals(a0, a1))
        {
            // We should always get here for any singleton well known base types:
            // MDER_Ignorance, MDER_False, MDER_True.
            return true;
        }
        if (a0.WKBT != a1.WKBT)
        {
            return false;
        }
        if (a0.cached_MDER_Any_identity is not null
            && a1.cached_MDER_Any_identity is not null)
        {
            if (String.Equals(a0.cached_MDER_Any_identity, a1.cached_MDER_Any_identity))
            {
                merge_two_same(a0, a1);
                return true;
            }
            return false;
        }
        Boolean result;
        switch (a0.WKBT)
        {
            case Internal_Well_Known_Base_Type.MDER_Ignorance:
            case Internal_Well_Known_Base_Type.MDER_False:
            case Internal_Well_Known_Base_Type.MDER_True:
                // We should never get here.
                throw new InvalidOperationException();
            case Internal_Well_Known_Base_Type.MDER_Integer:
                result = (((MDER_Integer)a0).as_BigInteger == ((MDER_Integer)a1).as_BigInteger);
                break;
            case Internal_Well_Known_Base_Type.MDER_Fraction:
                MDER_Fraction fa0 = (MDER_Fraction)a0;
                MDER_Fraction fa1 = (MDER_Fraction)a1;
                if (fa0.maybe_as_Decimal() is not null && fa1.maybe_as_Decimal() is not null)
                {
                    result = (fa0.maybe_as_Decimal() == fa1.maybe_as_Decimal());
                    break;
                }
                if (fa0.denominator() == fa1.denominator())
                {
                    result = (fa0.numerator() == fa1.numerator());
                    break;
                }
                fa0.ensure_coprime();
                fa1.ensure_coprime();
                result = (fa0.denominator() == fa1.denominator()
                    && fa0.numerator() == fa1.numerator());
                break;
            case Internal_Well_Known_Base_Type.MDER_Bits:
                result = Enumerable.SequenceEqual(
                    BitArray_to_List(((MDER_Bits)a0).bit_members),
                    BitArray_to_List(((MDER_Bits)a1).bit_members));
                break;
            case Internal_Well_Known_Base_Type.MDER_Blob:
                result = Enumerable.SequenceEqual(
                    ((MDER_Blob)a0).octet_members,
                    ((MDER_Blob)a1).octet_members);
                break;
            case Internal_Well_Known_Base_Type.MDER_Text:
                result = String.Equals(((MDER_Text)a0).code_point_members,
                    ((MDER_Text)a1).code_point_members);
                break;
            case Internal_Well_Known_Base_Type.MDER_Array:
                this.memory.Array__Collapse((MDER_Array)a0);
                this.memory.Array__Collapse((MDER_Array)a1);
                result = this.collapsed_Array_Struct_same(
                    ((MDER_Array)a0).tree_root_node, ((MDER_Array)a0).tree_root_node);
                break;
            case Internal_Well_Known_Base_Type.MDER_Set:
                // MDER_Set and MDER_Bag have the same internal representation.
                this.memory.Set__Collapse(set: ((MDER_Set)a0), want_indexed: true);
                this.memory.Set__Collapse(set: ((MDER_Set)a1), want_indexed: true);
                result = this.collapsed_Bag_Struct_same(
                    ((MDER_Set)a0).tree_root_node, ((MDER_Set)a0).tree_root_node);
                break;
            case Internal_Well_Known_Base_Type.MDER_Bag:
                // MDER_Set and MDER_Bag have the same internal representation.
                this.memory.Bag__Collapse(bag: ((MDER_Bag)a0), want_indexed: true);
                this.memory.Bag__Collapse(bag: ((MDER_Bag)a1), want_indexed: true);
                result = this.collapsed_Bag_Struct_same(
                    ((MDER_Set)a0).tree_root_node, ((MDER_Set)a0).tree_root_node);
                break;
            case Internal_Well_Known_Base_Type.MDER_Heading:
                HashSet<String> atnms0 = ((MDER_Heading)a0).attr_names;
                HashSet<String> atnms1 = ((MDER_Heading)a1).attr_names;
                return (atnms0.Count == atnms1.Count)
                    && Enumerable.All(atnms0, atnm => atnms1.Contains(atnm));
            case Internal_Well_Known_Base_Type.MDER_Tuple:
                Dictionary<String, MDER_Any> attrs0 = ((MDER_Tuple)a0).attrs;
                Dictionary<String, MDER_Any> attrs1 = ((MDER_Tuple)a1).attrs;
                // First test just that the Tuple headings are the same,
                // and only if they are, compare the attribute values.
                return (attrs0.Count == attrs1.Count)
                    && Enumerable.All(attrs0, attr => attrs1.ContainsKey(attr.Key))
                    && Enumerable.All(attrs0, attr => Any__same(attr.Value, attrs1[attr.Key]));
            case Internal_Well_Known_Base_Type.MDER_Tuple_Array:
                result = Any__same(((MDER_Tuple_Array)a0).heading, ((MDER_Tuple_Array)a1).heading)
                      && Any__same(((MDER_Tuple_Array)a0).body, ((MDER_Tuple_Array)a1).body);
                break;
            case Internal_Well_Known_Base_Type.MDER_Relation:
                result = Any__same(((MDER_Relation)a0).heading, ((MDER_Relation)a1).heading)
                      && Any__same(((MDER_Relation)a0).body, ((MDER_Relation)a1).body);
                break;
            case Internal_Well_Known_Base_Type.MDER_Tuple_Bag:
                result = Any__same(((MDER_Tuple_Bag)a0).heading, ((MDER_Tuple_Bag)a1).heading)
                      && Any__same(((MDER_Tuple_Bag)a0).body, ((MDER_Tuple_Bag)a1).body);
                break;
            case Internal_Well_Known_Base_Type.MDER_Article:
                result = Any__same(((MDER_Article)a0).label, ((MDER_Article)a1).label)
                      && Any__same(((MDER_Article)a0).attrs, ((MDER_Article)a1).attrs);
                break;
            case Internal_Well_Known_Base_Type.MDER_Excuse:
                result = Any__same(((MDER_Excuse)a0).label, ((MDER_Excuse)a1).label)
                      && Any__same(((MDER_Excuse)a0).attrs, ((MDER_Excuse)a1).attrs);
                break;
            case Internal_Well_Known_Base_Type.MDER_Variable:
            case Internal_Well_Known_Base_Type.MDER_Process:
            case Internal_Well_Known_Base_Type.MDER_Stream:
            case Internal_Well_Known_Base_Type.MDER_External:
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

    private List<Boolean> BitArray_to_List(BitArray value)
    {
        System.Collections.IEnumerator e = value.GetEnumerator();
        List<Boolean> list = new List<Boolean>();
        while (e.MoveNext())
        {
            list.Add((Boolean)e.Current);
        }
        return list;
    }

    private Boolean collapsed_Array_Struct_same(
        MDER_Array_Struct n0, MDER_Array_Struct n1)
    {
        if (Object.ReferenceEquals(n0, n1))
        {
            return true;
        }
        if (n0.local_symbolic_type == Symbolic_Array_Type.None
            && n1.local_symbolic_type == Symbolic_Array_Type.None)
        {
            // In theory we should never get here assuming that
            // the empty Array is optimized to return a constant
            // at selection time, but we will check anyway.
            return true;
        }
        if (n0.local_symbolic_type == Symbolic_Array_Type.Singular
            && n1.local_symbolic_type == Symbolic_Array_Type.Singular)
        {
            return ((n0.Local_Singular_Members().multiplicity
                    == n1.Local_Singular_Members().multiplicity)
                && Any__same(n0.Local_Singular_Members().member,
                    n1.Local_Singular_Members().member));
        }
        if (n0.local_symbolic_type == Symbolic_Array_Type.Arrayed
            && n1.local_symbolic_type == Symbolic_Array_Type.Arrayed)
        {
            // This works because MDER_Any Equals() calls Any__Same().
            return Enumerable.SequenceEqual(
                n0.Local_Arrayed_Members(),
                n1.Local_Arrayed_Members());
        }
        MDER_Array_Struct n0_ = n0;
        MDER_Array_Struct n1_ = n1;
        if (n0_.local_symbolic_type == Symbolic_Array_Type.Arrayed
            && n1_.local_symbolic_type == Symbolic_Array_Type.Singular)
        {
             n1_ = n0;
             n0_ = n1;
        }
        if (n0_.local_symbolic_type == Symbolic_Array_Type.Singular
            && n1_.local_symbolic_type == Symbolic_Array_Type.Arrayed)
        {
            Multiplied_Member sm = n0_.Local_Singular_Members();
            List<MDER_Any> am = n1_.Local_Arrayed_Members();
            return sm.multiplicity == am.Count
                && Enumerable.All(am, m => Any__same(m, sm.member));
        }
        // We should never get here.
        throw new NotImplementedException();
    }

    private Boolean collapsed_Bag_Struct_same(
        MDER_Bag_Struct n0, MDER_Bag_Struct n1)
    {
        if (Object.ReferenceEquals(n0, n1))
        {
            return true;
        }
        if (n0.local_symbolic_type == Symbolic_Bag_Type.None
            && n1.local_symbolic_type == Symbolic_Bag_Type.None)
        {
            // In theory we should never get here assuming that
            // the empty Array is optimized to return a constant
            // at selection time, but we will check anyway.
            return true;
        }
        if (n0.local_symbolic_type == Symbolic_Bag_Type.Indexed
            && n1.local_symbolic_type == Symbolic_Bag_Type.Indexed)
        {
            Dictionary<MDER_Any, Multiplied_Member> im0
                = n0.Local_Indexed_Members();
            Dictionary<MDER_Any, Multiplied_Member> im1
                = n1.Local_Indexed_Members();
            return im0.Count == im1.Count
                && Enumerable.All(
                    im0.Values,
                    m => im1.ContainsKey(m.member)
                        && im1[m.member].multiplicity == m.multiplicity
                );
        }
        // We should never get here.
        throw new NotImplementedException();
    }

    internal void merge_two_same(MDER_Any a0, MDER_Any a1)
    {
        // This may be run on 2 MDER_Any only if it was already
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

    internal Int64 Bits__count(MDER_Bits bits)
    {
        return bits.bit_members.Length;
    }

    internal Int64 Blob__count(MDER_Blob blob)
    {
        return blob.octet_members.Length;
    }

    internal Int64 Text__count(MDER_Text node)
    {
        if (node.cached_member_count is null)
        {
            if (node.has_any_non_BMP)
            {
                String s = node.code_point_members;
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
                node.cached_member_count = count;
            }
            else
            {
                node.cached_member_count = node.code_point_members.Length;
            }
        }
        return (Int64)node.cached_member_count;
    }

    internal Int64 Array__count(MDER_Array array)
    {
        return Array__node__tree_member_count(array.tree_root_node);
    }

    private Int64 Array__node__tree_member_count(MDER_Array_Struct node)
    {
        if (node.cached_members_meta.tree_member_count is null)
        {
            switch (node.local_symbolic_type)
            {
                case Symbolic_Array_Type.None:
                    node.cached_members_meta.tree_member_count = 0;
                    break;
                case Symbolic_Array_Type.Singular:
                    node.cached_members_meta.tree_member_count
                        = node.Local_Singular_Members().multiplicity;
                    break;
                case Symbolic_Array_Type.Arrayed:
                    node.cached_members_meta.tree_member_count
                        = node.Local_Arrayed_Members().Count;
                    break;
                case Symbolic_Array_Type.Catenated:
                    node.cached_members_meta.tree_member_count
                        = Array__node__tree_member_count(node.Tree_Catenated_Members().a0)
                        + Array__node__tree_member_count(node.Tree_Catenated_Members().a1);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        return (Int64)node.cached_members_meta.tree_member_count;
    }

    private MDER_Integer Bits__maybe_at(MDER_Bits bits, Int64 ord_pos)
    {
        return this.memory.MDER_Integer(bits.bit_members[(Int32)ord_pos] ? 1 : 0);
    }

    private MDER_Integer Blob__maybe_at(MDER_Blob blob, Int64 ord_pos)
    {
        return this.memory.MDER_Integer(blob.octet_members[(Int32)ord_pos]);
    }

    private MDER_Integer Text__maybe_at(MDER_Text node, Int64 ord_pos)
    {
        if (node.has_any_non_BMP)
        {
            String s = node.code_point_members;
            Int64 logical_i = 0;
            for (Int32 i = 0; i < s.Length; i++)
            {
                if (logical_i == ord_pos)
                {
                    if ((i+1) < s.Length
                        && Char.IsSurrogatePair(s[i], s[i+1]))
                    {
                        return this.memory.MDER_Integer(
                            Char.ConvertToUtf32(s[i], s[i+1]));
                    }
                    return this.memory.MDER_Integer(s[i]);
                }
                logical_i++;
                if ((i+1) < s.Length
                    && Char.IsSurrogatePair(s[i], s[i+1]))
                {
                    i++;
                }
            }
        }
        return this.memory.MDER_Integer(
            node.code_point_members[(Int32)ord_pos]);
    }

    private MDER_Any Array__maybe_at(MDER_Array array, Int64 ord_pos)
    {
        return Array__node__maybe_at(array.tree_root_node, ord_pos);
    }

    private MDER_Any Array__node__maybe_at(MDER_Array_Struct node, Int64 ord_pos)
    {
        if (node.cached_members_meta.tree_member_count == 0)
        {
            return null;
        }
        Int64 maybe_last_ord_pos_seen = -1;
        switch (node.local_symbolic_type)
        {
            case Symbolic_Array_Type.None:
                return null;
            case Symbolic_Array_Type.Singular:
                if (ord_pos <= (maybe_last_ord_pos_seen + node.Local_Singular_Members().multiplicity))
                {
                    return node.Local_Singular_Members().member;
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
                MDER_Any maybe_member = Array__node__maybe_at(node.Tree_Catenated_Members().a0, ord_pos);
                if (maybe_member is not null)
                {
                    return maybe_member;
                }
                maybe_last_ord_pos_seen += Array__node__tree_member_count(node.Tree_Catenated_Members().a0);
                if (ord_pos <= maybe_last_ord_pos_seen)
                {
                    return null;
                }
                Int64 ord_pos_within_succ = ord_pos - (1 + maybe_last_ord_pos_seen);
                return Array__node__maybe_at(node.Tree_Catenated_Members().a1, ord_pos_within_succ);
            default:
                throw new NotImplementedException();
        }
    }
}
