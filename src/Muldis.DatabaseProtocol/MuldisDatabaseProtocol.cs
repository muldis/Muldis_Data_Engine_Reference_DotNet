using System;
using System.Numerics;

[assembly: CLSCompliant(true)]

namespace Muldis.DatabaseProtocol
{
    public interface IInfo
    {
        Boolean Provides_Dot_Net_Muldis_DBP();

        IMachine Want_VM_API(Object requested_version);
    }

    public interface IMachine
    {
        IValue Evaluates(IValue function, IValue args = null);

        void Performs(IValue procedure, IValue args = null);

        IValue MD_Any(Object value);
    }

    public interface IValue
    {
        IMachine Machine();

        Boolean Export_Boolean();

        BigInteger Export_BigInteger();

        Int32 Export_Int32();

        IValue Current();

        void Assign(IValue value);

        Object Export_External_Object();
    }
}
