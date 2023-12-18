namespace Muldis.Data_Engine_Reference;

public class MDER_Variable : MDER_Any
{
    internal MDER_Any current_value;

    internal MDER_Variable(MDER_Machine machine, MDER_Any initial_current_value)
        : base(machine)
    {
        this.current_value = initial_current_value;
    }

    internal String _as_MUON_Variable_artifact()
    {
        // We display something useful for debugging purposes, but no
        // (transient) MDER_Variable can actually be rendered as MUON.
        return "`Some MDER_Variable value is here.`";
    }
}
