using System;
using System.Collections;
using System.Collections.Generic;
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
        IMD_Any Evaluates(IMD_Any function, IMD_Any args = null);

        void Performs(IMD_Any procedure, IMD_Any args = null);

        IMD_Any MD_Any(Object value);
    }

    public interface IMD_Any
    {
        IMachine Machine();

        Boolean Export_Boolean();

        BigInteger Export_BigInteger();

        Int32 Export_Int32();

        IMD_Any Current();

        void Assign(IMD_Any value);

        Object Export_External_Object();
    }
}
