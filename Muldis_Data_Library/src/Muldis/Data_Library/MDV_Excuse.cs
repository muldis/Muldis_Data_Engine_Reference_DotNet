// The interface type definer `Excuse` is semifinite.  An `Excuse`
// value is an explicitly stated reason for why, given some particular
// problem domain, a value is not being used that is ordinary for that
// domain.  For example, the typical integer division operation is not
// defined to give an integer result when the divisor is zero, and so a
// function for integer division could be defined to result in an
// `Excuse` value rather than throw an exception in that case.
// An `Excuse` is also characterized by an *exception* that is not
// meant to terminate execution of code early.

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
