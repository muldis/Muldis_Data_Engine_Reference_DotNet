namespace Muldis.Data_Library;

public readonly struct MDV_Ignorance : MDV_Excuse
{
    private static readonly MDV_Ignorance __only = new MDV_Ignorance();

    public override Int32 GetHashCode()
    {
        return 0;
    }

    public override String ToString()
    {
        return Internal_Identity.Ignorance();
    }

    public override Boolean Equals(Object? obj)
    {
        return obj is MDV_Ignorance;
    }

    public static MDV_Ignorance only()
    {
        return MDV_Ignorance.__only;
    }

    public MDV_Boolean same(MDV_Any topic_1)
    {
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic_1 is MDV_Ignorance);
    }
}
