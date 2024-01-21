namespace Muldis.Data_Library;

public interface MDV_Orderable<Specific_T> : MDV_Any
{
    // in_order

    public static abstract MDV_Boolean in_order(Specific_T topic_0, Specific_T topic_1);

    // before <

    public static abstract MDV_Boolean before(Specific_T topic_0, Specific_T topic_1);

    // after >

    public static abstract MDV_Boolean after(Specific_T topic_0, Specific_T topic_1);

    // before_or_same <= ≤

    public static abstract MDV_Boolean before_or_same(Specific_T topic_0, Specific_T topic_1);

    // after_or_same >= ≥

    public static abstract MDV_Boolean after_or_same(Specific_T topic_0, Specific_T topic_1);

    // min

    public static abstract Specific_T min(Specific_T topic_0, Specific_T topic_1);

    // max

    public static abstract Specific_T max(Specific_T topic_0, Specific_T topic_1);
}
