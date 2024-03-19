namespace Muldis.Data_Library;

public interface MDV_Bicessable<Specific_T>
    : MDV_Orderable<Specific_T>, MDV_Successable<Specific_T>
{
    public MDV_Bicessable<Specific_T> pred()
    {
        MDV_Bicessable<Specific_T> topic = this;
        return topic.nth_pred(MDV_Integer.positive_one());
    }

    public abstract MDV_Bicessable<Specific_T> nth_pred(MDV_Integer topic_1);
}
