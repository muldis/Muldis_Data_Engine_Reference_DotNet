namespace Muldis.Data_Library;

public readonly struct MDV_Name : MDV_Orderable<MDV_Name>
{
    private static readonly MDV_Name __empty = new MDV_Name(String.Empty);

    // A value of the .NET class String is immutable.
    // It should be safe to pass around without cloning.
    private readonly String __code_point_members_as_String;

    private MDV_Name(String code_point_members_as_String)
    {
        this.__code_point_members_as_String = code_point_members_as_String;
    }

    public override Int32 GetHashCode()
    {
        return this.__code_point_members_as_String.GetHashCode();
    }

    public override String ToString()
    {
        return Internal_Identity.Name(this);
    }

    public override Boolean Equals(Object? obj)
    {
        return (obj is MDV_Name specific_obj)
            && this._same(specific_obj);
    }

    public static MDV_Name from(String code_point_members_as_String)
    {
        if (code_point_members_as_String.Equals(String.Empty))
        {
            return MDV_Name.__empty;
        }
        for (Int32 i = 0; i < code_point_members_as_String.Length; i++)
        {
            if (Char.IsSurrogate(code_point_members_as_String[i]))
            {
                if ((i + 1) < code_point_members_as_String.Length
                    && Char.IsSurrogatePair(code_point_members_as_String[i],
                    code_point_members_as_String[i + 1]))
                {
                    i++;
                }
                else
                {
                    // The .NET String is malformed;
                    // it has a non-paired Unicode surrogate code point.
                    throw new ArgumentException();
                }
            }
        }
        return new MDV_Name(code_point_members_as_String);
    }

    public static MDV_Name empty_()
    {
        return MDV_Name.__empty;
    }

    public void Deconstruct(out String code_point_members_as_String)
    {
        code_point_members_as_String = this.__code_point_members_as_String;
    }

    public String code_point_members_as_String()
    {
        return this.__code_point_members_as_String;
    }

    public MDV_Boolean same(MDV_Any topic_1)
    {
        MDV_Name topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return (topic_1 is MDV_Name specific_topic_1)
            ? MDV_Boolean.from(topic_0._same(specific_topic_1))
            : MDV_Boolean.@false();
    }

    private Boolean _same(MDV_Name topic_1)
    {
        MDV_Name topic_0 = this;
        return topic_0.__code_point_members_as_String.Equals(
            topic_1.__code_point_members_as_String);
    }

    public MDV_Boolean in_order(MDV_Orderable<MDV_Name> topic_1)
    {
        MDV_Name topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic_0._in_order((MDV_Name) topic_1));
    }

    private Boolean _in_order(MDV_Name topic_1)
    {
        MDV_Name topic_0 = this;
        return String.Compare(topic_0.__code_point_members_as_String,
            topic_1.__code_point_members_as_String,
            StringComparison.Ordinal) <= 0;
    }

    // Given that .NET is deficient in contrast with other languages
    // by not automatically exposing "default" methods of interfaces in
    // classes that compose them, every affected composing class has
    // duplicate wrapper declarations of those methods, below this statement.
    // For brevity, input checks of the wrapped methods are not duplicated.

    public MDV_Boolean not_same(MDV_Any topic_1)
    {
        MDV_Any topic_0 = this;
        return ((MDV_Any) topic_0).not_same(topic_1);
    }

    public MDV_Any coalesce(MDV_Any topic_1)
    {
        MDV_Any topic_0 = this;
        return ((MDV_Any) topic_0).coalesce(topic_1);
    }

    public MDV_Any anticoalesce(MDV_Any topic_1)
    {
        MDV_Any topic_0 = this;
        return ((MDV_Any) topic_0).anticoalesce(topic_1);
    }
}
