namespace Muldis.Data_Engine_Reference.Internal;

internal class MDL_Relation : MDL_NQA
{
    internal readonly MDL_Heading heading;
    internal readonly MDL_Set body;

    internal MDL_Relation(Memory memory, MDL_Heading heading, MDL_Set body)
        : base(memory, Well_Known_Base_Type.MDL_Relation)
    {
        this.heading = heading;
        this.body = body;
    }
}
