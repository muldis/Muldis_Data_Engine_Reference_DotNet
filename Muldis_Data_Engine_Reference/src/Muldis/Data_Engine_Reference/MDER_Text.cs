namespace Muldis.Data_Engine_Reference;

public class MDER_Text : MDER_Any
{
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
    // code_point_members is represented using a standard .NET
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
    internal readonly String code_point_members;

    // Nullable Boolean
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
    internal readonly Boolean has_any_non_BMP;

    // Cached count of code point members of the Muldis Data Language Text.
    internal Nullable<Int64> cached_member_count;

    internal MDER_Text(MDER_Machine machine, String code_point_members,
        Boolean has_any_non_BMP)
        : base(machine, Internal_Well_Known_Base_Type.MDER_Text)
    {
        this.code_point_members = code_point_members;
        this.has_any_non_BMP = has_any_non_BMP;
    }
}
