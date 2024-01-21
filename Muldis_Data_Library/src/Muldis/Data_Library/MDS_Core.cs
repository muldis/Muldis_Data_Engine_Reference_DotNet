// Muldis Data Language package `System` segment `Core`.

namespace Muldis.Data_Library;

public static class MDS_Core
{
    // Selection type definer `Any`.
    // `Any` represents the *universal type*, which is the maximal data type
    // of the entire Muldis Data Language type system and consists of all
    // values which can possibly exist.  It is the union of all other types.
    // It is a *supertype* of every other type.
    // `Any` is an infinite type.
    // `Any` has a default value of `0bFALSE`.

    public static MDV_Boolean Any(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.@true();
    }

    // Selection type definer `None`.
    // `None` represents the *empty type*, which is the minimal data type
    // of the entire Muldis Data Language type system and consists of
    // exactly zero values.  It is the intersection of all other types.
    // It is a *subtype* of every other type.
    // `None` can not have a default value.

    public static MDV_Boolean None(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.@false();
    }

    public static MDV_Boolean same(MDV_Any topic_0, MDV_Any topic_1)
    {
        return MDV_Any.same(topic_0, topic_1);
    }

    public static MDV_Boolean not_same(MDV_Any topic_0, MDV_Any topic_1)
    {
        return MDV_Any.not_same(topic_0, topic_1);
    }

    // The interface type definer `Excuse` is semifinite.  An `Excuse`
    // value is an explicitly stated reason for why, given some particular
    // problem domain, a value is not being used that is ordinary for that
    // domain.  For example, the typical integer division operation is not
    // defined to give an integer result when the divisor is zero, and so a
    // function for integer division could be defined to result in an
    // `Excuse` value rather than throw an exception in that case.
    // An `Excuse` is also characterized by an *exception* that is not
    // meant to terminate execution of code early.

    public static MDV_Boolean Excuse(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Excuse);
    }

    // coalesce ??

    public static MDV_Any coalesce(MDV_Any topic_0, MDV_Any topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return (topic_0 is MDV_Excuse) ? topic_1 : topic_0;
    }

    // anticoalesce !!

    public static MDV_Any anticoalesce(MDV_Any topic_0, MDV_Any topic_1)
    {
        if (topic_0 is null || topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return (topic_0 is MDV_Excuse) ? topic_0 : topic_1;
    }

    // Singleton type definer `Ignorance`.
    // Represents foundation type `fdn::Ignorance`.
    // The `Ignorance` value represents the `Excuse` value which
    // simply says that an ordinary value for any given domain is missing
    // and that there is simply no excuse that has been given for this; in
    // other words, something has gone wrong without the slightest hint of
    // an explanation.  This is conceptually the most generic `Excuse`
    // value there is, to help with expedient development, but any uses
    // should be considered technical debt, to be replaced later.
    // `Ignorance` is a finite type.
    // `Ignorance` has a default value of `0iIGNORANCE`.

    public static MDV_Boolean Ignorance(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Ignorance);
    }

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

    public static MDV_Boolean Orderable(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Orderable<MDV_Any>);
    }

    // Singleton type definer `Before_All_Others`.
    // Represents foundation type `fdn::Before_All_Others`.
    // The `Before_All_Others` value represents a type-agnostic analogy of
    // negative infinity, an `Orderable` value that sorts *before* all
    // other values in the Muldis Data Language type system, and that is
    // its only meaning.  This value is expressly *not* meant to represent
    // any specific mathematical or physical concept of *infinity* or `∞`
    // (of which there are many), including those of the IEEE
    // floating-point standards.
    // `Before_All_Others` is a finite type.
    // `Before_All_Others` has a default value of ...TODO...

    public static MDV_Boolean Before_All_Others(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Before_All_Others);
    }

    // Singleton type definer `After_All_Others`.
    // Represents foundation type `fdn::After_All_Others`.
    // The `After_All_Others` value represents a type-agnostic analogy of
    // positive infinity, an `Orderable` value that sorts *after* all
    // other values in the Muldis Data Language type system, and that is
    // its only meaning.  This value is expressly *not* meant to represent
    // any specific mathematical or physical concept of *infinity* or `∞`
    // (of which there are many), including those of the IEEE
    // floating-point standards.
    // `After_All_Others` is a finite type.
    // `After_All_Others` has a default value of ...TODO...

    public static MDV_Boolean After_All_Others(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_After_All_Others);
    }

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

    public static MDV_Boolean Successable(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Successable<MDV_Any>);
    }

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

    public static MDV_Boolean Bicessable(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Bicessable<MDV_Any>);
    }

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

    public static MDV_Boolean Boolable(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Boolable<MDV_Any>);
    }

    // Selection type definer `Boolean`.
    // Represents foundation type `fdn::Boolean`.
    // A `Boolean` value is a general purpose 2-valued logic boolean or *truth
    // value*, or specifically it is one of the 2 values `0bFALSE` and `0bTRUE`.
    // `Boolean` is a finite type.
    // `Boolean` has a default value of `0bFALSE`.
    // `Boolean` is both `Orderable` and `Bicessable`;
    // its minimum value is `0bFALSE` and its maximum value is `0bTRUE`.

    public static MDV_Boolean Boolean(MDV_Any topic)
    {
        if (topic is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic is MDV_Boolean);
    }
}
