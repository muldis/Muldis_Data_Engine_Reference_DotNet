namespace Muldis.Data_Engine_Reference;

// Muldis.Data_Engine_Reference.Internal_Well_Known_Base_Type
// Enumerates Muldis Data Language base types that are considered well-known to
// this Muldis Data Language implementation, and that in particular have
// their own dedicated handling code or formats in the implementation.
// Every one of these types is either disjoint from or a proper subtype
// or proper supertype of each of the others of these types, and in
// particular many of these are proper subtypes of Article; however,
// every Muldis Data Language value is considered to have a best fit into exactly
// one of these types and that value is expected to be normalized
// towards its best fit storage format.

internal enum Internal_Well_Known_Base_Type
{
    MDER_Ignorance,
    MDER_False,
    MDER_True,
    MDER_Integer,
    MDER_Fraction,
    MDER_Bits,
    MDER_Blob,
    MDER_Text,
    MDER_Nesting,
    MDER_Pair,
    MDER_Lot,
    MDER_Array,
    MDER_Set,
    MDER_Bag,
    MDER_Mix,
    MDER_Interval,
    MDER_Interval_Set,
    MDER_Interval_Bag,
    MDER_Heading,
    MDER_Kit,
    MDER_Tuple,
    MDER_Renaming,
    MDER_Tuple_Array,
    MDER_Relation,
    MDER_Tuple_Bag,
    MDER_Article,
    MDER_Excuse,
    MDER_Variable,
    MDER_Process,
    MDER_Stream,
    MDER_External,
}
