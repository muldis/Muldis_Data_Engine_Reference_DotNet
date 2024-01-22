// The interface type definer `Fractional` is semifinite.  A `Fractional` value
// either is a rational exact numeric of some kind, expressible as a coprime
// *numerator* / *denominator* pair of `Integral` whose *denominator* is
// positive, or is something that can act as such.  Idiomatically a
// `Fractional` is a pure rational number which does not represent any kind
// of thing in particular, neither cardinal nor ordinal nor nominal; however
// some types which do represent such a particular kind of thing may choose to
// compose `Fractional` because it makes sense to provide its operators.  The
// default value of `Fractional` is the `Rational` value `0.0`.
// `Fractional` is `Orderable`; for each type composing `Fractional`, a
// value closer to negative infinity is ordered before a value closer to
// positive infinity.  In the general case it is not `Bicessable` nor does it
// have a minimum or maximum value, but sometimes a type that is `Fractional`
// will have either of those.  `Fractional` is composed by `Rational`.

namespace Muldis.Data_Library;

public interface MDV_Fractional<Specific_T>
    : MDV_Orderable<Specific_T>, MDV_Numerical<Specific_T>
{
}
