namespace Muldis.Data_Engine_Reference;

// Muldis.Data_Engine_Reference.MDER_Any
// Represents a Muldis Data Language "value", which is an individual constant that
// is not fixed in time or space.  Every Muldis Data Language value is unique,
// eternal, and immutable; it has no address and can not be updated.
// Several distinct MDER_Any objects may denote the same Muldis Data Language
// "value"; however, any time that two MDER_Any are discovered to
// denote the same Muldis Data Language value, any references to one may be safely
// replaced by references to the other.
// A design objective is implementing the "flyweight pattern",
// where MDER_Any objects share components directly or indirectly as much
// as is safely possible.  For example, if we detect duplication at
// runtime, we may replace references to one
// MDER_Any with another where the underlying virtual machine or
// garbage collector doesn't natively provide that ability.
// Similarly, proving equality of two "value" can often short-circuit.
// While MDER_Any are immutable from a user's perspective, their
// components may in fact mutate for memory sharing or consolidating.
// Iff a Muldis Data Language "value" is a "Handle" then it references something
// that possibly can mutate, such as a Muldis Data Language "variable".

public abstract class MDER_Any
{
    // MDER_Machine this Muldis Data Language "value" lives in.
    private readonly MDER_Machine __machine;

    // Muldis Data Language most specific well known base data type (__WKBT) this
    // "value" is a member of.  Helps interpret which subtype of MDER_Any
    // we have without having to use the language's type reflection system.
    // Some of these types have their own subset of specialized
    // representation formats for the sake of optimization.
    private readonly Internal_Well_Known_Base_Type __WKBT;

    // Normalized serialization of the Muldis Data Language "value" that its host
    // MDER_Any represents.  This is calculated lazily if needed,
    // typically when the "value" is a member of an indexed collection.
    // The serialization format either is or resembles a Muldis Object Notation Plain Text
    // literal for selecting the value, in the form of character strings
    // whose character code points are typically in the 0..127 range.
    private String? __cached_MDER_Any_identity;

    internal MDER_Any(MDER_Machine machine, Internal_Well_Known_Base_Type WKBT)
    {
        this.__machine = machine;
        this.__WKBT = WKBT;
    }

    public override String ToString()
    {
        return this._preview_as_String();
    }

    public MDER_Machine machine()
    {
        return this.__machine;
    }

    internal Internal_Well_Known_Base_Type _WKBT()
    {
        return this.__WKBT;
    }

    internal Boolean _has_cached_MDER_Any_identity()
    {
        return this.__cached_MDER_Any_identity is not null;
    }

    // Provides utility pure functions that accept any Muldis Data Language "value"
    // and derive a .NET String that uniquely identifies it.
    // This class is deterministic and guarantees that iff 2 MDER_Any are
    // logically considered to be the "same" Muldis Data Language value then they will
    // map to exactly the same .NET String value, and moreover, that iff 2
    // MDER_Any are logically considered to NOT be the "same" Muldis Data Language value,
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
    // A normal side effect of using Internal_Identity_Generator on a MDER_Any/etc
    // value is to update a cache therein to hold the serialization result.

    internal String _identity_as_String()
    {
        if (this.__cached_MDER_Any_identity is null)
        {
            this.__cached_MDER_Any_identity
                = this._as_MUON_Plain_Text_artifact(
                    Internal_MUON_Generator_Type.Identity, "");
        }
        return this.__cached_MDER_Any_identity;
    }

    // Provides utility pure functions that accept any Muldis Data Language "value"
    // and derive a .NET String that provides a "preview quick look"
    // serialization of that value.  The intended use of this class is to
    // underlie a .NET ToString() override for all .NET values representing
    // Muldis Data Language values so that debuggers including MS Visual Studio can
    // assist programmers at easily determining what the current logical
    // values of their program variables are, which would otherwise be
    // quite labour intensive for humans inspeding MDER_Any object fields.
    // This class is explicitly NOT guaranteed to be deterministic and so
    // different runs of its functions on what are logically the same
    // Muldis Data Language values might produce different String outputs; reasons for
    // that include not sorting collection elements and truncating results.
    // Given its purpose in aiding debugging, it is intended that this
    // class will NOT populate any calculation caching fields in MDER_Any/etc
    // objects, to help avoid heisenbugs.

    internal String _preview_as_String()
    {
        return this._as_MUON_Plain_Text_artifact(
            Internal_MUON_Generator_Type.Preview, "");
    }

    // TODO: Some of this text has obsolete notions and it should be rewritten.
    // Provides utility pure functions that accept any Muldis Data Language "Package"
    // value, which is a native Muldis Data Language "standard compilation unit", and
    // derive a "Text" value that is this "Package" encoded in compliance
    // with the "Muldis_Object_Notation_Plain_Text 'https://muldis.com' '0.300.0'"
    // formal specification.  This outputs of this generator are intended
    // for external use, whether for storage in foo.mdpt disk files or
    // other places, viewing by users, and reading by other programs, as a
    // means of interchange with any other system conforming to the spec.
    // This generator actually handles any Muldis Data Language "value", not just a "Package".
    // This class is completely deterministic and its exact output Muldis Data Language
    // Text/etc values are determined entirely by its input Package/etc values.
    // This generator is expressly kept as simple as possible such that it
    // has zero configuration options and only does simple pretty-printing;
    // it is only meant so that the Muldis_D_Foundation has a competent
    // serialization capability built-in whose result is easy enough to
    // read and that is easy to diff changes for.
    // This generator makes only the "foundational" flavor of
    // Muldis_Object_Notation_Plain_Text and provides a literal serialization, including
    // that all annotation and decoration values are output verbatim, with
    // no interpretation and with nothing left out.  Round-tripping will
    // only work as intended with a parser that doesn't produce decorations.
    // Typically it is up to bootstrapped higher-level libraries written in
    // Muldis Data Language to provide other generating options that are better
    // pretty-printed or provide any configuration or that serialize with
    // non-foundational Muldis_Object_Notation_Plain_Text or interpret decorations.
    // While this generator's output includes a "parsing unit predicate" on
    // request (its optionality is the sole configuration option), that
    // does not include any preceeding "shebang line", which is up to the
    // caller where such is desired.

    public MDER_Text as_MUON_Plain_Text_artifact_as_MDER_Text()
    {
        String artifact_as_String = this._as_MUON_Plain_Text_artifact(
            Internal_MUON_Generator_Type.Standard, "");
        Internal_Dot_Net_String_Unicode_Test_Result test_result
            = this.__machine._executor().Test_Dot_Net_String(artifact_as_String);
        return this.__machine.MDER_Text(
            "(Muldis_Object_Notation_Syntax:([Plain_Text,"
            + " \"https://muldis.com\", \"0.300.0\"]:\u000A"
            + artifact_as_String + "))\u000A",
            test_result == Internal_Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP
        );
    }

    public MDER_Blob as_MUON_Plain_Text_artifact_as_MDER_Blob()
    {
        throw new NotImplementedException();
    }

    // internal abstract String _as_MUON_Plain_Text_artifact(
    //     Internal_MUON_Generator_Type MGT, String indent);

    internal String _as_MUON_Plain_Text_artifact(
        Internal_MUON_Generator_Type MGT, String indent)
    {
        return this.__machine._MUON_generator().as_Any_artifact(this, indent);
    }
}

public class MDER_Any_Comparer : EqualityComparer<MDER_Any>
{
    public override Boolean Equals(MDER_Any? v1, MDER_Any? v2)
    {
        if (Object.ReferenceEquals(v1,v2))
        {
            // We get here if both args are null, if that is possible.
            return true;
        }
        if (v1 is null || v2 is null)
        {
            return false;
        }
        return v1.machine()._executor().Any__same(v1, v2);
    }

    public override Int32 GetHashCode(MDER_Any v)
    {
        if (v is null)
        {
            // Would we ever get here?
            return 0;
        }
        return v._identity_as_String().GetHashCode();
    }
}
