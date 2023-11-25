namespace Muldis.Data_Engine_Reference.Internal;

internal class MDL_Tuple_Bag : MDL_NQA
{
    internal readonly MDL_Heading heading;
    internal readonly MDL_Any body;

    internal MDL_Tuple_Bag(Memory memory, MDL_Heading heading, MDL_Any body)
        : base(memory, Well_Known_Base_Type.MDL_Tuple_Bag)
    {
        this.heading = heading;
        this.body = body;
    }
}
