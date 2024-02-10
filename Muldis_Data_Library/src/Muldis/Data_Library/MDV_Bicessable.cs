// The interface type definer `Bicessable` is semifinite.  A `Bicessable` value
// is an `Orderable` value for which, using the same canonical total order
// for its type, there exists definitive *predecessor* and *successor*
// values, at least where the given value isn't the first or last value on the
// line respectively.  Similarly, one can take any two values of a
// `Bicessable` type and produce an ordered list of all of that type's values
// which are on the line between those two values.  A primary quality of a
// type that is `Orderable` but not `Bicessable` is that you can take any
// two values of that type and then find a third value of that type which lies
// between the first two on the line; by definition for a `Bicessable` type,
// there is no third value between one of its values and that value's
// predecessor or successor value.  Note that
// while a generic rational numeric type may qualify as an ordinal type by
// some definitions of *ordinal*, since it is possible to count all the
// rationals if arranged a particular way, these types would not qualify as
// `Bicessable` here when that ordering is not the same as the one used for
// the same type's `Orderable` comparisons.  The default and minimum and
// maximum values of `Bicessable` are the same as those of `Orderable`.
// `Bicessable` is composed, directly or indirectly, by: `Boolean`,
// `Integral`, `Integer`.
//
// For some `Bicessable` types, there is the concept of a *quantum* or
// *step size*, where every consecutive pair of values on that type's value
// line are conceptually spaced apart at equal distances; this distance would
// be the quantum, and all steps along the value line are at exact multiples
// of that quantum.  However, `Bicessable` types in general don't need to be
// like this, and there can be different amounts of conceivable distance
// between consecutive values; a `Bicessable` type is just required to know
// where all the values are.  For example, `Integer` has a quantum while a
// type consisting just of prime integers does not.
//
// Note that while mathematics formally defines *predecessor* and
// *successor* for non-negative integers only, and some other programming
// languages extend this concept to real numbers with the meaning *minus one*
// and *plus one* respectively, Muldis Data Language only formally associates these terms
// with the quantum of *one* for types specifically representing integers;
// for `Bicessable` types in general, the terms just mean prior or next
// values and should not be conceptualized as mathematical operations.

namespace Muldis.Data_Library;

public interface MDV_Bicessable<Specific_T>
    : MDV_Orderable<Specific_T>, MDV_Successable<Specific_T>
{
    // pred

    public abstract MDV_Bicessable<Specific_T> pred();
}
