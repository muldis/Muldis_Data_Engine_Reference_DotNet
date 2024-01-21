using System.Numerics;

namespace Muldis.Data_Library;

internal static class Internal_Rational
{
    public static Boolean _in_order(MDV_Rational topic_0, MDV_Rational topic_1)
    {
        if (topic_0._denominator() == topic_1._denominator())
        {
            return (topic_0._numerator() <= topic_1._numerator());
        }
        // We need to compare the arguments in terms of their equivalent
        // fractions where the denominators are equal.
        // This typically means making the denominators larger, their least
        // common multiple; but first we will ensure coprime so the LCM
        // we will work with is also as small as possible.
        topic_0._ensure_normalized();
        topic_1._ensure_normalized();
        if (topic_0._denominator() == topic_1._denominator())
        {
            return (topic_0._numerator() <= topic_1._numerator());
        }
        BigInteger common_d = Internal_Integer._least_common_multiple(
            topic_0._denominator(), topic_1._denominator());
        return ((topic_0._numerator() * (common_d / topic_0._denominator()))
            <= (topic_1._numerator() * (common_d / topic_1._denominator())));
    }
}
