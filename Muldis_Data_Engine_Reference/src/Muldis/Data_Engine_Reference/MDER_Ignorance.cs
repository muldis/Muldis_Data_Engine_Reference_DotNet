namespace Muldis.Data_Engine_Reference;

public sealed class MDER_Ignorance : MDER_Any
{
    // The virtual machine that this "value" lives in.
    private readonly MDER_Machine __machine;

    internal MDER_Ignorance(MDER_Machine machine)
    {
        this.__machine = machine;
    }

    public override MDER_Machine machine()
    {
        return this.__machine;
    }
}
