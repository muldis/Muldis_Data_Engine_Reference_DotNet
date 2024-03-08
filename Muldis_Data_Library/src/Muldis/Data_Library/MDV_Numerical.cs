namespace Muldis.Data_Library;

public interface MDV_Numerical<Specific_T> : MDV_Any
{
    public abstract MDV_Boolean so_zero();

    public MDV_Boolean not_zero()
    {
        MDV_Numerical<Specific_T> topic = this;
        return topic.so_zero().not();
    }

    public abstract MDV_Numerical<Specific_T> zero();
}
