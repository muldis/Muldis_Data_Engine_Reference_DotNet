// The interface type definer `Integral` is semifinite.  An `Integral` value
// either is an exact integral number of some kind or is something that can
// act as such.  Idiomatically an `Integral` is a pure integer which does not
// represent any kind of thing in particular, neither cardinal nor ordinal nor
// nominal; however some types which do represent such a particular kind of
// thing may choose to compose `Integral` because it makes sense to provide
// its operators.  The default value of `Integral` is the `Integer` value
// `0`.  `Integral` is both `Orderable` and `Bicessable`.  For each type
// composing `Integral`, a value closer to negative infinity is ordered
// before a value closer to positive infinity, and the definition of
// *predecessor* and *successor* is exactly equal to subtracting or adding
// an integer positive-one respectively, while other `Bicessable` don't
// generally mean that.  In the general case, `Integral` has no minimum or
// maximum value, but often a type that is `Integral` will have them.
// `Integral` is composed by `Integer`.

namespace Muldis.Data_Library;

public interface MDV_Integral<Specific_T>
    : MDV_Bicessable<Specific_T>, MDV_Numerical<Specific_T>
{
}
