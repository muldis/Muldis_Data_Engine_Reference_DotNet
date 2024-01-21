// The interface type definer `Boolable` is semifinite.  A `Boolable` value has a
// canonical way of being cast to a `Boolean` value in a context-free manner,
// as the answer to the non-specific question "Is that so?" on the value taken
// in isolation, whatever that would conceivably mean for the value's type.
// The idiomatic predicate being asked has to do with whether or not something
// exists; for composing numeric types it is asking whether the number is
// nonzero; for composing collection types it is asking whether the collection
// has any members.  The primary reason for `Boolable` is to provide an easy
// consistent and terse way to ask a common predicate question such as this.
// The default value of `Boolable` is `False`.
//
// `Boolable` is composed, directly or indirectly, by: `Boolean`,
// `Numerical`, `Integral`, `Integer`, `Fractional`, `Rational`,
// `Emptyable`, `Stringy`, `Bits`, `Blob`, `Textual`, `Text`,
// `Homogeneous`, `Unionable`, `Discrete`, `Positional`, `Array`,
// `Set`, `Bag`, `Relational`, `Orderelation`, `Relation`,
// `Multirelation`, `Intervalish`, `Interval`, `Unionable_Intervalish`,
// `Set_Of_Interval`, `Bag_Of_Interval`.
//
// While conceivably `Boolable` could also be composed by `Attributive`, and
// hence `Tuple`, it isn't because that would set up a semantic conflict for
// `Relation` and `Multirelation` which are collections across 2 dimensions, and
// it was decided for those latter types that `Boolable` would apply to them
// explicitly in their `Homogeneous` dimension (by way of `Emptyable`) and
// not in their `Attributive` dimension.  As such, the `Attributive` functions
// `has_any_attrs` and `is_nullary` are provided as that dimension's direct
// analogies to the `Homogeneous` dimension's `Boolable`-implementing
// `has_any_members` (`to_Boolean`/`so`) and `is_empty` (`not_so`) functions.

namespace Muldis.Data_Library;

public interface MDV_Boolable<Specific_T> : MDV_Any
{
    // so ? to_Boolean

    public static abstract MDV_Boolean so(Specific_T topic);

    // not_so !?

    public static abstract MDV_Boolean not_so(Specific_T topic);
}
