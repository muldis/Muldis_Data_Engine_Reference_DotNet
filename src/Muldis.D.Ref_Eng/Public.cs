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
}
