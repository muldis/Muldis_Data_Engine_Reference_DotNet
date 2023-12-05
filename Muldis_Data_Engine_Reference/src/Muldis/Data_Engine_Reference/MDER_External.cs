namespace Muldis.Data_Engine_Reference;

internal class MDER_External : MDER_Any
{
    internal readonly Object external_value;

    internal MDER_External(MDER_Machine machine, Object external_value)
        : base(machine, Internal_Well_Known_Base_Type.MDER_External)
    {
        this.external_value = external_value;
    }
}
