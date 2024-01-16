namespace Muldis.Data_Library;

internal static class Internal_Any
{
    internal static Boolean _same(MDV_Any topic_0, MDV_Any topic_1)
    {
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
            (MDV_Before_All_Others specific_topic_0,
                MDV_Before_All_Others specific_topic_1) =>
                // We should never get here; however...
                true,
            (MDV_After_All_Others specific_topic_0,
                MDV_After_All_Others specific_topic_1) =>
                // We should never get here; however...
                true,
            (MDV_Boolean specific_topic_0, MDV_Boolean specific_topic_1) =>
                specific_topic_0.as_Boolean() == specific_topic_1.as_Boolean(),
            (MDV_Integer specific_topic_0, MDV_Integer specific_topic_1) =>
                Internal_Integer._same(specific_topic_0, specific_topic_1),
            (MDV_Rational specific_topic_0, MDV_Rational specific_topic_1) =>
                Internal_Rational._same(specific_topic_0, specific_topic_1),
            _ => throw new NotImplementedException(),
        };
    }
}
