namespace Muldis.Data_Engine_Reference;

internal class MDER_Tuple_Array : MDER_Any
{
    internal readonly MDER_Heading heading;
    internal readonly MDER_Array body;

    internal MDER_Tuple_Array(Internal_Memory memory, MDER_Heading heading, MDER_Array body)
        : base(memory, Internal_Well_Known_Base_Type.MDER_Tuple_Array)
    {
        this.heading = heading;
        this.body = body;
    }
}
