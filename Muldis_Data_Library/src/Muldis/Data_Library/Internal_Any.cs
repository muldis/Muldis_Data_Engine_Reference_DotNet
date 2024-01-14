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
        if (!Type.Equals(topic_0.GetType(), topic_1.GetType()))
        {
            return false;
        }
        return (topic_0, topic_1) switch
        {
            (MDV_Boolean specific_topic_0, MDV_Boolean specific_topic_1) =>
                specific_topic_0.as_Boolean() == specific_topic_1.as_Boolean(),
            _ => throw new NotImplementedException(),
        };
    }
}
