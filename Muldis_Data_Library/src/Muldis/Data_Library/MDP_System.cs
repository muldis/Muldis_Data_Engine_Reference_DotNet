// Muldis Data Language package `System`.

namespace Muldis.Data_Library;

public static class MDP_System
{
    public static MDV_Boolean Any(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.@true();
    }

    // Selection type definer `None`.
    // `None` represents the *empty type*, which is the minimal data type
    // of the entire Muldis Data Language type system and consists of
    // exactly zero values.  It is the intersection of all other types.
    // It is a *subtype* of every other type.
    // `None` can not have a default value.

    public static MDV_Boolean None(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.@false();
    }

    public static MDV_Boolean same(MDV_Any topic_0, MDV_Any topic_1)
    {
        return MDV_Any.same(topic_0, topic_1);
    }

    public static MDV_Boolean not_same(MDV_Any topic_0, MDV_Any topic_1)
    {
        return MDV_Any.not_same(topic_0, topic_1);
    }

    public static MDV_Boolean Excuse(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Excuse);
    }

    public static MDV_Any coalesce(MDV_Any topic_0, MDV_Any topic_1)
    {
        return MDV_Excuse.coalesce(topic_0, topic_1);
    }

    public static MDV_Any anticoalesce(MDV_Any topic_0, MDV_Any topic_1)
    {
        return MDV_Excuse.anticoalesce(topic_0, topic_1);
    }

    public static MDV_Boolean Ignorance(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Ignorance);
    }

    public static MDV_Boolean Orderable(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Orderable<MDV_Any>);
    }

    public static MDV_Boolean Before_All_Others(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Before_All_Others);
    }

    public static MDV_Boolean After_All_Others(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_After_All_Others);
    }

    public static MDV_Boolean Successable(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Successable<MDV_Any>);
    }

    public static MDV_Boolean Bicessable(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Bicessable<MDV_Any>);
    }

    public static MDV_Boolean Boolable(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Boolable<MDV_Any>);
    }

    public static MDV_Boolean Boolean(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Boolean);
    }

    public static MDV_Boolean in_order(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.in_order(topic_0, topic_1);
    }

    public static MDV_Boolean before(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.before(topic_0, topic_1);
    }

    public static MDV_Boolean after(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.after(topic_0, topic_1);
    }

    public static MDV_Boolean before_or_same(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.before_or_same(topic_0, topic_1);
    }

    public static MDV_Boolean after_or_same(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.after_or_same(topic_0, topic_1);
    }

    public static MDV_Boolean min(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.min(topic_0, topic_1);
    }

    public static MDV_Boolean max(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.max(topic_0, topic_1);
    }

    public static MDV_Any asset(MDV_Boolean topic)
    {
        return MDV_Boolean.asset(topic);
    }

    public static MDV_Boolean succ(MDV_Boolean topic)
    {
        return MDV_Boolean.succ(topic);
    }

    public static MDV_Boolean pred(MDV_Boolean topic)
    {
        return MDV_Boolean.pred(topic);
    }

    public static MDV_Boolean so(MDV_Boolean topic)
    {
        return MDV_Boolean.so(topic);
    }

    public static MDV_Boolean not_so(MDV_Boolean topic)
    {
        return MDV_Boolean.not_so(topic);
    }

    public static MDV_Boolean @false()
    {
        return MDV_Boolean.@false();
    }

    public static MDV_Boolean @true()
    {
        return MDV_Boolean.@true();
    }

    public static MDV_Boolean not(MDV_Boolean topic)
    {
        return MDV_Boolean.not(topic);
    }

    public static MDV_Boolean and(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.and(topic_0, topic_1);
    }

    public static MDV_Boolean nand(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.nand(topic_0, topic_1);
    }

    public static MDV_Boolean or(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.or(topic_0, topic_1);
    }

    public static MDV_Boolean nor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.nor(topic_0, topic_1);
    }

    public static MDV_Boolean xnor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.xnor(topic_0, topic_1);
    }

    public static MDV_Boolean xor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.xor(topic_0, topic_1);
    }

    public static MDV_Boolean imp(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.imp(topic_0, topic_1);
    }

    public static MDV_Boolean nimp(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.nimp(topic_0, topic_1);
    }

    public static MDV_Boolean @if(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.@if(topic_0, topic_1);
    }

    public static MDV_Boolean nif(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return MDV_Boolean.nif(topic_0, topic_1);
    }

    public static MDV_Boolean Numerical(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Numerical<MDV_Any>);
    }

    public static MDV_Boolean Integral(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Integral<MDV_Any>);
    }

    public static MDV_Boolean Integer(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Integer);
    }

    public static MDV_Integer greatest_common_divisor(
        MDV_Integer topic_0, MDV_Integer topic_1)
    {
        return MDV_Integer.greatest_common_divisor(topic_0, topic_1);
    }

    public static MDV_Integer least_common_multiple(
        MDV_Integer topic_0, MDV_Integer topic_1)
    {
        return MDV_Integer.least_common_multiple(topic_0, topic_1);
    }

    public static MDV_Boolean Fractional(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Fractional<MDV_Any>);
    }

    public static MDV_Boolean Rational(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Rational);
    }
}
