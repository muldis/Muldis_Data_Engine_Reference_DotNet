using System;

namespace Muldis.Data_Engine_Reference.Internal;

// Muldis.Data_Engine_Reference.Internal.Identity_Generator
// Provides utility pure functions that accept any Muldis Data Language "value"
// and derive a .NET String that uniquely identifies it.
// This class is deterministic and guarantees that iff 2 MDL_Any are
// logically considered to be the "same" Muldis Data Language value then they will
// map to exactly the same .NET String value, and moreover, that iff 2
// MDL_Any are logically considered to NOT be the "same" Muldis Data Language value,
// they are guaranteed to map to distinct .NET String values.
// The outputs of this generator are intended for internal use only,
// where the outputs are transient and only intended to be used within
// the same in-memory Muldis.Data_Engine_Reference VM instance that generated them.
// The intended use of this class is to produce normalized identity
// values for .NET collection indexes, Dictionary keys for example, or
// otherwise support the means of primary/last resort for set-like
// operations like duplicate elimination, relational joins, and more.
// The outputs of this class may possibly conform to the
// Muldis_Object_Notation_Plain_Text specification and be parseable to yield the
// original input values, but that is not guaranteed; even if that is
// the case, the outputs might be considerably less "pretty" as a
// trade-off to make the generating faster and less error-prone.
// A normal side effect of using Identity_Generator on a MDL_Any/etc
// value is to update a cache therein to hold the serialization result.

internal class Identity_Generator : MUON_Generator
{
    internal String MDL_Any_to_Identity_String(MDL_Any value)
    {
        return Any_Selector(value, "");
    }

    protected override String Any_Selector(MDL_Any value, String indent)
    {
        if (value.cached_MDL_Any_identity is null)
        {
            value.cached_MDL_Any_identity
                = Any_Selector_Foundation_Dispatch(value, indent);
        }
        return value.cached_MDL_Any_identity;
    }
}
