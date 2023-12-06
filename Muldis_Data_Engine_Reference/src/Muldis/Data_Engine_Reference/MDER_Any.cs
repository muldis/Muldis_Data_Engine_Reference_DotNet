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

    // Muldis Data Language most specific well known base data type (WKBT) this
    // "value" is a member of.  Helps interpret which subtype of MDER_Any
    // we have without having to use the language's type reflection system.
    // Some of these types have their own subset of specialized
    // representation formats for the sake of optimization.
    internal readonly Internal_Well_Known_Base_Type WKBT;

    // Normalized serialization of the Muldis Data Language "value" that its host
    // MDER_Any represents.  This is calculated lazily if needed,
    // typically when the "value" is a member of an indexed collection.
    // The serialization format either is or resembles a Muldis Object Notation Plain Text
    // literal for selecting the value, in the form of character strings
    // whose character code points are typically in the 0..127 range.
    internal String? cached_MDER_Any_identity;

    internal MDER_Any(MDER_Machine machine, Internal_Well_Known_Base_Type WKBT)
    {
        this.__machine = machine;
        this.WKBT = WKBT;
    }

    public override String ToString()
    {
        return this.__machine._preview_generator().MDER_Any_as_preview_String(this);
    }

    public MDER_Machine machine()
    {
        return this.__machine;
    }

    internal String MDER_Any_identity()
    {
        // This called function will test if cached_MDER_Any_identity
        // is null and assign it a value if so and use its value if not.
        return this.__machine._identity_generator().MDER_Any_as_identity_String(this);
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
        return v.MDER_Any_identity().GetHashCode();
    }
}
