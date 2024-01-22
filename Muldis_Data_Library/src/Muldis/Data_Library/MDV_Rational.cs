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
// Canonically the *numerator* / *denominator* pair is coprime.
// `Rational` is an infinite type.
// `Rational` has a default value of `0.0`.
// `Rational` is `Orderable`; it has no minimum or maximum value.

using System.Numerics;

namespace Muldis.Data_Library;

public sealed class MDV_Rational : MDV_Fractional<MDV_Rational>
{
    private static readonly MDV_Rational __negative_one
        = new MDV_Rational(-1,1,true);
    private static readonly MDV_Rational __zero
        = new MDV_Rational(0,1,true);
    private static readonly MDV_Rational __positive_one
        = new MDV_Rational(1,1,true);

    // A value of the .NET structure type BigInteger is immutable.
    // It should be safe to pass around without cloning.
    // A BigInteger is also optimized to handle small values compactly.
    // It would be counter-productive for MDV_Rational to have an alternate
    // representation like Int64, not only for the much greater maintenance
    // burden, but as it would likely use more memory or perform worse.
    private BigInteger __numerator;
    private BigInteger __denominator;

    // A value of the .NET structure type Boolean is immutable.
    // It should be safe to pass around without cloning.
    // A true value here declares we know the numerator/denominator pair is
    // normalized, meaning it is coprime, and the denominator is positive.
    // If this is false, the pair might still be normalized but we haven't
    // confirmed that.
    // If this is false, we still at least expect the denominator to be
    // positive; that kind of normalization at least must be done for all
    // MDV_Rational, even if making the pair have the lowest possible
    // magnitudes is done lazily only when needed.
    private Boolean __known_to_be_normalized;

    private MDV_Rational(BigInteger numerator, BigInteger denominator,
        Boolean known_to_be_normalized)
    {
        this.__numerator = numerator;
        this.__denominator = denominator;
        this.__known_to_be_normalized = known_to_be_normalized;
    }

    public override Int32 GetHashCode()
    {
        return (this.__numerator ^ this.__denominator).GetHashCode();
    }

    public override String ToString()
    {
        return Internal_Preview.Rational(this);
    }

    public override Boolean Equals(Object? obj)
    {
        return (obj is MDV_Rational specific_obj)
            ? MDV_Rational._specific_same(this, specific_obj)
            : false;
    }

    internal static MDV_Rational _from(
        BigInteger numerator, BigInteger denominator,
        Boolean known_to_be_normalized)
    {
        return new MDV_Rational(numerator, denominator,
            known_to_be_normalized);
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
        if (result_d < 0)
        {
            result_n = -result_n;
            result_d = -result_d;
        }
        BigInteger gcd = MDV_Integer._greatest_common_divisor(
            result_n, result_d);
        // Ensure numerator and denominator coprime.
        if (gcd > 1)
        {
            result_n = result_n / gcd;
            result_d = result_d / gcd;
        }
        return !result_d.IsOne ? MDV_Rational._from(result_n, result_d, true)
            : result_n.IsZero ? MDV_Rational.__zero
            : result_n.IsOne ? MDV_Rational.__positive_one
            : result_n == -1 ? MDV_Rational.__negative_one
            : MDV_Rational._from(result_n, result_d, true);
    }

    public static MDV_Rational from(Int64 numerator, Int64 denominator)
    {
        return MDV_Rational.from(
            (BigInteger)numerator, (BigInteger)denominator);
    }

    public static MDV_Rational from(Int32 numerator, Int32 denominator)
    {
        return MDV_Rational.from(
            (BigInteger)numerator, (BigInteger)denominator);
    }

    public static MDV_Rational negative_one()
    {
        return MDV_Rational.__negative_one;
    }

    public static MDV_Rational zero()
    {
        return MDV_Rational.__zero;
    }

    public static MDV_Rational positive_one()
    {
        return MDV_Rational.__positive_one;
    }

    internal BigInteger _numerator()
    {
        return this.__numerator;
    }

    public MDV_Integer numerator()
    {
        this._ensure_normalized();
        return MDV_Integer.from(this.__numerator);
    }

    public BigInteger numerator_as_BigInteger()
    {
        this._ensure_normalized();
        return this.__numerator;
    }

    public Int64 numerator_as_Int64()
    {
        this._ensure_normalized();
        // This will throw a .NET OverflowException if the value doesn't fit.
        return (Int64)this.__numerator;
    }

    public Int32 numerator_as_Int32()
    {
        this._ensure_normalized();
        // This will throw a .NET OverflowException if the value doesn't fit.
        return (Int32)this.__numerator;
    }

    internal BigInteger _denominator()
    {
        return this.__denominator;
    }

    public MDV_Integer denominator()
    {
        this._ensure_normalized();
        return MDV_Integer.from(this.__denominator);
    }

    public BigInteger denominator_as_BigInteger()
    {
        this._ensure_normalized();
        return this.__denominator;
    }

    public Int64 denominator_as_Int64()
    {
        this._ensure_normalized();
        // This will throw a .NET OverflowException if the value doesn't fit.
        return (Int64)this.__denominator;
    }

    public Int32 denominator_as_Int32()
    {
        this._ensure_normalized();
        // This will throw a .NET OverflowException if the value doesn't fit.
        return (Int32)this.__denominator;
    }

    internal Boolean _known_to_be_normalized()
    {
        return this.__known_to_be_normalized;
    }

    internal void _ensure_normalized()
    {
        if (this.__known_to_be_normalized)
        {
            return;
        }
        BigInteger gcd = MDV_Integer._greatest_common_divisor(
            this.__numerator, this.__denominator);
        if (gcd > 1)
        {
            // Make the numerator and denominator coprime.
            this.__numerator = this.__numerator / gcd;
            this.__denominator = this.__denominator / gcd;
        }
        this.__known_to_be_normalized = true;
    }

    internal static Boolean _specific_same(MDV_Rational topic_0, MDV_Rational topic_1)
    {
        if (topic_0.__denominator == topic_1.__denominator)
        {
            return (topic_0.__numerator == topic_1.__numerator);
        }
        topic_0._ensure_normalized();
        topic_1._ensure_normalized();
        return (topic_0.__denominator == topic_1.__denominator
            && topic_0.__numerator == topic_1.__numerator);
    }

    public static MDV_Boolean in_order(MDV_Rational topic_0, MDV_Rational topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(MDV_Rational._in_order(topic_0, topic_1));
    }

    internal static Boolean _in_order(MDV_Rational topic_0, MDV_Rational topic_1)
    {
        if (topic_0.__denominator == topic_1.__denominator)
        {
            return (topic_0.__numerator <= topic_1.__numerator);
        }
        // We need to compare the arguments in terms of their equivalent
        // fractions where the denominators are equal.
        // This typically means making the denominators larger, their least
        // common multiple; but first we will ensure coprime so the LCM
        // we will work with is also as small as possible.
        topic_0._ensure_normalized();
        topic_1._ensure_normalized();
        if (topic_0.__denominator == topic_1.__denominator)
        {
            return (topic_0.__numerator <= topic_1.__numerator);
        }
        BigInteger common_d = MDV_Integer._least_common_multiple(
            topic_0.__denominator, topic_1.__denominator);
        return ((topic_0.__numerator * (common_d / topic_0.__denominator))
            <= (topic_1.__numerator * (common_d / topic_1.__denominator)));
    }

    public static MDV_Boolean before(MDV_Rational topic_0, MDV_Rational topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(!MDV_Rational._in_order(topic_1, topic_0));
    }

    public static MDV_Boolean after(MDV_Rational topic_0, MDV_Rational topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(!MDV_Rational._in_order(topic_0, topic_1));
    }

    public static MDV_Boolean before_or_same(MDV_Rational topic_0, MDV_Rational topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(MDV_Rational._in_order(topic_0, topic_1));
    }

    public static MDV_Boolean after_or_same(MDV_Rational topic_0, MDV_Rational topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(MDV_Rational._in_order(topic_1, topic_0));
    }

    public static MDV_Rational min(MDV_Rational topic_0, MDV_Rational topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Rational._in_order(topic_0, topic_1) ? topic_0 : topic_1;
    }

    public static MDV_Rational max(MDV_Rational topic_0, MDV_Rational topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Rational._in_order(topic_0, topic_1) ? topic_1 : topic_0;
    }

    public static MDV_Boolean so(MDV_Rational topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic.__numerator != 0);
    }

    public static MDV_Boolean not_so(MDV_Rational topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic.__numerator == 0);
    }
}
