using System;
using System.Linq;
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
}
