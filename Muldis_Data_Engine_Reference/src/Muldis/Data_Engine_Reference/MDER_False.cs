namespace Muldis.Data_Engine_Reference;

public sealed class MDER_False : MDER_Boolean
{
    internal MDER_False(MDER_Machine machine)
        : base(machine, Internal_Well_Known_Base_Type.MDER_False)
    {
    }

    public override Boolean as_Boolean()
    {
        return false;
    }
}
