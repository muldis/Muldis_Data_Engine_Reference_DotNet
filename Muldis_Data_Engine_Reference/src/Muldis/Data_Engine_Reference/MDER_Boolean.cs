namespace Muldis.Data_Engine_Reference;

public sealed class MDER_Boolean : MDER_Any
{
    // The MDER_Machine VM that this MDER_Any "value" lives in.
    private readonly MDER_Machine __machine;

    private readonly Boolean __as_Boolean;

    internal MDER_Boolean(MDER_Machine machine, Boolean as_Boolean)
    {
        this.__machine = machine;
        this.__as_Boolean = as_Boolean;
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

    public Boolean as_Boolean()
    {
        return this.__as_Boolean;
    }

    internal override String _as_MUON_Plain_Text_artifact(String indent)
    {
        return this.__as_Boolean ? "0bTRUE" : "0bFALSE";
    }

    internal Boolean _MDER_Boolean__same(MDER_Boolean topic_1)
    {
        MDER_Boolean topic_0 = this;
        return topic_0.__as_Boolean == topic_1.__as_Boolean;
    }
}
