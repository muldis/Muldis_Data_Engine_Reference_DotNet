namespace Muldis.Data_Engine_Reference;

public class MDER_Excuse : MDER_Any
{
    // The Muldis Data Language value that is the "label" of this MDER_Excuse value.
    // TODO: Change this to a MDER_Nesting.
    internal readonly MDER_Any label;

    // The Muldis Data Language value that is the "attributes" of this MDER_Excuse value.
    internal readonly MDER_Tuple attrs;

    internal MDER_Excuse(MDER_Machine machine, MDER_Any label, MDER_Tuple attrs)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Excuse)
    {
        this.label = label;
        this.attrs = attrs;
    }
}
