namespace Muldis.Data_Engine_Reference;

public class MDER_Article : MDER_Any
{
    // The Muldis Data Language value that is the "label" of this MDER_Article value.
    // TODO: Change this to a MDER_Nesting.
    internal readonly MDER_Any label;

    // The Muldis Data Language value that is the "attributes" of this MDER_Article value.
    internal readonly MDER_Tuple attrs;

    internal MDER_Article(MDER_Machine machine, MDER_Any label, MDER_Tuple attrs)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Article)
    {
        this.label = label;
        this.attrs = attrs;
    }

    internal String _as_MUON_Article_artifact(String indent)
    {
        // TODO: Change Article so represented as Nesting+Kit pair.
        return "(Article:("
            + this.label._as_MUON_Any_artifact(indent)
            + " : "
            + this.attrs._as_MUON_Any_artifact(indent)
            + "))";
    }

    internal Boolean _MDER_Article__same(MDER_Article value_1)
    {
        MDER_Article value_0 = this;
        return value_0.label._MDER_Any__same(value_1.label)
            && value_0.attrs._MDER_Any__same(value_1.attrs);
    }
}
