namespace Muldis.Data_Engine_Reference;

public class MDER_Tuple_Array : MDER_Any
{
    internal readonly MDER_Heading heading;
    internal readonly MDER_Array body;

    internal MDER_Tuple_Array(MDER_Machine machine, MDER_Heading heading, MDER_Array body)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Tuple_Array)
    {
        this.heading = heading;
        this.body = body;
    }
}
