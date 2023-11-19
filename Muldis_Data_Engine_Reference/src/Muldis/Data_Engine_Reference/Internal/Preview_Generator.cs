using System;

namespace Muldis.Data_Engine_Reference.Internal;

// Muldis.Data_Engine_Reference.Internal.Preview_Generator
// Provides utility pure functions that accept any Muldis Data Language "value"
// and derive a .NET String that provides a "preview quick look"
// serialization of that value.  The intended use of this class is to
// underlie a .NET ToString() override for all .NET values representing
// Muldis Data Language values so that debuggers including MS Visual Studio can
// assist programmers at easily determining what the current logical
// values of their program variables are, which would otherwise be
// quite labour intensive for humans inspeding MDL_Any object fields.
// This class is explicitly NOT guaranteed to be deterministic and so
// different runs of its functions on what are logically the same
// Muldis Data Language values might produce different String outputs; reasons for
// that include not sorting collection elements and truncating results.
// Given its purpose in aiding debugging, it is intended that this
// class will NOT populate any calculation caching fields in MDL_Any/etc
// objects, to help avoid heisenbugs.

internal class Preview_Generator : MUON_Generator
{
    internal String MDL_Any_To_Preview_String(MDL_Any value)
    {
        return Any_Selector(value, "");
    }

    protected override String Any_Selector(MDL_Any value, String indent)
    {
        return Any_Selector_Foundation_Dispatch(value, indent);
    }
}
