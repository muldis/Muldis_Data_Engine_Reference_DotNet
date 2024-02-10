// Selection type definer `Any`.
// `Any` represents the *universal type*, which is the maximal data type
// of the entire Muldis Data Language type system and consists of all
// values which can possibly exist.  It is the union of all other types.
// It is a *supertype* of every other type.
// `Any` is an infinite type.
// `Any` has a default value of `0bFALSE`.

namespace Muldis.Data_Library;

public interface MDV_Any
{
    // same =

    public abstract MDV_Boolean same(MDV_Any topic_1);

    // not_same != ≠

    public MDV_Boolean not_same(MDV_Any topic_1)
    {
        MDV_Any topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.same(topic_1).not();
    }

    // coalesce ??

    public MDV_Any coalesce(MDV_Any topic_1)
    {
        MDV_Any topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return (topic_0 is MDV_Excuse) ? topic_1 : topic_0;
    }

    // anticoalesce !!

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
