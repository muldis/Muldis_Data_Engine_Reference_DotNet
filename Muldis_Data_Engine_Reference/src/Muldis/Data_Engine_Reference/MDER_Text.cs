namespace Muldis.Data_Engine_Reference;

public sealed class MDER_Text : MDER_Any
{
    // The MDER_Machine VM that this MDER_Any "value" lives in.
    private readonly MDER_Machine __machine;

    // Surrogate identity for this MDER_Any with a simpler representation.
    private String? __cached_identity_as_String;

    // A value of the .NET class String is immutable,
    // It should be safe to pass around without cloning.
    // Represents a Muldis Data Language Text value where each member value is
    // a Muldis Data Language Integer in the range {0..0xD7FF,0xE000..0x10FFFF}.
    // A .NET String is the simplest storage representation for that
    // type which doesn't internally use trees for sharing or multipliers.
    // This is the canonical storage type for a regular character string.
    // Each logical member represents a single Unicode standard character
    // code point from either the Basic Multilingual Plane (BMP), which
    // is those member values in the range {0..0xD7FF,0xE000..0xFFFF},
    // or from either of the 16 supplementary planes, which is those
    // member values in the range {0x10000..0x10FFFF}.
    // code_point_members_as_String is represented using a standard .NET
    // String value for simplicity but a String has a different native
    // concept of components; it is formally an array of .NET Char
    // each of which is either a whole BMP code point or half of a
    // non-BMP code point; a non-BMP code point is represented by a pair
    // of consecutive Char with numeric values in {0xD800..0xDFFF};
    // therefore, the native "length" of a String only matches the
    // "length" of the Muldis Data Language Text when all code points are in the BMP.
    // While it is possible for a .NET String to contain an isolated
    // "surrogate" Char outside of a proper "surrogate pair", both
    // Muldis.Data_Engine_Reference forbids such a malformed String
    // from either being used internally or being passed in by the API.
    private readonly String __code_point_members_as_String;

    // This is true iff we know that at least 1 code point member is NOT
    // in the Basic Multilingual Plane (BMP); this is false iff we know
    // that there is no such code point member.  That is, with respect
    // to a .NET String, this is true iff we know the String has at least
    // 1 "surrogate pair".  We cache this knowledge because a .NET String
    // with any non-BMP Char is more complicated to count the members
    // of or access members by ordinal position or do some other stuff.
    // This field is always defined as a side-effect of Muldis.Data_Engine_Reference
    // forbidding a malformed String and so they are always tested at
    // the borders, that test also revealing if a String has non-BMP chars.
    private readonly Boolean __has_any_non_BMP;

    // Cached count of code point members of the Muldis Data Language Text.
    private Int32? __cached_code_point_count;

    internal MDER_Text(MDER_Machine machine,
        String code_point_members_as_String, Boolean has_any_non_BMP)
    {
        this.__machine = machine;
        this.__code_point_members_as_String = code_point_members_as_String;
        this.__has_any_non_BMP = has_any_non_BMP;
    }

    public override MDER_Machine machine()
    {
        return this.__machine;
    }

    internal override String _identity_as_String()
    {
        if (this.__cached_identity_as_String is null)
        {
            this.__cached_identity_as_String
                = this._as_MUON_Plain_Text_artifact("");
        }
        return this.__cached_identity_as_String;
    }

    public String code_point_members_as_String()
    {
        return this.__code_point_members_as_String;
    }

    internal Boolean _has_any_non_BMP()
    {
        return this.__has_any_non_BMP;
    }

    internal override String _as_MUON_Plain_Text_artifact(String indent)
    {
        return this._from_String_as_MUON_Text_artifact(
            this.__code_point_members_as_String);
    }

    internal Boolean _MDER_Text__same(MDER_Text topic_1)
    {
        MDER_Text topic_0 = this;
        return String.Equals(topic_0.__code_point_members_as_String,
            topic_1.__code_point_members_as_String);
    }

    internal Int32 _MDER_Text__code_point_count()
    {
        if (this.__cached_code_point_count is null)
        {
            this.__cached_code_point_count
                = Internal_Unicode.code_point_count(
                    this.__code_point_members_as_String,
                    this.__has_any_non_BMP
                );
        }
        return (Int32)this.__cached_code_point_count;
    }

    internal MDER_Integer? _MDER_Text__code_point_maybe_at(Int32 ord_pos)
    {
        if (ord_pos >= this.__cached_code_point_count)
        {
            return null;
        }
        Int32? maybe_code_point = Internal_Unicode.code_point_maybe_at(
            this.__code_point_members_as_String,
            this.__has_any_non_BMP, ord_pos);
        return (maybe_code_point is null) ? null
            : this.machine().MDER_Integer((Int32)maybe_code_point);
    }
}
