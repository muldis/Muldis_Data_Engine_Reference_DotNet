namespace Muldis.Data_Engine_Reference;

public sealed class MDER_True : MDER_Boolean
{
    internal MDER_True(MDER_Machine machine)
        : base(machine, Internal_Well_Known_Base_Type.MDER_True)
    {
    }
}
