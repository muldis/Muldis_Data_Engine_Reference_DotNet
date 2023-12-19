namespace Muldis.Data_Engine_Reference;

public sealed class MDER_Process : MDER_Any
{
    internal MDER_Process(MDER_Machine machine) : base(machine)
    {
    }

    internal String _as_MUON_Process_artifact()
    {
        // We display something useful for debugging purposes, but no
        // (transient) MDER_Process can actually be rendered as MUON.
        return "`Some MDER_Process value is here.`";
    }
}
