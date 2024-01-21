namespace Muldis.Data_Library;

public readonly struct MDV_Boolean
    : MDV_Bicessable<MDV_Boolean>, MDV_Boolable<MDV_Boolean>
{
    private static readonly MDV_Boolean __false = new MDV_Boolean(false);
    private static readonly MDV_Boolean __true = new MDV_Boolean(true);

    // A value of the .NET structure type Boolean is immutable.
    // It should be safe to pass around without cloning.
    private readonly Boolean __as_Boolean;

    private MDV_Boolean(Boolean as_Boolean)
    {
        this.__as_Boolean = as_Boolean;
    }

    public override Int32 GetHashCode()
    {
        return this.__as_Boolean ? 1 : 0;
    }

    public override String ToString()
    {
        return Internal_Preview.Boolean(this);
    }

    public override Boolean Equals(Object? obj)
    {
        return (obj is MDV_Boolean specific_obj)
            ? MDV_Boolean._specific_same(this, specific_obj)
            : false;
    }

    public static MDV_Boolean from(Boolean as_Boolean)
    {
        return as_Boolean ? MDV_Boolean.__true : MDV_Boolean.__false;
    }

    public Boolean as_Boolean()
    {
        return this.__as_Boolean;
    }

    internal static Boolean _specific_same(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return topic_0.__as_Boolean == topic_1.__as_Boolean;
    }

    public static MDV_Boolean in_order(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(MDV_Boolean._in_order(topic_0, topic_1));
    }

    internal static Boolean _in_order(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return !topic_0.__as_Boolean || topic_1.__as_Boolean;
    }

    public static MDV_Boolean before(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(!MDV_Boolean._in_order(topic_1, topic_0));
    }

    public static MDV_Boolean after(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(!MDV_Boolean._in_order(topic_0, topic_1));
    }

    public static MDV_Boolean before_or_same(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(MDV_Boolean._in_order(topic_0, topic_1));
    }

    public static MDV_Boolean after_or_same(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(MDV_Boolean._in_order(topic_1, topic_0));
    }

    public static MDV_Boolean min(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean._in_order(topic_0, topic_1) ? topic_0 : topic_1;
    }

    public static MDV_Boolean max(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean._in_order(topic_0, topic_1) ? topic_1 : topic_0;
    }

    public static MDV_Any asset(MDV_Boolean topic)
    {
        return topic;
    }

    public static MDV_Boolean succ(MDV_Boolean topic)
    {
        return !topic.__as_Boolean ? MDV_Boolean.@true()
            : throw new NotImplementedException();
        // Alternate conceptually is MDV_After_All_Others.
    }

    public static MDV_Boolean pred(MDV_Boolean topic)
    {
        return topic.__as_Boolean ? MDV_Boolean.@false()
            : throw new NotImplementedException();
        // Alternate conceptually is MDV_Before_All_Others.
    }

    public static MDV_Boolean so(MDV_Boolean topic)
    {
        return topic;
    }

    public static MDV_Boolean not_so(MDV_Boolean topic)
    {
        return MDV_Boolean.from(!topic.__as_Boolean);
    }

    // false ⊥

    public static MDV_Boolean @false()
    {
        return MDV_Boolean.__false;
    }

    // true ⊤

    public static MDV_Boolean @true()
    {
        return MDV_Boolean.__true;
    }

    // not ! ¬

    public static MDV_Boolean not(MDV_Boolean topic)
    {
        return MDV_Boolean.from(!topic.__as_Boolean);
    }

    // and ∧

    public static MDV_Boolean and(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(topic_0.__as_Boolean && topic_1.__as_Boolean);
    }

    // nand not_and ⊼ ↑

    public static MDV_Boolean nand(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(!topic_0.__as_Boolean || !topic_1.__as_Boolean);
    }

    // or ∨

    public static MDV_Boolean or(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(topic_0.__as_Boolean || topic_1.__as_Boolean);
    }

    // nor not_or ⊽ ↓

    public static MDV_Boolean nor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(!topic_0.__as_Boolean && !topic_1.__as_Boolean);
    }

    // xnor iff ↔

    public static MDV_Boolean xnor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(topic_0.__as_Boolean == topic_1.__as_Boolean);
    }

    // xor ⊻ ↮

    public static MDV_Boolean xor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(topic_0.__as_Boolean != topic_1.__as_Boolean);
    }

    // imp implies →

    public static MDV_Boolean imp(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return topic_0.__as_Boolean ? topic_1 : MDV_Boolean.__true;
    }

    // nimp not_implies ↛

    public static MDV_Boolean nimp(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.not(MDV_Boolean.imp(topic_0, topic_1));
    }

    // if ←

    public static MDV_Boolean @if(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.imp(topic_1, topic_0);
    }

    // nif not_if ↚

    public static MDV_Boolean nif(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.nimp(topic_1, topic_0);
    }
}
