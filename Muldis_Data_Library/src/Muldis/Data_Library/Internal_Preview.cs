namespace Muldis.Data_Library;

internal static class Internal_Preview
{
    internal static String Boolean(MDV_Boolean topic)
    {
        return topic.as_Boolean() ? "0bTRUE" : "0bFALSE";
    }
}
