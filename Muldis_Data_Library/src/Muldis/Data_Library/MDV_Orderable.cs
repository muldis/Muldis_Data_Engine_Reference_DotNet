namespace Muldis.Data_Library;

public interface MDV_Orderable<Specific_T> : MDV_Any
{
    public abstract MDV_Boolean in_order(MDV_Orderable<Specific_T> topic_1);

    public MDV_Boolean before(MDV_Orderable<Specific_T> topic_1)
    {
        MDV_Orderable<Specific_T> topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_1.in_order(topic_0).not();
    }

    public MDV_Boolean after(MDV_Orderable<Specific_T> topic_1)
    {
        MDV_Orderable<Specific_T> topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.in_order(topic_1).not();
    }

    public MDV_Boolean before_or_same(MDV_Orderable<Specific_T> topic_1)
    {
        MDV_Orderable<Specific_T> topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.in_order(topic_1);
    }

    public MDV_Boolean after_or_same(MDV_Orderable<Specific_T> topic_1)
    {
        MDV_Orderable<Specific_T> topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_1.in_order(topic_0);
    }

    public MDV_Orderable<Specific_T> min(MDV_Orderable<Specific_T> topic_1)
    {
        MDV_Orderable<Specific_T> topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.in_order(topic_1).as_Boolean() ? topic_0 : topic_1;
    }

    public MDV_Orderable<Specific_T> max(MDV_Orderable<Specific_T> topic_1)
    {
        MDV_Orderable<Specific_T> topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.in_order(topic_1).as_Boolean() ? topic_1 : topic_0;
    }
}
