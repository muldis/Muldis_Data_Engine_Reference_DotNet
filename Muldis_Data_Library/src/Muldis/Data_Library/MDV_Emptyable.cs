// The interface type definer `Emptyable` is semifinite.  An `Emptyable`
// value is an aggregate value that can have either zero or more than zero
// components.  The primary reason for `Emptyable` is to provide easy
// consistent and terse ways to ask if an aggregate has any values, or to ask
// for the value with no members of the same type as a given aggregate value.
// The default value of `Emptyable` is the `Bits` value with zero members.
//
// `Emptyable` is composed, directly or indirectly, by:
// `Homogeneous`, `Unionable`, `Discrete`, `Positional`, `Bits`,
// `Blob`, `Textual`, `Text`, `Array`, `Set`, `Bag`, `Relational`,
// `Orderelation`, `Relation`, `Multirelation`, `Intervalish`, `Interval`,
// `Unionable_Intervalish`, `Set_Of_Interval`, `Bag_Of_Interval`.

namespace Muldis.Data_Library;

public interface MDV_Emptyable<Specific_T> : MDV_Any
{
    // so_empty

    public abstract MDV_Boolean so_empty();

    // not_empty

    public MDV_Boolean not_empty()
    {
        MDV_Emptyable<Specific_T> topic = this;
        return topic.so_empty().not();
    }

    // empty

    public abstract MDV_Emptyable<Specific_T> empty();
}
