using System;
using System.Numerics;

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

        IMdbpValue MdbpImport(Object value);
    }

    public interface IMdbpValue
    {
        IMdbpMachine MdbpMachine();

        Boolean Export_Boolean();

        BigInteger Export_BigInteger();

        Int32 Export_Int32();

        IMdbpValue MdbpCurrent();

        void MdbpAssign(IMdbpValue value);

        Object Export_External_Object();
    }
}
