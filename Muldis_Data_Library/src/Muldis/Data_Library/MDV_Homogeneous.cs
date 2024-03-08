namespace Muldis.Data_Library;

public interface MDV_Homogeneous<Specific_T> : MDV_Any
{
    public abstract MDV_Boolean so_empty();

    public MDV_Boolean not_empty()
    {
        MDV_Homogeneous<Specific_T> topic = this;
        return topic.so_empty().not();
    }

    public abstract MDV_Homogeneous<Specific_T> empty();
}
