namespace Muldis.Data_Engine_Reference;

public class MDER_External : MDER_Any
{
    internal readonly Object external_value;

    internal MDER_External(MDER_Machine machine, Object external_value)
        : base(machine)
    {
        this.external_value = external_value;
    }
}
