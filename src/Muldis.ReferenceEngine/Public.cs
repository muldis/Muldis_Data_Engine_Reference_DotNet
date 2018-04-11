using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Muldis.DatabaseProtocol;
using Muldis.ReferenceEngine;
using Muldis.ReferenceEngine.Value;

[assembly: CLSCompliant(true)]

namespace Muldis.ReferenceEngine
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
        internal Core.Memory   m_memory;
        internal Core.Executor m_executor;

        internal Machine init()
        {
            m_memory   = new Core.Memory();
            m_executor = m_memory.Executor;
            return this;
        }

        public IMD_Any Evaluates(IMD_Any function, IMD_Any args = null)
        {
            return (IMD_Any)new MD_Any().init(this,
                m_executor.Evaluates(((MD_Any)function).m_value, ((MD_Any)args).m_value));
        }

        public void Performs(IMD_Any procedure, IMD_Any args = null)
        {
            m_executor.Performs(((MD_Any)procedure).m_value, ((MD_Any)args).m_value);
        }

        public IMD_Any MD_Any(Object value)
        {
            if (value != null)
            {
                String type_name = value.GetType().FullName;
                if (type_name.StartsWith("Muldis.ReferenceEngine.Value."))
                {
                    return (IMD_Any)value;
                }
                if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
                {
                    return (IMD_Any)new MD_Any().init(this, Import_Qualified(
                        (KeyValuePair<String,Object>)value));
                }
            }
            return (IMD_Any)new MD_Any().init(this, Import_Unqualified(value));
        }

        private Core.MD_Any Core_MD_Any(Object value)
        {
            if (value != null)
            {
                String type_name = value.GetType().FullName;
                if (type_name.StartsWith("Muldis.ReferenceEngine.Value."))
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
            // Note that .NET guarantees the .Key is never null.
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
                        return m_memory.MD_Boolean((Boolean)v);
                    }
                    break;
                case "Integer":
                    if (type_name == "System.Int32")
                    {
                        return m_memory.MD_Integer((Int32)v);
                    }
                    if (type_name == "System.Numerics.BigInteger")
                    {
                        return m_memory.MD_Integer((BigInteger)v);
                    }
                    break;
                case "Fraction":
                    if (type_name == "System.Decimal")
                    {
                        return m_memory.MD_Fraction((Decimal)v);
                    }
                    if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
                    {
                        Object numerator   = ((KeyValuePair<Object,Object>)v).Key;
                        Object denominator = ((KeyValuePair<Object,Object>)v).Value;
                        // Note that .NET guarantees the .Key is never null.
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
                        if (numerator.GetType().FullName == "Muldis.ReferenceEngine.Value.MD_Any"
                            && ((Muldis.ReferenceEngine.Value.MD_Any)numerator).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Integer
                            && denominator.GetType().FullName == "Muldis.ReferenceEngine.Value.MD_Any"
                            && ((Muldis.ReferenceEngine.Value.MD_Any)denominator).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Integer)
                        {
                            return Core_MD_Fraction(
                                ((Muldis.ReferenceEngine.Value.MD_Any)numerator  ).m_value.MD_Integer(),
                                ((Muldis.ReferenceEngine.Value.MD_Any)denominator).m_value.MD_Integer()
                            );
                        }
                    }
                    break;
                case "Bits":
                    if (type_name == "System.Collections.BitArray")
                    {
                        // BitArrays are mutable so clone argument to protect our internals.
                        return m_memory.MD_Bits(new BitArray((BitArray)v));
                    }
                    break;
                case "Blob":
                    if (type_name == "System.Byte[]")
                    {
                        // Arrays are mutable so clone argument to protect our internals.
                        return m_memory.MD_Blob(((Byte[])v).ToArray());
                    }
                    break;
                case "Text":
                    if (type_name == "System.String")
                    {
                        Core.Dot_Net_String_Unicode_Test_Result tr = m_memory.Test_Dot_Net_String((String)v);
                        if (tr == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Text with a malformed .NET String."
                            );
                        }
                        return m_memory.MD_Text(
                            (String)v,
                            (tr == Core.Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
                        );
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
                    if (type_name == "System.Object[]")
                    {
                        return Core_MD_Tuple(
                            a0: ((Object[])v).Length >= 1 ? ((Object[])v)[0] : null,
                            a1: ((Object[])v).Length >= 2 ? ((Object[])v)[1] : null,
                            a2: ((Object[])v).Length >= 3 ? ((Object[])v)[2] : null,
                            attrs: ((Object[])v).Length >= 4 ? (Dictionary<String,Object>)(((Object[])v)[3]) : null
                        );
                    }
                    if (type_name.StartsWith("System.Collections.Generic.Dictionary`"))
                    {
                        return Core_MD_Tuple(attrs: (Dictionary<String,Object>)v);
                    }
                    break;
                case "Heading":
                    if (type_name == "System.Object[]")
                    {
                        return Core_MD_Heading(
                            a0: ((Object[])v).Length >= 1 ? (Nullable<Boolean>)((Object[])v)[0] : null,
                            a1: ((Object[])v).Length >= 2 ? (Nullable<Boolean>)((Object[])v)[1] : null,
                            a2: ((Object[])v).Length >= 3 ? (Nullable<Boolean>)((Object[])v)[2] : null,
                            attr_names: ((Object[])v).Length >= 4 ? (HashSet<String>)(((Object[])v)[3]) : null
                        );
                    }
                    if (type_name.StartsWith("System.Collections.Generic.HashSet`"))
                    {
                        return Core_MD_Heading(attr_names: (HashSet<String>)v);
                    }
                    break;
                case "Tuple_Array":
                    if (type_name == "Muldis.ReferenceEngine.Value.MD_Any"
                        && ((Muldis.ReferenceEngine.Value.MD_Any)v).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Tuple
                        && ((Muldis.ReferenceEngine.Value.MD_Any)v).m_value.Member_Status_in_WKT(Core.MD_Well_Known_Type.Heading) == true)
                    {
                        return Core_MD_Tuple_Array__Heading(heading: (Muldis.ReferenceEngine.Value.MD_Any)v);
                    }
                    if (type_name == "Muldis.ReferenceEngine.Value.MD_Any"
                        && ((Muldis.ReferenceEngine.Value.MD_Any)v).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Array)
                    {
                        return Core_MD_Tuple_Array__Body(body: (Muldis.ReferenceEngine.Value.MD_Any)v);
                    }
                    break;
                case "Relation":
                    if (type_name == "Muldis.ReferenceEngine.Value.MD_Any"
                        && ((Muldis.ReferenceEngine.Value.MD_Any)v).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Tuple
                        && ((Muldis.ReferenceEngine.Value.MD_Any)v).m_value.Member_Status_in_WKT(Core.MD_Well_Known_Type.Heading) == true)
                    {
                        return Core_MD_Relation__Heading(heading: (Muldis.ReferenceEngine.Value.MD_Any)v);
                    }
                    if (type_name == "Muldis.ReferenceEngine.Value.MD_Any"
                        && ((Muldis.ReferenceEngine.Value.MD_Any)v).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Set)
                    {
                        return Core_MD_Relation__Body(body: (Muldis.ReferenceEngine.Value.MD_Any)v);
                    }
                    break;
                case "Tuple_Bag":
                    if (type_name == "Muldis.ReferenceEngine.Value.MD_Any"
                        && ((Muldis.ReferenceEngine.Value.MD_Any)v).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Tuple
                        && ((Muldis.ReferenceEngine.Value.MD_Any)v).m_value.Member_Status_in_WKT(Core.MD_Well_Known_Type.Heading) == true)
                    {
                        return Core_MD_Tuple_Bag__Heading(heading: (Muldis.ReferenceEngine.Value.MD_Any)v);
                    }
                    if (type_name == "Muldis.ReferenceEngine.Value.MD_Any"
                        && ((Muldis.ReferenceEngine.Value.MD_Any)v).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Bag)
                    {
                        return Core_MD_Tuple_Bag__Body(body: (Muldis.ReferenceEngine.Value.MD_Any)v);
                    }
                    break;
                case "Article":
                    if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
                    {
                        Object label = ((KeyValuePair<Object,Object>)v).Key;
                        Object attrs = ((KeyValuePair<Object,Object>)v).Value;
                        // Note that .NET guarantees the .Key is never null.
                        if (attrs == null)
                        {
                            throw new ArgumentNullException
                            (
                                paramName: "value",
                                message: "Can't select MD_Article with a null Article attrs."
                            );
                        }
                        Core.MD_Any attrs_cv = Core_MD_Any(attrs);
                        if (attrs_cv.MD_MSBT != Core.MD_Well_Known_Base_Type.MD_Tuple)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Article with an Article attrs"
                                    + " that doesn't evaluate as a MD_Tuple."
                            );
                        }
                        if (label.GetType().FullName == "System.String")
                        {
                            if (m_memory.Test_Dot_Net_String((String)label)
                                == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                            {
                                throw new ArgumentException
                                (
                                    paramName: "value",
                                    message: "Can't select MD_Article with label"
                                        + " that is a malformed .NET String."
                                );
                            }
                            return m_memory.MD_Article(
                                m_memory.MD_Attr_Name((String)label),
                                attrs_cv
                            );
                        }
                        if (label.GetType().FullName == "System.String[]")
                        {
                            foreach (String s in ((String[])label))
                            {
                                if (m_memory.Test_Dot_Net_String(s)
                                    == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                                {
                                    throw new ArgumentException
                                    (
                                        paramName: "value",
                                        message: "Can't select MD_Article with label"
                                            + " that includes a malformed .NET String."
                                    );
                                }
                            }
                            return m_memory.MD_Article(
                                m_memory.MD_Array(new List<Core.MD_Any>(((String[])label).Select(
                                    m => m_memory.MD_Attr_Name(m)
                                ))),
                                attrs_cv
                            );
                        }
                        if (label.GetType().FullName.StartsWith("Muldis.ReferenceEngine.Value."))
                        {
                            return m_memory.MD_Article(Core_MD_Any(label), attrs_cv);
                        }
                    }
                    break;
                case "New_Variable":
                    if (type_name.StartsWith("Muldis.ReferenceEngine.Value."))
                    {
                        return m_memory.New_MD_Variable(
                            ((Muldis.ReferenceEngine.Value.MD_Any)v).m_value);
                    }
                    break;
                case "New_Process":
                    if (v == null)
                    {
                        return m_memory.New_MD_Process();
                    }
                    break;
                case "New_Stream":
                    if (v == null)
                    {
                        return m_memory.New_MD_Stream();
                    }
                    break;
                case "New_External":
                    return m_memory.New_MD_External(v);
                case "Excuse":
                    if (v == null)
                    {
                        return m_memory.Simple_MD_Excuse("No_Reason");
                    }
                    if (type_name == "System.DBNull")
                    {
                        return m_memory.Simple_MD_Excuse("No_Reason");
                    }
                    if (type_name == "System.String")
                    {
                        if (m_memory.Test_Dot_Net_String((String)v)
                            == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Excuse with label"
                                    + " that is a malformed .NET String."
                            );
                        }
                        return m_memory.Simple_MD_Excuse((String)v);
                    }
                    throw new NotImplementedException();
                case "Attr_Name":
                    if (type_name == "System.String")
                    {
                        if (m_memory.Test_Dot_Net_String((String)v)
                            == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Attr_Name with a malformed .NET String."
                            );
                        }
                        return m_memory.MD_Attr_Name((String)v);
                    }
                    break;
                case "Attr_Name_List":
                    if (type_name == "System.String[]")
                    {
                        foreach (String s in ((String[])v))
                        {
                            if (m_memory.Test_Dot_Net_String(s)
                                == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                            {
                                throw new ArgumentException
                                (
                                    paramName: "value",
                                    message: "Can't select MD_Attr_Name_List with"
                                        + " member that is a malformed .NET String."
                                );
                            }
                        }
                        return m_memory.MD_Array(new List<Core.MD_Any>(((String[])v).Select(
                            m => m_memory.MD_Attr_Name(m)
                        )));
                    }
                    break;
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
                return m_memory.Simple_MD_Excuse("No_Reason");
            }
            String type_name = value.GetType().FullName;
            if (type_name == "System.DBNull")
            {
                return m_memory.Simple_MD_Excuse("No_Reason");
            }
            if (type_name == "System.Boolean")
            {
                return m_memory.MD_Boolean((Boolean)value);
            }
            if (type_name == "System.Int32")
            {
                return m_memory.MD_Integer((Int32)value);
            }
            if (type_name == "System.Numerics.BigInteger")
            {
                return m_memory.MD_Integer((BigInteger)value);
            }
            if (type_name == "System.Decimal")
            {
                return m_memory.MD_Fraction((Decimal)value);
            }
            if (type_name == "System.Collections.BitArray")
            {
                // BitArrays are mutable so clone argument to protect our internals.
                return m_memory.MD_Bits(new BitArray((BitArray)value));
            }
            if (type_name == "System.Byte[]")
            {
                // Arrays are mutable so clone argument to protect our internals.
                return m_memory.MD_Blob(((Byte[])value).ToArray());
            }
            if (type_name == "System.String")
            {
                Core.Dot_Net_String_Unicode_Test_Result tr = m_memory.Test_Dot_Net_String((String)value);
                if (tr == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                {
                    throw new ArgumentException
                    (
                        paramName: "value",
                        message: "Can't select MD_Text with a malformed .NET String."
                    );
                }
                return m_memory.MD_Text(
                    (String)value,
                    (tr == Core.Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
                );
            }
            throw new NotImplementedException("Unhandled MDBP value type ["+type_name+"].");
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
            return m_memory.MD_Fraction(numerator, denominator);
        }

        private Core.MD_Any Core_MD_Array(List<Object> members)
        {
            return m_memory.MD_Array(
                new List<Core.MD_Any>(members.Select(
                    m => Core_MD_Any(m)
                ))
            );
        }

        private Core.MD_Any Core_MD_Set(List<Object> members)
        {
            return m_memory.MD_Set(
                new List<Core.Multiplied_Member>(members.Select(
                    m => new Core.Multiplied_Member(Core_MD_Any(m))
                ))
            );
        }

        private Core.MD_Any Core_MD_Bag(List<Object> members)
        {
            return m_memory.MD_Bag(
                new List<Core.Multiplied_Member>(members.Select(
                    m => new Core.Multiplied_Member(Core_MD_Any(m))
                ))
            );
        }

        private Core.MD_Any Core_MD_Tuple(
            Object a0 = null, Object a1 = null, Object a2 = null,
            Dictionary<String,Object> attrs = null)
        {
            if (attrs != null)
            {
                foreach (String atnm in attrs.Keys)
                {
                    if (m_memory.Test_Dot_Net_String(atnm)
                        == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                    {
                        throw new ArgumentException
                        (
                            paramName: "attrs",
                            message: "Can't select MD_Tuple with attribute"
                                + " name that is a malformed .NET String."
                        );
                    }
                }
            }
            // Dictionary are mutable so clone argument to protect caller.
            attrs = (attrs == null) ? new Dictionary<String,Object>()
                : new Dictionary<String,Object>(attrs);
            if (a0 != null)
            {
                if (attrs.ContainsKey("\u0000"))
                {
                    throw new ArgumentException
                    (
                        paramName: "attrs",
                        message: "Can't select MD_Tuple with same-named"
                            + " attribute in both [a0] and [attrs] args."
                    );
                }
                attrs.Add("\u0000", a0);
            }
            if (a1 != null)
            {
                if (attrs.ContainsKey("\u0001"))
                {
                    throw new ArgumentException
                    (
                        paramName: "attrs",
                        message: "Can't select MD_Tuple with same-named"
                            + " attribute in both [a1] and [attrs] args."
                    );
                }
                attrs.Add("\u0001", a1);
            }
            if (a2 != null)
            {
                if (attrs.ContainsKey("\u0002"))
                {
                    throw new ArgumentException
                    (
                        paramName: "attrs",
                        message: "Can't select MD_Tuple with same-named"
                            + " attribute in both [a2] and [attrs] args."
                    );
                }
                attrs.Add("\u0002", a2);
            }
            return m_memory.MD_Tuple(
                new Dictionary<String,Core.MD_Any>(attrs.ToDictionary(
                    a => a.Key, a => Core_MD_Any(a.Value)))
            );
        }

        private Core.MD_Any Core_MD_Heading(
            Nullable<Boolean> a0 = null, Nullable<Boolean> a1 = null,
            Nullable<Boolean> a2 = null, HashSet<String> attr_names = null)
        {
            return Core_MD_Tuple(
                a0: (a0 == null || a0 == false) ? (Object)null : true,
                a1: (a1 == null || a1 == false) ? (Object)null : true,
                a2: (a2 == null || a2 == false) ? (Object)null : true,
                attrs: attr_names == null ? null
                    : new Dictionary<String,Object>(
                        attr_names.ToDictionary(a => a, a => (Object)true))
            );
        }

        private Core.MD_Any Core_MD_Tuple_Array__Heading(IMD_Any heading)
        {
            Core.MD_Any hv = ((MD_Any)heading).m_value;
            Core.MD_Any bv = m_memory.MD_Array_C0;
            return m_memory.MD_Tuple_Array(hv, bv);
        }

        private Core.MD_Any Core_MD_Tuple_Array__Body(IMD_Any body)
        {
            Core.MD_Any bv = ((MD_Any)body).m_value;
            if (!m_memory.Array__Is_Relational(bv))
            {
                throw new ArgumentException
                (
                    paramName: "body",
                    message: "Can't select MD_Tuple_Array from a MD_Array whose"
                        + " members aren't all MD_Tuple with a common heading."
                );
            }
            Core.MD_Any hv = m_memory.Array__Pick_Arbitrary_Member(bv);
            if (hv == null)
            {
                throw new ArgumentException
                (
                    paramName: "body",
                    message: "Can't select MD_Tuple_Array from empty MD_Array."
                );
            }
            return m_memory.MD_Tuple_Array(hv, bv);
        }

        private Core.MD_Any Core_MD_Relation__Heading(IMD_Any heading)
        {
            Core.MD_Any hv = ((MD_Any)heading).m_value;
            Core.MD_Any bv = m_memory.MD_Set_C0;
            return m_memory.MD_Relation(hv, bv);
        }

        private Core.MD_Any Core_MD_Relation__Body(IMD_Any body)
        {
            Core.MD_Any bv = ((MD_Any)body).m_value;
            if (!m_memory.Set__Is_Relational(bv))
            {
                throw new ArgumentException
                (
                    paramName: "body",
                    message: "Can't select MD_Relation from a MD_Set whose"
                        + " members aren't all MD_Tuple with a common heading."
                );
            }
            Core.MD_Any hv = m_memory.Set__Pick_Arbitrary_Member(bv);
            if (hv == null)
            {
                throw new ArgumentException
                (
                    paramName: "body",
                    message: "Can't select MD_Relation from empty MD_Set."
                );
            }
            return m_memory.MD_Relation(hv, bv);
        }

        private Core.MD_Any Core_MD_Tuple_Bag__Heading(IMD_Any heading)
        {
            Core.MD_Any hv = ((MD_Any)heading).m_value;
            Core.MD_Any bv = m_memory.MD_Bag_C0;
            return m_memory.MD_Tuple_Bag(hv, bv);
        }

        private Core.MD_Any Core_MD_Tuple_Bag__Body(IMD_Any body)
        {
            Core.MD_Any bv = ((MD_Any)body).m_value;
            if (!m_memory.Bag__Is_Relational(bv))
            {
                throw new ArgumentException
                (
                    paramName: "body",
                    message: "Can't select MD_Tuple_Bag from a MD_Bag whose"
                        + " members aren't all MD_Tuple with a common heading."
                );
            }
            Core.MD_Any hv = m_memory.Bag__Pick_Arbitrary_Member(bv);
            if (hv == null)
            {
                throw new ArgumentException
                (
                    paramName: "body",
                    message: "Can't select MD_Tuple_Bag from empty MD_Bag."
                );
            }
            return m_memory.MD_Tuple_Bag(hv, bv);
        }
    }
}

namespace Muldis.ReferenceEngine.Value
{
    public class MD_Any : IMD_Any
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

        public override String ToString()
        {
            return m_value.ToString();
        }

        public IMachine Machine()
        {
            return m_machine;
        }

        public Boolean Export_Boolean()
        {
            return m_value.MD_Boolean().Value;
        }

        public BigInteger Export_BigInteger()
        {
            return m_value.MD_Integer();
        }

        public Int32 Export_Int32()
        {
            // This will throw an OverflowException if the value is too large.
            return (Int32)m_value.MD_Integer();
        }

        public IMD_Any Current()
        {
            return (IMD_Any)new MD_Any().init(m_machine, m_value.MD_Variable());
        }

        public void Assign(IMD_Any value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            m_value.Details = ((MD_Any)value).m_value;
        }

        public Object Export_External_Object()
        {
            return m_value.MD_External();
        }
    }
}
