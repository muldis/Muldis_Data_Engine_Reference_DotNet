// The interface type definer `Numerical` is semifinite.  A `Numerical` value
// either is a simple number of some kind or is something that can act as a
// simple number.  A *simple number* means, typically speaking, any rational
// real number, those numbers that can be derived simply by multiplying or
// dividing integers.  All operators defined by the `System` package
// for `Numerical` are expect to be closed over the real rational numbers,
// and consist mainly of addition, subtraction, multiplication, and division
// of any rationals, plus exponentiation of any rationals to integer powers
// only.  Idiomatically a `Numerical` is a pure number which does not
// represent any kind of thing in particular, neither cardinal nor ordinal nor
// nominal; however some types which do represent such a particular kind of
// thing may choose to compose `Numerical` because it makes sense to provide
// its operators.  The default value of `Numerical` is the `Integer` value
// `0`.  A `Numerical` in the general case is not `Orderable`, but often a
// type that is numeric is also orderable.
//
// `Numerical` is composed, directly or indirectly, by: `Integral`,
// `Integer`, `Fractional`, `Rational`, `Quantitative`, `Quantity`.
// It is also composed by a lot of additional type definers defined by other
// Muldis Data Language packages such as `System::Math`;
// these include types for irrational or algebraic or complex numbers or
// quaternions or rational types with a fixed precision or scale or
// floating-point types and so on.

namespace Muldis.Data_Library;

public interface MDV_Numerical<Specific_T> : MDV_Any
{
    // so_zero

    public abstract MDV_Boolean so_zero();

    // not_zero

    public MDV_Boolean not_zero()
    {
        MDV_Numerical<Specific_T> topic = this;
        return topic.so_zero().not();
    }
}
