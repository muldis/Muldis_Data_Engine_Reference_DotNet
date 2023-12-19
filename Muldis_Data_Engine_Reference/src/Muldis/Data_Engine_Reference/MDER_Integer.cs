using System.Numerics;

namespace Muldis.Data_Engine_Reference;

public sealed class MDER_Integer : MDER_Any
{
    // The virtual machine that this "value" lives in.
    private readonly MDER_Machine __machine;

    // A value of the .NET structure type BigInteger is immutable.
    // It should be safe to pass around without cloning.
    // A BigInteger is also optimized to handle small values compactly.
    // It would be counter-productive for MDER_Integer to have an alternate
    // representation like Int64, not only for the much greater maintenance
    // burden, but as it would likely use more memory or perform worse.
    private readonly BigInteger __as_BigInteger;

    internal MDER_Integer(MDER_Machine machine, BigInteger as_BigInteger)
    {
        this.__machine = machine;
        this.__as_BigInteger = as_BigInteger;
    }

    public override MDER_Machine machine()
    {
        return this.__machine;
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
