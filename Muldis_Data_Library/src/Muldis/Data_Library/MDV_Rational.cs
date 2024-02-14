// Selection type definer `Rational`.
// Represents foundation type `fdn::Rational`.
// A `Rational` value is a general purpose exact rational number of any
// magnitude and precision, which explicitly does not represent any
// kind of thing in particular, neither cardinal nor ordinal nor nominal.
// A `Rational` value is characterized by the pairing of a *numerator*,
// which is an `Integer` value, with a *denominator*, which is an
// `Integer` value that is non-zero.
// The intended interpretation of a `Rational` is as the rational number
// that results from evaluating the given 2 integers as the mathematical
// expression `numerator/denominator`, such that `/` means divide.
// Canonically the *numerator* / *denominator* pair is normalized/reduced/coprime.
// `Rational` is an infinite type.
// `Rational` has a default value of `0.0`.
// `Rational` is `Orderable`; it has no minimum or maximum value.

using System.Numerics;

namespace Muldis.Data_Library;

public readonly struct MDV_Rational
    : MDV_Orderable<MDV_Rational>, MDV_Numerical<MDV_Rational>
{
    private static readonly MDV_Rational __negative_one
        = new MDV_Rational(BigInteger.MinusOne, BigInteger.One);
    private static readonly MDV_Rational __zero
        = new MDV_Rational(BigInteger.Zero, BigInteger.One);
    private static readonly MDV_Rational __positive_one
        = new MDV_Rational(BigInteger.One, BigInteger.One);

    // A value of the .NET structure type BigInteger is immutable.
    // It should be safe to pass around without cloning.
    // The numerator/denominator pair is normalized/reduced/coprime,
    // and the denominator is positive.
    private readonly BigInteger __numerator;
    private readonly BigInteger __denominator;

    private MDV_Rational(BigInteger numerator, BigInteger denominator)
    {
        this.__numerator = numerator;
        this.__denominator = denominator;
    }

    public override Int32 GetHashCode()
    {
        return this.__numerator.GetHashCode() ^ this.__denominator.GetHashCode();
    }

    public override String ToString()
    {
        return Internal_Identity.Rational(this);
    }

    public override Boolean Equals(Object? obj)
    {
        return (obj is MDV_Rational specific_obj)
            && this._same(specific_obj);
    }

    public static MDV_Rational from(
        MDV_Integer numerator, MDV_Integer denominator)
    {
        return MDV_Rational.from(
            numerator.as_BigInteger(), denominator.as_BigInteger());
    }

    public static MDV_Rational from(
        BigInteger numerator, BigInteger denominator)
    {
        if (denominator.IsZero)
        {
            throw new DivideByZeroException();
        }
        BigInteger result_n = numerator;
        BigInteger result_d = denominator;
        // Ensure denominator is positive.
        if (result_d < BigInteger.Zero)
        {
            result_n = -result_n;
            result_d = -result_d;
        }
        BigInteger gcd = MDV_Integer._greatest_common_divisor(
            result_n, result_d);
        // Ensure numerator and denominator normalized/reduced/coprime.
        if (gcd > BigInteger.One)
        {
            result_n = result_n / gcd;
            result_d = result_d / gcd;
        }
        return !result_d.IsOne ? new MDV_Rational(result_n, result_d)
            : result_n.IsZero ? MDV_Rational.__zero
            : result_n.IsOne ? MDV_Rational.__positive_one
            : result_n.Equals(BigInteger.MinusOne) ? MDV_Rational.__negative_one
            : new MDV_Rational(result_n, result_d);
    }

    public static MDV_Rational from(Int64 numerator, Int64 denominator)
    {
        return MDV_Rational.from(
            (BigInteger) numerator, (BigInteger) denominator);
    }

    public static MDV_Rational from(Int32 numerator, Int32 denominator)
    {
        return MDV_Rational.from(
            (BigInteger) numerator, (BigInteger) denominator);
    }

    public static MDV_Rational negative_one()
    {
        return MDV_Rational.__negative_one;
    }

    public static MDV_Rational zero_()
    {
        return MDV_Rational.__zero;
    }

    public static MDV_Rational positive_one()
    {
        return MDV_Rational.__positive_one;
    }

    public void Deconstruct(
        out MDV_Integer numerator, out MDV_Integer denominator)
    {
        numerator = MDV_Integer.from(this.__numerator);
        denominator = MDV_Integer.from(this.__denominator);
    }

    public void Deconstruct(
        out BigInteger numerator, out BigInteger denominator)
    {
        numerator = this.__numerator;
        denominator = this.__denominator;
    }

    public void Deconstruct(out Int64 numerator, out Int64 denominator)
    {
        // This will throw a .NET OverflowException if the value doesn't fit.
        numerator = (Int64) this.__numerator;
        denominator = (Int64) this.__denominator;
    }

    public void Deconstruct(out Int32 numerator, out Int32 denominator)
    {
        // This will throw a .NET OverflowException if the value doesn't fit.
        numerator = (Int32) this.__numerator;
        denominator = (Int32) this.__denominator;
    }

    public MDV_Integer numerator()
    {
        return MDV_Integer.from(this.__numerator);
    }

    public BigInteger numerator_as_BigInteger()
    {
        return this.__numerator;
    }

    public Int64 numerator_as_Int64()
    {
        // This will throw a .NET OverflowException if the value doesn't fit.
        return (Int64) this.__numerator;
    }

    public Int32 numerator_as_Int32()
    {
        // This will throw a .NET OverflowException if the value doesn't fit.
        return (Int32) this.__numerator;
    }

    public MDV_Integer denominator()
    {
        return MDV_Integer.from(this.__denominator);
    }

    public BigInteger denominator_as_BigInteger()
    {
        return this.__denominator;
    }

    public Int64 denominator_as_Int64()
    {
        // This will throw a .NET OverflowException if the value doesn't fit.
        return (Int64) this.__denominator;
    }

    public Int32 denominator_as_Int32()
    {
        // This will throw a .NET OverflowException if the value doesn't fit.
        return (Int32) this.__denominator;
    }

    public MDV_Boolean same(MDV_Any topic_1)
    {
        MDV_Rational topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return (topic_1 is MDV_Rational specific_topic_1)
            ? MDV_Boolean.from(topic_0._same(specific_topic_1))
            : MDV_Boolean.@false();
    }

    private Boolean _same(MDV_Rational topic_1)
    {
        MDV_Rational topic_0 = this;
        return (topic_0.__denominator.Equals(topic_1.__denominator)
            && topic_0.__numerator.Equals(topic_1.__numerator));
    }

    public MDV_Boolean in_order(MDV_Orderable<MDV_Rational> topic_1)
    {
        MDV_Rational topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic_0._in_order((MDV_Rational) topic_1));
    }

    private Boolean _in_order(MDV_Rational topic_1)
    {
        MDV_Rational topic_0 = this;
        if (topic_0.__denominator.Equals(topic_1.__denominator))
        {
            return (topic_0.__numerator <= topic_1.__numerator);
        }
        // We need to compare the arguments in terms of their equivalent
        // fractions where the denominators are equal.
        BigInteger common_d = MDV_Integer._least_common_multiple(
            topic_0.__denominator, topic_1.__denominator);
        return ((topic_0.__numerator * (common_d / topic_0.__denominator))
            <= (topic_1.__numerator * (common_d / topic_1.__denominator)));
    }

    public MDV_Boolean so_zero()
    {
        MDV_Rational topic = this;
        return MDV_Boolean.from(topic.__numerator.IsZero);
    }

    public MDV_Numerical<MDV_Rational> zero()
    {
        return MDV_Rational.__zero;
    }
}
