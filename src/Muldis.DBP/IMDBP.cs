using System;

[assembly: CLSCompliant(true)]

namespace Muldis.DBP
{
    public interface IInfo
    {
        Boolean Provides_Dot_Net_Muldis_DBP();

        IMachine Want_VM_API(Object requested_version);
    }

    public interface IMachine
    {
        IImporter Importer();
    }

    public interface IImporter
    {
        IMachine Machine();
    }

    public interface IMD_Any
    {
        IMachine Machine();
    }
}
