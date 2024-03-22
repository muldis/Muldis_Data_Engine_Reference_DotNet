namespace Muldis.Data_Library;

public static class MDP_System<Specific_T>
{
    public static MDV_Boolean Any(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.@true();
    }

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
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.same(topic_1);
    }

    public static MDV_Boolean not_same(MDV_Any topic_0, MDV_Any topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.not_same(topic_1);
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
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.coalesce(topic_1);
    }

    public static MDV_Any anticoalesce(MDV_Any topic_0, MDV_Any topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.anticoalesce(topic_1);
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

    public static MDV_Boolean in_order(
        MDV_Orderable<Specific_T> topic_0, MDV_Orderable<Specific_T> topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.in_order(topic_1);
    }

    public static MDV_Boolean before(
        MDV_Orderable<Specific_T> topic_0, MDV_Orderable<Specific_T> topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.before(topic_1);
    }

    public static MDV_Boolean after(
        MDV_Orderable<Specific_T> topic_0, MDV_Orderable<Specific_T> topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.after(topic_1);
    }

    public static MDV_Boolean before_or_same(
        MDV_Orderable<Specific_T> topic_0, MDV_Orderable<Specific_T> topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.before_or_same(topic_1);
    }

    public static MDV_Boolean after_or_same(
        MDV_Orderable<Specific_T> topic_0, MDV_Orderable<Specific_T> topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.after_or_same(topic_1);
    }

    public static MDV_Orderable<Specific_T> min(
        MDV_Orderable<Specific_T> topic_0, MDV_Orderable<Specific_T> topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.min(topic_1);
    }

    public static MDV_Orderable<Specific_T> max(
        MDV_Orderable<Specific_T> topic_0, MDV_Orderable<Specific_T> topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.max(topic_1);
    }

    public static MDV_Boolean Boolean(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Boolean);
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
        return topic.not();
    }

    public static MDV_Boolean and(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return topic_0.and(topic_1);
    }

    public static MDV_Boolean nand(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return topic_0.nand(topic_1);
    }

    public static MDV_Boolean or(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return topic_0.or(topic_1);
    }

    public static MDV_Boolean nor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return topic_0.nor(topic_1);
    }

    public static MDV_Boolean xnor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return topic_0.xnor(topic_1);
    }

    public static MDV_Boolean xor(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return topic_0.xor(topic_1);
    }

    public static MDV_Boolean imp(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return topic_0.imp(topic_1);
    }

    public static MDV_Boolean nimp(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return topic_0.nimp(topic_1);
    }

    public static MDV_Boolean @if(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return topic_0.@if(topic_1);
    }

    public static MDV_Boolean nif(MDV_Boolean topic_0, MDV_Boolean topic_1)
    {
        return topic_0.nif(topic_1);
    }

    public static MDV_Boolean Numerical(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Numerical<MDV_Any>);
    }

    public static MDV_Boolean so_zero(MDV_Numerical<Specific_T> topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return topic.so_zero();
    }

    public static MDV_Boolean not_zero(MDV_Numerical<Specific_T> topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return topic.not_zero();
    }

    public static MDV_Numerical<Specific_T> zero(MDV_Numerical<Specific_T> topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return topic.zero();
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
        return topic_0.greatest_common_divisor(topic_1);
    }

    public static MDV_Integer least_common_multiple(
        MDV_Integer topic_0, MDV_Integer topic_1)
    {
        return topic_0.least_common_multiple(topic_1);
    }

    public static MDV_Boolean Rational(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Rational);
    }

    public static MDV_Boolean Homogeneous(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Homogeneous<Specific_T>);
    }

    public static MDV_Boolean so_empty(MDV_Homogeneous<Specific_T> topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return topic.so_empty();
    }

    public static MDV_Boolean not_empty(MDV_Homogeneous<Specific_T> topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return topic.not_empty();
    }

    public static MDV_Homogeneous<Specific_T> empty(MDV_Homogeneous<Specific_T> topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return topic.empty();
    }

    public static MDV_Boolean Positional(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Positional<Specific_T>);
    }

    public static MDV_Boolean Text(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Text);
    }

    public static MDV_Boolean Name(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Name);
    }
}
