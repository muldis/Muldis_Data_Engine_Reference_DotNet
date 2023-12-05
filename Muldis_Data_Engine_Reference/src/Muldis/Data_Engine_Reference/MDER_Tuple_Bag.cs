namespace Muldis.Data_Engine_Reference;

internal class MDER_Tuple_Bag : MDER_Any
{
    internal readonly MDER_Heading heading;
    internal readonly MDER_Bag body;

    internal MDER_Tuple_Bag(Internal_Memory memory, MDER_Heading heading, MDER_Bag body)
        : base(memory, Internal_Well_Known_Base_Type.MDER_Tuple_Bag)
    {
        this.heading = heading;
        this.body = body;
    }
}
