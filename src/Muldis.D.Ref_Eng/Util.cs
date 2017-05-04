namespace Muldis.D.Ref_Eng.Util
{
    // Muldis.D.Ref_Eng.Util.Codepoint_Array
    // This type is for internal use only.
    // Conceptually an Array of Unicode character codepoints, but also
    // supports non-Unicode codepoints, integers in the range 0..0x7FFFFFFF.
    // Used to represent a Muldis D Tuple attribute name, any unqualified identifier.
    // Used as canonical storage type for a Muldis D Text value / a regular character string.
    // Used as internal generic Muldis D value serialization format for indexing.
    // Conceptually an ordered sequence of System.Int32 and actually either
    // that or one of several alternate storage foramts.

    public class Codepoint_Array
    {
        public System.Collections.Generic.List<System.Int32> As_List { get; set; }

        public System.Nullable<System.Int32> Cached_HashCode { get; set; }
    }

    public class Codepoint_Array_Comparer
        : System.Collections.Generic.EqualityComparer<Codepoint_Array>
    {
        public override System.Boolean Equals(Codepoint_Array v1, Codepoint_Array v2)
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
            if (System.Object.ReferenceEquals(v1, v2))
            {
                return true;
            }
            if (v1.As_List.Count != v2.As_List.Count)
            {
                return false;
            }
            if (v1.As_List.Count == 0)
            {
                return true;
            }
            return System.Linq.Enumerable.SequenceEqual(v1.As_List, v2.As_List);
        }

        // When hashing the string we merge each 4 consecutive characters by
        // catenation of each one's lower 8 bits so we can use millions of
        // hash buckets rather than just about 64 buckets (#letters+digits);
        // however it doesn't resist degeneration of hash-based collections
        // such as when many strings used as keys have just 8-character
        // repetitions in the form ABCDABCD and so all resolve to bucket 0.

        public override System.Int32 GetHashCode(Codepoint_Array v)
        {
            if (v == null)
            {
                // Would we ever get here?
                return 0;
            }
            if (v.Cached_HashCode == null)
            {
                v.Cached_HashCode = 0;
                System.Int32[] members = v.As_List.ToArray();
                for (System.Int32 i = 0; i < members.Length; i += 4)
                {
                    System.Int32 chunk_size = System.Math.Min(4, members.Length - i);
                    System.Int32 m1 =                  16777216 * (members[i  ] % 128);
                    System.Int32 m2 = (chunk_size <= 2) ? 65536 * (members[i+1] % 256) : 0;
                    System.Int32 m3 = (chunk_size <= 3) ?   256 * (members[i+2] % 256) : 0;
                    System.Int32 m4 = (chunk_size <= 4) ?          members[i+3] % 256  : 0;
                    v.Cached_HashCode = v.Cached_HashCode ^ (m1 + m2 + m3 + m4);
                }
            }
            return (System.Int32)v.Cached_HashCode;
        }
    }
}
