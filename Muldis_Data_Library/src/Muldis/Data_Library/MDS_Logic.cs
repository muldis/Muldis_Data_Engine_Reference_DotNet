// Muldis Data Language package `System` segment `Logic`.

namespace Muldis.Data_Library;

public static class MDS_Logic
{
    public static MDV_Boolean so(MDV_Boolean topic)
    {
        return MDV_Boolean.so(topic);
    }

    public static MDV_Boolean not_so(MDV_Boolean topic)
    {
        return MDV_Boolean.not_so(topic);
    }

    public static MDV_Boolean @false()
    {
        return MDV_Boolean.@false();
    }

    public static MDV_Boolean @true()
    {
        return MDV_Boolean.@true();
    }

    public static MDV_Boolean not(MDV_Boolean topic)
    {
        return MDV_Boolean.not(topic);
    }

    public static MDV_Boolean and(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.and(topic_0, topic_1);
    }

    public static MDV_Boolean nand(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.nand(topic_0, topic_1);
    }

    public static MDV_Boolean or(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.or(topic_0, topic_1);
    }

    public static MDV_Boolean nor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.nor(topic_0, topic_1);
    }

    public static MDV_Boolean xnor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.xnor(topic_0, topic_1);
    }

    public static MDV_Boolean xor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.xor(topic_0, topic_1);
    }

    public static MDV_Boolean imp(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.imp(topic_0, topic_1);
    }

    public static MDV_Boolean nimp(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.nimp(topic_0, topic_1);
    }

    public static MDV_Boolean @if(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.@if(topic_0, topic_1);
    }

    public static MDV_Boolean nif(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.nif(topic_0, topic_1);
    }
}
