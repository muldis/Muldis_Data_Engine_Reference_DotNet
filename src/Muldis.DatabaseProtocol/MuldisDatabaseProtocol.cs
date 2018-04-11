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
    }

    public interface IMD_Boolean : IMD_Any
    {
        Boolean Export_Boolean();
    }

    public interface IMD_Integer : IMD_Any
    {
        BigInteger Export_BigInteger();

        Int32 Export_Int32();
    }

    public interface IMD_Fraction : IMD_Any
    {
    }

    public interface IMD_Bits : IMD_Any
    {
    }

    public interface IMD_Blob : IMD_Any
    {
    }

    public interface IMD_Text : IMD_Any
    {
    }

    public interface IMD_Array : IMD_Any
    {
    }

    public interface IMD_Set : IMD_Any
    {
    }

    public interface IMD_Bag : IMD_Any
    {
    }

    public interface IMD_Tuple : IMD_Any
    {
    }

    public interface IMD_Heading : IMD_Any
    {
    }

    public interface IMD_Tuple_Array : IMD_Any
    {
    }

    public interface IMD_Relation : IMD_Any
    {
    }

    public interface IMD_Tuple_Bag : IMD_Any
    {
    }

    public interface IMD_Article : IMD_Any
    {
    }

    public interface IMD_Handle : IMD_Any
    {
    }

    public interface IMD_Variable : IMD_Handle
    {
        IMD_Any Current();

        void Assign(IMD_Any value);
    }

    public interface IMD_Process : IMD_Handle
    {
    }

    public interface IMD_Stream : IMD_Handle
    {
    }

    public interface IMD_External : IMD_Handle
    {
        Object Export_External_Object();
    }

    public interface IMD_Excuse : IMD_Any
    {
    }

    public interface IMD_No_Reason : IMD_Excuse
    {
    }
}
