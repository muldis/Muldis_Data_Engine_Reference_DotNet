namespace Muldis.Data_Engine_Reference;

public sealed class MDER_True : MDER_Boolean
{
    // The virtual machine that this "value" lives in.
    private readonly MDER_Machine __machine;

    internal MDER_True(MDER_Machine machine)
    {
        this.__machine = machine;
    }

    public override MDER_Machine machine()
    {
        return this.__machine;
    }

    public override Boolean as_Boolean()
    {
        return true;
    }

    internal override String _as_MUON_Plain_Text_artifact(String indent)
    {
        return "0bTRUE";
    }
}
