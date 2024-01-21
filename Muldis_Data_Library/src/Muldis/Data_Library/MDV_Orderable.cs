namespace Muldis.Data_Library;

public interface MDV_Orderable<Specific_T> : MDV_Any
{
    // in_order

    public static abstract MDV_Boolean in_order(Specific_T topic_0, Specific_T topic_1);
}
