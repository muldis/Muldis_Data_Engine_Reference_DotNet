using System.Text;
using System.Text.RegularExpressions;

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

    internal static String Text(MDV_Text topic)
    {
        return Internal_Preview._Text(topic.code_point_members_as_String());
    }

    private static String _Text(String topic)
    {
        if (topic.Equals(String.Empty))
        {
            return "\"\"";
        }
        if (topic.Length == 1 && ((Int32)(Char)topic[0]) <= 0x1F)
        {
            // Format as a code-point-text.
            return "0t" + ((Int32)(Char)topic[0]);
        }
        if (Regex.IsMatch(topic, @"\A[A-Za-z_][A-Za-z_0-9]*\z"))
        {
            // Format as a nonquoted-alphanumeric-text.
            return topic;
        }
        // Else, format as a quoted text.
        if (String.Equals(topic, "") || !Regex.IsMatch(topic,
            "[\u0000-\u001F\"\\`\u007F-\u009F]"))
        {
            return "\"" + topic + "\"";
        }
        StringBuilder sb = new StringBuilder(topic.Length);
        for (Int32 i = 0; i < topic.Length; i++)
        {
            Int32 c = (Int32)(Char)topic[i];
            switch (c)
            {
                case 0x7:
                    sb.Append(@"\a");
                    break;
                case 0x8:
                    sb.Append(@"\b");
                    break;
                case 0x9:
                    sb.Append(@"\t");
                    break;
                case 0xA:
                    sb.Append(@"\n");
                    break;
                case 0xB:
                    sb.Append(@"\v");
                    break;
                case 0xC:
                    sb.Append(@"\f");
                    break;
                case 0xD:
                    sb.Append(@"\r");
                    break;
                case 0x1B:
                    sb.Append(@"\e");
                    break;
                case 0x22:
                    sb.Append(@"\q");
                    break;
                case 0x5C:
                    sb.Append(@"\k");
                    break;
                case 0x60:
                    sb.Append(@"\g");
                    break;
                default:
                    if (c <= 0x1F || (c >= 0x7F && c <= 0x9F))
                    {
                        sb.Append(@"\(0t" + c.ToString() + ")");
                    }
                    else
                    {
                        sb.Append((Char)c);
                    }
                    break;
            }
        }
        return "\"" + sb.ToString() + "\"";
    }
}
