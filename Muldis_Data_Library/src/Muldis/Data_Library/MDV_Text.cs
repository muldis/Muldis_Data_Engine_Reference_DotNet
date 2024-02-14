// Selection type definer `Text`.
// Represents foundation type `fdn::Text`.
// A `Text` value is characterized by an arbitrarily-large ordered sequence of
// Unicode standard *character code points*, where each distinct code point
// corresponds to a distinct integer in the set `[0..0xD7FF,0xE000..0x10FFFF]`,
// which explicitly does not represent any kind of thing in particular.
// Each character is taken from a finite repertoire having 0x10F7FF members,
// but `Text` imposes no limit on the length of each character sequence.
// `Text` is an infinite type.
// `Text` has a default value of `""` (the empty character string).
// `Text` is `Orderable`; its minimum value is `""`; it has no maximum value;
// its ordering algorithm corresponds directly to that of `Array`,
// pairwise as integer sequences.

namespace Muldis.Data_Library;

public readonly struct MDV_Text : MDV_Positional<MDV_Text>
{
    private static readonly MDV_Text __empty = new MDV_Text(String.Empty, false, 0);

    // A value of the .NET class String is immutable.
    // It should be safe to pass around without cloning.
    // Each logical member represents a single Unicode standard character
    // code point from either the Basic Multilingual Plane (BMP), which
    // is those member values in the range [0..0xD7FF,0xE000..0xFFFF],
    // or from either of the 16 supplementary planes, which is those
    // member values in the range [0x10000..0x10FFFF].
    // A .NET String is instead natively composed of a sequence of
    // .NET Char that are each in the range [0..0xFFFF],
    // such that each BMP member is represented by 1 element and each
    // non-BMP member is represented by 2 consecutive elements that are a
    // "surrogate pair" in the range [0xD800..0xDFFF].
    // Therefore, the native "length" of a .NET String only matches the
    // logical "length" of the Text when all code points are in the BMP.
    // While it is possible for a .NET String to contain an isolated
    // "surrogate" Char outside of a proper "surrogate pair", we forbid
    // such a malformed .NET String from being used.
    private readonly String __code_point_members_as_String;

    // This is true iff we know that at least 1 code point member is NOT
    // in the Basic Multilingual Plane (BMP); this is false iff we know
    // that there is no such code point member.  That is, with respect
    // to a .NET String, this is true iff we know the .NET String has at
    // least 1 "surrogate pair".  While this value can be derived
    // from __code_point_members_as_String, we cache it for performance.
    private readonly Boolean __has_any_non_BMP;

    // Count of the code point members.  While this value can be derived
    // from __code_point_members_as_String, we cache it for performance.
    private readonly Int32 __count_of_code_point_members;

    private MDV_Text(String code_point_members_as_String,
        Boolean has_any_non_BMP, Int32 count_of_code_point_members)
    {
        this.__code_point_members_as_String = code_point_members_as_String;
        this.__has_any_non_BMP = has_any_non_BMP;
        this.__count_of_code_point_members = count_of_code_point_members;
    }

    public override Int32 GetHashCode()
    {
        return this.__code_point_members_as_String.GetHashCode();
    }

    public override String ToString()
    {
        return Internal_Identity.Text(this);
    }

    public override Boolean Equals(Object? obj)
    {
        return (obj is MDV_Text specific_obj)
            && this._same(specific_obj);
    }

    public static MDV_Text from(String code_point_members_as_String)
    {
        if (code_point_members_as_String.Equals(String.Empty))
        {
            return MDV_Text.__empty;
        }
        Boolean has_any_non_BMP = false;
        Int32 count_of_code_point_members = 0;
        for (Int32 i = 0; i < code_point_members_as_String.Length; i++)
        {
            count_of_code_point_members++;
            if (Char.IsSurrogate(code_point_members_as_String[i]))
            {
                if ((i+1) < code_point_members_as_String.Length
                    && Char.IsSurrogatePair(code_point_members_as_String[i],
                    code_point_members_as_String[i+1]))
                {
                    has_any_non_BMP = true;
                    i++;
                }
                else
                {
                    // The .NET String is malformed;
                    // it has a non-paired Unicode surrogate code point.
                    throw new ArgumentException();
                }
            }
        }
        return new MDV_Text(code_point_members_as_String,
            has_any_non_BMP, count_of_code_point_members);
    }

    public static MDV_Text empty_()
    {
        return MDV_Text.__empty;
    }

    public void Deconstruct(out String code_point_members_as_String)
    {
        code_point_members_as_String = this.__code_point_members_as_String;
    }

    public String code_point_members_as_String()
    {
        return this.__code_point_members_as_String;
    }

    public MDV_Boolean same(MDV_Any topic_1)
    {
        MDV_Text topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return (topic_1 is MDV_Text specific_topic_1)
            ? MDV_Boolean.from(topic_0._same(specific_topic_1))
            : MDV_Boolean.@false();
    }

    private Boolean _same(MDV_Text topic_1)
    {
        MDV_Text topic_0 = this;
        return topic_0.__code_point_members_as_String.Equals(
            topic_1.__code_point_members_as_String);
    }

    public MDV_Boolean in_order(MDV_Orderable<MDV_Text> topic_1)
    {
        MDV_Text topic_0 = this;
        if (topic_1 is null)
        {
            throw new ArgumentNullException();
        }
        return MDV_Boolean.from(topic_0._in_order((MDV_Text) topic_1));
    }

    private Boolean _in_order(MDV_Text topic_1)
    {
        MDV_Text topic_0 = this;
        return String.Compare(topic_0.__code_point_members_as_String,
            topic_1.__code_point_members_as_String,
            StringComparison.Ordinal) <= 0;
    }

    public MDV_Boolean so_empty()
    {
        MDV_Text topic = this;
        return MDV_Boolean.from(topic.__code_point_members_as_String.Equals(String.Empty));
    }

    public MDV_Emptyable<MDV_Text> empty()
    {
        return MDV_Text.__empty;
    }
}
