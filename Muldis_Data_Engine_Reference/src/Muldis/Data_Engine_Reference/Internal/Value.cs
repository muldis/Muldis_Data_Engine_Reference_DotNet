using System;
using System.Collections.Generic;
using System.Numerics;

namespace Muldis.Data_Engine_Reference.Internal;

// Muldis.Data_Engine_Reference.Internal.MDL_Fraction_Struct
// When a Muldis.Data_Engine_Reference.Internal.MDL_Any is representing a MDL_Fraction,
// an MDL_Fraction_Struct is used by it to hold the MDL_Fraction-specific details.

internal class MDL_Fraction_Struct
{
    // The as_Decimal field is optionally valued if the MDL_Fraction
    // value is known small enough to fit in the range it can represent
    // and it is primarily used when the MDL_Fraction value was input to
    // the system as a .NET Decimal in the first place or was output
    // from the system as such; a MDL_Fraction input first as a Decimal
    // is only copied to the as_pair field when needed;
    // if both said fields are valued at once, they are redundant.
    internal Nullable<Decimal> as_Decimal;

    // The as_pair field, comprising a numerator+denominator field
    // pair, can represent any
    // MDL_Fraction value at all, such that the Fraction's value is
    // defined as the fractional division of numerator by denominator.
    // as_pair might not be defined if as_Decimal is defined.
    internal MDL_Fraction_Pair as_pair;

    internal void Ensure_Coprime()
    {
        Ensure_Pair();
        if (this.as_pair.cached_is_coprime == true)
        {
            return;
        }
        // Note that GreatestCommonDivisor() always has a non-negative result.
        BigInteger gcd = BigInteger.GreatestCommonDivisor(this.as_pair.numerator, this.as_pair.denominator);
        if (gcd > 1)
        {
            // Make the numerator and denominator coprime.
            this.as_pair.numerator   = this.as_pair.numerator   / gcd;
            this.as_pair.denominator = this.as_pair.denominator / gcd;
        }
        this.as_pair.cached_is_coprime = true;
    }

    internal void Ensure_Pair()
    {
        if (this.as_pair is not null)
        {
            return;
        }
        // If numerator+denominator are null, as_Decimal must not be.
        Int32[] dec_bits = Decimal.GetBits((Decimal)this.as_Decimal);
        // https://msdn.microsoft.com/en-us/library/system.decimal.getbits(v=vs.110).aspx
        // The GetBits spec says that it returns 4 32-bit integers
        // representing the 128 bits of the Decimal itself; of these,
        // the first 3 integers' bits give the mantissa,
        // the 4th integer's bits give the sign and the scale factor.
        // "Bits 16 to 23 must contain an exponent between 0 and 28,
        // which indicates the power of 10 to divide the integer number."
        Int32 scale_factor_int = ((dec_bits[3] >> 16) & 0x7F);
        Decimal denominator_dec = 1M;
        // Decimal doesn't have exponentiation op so we do it manually.
        for (Int32 i = 1; i <= scale_factor_int; i++)
        {
            denominator_dec = denominator_dec * 10M;
        }
        Decimal numerator_dec = (Decimal)this.as_Decimal * denominator_dec;
        as_pair = new MDL_Fraction_Pair {
            numerator = new BigInteger(numerator_dec),
            denominator = new BigInteger(denominator_dec),
            cached_is_terminating_decimal = true,
        };
    }

    internal Boolean Is_Terminating_Decimal()
    {
        if (this.as_Decimal is not null)
        {
            return true;
        }
        if (this.as_pair.cached_is_terminating_decimal is null)
        {
            Ensure_Coprime();
            Boolean found_all_2_factors = false;
            Boolean found_all_5_factors = false;
            BigInteger confirmed_quotient = this.as_pair.denominator;
            BigInteger attempt_quotient = 1;
            BigInteger attempt_remainder = 0;
            while (!found_all_2_factors)
            {
                attempt_quotient = BigInteger.DivRem(
                    confirmed_quotient, 2, out attempt_remainder);
                if (attempt_remainder > 0)
                {
                    found_all_2_factors = true;
                }
                else
                {
                    confirmed_quotient = attempt_quotient;
                }
            }
            while (!found_all_5_factors)
            {
                attempt_quotient = BigInteger.DivRem(
                    confirmed_quotient, 5, out attempt_remainder);
                if (attempt_remainder > 0)
                {
                    found_all_5_factors = true;
                }
                else
                {
                    confirmed_quotient = attempt_quotient;
                }
            }
            this.as_pair.cached_is_terminating_decimal = (confirmed_quotient == 1);
        }
        return (Boolean)this.as_pair.cached_is_terminating_decimal;
    }

    internal Int32 Pair_Decimal_Denominator_Scale()
    {
        if (this.as_pair is null || !Is_Terminating_Decimal())
        {
            throw new InvalidOperationException();
        }
        Ensure_Coprime();
        for (Int32 dec_scale = 0; dec_scale <= Int32.MaxValue; dec_scale++)
        {
            // BigInteger.Pow() can only take an Int32 exponent anyway.
            if (BigInteger.Remainder(BigInteger.Pow(10,dec_scale), this.as_pair.denominator) == 0)
            {
                return dec_scale;
            }
        }
        // If somehow the denominator can be big enough that we'd actually get here.
        throw new NotImplementedException();
    }
}

// Muldis.Data_Engine_Reference.Internal.MDL_Fraction_Pair
// Represents a numerator/denominator pair.

internal class MDL_Fraction_Pair
{
    // The numerator+denominator field pair can represent any
    // MDL_Fraction value at all, such that the Fraction's value is
    // defined as the fractional division of numerator by denominator.
    // denominator may never be zero; otherwise, the pair must always
    // be normalized at least such that the denominator is positive.
    internal BigInteger numerator;
    internal BigInteger denominator;

    // This is true iff we know that the numerator and
    // denominator are coprime (their greatest common divisor is 1);
    // this is false iff we know that they are not coprime.
    // While the pair typically need to be coprime in order to reliably
    // determine if 2 MDL_Fraction represent the same Muldis Data Language value,
    // we don't necessarily store them that way for efficiency sake.
    internal Nullable<Boolean> cached_is_coprime;

    // This is true iff we know that the MDL_Fraction value can be
    // represented as a terminating decimal number, meaning that the
    // prime factorization of the MDL_Fraction's coprime denominator
    // consists only of 2s and 5s; this is false iff we know that the
    // MDL_Fraction value would be an endlessly repeating decimal
    // number, meaning that the MDL_Fraction's coprime denominator has
    // at least 1 prime factor that is not a 2 or a 5.
    // The identity serialization of a MDL_Fraction uses a single decimal
    // number iff it would terminate and a coprime integer pair otherwise.
    // This field may be true even if cached_is_coprime isn't because
    // this MDL_Fraction_Pair was derived from a Decimal.
    internal Nullable<Boolean> cached_is_terminating_decimal;

    // Iff this field is defined, we ensure that both the current
    // MDL_Fraction_Struct has a denominator equal to it, and also that
    // any other MDL_Fraction_Struct derived from it has the same
    // denominator as well, iff the MDL_Fraction value can be exactly
    // represented by such a MDL_Fraction_Struct.
    // Having this field defined tends to suppress automatic efforts to
    // normalize the MDL_Fraction_Struct to a coprime state.
    // The main purpose of this field is to aid in system performance
    // when a lot of math, particularly addition and subtraction, is
    // done with rationals having a common conceptual fixed precision,
    // so that the performance is then closer to integer math.
    // internal Nullable<BigInteger> denominator_affinity;
}

// Muldis.Data_Engine_Reference.Internal.Symbolic_Array_Type
// Enumerates the various ways that a MDL_Array collection can be defined
// symbolically in terms of other collections.
// None means the collection simply has zero members.

internal enum Symbolic_Array_Type
{
    None,
    Singular,
    Arrayed,
    Catenated,
}

// Muldis.Data_Engine_Reference.Internal.MDL_Array_Struct
// When a Muldis.Data_Engine_Reference.Internal.MDL_Any is representing a MDL_Array,
// a MDL_Array_Struct is used by it to hold the MDL_Array-specific details.
// The "tree" is actually a uni-directional graph as multiple nodes can
// cite the same other conceptually immutable nodes as their children.
// MDL_Array members are only directly stored in leaf nodes of the graph.
// It is important to note that the same logical Muldis Data Language Array value
// may be represented by several different representation formats here,

internal class MDL_Array_Struct
{
    // Local Symbolic Type (LST) determines the role this node plays in
    // the tree.  Determines interpreting "members" field.
    internal Symbolic_Array_Type local_symbolic_type;

    // Members of this Muldis Data Language Array, in one of several possible
    // specialized representation formats depending on the data type.
    // Iff LST is None, this field is simply null.
        // This is a leaf node explicitly defining zero Array members.
        // Guarantees the Array has exactly zero members.
    // Iff LST is Singular, this field holds a Multiplied_Member.
        // This is a leaf node defining 1..N Array members, every one of
        // which is the same value, which can be of any type;
        // Singular is the basis for compactly representing either a
        // single-element Array or a repeating portion of a sparse Array.
        // Guarantees the Array has at least 1 member and that all
        // Array members are the same value.
    // Iff LST is Arrayed, this field holds a List<MDL_Any>.
        // This is a leaf node defining 2..N Array members, each of which
        // can be of any type; this is the most common format.
        // Guarantees the Array has at least 2 members but does not
        // guarantee that at least 2 members have distinct values.
    // Iff LST is Catenated, this field holds a MDL_Array_Pair.
        // This is a non-leaf node with 2 direct child Array nodes;
        // this Array's members are defined as the catenation of
        // the pair's a0 and a1 nodes in that order.
        // Makes no guarantees that the Array is none/singular/otherwise.
    internal Object members;

    // A cache of calculations about this Array's members.
    internal Cached_Members_Meta cached_members_meta;

    internal Multiplied_Member Local_Singular_Members()
    {
        return (Multiplied_Member)this.members;
    }

    internal List<MDL_Any> Local_Arrayed_Members()
    {
        return (List<MDL_Any>)this.members;
    }

    internal MDL_Array_Pair Tree_Catenated_Members()
    {
        return (MDL_Array_Pair)this.members;
    }
}

// Muldis.Data_Engine_Reference.Internal.MDL_Array_Pair
// Represents an ordered pair of MDL_Array, typically corresponding in
// order to the "0" and "1" conceptually-ordered arg/attr to a function.

internal class MDL_Array_Pair
{
    // This is the first conceptually-ordered arg/attr.
    internal MDL_Array_Struct a0;

    // This is the second conceptually-ordered arg/attr.
    internal MDL_Array_Struct a1;

    internal MDL_Array_Pair(MDL_Array_Struct a0, MDL_Array_Struct a1)
    {
        this.a0 = a0;
        this.a1 = a1;
    }
}

// Muldis.Data_Engine_Reference.Internal.Symbolic_Bag_Type
// Enumerates the various ways that a MDL_Bag collection can be defined
// symbolically in terms of other collections.
// None means the collection simply has zero members.

internal enum Symbolic_Bag_Type
{
    None,
    Singular,
    Arrayed,
    Indexed,
    Unique,
    Summed,
}

// Muldis.Data_Engine_Reference.Internal.Multiplied_Member
// Represents a multiset of 1..N members of a collection where every
// member is the same Muldis Data Language value.

internal class Multiplied_Member
{
    // The Muldis Data Language value that every member of this multiset is.
    internal MDL_Any member;

    // The count of members of this multiset.
    internal Int64 multiplicity;

    internal Multiplied_Member(MDL_Any member, Int64 multiplicity = 1)
    {
        this.member       = member;
        this.multiplicity = multiplicity;
    }

    internal Multiplied_Member Clone()
    {
        return (Multiplied_Member)this.MemberwiseClone();
    }
}

// Muldis.Data_Engine_Reference.Internal.MDL_Bag_Struct
// When a Muldis.Data_Engine_Reference.Internal.MDL_Any is representing a MDL_Bag,
// a MDL_Bag_Struct is used by it to hold the MDL_Bag-specific details.
// Also used for MDL_Set, so any MDL_Bag reference generally should be
// read as either MDL_Set or MDL_Bag.
// The "tree" is actually a uni-directional graph as multiple nodes can
// cite the same other conceptually immutable nodes as their children.
// MDL_Bag members are only directly stored in leaf nodes of the graph.
// It is important to note that the same logical Muldis Data Language Bag value
// may be represented by several different representation formats here,
// Note that MDL_Bag is the most complicated Muldis Data Language Foundation type to
// implement while at the same time is the least critical to the
// internals; it is mainly just used for user data.

internal class MDL_Bag_Struct
{
    // Local Symbolic Type (LST) determines the role this node plays in
    // the tree.  Determines interpreting "members" field.
    internal Symbolic_Bag_Type local_symbolic_type;

    // Members of this Muldis Data Language Bag, in one of several possible
    // specialized representation formats depending on the data type.
    // Iff LST is None, this field is simply null.
        // This is a leaf node explicitly defining zero Bag members.
        // Guarantees the Bag has exactly zero members.
    // Iff LST is Singular, this field holds a Multiplied_Member.
        // This is a leaf node defining 1..N Bag members, every one of
        // which is the same value, which can be of any type;
        // Singular is the basis for compactly representing either a
        // single-element Bag or a repeating portion of a sparse Bag.
        // Guarantees the Bag has at least 1 member and that all
        // Bag members are the same value.
    // Iff LST is Arrayed, this field holds a List<Multiplied_Member>.
        // This is a leaf node defining 2..N Bag members, each of which
        // can be of any type; this is the most common format for a Bag
        // that is just storing members without any operations.
        // Guarantees the Bag has at least 2 members but does not
        // guarantee that at least 2 members have distinct values.
    // Iff LST is Indexed, this field holds a Dictionary<MDL_Any,Multiplied_Member>.
        // This is a leaf node defining 2..N Bag members, each of which
        // can be of any type; this is the most common format for a Bag
        // that has had some searches or operations performed on it.
        // The Dictionary has one key-asset pair for each distinct Muldis Data Language
        // "value", all of which are indexed by cached_MDL_Any_identity.
        // Guarantees the Bag has at least 1 member.
    // Iff LST is Unique, this field holds a MDL_Bag_Struct.
        // This is a non-leaf node with 1 direct child Bag node;
        // this Bag's members are defined as the child's unique members.
        // Makes no guarantees that the Bag is none/singular/otherwise.
    // Iff LST is Summed, this field holds a MDL_Bag_Pair.
        // This is a non-leaf node with 2 direct child Bag nodes;
        // this Bag's members are defined as the catenation of
        // the pair's a0 and a1 nodes in that order.
        // Makes no guarantees that the Bag is none/singular/otherwise.
    internal Object members;

    // A cache of calculations about this Bag's members.
    internal Cached_Members_Meta cached_members_meta;

    internal Multiplied_Member Local_Singular_Members()
    {
        return (Multiplied_Member)this.members;
    }

    internal List<Multiplied_Member> Local_Arrayed_Members()
    {
        return (List<Multiplied_Member>)this.members;
    }

    internal Dictionary<MDL_Any,Multiplied_Member> Local_Indexed_Members()
    {
        return (Dictionary<MDL_Any,Multiplied_Member>)this.members;
    }

    internal MDL_Bag_Struct Tree_Unique_Members()
    {
        return (MDL_Bag_Struct)this.members;
    }

    internal MDL_Bag_Pair Tree_Summed_Members()
    {
        return (MDL_Bag_Pair)this.members;
    }
}

// Muldis.Data_Engine_Reference.Internal.MDL_Bag_Pair
// Represents an ordered pair of MDL_Bag, typically corresponding in
// order to the "0" and "1" conceptually-ordered arg/attr to a function.

internal class MDL_Bag_Pair
{
    // This is the first conceptually-ordered arg/attr.
    internal MDL_Bag_Struct a0;

    // This is the second conceptually-ordered arg/attr.
    internal MDL_Bag_Struct a1;

    internal MDL_Bag_Pair(MDL_Bag_Struct a0, MDL_Bag_Struct a1)
    {
        this.a0 = a0;
        this.a1 = a1;
    }
}

// Muldis.Data_Engine_Reference.Internal.cached_members_meta
// Represents cached metadata for the members of a Muldis Data Language "discrete
// homogeneous" collection such as an Array or Bag, particularly a
// collection implemented as a tree of nodes, where each node may
// either define members locally or refer to child nodes but not both.

internal class Cached_Members_Meta
{
    // Nullable Integer
    // Cached count of members of the Muldis Data Language Array/Bag represented by
    // this tree node including those defined by it and child nodes.
    internal Nullable<Int64> tree_member_count;

    // Nullable Boolean
    // This is true iff we know that no 2 members of the Muldis Data Language
    // Array/Bag represented by this tree node (including child nodes)
    // are the same value, and false iff we know that at least 2
    // members are the same value.
    internal Nullable<Boolean> tree_all_unique;

    // Nullable Boolean
    // This is true iff we know that the Muldis Data Language Array/Bag represented
    // by this tree node (as with tree_all_unique) has no member that
    // is not a Muldis Data Language Tuple and has no 2 members that do not have
    // the same heading; this is false iff we know that any member is
    // not a Tuple or that any 2 members do not have the same heading.
    internal Nullable<Boolean> tree_relational;
}

// Muldis.Data_Engine_Reference.Internal.MDL_Article_Struct
// When a Muldis.Data_Engine_Reference.Internal.MDL_Any is representing a MDL_Article,
// a MDL_Article_Struct is used by it to hold the MDL_Article-specific details.
// TODO: Change Article so represented as Nesting+Kit pair.

internal class MDL_Article_Struct
{
    // The Muldis Data Language value that is the "label" of this MDL_Article value.
    internal MDL_Any label;

    // The Muldis Data Language value that is the "attributes" of this MDL_Article value.
    internal MDL_Any attrs;
}
