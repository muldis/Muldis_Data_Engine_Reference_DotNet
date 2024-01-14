// Muldis Data Language package `System` segment `Order`.

namespace Muldis.Data_Library;

public static class MDS_Order
{
    // in_order
    // public static MDV_Boolean in_order(MDV_Orderable topic_0, MDV_Orderable topic_1)

    public static MDV_Boolean in_order(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(!topic_0.as_Boolean() || topic_1.as_Boolean());
    }

    // before <
    // public static MDV_Boolean before(MDV_Orderable topic_0, MDV_Orderable topic_1)

    public static MDV_Boolean before(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(!topic_0.as_Boolean() && topic_1.as_Boolean());
    }

    // after >
    // public static MDV_Boolean after(MDV_Orderable topic_0, MDV_Orderable topic_1)

    public static MDV_Boolean after(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(topic_0.as_Boolean() && !topic_1.as_Boolean());
    }

    // before_or_same <= ≤
    // public static MDV_Boolean before_or_same(MDV_Orderable topic_0, MDV_Orderable topic_1)

    public static MDV_Boolean before_or_same(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(!topic_0.as_Boolean() || topic_1.as_Boolean());
    }

    // after_or_same >= ≥
    // public static MDV_Boolean after_or_same(MDV_Orderable topic_0, MDV_Orderable topic_1)

    public static MDV_Boolean after_or_same(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from( topic_0.as_Boolean() || !topic_1.as_Boolean());
    }

    // min
    // public static MDV_Orderable min(MDV_Orderable topic_0, MDV_Orderable topic_1)

    public static MDV_Boolean min(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(topic_0.as_Boolean() && topic_1.as_Boolean());
    }

    // max
    // public static MDV_Orderable max(MDV_Orderable topic_0, MDV_Orderable topic_1)

    public static MDV_Boolean max(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.from(topic_0.as_Boolean() || topic_1.as_Boolean());
    }

    // asset
    // public static MDV_Any asset(MDV_Successable topic_1)

    public static MDV_Boolean asset(MDV_Boolean topic)
    {
        return topic;
    }

    // succ
    // public static MDV_Successable succ(MDV_Successable topic_1)

    public static MDV_Boolean succ(MDV_Boolean topic)
    {
        if (!topic.as_Boolean())
        {
            MDV_Boolean.@true();
        }
        // TODO: Return excuse After_All_Others.
        throw new NotImplementedException();
    }

    // pred
    // public static MDV_Bicessable pred(MDV_Bicessable topic)

    public static MDV_Boolean pred(MDV_Boolean topic)
    {
        if (topic.as_Boolean())
        {
            MDV_Boolean.@false();
        }
        // TODO: Return excuse Before_All_Others.
        throw new NotImplementedException();
    }
}
