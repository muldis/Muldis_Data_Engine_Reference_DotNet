using System;
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
                case Core.MD_Foundation_Type.MD_Tuple:
                    return (IMD_Tuple)new MD_Tuple().init(m_machine, value);
                case Core.MD_Foundation_Type.MD_Capsule:
                    return (MD_Capsule)new MD_Capsule().init(m_machine, value);
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
                case "Tuple":
                    if (type_name.StartsWith("System.Collections.Generic.Dictionary`"))
                    {
                        return Core_MD_Tuple(attrs: (Dictionary<String,Object>)v);
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
                                m_memory.MD_Attr_Name(m_memory.Codepoint_Array((String)label)),
                                attrs_cv
                            );
                        }
                        if (label.GetType().FullName == "System.String[]")
                        {
                            return m_memory.MD_Capsule(
                                m_memory.MD_Array(new List<Core.MD_Any>(((String[])label).Select(
                                    m => m_memory.MD_Attr_Name(m_memory.Codepoint_Array(m))
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
                throw new NotImplementedException();
            }
            String type_name = value.GetType().FullName;
            if (type_name == "System.DBNull")
            {
                throw new NotImplementedException();
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
                    ? (Nullable<KeyValuePair<Core.Codepoint_Array,Core.MD_Any>>)null
                    : new KeyValuePair<Core.Codepoint_Array,Core.MD_Any>(
                        m_memory.Codepoint_Array(attr.Value.Key),
                        Core_MD_Any(attr.Value.Value)
                    ),
                multi_oa: attrs == null ? null
                    : new Dictionary<Core.Codepoint_Array,Core.MD_Any>(
                        attrs.ToDictionary(
                            m => m_memory.Codepoint_Array(m.Key),
                            m => Core_MD_Any(m.Value)
                        )
                    )
            );
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
                m_memory.MD_Attr_Name(m_memory.Codepoint_Array(label)),
                ((MD_Any)attrs).m_value
            );
        }

        private Core.MD_Any Core_MD_Capsule(String[] label, IMD_Tuple attrs)
        {
            return m_memory.MD_Capsule(
                m_memory.MD_Array(new List<Core.MD_Any>(label.Select(
                    m => m_memory.MD_Attr_Name(m_memory.Codepoint_Array(m))
                ))),
                ((MD_Any)attrs).m_value
            );
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

    public class MD_Tuple : MD_Any, IMD_Tuple
    {
    }

    public class MD_Capsule : MD_Any, IMD_Capsule
    {
    }
}
