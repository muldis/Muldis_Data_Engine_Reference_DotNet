namespace Muldis.Data_Engine_Reference;

public class MDER_Heading : MDER_Any
{
    internal readonly HashSet<String> attr_names;

    internal MDER_Heading(MDER_Machine machine, HashSet<String> attr_names)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Heading)
    {
        this.attr_names = attr_names;
    }
}
