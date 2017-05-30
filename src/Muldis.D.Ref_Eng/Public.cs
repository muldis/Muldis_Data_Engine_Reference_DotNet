using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Muldis.DBP;
using Muldis.D.Ref_Eng;
using Muldis.D.Ref_Eng.Value;

[assembly: CLSCompliant(true)]

namespace Muldis.D.Ref_Eng
{
    public class Info : IInfo
    {
        public Boolean Provides_Dot_Net_Muldis_DBP()
        {
            return true;
        }

        public IMachine Want_VM_API(Object requested_version)
        {
            String[] only_supported_version = new String[]
                {"Muldis_DBP_DotNet", "http://muldis.com", "0.1.0.-9"};
            if (requested_version != null
                && requested_version.GetType().FullName == "System.String[]")
            {
                if (Enumerable.SequenceEqual(
                    (String[])requested_version, only_supported_version))
                {
                    // We support the requested specific MDBP version.
                    return new Machine().init();
                }
            }
            // We don't support the requested specific MDBP version.
            return null;
        }
    }

    public class Machine : IMachine
    {
        internal Core.Memory m_memory;

        internal Machine init()
        {
            m_memory = new Core.Memory();
            return this;
        }

        public IImporter Importer()
        {
            return new Importer().init(this);
        }
    }

    public class Importer : IImporter
    {
        internal Machine     m_machine;
        internal Core.Memory m_memory;

        internal Importer init(Machine machine)
        {
            m_machine = machine;
            m_memory  = machine.m_memory;
            return this;
        }

        public IMachine Machine()
        {
            return m_machine;
        }

        public IMD_Any MD_Any(Object value)
        {
            if (value != null)
            {
                String type_name = value.GetType().FullName;
                if (type_name.StartsWith("Muldis.D.Ref_Eng.Value."))
                {
                    return (IMD_Any)value;
                }
                if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
                {
                    return Best_Fit_Public_Value(Import_Qualified(
                        (KeyValuePair<String,Object>)value));
                }
            }
            return Best_Fit_Public_Value(Import_Unqualified(value));
        }

        internal IMD_Any Best_Fit_Public_Value(Core.MD_Any value)
        {
            switch (value.AS.MD_Foundation_Type)
            {
                case Core.MD_Foundation_Type.MD_Boolean:
                    return (IMD_Boolean)new MD_Boolean().init(m_machine, value);
                case Core.MD_Foundation_Type.MD_Integer:
                    return (IMD_Integer)new MD_Integer().init(m_machine, value);
                case Core.MD_Foundation_Type.MD_Array:
                    if (value.AS.Cached_WKT.Contains(Core.MD_Well_Known_Type.String))
                    {
                        return (MD_String)new MD_String().init(m_machine, value);
                    }
                    return (IMD_Array)new MD_Array().init(m_machine, value);
                case Core.MD_Foundation_Type.MD_Bag:
                    return (IMD_Bag)new MD_Bag().init(m_machine, value);
                case Core.MD_Foundation_Type.MD_Tuple:
                    if (value.AS.Cached_WKT.Contains(Core.MD_Well_Known_Type.Heading))
                    {
                        return (MD_Heading)new MD_Heading().init(m_machine, value);
                    }
                    return (IMD_Tuple)new MD_Tuple().init(m_machine, value);
                case Core.MD_Foundation_Type.MD_Capsule:
                    if (value.AS.Cached_WKT.Contains(Core.MD_Well_Known_Type.Fraction))
                    {
                        return (MD_Fraction)new MD_Fraction().init(m_machine, value);
                    }
                    if (value.AS.Cached_WKT.Contains(Core.MD_Well_Known_Type.Bits))
                    {
                        return (MD_Bits)new MD_Bits().init(m_machine, value);
                    }
                    if (value.AS.Cached_WKT.Contains(Core.MD_Well_Known_Type.Blob))
                    {
                        return (MD_Blob)new MD_Blob().init(m_machine, value);
                    }
                    if (value.AS.Cached_WKT.Contains(Core.MD_Well_Known_Type.Text))
                    {
                        return (MD_Text)new MD_Text().init(m_machine, value);
                    }
                    if (value.AS.Cached_WKT.Contains(Core.MD_Well_Known_Type.Set))
                    {
                        return (MD_Set)new MD_Set().init(m_machine, value);
                    }
                    if (value.AS.Cached_WKT.Contains(Core.MD_Well_Known_Type.Tuple_Array))
                    {
                        return (MD_Tuple_Array)new MD_Tuple_Array().init(m_machine, value);
                    }
                    if (value.AS.Cached_WKT.Contains(Core.MD_Well_Known_Type.Relation))
                    {
                        return (MD_Relation)new MD_Relation().init(m_machine, value);
                    }
                    if (value.AS.Cached_WKT.Contains(Core.MD_Well_Known_Type.Tuple_Bag))
                    {
                        return (MD_Tuple_Bag)new MD_Tuple_Bag().init(m_machine, value);
                    }
                    if (value.AS.Cached_WKT.Contains(Core.MD_Well_Known_Type.Interval))
                    {
                        return (MD_Interval)new MD_Interval().init(m_machine, value);
                    }
                    if (value.AS.Cached_WKT.Contains(Core.MD_Well_Known_Type.Excuse))
                    {
                        if (ReferenceEquals(value, Core_MD_Excuse("No_Reason")))
                        {
                            return (MD_Excuse_No_Reason)new MD_Excuse_No_Reason()
                                .init(m_machine, value);
                        }
                        return (MD_Excuse)new MD_Excuse().init(m_machine, value);
                    }
                    return (MD_Capsule)new MD_Capsule().init(m_machine, value);
                case Core.MD_Foundation_Type.MD_Handle:
                    switch (value.AS.MD_Handle.MD_Handle_Type)
                    {
                        case Core.MD_Handle_Type.MD_Variable:
                            return (IMD_Variable)new MD_Variable().init(m_machine, value);
                        case Core.MD_Handle_Type.MD_Process:
                            return (MD_Process)new MD_Process().init(m_machine, value);
                        case Core.MD_Handle_Type.MD_Stream:
                            return (MD_Stream)new MD_Stream().init(m_machine, value);
                        case Core.MD_Handle_Type.MD_External:
                            return (MD_External)new MD_External().init(m_machine, value);
                        default:
                            throw new NotImplementedException();
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        private Core.MD_Any Core_MD_Any(Object value)
        {
            if (value != null)
            {
                String type_name = value.GetType().FullName;
                if (type_name.StartsWith("Muldis.D.Ref_Eng.Value."))
                {
                    return ((MD_Any)value).m_value;
                }
                if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
                {
                    return Import_Qualified((KeyValuePair<String,Object>)value);
                }
            }
            return Import_Unqualified(value);
        }

        private Core.MD_Any Import_Qualified(KeyValuePair<String,Object> value)
        {
            Object v = value.Value;
            // Note that .Net guarantees the .Key is never null.
            if (v == null && value.Key != "Excuse")
            {
                throw new ArgumentNullException
                (
                    paramName: "value",
                    message: "Can't select MD_Any with a KeyValuePair operand"
                        + " with a null Value property (except with [Excuse] Key)."
                );
            }
            String type_name = v == null ? null : v.GetType().FullName;
            switch (value.Key)
            {
                case "Boolean":
                    if (type_name == "System.Boolean")
                    {
                        return Core_MD_Boolean((Boolean)v);
                    }
                    break;
                case "Integer":
                    if (type_name == "System.Int32")
                    {
                        return Core_MD_Integer((Int32)v);
                    }
                    if (type_name == "System.Numerics.BigInteger")
                    {
                        return Core_MD_Integer((BigInteger)v);
                    }
                    break;
                case "Fraction":
                    if (type_name == "System.Decimal")
                    {
                        return Core_MD_Fraction((Decimal)v);
                    }
                    if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
                    {
                        Object numerator   = ((KeyValuePair<Object,Object>)v).Key;
                        Object denominator = ((KeyValuePair<Object,Object>)v).Value;
                        // Note that .Net guarantees the .Key is never null.
                        if (denominator == null)
                        {
                            throw new ArgumentNullException
                            (
                                paramName: "value",
                                message: "Can't select MD_Fraction with a null denominator."
                            );
                        }
                        if (numerator.GetType().FullName == "System.Int32"
                            && denominator.GetType().FullName == "System.Int32")
                        {
                            return Core_MD_Fraction((Int32)numerator, (Int32)denominator);
                        }
                        if (numerator.GetType().FullName == "System.Numerics.BigInteger"
                            && denominator.GetType().FullName == "System.Numerics.BigInteger")
                        {
                            return Core_MD_Fraction((BigInteger)numerator, (BigInteger)denominator);
                        }
                        if (numerator.GetType().FullName == "Muldis.D.Ref_Eng.Value.MD_Integer"
                            && denominator.GetType().FullName == "Muldis.D.Ref_Eng.Value.MD_Integer")
                        {
                            return Core_MD_Fraction(
                                ((MD_Integer)numerator  ).m_value.AS.MD_Integer,
                                ((MD_Integer)denominator).m_value.AS.MD_Integer
                            );
                        }
                    }
                    break;
                case "String":
                    if (type_name == "System.Int32[]")
                    {
                        return Core_MD_String((Int32[])v);
                    }
                    if (type_name == "System.Numerics.BigInteger[]")
                    {
                        return Core_MD_String((BigInteger[])v);
                    }
                    break;
                case "Bits":
                    if (type_name == "System.Collections.BitArray")
                    {
                        return Core_MD_Bits((BitArray)v);
                    }
                    break;
                case "Blob":
                    if (type_name == "System.Byte[]")
                    {
                        return Core_MD_Blob((Byte[])v);
                    }
                    break;
                case "Text":
                    if (type_name == "System.String")
                    {
                        return Core_MD_Text((String)v);
                    }
                    break;
                case "Array":
                    if (type_name.StartsWith("System.Collections.Generic.List`"))
                    {
                        return Core_MD_Array((List<Object>)v);
                    }
                    break;
                case "Set":
                    if (type_name.StartsWith("System.Collections.Generic.List`"))
                    {
                        return Core_MD_Set((List<Object>)v);
                    }
                    break;
                case "Bag":
                    if (type_name.StartsWith("System.Collections.Generic.List`"))
                    {
                        return Core_MD_Bag((List<Object>)v);
                    }
                    break;
                case "Tuple":
                    if (type_name.StartsWith("System.Collections.Generic.Dictionary`"))
                    {
                        return Core_MD_Tuple(attrs: (Dictionary<String,Object>)v);
                    }
                    break;
                case "Heading":
                    if (type_name.StartsWith("System.Collections.Generic.HashSet`"))
                    {
                        return Core_MD_Heading(attr_names: (HashSet<String>)v);
                    }
                    break;
                case "Tuple_Array":
                    if (type_name == "Muldis.D.Ref_Eng.Value.MD_Heading")
                    {
                        return Core_MD_Tuple_Array(heading: (Muldis.D.Ref_Eng.Value.MD_Heading)v);
                    }
                    if (type_name == "Muldis.D.Ref_Eng.Value.MD_Set")
                    {
                        return Core_MD_Tuple_Array(body: (Muldis.D.Ref_Eng.Value.MD_Array)v);
                    }
                    break;
                case "Relation":
                    if (type_name == "Muldis.D.Ref_Eng.Value.MD_Heading")
                    {
                        return Core_MD_Relation(heading: (Muldis.D.Ref_Eng.Value.MD_Heading)v);
                    }
                    if (type_name == "Muldis.D.Ref_Eng.Value.MD_Set")
                    {
                        return Core_MD_Relation(body: (Muldis.D.Ref_Eng.Value.MD_Set)v);
                    }
                    break;
                case "Tuple_Bag":
                    if (type_name == "Muldis.D.Ref_Eng.Value.MD_Heading")
                    {
                        return Core_MD_Tuple_Bag(heading: (Muldis.D.Ref_Eng.Value.MD_Heading)v);
                    }
                    if (type_name == "Muldis.D.Ref_Eng.Value.MD_Set")
                    {
                        return Core_MD_Tuple_Bag(body: (Muldis.D.Ref_Eng.Value.MD_Bag)v);
                    }
                    break;
                case "Interval":
                    if (type_name.StartsWith("System.Collections.Generic.Dictionary`"))
                    {
                        Dictionary<String,Object> attrs = (Dictionary<String,Object>)v;
                        if (!attrs.ContainsKey("min"))
                        {
                            throw new KeyNotFoundException(
                                "Can't select MD_Interval without a [min] value.");
                        }
                        if (!attrs.ContainsKey("max"))
                        {
                            throw new KeyNotFoundException(
                                "Can't select MD_Interval without a [max] value.");
                        }
                        if (attrs.ContainsKey("excludes_min")
                            && attrs["excludes_min"].GetType().FullName != "System.Boolean")
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Interval with a non-Boolean [excludes_min]."
                            );
                        }
                        if (attrs.ContainsKey("excludes_max")
                            && attrs["excludes_max"].GetType().FullName != "System.Boolean")
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Interval with a non-Boolean [excludes_max]."
                            );
                        }
                        return Core_MD_Interval(
                            min: Core_MD_Any(attrs["min"]),
                            max: Core_MD_Any(attrs["max"]),
                            excludes_min: attrs.ContainsKey("excludes_min")
                                ? (Boolean)attrs["excludes_min"] : false,
                            excludes_max: attrs.ContainsKey("excludes_max")
                                ? (Boolean)attrs["excludes_max"] : false
                        );
                    }
                    break;
                case "Capsule":
                    if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
                    {
                        Object label = ((KeyValuePair<Object,Object>)v).Key;
                        Object attrs = ((KeyValuePair<Object,Object>)v).Value;
                        // Note that .Net guarantees the .Key is never null.
                        if (attrs == null)
                        {
                            throw new ArgumentNullException
                            (
                                paramName: "value",
                                message: "Can't select MD_Capsule with a null Capsule attrs."
                            );
                        }
                        Core.MD_Any attrs_cv = Core_MD_Any(attrs);
                        if (attrs_cv.AS.MD_Foundation_Type != Core.MD_Foundation_Type.MD_Tuple)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Capsule with a Capsule attrs"
                                    + " that doesn't evaluate as a MD_Tuple."
                            );
                        }
                        if (label.GetType().FullName == "System.String")
                        {
                            return m_memory.MD_Capsule(
                                m_memory.MD_Attr_Name((String)label),
                                attrs_cv
                            );
                        }
                        if (label.GetType().FullName == "System.String[]")
                        {
                            return m_memory.MD_Capsule(
                                m_memory.MD_Array(new List<Core.MD_Any>(((String[])label).Select(
                                    m => m_memory.MD_Attr_Name(m)
                                ))),
                                attrs_cv
                            );
                        }
                        if (label.GetType().FullName.StartsWith("Muldis.D.Ref_Eng.Value."))
                        {
                            return m_memory.MD_Capsule(Core_MD_Any(label), attrs_cv);
                        }
                    }
                    break;
                case "New_Variable":
                    if (type_name.StartsWith("Muldis.D.Ref_Eng.Value."))
                    {
                        return Core_MD_Variable((Muldis.D.Ref_Eng.Value.MD_Any)v);
                    }
                    break;
                case "New_Process":
                    if (v == null)
                    {
                        return Core_MD_Process();
                    }
                    break;
                case "New_Stream":
                    if (v == null)
                    {
                        return Core_MD_Stream();
                    }
                    break;
                case "New_External":
                    return Core_MD_External(v);
                case "Excuse":
                    if (v == null)
                    {
                        return Core_MD_Excuse("No_Reason");
                    }
                    if (type_name == "System.DBNull")
                    {
                        return Core_MD_Excuse("No_Reason");
                    }
                    if (type_name == "System.String")
                    {
                        return Core_MD_Excuse((String)v);
                    }
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException(
                        "Unhandled MDBP value type ["+value.Key+"]+["+type_name+"].");
            }
            // Duplicated as Visual Studio says otherwise not all code paths return a value.
            throw new NotImplementedException(
                "Unhandled MDBP value type ["+value.Key+"]+["+type_name+"].");
        }

        private Core.MD_Any Import_Unqualified(Object value)
        {
            if (value == null)
            {
                return Core_MD_Excuse("No_Reason");
            }
            String type_name = value.GetType().FullName;
            if (type_name == "System.DBNull")
            {
                return Core_MD_Excuse("No_Reason");
            }
            if (type_name == "System.Boolean")
            {
                return Core_MD_Boolean((Boolean)value);
            }
            if (type_name == "System.Int32")
            {
                return Core_MD_Integer((Int32)value);
            }
            if (type_name == "System.Numerics.BigInteger")
            {
                return Core_MD_Integer((BigInteger)value);
            }
            if (type_name == "System.Decimal")
            {
                return Core_MD_Fraction((Decimal)value);
            }
            if (type_name == "System.Int32[]")
            {
                return Core_MD_String((Int32[])value);
            }
            if (type_name == "System.Numerics.BigInteger[]")
            {
                return Core_MD_String((BigInteger[])value);
            }
            if (type_name == "System.Collections.BitArray")
            {
                return Core_MD_Bits((BitArray)value);
            }
            if (type_name == "System.Byte[]")
            {
                return Core_MD_Blob((Byte[])value);
            }
            if (type_name == "System.String")
            {
                return Core_MD_Text((String)value);
            }
            throw new NotImplementedException("Unhandled MDBP value type ["+type_name+"].");
        }

        public IMD_Boolean MD_Boolean(Boolean value)
        {
            return (IMD_Boolean)new MD_Boolean().init(m_machine,
                Core_MD_Boolean(value));
        }

        private Core.MD_Any Core_MD_Boolean(Boolean value)
        {
            return m_memory.MD_Boolean(value);
        }

        public IMD_Integer MD_Integer(BigInteger value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            return (IMD_Integer)new MD_Integer().init(m_machine,
                Core_MD_Integer(value));
        }

        public IMD_Integer MD_Integer(Int32 value)
        {
            return (IMD_Integer)new MD_Integer().init(m_machine,
                Core_MD_Integer(value));
        }

        private Core.MD_Any Core_MD_Integer(BigInteger value)
        {
            return m_memory.MD_Integer(value);
        }

        public IMD_Fraction MD_Fraction(IMD_Integer numerator, IMD_Integer denominator)
        {
            if (numerator == null)
            {
                throw new ArgumentNullException("numerator");
            }
            if (denominator == null)
            {
                throw new ArgumentNullException("denominator");
            }
            return (IMD_Fraction)new MD_Fraction().init(m_machine,
                Core_MD_Fraction(
                    ((MD_Integer)numerator  ).m_value.AS.MD_Integer,
                    ((MD_Integer)denominator).m_value.AS.MD_Integer
                )
            );
        }

        public IMD_Fraction MD_Fraction(BigInteger numerator, BigInteger denominator)
        {
            if (numerator == null)
            {
                throw new ArgumentNullException("numerator");
            }
            if (denominator == null)
            {
                throw new ArgumentNullException("denominator");
            }
            return (IMD_Fraction)new MD_Fraction().init(m_machine,
                Core_MD_Fraction(numerator, denominator));
        }

        public IMD_Fraction MD_Fraction(Int32 numerator, Int32 denominator)
        {
            return (IMD_Fraction)new MD_Fraction().init(m_machine,
                Core_MD_Fraction(numerator, denominator));
        }

        public IMD_Fraction MD_Fraction(Decimal value)
        {
            return (IMD_Fraction)new MD_Fraction().init(m_machine,
                Core_MD_Fraction(value));
        }

        private Core.MD_Any Core_MD_Fraction(Decimal value)
        {
            Int32[] dec_bits = Decimal.GetBits(value);
            // https://msdn.microsoft.com/en-us/library/system.decimal.getbits(v=vs.110).aspx
            // The GetBits spec says that it returns 4 32-bit integers
            // representing the 128 bits of the Decimal itself; of these,
            // the first 3 integers' bits give the mantissa,
            // the 4th integer's bits give the sign and the scale factor.
            // "Bits 16 to 23 must contain an exponent between 0 and 28,
            // which indicates the power of 10 to divide the integer number."
            Int32 scale_factor_int = ((dec_bits[3] >> 16) & 0x7F);
            Decimal denominator_dec = 1M;
            // Decimal doesn't have exponentiation op so we do it manually.
            for (Int32 i = 1; i <= scale_factor_int; i++)
            {
                denominator_dec = denominator_dec * 10M;
            }
            Decimal numerator_dec = value * denominator_dec;
            return Core_MD_Fraction(new BigInteger(numerator_dec),
                new BigInteger(denominator_dec));
        }

        private Core.MD_Any Core_MD_Fraction(BigInteger numerator, BigInteger denominator)
        {
            if (denominator == 0)
            {
                throw new ArgumentException
                (
                    paramName: "denominator",
                    message: "Can't select MD_Fraction with a denominator of zero."
                );
            }
            // Note that GreatestCommonDivisor() always has a non-negative result.
            BigInteger gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);
            if (gcd > 1)
            {
                // Make the numerator and denominator coprime.
                numerator   = (denominator > 0 ? numerator   : -numerator  ) / gcd;
                denominator = (denominator > 0 ? denominator : -denominator) / gcd;
            }
            Core.MD_Any fraction = m_memory.MD_Capsule(
                m_memory.MD_Attr_Name("Fraction"),
                m_memory.MD_Tuple(
                    multi_oa: new Dictionary<String,Core.MD_Any>()
                    {
                        {"numerator"  , m_memory.MD_Integer(numerator  )},
                        {"denominator", m_memory.MD_Integer(denominator)},
                    }
                )
            );
            fraction.AS.Cached_WKT.Add(Core.MD_Well_Known_Type.Fraction);
            return fraction;
        }

        public IMD_String MD_String(BigInteger[] members)
        {
            if (members == null)
            {
                throw new ArgumentNullException("members");
            }
            return (IMD_String)new MD_String().init(m_machine,
                Core_MD_String(members));
        }

        public IMD_String MD_String(Int32[] members)
        {
            if (members == null)
            {
                throw new ArgumentNullException("members");
            }
            return (IMD_String)new MD_String().init(m_machine,
                Core_MD_String(members));
        }

        private Core.MD_Any Core_MD_String(BigInteger[] members)
        {
            return m_memory.MD_Array(
                members: new List<Core.MD_Any>(members.Select(
                    m => m_memory.MD_Integer(m)
                )),
                known_is_string: true
            );
        }

        private Core.MD_Any Core_MD_String(Int32[] members)
        {
            return m_memory.Int32_MD_String(members.ToArray());
        }

        public IMD_Bits MD_Bits(BitArray members)
        {
            if (members == null)
            {
                throw new ArgumentNullException("members");
            }
            return (IMD_Bits)new MD_Bits().init(m_machine,
                Core_MD_Bits(members));
        }

        private Core.MD_Any Core_MD_Bits(BitArray members)
        {
            // BitArrays are mutable so clone argument to protect our internals.
            Core.MD_Any bits = m_memory.MD_Capsule(
                m_memory.MD_Attr_Name("Bits"),
                m_memory.MD_Tuple(
                    only_oa: new KeyValuePair<String,Core.MD_Any>("bits",
                        m_memory.Bit_MD_String(new BitArray(members)))
                )
            );
            bits.AS.Cached_WKT.Add(Core.MD_Well_Known_Type.Bits);
            return bits;
        }

        public IMD_Blob MD_Blob(Byte[] members)
        {
            if (members == null)
            {
                throw new ArgumentNullException("members");
            }
            return (IMD_Blob)new MD_Blob().init(m_machine,
                Core_MD_Blob(members));
        }

        private Core.MD_Any Core_MD_Blob(Byte[] members)
        {
            // Arrays are mutable so clone argument to protect our internals.
            Core.MD_Any blob = m_memory.MD_Capsule(
                m_memory.MD_Attr_Name("Blob"),
                m_memory.MD_Tuple(
                    only_oa: new KeyValuePair<String,Core.MD_Any>("octets",
                        m_memory.Octet_MD_String(members.ToArray()))
                )
            );
            blob.AS.Cached_WKT.Add(Core.MD_Well_Known_Type.Blob);
            return blob;
        }

        public IMD_Text MD_Text(String members)
        {
            if (members == null)
            {
                throw new ArgumentNullException("members");
            }
            return (IMD_Text)new MD_Text().init(m_machine,
                Core_MD_Text(members));
        }

        private Core.MD_Any Core_MD_Text(String members)
        {
            Core.MD_Any text = m_memory.MD_Capsule(
                m_memory.MD_Attr_Name("Text"),
                m_memory.MD_Tuple(
                    only_oa: new KeyValuePair<String,Core.MD_Any>(
                        "maximal_chars", m_memory.Codepoint_MD_String(members)
                    )
                )
            );
            text.AS.Cached_WKT.Add(Core.MD_Well_Known_Type.Text);
            return text;
        }

        public IMD_Array MD_Array(List<Object> members)
        {
            if (members == null)
            {
                throw new ArgumentNullException("members");
            }
            return (IMD_Array)new MD_Array().init(m_machine,
                Core_MD_Array(members));
        }

        private Core.MD_Any Core_MD_Array(List<Object> members)
        {
            return m_memory.MD_Array(
                new List<Core.MD_Any>(members.Select(
                    m => Core_MD_Any(m)
                ))
            );
        }

        public IMD_Set MD_Set(List<Object> members)
        {
            if (members == null)
            {
                throw new ArgumentNullException("members");
            }
            return (IMD_Set)new MD_Set().init(m_machine,
                Core_MD_Set(members));
        }

        private Core.MD_Any Core_MD_Set(List<Object> members)
        {
            Core.MD_Any set = m_memory.MD_Capsule(
                m_memory.MD_Attr_Name("Set"),
                m_memory.MD_Tuple(
                    only_oa: new KeyValuePair<String,Core.MD_Any>("members",
                        Core_MD_Bag(members: members, with_unique: true))
                )
            );
            set.AS.Cached_WKT.Add(Core.MD_Well_Known_Type.Set);
            return set;
        }

        public IMD_Bag MD_Bag(List<Object> members)
        {
            if (members == null)
            {
                throw new ArgumentNullException("members");
            }
            return (IMD_Bag)new MD_Bag().init(m_machine,
                Core_MD_Bag(members));
        }

        private Core.MD_Any Core_MD_Bag(
            List<Object> members, Boolean with_unique = false)
        {
            return m_memory.MD_Bag(
                members: new List<Core.Multiplied_Member>(members.Select(
                    m => new Core.Multiplied_Member(Core_MD_Any(m))
                )),
                with_unique: with_unique
            );
        }

        public IMD_Tuple MD_Tuple(
            Object a0 = null, Object a1 = null, Object a2 = null,
            Nullable<KeyValuePair<String,Object>> attr = null,
            Dictionary<String,Object> attrs = null)
        {
            return (IMD_Tuple)new MD_Tuple().init(m_machine,
                Core_MD_Tuple(a0, a1, a2, attr, attrs));
        }

        private Core.MD_Any Core_MD_Tuple(
            Object a0 = null, Object a1 = null, Object a2 = null,
            Nullable<KeyValuePair<String,Object>> attr = null,
            Dictionary<String,Object> attrs = null)
        {
            // Start by normalizing the distribution of the attribute
            // definitions among the arguments to match the distribution
            // used by our internals, and check for duplicate names.
            if (attr != null && attrs != null)
            {
                if (attrs.ContainsKey(attr.Value.Key))
                {
                    throw new ArgumentException
                    (
                        paramName: "attr",
                        message: "Can't select MD_Tuple with same-named"
                            + " attribute in both [attr] and [attrs] args."
                    );
                }
            }
            if (attr != null)
            {
                if (attr.Value.Key == "\u0000")
                {
                    if (a0 != null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "attr",
                            message: "Can't select MD_Tuple with same-named"
                                + " attribute in both [a0] and [attr] args."
                        );
                    }
                    a0 = attr.Value.Value;
                    attr = null;
                }
                if (attr.Value.Key == "\u0001")
                {
                    if (a1 != null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "attr",
                            message: "Can't select MD_Tuple with same-named"
                                + " attribute in both [a1] and [attr] args."
                        );
                    }
                    a1 = attr.Value.Value;
                    attr = null;
                }
                if (attr.Value.Key == "\u0002")
                {
                    if (a2 != null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "attr",
                            message: "Can't select MD_Tuple with same-named"
                                + " attribute in both [a2] and [attr] args."
                        );
                    }
                    a2 = attr.Value.Value;
                    attr = null;
                }
            }
            if (attrs != null)
            {
                if (attrs.ContainsKey("\u0000") || attrs.ContainsKey("\u0001")
                    || attrs.ContainsKey("\u0002"))
                {
                    // Dictionary are mutable so clone argument to protect caller.
                    attrs = new Dictionary<String,Object>(attrs);
                    if (attrs.ContainsKey("\u0000"))
                    {
                        if (a0 != null)
                        {
                            throw new ArgumentException
                            (
                                paramName: "attrs",
                                message: "Can't select MD_Tuple with same-named"
                                    + " attribute in both [a0] and [attrs] args."
                            );
                        }
                        a0 = attrs["\u0000"];
                        attrs.Remove("\u0000");
                    }
                    if (attrs.ContainsKey("\u0001"))
                    {
                        if (a1 != null)
                        {
                            throw new ArgumentException
                            (
                                paramName: "attrs",
                                message: "Can't select MD_Tuple with same-named"
                                    + " attribute in both [a1] and [attrs] args."
                            );
                        }
                        a1 = attrs["\u0001"];
                        attrs.Remove("\u0001");
                    }
                    if (attrs.ContainsKey("\u0002"))
                    {
                        if (a2 != null)
                        {
                            throw new ArgumentException
                            (
                                paramName: "attrs",
                                message: "Can't select MD_Tuple with same-named"
                                    + " attribute in both [a2] and [attrs] args."
                            );
                        }
                        a2 = attrs["\u0002"];
                        attrs.Remove("\u0002");
                    }
                }
                if (attr != null && attrs.Count >= 1)
                {
                    // Dictionary are mutable so clone argument to protect caller.
                    attrs = new Dictionary<String,Object>(attrs);
                    attrs.Add(attr.Value.Key, attr.Value.Value);
                    attr = null;
                }
                else if (attr == null && attrs.Count == 1)
                {
                    attr = Enumerable.Single(attrs);
                    attrs = null;
                }
                else if (attrs.Count == 0)
                {
                    attrs = null;
                }
            }
            // Now perform the attributes' importing proper.
            return m_memory.MD_Tuple(
                a0: a0 == null ? null : Core_MD_Any(a0),
                a1: a1 == null ? null : Core_MD_Any(a1),
                a2: a2 == null ? null : Core_MD_Any(a2),
                only_oa: attr == null
                    ? (Nullable<KeyValuePair<String,Core.MD_Any>>)null
                    : new KeyValuePair<String,Core.MD_Any>(
                        attr.Value.Key, Core_MD_Any(attr.Value.Value)),
                multi_oa: attrs == null ? null
                    : new Dictionary<String,Core.MD_Any>(attrs.ToDictionary(
                        a => a.Key, a => Core_MD_Any(a.Value)))
            );
        }

        public IMD_Heading MD_Heading(
            Nullable<Boolean> a0 = null, Nullable<Boolean> a1 = null,
            Nullable<Boolean> a2 = null,
            String attr_name = null, HashSet<String> attr_names = null)
        {
            return (IMD_Heading)new MD_Heading().init(m_machine,
                Core_MD_Heading(a0, a1, a2, attr_name, attr_names));
        }

        private Core.MD_Any Core_MD_Heading(
            Nullable<Boolean> a0 = null, Nullable<Boolean> a1 = null,
            Nullable<Boolean> a2 = null,
            String attr_name = null, HashSet<String> attr_names = null)
        {
            return Core_MD_Tuple(
                a0: (a0 == null || a0 == false) ? (Object)null : true,
                a1: (a1 == null || a1 == false) ? (Object)null : true,
                a2: (a2 == null || a2 == false) ? (Object)null : true,
                attr: attr_name == null
                    ? (Nullable<KeyValuePair<String,Object>>)null
                    : new KeyValuePair<String,Object>(attr_name, true),
                attrs: attr_names == null ? null
                    : new Dictionary<String,Object>(
                        attr_names.ToDictionary(a => a, a => (Object)true))
            );
        }

        public IMD_Tuple_Array MD_Tuple_Array(IMD_Heading heading)
        {
            if (heading == null)
            {
                throw new ArgumentNullException("heading");
            }
            return (IMD_Tuple_Array)new MD_Tuple_Array().init(m_machine,
                Core_MD_Tuple_Array(heading));
        }

        public IMD_Tuple_Array MD_Tuple_Array(IMD_Array body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }
            return (IMD_Tuple_Array)new MD_Tuple_Array().init(m_machine,
                Core_MD_Tuple_Array(body));
        }

        private Core.MD_Any Core_MD_Tuple_Array(IMD_Heading heading)
        {
            Core.MD_Any hv = ((MD_Heading)heading).m_value;
            Core.MD_Any bv = m_memory.MD_Array_C0;
            Core.MD_Any tuple_array = m_memory.MD_Capsule(
                m_memory.MD_Attr_Name("Tuple_Array"),
                m_memory.MD_Tuple(
                    multi_oa: new Dictionary<String,Core.MD_Any>()
                        {{"heading", hv}, {"body", bv}}
                )
            );
            tuple_array.AS.Cached_WKT.Add(Core.MD_Well_Known_Type.Tuple_Array);
            return tuple_array;
        }

        private Core.MD_Any Core_MD_Tuple_Array(IMD_Array body)
        {
            Core.MD_Any bv = ((MD_Array)body).m_value;
            if (!m_memory.Array__Is_Relational(bv))
            {
                throw new ArgumentException
                (
                    paramName: "body",
                    message: "Can't select MD_Tuple_Array from a MD_Array whose"
                        + " members aren't all MD_Tuple with a common heading."
                );
            }
            Core.MD_Any hv = m_memory.Array__Pick_Random_Member(bv);
            if (hv == null)
            {
                throw new ArgumentException
                (
                    paramName: "body",
                    message: "Can't select MD_Tuple_Array from empty MD_Array."
                );
            }
            Core.MD_Any tuple_array = m_memory.MD_Capsule(
                m_memory.MD_Attr_Name("Tuple_Array"),
                m_memory.MD_Tuple(
                    multi_oa: new Dictionary<String,Core.MD_Any>()
                        {{"heading", hv}, {"body", bv}}
                )
            );
            tuple_array.AS.Cached_WKT.Add(Core.MD_Well_Known_Type.Tuple_Array);
            return tuple_array;
        }

        public IMD_Relation MD_Relation(IMD_Heading heading)
        {
            if (heading == null)
            {
                throw new ArgumentNullException("heading");
            }
            return (IMD_Relation)new MD_Relation().init(m_machine,
                Core_MD_Relation(heading));
        }

        public IMD_Relation MD_Relation(IMD_Set body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }
            return (IMD_Relation)new MD_Relation().init(m_machine,
                Core_MD_Relation(body));
        }

        private Core.MD_Any Core_MD_Relation(IMD_Heading heading)
        {
            Core.MD_Any hv = ((MD_Heading)heading).m_value;
            Core.MD_Any bv = m_memory.MD_Capsule(
                m_memory.MD_Attr_Name("Set"),
                m_memory.MD_Tuple(
                    only_oa: new KeyValuePair<String,Core.MD_Any>("members",
                        m_memory.MD_Bag_C0)
                )
            );
            bv.AS.Cached_WKT.Add(Core.MD_Well_Known_Type.Set);
            Core.MD_Any relation = m_memory.MD_Capsule(
                m_memory.MD_Attr_Name("Relation"),
                m_memory.MD_Tuple(
                    multi_oa: new Dictionary<String,Core.MD_Any>()
                        {{"heading", hv}, {"body", bv}}
                )
            );
            relation.AS.Cached_WKT.Add(Core.MD_Well_Known_Type.Relation);
            return relation;
        }

        private Core.MD_Any Core_MD_Relation(IMD_Set body)
        {
            Core.MD_Any bv = ((MD_Set)body).m_value;
            if (!m_memory.Set__Is_Relational(bv))
            {
                throw new ArgumentException
                (
                    paramName: "body",
                    message: "Can't select MD_Relation from a MD_Set whose"
                        + " members aren't all MD_Tuple with a common heading."
                );
            }
            Core.MD_Any hv = m_memory.Set__Pick_Random_Member(bv);
            if (hv == null)
            {
                throw new ArgumentException
                (
                    paramName: "body",
                    message: "Can't select MD_Relation from empty MD_Set."
                );
            }
            Core.MD_Any relation = m_memory.MD_Capsule(
                m_memory.MD_Attr_Name("Relation"),
                m_memory.MD_Tuple(
                    multi_oa: new Dictionary<String,Core.MD_Any>()
                        {{"heading", hv}, {"body", bv}}
                )
            );
            relation.AS.Cached_WKT.Add(Core.MD_Well_Known_Type.Relation);
            return relation;
        }

        public IMD_Tuple_Bag MD_Tuple_Bag(IMD_Heading heading)
        {
            if (heading == null)
            {
                throw new ArgumentNullException("heading");
            }
            return (IMD_Tuple_Bag)new MD_Tuple_Bag().init(m_machine,
                Core_MD_Tuple_Bag(heading));
        }

        public IMD_Tuple_Bag MD_Tuple_Bag(IMD_Bag body)
        {
            if (body == null)
            {
                throw new ArgumentNullException("body");
            }
            return (IMD_Tuple_Bag)new MD_Tuple_Bag().init(m_machine,
                Core_MD_Tuple_Bag(body));
        }

        private Core.MD_Any Core_MD_Tuple_Bag(IMD_Heading heading)
        {
            Core.MD_Any hv = ((MD_Heading)heading).m_value;
            Core.MD_Any bv = m_memory.MD_Bag_C0;
            Core.MD_Any tuple_bag = m_memory.MD_Capsule(
                m_memory.MD_Attr_Name("Tuple_Bag"),
                m_memory.MD_Tuple(
                    multi_oa: new Dictionary<String,Core.MD_Any>()
                        {{"heading", hv}, {"body", bv}}
                )
            );
            tuple_bag.AS.Cached_WKT.Add(Core.MD_Well_Known_Type.Tuple_Bag);
            return tuple_bag;
        }

        private Core.MD_Any Core_MD_Tuple_Bag(IMD_Bag body)
        {
            Core.MD_Any bv = ((MD_Bag)body).m_value;
            if (!m_memory.Bag__Is_Relational(bv))
            {
                throw new ArgumentException
                (
                    paramName: "body",
                    message: "Can't select MD_Tuple_Bag from a MD_Bag whose"
                        + " members aren't all MD_Tuple with a common heading."
                );
            }
            Core.MD_Any hv = m_memory.Bag__Pick_Random_Member(bv);
            if (hv == null)
            {
                throw new ArgumentException
                (
                    paramName: "body",
                    message: "Can't select MD_Tuple_Bag from empty MD_Bag."
                );
            }
            Core.MD_Any tuple_bag = m_memory.MD_Capsule(
                m_memory.MD_Attr_Name("Tuple_Bag"),
                m_memory.MD_Tuple(
                    multi_oa: new Dictionary<String,Core.MD_Any>()
                        {{"heading", hv}, {"body", bv}}
                )
            );
            tuple_bag.AS.Cached_WKT.Add(Core.MD_Well_Known_Type.Tuple_Bag);
            return tuple_bag;
        }

        public IMD_Interval MD_Interval(IMD_Any min, IMD_Any max,
            Boolean excludes_min = false, Boolean excludes_max = false)
        {
            if (min == null)
            {
                throw new ArgumentNullException("min");
            }
            if (max == null)
            {
                throw new ArgumentNullException("max");
            }
            return (IMD_Interval)new MD_Interval().init(m_machine,
                Core_MD_Interval(((MD_Any)min).m_value,
                    ((MD_Any)max).m_value, excludes_min, excludes_max));
        }

        private Core.MD_Any Core_MD_Interval(Core.MD_Any min, Core.MD_Any max,
            Boolean excludes_min = false, Boolean excludes_max = false)
        {
            Core.MD_Any interval = m_memory.MD_Capsule(
                m_memory.MD_Attr_Name("Interval"),
                m_memory.MD_Tuple(
                    multi_oa: new Dictionary<String,Core.MD_Any>()
                    {
                        {"min", min},
                        {"max", max},
                        {"excludes_min", m_memory.MD_Boolean(excludes_min)},
                        {"excludes_max", m_memory.MD_Boolean(excludes_max)},
                    }
                )
            );
            interval.AS.Cached_WKT.Add(Core.MD_Well_Known_Type.Interval);
            return interval;
        }

        public IMD_Capsule MD_Capsule(IMD_Any label, IMD_Tuple attrs)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }
            if (attrs == null)
            {
                throw new ArgumentNullException("attrs");
            }
            return (IMD_Capsule)new MD_Capsule().init(m_machine,
                Core_MD_Capsule(label, attrs));
        }

        public IMD_Capsule MD_Capsule(String label, IMD_Tuple attrs)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }
            if (attrs == null)
            {
                throw new ArgumentNullException("attrs");
            }
            return (IMD_Capsule)new MD_Capsule().init(m_machine,
                Core_MD_Capsule(label, attrs));
        }

        public IMD_Capsule MD_Capsule(String[] label, IMD_Tuple attrs)
        {
            if (label == null)
            {
                throw new ArgumentNullException("label");
            }
            if (attrs == null)
            {
                throw new ArgumentNullException("attrs");
            }
            return (IMD_Capsule)new MD_Capsule().init(m_machine,
                Core_MD_Capsule(label, attrs));
        }

        private Core.MD_Any Core_MD_Capsule(IMD_Any label, IMD_Tuple attrs)
        {
            return m_memory.MD_Capsule(
                ((MD_Any)label).m_value, ((MD_Any)attrs).m_value);
        }

        private Core.MD_Any Core_MD_Capsule(String label, IMD_Tuple attrs)
        {
            return m_memory.MD_Capsule(
                m_memory.MD_Attr_Name(label), ((MD_Any)attrs).m_value);
        }

        private Core.MD_Any Core_MD_Capsule(String[] label, IMD_Tuple attrs)
        {
            return m_memory.MD_Capsule(
                m_memory.MD_Array(new List<Core.MD_Any>(label.Select(
                    m => m_memory.MD_Attr_Name(m)
                ))),
                ((MD_Any)attrs).m_value
            );
        }

        public IMD_Variable New_MD_Variable(IMD_Any initial_current_value)
        {
            if (initial_current_value == null)
            {
                throw new ArgumentNullException("initial_current_value");
            }
            return (IMD_Variable)new MD_Variable().init(m_machine,
                Core_MD_Variable(initial_current_value));
        }

        private Core.MD_Any Core_MD_Variable(IMD_Any initial_current_value)
        {
            return m_memory.New_MD_Variable(((MD_Any)initial_current_value).m_value);
        }

        public IMD_Process New_MD_Process()
        {
            return (IMD_Process)new MD_Process().init(m_machine, Core_MD_Process());
        }

        private Core.MD_Any Core_MD_Process()
        {
            return m_memory.New_MD_Process();
        }

        public IMD_Stream New_MD_Stream()
        {
            return (IMD_Stream)new MD_Stream().init(m_machine, Core_MD_Stream());
        }

        private Core.MD_Any Core_MD_Stream()
        {
            return m_memory.New_MD_Stream();
        }

        public IMD_External New_MD_External(Object value)
        {
            return (IMD_External)new MD_External().init(m_machine,
                Core_MD_External(value));
        }

        private Core.MD_Any Core_MD_External(Object value)
        {
            return m_memory.New_MD_External(value);
        }

        public IMD_Excuse MD_Excuse(IMD_Tuple attrs)
        {
            if (attrs == null)
            {
                throw new ArgumentNullException("attrs");
            }
            return (IMD_Excuse)new MD_Excuse().init(m_machine,
                Core_MD_Excuse(attrs));
        }

        public IMD_Excuse MD_Excuse(String value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value == "No_Reason")
            {
                return (IMD_Excuse_No_Reason)new MD_Excuse_No_Reason()
                    .init(m_machine, Core_MD_Excuse("No_Reason"));
            }
            return (IMD_Excuse)new MD_Excuse().init(m_machine,
                Core_MD_Excuse(value));
        }

        private Core.MD_Any Core_MD_Excuse(IMD_Tuple attrs)
        {
            return m_memory.MD_Excuse(((MD_Tuple)attrs).m_value);
        }

        private Core.MD_Any Core_MD_Excuse(String value)
        {
            return m_memory.Simple_MD_Excuse(value);
        }

        public IMD_Excuse_No_Reason MD_Excuse_No_Reason()
        {
            return (IMD_Excuse_No_Reason)new MD_Excuse_No_Reason()
                .init(m_machine, Core_MD_Excuse("No_Reason"));
        }
    }
}

namespace Muldis.D.Ref_Eng.Value
{
    public abstract class MD_Any : IMD_Any
    {
        internal Machine     m_machine;
        internal Core.MD_Any m_value;

        internal MD_Any init(Machine machine)
        {
            m_machine = machine;
            return this;
        }

        internal MD_Any init(Machine machine, Core.MD_Any value)
        {
            m_machine = machine;
            m_value   = value;
            return this;
        }

        public IMachine Machine()
        {
            return m_machine;
        }
    }

    public class MD_Boolean : MD_Any, IMD_Boolean
    {
        public Boolean Export_Boolean()
        {
            return m_value.AS.MD_Boolean;
        }
    }

    public class MD_Integer : MD_Any, IMD_Integer
    {
        public BigInteger Export_BigInteger()
        {
            return m_value.AS.MD_Integer;
        }

        public Int32 Export_Int32()
        {
            // This will throw an OverflowException if the value is too large.
            return (Int32)m_value.AS.MD_Integer;
        }
    }

    public class MD_Fraction : MD_Capsule, IMD_Fraction
    {
    }

    public class MD_String : MD_Array, IMD_String
    {
    }

    public class MD_Bits : MD_Capsule, IMD_Bits
    {
    }

    public class MD_Blob : MD_Capsule, IMD_Blob
    {
    }

    public class MD_Text : MD_Capsule, IMD_Text
    {
    }

    public class MD_Array : MD_Any, IMD_Array
    {
    }

    public class MD_Set : MD_Capsule, IMD_Set
    {
    }

    public class MD_Bag : MD_Any, IMD_Bag
    {
    }

    public class MD_Tuple : MD_Any, IMD_Tuple
    {
    }

    public class MD_Heading : MD_Tuple, IMD_Heading
    {
    }

    public class MD_Tuple_Array : MD_Capsule, IMD_Tuple_Array
    {
    }

    public class MD_Relation : MD_Capsule, IMD_Relation
    {
    }

    public class MD_Tuple_Bag : MD_Capsule, IMD_Tuple_Bag
    {
    }

    public class MD_Interval : MD_Capsule, IMD_Interval
    {
    }

    public class MD_Capsule : MD_Any, IMD_Capsule
    {
    }

    public abstract class MD_Handle : MD_Any, IMD_Handle
    {
    }

    public class MD_Variable : MD_Handle, IMD_Variable
    {
    }

    public class MD_Process : MD_Handle, IMD_Process
    {
    }

    public class MD_Stream : MD_Handle, IMD_Stream
    {
    }

    public class MD_External : MD_Handle, IMD_External
    {
    }

    public class MD_Excuse : MD_Capsule, IMD_Excuse
    {
    }

    public class MD_Excuse_No_Reason : MD_Excuse, IMD_Excuse_No_Reason
    {
    }
}
