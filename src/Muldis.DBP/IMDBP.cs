using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

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

        IMD_Any MD_Any(Object value);

        IMD_Boolean MD_Boolean(Boolean value);

        IMD_Integer MD_Integer(BigInteger value);

        IMD_Integer MD_Integer(Int32 value);

        IMD_Fraction MD_Fraction(IMD_Integer numerator, IMD_Integer denominator);

        IMD_Fraction MD_Fraction(BigInteger numerator, BigInteger denominator);

        IMD_Fraction MD_Fraction(Int32 numerator, Int32 denominator);

        IMD_Fraction MD_Fraction(Decimal value);

        IMD_String MD_String(BigInteger[] members);

        IMD_String MD_String(Int32[] members);

        IMD_Bits MD_Bits(BitArray members);

        IMD_Blob MD_Blob(Byte[] members);

        IMD_Text MD_Text(String members);

        IMD_Array MD_Array(List<Object> members);

        IMD_Set MD_Set(List<Object> members);

        IMD_Bag MD_Bag(List<Object> members);

        IMD_Tuple MD_Tuple(
            Object a0 = null, Object a1 = null, Object a2 = null,
            Nullable<KeyValuePair<String,Object>> attr = null,
            Dictionary<String,Object> attrs = null);

        IMD_Heading MD_Heading(
            Nullable<Boolean> a0 = null, Nullable<Boolean> a1 = null,
            Nullable<Boolean> a2 = null,
            String attr_name = null, HashSet<String> attr_names = null);

        IMD_Tuple_Array MD_Tuple_Array(IMD_Heading heading);

        IMD_Tuple_Array MD_Tuple_Array(IMD_Array body);

        IMD_Relation MD_Relation(IMD_Heading heading);

        IMD_Relation MD_Relation(IMD_Set body);

        IMD_Tuple_Bag MD_Tuple_Bag(IMD_Heading heading);

        IMD_Tuple_Bag MD_Tuple_Bag(IMD_Bag body);

        IMD_Capsule MD_Capsule(IMD_Any label, IMD_Tuple attrs);

        IMD_Capsule MD_Capsule(String label, IMD_Tuple attrs);

        IMD_Capsule MD_Capsule(String[] label, IMD_Tuple attrs);

        IMD_Excuse MD_Excuse(IMD_Tuple attrs);

        IMD_Excuse MD_Excuse(String value);

        IMD_Excuse_No_Reason MD_Excuse_No_Reason();
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

    public interface IMD_Fraction : IMD_Capsule
    {
    }

    public interface IMD_String : IMD_Array
    {
    }

    public interface IMD_Bits : IMD_Capsule
    {
    }

    public interface IMD_Blob : IMD_Capsule
    {
    }

    public interface IMD_Text : IMD_Capsule
    {
    }

    public interface IMD_Array : IMD_Any
    {
    }

    public interface IMD_Set : IMD_Capsule
    {
    }

    public interface IMD_Bag : IMD_Any
    {
    }

    public interface IMD_Tuple : IMD_Any
    {
    }

    public interface IMD_Heading : IMD_Tuple
    {
    }

    public interface IMD_Tuple_Array : IMD_Capsule
    {
    }

    public interface IMD_Relation : IMD_Capsule
    {
    }

    public interface IMD_Tuple_Bag : IMD_Capsule
    {
    }

    public interface IMD_Capsule : IMD_Any
    {
    }

    public interface IMD_Excuse : IMD_Capsule
    {
    }

    public interface IMD_Excuse_No_Reason : IMD_Excuse
    {
    }
}
