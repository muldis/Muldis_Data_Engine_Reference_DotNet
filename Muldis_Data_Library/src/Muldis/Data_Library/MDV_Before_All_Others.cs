namespace Muldis.Data_Library;

public readonly struct MDV_Before_All_Others
    : MDV_Excuse, MDV_Orderable<MDV_Before_All_Others>
{
    private static readonly MDV_Before_All_Others __only
        = new MDV_Before_All_Others();

    public override Int32 GetHashCode()
    {
        return 0;
    }

    public override String ToString()
    {
        return Internal_Preview.Before_All_Others();
    }

    public override Boolean Equals(Object? obj)
    {
        return obj is MDV_Before_All_Others;
    }

    public static MDV_Before_All_Others only()
    {
        return MDV_Before_All_Others.__only;
    }

    public static MDV_Boolean in_order(MDV_Before_All_Others topic_0, MDV_Before_All_Others topic_1)
    {
        return MDV_Boolean.@true();
    }
}
