using System;
using System.Collections.Generic;

[assembly: CLSCompliant(true)]

namespace Muldis.DatabaseProtocol
{
    public interface IMdbpInfo
    {
        Boolean ProvidesMuldisDatabaseProtocolInfo();

        IMdbpMachine MdbpWantMachineApi(Object requestedVersion);
    }

    public interface IMdbpMachine
    {
        IMdbpValue MdbpEvaluate(IMdbpValue function, IMdbpValue args = null);

        void MdbpPerform(IMdbpValue procedure, IMdbpValue args = null);

        IMdbpValue MdbpCurrent(IMdbpValue variable);

        void MdbpAssign(IMdbpValue variable, IMdbpValue value);

        IMdbpValue MdbpImport(Object value);

        IMdbpValue MdbpImportQualified(KeyValuePair<String,Object> value);

        Object MdbpExport(IMdbpValue value);

        KeyValuePair<String,Object> MdbpExportQualified(IMdbpValue value);
    }

    public interface IMdbpValue
    {
        IMdbpMachine MdbpMachine();
    }
}
