namespace Muldis.Data_Engine_Reference;

internal class MDER_Tuple_Bag : MDER_Any
{
    internal readonly MDER_Heading heading;
    internal readonly MDER_Bag body;

    internal MDER_Tuple_Bag(MDER_Machine machine, MDER_Heading heading, MDER_Bag body)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Tuple_Bag)
    {
        this.heading = heading;
        this.body = body;
    }
}
