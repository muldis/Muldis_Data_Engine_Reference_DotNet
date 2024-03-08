namespace Muldis.Data_Library;

public readonly struct MDV_Boolean : MDV_Bicessable<MDV_Boolean>
{
    private static readonly MDV_Boolean __false = new MDV_Boolean(false);
    private static readonly MDV_Boolean __true = new MDV_Boolean(true);

    // A value of the .NET structure type Boolean is immutable.
    // It should be safe to pass around without cloning.
    private readonly Boolean __as_Boolean;

    private MDV_Boolean(Boolean as_Boolean)
    {
        this.__as_Boolean = as_Boolean;
    }

    public override Int32 GetHashCode()
    {
        return this.__as_Boolean ? 1 : 0;
    }

    public override String ToString()
    {
        return Internal_Identity.Boolean(this);
    }

    public override Boolean Equals(Object? obj)
    {
        return (obj is MDV_Boolean specific_obj)
            && this._same(specific_obj);
    }

    public static MDV_Boolean from(Boolean as_Boolean)
    {
        return as_Boolean ? MDV_Boolean.__true : MDV_Boolean.__false;
    }

    public void Deconstruct(out Boolean as_Boolean)
    {
        as_Boolean = this.__as_Boolean;
    }

    public Boolean as_Boolean()
    {
        return this.__as_Boolean;
    }

    public MDV_Boolean same(MDV_Any topic_1)
    {
        MDV_Boolean topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return (topic_1 is MDV_Boolean specific_topic_1)
            ? MDV_Boolean.from(topic_0._same(specific_topic_1))
            : MDV_Boolean.__false;
    }

    private Boolean _same(MDV_Boolean topic_1)
    {
        MDV_Boolean topic_0 = this;
        return topic_0.__as_Boolean.Equals(topic_1.__as_Boolean);
    }

    public MDV_Boolean in_order(MDV_Orderable<MDV_Boolean> topic_1)
    {
        MDV_Boolean topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic_0._in_order((MDV_Boolean) topic_1));
    }

    private Boolean _in_order(MDV_Boolean topic_1)
    {
        MDV_Boolean topic_0 = this;
        return !topic_0.__as_Boolean || topic_1.__as_Boolean;
    }

    public MDV_Any asset()
    {
        MDV_Boolean topic = this;
        return topic;
    }

    public MDV_Successable<MDV_Boolean> succ()
    {
        MDV_Boolean topic = this;
        if (topic.__as_Boolean)
        {
            // Alternate conceptually is After_All_Others.
            throw new NotImplementedException();
        }
        return MDV_Boolean.@true();
    }

    public MDV_Bicessable<MDV_Boolean> pred()
    {
        MDV_Boolean topic = this;
        if (!topic.__as_Boolean)
        {
            // Alternate conceptually is Before_All_Others.
            throw new NotImplementedException();
        }
        return MDV_Boolean.@false();
    }

    public static MDV_Boolean @false()
    {
        return MDV_Boolean.__false;
    }

    public static MDV_Boolean @true()
    {
        return MDV_Boolean.__true;
    }

    public MDV_Boolean not()
    {
        MDV_Boolean topic = this;
        return MDV_Boolean.from(!topic.__as_Boolean);
    }

    public MDV_Boolean and(MDV_Boolean topic_1)
    {
        MDV_Boolean topic_0 = this;
        return MDV_Boolean.from(topic_0.__as_Boolean && topic_1.__as_Boolean);
    }

    public MDV_Boolean nand(MDV_Boolean topic_1)
    {
        MDV_Boolean topic_0 = this;
        return MDV_Boolean.from(!topic_0.__as_Boolean || !topic_1.__as_Boolean);
    }

    public MDV_Boolean or(MDV_Boolean topic_1)
    {
        MDV_Boolean topic_0 = this;
        return MDV_Boolean.from(topic_0.__as_Boolean || topic_1.__as_Boolean);
    }

    public MDV_Boolean nor(MDV_Boolean topic_1)
    {
        MDV_Boolean topic_0 = this;
        return MDV_Boolean.from(!topic_0.__as_Boolean && !topic_1.__as_Boolean);
    }

    public MDV_Boolean xnor(MDV_Boolean topic_1)
    {
        MDV_Boolean topic_0 = this;
        return MDV_Boolean.from(topic_0.__as_Boolean.Equals(topic_1.__as_Boolean));
    }

    public MDV_Boolean xor(MDV_Boolean topic_1)
    {
        MDV_Boolean topic_0 = this;
        return MDV_Boolean.from(!topic_0.__as_Boolean.Equals(topic_1.__as_Boolean));
    }

    public MDV_Boolean imp(MDV_Boolean topic_1)
    {
        MDV_Boolean topic_0 = this;
        return topic_0.__as_Boolean ? topic_1 : MDV_Boolean.__true;
    }

    public MDV_Boolean nimp(MDV_Boolean topic_1)
    {
        MDV_Boolean topic_0 = this;
        return topic_0.imp(topic_1).not();
    }

    public MDV_Boolean @if(MDV_Boolean topic_1)
    {
        MDV_Boolean topic_0 = this;
        return topic_1.imp(topic_0);
    }

    public MDV_Boolean nif(MDV_Boolean topic_1)
    {
        MDV_Boolean topic_0 = this;
        return topic_1.nimp(topic_0);
    }
}
