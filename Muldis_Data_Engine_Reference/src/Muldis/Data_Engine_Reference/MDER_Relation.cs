namespace Muldis.Data_Engine_Reference;

public class MDER_Relation : MDER_Any
{
    internal readonly MDER_Heading heading;
    internal readonly MDER_Set body;

    internal MDER_Relation(MDER_Machine machine, MDER_Heading heading, MDER_Set body)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Relation)
    {
        this.heading = heading;
        this.body = body;
    }

    internal String _as_MUON_Relation_artifact(String indent)
    {
        return "(Relation:("
            + ((MDER_Heading)this.heading)._as_MUON_Heading_artifact()
            + " : "
            + this.body._as_MUON_Any_artifact(indent)
            + "))";
    }

    internal Boolean _MDER_Relation__same(MDER_Relation topic_1)
    {
        MDER_Relation topic_0 = this;
        return topic_0.heading._MDER_Any__same(topic_1.heading)
            && topic_0.body._MDER_Any__same(topic_1.body);
    }
}
