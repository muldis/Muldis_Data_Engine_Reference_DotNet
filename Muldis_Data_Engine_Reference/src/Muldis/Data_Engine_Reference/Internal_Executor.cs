using System.Numerics;

namespace Muldis.Data_Engine_Reference;

internal class Internal_Executor
{
    private readonly MDER_Machine __machine;

    internal Internal_Executor(MDER_Machine machine)
    {
        this.__machine = machine;
    }

    internal MDER_Machine machine()
    {
        return this.__machine;
    }

    internal MDER_Any Evaluates(MDER_Any function, MDER_Any? args = null)
    {
        if (args is null)
        {
            args = this.__machine.MDER_Tuple_D0;
        }
        if (args._WKBT() != Internal_Well_Known_Base_Type.MDER_Tuple)
        {
            throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                "X_Args_Not_Tuple").ToString());
        }
        MDER_Tuple args_as_Tuple = (MDER_Tuple)args;
        if (Is_Absolute_Name(function))
        {
            if (Object.ReferenceEquals(
                ((MDER_Array)function).Array__maybe_at(0), this.__machine.MDER_Attr_Name("foundation")))
            {
                return evaluate_foundation_function(
                    (MDER_Text)((MDER_Array)function).Array__maybe_at(1)!, args_as_Tuple);
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
        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
            "X_Malformed_Function_Call").ToString());
    }

    internal void Performs(MDER_Any procedure, MDER_Any? args = null)
    {
        if (args is null)
        {
            args = this.__machine.MDER_Tuple_D0;
        }
        if (args._WKBT() != Internal_Well_Known_Base_Type.MDER_Tuple)
        {
            throw new ArgumentException(this.__machine.Simple_MDER_Excuse("X_Args_Not_Tuple").ToString());
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
        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
            "X_Malformed_Procedure_Call").ToString());
    }

    internal Boolean Is_Excuse(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Fraction(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Bits(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Blob(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Text(MDER_Any topic)
    {
        return topic._WKBT() == Internal_Well_Known_Base_Type.MDER_Text;
    }

    internal Boolean Is_String(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Set(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Tuple_Array(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Relation(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Tuple_Bag(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Interval(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Package(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Function(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Procedure(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Function_Call(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Procedure_Call(MDER_Any topic)
    {
        throw new NotImplementedException();
    }

    internal Boolean Is_Heading(MDER_Any topic)
    {
        return topic._WKBT() == Internal_Well_Known_Base_Type.MDER_Heading;
    }

    internal Boolean Is_Attr_Name_List(MDER_Any topic)
    {
        if (topic._WKBT() != Internal_Well_Known_Base_Type.MDER_Array)
        {
            return false;
        }
        // TODO: Replace this loop with an "All" higher order function call.
        Int64 count = ((MDER_Array)topic).Discrete__count();
        for (Int64 i = 0; i < count; i++)
        {
            if (!Is_Text(((MDER_Array)topic).Array__maybe_at(i)!))
            {
                return false;
            }
        }
        return true;
    }

    internal Boolean Is_Local_Name(MDER_Any topic)
    {
        if (!Is_Attr_Name_List(topic))
        {
            return false;
        }
        Int64 count = ((MDER_Array)topic).Discrete__count();
        if (count == 0)
        {
            return false;
        }
        MDER_Any first = ((MDER_Array)topic).Array__maybe_at(0)!;
        if ((Object.ReferenceEquals(first, this.__machine.MDER_Attr_Name("foundation")) && count == 2)
            || (Object.ReferenceEquals(first, this.__machine.MDER_Attr_Name("used")) && count >= 2)
            || (Object.ReferenceEquals(first, this.__machine.MDER_Attr_Name("package")) && count >= 1)
            || (Object.ReferenceEquals(first, this.__machine.MDER_Attr_Name("folder")) && count >= 1)
            || (Object.ReferenceEquals(first, this.__machine.MDER_Attr_Name("material")) && count == 1)
            || (Object.ReferenceEquals(first, this.__machine.MDER_Attr_Name("floating")) && count >= 2))
        {
            return true;
        }
        return false;
    }

    internal Boolean Is_Absolute_Name(MDER_Any topic)
    {
        if (!Is_Local_Name(topic))
        {
            return false;
        }
        MDER_Any first = ((MDER_Array)topic).Array__maybe_at(0)!;
        if (Object.ReferenceEquals(first, this.__machine.MDER_Attr_Name("foundation"))
            || Object.ReferenceEquals(first, this.__machine.MDER_Attr_Name("used"))
            || Object.ReferenceEquals(first, this.__machine.MDER_Attr_Name("package")))
        {
            return true;
        }
        return false;
    }

    internal MDER_Any evaluate_foundation_function(MDER_Text func_name, MDER_Tuple args)
    {
        String func_name_s = func_name.code_point_members_as_String();

        // TYPE DEFINERS

        if (Internal_Constants.Strings__Foundation_Type_Definer_Function_Names().Contains(func_name_s))
        {
            if (!this.Tuple__Has_Heading(args, this.__machine.MDER_Heading_0))
            {
                throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                    "X_Type_Definer_Function_Args_Not_MDER_Heading_0").ToString());
            }
            MDER_Any v = args.attrs["\u0000"];
            switch (func_name_s)
            {
                case "Any":
                    return this.__machine.MDER_True();
                case "None":
                    return this.__machine.MDER_False();

                // FOUNDATION BASE TYPE DEFINERS

                case "Ignorance":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_Ignorance
                    );
                case "Boolean":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_False
                            || v._WKBT() == Internal_Well_Known_Base_Type.MDER_True
                    );
                case "Integer":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_Integer
                    );
                case "Array":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_Array
                    );
                case "Bag":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_Bag
                    );
                case "Tuple":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_Tuple
                    );
                case "Article":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_Article
                    );
                case "Handle":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_Variable
                            || v._WKBT() == Internal_Well_Known_Base_Type.MDER_Process
                            || v._WKBT() == Internal_Well_Known_Base_Type.MDER_Stream
                            || v._WKBT() == Internal_Well_Known_Base_Type.MDER_External
                    );

                // HANDLE SUBTYPE DEFINERS

                case "Variable":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_Variable
                    );
                case "Process":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_Process
                    );
                case "Stream":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_Stream
                    );
                case "External":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_External
                    );

                // ARRAY SUBTYPE DEFINERS

                // TUPLE SUBTYPE DEFINERS

                // ARTICLE SUBTYPE DEFINERS

                case "Blob":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_Blob
                    );

                case "Text":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_Text
                    );

                case "Excuse":
                    return this.__machine.MDER_Boolean(
                        v._WKBT() == Internal_Well_Known_Base_Type.MDER_Excuse
                    );

                default:
                    throw new NotImplementedException();
            }
        }

        // NON TYPE DEFINER UNARY FUNCTIONS

        if (Internal_Constants.Strings__Foundation_NTD_Unary_Function_Names().Contains(func_name_s))
        {
            if (!this.Tuple__Has_Heading(args, this.__machine.MDER_Heading_0))
            {
                throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                    "X_Non_Type_Definer_Unary_Function_Args_Not_MDER_Heading_0").ToString());
            }
            MDER_Any v = args.attrs["\u0000"];
            switch (func_name_s)
            {
                case "Integer_opposite":
                    if (v._WKBT() != Internal_Well_Known_Base_Type.MDER_Integer)
                    {
                        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                            "X_Integer_opposite_Arg_0_Not_Integer").ToString());
                    }
                    return this.__machine.MDER_Integer(-((MDER_Integer)v).as_BigInteger());
                case "Integer_modulus":
                    if (v._WKBT() != Internal_Well_Known_Base_Type.MDER_Integer)
                    {
                        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                            "X_Integer_modulus_Arg_0_Not_Integer").ToString());
                    }
                    return this.__machine.MDER_Integer(BigInteger.Abs(((MDER_Integer)v).as_BigInteger()));
                case "Integer_factorial":
                    if (v._WKBT() != Internal_Well_Known_Base_Type.MDER_Integer
                        || ((MDER_Integer)v).as_BigInteger() < 0)
                    {
                        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                            "X_Integer_factorial_Arg_0_Not_Integer_NN").ToString());
                    }
                    // Note that System.Numerics.BigInteger doesn't come
                    // with a Factorial(n) so we have to do it ourselves.
                    return this.__machine.MDER_Integer(Integer__factorial(((MDER_Integer)v).as_BigInteger()));
                case "Array_count":
                    if (v._WKBT() != Internal_Well_Known_Base_Type.MDER_Array)
                    {
                        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                            "X_Array_count_Arg_0_Not_Array").ToString());
                    }
                    return this.__machine.MDER_Integer(((MDER_Array)v).Discrete__count());
                case "Bag_unique":
                    if (v._WKBT() != Internal_Well_Known_Base_Type.MDER_Bag)
                    {
                        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                            "X_Bag_unique_Arg_0_Not_Bag").ToString());
                    }
                    return new MDER_Bag(this.__machine,
                        new Internal_MDER_Discrete_Struct {
                            local_symbolic_type = Internal_Symbolic_Discrete_Type.Unique,
                            members = ((MDER_Bag)v).tree_root_node,
                            cached_tree_all_unique = true,
                        }
                    );
                case "Tuple_degree":
                    if (v._WKBT() != Internal_Well_Known_Base_Type.MDER_Tuple)
                    {
                        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                            "X_Tuple_degree_Arg_0_Not_Tuple").ToString());
                    }
                    return this.__machine.MDER_Integer(((MDER_Tuple)v).attrs.Count);
                case "Tuple_heading":
                    if (v._WKBT() != Internal_Well_Known_Base_Type.MDER_Tuple)
                    {
                        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                            "X_Tuple_heading_Arg_0_Not_Tuple").ToString());
                    }
                    return this.Tuple__Heading((MDER_Tuple)v);
                case "Text_from_UTF_8_Blob":
                    if (v._WKBT() != Internal_Well_Known_Base_Type.MDER_Blob)
                    {
                        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                            "X_Text_from_UTF_8_Blob_Arg_0_Not_Blob").ToString());
                    }
                    return this.MDER_Text_from_UTF_8_MDER_Blob((MDER_Blob)v);
                case "MDPT_Parsing_Unit_Text_to_Any":
                    if (v._WKBT() != Internal_Well_Known_Base_Type.MDER_Text)
                    {
                        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                            "X_MDPT_Parsing_Unit_Text_to_Any_Arg_0_Not_Text").ToString());
                    }
                    // TODO: Everything.
                    return this.__machine.MDER_Ignorance();
                default:
                    throw new NotImplementedException();
            }
        }

        // BINARY FUNCTIONS

        if (Internal_Constants.Strings__Foundation_Binary_Function_Names().Contains(func_name_s))
        {
            if (!this.Tuple__Has_Heading(args, this.__machine.MDER_Heading_0_1))
            {
                throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                    "X_Binary_Function_Args_Not_MDER_Heading_0_1").ToString());
            }
            MDER_Any a0 = args.attrs["\u0000"];
            MDER_Any a1 = args.attrs["\u0001"];
            switch (func_name_s)
            {
                case "same":
                    return this.__machine.MDER_Boolean(a0._MDER_Any__same(a1));
                case "Integer_plus":
                    if (a0._WKBT() != Internal_Well_Known_Base_Type.MDER_Integer)
                    {
                        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                            "X_Integer_plus_Arg_0_Not_Integer").ToString());
                    }
                    if (a1._WKBT() != Internal_Well_Known_Base_Type.MDER_Integer)
                    {
                        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                            "X_Integer_plus_Arg_1_Not_Integer").ToString());
                    }
                    return this.__machine.MDER_Integer(((MDER_Integer)a0).as_BigInteger() + ((MDER_Integer)a1).as_BigInteger());
                case "Array_at":
                    if (a0._WKBT() != Internal_Well_Known_Base_Type.MDER_Array)
                    {
                        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                            "X_Array_at_Arg_0_Not_Array").ToString());
                    }
                    if (a1._WKBT() != Internal_Well_Known_Base_Type.MDER_Integer
                        || ((MDER_Integer)a1).as_BigInteger() < 0)
                    {
                        throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
                            "X_Array_at_Arg_1_Not_Integer_NN").ToString());
                    }
                    MDER_Any maybe_member = ((MDER_Array)a0).Array__maybe_at((Int64)((MDER_Integer)a1).as_BigInteger())!;
                    if (maybe_member is null)
                    {
                        throw new ArgumentException(
                            this.__machine.well_known_excuses["No_Such_Ord_Pos"].ToString());
                    }
                    return maybe_member;
                default:
                    throw new NotImplementedException();
            }
        }

        // TERNARY FUNCTIONS

        if (Internal_Constants.Strings__Foundation_Ternary_Function_Names().Contains(func_name_s))
        {
            if (!this.Tuple__Has_Heading(args, this.__machine.MDER_Heading_0_1_2))
            {
                throw new ArgumentException(this.__machine.Simple_MDER_Excuse(
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

    private BigInteger Integer__factorial(BigInteger topic)
    {
        // Note that System.Numerics.BigInteger doesn't come
        // with a Factorial(n) so we have to do it ourselves.
        // Note we calculate this by iteration rather than recursion in
        // order to scale better ... unless the C# compiler is smart
        // enough to optimize tail recursion anyway.
        BigInteger result = 1;
        for (Int32 i = 2; i <= topic; i++)
        {
            result *= i;
        }
        return result;
    }

    internal MDER_Heading Tuple__Heading(MDER_Tuple tuple)
    {
        return this.__machine.MDER_Heading(new HashSet<String>(tuple.attrs.Keys));
    }

    internal MDER_Heading? Tuple__maybe_Heading(MDER_Tuple? tuple)
    {
        if (tuple is null)
        {
            return null;
        }
        return this.__machine.MDER_Heading(new HashSet<String>(tuple.attrs.Keys));
    }

    internal Boolean Tuple__Has_Heading(MDER_Tuple t, MDER_Heading h)
    {
        Dictionary<String, MDER_Any> attrs = t.attrs;
        HashSet<String> attr_names = h.attr_names;
        return (attrs.Count == attr_names.Count)
            && Enumerable.All(attr_names, attr_name => attrs.ContainsKey(attr_name));
    }

    internal MDER_Any MDER_Text_from_UTF_8_MDER_Blob(MDER_Blob blob)
    {
        Byte[] octets = blob._octet_members_as_Byte_array();
        String? code_points = Internal_Unicode.maybe_String_from_UTF_8_Byte_array(octets);
        if (code_points is null)
        {
            return this.__machine.Simple_MDER_Excuse("X_Unicode_Blob_Not_UTF_8");
        }
        Internal_Unicode_Test_Result tr = Internal_Unicode.test_String(code_points);
        if (tr == Internal_Unicode_Test_Result.Is_Malformed)
        {
            return this.__machine.Simple_MDER_Excuse("X_Unicode_Blob_Not_UTF_8");
        }
        return this.__machine._MDER_Text(
            code_points,
            (tr == Internal_Unicode_Test_Result.Valid_Has_Non_BMP)
        );
    }
}
