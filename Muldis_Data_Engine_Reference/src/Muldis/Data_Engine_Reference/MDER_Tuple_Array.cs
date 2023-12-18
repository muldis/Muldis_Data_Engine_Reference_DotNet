namespace Muldis.Data_Engine_Reference;

public class MDER_Tuple_Array : MDER_Any
{
    internal readonly MDER_Heading heading;
    internal readonly MDER_Array body;

    internal MDER_Tuple_Array(MDER_Machine machine, MDER_Heading heading, MDER_Array body)
        : base(machine)
    {
        this.heading = heading;
        this.body = body;
    }

    internal String _as_MUON_Tuple_Array_artifact(String indent)
    {
        return "(Tuple_Array:("
            + ((MDER_Heading)this.heading)._as_MUON_Heading_artifact()
            + " : "
            + this.body._as_MUON_Any_artifact(indent)
            + "))";
    }

    internal Boolean _MDER_Tuple_Array__same(MDER_Tuple_Array topic_1)
    {
        MDER_Tuple_Array topic_0 = this;
        return topic_0.heading._MDER_Any__same(topic_1.heading)
            && topic_0.body._MDER_Any__same(topic_1.body);
    }
}
