namespace Muldis.Data_Engine_Reference.Internal;

internal class MDL_Tuple_Array : MDL_NQA
{
    internal readonly MDL_Heading heading;
    internal readonly MDL_Array body;

    internal MDL_Tuple_Array(Memory memory, MDL_Heading heading, MDL_Array body)
        : base(memory, Well_Known_Base_Type.MDL_Tuple_Array)
    {
        this.heading = heading;
        this.body = body;
    }
}
