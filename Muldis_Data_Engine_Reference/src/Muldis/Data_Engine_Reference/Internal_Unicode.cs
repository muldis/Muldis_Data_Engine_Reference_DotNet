using System.Text;

namespace Muldis.Data_Engine_Reference;

// Muldis.Data_Engine_Reference.Internal_Unicode
// Provides compensations for .NET representing character data encoded with
// UTF-16 such that some Unicode code points are represented by a surrogate
// pair of String elements, and that a String may also be mal-formed with
// surrogate code points not appearing with appropriate counterparts.

internal static class Internal_Unicode
{
    internal static Internal_Unicode_Test_Result test_String(String topic)
    {
        Internal_Unicode_Test_Result ok_result
            = Internal_Unicode_Test_Result.Valid_Is_All_BMP;
        for (Int32 i = 0; i < topic.Length; i++)
        {
            if (Char.IsSurrogate(topic[i]))
            {
                if ((i+1) < topic.Length
                    && Char.IsSurrogatePair(topic[i], topic[i+1]))
                {
                    ok_result = Internal_Unicode_Test_Result.Valid_Has_Non_BMP;
                    i++;
                }
                else
                {
                    return Internal_Unicode_Test_Result.Is_Malformed;
                }
            }
        }
        return ok_result;
    }

    internal static Int32 code_point_count(
        String topic, Boolean has_any_non_BMP)
    {
        if (!has_any_non_BMP)
        {
            return topic.Length;
        }
        Int32 count = 0;
        for (Int32 i = 0; i < topic.Length; i++)
        {
            count++;
            if ((i+1) < topic.Length
                && Char.IsSurrogatePair(topic[i], topic[i+1]))
            {
                i++;
            }
        }
        return count;
    }

    internal static Int32? code_point_maybe_at(
        String topic, Boolean has_any_non_BMP, Int32 ord_pos)
    {
        if (!has_any_non_BMP)
        {
            return (ord_pos >= topic.Length) ? null : topic[ord_pos];
        }
        Int32 code_point_i = 0;
        for (Int32 i = 0; i < topic.Length; i++)
        {
            if (code_point_i == ord_pos)
            {
                if ((i+1) < topic.Length
                    && Char.IsSurrogatePair(topic[i], topic[i+1]))
                {
                    return Char.ConvertToUtf32(topic[i], topic[i+1]);
                }
                return topic[i];
            }
            code_point_i++;
            if ((i+1) < topic.Length
                && Char.IsSurrogatePair(topic[i], topic[i+1]))
            {
                i++;
            }
        }
        return null;
    }

    internal static String? maybe_String_from_UTF_8_array_of_Byte(Byte[] topic)
    {
        UTF8Encoding enc = new UTF8Encoding
        (
            encoderShouldEmitUTF8Identifier: false,
            throwOnInvalidBytes: true
        );
        try
        {
            return enc.GetString(topic);
        }
        catch
        {
            return null;
        }
    }
}
