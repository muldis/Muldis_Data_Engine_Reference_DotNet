// The interface type definer `Orderable` is semifinite.  An `Orderable` value has
// all of the traditional comparison operators defined for it, such that
// for each composing type *T* of `Orderable`, the set of values of *T*
// can be deterministically mutually sorted by Muldis Data Language into a
// canonical total order.  But *T* otherwise does not necessarily have
// conceptually a total order in the normal sense or that order is different
// than what the provided comparison operators give you.  An `Orderable` type
// is a type for which one can take all of its values and place them on a line
// such that each value is definitively considered *before* all of the values
// one one side and *after* all of the values on the other side.
//
// The default value of `Orderable` is the `Integer` value `0`.  The
// minimum and maximum values of `Orderable` are `\!Before_All_Others`
// and `\!After_All_Others`, respectively; these 2 `Excuse` values are
// canonically considered to be before and after, respectively, *every* other
// value of the Muldis Data Language type system, regardless of whether those values are
// members a type for which an `Orderable`-composing type definer exists.  The
// two values `\!Before_All_Others` and `\!After_All_Others` can be useful in
// defining an *interval* that is partially or completely unbounded, and to use
// as *two-sided identity element* values for chained order-comparisons.
// To be clear, Muldis Data Language does not actually system-define a
// default total order for the whole type system, nor does it define
// any orderings between discrete regular types, including with the
// conceptually minimum and maximum values, so the set of all values
// comprising `Orderable` itself only has a partial order, but users can
// define a cross-composer total order for their own use cases as desired.
//
// `Orderable` is composed, directly or indirectly, by:
// `Before_All_Others`, `After_All_Others`, `Bicessable`,
// `Boolean`, `Integral`, `Integer`, `Fractional`, `Rational`,
// `Stringy`, `Bits`, `Blob`, `Textual`, `Text`, `Positional`, `Array`,
// `Orderelation`.

namespace Muldis.Data_Library;

public interface MDV_Orderable<Specific_T> : MDV_Any
{
    // in_order

    public abstract MDV_Boolean in_order(MDV_Orderable<Specific_T> topic_1);

    // before <

    public MDV_Boolean before(MDV_Orderable<Specific_T> topic_1)
    {
        MDV_Orderable<Specific_T> topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_1.in_order(topic_0).not();
    }

    // after >

    public MDV_Boolean after(MDV_Orderable<Specific_T> topic_1)
    {
        MDV_Orderable<Specific_T> topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.in_order(topic_1).not();
    }

    // before_or_same <= ≤

    public MDV_Boolean before_or_same(MDV_Orderable<Specific_T> topic_1)
    {
        MDV_Orderable<Specific_T> topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.in_order(topic_1);
    }

    // after_or_same >= ≥

    public MDV_Boolean after_or_same(MDV_Orderable<Specific_T> topic_1)
    {
        MDV_Orderable<Specific_T> topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_1.in_order(topic_0);
    }

    // min

    public MDV_Orderable<Specific_T> min(MDV_Orderable<Specific_T> topic_1)
    {
        MDV_Orderable<Specific_T> topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.in_order(topic_1).as_Boolean() ? topic_0 : topic_1;
    }

    // max

    public MDV_Orderable<Specific_T> max(MDV_Orderable<Specific_T> topic_1)
    {
        MDV_Orderable<Specific_T> topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return topic_0.in_order(topic_1).as_Boolean() ? topic_1 : topic_0;
    }
}
