using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Muldis.Data_Engine_Reference;

public class MDER_Machine
{
    internal Core.Memory   m_memory;
    internal Core.Executor m_executor;

    public MDER_Machine()
    {
        m_memory   = new Core.Memory();
        m_executor = m_memory.Executor;
    }

    public MDER_V_Any MDER_evaluate(MDER_V_Any function, MDER_V_Any args = null)
    {
        if (function is null)
        {
            throw new ArgumentNullException("function");
        }
        return new MDER_V_Any(this,
            m_executor.Evaluates(function.m_value, args is null ? null : args.m_value));
    }

    public void MDER_perform(MDER_V_Any procedure, MDER_V_Any args = null)
    {
        if (procedure is null)
        {
            throw new ArgumentNullException("procedure");
        }
        m_executor.Performs(procedure.m_value, args is null ? null : args.m_value);
    }

    public MDER_V_Any MDER_current(MDER_V_Any variable)
    {
        if (variable is null)
        {
            throw new ArgumentNullException("variable");
        }
        return new MDER_V_Any(this, variable.m_value.MD_Variable());
    }

    public void MDER_assign(MDER_V_Any variable, MDER_V_Any newCurrent)
    {
        if (variable is null)
        {
            throw new ArgumentNullException("variable");
        }
        if (newCurrent is null)
        {
            throw new ArgumentNullException("newCurrent");
        }
        variable.m_value.Details = newCurrent.m_value;
    }

    public MDER_V_Any MDER_import(Object value)
    {
        if (value is not null && value.GetType().FullName == "Muldis.Data_Engine_Reference.MDER_V_Any")
        {
            return (MDER_V_Any)value;
        }
        return new MDER_V_Any(this, Import__Tree(value));
    }

    public MDER_V_Any MDER_import_qualified(KeyValuePair<String,Object> value)
    {
        return new MDER_V_Any(this, Import__Tree_Qualified(value));
    }

    private Core.MD_Any Import__Tree(Object value)
    {
        if (value is not null)
        {
            String type_name = value.GetType().FullName;
            if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any")
            {
                return ((MDER_V_Any)value).m_value;
            }
            if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
            {
                return Import__Tree_Qualified((KeyValuePair<String,Object>)value);
            }
        }
        return Import__Tree_Unqualified(value);
    }

    private Core.MD_Any Import__Tree_Qualified(KeyValuePair<String,Object> value)
    {
        Object v = value.Value;
        // Note that .NET guarantees the .Key is never null.
        if (v is null && value.Key != "Excuse")
        {
            throw new ArgumentNullException
            (
                paramName: "value",
                message: "Can't select MD_Any with a KeyValuePair operand"
                    + " with a null Value property (except with [Excuse] Key)."
            );
        }
        String type_name = v is null ? null : v.GetType().FullName;
        switch (value.Key)
        {
            case "Boolean":
                if (type_name == "System.Boolean")
                {
                    return m_memory.MD_Boolean((Boolean)v);
                }
                break;
            case "Integer":
                if (type_name == "System.Int32")
                {
                    return m_memory.MD_Integer((Int32)v);
                }
                if (type_name == "System.Numerics.BigInteger")
                {
                    return m_memory.MD_Integer((BigInteger)v);
                }
                break;
            case "Fraction":
                if (type_name == "System.Decimal")
                {
                    return m_memory.MD_Fraction((Decimal)v);
                }
                if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
                {
                    Object numerator   = ((KeyValuePair<Object,Object>)v).Key;
                    Object denominator = ((KeyValuePair<Object,Object>)v).Value;
                    // Note that .NET guarantees the .Key is never null.
                    if (denominator is null)
                    {
                        throw new ArgumentNullException
                        (
                            paramName: "value",
                            message: "Can't select MD_Fraction with a null denominator."
                        );
                    }
                    if (numerator.GetType().FullName == "System.Int32"
                        && denominator.GetType().FullName == "System.Int32")
                    {
                        if (((Int32)denominator) == 0)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Fraction with a denominator of zero."
                            );
                        }
                        return m_memory.MD_Fraction((Int32)numerator, (Int32)denominator);
                    }
                    if (numerator.GetType().FullName == "System.Numerics.BigInteger"
                        && denominator.GetType().FullName == "System.Numerics.BigInteger")
                    {
                        if (((BigInteger)denominator) == 0)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Fraction with a denominator of zero."
                            );
                        }
                        return m_memory.MD_Fraction((BigInteger)numerator, (BigInteger)denominator);
                    }
                    if (numerator.GetType().FullName == "Muldis.Data_Engine_Reference.MDER_V_Any"
                        && ((Muldis.Data_Engine_Reference.MDER_V_Any)numerator).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Integer
                        && denominator.GetType().FullName == "Muldis.Data_Engine_Reference.MDER_V_Any"
                        && ((Muldis.Data_Engine_Reference.MDER_V_Any)denominator).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Integer)
                    {
                        if (((Muldis.Data_Engine_Reference.MDER_V_Any)denominator).m_value.MD_Integer() == 0)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Fraction with a denominator of zero."
                            );
                        }
                        return m_memory.MD_Fraction(
                            ((Muldis.Data_Engine_Reference.MDER_V_Any)numerator  ).m_value.MD_Integer(),
                            ((Muldis.Data_Engine_Reference.MDER_V_Any)denominator).m_value.MD_Integer()
                        );
                    }
                }
                break;
            case "Bits":
                if (type_name == "System.Collections.BitArray")
                {
                    // BitArrays are mutable so clone argument to protect our internals.
                    return m_memory.MD_Bits(new BitArray((BitArray)v));
                }
                break;
            case "Blob":
                if (type_name == "System.Byte[]")
                {
                    // Arrays are mutable so clone argument to protect our internals.
                    return m_memory.MD_Blob(((Byte[])v).ToArray());
                }
                break;
            case "Text":
                if (type_name == "System.String")
                {
                    Core.Dot_Net_String_Unicode_Test_Result tr = m_memory.Test_Dot_Net_String((String)v);
                    if (tr == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Text with a malformed .NET String."
                        );
                    }
                    return m_memory.MD_Text(
                        (String)v,
                        (tr == Core.Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
                    );
                }
                break;
            case "Array":
                if (type_name.StartsWith("System.Collections.Generic.List`"))
                {
                    return m_memory.MD_Array(
                        new List<Core.MD_Any>(((List<Object>)v).Select(
                            m => Import__Tree(m)
                        ))
                    );
                }
                break;
            case "Set":
                if (type_name.StartsWith("System.Collections.Generic.List`"))
                {
                    return m_memory.MD_Set(
                        new List<Core.Multiplied_Member>(((List<Object>)v).Select(
                            m => new Core.Multiplied_Member(Import__Tree(m))
                        ))
                    );
                }
                break;
            case "Bag":
                if (type_name.StartsWith("System.Collections.Generic.List`"))
                {
                    return m_memory.MD_Bag(
                        new List<Core.Multiplied_Member>(((List<Object>)v).Select(
                            m => new Core.Multiplied_Member(Import__Tree(m))
                        ))
                    );
                }
                break;
            case "Tuple":
                if (type_name == "System.Object[]")
                {
                    Dictionary<String,Object> attrs = new Dictionary<String,Object>();
                    for (Int32 i = 0; i < ((Object[])v).Length; i++)
                    {
                        attrs.Add(Char.ConvertFromUtf32(i), ((Object[])v)[i]);
                    }
                    return m_memory.MD_Tuple(
                        new Dictionary<String,Core.MD_Any>(attrs.ToDictionary(
                            a => a.Key, a => Import__Tree(a.Value)))
                    );
                }
                if (type_name.StartsWith("System.Collections.Generic.Dictionary`"))
                {
                    Dictionary<String,Object> attrs = (Dictionary<String,Object>)v;
                    foreach (String atnm in attrs.Keys)
                    {
                        if (m_memory.Test_Dot_Net_String(atnm)
                            == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Tuple with attribute"
                                    + " name that is a malformed .NET String."
                            );
                        }
                    }
                    return m_memory.MD_Tuple(
                        new Dictionary<String,Core.MD_Any>(attrs.ToDictionary(
                            a => a.Key, a => Import__Tree(a.Value)))
                    );
                }
                break;
            case "Heading":
                if (type_name == "System.Object[]")
                {
                    HashSet<String> attr_names = (HashSet<String>)v;
                    for (Int32 i = 0; i < ((Object[])v).Length; i++)
                    {
                        if (((Object[])v)[0] is not null && (Boolean)((Object[])v)[0])
                        {
                            attr_names.Add(Char.ConvertFromUtf32(i));
                        }
                    }
                    return m_memory.MD_Tuple(
                        new Dictionary<String,Core.MD_Any>(
                            attr_names.ToDictionary(a => a, a => m_memory.MD_True))
                    );
                }
                if (type_name.StartsWith("System.Collections.Generic.HashSet`"))
                {
                    HashSet<String> attr_names = (HashSet<String>)v;
                    foreach (String atnm in attr_names)
                    {
                        if (m_memory.Test_Dot_Net_String(atnm)
                            == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Heading with attribute"
                                    + " name that is a malformed .NET String."
                            );
                        }
                    }
                    return m_memory.MD_Tuple(
                        new Dictionary<String,Core.MD_Any>(
                            attr_names.ToDictionary(a => a, a => m_memory.MD_True))
                    );
                }
                break;
            case "Tuple_Array":
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any"
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Tuple
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value.Member_Status_in_WKT(Core.MD_Well_Known_Type.Heading) == true)
                {
                    Core.MD_Any hv = ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value;
                    Core.MD_Any bv = m_memory.MD_Array_C0;
                    return m_memory.MD_Tuple_Array(hv, bv);
                }
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any"
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Array)
                {
                    Core.MD_Any bv = ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value;
                    if (!m_memory.Array__Is_Relational(bv))
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Tuple_Array from a MD_Array whose"
                                + " members aren't all MD_Tuple with a common heading."
                        );
                    }
                    Core.MD_Any hv = m_memory.Array__Pick_Arbitrary_Member(bv);
                    if (hv is null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Tuple_Array from empty MD_Array."
                        );
                    }
                    return m_memory.MD_Tuple_Array(hv, bv);
                }
                break;
            case "Relation":
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any"
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Tuple
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value.Member_Status_in_WKT(Core.MD_Well_Known_Type.Heading) == true)
                {
                    Core.MD_Any hv = ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value;
                    Core.MD_Any bv = m_memory.MD_Set_C0;
                    return m_memory.MD_Relation(hv, bv);
                }
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any"
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Set)
                {
                    Core.MD_Any bv = ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value;
                    if (!m_memory.Set__Is_Relational(bv))
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Relation from a MD_Set whose"
                                + " members aren't all MD_Tuple with a common heading."
                        );
                    }
                    Core.MD_Any hv = m_memory.Set__Pick_Arbitrary_Member(bv);
                    if (hv is null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Relation from empty MD_Set."
                        );
                    }
                    return m_memory.MD_Relation(hv, bv);
                }
                break;
            case "Tuple_Bag":
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any"
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Tuple
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value.Member_Status_in_WKT(Core.MD_Well_Known_Type.Heading) == true)
                {
                    Core.MD_Any hv = ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value;
                    Core.MD_Any bv = m_memory.MD_Bag_C0;
                    return m_memory.MD_Tuple_Bag(hv, bv);
                }
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any"
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value.MD_MSBT == Core.MD_Well_Known_Base_Type.MD_Bag)
                {
                    Core.MD_Any bv = ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value;
                    if (!m_memory.Bag__Is_Relational(bv))
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Tuple_Bag from a MD_Bag whose"
                                + " members aren't all MD_Tuple with a common heading."
                        );
                    }
                    Core.MD_Any hv = m_memory.Bag__Pick_Arbitrary_Member(bv);
                    if (hv is null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Tuple_Bag from empty MD_Bag."
                        );
                    }
                    return m_memory.MD_Tuple_Bag(hv, bv);
                }
                break;
            case "Article":
                if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
                {
                    Object label = ((KeyValuePair<Object,Object>)v).Key;
                    Object attrs = ((KeyValuePair<Object,Object>)v).Value;
                    // Note that .NET guarantees the .Key is never null.
                    if (attrs is null)
                    {
                        throw new ArgumentNullException
                        (
                            paramName: "value",
                            message: "Can't select MD_Article with a null Article attrs."
                        );
                    }
                    Core.MD_Any attrs_cv = Import__Tree(attrs);
                    if (attrs_cv.MD_MSBT != Core.MD_Well_Known_Base_Type.MD_Tuple)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Article with an Article attrs"
                                + " that doesn't evaluate as a MD_Tuple."
                        );
                    }
                    if (label.GetType().FullName == "System.String")
                    {
                        if (m_memory.Test_Dot_Net_String((String)label)
                            == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Article with label"
                                    + " that is a malformed .NET String."
                            );
                        }
                        return m_memory.MD_Article(
                            m_memory.MD_Attr_Name((String)label),
                            attrs_cv
                        );
                    }
                    if (label.GetType().FullName == "System.String[]")
                    {
                        foreach (String s in ((String[])label))
                        {
                            if (m_memory.Test_Dot_Net_String(s)
                                == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                            {
                                throw new ArgumentException
                                (
                                    paramName: "value",
                                    message: "Can't select MD_Article with label"
                                        + " that includes a malformed .NET String."
                                );
                            }
                        }
                        return m_memory.MD_Article(
                            m_memory.MD_Array(new List<Core.MD_Any>(((String[])label).Select(
                                m => m_memory.MD_Attr_Name(m)
                            ))),
                            attrs_cv
                        );
                    }
                    if (label.GetType().FullName == "Muldis.Data_Engine_Reference.MDER_V_Any")
                    {
                        return m_memory.MD_Article(Import__Tree(label), attrs_cv);
                    }
                }
                break;
            case "New_Variable":
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any")
                {
                    return m_memory.New_MD_Variable(
                        ((Muldis.Data_Engine_Reference.MDER_V_Any)v).m_value);
                }
                break;
            case "New_Process":
                if (v is null)
                {
                    return m_memory.New_MD_Process();
                }
                break;
            case "New_Stream":
                if (v is null)
                {
                    return m_memory.New_MD_Stream();
                }
                break;
            case "New_External":
                return m_memory.New_MD_External(v);
            case "Excuse":
                if (v is null)
                {
                    return m_memory.Simple_MD_Excuse("No_Reason");
                }
                if (type_name == "System.DBNull")
                {
                    return m_memory.Simple_MD_Excuse("No_Reason");
                }
                if (type_name == "System.String")
                {
                    if (m_memory.Test_Dot_Net_String((String)v)
                        == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Excuse with label"
                                + " that is a malformed .NET String."
                        );
                    }
                    return m_memory.Simple_MD_Excuse((String)v);
                }
                throw new NotImplementedException();
            case "Attr_Name":
                if (type_name == "System.String")
                {
                    if (m_memory.Test_Dot_Net_String((String)v)
                        == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Attr_Name with a malformed .NET String."
                        );
                    }
                    return m_memory.MD_Attr_Name((String)v);
                }
                break;
            case "Attr_Name_List":
                if (type_name == "System.String[]")
                {
                    foreach (String s in ((String[])v))
                    {
                        if (m_memory.Test_Dot_Net_String(s)
                            == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Attr_Name_List with"
                                    + " member that is a malformed .NET String."
                            );
                        }
                    }
                    return m_memory.MD_Array(new List<Core.MD_Any>(((String[])v).Select(
                        m => m_memory.MD_Attr_Name(m)
                    )));
                }
                break;
            default:
                throw new NotImplementedException(
                    "Unhandled MDER value type ["+value.Key+"]+["+type_name+"].");
        }
        // Duplicated as Visual Studio says otherwise not all code paths return a value.
        throw new NotImplementedException(
            "Unhandled MDER value type ["+value.Key+"]+["+type_name+"].");
    }

    private Core.MD_Any Import__Tree_Unqualified(Object value)
    {
        if (value is null)
        {
            return m_memory.Simple_MD_Excuse("No_Reason");
        }
        String type_name = value.GetType().FullName;
        if (type_name == "System.DBNull")
        {
            return m_memory.Simple_MD_Excuse("No_Reason");
        }
        if (type_name == "System.Boolean")
        {
            return m_memory.MD_Boolean((Boolean)value);
        }
        if (type_name == "System.Int32")
        {
            return m_memory.MD_Integer((Int32)value);
        }
        if (type_name == "System.Numerics.BigInteger")
        {
            return m_memory.MD_Integer((BigInteger)value);
        }
        if (type_name == "System.Decimal")
        {
            return m_memory.MD_Fraction((Decimal)value);
        }
        if (type_name == "System.Collections.BitArray")
        {
            // BitArrays are mutable so clone argument to protect our internals.
            return m_memory.MD_Bits(new BitArray((BitArray)value));
        }
        if (type_name == "System.Byte[]")
        {
            // Arrays are mutable so clone argument to protect our internals.
            return m_memory.MD_Blob(((Byte[])value).ToArray());
        }
        if (type_name == "System.String")
        {
            Core.Dot_Net_String_Unicode_Test_Result tr = m_memory.Test_Dot_Net_String((String)value);
            if (tr == Core.Dot_Net_String_Unicode_Test_Result.Is_Malformed)
            {
                throw new ArgumentException
                (
                    paramName: "value",
                    message: "Can't select MD_Text with a malformed .NET String."
                );
            }
            return m_memory.MD_Text(
                (String)value,
                (tr == Core.Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
            );
        }
        throw new NotImplementedException("Unhandled MDER value type ["+type_name+"].");
    }

    public Object MDER_export(MDER_V_Any value)
    {
        if (value is null)
        {
            throw new ArgumentNullException("value");
        }
        Core.MD_Any v = value.m_value;
        switch (v.MD_MSBT)
        {
            case Core.MD_Well_Known_Base_Type.MD_Boolean:
                return v.MD_Boolean().Value;
            case Core.MD_Well_Known_Base_Type.MD_Integer:
                return v.MD_Integer();
            case Core.MD_Well_Known_Base_Type.MD_External:
                return v.MD_External();
            default:
                return MDER_export_qualified(value);
        }
    }

    public KeyValuePair<String,Object> MDER_export_qualified(MDER_V_Any value)
    {
        if (value is null)
        {
            throw new ArgumentNullException("value");
        }
        Core.MD_Any v = value.m_value;
        switch (v.MD_MSBT)
        {
            case Core.MD_Well_Known_Base_Type.MD_Boolean:
                return new KeyValuePair<String,Object>("Boolean",
                    v.MD_Boolean().Value);
            case Core.MD_Well_Known_Base_Type.MD_Integer:
                return new KeyValuePair<String,Object>("Integer",
                    v.MD_Integer());
            case Core.MD_Well_Known_Base_Type.MD_Fraction:
                throw new NotImplementedException();
            case Core.MD_Well_Known_Base_Type.MD_Bits:
                throw new NotImplementedException();
            case Core.MD_Well_Known_Base_Type.MD_Blob:
                throw new NotImplementedException();
            case Core.MD_Well_Known_Base_Type.MD_Text:
                throw new NotImplementedException();
            case Core.MD_Well_Known_Base_Type.MD_Array:
                throw new NotImplementedException();
            case Core.MD_Well_Known_Base_Type.MD_Set:
                throw new NotImplementedException();
            case Core.MD_Well_Known_Base_Type.MD_Bag:
                throw new NotImplementedException();
            case Core.MD_Well_Known_Base_Type.MD_Tuple:
                throw new NotImplementedException();
            case Core.MD_Well_Known_Base_Type.MD_Article:
                throw new NotImplementedException();
            case Core.MD_Well_Known_Base_Type.MD_Variable:
                throw new NotImplementedException();
            case Core.MD_Well_Known_Base_Type.MD_Process:
                throw new NotImplementedException();
            case Core.MD_Well_Known_Base_Type.MD_Stream:
                throw new NotImplementedException();
            case Core.MD_Well_Known_Base_Type.MD_External:
                return new KeyValuePair<String,Object>("New_External",
                    v.MD_External());
            case Core.MD_Well_Known_Base_Type.MD_Excuse:
                throw new NotImplementedException();
            default:
                throw new NotImplementedException();
        }
    }
}
