using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Muldis.Data_Engine_Reference.Internal;

// Muldis.Data_Engine_Reference.Internal.MDL_Any
// Represents a Muldis Data Language "value", which is an individual constant that
// is not fixed in time or space.  Every Muldis Data Language value is unique,
// eternal, and immutable; it has no address and can not be updated.
// Several distinct MDL_Any objects may denote the same Muldis Data Language
// "value"; however, any time that two MDL_Any are discovered to
// denote the same Muldis Data Language value, any references to one may be safely
// replaced by references to the other.
// A design objective is implementing the "flyweight pattern",
// where MDL_Any objects share components directly or indirectly as much
// as is safely possible.  For example, if we detect duplication at
// runtime, we may replace references to one
// MDL_Any with another where the underlying virtual machine or
// garbage collector doesn't natively provide that ability.
// Similarly, proving equality of two "value" can often short-circuit.
// While MDL_Any are immutable from a user's perspective, their
// components may in fact mutate for memory sharing or consolidating.
// Iff a Muldis Data Language "value" is a "Handle" then it references something
// that possibly can mutate, such as a Muldis Data Language "variable".

internal class MDL_Any
{
    // Memory pool this Muldis Data Language "value" lives in.
    internal Memory Memory { get; set; }

    // Muldis Data Language most specific well known base data type (WKBT) this
    // "value" is a member of.  Determines interpreting Details field.
    // Some of these types have their own subset of specialized
    // representation formats for the sake of optimization.
    internal Well_Known_Base_Type WKBT { get; set; }

    // Details of this Muldis Data Language "value", in one of several possible
    // specialized representation formats depending on the data type.
    // Iff WKBT is MDL_Boolean, this field holds a Nullable<Boolean>.
    // Iff WKBT is MDL_Integer, this field holds a BigInteger.
        // While we conceptually could special case smaller integers with
        // additional fields for performance, we won't, mainly to keep
        // things simpler, and because BigInteger special-cases internally.
    // Iff WKBT is MDL_Fraction, this field holds a MDL_Fraction_Struct.
    // Iff WKBT is MDL_Bits, this field holds a BitArray.
        // Consider a MDL_Bits_Struct if we want symbolic like MDL_Array.
    // Iff WKBT is MDL_Blob, this field holds a Byte[].
        // Consider a MDL_Blob_Struct if we want symbolic like MDL_Array.
    // Iff WKBT is MDL_Text, this field holds a MDL_Text_Struct.
    // Iff WKBT is MDL_Array, this field holds a MDL_Array_Struct.
    // Iff WKBT is MDL_Set, this field holds a MDL_Bag_Struct (like MDL_Bag).
    // Iff WKBT is MDL_Bag, this field holds a MDL_Bag_Struct (like MDL_Set).
    // Iff WKBT is MDL_Tuple, this field holds a Dictionary<String,MDL_Any> (like MDL_Excuse).
    // Iff WKBT is MDL_Article, this field holds a MDL_Article_Struct.
    // Iff WKBT is MDL_Variable, this field holds a MDL_Any.
        // For a MDL_Variable, Details holds its Current_Value.
        // This can become a MDL_Variable_Struct if we want to store other things.
    // Iff WKBT is MDL_Process, this field holds an Object.
        // TODO: Replace this with some other type when we know what that is.
    // Iff WKBT is MDL_Stream, this field holds an Object.
        // TODO: Replace this with some other type when we know what that is.
    // Iff WKBT is MDL_External, this field holds an Object.
        // The entity that is defined and managed externally to the Muldis
        // D language environment, which the MDL_External value is an opaque
        // and transient reference to.
    // Iff WKBT is MDL_Excuse, this field holds a Dictionary<String,MDL_Any> (like MDL_Tuple).
        // TODO: Change Excuse so represented as Nesting+Kit pair.
    internal Object Details { get; set; }

    // Set of well-known Muldis Data Language types that this Muldis Data Language "value" is
    // known to either be or not be a member of.  This is populated
    // semi-lazily as needed.  cached_WKT_statuses is null iff the set
    // is empty. A type is in cached_WKT_statuses with Boolean=true iff
    // we know the value is a member of the type and with Boolean=false
    // iff we know the value is not a member of the type; when
    // cached_WKT_statuses doesn't mention a type at all, we either
    // didn't test the membership or decided not to cache the result.
    // This set excludes on purpose the subset of well-known types that
    // should be trivial to test membership of by other means; in
    // particular it excludes {Any,None}, the Well_Known_Base_Type;
    // types not excluded are more work to test.
    internal Dictionary<Well_Known_Type,Boolean> cached_WKT_statuses { get; set; }

    // Normalized serialization of the Muldis Data Language "value" that its host
    // MDL_Any represents.  This is calculated lazily if needed,
    // typically when the "value" is a member of an indexed collection.
    // The serialization format either is or resembles a Muldis Data Language Plain Text
    // literal for selecting the value, in the form of character strings
    // whose character code points are typically in the 0..127 range.
    internal String Cached_MD_Any_Identity { get; set; }

    internal Boolean Same(MDL_Any value)
    {
        return Memory.Executor.Any__same(this, value);
    }

    internal String MDL_Any_Identity()
    {
        // This called function will test if Cached_MD_Any_Identity
        // is null and assign it a value if so and use its value if not.
        return Memory.Identity_Generator.MDL_Any_to_Identity_String(this);
    }

    public override String ToString()
    {
        return Memory.Preview_Generator.MDL_Any_To_Preview_String(this);
    }

    internal Nullable<Boolean> MDL_Boolean()
    {
        return (Nullable<Boolean>)Details;
    }

    internal BigInteger MDL_Integer()
    {
        return (BigInteger)Details;
    }

    internal MDL_Fraction_Struct MDL_Fraction()
    {
        return (MDL_Fraction_Struct)Details;
    }

    internal BitArray MDL_Bits()
    {
        return (BitArray)Details;
    }

    internal Byte[] MDL_Blob()
    {
        return (Byte[])Details;
    }

    internal MDL_Text_Struct MDL_Text()
    {
        return (MDL_Text_Struct)Details;
    }

    internal MDL_Array_Struct MDL_Array()
    {
        return (MDL_Array_Struct)Details;
    }

    internal MDL_Bag_Struct MDL_Set()
    {
        return MDL_Bag();
    }

    internal MDL_Bag_Struct MDL_Bag()
    {
        return (MDL_Bag_Struct)Details;
    }

    internal Dictionary<String,MDL_Any> MDL_Tuple()
    {
        return (Dictionary<String,MDL_Any>)Details;
    }

    internal MDL_Article_Struct MDL_Article()
    {
        return (MDL_Article_Struct)Details;
    }

    internal MDL_Any MDL_Variable()
    {
        return (MDL_Any)Details;
    }

    internal Object MDL_Process()
    {
        return (Object)Details;
    }

    internal Object MDL_Stream()
    {
        return (Object)Details;
    }

    internal Object MDL_External()
    {
        return (Object)Details;
    }

    internal Dictionary<String,MDL_Any> MDL_Excuse()
    {
        return (Dictionary<String,MDL_Any>)Details;
    }

    internal Nullable<Boolean> member_status_in_WKT(Well_Known_Type type)
    {
        return cached_WKT_statuses is null            ? (Nullable<Boolean>)null
             : !cached_WKT_statuses.ContainsKey(type) ? (Nullable<Boolean>)null
             :                                          cached_WKT_statuses[type];
    }

    internal void declare_member_status_in_WKT(Well_Known_Type type, Boolean status)
    {
        if (cached_WKT_statuses is null)
        {
            cached_WKT_statuses = new Dictionary<Well_Known_Type,Boolean>();
        }
        if (!cached_WKT_statuses.ContainsKey(type))
        {
            cached_WKT_statuses.Add(type, status);
        }
        else
        {
            cached_WKT_statuses[type] = status;
        }
    }
}

internal class MDL_Any_Comparer : EqualityComparer<MDL_Any>
{
    public override Boolean Equals(MDL_Any v1, MDL_Any v2)
    {
        if (v1 is null && v2 is null)
        {
            // Would we ever get here?
            return true;
        }
        if (v1 is null || v2 is null)
        {
            return false;
        }
        return v1.Same(v2);
    }

    public override Int32 GetHashCode(MDL_Any v)
    {
        if (v is null)
        {
            // Would we ever get here?
            return 0;
        }
        return v.MDL_Any_Identity().GetHashCode();
    }
}

// Muldis.Data_Engine_Reference.Internal.MDL_Fraction_Struct
// When a Muldis.Data_Engine_Reference.Internal.MDL_Any is representing a MDL_Fraction,
// an MDL_Fraction_Struct is used by it to hold the MDL_Fraction-specific details.

internal class MDL_Fraction_Struct
{
    // The As_Decimal field is optionally valued if the MDL_Fraction
    // value is known small enough to fit in the range it can represent
    // and it is primarily used when the MDL_Fraction value was input to
    // the system as a .NET Decimal in the first place or was output
    // from the system as such; a MDL_Fraction input first as a Decimal
    // is only copied to the As_Pair field when needed;
    // if both said fields are valued at once, they are redundant.
    internal Nullable<Decimal> As_Decimal { get; set; }

    // The As_Pair field, comprising a Numerator+Denominator field
    // pair, can represent any
    // MDL_Fraction value at all, such that the Fraction's value is
    // defined as the fractional division of Numerator by Denominator.
    // As_Pair might not be defined if As_Decimal is defined.
    internal MDL_Fraction_Pair As_Pair { get; set; }

    internal void Ensure_Coprime()
    {
        Ensure_Pair();
        if (As_Pair.Cached_Is_Coprime == true)
        {
            return;
        }
        // Note that GreatestCommonDivisor() always has a non-negative result.
        BigInteger gcd = BigInteger.GreatestCommonDivisor(As_Pair.Numerator, As_Pair.Denominator);
        if (gcd > 1)
        {
            // Make the numerator and denominator coprime.
            As_Pair.Numerator   = As_Pair.Numerator   / gcd;
            As_Pair.Denominator = As_Pair.Denominator / gcd;
        }
        As_Pair.Cached_Is_Coprime = true;
    }

    internal void Ensure_Pair()
    {
        if (As_Pair is not null)
        {
            return;
        }
        // If Numerator+Denominator are null, As_Decimal must not be.
        Int32[] dec_bits = Decimal.GetBits((Decimal)As_Decimal);
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
        Decimal numerator_dec = (Decimal)As_Decimal * denominator_dec;
        As_Pair = new MDL_Fraction_Pair {
            Numerator = new BigInteger(numerator_dec),
            Denominator = new BigInteger(denominator_dec),
            Cached_Is_Terminating_Decimal = true,
        };
    }

    internal Boolean Is_Terminating_Decimal()
    {
        if (As_Decimal is not null)
        {
            return true;
        }
        if (As_Pair.Cached_Is_Terminating_Decimal is null)
        {
            Ensure_Coprime();
            Boolean found_all_2_factors = false;
            Boolean found_all_5_factors = false;
            BigInteger confirmed_quotient = As_Pair.Denominator;
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
            As_Pair.Cached_Is_Terminating_Decimal = (confirmed_quotient == 1);
        }
        return (Boolean)As_Pair.Cached_Is_Terminating_Decimal;
    }

    internal Int32 Pair_Decimal_Denominator_Scale()
    {
        if (As_Pair is null || !Is_Terminating_Decimal())
        {
            throw new InvalidOperationException();
        }
        Ensure_Coprime();
        for (Int32 dec_scale = 0; dec_scale <= Int32.MaxValue; dec_scale++)
        {
            // BigInteger.Pow() can only take an Int32 exponent anyway.
            if (BigInteger.Remainder(BigInteger.Pow(10,dec_scale), As_Pair.Denominator) == 0)
            {
                return dec_scale;
            }
        }
        // If somehow the Denominator can be big enough that we'd actually get here.
        throw new NotImplementedException();
    }
}

// Muldis.Data_Engine_Reference.Internal.MDL_Fraction_Pair
// Represents a numerator/denominator pair.

internal class MDL_Fraction_Pair
{
    // The Numerator+Denominator field pair can represent any
    // MDL_Fraction value at all, such that the Fraction's value is
    // defined as the fractional division of Numerator by Denominator.
    // Denominator may never be zero; otherwise, the pair must always
    // be normalized at least such that the Denominator is positive.
    internal BigInteger Numerator { get; set; }
    internal BigInteger Denominator { get; set; }

    // This is true iff we know that the Numerator and
    // Denominator are coprime (their greatest common divisor is 1);
    // this is false iff we know that they are not coprime.
    // While the pair typically need to be coprime in order to reliably
    // determine if 2 MDL_Fraction represent the same Muldis Data Language value,
    // we don't necessarily store them that way for efficiency sake.
    internal Nullable<Boolean> Cached_Is_Coprime { get; set; }

    // This is true iff we know that the MDL_Fraction value can be
    // represented as a terminating decimal number, meaning that the
    // prime factorization of the MDL_Fraction's coprime Denominator
    // consists only of 2s and 5s; this is false iff we know that the
    // MDL_Fraction value would be an endlessly repeating decimal
    // number, meaning that the MDL_Fraction's coprime Denominator has
    // at least 1 prime factor that is not a 2 or a 5.
    // The identity serialization of a MDL_Fraction uses a single decimal
    // number iff it would terminate and a coprime integer pair otherwise.
    // This field may be true even if Cached_Is_Coprime isn't because
    // this MDL_Fraction_Pair was derived from a Decimal.
    internal Nullable<Boolean> Cached_Is_Terminating_Decimal { get; set; }

    // Iff this field is defined, we ensure that both the current
    // MDL_Fraction_Struct has a Denominator equal to it, and also that
    // any other MDL_Fraction_Struct derived from it has the same
    // Denominator as well, iff the MDL_Fraction value can be exactly
    // represented by such a MDL_Fraction_Struct.
    // Having this field defined tends to suppress automatic efforts to
    // normalize the MDL_Fraction_Struct to a coprime state.
    // The main purpose of this field is to aid in system performance
    // when a lot of math, particularly addition and subtraction, is
    // done with rationals having a common conceptual fixed precision,
    // so that the performance is then closer to integer math.
    internal Nullable<BigInteger> Denominator_Affinity { get; set; }
}

// Muldis.Data_Engine_Reference.Internal.MDL_Text_Struct
// When a Muldis.Data_Engine_Reference.Internal.MDL_Any is representing a MDL_Text,
// an MDL_Text_Struct is used by it to hold the MDL_Text-specific details.

internal class MDL_Text_Struct
{
    // Represents a Muldis Data Language Text value where each member value is
    // a Muldis Data Language Integer in the range {0..0xD7FF,0xE000..0x10FFFF}.
    // A .NET String is the simplest storage representation for that
    // type which doesn't internally use trees for sharing or multipliers.
    // This is the canonical storage type for a regular character string.
    // Each logical member represents a single Unicode standard character
    // code point from either the Basic Multilingual Plane (BMP), which
    // is those member values in the range {0..0xD7FF,0xE000..0xFFFF},
    // or from either of the 16 supplementary planes, which is those
    // member values in the range {0x10000..0x10FFFF}.
    // Code_Point_Members is represented using a standard .NET
    // String value for simplicity but a String has a different native
    // concept of components; it is formally an array of .NET Char
    // each of which is either a whole BMP code point or half of a
    // non-BMP code point; a non-BMP code point is represented by a pair
    // of consecutive Char with numeric values in {0xD800..0xDFFF};
    // therefore, the native "length" of a String only matches the
    // "length" of the Muldis Data Language Text when all code points are in the BMP.
    // While it is possible for a .NET String to contain an isolated
    // "surrogate" Char outside of a proper "surrogate pair", both
    // Muldis.Data_Engine_Reference forbids such a malformed String
    // from either being used internally or being passed in by the API.
    internal String Code_Point_Members { get; set; }

    // Nullable Boolean
    // This is true iff we know that at least 1 code point member is NOT
    // in the Basic Multilingual Plane (BMP); this is false iff we know
    // that there is no such code point member.  That is, with respect
    // to a .NET String, this is true iff we know the String has at least
    // 1 "surrogate pair".  We cache this knowledge because a .NET String
    // with any non-BMP Char is more complicated to count the members
    // of or access members by ordinal position or do some other stuff.
    // This field is always defined as a side-effect of Muldis.Data_Engine_Reference
    // forbidding a malformed String and so they are always tested at
    // the borders, that test also revealing if a String has non-BMP chars.
    internal Boolean Has_Any_Non_BMP { get; set; }

    // Cached count of code point members of the Muldis Data Language Text.
    internal Nullable<Int64> Cached_Member_Count { get; set; }
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
    // the tree.  Determines interpreting Members field.
    internal Symbolic_Array_Type Local_Symbolic_Type { get; set; }

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
        // the pair's A0 and A1 nodes in that order.
        // Makes no guarantees that the Array is none/singular/otherwise.
    internal Object Members { get; set; }

    // A cache of calculations about this Array's members.
    internal Cached_Members_Meta Cached_Members_Meta { get; set; }

    internal Multiplied_Member Local_Singular_Members()
    {
        return (Multiplied_Member)Members;
    }

    internal List<MDL_Any> Local_Arrayed_Members()
    {
        return (List<MDL_Any>)Members;
    }

    internal MDL_Array_Pair Tree_Catenated_Members()
    {
        return (MDL_Array_Pair)Members;
    }
}

// Muldis.Data_Engine_Reference.Internal.MDL_Array_Pair
// Represents an ordered pair of MDL_Array, typically corresponding in
// order to the "0" and "1" conceptually-ordered arg/attr to a function.

internal class MDL_Array_Pair
{
    // This is the first conceptually-ordered arg/attr.
    internal MDL_Array_Struct A0 { get; set; }

    // This is the second conceptually-ordered arg/attr.
    internal MDL_Array_Struct A1 { get; set; }

    internal MDL_Array_Pair(MDL_Array_Struct a0, MDL_Array_Struct a1)
    {
        A0 = a0;
        A1 = a1;
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
    internal MDL_Any Member { get; set; }

    // The count of members of this multiset.
    internal Int64 Multiplicity { get; set; }

    internal Multiplied_Member(MDL_Any member, Int64 multiplicity = 1)
    {
        Member       = member;
        Multiplicity = multiplicity;
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
    // the tree.  Determines interpreting Members field.
    internal Symbolic_Bag_Type Local_Symbolic_Type { get; set; }

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
        // "value", all of which are indexed by Cached_MD_Any_Identity.
        // Guarantees the Bag has at least 1 member.
    // Iff LST is Unique, this field holds a MDL_Bag_Struct.
        // This is a non-leaf node with 1 direct child Bag node;
        // this Bag's members are defined as the child's unique members.
        // Makes no guarantees that the Bag is none/singular/otherwise.
    // Iff LST is Summed, this field holds a MDL_Bag_Pair.
        // This is a non-leaf node with 2 direct child Bag nodes;
        // this Bag's members are defined as the catenation of
        // the pair's A0 and A1 nodes in that order.
        // Makes no guarantees that the Bag is none/singular/otherwise.
    internal Object Members { get; set; }

    // A cache of calculations about this Bag's members.
    internal Cached_Members_Meta Cached_Members_Meta { get; set; }

    internal Multiplied_Member Local_Singular_Members()
    {
        return (Multiplied_Member)Members;
    }

    internal List<Multiplied_Member> Local_Arrayed_Members()
    {
        return (List<Multiplied_Member>)Members;
    }

    internal Dictionary<MDL_Any,Multiplied_Member> Local_Indexed_Members()
    {
        return (Dictionary<MDL_Any,Multiplied_Member>)Members;
    }

    internal MDL_Bag_Struct Tree_Unique_Members()
    {
        return (MDL_Bag_Struct)Members;
    }

    internal MDL_Bag_Pair Tree_Summed_Members()
    {
        return (MDL_Bag_Pair)Members;
    }
}

// Muldis.Data_Engine_Reference.Internal.MDL_Bag_Pair
// Represents an ordered pair of MDL_Bag, typically corresponding in
// order to the "0" and "1" conceptually-ordered arg/attr to a function.

internal class MDL_Bag_Pair
{
    // This is the first conceptually-ordered arg/attr.
    internal MDL_Bag_Struct A0 { get; set; }

    // This is the second conceptually-ordered arg/attr.
    internal MDL_Bag_Struct A1 { get; set; }

    internal MDL_Bag_Pair(MDL_Bag_Struct a0, MDL_Bag_Struct a1)
    {
        A0 = a0;
        A1 = a1;
    }
}

// Muldis.Data_Engine_Reference.Internal.Cached_Members_Meta
// Represents cached metadata for the members of a Muldis Data Language "discrete
// homogeneous" collection such as an Array or Bag, particularly a
// collection implemented as a tree of nodes, where each node may
// either define members locally or refer to child nodes but not both.

internal class Cached_Members_Meta
{
    // Nullable Integer
    // Cached count of members of the Muldis Data Language Array/Bag represented by
    // this tree node including those defined by it and child nodes.
    internal Nullable<Int64> Tree_Member_Count { get; set; }

    // Nullable Boolean
    // This is true iff we know that no 2 members of the Muldis Data Language
    // Array/Bag represented by this tree node (including child nodes)
    // are the same value, and false iff we know that at least 2
    // members are the same value.
    internal Nullable<Boolean> Tree_All_Unique { get; set; }

    // Nullable Boolean
    // This is true iff we know that the Muldis Data Language Array/Bag represented
    // by this tree node (as with Tree_All_Unique) has no member that
    // is not a Muldis Data Language Tuple and has no 2 members that do not have
    // the same heading; this is false iff we know that any member is
    // not a Tuple or that any 2 members do not have the same heading.
    internal Nullable<Boolean> Tree_Relational { get; set; }
}

// Muldis.Data_Engine_Reference.Internal.MDL_Article_Struct
// When a Muldis.Data_Engine_Reference.Internal.MDL_Any is representing a MDL_Article,
// a MDL_Article_Struct is used by it to hold the MDL_Article-specific details.
// TODO: Change Article so represented as Nesting+Kit pair.

internal class MDL_Article_Struct
{
    // The Muldis Data Language value that is the "label" of this MDL_Article value.
    internal MDL_Any Label { get; set; }

    // The Muldis Data Language value that is the "attributes" of this MDL_Article value.
    internal MDL_Any Attrs { get; set; }
}
