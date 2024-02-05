// Selection type definer `Integer`.
// Represents foundation type `fdn::Integer`.
// An `Integer` value is a general purpose exact integral number of any
// magnitude, which explicitly does not represent any kind of
// thing in particular, neither cardinal nor ordinal nor nominal.
// `Integer` is an infinite type.
// `Integer` has a default value of `0`.
// `Integer` is both `Orderable` and `Bicessable`;
// it has no minimum or maximum value.

using System.Numerics;

namespace Muldis.Data_Library;

public readonly struct MDV_Integer
    : MDV_Bicessable<MDV_Integer>, MDV_Numerical<MDV_Integer>
{
    private static readonly MDV_Integer __negative_one = new MDV_Integer(-1);
    private static readonly MDV_Integer __zero = new MDV_Integer(0);
    private static readonly MDV_Integer __positive_one = new MDV_Integer(1);

    // A value of the .NET structure type BigInteger is immutable.
    // It should be safe to pass around without cloning.
    // A BigInteger is also optimized to handle small values compactly.
    // It would be counter-productive for MDV_Integer to have an alternate
    // representation like Int64, not only for the much greater maintenance
    // burden, but as it would likely use more memory or perform worse.
    private readonly BigInteger __as_BigInteger;

    private MDV_Integer(BigInteger as_BigInteger)
    {
        this.__as_BigInteger = as_BigInteger;
    }

    public override Int32 GetHashCode()
    {
        return this.__as_BigInteger.GetHashCode();
    }

    public override String ToString()
    {
        return Internal_Preview.Integer(this);
    }

    public override Boolean Equals(Object? obj)
    {
        return (obj is MDV_Integer specific_obj)
            ? this._same(specific_obj)
            : false;
    }

    public static MDV_Integer from(BigInteger as_BigInteger)
    {
        return as_BigInteger.IsZero ? MDV_Integer.__zero
            : as_BigInteger.IsOne ? MDV_Integer.__positive_one
            : as_BigInteger.Equals(-1) ? MDV_Integer.__negative_one
            : new MDV_Integer(as_BigInteger);
    }

    public static MDV_Integer from(Int64 as_Int64)
    {
        return MDV_Integer.from((BigInteger)as_Int64);
    }

    public static MDV_Integer from(Int32 as_Int32)
    {
        return MDV_Integer.from((BigInteger)as_Int32);
    }

    public static MDV_Integer negative_one()
    {
        return MDV_Integer.__negative_one;
    }

    public static MDV_Integer zero()
    {
        return MDV_Integer.__zero;
    }

    public static MDV_Integer positive_one()
    {
        return MDV_Integer.__positive_one;
    }

    public void Deconstruct(out BigInteger as_BigInteger)
    {
        as_BigInteger = this.__as_BigInteger;
    }

    public void Deconstruct(out Int64 as_Int64)
    {
        // This will throw a .NET OverflowException if the value doesn't fit.
        as_Int64 = (Int64)this.__as_BigInteger;
    }

    public void Deconstruct(out Int32 as_Int32)
    {
        // This will throw a .NET OverflowException if the value doesn't fit.
        as_Int32 = (Int32)this.__as_BigInteger;
    }

    public BigInteger as_BigInteger()
    {
        return this.__as_BigInteger;
    }

    public Int64 as_Int64()
    {
        // This will throw a .NET OverflowException if the value doesn't fit.
        return (Int64)this.__as_BigInteger;
    }

    public Int32 as_Int32()
    {
        // This will throw a .NET OverflowException if the value doesn't fit.
        return (Int32)this.__as_BigInteger;
    }

    public MDV_Boolean same(MDV_Any topic_1)
    {
        MDV_Integer topic_0 = this;
        return (topic_1 is MDV_Integer specific_topic_1)
            ? MDV_Boolean.from(topic_0._same(specific_topic_1))
            : MDV_Boolean.@false();
    }

    public MDV_Boolean not_same(MDV_Any topic_1)
    {
        MDV_Integer topic_0 = this;
        return (topic_1 is MDV_Integer specific_topic_1)
            ? MDV_Boolean.from(!topic_0._same(specific_topic_1))
            : MDV_Boolean.@true();
    }

    internal Boolean _same(MDV_Integer topic_1)
    {
        MDV_Integer topic_0 = this;
        return topic_0.__as_BigInteger.Equals(topic_1.__as_BigInteger);
    }

    public MDV_Boolean in_order(MDV_Integer topic_1)
    {
        MDV_Integer topic_0 = this;
        return MDV_Boolean.from(topic_0._in_order(topic_1));
    }

    internal Boolean _in_order(MDV_Integer topic_1)
    {
        MDV_Integer topic_0 = this;
        return topic_0.__as_BigInteger <= topic_1.__as_BigInteger;
    }

    public MDV_Boolean before(MDV_Integer topic_1)
    {
        MDV_Integer topic_0 = this;
        return MDV_Boolean.from(!topic_1._in_order(topic_0));
    }

    public MDV_Boolean after(MDV_Integer topic_1)
    {
        MDV_Integer topic_0 = this;
        return MDV_Boolean.from(!topic_0._in_order(topic_1));
    }

    public MDV_Boolean before_or_same(MDV_Integer topic_1)
    {
        MDV_Integer topic_0 = this;
        return MDV_Boolean.from(topic_0._in_order(topic_1));
    }

    public MDV_Boolean after_or_same(MDV_Integer topic_1)
    {
        MDV_Integer topic_0 = this;
        return MDV_Boolean.from(topic_1._in_order(topic_0));
    }

    public MDV_Integer min(MDV_Integer topic_1)
    {
        MDV_Integer topic_0 = this;
        return topic_0._in_order(topic_1) ? topic_0 : topic_1;
    }

    public MDV_Integer max(MDV_Integer topic_1)
    {
        MDV_Integer topic_0 = this;
        return topic_0._in_order(topic_1) ? topic_1 : topic_0;
    }

    public MDV_Any asset()
    {
        MDV_Integer topic = this;
        return topic;
    }

    public MDV_Integer succ()
    {
        MDV_Integer topic = this;
        return MDV_Integer.from(topic.__as_BigInteger + 1);
    }

    public MDV_Integer pred()
    {
        MDV_Integer topic = this;
        return MDV_Integer.from(topic.__as_BigInteger - 1);
    }

    public MDV_Boolean so_zero()
    {
        MDV_Integer topic = this;
        return MDV_Boolean.from(topic.__as_BigInteger.IsZero);
    }

    public MDV_Boolean not_zero()
    {
        MDV_Integer topic = this;
        return MDV_Boolean.from(!topic.__as_BigInteger.IsZero);
    }

    // greatest_common_divisor gcd

    public MDV_Integer greatest_common_divisor(MDV_Integer topic_1)
    {
        MDV_Integer topic_0 = this;
        return MDV_Integer.from(MDV_Integer._greatest_common_divisor(
            topic_0.__as_BigInteger, topic_1.__as_BigInteger));
    }

    internal static BigInteger _greatest_common_divisor(
        BigInteger topic_0, BigInteger topic_1)
    {
        // Note that greatest common divisor always has a non-negative result.
        return BigInteger.GreatestCommonDivisor(topic_0, topic_1);
    }

    // least_common_multiple lcm

    public MDV_Integer least_common_multiple(MDV_Integer topic_1)
    {
        MDV_Integer topic_0 = this;
        return MDV_Integer.from(MDV_Integer._least_common_multiple(
            topic_0.__as_BigInteger, topic_1.__as_BigInteger));
    }

    internal static BigInteger _least_common_multiple(
        BigInteger topic_0, BigInteger topic_1)
    {
        // Note that least common multiple always has a non-negative result.
        if (topic_0.IsZero || topic_1.IsZero)
        {
            return 0;
        }
        return (BigInteger.Abs(topic_0) * (
            BigInteger.Abs(topic_1)
                / BigInteger.GreatestCommonDivisor(topic_0, topic_1)
        ));
    }
}
