using System.Numerics;

namespace Muldis.Data_Engine_Reference.Internal;

internal class MDL_Integer : MDL_NQA
{
    // While we conceptually could special case smaller integers with
    // additional fields for performance, we won't, mainly to keep
    // things simpler, and because BigInteger special-cases internally.
    internal readonly BigInteger as_BigInteger;

    internal MDL_Integer(Memory memory, BigInteger as_BigInteger)
        : base(memory, Well_Known_Base_Type.MDL_Integer)
    {
        this.as_BigInteger = as_BigInteger;
    }
}
