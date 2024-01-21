namespace Muldis.Data_Library;

public interface MDV_Excuse : MDV_Any
{
    // coalesce ??

    public static MDV_Any coalesce(MDV_Any topic_0, MDV_Any topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return (topic_0 is MDV_Excuse) ? topic_1 : topic_0;
    }

    // anticoalesce !!

    public static MDV_Any anticoalesce(MDV_Any topic_0, MDV_Any topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return (topic_0 is MDV_Excuse) ? topic_0 : topic_1;
    }
}
