// Muldis Data Language package `System` segment `Core`.

namespace Muldis.Data_Library;

public static class MDS_Core
{
    public static MDV_Boolean Any(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.@true();
    }

    // Selection type definer `None`.
    // `None` represents the *empty type*, which is the minimal data type
    // of the entire Muldis Data Language type system and consists of
    // exactly zero values.  It is the intersection of all other types.
    // It is a *subtype* of every other type.
    // `None` can not have a default value.

    public static MDV_Boolean None(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.@false();
    }

    public static MDV_Boolean same(MDV_Any topic_0, MDV_Any topic_1)
    {
        return MDV_Any.same(topic_0, topic_1);
    }

    public static MDV_Boolean not_same(MDV_Any topic_0, MDV_Any topic_1)
    {
        return MDV_Any.not_same(topic_0, topic_1);
    }

    public static MDV_Boolean Excuse(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Excuse);
    }

    public static MDV_Any coalesce(MDV_Any topic_0, MDV_Any topic_1)
    {
        return MDV_Excuse.coalesce(topic_0, topic_1);
    }

    public static MDV_Any anticoalesce(MDV_Any topic_0, MDV_Any topic_1)
    {
        return MDV_Excuse.anticoalesce(topic_0, topic_1);
    }

    public static MDV_Boolean Ignorance(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Ignorance);
    }

    public static MDV_Boolean Orderable(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Orderable<MDV_Any>);
    }

    public static MDV_Boolean Before_All_Others(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Before_All_Others);
    }

    public static MDV_Boolean After_All_Others(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_After_All_Others);
    }

    public static MDV_Boolean Successable(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Successable<MDV_Any>);
    }

    public static MDV_Boolean Bicessable(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Bicessable<MDV_Any>);
    }

    public static MDV_Boolean Boolable(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Boolable<MDV_Any>);
    }

    public static MDV_Boolean Boolean(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Boolean);
    }
}
