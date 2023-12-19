namespace Muldis.Data_Engine_Reference;

public sealed class MDER_Stream : MDER_Any
{
    // The virtual machine that this "value" lives in.
    private readonly MDER_Machine __machine;

    internal MDER_Stream(MDER_Machine machine)
    {
        this.__machine = machine;
    }

    public override MDER_Machine machine()
    {
        return this.__machine;
    }

    internal override String _as_MUON_Plain_Text_artifact(String indent)
    {
        // We display something useful for debugging purposes, but no
        // (transient) MDER_Stream can actually be rendered as MUON.
        return "`Some MDER_Stream value is here.`";
    }
}
