using System.Numerics;

namespace Muldis.Data_Engine_Reference;

public class MDER_Integer : MDER_Any
{
    // While we conceptually could special case smaller integers with
    // additional fields for performance, we won't, mainly to keep
    // things simpler, and because BigInteger special-cases internally.
    internal readonly BigInteger as_BigInteger;

    internal MDER_Integer(MDER_Machine machine, BigInteger as_BigInteger)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Integer)
    {
        this.as_BigInteger = as_BigInteger;
    }

    internal String _as_MUON_Integer_artifact()
    {
        return this.as_BigInteger.ToString();
    }
}
