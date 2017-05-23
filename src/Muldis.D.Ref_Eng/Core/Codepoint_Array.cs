using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Muldis.D.Ref_Eng.Core
{
    // Muldis.D.Ref_Eng.Core.Codepoint_Array
    // Conceptually an Array of Unicode standard character codepoints,
    // integers in the set {0..0xD7FF,0xE000..0x10FFFF},
    // but additionally supports non-Unicode codepoints,
    // integers in the set {-0x80000000..-1,0xD800..0xDFFF,0x110000..0x7FFFFFFF}.
    // Used to represent a Muldis D Tuple attribute name, any unqualified identifier.
    // Used as canonical storage type for a Muldis D Text value / a regular character string.
    // Used as internal generic Muldis D value serialization format for indexing.
    // Conceptually an ordered sequence of Int32 and actually either
    // that or one of several alternate storage foramts.
    // WARNING: This class isn't especially reliable now for representing
    // the general case of ordered sequences of Int32 as some operations
    // will unconditionally cast to a String due to corner cutting
    // assumptions we would normally just handle Unicode; that will change.

    internal class Codepoint_Array
    {
        // Array of System.Int32
        // This option supports any codepoints in the signed 32-bit range -0x80000000..0x7FFFFFFF.
        internal Int32[] Maybe_As_List { get; set; }

        // System.String aka Array of System.Char
        // This option is the one used preferentially in the general case,
        // except in special cases where the List form is known to be better.
        // This option supports any codepoints representable by UTF-16,
        // in the 21-bit range 0..0x10FFFF, which includes all of the
        // official Unicode standard character codepoints,
        // but System.String will also represent UTF-16 surrogate codepoints
        // (in the range 0xD800..0xDFFF) that aren't in a valid surrogate pair.
        // A .Net String is conceptualized in terms of an array of Char objects
        // each of which is a UTF-16 code unit; conceptualization in terms
        // of whole Unicode code points is NOT what String does; so for example
        // String.Length will return 2 for a surrogate pair rather than 1.
        // Moreover, it appears that a .Net String can also be mal-formed
        // such as having either high or low surrogates outside of pairs.
        // https://codeblog.jonskeet.uk/2014/11/07/when-is-a-string-not-a-string/
        // In contrast, the official count of codepoints in a Codepoint_Array
        // is based on its List representation (a valid UTF-16 surrogate pair = 1).
        // For now, Codepoint_Array supports lossless conversion between its
        // List and String forms, meaning valid surrogate pairs in a String
        // will be merged into a single List element, while any other Char that
        // isn't part of a valid surrogate pair will be retained as 1 List element.
        internal String Maybe_As_String { get; set; }

        // Array of System.Byte
        // This option is the most compact format for typical character
        // data, using one octet per such character instead of 2 or 4.
        // It is used preferentially for data coming from or going to the
        // file system or user I/O or networks.
        // This option supports any codepoints representable by UTF-8,
        // in the 21-bit range 0..0x10FFFF, which includes all of the
        // official Unicode standard character codepoints, and possibly more.
        // This option is also valid ASCII when all codepoints are in 0..127.
        internal Byte[] Maybe_As_UTF_8_Octets { get; set; }

        // Nullable Boolean
        // This is true if we know that all of the character codepoints are
        // valid standard Unicode codepoints, and false if we know any aren't.
        internal Nullable<Boolean> Cached_Is_Unicode { get; set; }

        internal Nullable<Int32> Cached_HashCode { get; set; }

        internal Codepoint_Array(Int32[] value)
        {
            Maybe_As_List = value;
        }

        internal Codepoint_Array(String value)
        {
            Maybe_As_String = value;
        }

        internal Codepoint_Array(Byte[] value)
        {
            Maybe_As_UTF_8_Octets = value;
        }

        internal Int32[] As_List()
        {
            if (Maybe_As_List == null)
            {
                if (Maybe_As_String == null)
                {
                    if (Maybe_As_UTF_8_Octets == null)
                    {
                        throw new InvalidOperationException(
                            "Can't produce List representation from"
                            + " Codepoint_Array as it is malformed.");
                    }
                    String s = As_String();
                }
                List<Int32> list = new List<Int32>(Maybe_As_String.Length);
                Cached_Is_Unicode = true;
                for (Int32 i = 0; i < Maybe_As_String.Length; i++)
                {
                    if ((i+1) < Maybe_As_String.Length
                        && Char.IsSurrogatePair(Maybe_As_String[i], Maybe_As_String[i+1]))
                    {
                        list.Add(Char.ConvertToUtf32(
                            Maybe_As_String[i], Maybe_As_String[i+1]));
                        i++;
                    }
                    else
                    {
                        list.Add(Maybe_As_String[i]);
                        if (Char.IsSurrogate(Maybe_As_String[i]))
                        {
                            Cached_Is_Unicode = false;
                        }
                    }
                }
                Maybe_As_List = list.ToArray();
            }
            return Maybe_As_List;
        }

        internal String As_String()
        {
            if (Maybe_As_String == null)
            {
                if (Maybe_As_UTF_8_Octets != null)
                {
                    UTF8Encoding enc = new UTF8Encoding
                    (
                        encoderShouldEmitUTF8Identifier: false,
                        throwOnInvalidBytes: true
                    );
                    try
                    {
                        Maybe_As_String = enc.GetString(Maybe_As_UTF_8_Octets);
                    }
                    catch (Exception e)
                    {
                        throw new FormatException(
                            message: "Can't produce String representation"
                                + " from Codepoint_Array as its UTF_8_Octets"
                                + " representation is not valid UTF-8.",
                            innerException: e
                        );
                    }
                    return Maybe_As_String;
                }
                if (Maybe_As_List == null)
                {
                    throw new InvalidOperationException(
                        "Can't produce String representation from"
                        + " Codepoint_Array as it is malformed.");
                }
                StringBuilder sb = new StringBuilder(
                    Maybe_As_List.Length, 2 * Maybe_As_List.Length);
                Cached_Is_Unicode = true;
                for (Int32 i = 0; i < Maybe_As_List.Length; i++)
                {
                    if (Maybe_As_List[i] < 0)
                    {
                        Cached_Is_Unicode = false;
                        throw new InvalidOperationException(
                            "Can't produce String representation from Codepoint_Array"
                            + " as its List representation includes negative codepoints.");
                    }
                    else if (Maybe_As_List[i] <= 0xFFFF)
                    {
                        sb.Append((Char)Maybe_As_List[i]);
                        if (Maybe_As_List[i] >= 0xD800 && Maybe_As_List[i] <= 0xDFFF)
                        {
                            Cached_Is_Unicode = false;
                        }
                    }
                    else if (Maybe_As_List[i] <= 0x10FFFF)
                    {
                        sb.Append(Char.ConvertFromUtf32(Maybe_As_List[i]));
                    }
                    else
                    {
                        Cached_Is_Unicode = false;
                        throw new InvalidOperationException(
                            "Can't produce String representation from Codepoint_Array"
                            + " as its List representation includes codepoints above"
                            + " the 21-bit range a UTF-16 .Net String can represent.");
                    }
                }
                Maybe_As_String = sb.ToString();
            }
            return Maybe_As_String;
        }

        internal Byte[] As_UTF_8_Octets()
        {
            if (Maybe_As_UTF_8_Octets == null)
            {
                if (Maybe_As_String == null)
                {
                    if (Maybe_As_List == null)
                    {
                        throw new InvalidOperationException(
                            "Can't produce UTF_8_Octets representation from"
                            + " Codepoint_Array as it is malformed.");
                    }
                    String s = As_String();
                }
                UTF8Encoding enc = new UTF8Encoding
                (
                    encoderShouldEmitUTF8Identifier: false,
                    throwOnInvalidBytes: true
                );
                try
                {
                    Maybe_As_UTF_8_Octets = enc.GetBytes(Maybe_As_String);
                }
                catch (Exception e)
                {
                    throw new FormatException(
                        message: "Can't produce UTF_8_Octets representation"
                            + " from Codepoint_Array as its String"
                            + " representation is invalid somehow.",
                        innerException: e
                    );
                }
            }
            return Maybe_As_UTF_8_Octets;
        }

        internal Boolean Is_Unicode()
        {
            if (Cached_Is_Unicode == null)
            {
                if (Maybe_As_List != null)
                {
                    Cached_Is_Unicode = Enumerable.All(Maybe_As_List,
                        c => c >= 0 && c <= 0xD7FF || c >= 0xE000 && c <= 0x10FFFF);
                }
                else if (Maybe_As_String != null || Maybe_As_UTF_8_Octets != null)
                {
                    String s = As_String();
                    Cached_Is_Unicode = true;
                    for (Int32 i = 0; i < s.Length; i++)
                    {
                        if (Char.IsSurrogate(s[i])
                            && !((i+1) < s.Length
                            && Char.IsSurrogatePair(s[i], s[i+1])))
                        {
                            Cached_Is_Unicode = false;
                            break;
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException(
                        "Can't determine if Codepoint_Array is valid Unicode"
                        + " as it is malformed.");
                }
            }
            return (Boolean)Cached_Is_Unicode;
        }

        internal Boolean Has_Any_Members()
        {
            if (Maybe_As_String != null)
            {
                return Maybe_As_String != "";
            }
            else if (Maybe_As_List != null)
            {
                return Maybe_As_List.Length != 0;  // Is there a faster test?
            }
            else if (Maybe_As_UTF_8_Octets != null)
            {
                return Maybe_As_UTF_8_Octets.Length != 0;
            }
            else
            {
                throw new InvalidOperationException(
                    "Can't determine if Codepoint_Array has any members"
                    + " as it is malformed.");
            }
        }

        internal Int32 Count()
        {
            return this.As_List().Length;
        }

        internal Boolean May_Cache()
        {
            if (Maybe_As_List != null)
            {
                return Maybe_As_List.Length <= 200;
            }
            else if (Maybe_As_String != null)
            {
                return Maybe_As_String.Length <= 200;
            }
            else if (Maybe_As_UTF_8_Octets != null)
            {
                return Maybe_As_UTF_8_Octets.Length <= 200;
            }
            else
            {
                return false;  // This is a malformed Codepoint_Array object.
            }
        }
    }

    internal class Codepoint_Array_Comparer : EqualityComparer<Codepoint_Array>
    {
        public override Boolean Equals(Codepoint_Array v1, Codepoint_Array v2)
        {
            if (v1 == null && v2 == null)
            {
                // Would we ever get here?
                return true;
            }
            if (v1 == null || v2 == null)
            {
                return false;
            }
            if (Object.ReferenceEquals(v1, v2))
            {
                return true;
            }
            if (v1.Maybe_As_String == null || v2.Maybe_As_String == null)
            {
                if (v1.Maybe_As_List != null && v2.Maybe_As_List != null)
                {
                    // We don't yet have both String representations but we do
                    // already have both List representations, so use the latter.
                    return Enumerable.SequenceEqual(v1.Maybe_As_List, v2.Maybe_As_List);
                }
                if (v1.Maybe_As_UTF_8_Octets != null && v2.Maybe_As_UTF_8_Octets != null)
                {
                    return Enumerable.SequenceEqual(
                        v1.Maybe_As_UTF_8_Octets, v2.Maybe_As_UTF_8_Octets);
                }
            }
            // We don't have a reason to prefer using List reprs so use String reprs.
            return v1.As_String() == v2.As_String();
        }

        // We always hash using the same representations to guarantee
        // that two logically same Codepoint_Array hash the same;
        // for now that representation of choice is String in the interest
        // of overall performance and potentially better security,
        // but at the cost that we'd have to come up with a plan if users
        // would reasonably ever use codepoints outside the 21 bit Unicode range.
        // THE FOLLOWING LINES ARE SPECIFIC TO THE COMMENTED-OUT LIST VERSION.
        // When hashing the string we merge each 4 consecutive characters by
        // catenation of each one's lower 8 bits so we can use millions of
        // hash buckets rather than just about 64 buckets (#letters+digits);
        // however it doesn't resist degeneration of hash-based collections
        // such as when many strings used as keys have just 8-character
        // repetitions in the form ABCDABCD and so all resolve to bucket 0.

        public override Int32 GetHashCode(Codepoint_Array v)
        {
            if (v == null)
            {
                // Would we ever get here?
                return 0;
            }
            if (v.Cached_HashCode == null)
            {
                v.Cached_HashCode = v.As_String().GetHashCode();
                // v.Cached_HashCode = 0;
                // Int32[] members = v.As_List().ToArray();
                // for (Int32 i = 0; i < members.Length; i += 4)
                // {
                    // Int32 chunk_size = Math.Min(4, members.Length - i);
                    // Int32 m1 =                  16777216 * (members[i  ] % 128);
                    // Int32 m2 = (chunk_size <= 2) ? 65536 * (members[i+1] % 256) : 0;
                    // Int32 m3 = (chunk_size <= 3) ?   256 * (members[i+2] % 256) : 0;
                    // Int32 m4 = (chunk_size <= 4) ?          members[i+3] % 256  : 0;
                    // v.Cached_HashCode = v.Cached_HashCode ^ (m1 + m2 + m3 + m4);
                // }
            }
            return (Int32)v.Cached_HashCode;
        }
    }
}
