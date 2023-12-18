namespace Muldis.Data_Engine_Reference;

public class MDER_Stream : MDER_Any
{
    internal MDER_Stream(MDER_Machine machine) : base(machine)
    {
    }

    internal String _as_MUON_Stream_artifact()
    {
        // We display something useful for debugging purposes, but no
        // (transient) MDER_Stream can actually be rendered as MUON.
        return "`Some MDER_Stream value is here.`";
    }
}
