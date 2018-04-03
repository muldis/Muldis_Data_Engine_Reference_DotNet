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
        IExecutor Executor();

        IImporter Importer();
    }

    public interface IExecutor
    {
        IMachine Machine();

        IMD_Any Evaluates(IMD_Any function, IMD_Any args = null);

        void Performs(IMD_Any procedure, IMD_Any args = null);
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

        IMD_Bits MD_Bits(BitArray members);

        IMD_Blob MD_Blob(Byte[] members);

        IMD_Text MD_Text(String members);

        IMD_Array MD_Array(List<Object> members);

        IMD_Set MD_Set(List<Object> members);

        IMD_Bag MD_Bag(List<Object> members);

        IMD_Tuple MD_Tuple(
            Object a0 = null, Object a1 = null, Object a2 = null,
            Dictionary<String,Object> attrs = null);

        IMD_Heading MD_Heading(
            Nullable<Boolean> a0 = null, Nullable<Boolean> a1 = null,
            Nullable<Boolean> a2 = null, HashSet<String> attr_names = null);

        IMD_Tuple_Array MD_Tuple_Array(IMD_Heading heading);

        IMD_Tuple_Array MD_Tuple_Array(IMD_Array body);

        IMD_Relation MD_Relation(IMD_Heading heading);

        IMD_Relation MD_Relation(IMD_Set body);

        IMD_Tuple_Bag MD_Tuple_Bag(IMD_Heading heading);

        IMD_Tuple_Bag MD_Tuple_Bag(IMD_Bag body);

        IMD_Article MD_Article(IMD_Any label, IMD_Tuple attrs);

        IMD_Article MD_Article(String label, IMD_Tuple attrs);

        IMD_Article MD_Article(String[] label, IMD_Tuple attrs);

        IMD_Variable New_MD_Variable(IMD_Any initial_current_value);

        IMD_Process New_MD_Process();

        IMD_Stream New_MD_Stream();

        IMD_External New_MD_External(Object value);

        IMD_Excuse MD_Excuse(IMD_Tuple attrs);

        IMD_Excuse MD_Excuse(String value);

        IMD_No_Reason MD_No_Reason();

        IMD_Heading MD_Attr_Name(String label);

        IMD_Array MD_Attr_Name_List(String[] label);
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
