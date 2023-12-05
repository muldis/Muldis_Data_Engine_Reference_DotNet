namespace Muldis.Data_Engine_Reference;

internal class MDER_Tuple : MDER_Any
{
    internal readonly Dictionary<String, MDER_Any> attrs;

    internal MDER_Tuple(MDER_Machine machine, Dictionary<String, MDER_Any> attrs)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Tuple)
    {
        this.attrs = attrs;
    }
}
