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

    public MDV_Boolean same(MDV_Any topic_1)
    {
        MDV_Any topic_0 = this;
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic_0._same(topic_1));
    }

    // not_same != ≠

    public MDV_Boolean not_same(MDV_Any topic_1)
    {
        MDV_Any topic_0 = this;
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(!topic_0._same(topic_1));
    }

    internal Boolean _same(MDV_Any topic_1)
    {
        MDV_Any topic_0 = this;
        // Note: We can't rely on Object.ReferenceEquals() to recognize 2
        // .NET structure values as being the same since they would have
        // been autoboxed on the way into this context; however, this may
        // return true for some structures as with some non-structures.
        if (Object.ReferenceEquals(topic_0, topic_1))
        {
            return true;
        }
        // TODO: Reinforce this check to be resilient in the face of
        // possible subclassing of the built-in types that might result in
        // this returning false in error were two operands somehow created
        // in different ways either by way of or not the same subclass.
        if (!Type.Equals(topic_0.GetType(), topic_1.GetType()))
        {
            return false;
        }
        return (topic_0, topic_1) switch
        {
            (MDV_Ignorance specific_topic_0, MDV_Ignorance specific_topic_1) =>
                // We should never get here; however...
                true,
            (MDV_Boolean specific_topic_0, MDV_Boolean specific_topic_1) =>
                specific_topic_0._specific_same(specific_topic_1),
            (MDV_Integer specific_topic_0, MDV_Integer specific_topic_1) =>
                specific_topic_0._specific_same(specific_topic_1),
            (MDV_Rational specific_topic_0, MDV_Rational specific_topic_1) =>
                specific_topic_0._specific_same(specific_topic_1),
            _ => throw new NotImplementedException(),
        };
    }

    // coalesce ??

    public MDV_Any coalesce(MDV_Any topic_1)
    {
        MDV_Any topic_0 = this;
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return (topic_0 is MDV_Excuse) ? topic_1 : topic_0;
    }

    // anticoalesce !!

    public MDV_Any anticoalesce(MDV_Any topic_1)
    {
        MDV_Any topic_0 = this;
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return (topic_0 is MDV_Excuse) ? topic_0 : topic_1;
    }
}
