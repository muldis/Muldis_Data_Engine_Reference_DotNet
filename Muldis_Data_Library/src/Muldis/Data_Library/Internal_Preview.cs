namespace Muldis.Data_Library;

internal static class Internal_Preview
{
    internal static String Ignorance()
    {
        return "0iIGNORANCE";
    }

    internal static String Boolean(MDV_Boolean topic)
    {
        return topic.as_Boolean() ? "0bTRUE" : "0bFALSE";
    }

    internal static String Integer(MDV_Integer topic)
    {
        return topic.as_BigInteger().ToString();
    }

    internal static String Rational(MDV_Rational topic)
    {
        return topic.numerator_as_BigInteger().ToString()
            + "/" + topic.denominator_as_BigInteger().ToString();
    }
}
