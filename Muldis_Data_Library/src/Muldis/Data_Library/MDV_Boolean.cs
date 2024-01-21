namespace Muldis.Data_Library;

public readonly struct MDV_Boolean
    : MDV_Bicessable<MDV_Boolean>, MDV_Boolable<MDV_Boolean>
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
        return Internal_Preview.Boolean(this);
    }

    public override Boolean Equals(Object? obj)
    {
        return (obj is MDV_Boolean specific_obj)
            ? this.__as_Boolean == specific_obj.__as_Boolean
            : false;
    }

    public static MDV_Boolean from(Boolean as_Boolean)
    {
        return as_Boolean ? MDV_Boolean.__true : MDV_Boolean.__false;
    }

    public static MDV_Boolean @false()
    {
        return MDV_Boolean.__false;
    }

    public static MDV_Boolean @true()
    {
        return MDV_Boolean.__true;
    }

    public Boolean as_Boolean()
    {
        return this.__as_Boolean;
    }
}
