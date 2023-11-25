namespace Muldis.Data_Engine_Reference.Internal;

// Muldis.Data_Engine_Reference.Internal.MDL_Any
// Represents a Muldis Data Language "value", which is an individual constant that
// is not fixed in time or space.  Every Muldis Data Language value is unique,
// eternal, and immutable; it has no address and can not be updated.
// Several distinct MDL_Any objects may denote the same Muldis Data Language
// "value"; however, any time that two MDL_Any are discovered to
// denote the same Muldis Data Language value, any references to one may be safely
// replaced by references to the other.
// A design objective is implementing the "flyweight pattern",
// where MDL_Any objects share components directly or indirectly as much
// as is safely possible.  For example, if we detect duplication at
// runtime, we may replace references to one
// MDL_Any with another where the underlying virtual machine or
// garbage collector doesn't natively provide that ability.
// Similarly, proving equality of two "value" can often short-circuit.
// While MDL_Any are immutable from a user's perspective, their
// components may in fact mutate for memory sharing or consolidating.
// Iff a Muldis Data Language "value" is a "Handle" then it references something
// that possibly can mutate, such as a Muldis Data Language "variable".

internal class MDL_Any
{
    // Memory pool this Muldis Data Language "value" lives in.
    internal Memory memory;

    // Muldis Data Language most specific well known base data type (WKBT) this
    // "value" is a member of.  Helps interpret which subtype of MDL_Any
    // we have without having to use the language's type reflection system.
    // Some of these types have their own subset of specialized
    // representation formats for the sake of optimization.
    internal Well_Known_Base_Type WKBT;

    // Normalized serialization of the Muldis Data Language "value" that its host
    // MDL_Any represents.  This is calculated lazily if needed,
    // typically when the "value" is a member of an indexed collection.
    // The serialization format either is or resembles a Muldis Object Notation Plain Text
    // literal for selecting the value, in the form of character strings
    // whose character code points are typically in the 0..127 range.
    internal String cached_MDL_Any_identity;

    public override String ToString()
    {
        return this.memory.preview_generator.MDL_Any_as_preview_String(this);
    }

    internal String MDL_Any_identity()
    {
        // This called function will test if cached_MDL_Any_identity
        // is null and assign it a value if so and use its value if not.
        return this.memory.identity_generator.MDL_Any_as_identity_String(this);
    }
}

internal class MDL_Any_Comparer : EqualityComparer<MDL_Any>
{
    public override Boolean Equals(MDL_Any v1, MDL_Any v2)
    {
        if (v1 is null && v2 is null)
        {
            // Would we ever get here?
            return true;
        }
        if (v1 is null || v2 is null)
        {
            return false;
        }
        return v1.memory.executor.Any__same(v1, v2);
    }

    public override Int32 GetHashCode(MDL_Any v)
    {
        if (v is null)
        {
            // Would we ever get here?
            return 0;
        }
        return v.MDL_Any_identity().GetHashCode();
    }
}
