namespace Muldis.Data_Library;

internal static class Internal_Preview
{
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
        return topic._numerator().ToString()
            + "/" + topic._denominator().ToString();
    }
}
