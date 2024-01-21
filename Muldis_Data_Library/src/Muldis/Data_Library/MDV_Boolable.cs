namespace Muldis.Data_Library;

public interface MDV_Boolable<Specific_T> : MDV_Any
{
    // so ? to_Boolean

    public static abstract MDV_Boolean so(Specific_T topic);

    // not_so !?

    public static abstract MDV_Boolean not_so(Specific_T topic);
}
