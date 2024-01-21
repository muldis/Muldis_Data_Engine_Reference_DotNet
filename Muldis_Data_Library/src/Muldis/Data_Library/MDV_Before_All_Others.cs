// Singleton type definer `Before_All_Others`.
// Represents foundation type `fdn::Before_All_Others`.
// The `Before_All_Others` value represents a type-agnostic analogy of
// negative infinity, an `Orderable` value that sorts *before* all
// other values in the Muldis Data Language type system, and that is
// its only meaning.  This value is expressly *not* meant to represent
// any specific mathematical or physical concept of *infinity* or `∞`
// (of which there are many), including those of the IEEE
// floating-point standards.
// `Before_All_Others` is a finite type.
// `Before_All_Others` has a default value of ...TODO...

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

    public static MDV_Boolean before(MDV_Before_All_Others topic_0, MDV_Before_All_Others topic_1)
    {
        return MDV_Boolean.@false();
    }

    public static MDV_Boolean after(MDV_Before_All_Others topic_0, MDV_Before_All_Others topic_1)
    {
        return MDV_Boolean.@false();
    }

    public static MDV_Boolean before_or_same(MDV_Before_All_Others topic_0, MDV_Before_All_Others topic_1)
    {
        return MDV_Boolean.@true();
    }

    public static MDV_Boolean after_or_same(MDV_Before_All_Others topic_0, MDV_Before_All_Others topic_1)
    {
        return MDV_Boolean.@true();
    }

    public static MDV_Before_All_Others min(MDV_Before_All_Others topic_0, MDV_Before_All_Others topic_1)
    {
        return topic_0;
    }

    public static MDV_Before_All_Others max(MDV_Before_All_Others topic_0, MDV_Before_All_Others topic_1)
    {
        return topic_0;
    }
}
