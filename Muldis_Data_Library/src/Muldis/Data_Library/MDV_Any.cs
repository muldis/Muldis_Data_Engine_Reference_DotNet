namespace Muldis.Data_Library;

public interface MDV_Any
{
    public abstract MDV_Boolean same(MDV_Any topic_1);

    public MDV_Boolean not_same(MDV_Any topic_1)
    {
        MDV_Any topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.same(topic_1).not();
    }

    public MDV_Any coalesce(MDV_Any topic_1)
    {
        MDV_Any topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return (topic_0 is MDV_Excuse) ? topic_1 : topic_0;
    }

    public MDV_Any anticoalesce(MDV_Any topic_1)
    {
        MDV_Any topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return (topic_0 is MDV_Excuse) ? topic_0 : topic_1;
    }
}
