using System;
using System.Collections.Generic;

namespace Muldis.D.Ref_Eng.Core
{
    // Muldis.D.Ref_Eng.Core.MD_Well_Known_Type
    // Enumerates Muldis D types that are considered well-known to this
    // Muldis D language implementation.  Typically these are defined in
    // the Muldis D Foundation language spec even if most of their related
    // operators are not.  Typically each one is either equal to one of the
    // 7 Muldis D Foundation types or it is a Capsule subtype whose label
    // is a simple unqualified Attr_Name.  Typically a given Muldis D value
    // would be a member of several of these types rather than just one.
    // The Muldis D "Any" type is omitted as it implicitly always applies;
    // this omission is subject to be changed.

    internal enum MD_Well_Known_Type
    {
        Excuse,
        Boolean,
        Integer,
        Fraction,
        Bits,
        Blob,
        Text,
        Text__Unicode,
        Text__ASCII,
        Array,
        String,
        Set,
        Bag,
        Tuple,
        Tuple_Array,
        Relation,
        Tuple_Bag,
        Interval,
        Capsule,
        Handle,
        Variable,
        Process,
        Stream,
        External,
        Package,
        Function,
        Procedure,
        Signature,
        Signature__Conjunction,
        Signature__Disjunction,
        Signature__Tuple_Attrs_Match_Simple,
        Signature__Tuple_Attrs_Match,
        Signature__Capsule_Match,
        Expression,
        Literal,
        Args,
        Evaluates,
        Array_Selector,
        Set_Selector,
        Bag_Selector,
        Tuple_Selector,
        Capsule_Selector,
        If_Then_Else_Expr,
        And_Then,
        Or_Else,
        Given_When_Default_Expr,
        Guard,
        Factorization,
        Expansion,
        Vars,
        New,
        Current,
        Statement,
        Declare,
        Performs,
        If_Then_Else_Stmt,
        Given_When_Default_Stmt,
        Block,
        Leave,
        Iterate,
        Heading,
        Attr_Name,
        Attr_Name_List,
        Local_Name,
        Absolute_Name,
        Routine_Call,
        Function_Call,
        Function_Call_But_0,
        Function_Call_But_0_1,
        Procedure_Call,
    };

    internal static class Constants
    {
        internal static String[] Strings__Well_Known_Excuses()
        {
            return new String[] {
                "No_Reason",
                "Before_All_Others",
                "After_All_Others",
                "Div_By_Zero",
                "Zero_To_The_Zero",
                "No_Empty_Value",
                "No_Such_Ord_Pos",
                "No_Such_Attr_Name",
                "Not_Same_Heading",
            };
        }
    }
}
