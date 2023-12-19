namespace Muldis.Data_Engine_Reference;

public sealed class MDER_Tuple : MDER_Any
{
    internal readonly Dictionary<String, MDER_Any> attrs;

    internal MDER_Tuple(MDER_Machine machine, Dictionary<String, MDER_Any> attrs)
        : base(machine)
    {
        this.attrs = attrs;
    }

    internal String _as_MUON_Tuple_artifact(String indent)
    {
        String ati = indent + "\u0009";
        return Object.ReferenceEquals(this, this.machine().MDER_Tuple_D0) ? "(Tuple:{})"
            : "(Tuple:{\u000A"
                + String.Concat(Enumerable.Select(
                        Enumerable.OrderBy(this.attrs, a => a.Key),
                        a => ati + this._from_String_as_MUON_Text_artifact(a.Key) + " : "
                            + a.Value._as_MUON_Any_artifact(ati) + ",\u000A"))
                + indent + "})";
    }

    internal Boolean _MDER_Tuple__same(MDER_Tuple topic_1)
    {
        MDER_Tuple topic_0 = this;
        Dictionary<String, MDER_Any> attrs0 = topic_0.attrs;
        Dictionary<String, MDER_Any> attrs1 = topic_1.attrs;
        // First test just that the Tuple headings are the same,
        // and only if they are, compare the attribute values.
        return (attrs0.Count == attrs1.Count)
            && Enumerable.All(attrs0, attr => attrs1.ContainsKey(attr.Key))
            && Enumerable.All(attrs0, attr => attr.Value._MDER_Any__same(attrs1[attr.Key]));
    }
}
