// The interface type definer `Successable` is semifinite.  A `Successable` value
// is a member of a conceptually noncontiguous totally ordered type; it has a
// definitive *successor* value of that type, at least where the given value
// isn't the last value.
//
// The primary reason for `Successable` is to provide an easy consistent and
// terse API for a generator of arbitrary sequences of values of any type.  In
// this context, a `Successable` value defines a complete self-contained
// *state* for a sequence generator, which is everything the generator needs
// to know to both emit a *current* value, which we call the *asset*, as
// well as determine all subsequent values of the sequence without any further
// input.  To keep the fundamental general case API simple, there is just the
// a monadic function to derive the next state from the current one, and a
// monadic function to extract the asset from the current state, so actually
// reading a sequence of values requires 2 function calls per value in the
// general case.  For some trivial cases of `Successable`, the *state* and
// *asset* are one and the same, so just 1 function call per value is needed.
// Keep in mind that asset values may repeat in a sequence, so it is not them
// but rather the state values that have the total order property.
//
// `Successable` is a less rigorous analogy to `Bicessable`, where the
// latter also requires the ability to produce the *predecessor* value of the
// given value, as well as the ability to determine if 2 arbitrary values are
// in order.  While conceptually a `Successable` has those features, formally
// it is not required to because for some types it may be onerous or
// unnecessary for its mandate to support those features; for example,
// producing a successor state may disgard information otherwise needed to
// recall any of its predecessors.
//
// The default and minimum and maximum values of `Successable` are the same
// as those of `Orderable`.  `Successable` is composed, directly or
// indirectly, by: `Bicessable`, `Boolean`, `Integral`, `Integer`.
//
// `Successable` is intended to be a generalized tool for performing *list
// comprehension* or *set comprehension*.  The typically idiomatic and more
// efficient way to do many kinds of such *comprehensions* is to use the
// features of various `Homogeneous` types to map an existing list or set to
// another using generic member mapping and filtering functions, such as a
// list of even integers less than a hundred.  With those cases, the
// map/filter approach can permit processing members in any order or in
// parallel, and avoiding unnecessary intermediate values.  In contrast, the
// primary intended uses of `Successable` is when either you want to produce
// or process a potentially infinite-sized list (lazily) or especially produce
// a sequence with uneven step sizes, such as an arbitrary number of
// Fibonacci.  This is for cases where it may be necessary to calculate all
// the intermediate values in order to arrive at a desired nth one, and doing
// them out of sequence or in parallel may not be an option.

namespace Muldis.Data_Library;

public interface MDV_Successable<Specific_T> : MDV_Any
{
    // asset

    public static abstract MDV_Any asset(Specific_T topic);

    // succ

    public static abstract Specific_T succ(Specific_T topic);
}
