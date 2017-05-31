using System;
using Muldis.D.Ref_Eng.Core;

namespace Muldis.D.Ref_Eng.Core.Plain_Text
{
    internal abstract class Parser
    {
    }

    // Muldis.D.Ref_Eng.Core.Plain_Text.Standard_Parser
    // Provides utility pure functions that accept Muldis D Plain Text (MDPT)
    // source code, either a complete "parsing unit" that might comprise a
    // foo.mdpt disk file or appropriate portions of such, usually input as
    // a "Text" value, and derives by parsing it the actual Muldis D "value"
    // that the source code denotes; typically the result is a value of the
    // "Package" type, which is the "native" form of Muldis D source code
    // and a "standard compilation unit".
    // The input "Text" source code is expected to conform to the formal
    // specification "Muldis_D_Plain_Text 'http://muldis.com' '0.201.0.-9'"
    // and would typically either be hand-written by users or be generated
    // by code such as the X or Y classes below.
    // This class is completely deterministic and its exact output Muldis D
    // Package/etc values are determined entirely by its input Text/etc values.

    internal class Standard_Parser : Parser
    {
        internal MD_Any MDPT_Parsing_Unit_MD_Text_to_MD_Any(
            MD_Any parsing_unit)
        {
            // TODO: Everything.
            return parsing_unit.AS.Memory.Well_Known_Excuses["No_Reason"];
        }
    }

    internal abstract class Generator
    {
    }

    // Muldis.D.Ref_Eng.Core.Plain_Text.Standard_Generator
    // Provides utility pure functions that accept any Muldis D "Package"
    // value, which is a native Muldis D "standard compilation unit", and
    // derive a "Text" value that is this "Package" encoded in compliance
    // with the "Muldis_D_Plain_Text 'http://muldis.com' '0.201.0.-9'"
    // formal specification.  This outputs of this generator are intended
    // for external use, whether for storage in foo.mdpt disk files or
    // other places, viewing by users, and reading by other programs, as a
    // means of interchange with any other system conforming to the spec.
    // This generator will output all code annotations attached to the
    // regular semantic code and will also respect any relevant attached
    // code decorations where possible, or it otherwise has some
    // configuration options for pretty-printing the output code strings.
    // This class is completely deterministic and its exact output Muldis D
    // Text/etc values are determined entirely by its input Package/etc values.

    internal class Standard_Generator : Generator
    {
        internal MD_Any MD_Package_to_MDPT_MD_Text(MD_Any package)
        {
            // TODO: Everything.
            return package.AS.Memory.Well_Known_Excuses["No_Reason"];
        }
    }

    // Muldis.D.Ref_Eng.Core.Plain_Text.Identity_Generator
    // Provides utility pure functions that accept any Muldis D "value"
    // and derive a .Net String that uniquely identifies it.
    // This class is deterministic and guarantees that iff 2 MD_Any are
    // logically considered to be the "same" Muldis D value then they will
    // map to exactly the same .Net String value, and moreover, that iff 2
    // MD_Any are logically considered to NOT be the "same" Muldis D value,
    // they are guaranteed to map to distinct .Net String values.
    // The outputs of this generator are intended for internal use only,
    // where the outputs are transient and only intended to be used within
    // the same in-memory Muldis.D.Ref_Eng VM instance that generated them.
    // The intended use of this class is to produce normalized identity
    // values for .Net collection indexes, Dictionary keys for example, or
    // otherwise support the means of primary/last resort for set-like
    // operations like duplicate elimination, relational joins, and more.
    // The outputs of this class may possibly conform to the
    // Muldis_D_Plain_Text specification and be parseable to yield the
    // original input values, but that is not guaranteed; even if that is
    // the case, the outputs would be considerably less "pretty" as a
    // trade-off to make the generating faster and less error-prone.
    // A normal side effect of using Identity_Generator on a MD_Any/etc
    // value is to update a cache therein to hold the serialization result.

    internal class Identity_Generator : Generator
    {
        internal String MD_Any_to_Identity_String(MD_Any value)
        {
            return Any_Selector(value);
        }

        private String Any_Selector(MD_Any value)
        {
            if (value.AS.Cached_MD_Any_Identity == null)
            {
                // TODO: Everything.
                value.AS.Cached_MD_Any_Identity = "42";
            }
            return value.AS.Cached_MD_Any_Identity;
        }
    }

    // Muldis.D.Ref_Eng.Core.Plain_Text.Preview_Generator
    // Provides utility pure functions that accept any Muldis D "value"
    // and derive a .Net String that provides a "preview quick look"
    // serialization of that value.  The intended use of this class is to
    // underlie a .Net ToString() override for all .Net values representing
    // Muldis D values so that debuggers including MS Visual Studio can
    // assist programmers at easily determining what the current logical
    // values of their program variables are, which would otherwise be
    // quite labour intensive for humans inspeding MD_Any object fields.
    // This class is explicitly NOT guaranteed to be deterministic and so
    // different runs of its functions on what are logically the same
    // Muldis D values might produce different String outputs; reasons for
    // that include not sorting collection elements and truncating results.
    // Given its purpose in aiding debugging, it is intended that this
    // class will NOT populate any calculation caching fields in MD_Any/etc
    // objects, to help avoid heisenbugs.

    internal class Preview_Generator : Generator
    {
        internal String MD_Any_To_Preview_String(MD_Any value)
        {
            return Any_Selector(value);
        }

        private String Any_Selector(MD_Any value)
        {
            // TODO: Everything.
            return "a not yet specified IMD_Any value";
        }
    }
}
