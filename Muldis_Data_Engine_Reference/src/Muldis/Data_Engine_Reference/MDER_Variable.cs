namespace Muldis.Data_Engine_Reference;

public sealed class MDER_Variable : MDER_Any
{
    // The MDER_Machine VM that this MDER_Any "value" lives in.
    private readonly MDER_Machine __machine;

    internal MDER_Any current_value;

    internal MDER_Variable(MDER_Machine machine, MDER_Any initial_current_value)
    {
        this.__machine = machine;
        this.current_value = initial_current_value;
    }

    public override MDER_Machine machine()
    {
        return this.__machine;
    }

    internal override String _identity_as_String()
    {
        return this._as_MUON_Plain_Text_artifact("");
    }

    internal override String _preview_as_String()
    {
        return this._as_MUON_Plain_Text_artifact("");
    }

    internal override String _as_MUON_Plain_Text_artifact(String indent)
    {
        // We display something useful for debugging purposes, but no
        // (transient) MDER_Variable can actually be rendered as MUON.
        return "`Some MDER_Variable value is here.`";
    }
}
