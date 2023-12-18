namespace Muldis.Data_Engine_Reference;

public class MDER_External : MDER_Any
{
    internal readonly Object external_value;

    internal MDER_External(MDER_Machine machine, Object external_value)
        : base(machine)
    {
        this.external_value = external_value;
    }

    internal String _as_MUON_External_artifact()
    {
        // We display something useful for debugging purposes, but no
        // (transient) MDER_External can actually be rendered as MUON.
        return "`Some MDER_External value is here.`";
    }
}
