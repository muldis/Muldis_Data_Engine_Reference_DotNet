namespace Muldis.Data_Engine_Reference;

internal class MDER_Relation : MDER_Any
{
    internal readonly MDER_Heading heading;
    internal readonly MDER_Set body;

    internal MDER_Relation(Internal_Memory memory, MDER_Heading heading, MDER_Set body)
        : base(memory, Internal_Well_Known_Base_Type.MDER_Relation)
    {
        this.heading = heading;
        this.body = body;
    }
}
