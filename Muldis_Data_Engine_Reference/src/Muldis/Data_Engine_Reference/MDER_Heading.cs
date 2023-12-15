namespace Muldis.Data_Engine_Reference;

public class MDER_Heading : MDER_Any
{
    internal readonly HashSet<String> attr_names;

    internal MDER_Heading(MDER_Machine machine, HashSet<String> attr_names)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Heading)
    {
        this.attr_names = attr_names;
    }

    internal String _as_MUON_Heading_artifact()
    {
        return Object.ReferenceEquals(this, this.machine().MDER_Tuple_D0) ? "(Heading:{})"
            : "(Heading:{"
                + String.Concat(Enumerable.Select(
                        Enumerable.OrderBy(this.attr_names, a => a),
                        a => this._from_String_as_MUON_Text_artifact(a) + ","))
                + "})";
    }

    internal Boolean _MDER_Heading__same(MDER_Heading topic_1)
    {
        MDER_Heading topic_0 = this;
        HashSet<String> atnms0 = topic_0.attr_names;
        HashSet<String> atnms1 = topic_1.attr_names;
        return (atnms0.Count == atnms1.Count)
            && Enumerable.All(atnms0, atnm => atnms1.Contains(atnm));
    }
}
