namespace Muldis.Data_Engine_Reference.Internal;

// Muldis.Data_Engine_Reference.Internal.Well_Known_Type
// Enumerates Muldis Data Language types that are considered well-known to this
// Muldis Data Language implementation.  Typically these are defined in
// the Muldis Data Language Foundation language spec even if most of their related
// operators are not.  Typically each one is an Article subtype.
// This set excludes on purpose the subset of well-known types that
// should be trivial to test membership of by other means; in
// particular it excludes {Any, None}, the Well_Known_Base_Type;
// types not excluded are more work to test.

internal enum Well_Known_Type
{
    Integer_NN,
    Integer_P,
    Text__ASCII,
    Tuple_Array,
    Relation,
    Tuple_Bag,
    Interval,
    Package,
    Function,
    Procedure,
    Signature,
    Signature__Conjunction,
    Signature__Disjunction,
    Signature__Tuple_Attrs_Match_Simple,
    Signature__Tuple_Attrs_Match,
    Signature__Article_Match,
    Expression,
    Literal,
    Args,
    Evaluates,
    Array_Selector,
    Set_Selector,
    Bag_Selector,
    Tuple_Selector,
    Article_Selector,
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
}
