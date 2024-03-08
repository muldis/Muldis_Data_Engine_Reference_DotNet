namespace Muldis.Data_Library;

public interface MDV_Successable<Specific_T> : MDV_Any
{
    public abstract MDV_Any asset();

    public abstract MDV_Successable<Specific_T> succ();
}
