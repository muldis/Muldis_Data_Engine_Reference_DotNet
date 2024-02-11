// The interface type definer `Positional` is semifinite.  A `Positional` value is
// a `Discrete` value; it is a homogeneous
// aggregate of other, *member* values that are arranged in an explicit total
// order and can both be enumerated in that order as well as be looked up by
// integral ordinal position against that order; there is a single
// canonical interpretation of where each *member* begins and ends within the
// aggregate.  A `Positional` value is dense, meaning that every one of its
// members exists at a distinct adjacent integral position; a `Positional`
// preserves the identity that the count of its members is equal to one plus
// the difference of its first and last ordinal positions.  Idiomatically, a
// `Positional` value is zero-based, meaning its first-ordered member is at
// ordinal position `0`, but that doesn't have to be the case.  `Positional`
// requires that for every composing type definer *T* there is a single integral
// value *P* such that the first ordinal position of every value of *T* is
// *P*; as such, any catenation or slice of `Positional` values would have
// well-definined shifting of member values between ordinal positions.
//
// The default value of `Positional` is the `Array` value with zero members,
// `[]`.  `Positional` is `Orderable` in the general case conditionally
// depending on whether all of its member values are mutually `Orderable`
// themselves; its minimum value is the same `[]` as its default value; it
// has no maximum value.  The ordering algorithm of `Positional` is based on
// pairwise comparison of its members by matching ordinal position starting at the lowest
// ordinal position; iff `Positional` value X is a leading sub-sequence of `Positional`
// value Y, then X is ordered before Y; otherwise, the mutual ordering of the
// lowest-ordinal-positioned non-matching members of X and Y determines that the ordering
// of X and Y as a whole are the same as said members.
//
// `Positional` is composed, directly or indirectly, by: `Bits`, `Blob`,
// `Textual`, `Text`, `Array`, `Orderelation`.

namespace Muldis.Data_Library;

public interface MDV_Positional<Specific_T>
    : MDV_Orderable<Specific_T>, MDV_Emptyable<Specific_T>
{
}
