// Muldis Data Language package `System` segment `Order`.

namespace Muldis.Data_Library;

public static class MDS_Order
{
    public static MDV_Boolean in_order(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.in_order(topic_0, topic_1);
    }

    public static MDV_Boolean before(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.before(topic_0, topic_1);
    }

    public static MDV_Boolean after(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.after(topic_0, topic_1);
    }

    public static MDV_Boolean before_or_same(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.before_or_same(topic_0, topic_1);
    }

    public static MDV_Boolean after_or_same(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.after_or_same(topic_0, topic_1);
    }

    public static MDV_Boolean min(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.min(topic_0, topic_1);
    }

    public static MDV_Boolean max(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.max(topic_0, topic_1);
    }

    public static MDV_Any asset(MDV_Boolean topic)
    {
        return MDV_Boolean.asset(topic);
    }

    public static MDV_Boolean succ(MDV_Boolean topic)
    {
        return MDV_Boolean.succ(topic);
    }

    public static MDV_Boolean pred(MDV_Boolean topic)
    {
        return MDV_Boolean.pred(topic);
    }
}
