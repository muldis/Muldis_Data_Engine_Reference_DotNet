// Muldis Data Language package `System` segment `Order`.

namespace Muldis.Data_Library;

public static class MDS_Order
{
    public static MDV_Boolean in_order(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.in_order(topic_0, topic_1);
    }

    // before <
    // public static MDV_Boolean before(MDV_Orderable topic_0, MDV_Orderable topic_1)

    public static MDV_Boolean before(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(!MDV_Boolean._in_order(topic_1, topic_0));
    }

    // after >
    // public static MDV_Boolean after(MDV_Orderable topic_0, MDV_Orderable topic_1)

    public static MDV_Boolean after(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(!MDV_Boolean._in_order(topic_0, topic_1));
    }

    // before_or_same <= ≤
    // public static MDV_Boolean before_or_same(MDV_Orderable topic_0, MDV_Orderable topic_1)

    public static MDV_Boolean before_or_same(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(MDV_Boolean._in_order(topic_0, topic_1));
    }

    // after_or_same >= ≥
    // public static MDV_Boolean after_or_same(MDV_Orderable topic_0, MDV_Orderable topic_1)

    public static MDV_Boolean after_or_same(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(MDV_Boolean._in_order(topic_1, topic_0));
    }

    // min
    // public static MDV_Orderable min(MDV_Orderable topic_0, MDV_Orderable topic_1)

    public static MDV_Boolean min(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean._in_order(topic_0, topic_1) ? topic_0 : topic_1;
    }

    // max
    // public static MDV_Orderable max(MDV_Orderable topic_0, MDV_Orderable topic_1)

    public static MDV_Boolean max(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean._in_order(topic_0, topic_1) ? topic_1 : topic_0;
    }

    // asset
    // public static MDV_Any asset(MDV_Successable topic)

    public static MDV_Boolean asset(MDV_Boolean topic)
    {
        return topic;
    }

    // succ
    // public static MDV_Successable|MDV_After_All_Others succ(MDV_Successable topic)

    public static MDV_Any succ(MDV_Boolean topic)
    {
        return !topic.as_Boolean() ? MDV_Boolean.@true()
            : MDV_After_All_Others.only();
    }

    // pred
    // public static MDV_Bicessable|MDV_Before_All_Others pred(MDV_Bicessable topic)

    public static MDV_Any pred(MDV_Boolean topic)
    {
        return topic.as_Boolean() ? MDV_Boolean.@false()
           : MDV_Before_All_Others.only();
    }
}
