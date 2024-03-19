namespace Muldis.Data_Library;

public interface MDV_Successable<Specific_T> : MDV_Any
{
    public abstract MDV_Any asset();

    public MDV_Successable<Specific_T> succ()
    {
        MDV_Successable<Specific_T> topic = this;
        return topic.nth_succ(MDV_Integer.positive_one());
    }

    public abstract MDV_Successable<Specific_T> nth_succ(MDV_Integer topic_1);
}
