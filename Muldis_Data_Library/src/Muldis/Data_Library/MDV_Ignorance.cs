namespace Muldis.Data_Library;

public readonly struct MDV_Ignorance : MDV_Any, MDV_Excuse
{
    private static readonly MDV_Ignorance __only = new MDV_Ignorance();

    public override Int32 GetHashCode()
    {
        return 0;
    }

    public override String ToString()
    {
        return Internal_Preview.Ignorance();
    }

    public override Boolean Equals(Object? obj)
    {
        return obj is MDV_Ignorance;
    }

    public static MDV_Ignorance only()
    {
        return MDV_Ignorance.__only;
    }
}
