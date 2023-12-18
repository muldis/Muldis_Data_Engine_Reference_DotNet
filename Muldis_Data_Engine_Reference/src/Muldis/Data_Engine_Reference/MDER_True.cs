namespace Muldis.Data_Engine_Reference;

public sealed class MDER_True : MDER_Boolean
{
    internal MDER_True(MDER_Machine machine) : base(machine)
    {
    }

    public override Boolean as_Boolean()
    {
        return true;
    }
}
