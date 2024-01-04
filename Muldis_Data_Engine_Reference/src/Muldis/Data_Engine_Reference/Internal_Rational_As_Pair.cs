using System.Numerics;

namespace Muldis.Data_Engine_Reference;

// Muldis.Data_Engine_Reference.Internal_Rational_As_Pair
// Represents a numerator/denominator pair.
// Only used internally by Muldis.Data_Engine_Reference.MDER_Rational.

internal sealed class Internal_Rational_As_Pair
{
    // The numerator+denominator field pair can represent any
    // MDER_Rational value at all, such that the Rational's value is
    // defined as the fractional division of numerator by denominator.
    // denominator may never be zero; otherwise, the pair must always
    // be normalized at least such that the denominator is positive.
    internal BigInteger numerator;
    internal BigInteger denominator;

    // This is true iff we know that the numerator and
    // denominator are coprime (their greatest common divisor is 1);
    // this is false iff we know that they are not coprime.
    // While the pair typically need to be coprime in order to reliably
    // determine if 2 MDER_Rational represent the same Muldis Data Language value,
    // we don't necessarily store them that way for efficiency sake.
    internal Boolean? cached_is_coprime;

    // This is true iff we know that the MDER_Rational value can be
    // represented as a terminating decimal number, meaning that the
    // prime factorization of the MDER_Rational's coprime denominator
    // consists only of 2s and 5s; this is false iff we know that the
    // MDER_Rational value would be an endlessly repeating decimal
    // number, meaning that the MDER_Rational's coprime denominator has
    // at least 1 prime factor that is not a 2 or a 5.
    // The identity serialization of a MDER_Rational uses a single decimal
    // number iff it would terminate and a coprime integer pair otherwise.
    // This field may be true even if cached_is_coprime isn't because
    // this Internal_Rational_As_Pair was derived from a Decimal.
    internal Boolean? cached_is_terminating_decimal;

    // Iff this field is defined, we ensure that both the current
    // MDER_Rational_Struct has a denominator equal to it, and also that
    // any other MDER_Rational_Struct derived from it has the same
    // denominator as well, iff the MDER_Rational value can be exactly
    // represented by such a MDER_Rational_Struct.
    // Having this field defined tends to suppress automatic efforts to
    // normalize the MDER_Rational_Struct to a coprime state.
    // The main purpose of this field is to aid in system performance
    // when a lot of math, particularly addition and subtraction, is
    // done with rationals having a common conceptual fixed precision,
    // so that the performance is then closer to integer math.
    // internal BigInteger? denominator_affinity;

    internal Internal_Rational_As_Pair(BigInteger numerator, BigInteger denominator)
    {
        this.numerator = numerator;
        this.denominator = denominator;
    }
}
