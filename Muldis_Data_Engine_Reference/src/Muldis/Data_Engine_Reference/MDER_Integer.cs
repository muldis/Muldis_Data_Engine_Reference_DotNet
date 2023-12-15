using System.Numerics;

namespace Muldis.Data_Engine_Reference;

public class MDER_Integer : MDER_Any
{
    // While we conceptually could special case smaller integers with
    // additional fields for performance, we won't, mainly to keep
    // things simpler, and because BigInteger special-cases internally.
    private readonly BigInteger __as_BigInteger;

    internal MDER_Integer(MDER_Machine machine, BigInteger as_BigInteger)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Integer)
    {
        this.__as_BigInteger = as_BigInteger;
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
