namespace Muldis.Data_Engine_Reference;

public class MDER_Tuple_Bag : MDER_Any
{
    internal readonly MDER_Heading heading;
    internal readonly MDER_Bag body;

    internal MDER_Tuple_Bag(MDER_Machine machine, MDER_Heading heading, MDER_Bag body)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Tuple_Bag)
    {
        this.heading = heading;
        this.body = body;
    }

    internal String _as_MUON_Tuple_Bag_artifact(String indent)
    {
        return "(Tuple_Bag:("
            + ((MDER_Heading)this.heading)._as_MUON_Heading_artifact()
            + " : "
            + this.body._as_MUON_Any_artifact(indent)
            + "))";
    }

    internal Boolean _MDER_Tuple_Bag__same(MDER_Tuple_Bag value_1)
    {
        MDER_Tuple_Bag value_0 = this;
        return value_0.heading._MDER_Any__same(value_1.heading)
            && value_0.body._MDER_Any__same(value_1.body);
    }
}
