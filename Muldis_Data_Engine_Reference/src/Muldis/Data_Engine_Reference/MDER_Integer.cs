using System.Numerics;

namespace Muldis.Data_Engine_Reference;

internal class MDER_Integer : MDER_Any
{
    // While we conceptually could special case smaller integers with
    // additional fields for performance, we won't, mainly to keep
    // things simpler, and because BigInteger special-cases internally.
    internal readonly BigInteger as_BigInteger;

    internal MDER_Integer(Memory memory, BigInteger as_BigInteger)
        : base(memory, Well_Known_Base_Type.MDER_Integer)
    {
        this.as_BigInteger = as_BigInteger;
    }
}
