// Muldis Data Language package `System` segment `Logic`.

namespace Muldis.Data_Library;

public static class MDS_Logic
{
    // so ? to_Boolean
    // public static MDV_Boolean so(MDV_Boolable topic)

    public static MDV_Boolean so(MDV_Boolean topic)
    {
        return topic;
    }

    // not_so !?
    // public static MDV_Boolean not_so(MDV_Boolable topic)

    public static MDV_Boolean not_so(MDV_Boolean topic)
    {
        return MDV_Boolean.from(!topic.as_Boolean());
    }

    // false ⊥

    public static MDV_Boolean @false()
    {
        return MDV_Boolean.@false();
    }

    // true ⊤

    public static MDV_Boolean @true()
    {
        return MDV_Boolean.@true();
    }

    // not ! ¬

    public static MDV_Boolean not(MDV_Boolean topic)
    {
        return MDV_Boolean.from(!topic.as_Boolean());
    }

    // and ∧

    public static MDV_Boolean and(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(topic_0.as_Boolean() && topic_1.as_Boolean());
    }

    // nand not_and ⊼ ↑

    public static MDV_Boolean nand(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(!topic_0.as_Boolean() || !topic_1.as_Boolean());
    }

    // or ∨

    public static MDV_Boolean or(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(topic_0.as_Boolean() || topic_1.as_Boolean());
    }

    // nor not_or ⊽ ↓

    public static MDV_Boolean nor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(!topic_0.as_Boolean() && !topic_1.as_Boolean());
    }

    // xnor iff ↔

    public static MDV_Boolean xnor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(topic_0.as_Boolean() == topic_1.as_Boolean());
    }

    // xor ⊻ ↮

    public static MDV_Boolean xor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(topic_0.as_Boolean() != topic_1.as_Boolean());
    }

    // imp implies →

    public static MDV_Boolean imp(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return topic_0.as_Boolean() ? topic_1 : MDV_Boolean.@true();
    }

    // nimp not_implies ↛

    public static MDV_Boolean nimp(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDS_Logic.not(MDS_Logic.imp(topic_0, topic_1));
    }

    // if ←

    public static MDV_Boolean @if(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDS_Logic.imp(topic_1, topic_0);
    }

    // nif not_if ↚

    public static MDV_Boolean nif(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDS_Logic.nimp(topic_1, topic_0);
    }
}
