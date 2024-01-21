using System.Numerics;

namespace Muldis.Data_Library;

public readonly struct MDV_Integer : MDV_Integral<MDV_Integer>
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
            ? Internal_Integer._same(this, specific_obj)
            : false;
    }

    public static MDV_Integer from(BigInteger as_BigInteger)
    {
        return as_BigInteger.IsZero ? MDV_Integer.__zero
            : as_BigInteger.IsOne ? MDV_Integer.__positive_one
            : as_BigInteger == -1 ? MDV_Integer.__negative_one
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
}
