namespace Muldis.Data_Engine_Reference;

internal class MDL_Tuple_Bag : MDL_Any
{
    internal readonly MDL_Heading heading;
    internal readonly MDL_Bag body;

    internal MDL_Tuple_Bag(Memory memory, MDL_Heading heading, MDL_Bag body)
        : base(memory, Well_Known_Base_Type.MDL_Tuple_Bag)
    {
        this.heading = heading;
        this.body = body;
    }
}
