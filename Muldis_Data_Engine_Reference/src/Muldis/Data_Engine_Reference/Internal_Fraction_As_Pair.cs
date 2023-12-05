using System.Numerics;

namespace Muldis.Data_Engine_Reference;

// Muldis.Data_Engine_Reference.Internal_Fraction_As_Pair
// Represents a numerator/denominator pair.
// Only used internally by Muldis.Data_Engine_Reference.MDER_Fraction.

internal class Internal_Fraction_As_Pair
{
    // The numerator+denominator field pair can represent any
    // MDER_Fraction value at all, such that the Fraction's value is
    // defined as the fractional division of numerator by denominator.
    // denominator may never be zero; otherwise, the pair must always
    // be normalized at least such that the denominator is positive.
    internal BigInteger numerator;
    internal BigInteger denominator;

    // This is true iff we know that the numerator and
    // denominator are coprime (their greatest common divisor is 1);
    // this is false iff we know that they are not coprime.
    // While the pair typically need to be coprime in order to reliably
    // determine if 2 MDER_Fraction represent the same Muldis Data Language value,
    // we don't necessarily store them that way for efficiency sake.
    internal Nullable<Boolean> cached_is_coprime;

    // This is true iff we know that the MDER_Fraction value can be
    // represented as a terminating decimal number, meaning that the
    // prime factorization of the MDER_Fraction's coprime denominator
    // consists only of 2s and 5s; this is false iff we know that the
    // MDER_Fraction value would be an endlessly repeating decimal
    // number, meaning that the MDER_Fraction's coprime denominator has
    // at least 1 prime factor that is not a 2 or a 5.
    // The identity serialization of a MDER_Fraction uses a single decimal
    // number iff it would terminate and a coprime integer pair otherwise.
    // This field may be true even if cached_is_coprime isn't because
    // this Internal_Fraction_As_Pair was derived from a Decimal.
    internal Nullable<Boolean> cached_is_terminating_decimal;

    // Iff this field is defined, we ensure that both the current
    // MDER_Fraction_Struct has a denominator equal to it, and also that
    // any other MDER_Fraction_Struct derived from it has the same
    // denominator as well, iff the MDER_Fraction value can be exactly
    // represented by such a MDER_Fraction_Struct.
    // Having this field defined tends to suppress automatic efforts to
    // normalize the MDER_Fraction_Struct to a coprime state.
    // The main purpose of this field is to aid in system performance
    // when a lot of math, particularly addition and subtraction, is
    // done with rationals having a common conceptual fixed precision,
    // so that the performance is then closer to integer math.
    // internal Nullable<BigInteger> denominator_affinity;

    internal Internal_Fraction_As_Pair(BigInteger numerator, BigInteger denominator)
    {
        this.numerator = numerator;
        this.denominator = denominator;
    }
}
