namespace Muldis.Data_Engine_Reference;

public abstract class MDER_Boolean : MDER_Any
{
    internal MDER_Boolean(MDER_Machine machine, Internal_Well_Known_Base_Type WKBT)
        : base(machine, WKBT)
    {
    }

    public abstract Boolean as_Boolean();
}
