using System.Numerics;

namespace Muldis.Data_Library;

internal static class Internal_Integer
{
    public static MDV_Integer greatest_common_divisor(
        MDV_Integer topic_0, MDV_Integer topic_1)
    {
        return MDV_Integer.from(Internal_Integer._greatest_common_divisor(
            topic_0.as_BigInteger(), topic_1.as_BigInteger()));
    }

    public static BigInteger _greatest_common_divisor(
        BigInteger topic_0, BigInteger topic_1)
    {
        // Note that greatest common divisor always has a non-negative result.
        return BigInteger.GreatestCommonDivisor(topic_0, topic_1);
    }

    public static MDV_Integer least_common_multiple(
        MDV_Integer topic_0, MDV_Integer topic_1)
    {
        return MDV_Integer.from(Internal_Integer._least_common_multiple(
            topic_0.as_BigInteger(), topic_1.as_BigInteger()));
    }

    public static BigInteger _least_common_multiple(
        BigInteger topic_0, BigInteger topic_1)
    {
        // Note that least common multiple always has a non-negative result.
        if (topic_0.IsZero || topic_1.IsZero)
        {
            return 0;
        }
        return (BigInteger.Abs(topic_0) * (BigInteger.Abs(topic_1)
            / BigInteger.GreatestCommonDivisor(topic_0, topic_1)));
    }
}
