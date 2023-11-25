namespace Muldis.Data_Engine_Reference.Internal;

// Muldis.Data_Engine_Reference.Internal.Well_Known_Base_Type
// Enumerates Muldis Data Language base types that are considered well-known to
// this Muldis Data Language implementation, and that in particular have
// their own dedicated handling code or formats in the implementation.
// Every one of these types is either disjoint from or a proper subtype
// or proper supertype of each of the others of these types, and in
// particular many of these are proper subtypes of Article; however,
// every Muldis Data Language value is considered to have a best fit into exactly
// one of these types and that value is expected to be normalized
// towards its best fit storage format.

internal enum Well_Known_Base_Type
{
    MDL_Ignorance,
    MDL_False,
    MDL_True,
    MDL_Integer,
    MDL_Fraction,
    MDL_Bits,
    MDL_Blob,
    MDL_Text,
    MDL_Array,
    MDL_Set,
    MDL_Bag,
    MDL_Tuple_Array,
    MDL_Relation,
    MDL_Tuple_Bag,
    MDL_Heading,
    MDL_Tuple,
    MDL_Article,
    MDL_Variable,
    MDL_Process,
    MDL_Stream,
    MDL_External,
    MDL_Excuse,
}
