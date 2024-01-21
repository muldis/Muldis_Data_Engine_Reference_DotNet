// Singleton type definer `After_All_Others`.
// Represents foundation type `fdn::After_All_Others`.
// The `After_All_Others` value represents a type-agnostic analogy of
// positive infinity, an `Orderable` value that sorts *after* all
// other values in the Muldis Data Language type system, and that is
// its only meaning.  This value is expressly *not* meant to represent
// any specific mathematical or physical concept of *infinity* or `∞`
// (of which there are many), including those of the IEEE
// floating-point standards.
// `After_All_Others` is a finite type.
// `After_All_Others` has a default value of ...TODO...

namespace Muldis.Data_Library;

public readonly struct MDV_After_All_Others
    : MDV_Excuse, MDV_Orderable<MDV_After_All_Others>
{
    private static readonly MDV_After_All_Others __only
        = new MDV_After_All_Others();

    public override Int32 GetHashCode()
    {
        return 0;
    }

    public override String ToString()
    {
        return Internal_Preview.After_All_Others();
    }

    public override Boolean Equals(Object? obj)
    {
        return obj is MDV_After_All_Others;
    }

    public static MDV_After_All_Others only()
    {
        return MDV_After_All_Others.__only;
    }

    public static MDV_Boolean in_order(MDV_After_All_Others topic_0, MDV_After_All_Others topic_1)
    {
        return MDV_Boolean.@true();
    }

    public static MDV_Boolean before(MDV_After_All_Others topic_0, MDV_After_All_Others topic_1)
    {
        return MDV_Boolean.@false();
    }

    public static MDV_Boolean after(MDV_After_All_Others topic_0, MDV_After_All_Others topic_1)
    {
        return MDV_Boolean.@false();
    }

    public static MDV_Boolean before_or_same(MDV_After_All_Others topic_0, MDV_After_All_Others topic_1)
    {
        return MDV_Boolean.@true();
    }

    public static MDV_Boolean after_or_same(MDV_After_All_Others topic_0, MDV_After_All_Others topic_1)
    {
        return MDV_Boolean.@true();
    }

    public static MDV_After_All_Others min(MDV_After_All_Others topic_0, MDV_After_All_Others topic_1)
    {
        return topic_0;
    }

    public static MDV_After_All_Others max(MDV_After_All_Others topic_0, MDV_After_All_Others topic_1)
    {
        return topic_0;
    }
}
