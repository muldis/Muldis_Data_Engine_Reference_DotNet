namespace Muldis.Data_Engine_Reference;

public sealed class MDER_Relation : MDER_Any
{
    // The virtual machine that this "value" lives in.
    private readonly MDER_Machine __machine;

    internal readonly MDER_Heading heading;
    internal readonly MDER_Set body;

    internal MDER_Relation(MDER_Machine machine, MDER_Heading heading, MDER_Set body)
    {
        this.__machine = machine;
        this.heading = heading;
        this.body = body;
    }

    public override MDER_Machine machine()
    {
        return this.__machine;
    }

    internal override String _as_MUON_Plain_Text_artifact(String indent)
    {
        return "(Relation:("
            + ((MDER_Heading)this.heading)._as_MUON_Plain_Text_artifact(indent)
            + " : "
            + this.body._as_MUON_Plain_Text_artifact(indent)
            + "))";
    }

    internal Boolean _MDER_Relation__same(MDER_Relation topic_1)
    {
        MDER_Relation topic_0 = this;
        return topic_0.heading._MDER_Any__same(topic_1.heading)
            && topic_0.body._MDER_Any__same(topic_1.body);
    }
}
