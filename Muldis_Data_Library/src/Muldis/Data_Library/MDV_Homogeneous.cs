// The interface type definer `Homogeneous` is semifinite.  A `Homogeneous` value
// is a *collective* value such that every one of its component *members* is
// conceptually of a common data type with its fellow members or should be
// interpreted logically in the same way.  Idiomatically a `Homogeneous` is a
// generic collection which does not as a whole represent any kind of thing in
// particular, such as a text or a graphic, and is simply the sum of its
// *members*; however some types which do represent such a particular kind of
// thing may choose to compose `Homogeneous` because it makes sense to
// provide its operators.  The default value of `Homogeneous` is the `Array`
// value with zero members, `[]`.
//
// If a `Homogeneous` value is also `Unionable`, then another value of its
// collection type can be derived by either inserting new members whose values
// are distinct from those already in the collection or by removing arbitrary
// members from the collection; otherwise, that may not be possible.
// If a `Homogeneous` value is also `Discrete`, all of its members can be
// enumerated as individuals and counted; otherwise, that may not be possible.
// If a `Homogeneous` value is also `Positional`, all of its members are
// arranged in an explicit total order and can both be enumerated in that
// order as well as be looked up by integral ordinal position against
// that order; otherwise, that may not be possible.  If a `Homogeneous` value
// is also `Setty`, all of its members are guaranteed to be distinct values;
// otherwise, duplication of values may occur amongst members.
//
// `Homogeneous` is composed, directly or indirectly, by:
// `Unionable`, `Discrete`, `Positional`,
// `Bits`, `Blob`, `Textual`, `Text`, `Array`, `Set`, `Bag`,
// `Relational`, `Orderelation`, `Relation`, `Multirelation`, `Intervalish`,
// `Interval`, `Unionable_Intervalish`, `Set_Of_Interval`, `Bag_Of_Interval`.

namespace Muldis.Data_Library;

public interface MDV_Homogeneous<Specific_T> : MDV_Any
{
    // so_empty

    public abstract MDV_Boolean so_empty();

    // not_empty

    public MDV_Boolean not_empty()
    {
        MDV_Homogeneous<Specific_T> topic = this;
        return topic.so_empty().not();
    }

    // empty

    public abstract MDV_Homogeneous<Specific_T> empty();
}
