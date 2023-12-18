namespace Muldis.Data_Engine_Reference;

public abstract class MDER_Boolean : MDER_Any
{
    internal MDER_Boolean(MDER_Machine machine) : base(machine)
    {
    }

    public abstract Boolean as_Boolean();
}
