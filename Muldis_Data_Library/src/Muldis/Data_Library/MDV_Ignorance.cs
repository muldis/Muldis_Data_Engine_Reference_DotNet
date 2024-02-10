// Singleton type definer `Ignorance`.
// Represents foundation type `fdn::Ignorance`.
// The `Ignorance` value represents the `Excuse` value which
// simply says that an ordinary value for any given domain is missing
// and that there is simply no excuse that has been given for this; in
// other words, something has gone wrong without the slightest hint of
// an explanation.  This is conceptually the most generic `Excuse`
// value there is, to help with expedient development, but any uses
// should be considered technical debt, to be replaced later.
// `Ignorance` is a finite type.
// `Ignorance` has a default value of `0iIGNORANCE`.

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

    public MDV_Boolean same(MDV_Any topic_1)
    {
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic_1 is MDV_Ignorance);
    }
}
