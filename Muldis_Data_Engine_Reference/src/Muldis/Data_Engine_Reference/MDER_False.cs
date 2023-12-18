namespace Muldis.Data_Engine_Reference;

public sealed class MDER_False : MDER_Boolean
{
    internal MDER_False(MDER_Machine machine) : base(machine)
    {
    }

    public override Boolean as_Boolean()
    {
        return false;
    }
}
