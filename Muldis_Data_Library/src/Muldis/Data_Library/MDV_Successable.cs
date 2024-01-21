namespace Muldis.Data_Library;

public interface MDV_Successable<Specific_T> : MDV_Any
{
    // asset

    public static abstract MDV_Any asset(Specific_T topic);

    // succ

    public static abstract Specific_T succ(Specific_T topic);
}
