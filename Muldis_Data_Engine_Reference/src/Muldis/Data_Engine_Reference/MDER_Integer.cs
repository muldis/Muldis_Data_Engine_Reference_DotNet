using System.Numerics;

namespace Muldis.Data_Engine_Reference;

public class MDER_Integer : MDER_Any
{
    // The .NET structure type BigInteger is immutable, like the other .NET
    // numeric types, so it is safe to pass around without cloning.
    // A BigInteger is also optimized to handle small values compactly.
    // It would be counter-productive for MDER_Integer to have an alternate
    // representation like Int64, not only for the much greater maintenance
    // burden, but as it would likely use more memory or perform worse.
    private readonly BigInteger __as_BigInteger;

    internal MDER_Integer(MDER_Machine machine, BigInteger as_BigInteger)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Integer)
    {
        this.__as_BigInteger = as_BigInteger;
    }

    public Int32 as_Int32()
    {
        // This will throw a .NET OverflowException if the value doesn't fit.
        return (Int32)this.__as_BigInteger;
    }

    public Int64 as_Int64()
    {
        // This will throw a .NET OverflowException if the value doesn't fit.
        return (Int64)this.__as_BigInteger;
    }

    public BigInteger as_BigInteger()
    {
        return this.__as_BigInteger;
    }

    internal String _as_MUON_Integer_artifact()
    {
        return this.__as_BigInteger.ToString();
    }

    internal Boolean _MDER_Integer__same(MDER_Integer topic_1)
    {
        MDER_Integer topic_0 = this;
        return topic_0.__as_BigInteger == topic_1.__as_BigInteger;
    }
}
