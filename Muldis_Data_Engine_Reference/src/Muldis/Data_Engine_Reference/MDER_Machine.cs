using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Muldis.Data_Engine_Reference.Internal;

namespace Muldis.Data_Engine_Reference;

public class MDER_Machine
{
    internal Memory memory;
    internal Executor executor;

    public MDER_Machine()
    {
        this.memory = new Memory();
        this.executor = this.memory.Executor;
    }

    public MDER_V_Any MDER_evaluate(MDER_V_Any function, MDER_V_Any args = null)
    {
        if (function is null)
        {
            throw new ArgumentNullException("function");
        }
        return new MDER_V_Any(this,
            this.executor.Evaluates(function.memory_value, args is null ? null : args.memory_value));
    }

    public void MDER_perform(MDER_V_Any procedure, MDER_V_Any args = null)
    {
        if (procedure is null)
        {
            throw new ArgumentNullException("procedure");
        }
        this.executor.Performs(procedure.memory_value, args is null ? null : args.memory_value);
    }

    public MDER_V_Any MDER_current(MDER_V_Any variable)
    {
        if (variable is null)
        {
            throw new ArgumentNullException("variable");
        }
        return new MDER_V_Any(this, variable.memory_value.MD_Variable());
    }

    public void MDER_assign(MDER_V_Any variable, MDER_V_Any new_current)
    {
        if (variable is null)
        {
            throw new ArgumentNullException("variable");
        }
        if (new_current is null)
        {
            throw new ArgumentNullException("new_current");
        }
        variable.memory_value.Details = new_current.memory_value;
    }

    public MDER_V_Any MDER_import(Object value)
    {
        if (value is not null && value.GetType().FullName == "Muldis.Data_Engine_Reference.MDER_V_Any")
        {
            return (MDER_V_Any)value;
        }
        return new MDER_V_Any(this, import__tree(value));
    }

    public MDER_V_Any MDER_import_qualified(KeyValuePair<String,Object> value)
    {
        return new MDER_V_Any(this, import__tree_qualified(value));
    }

    private MD_Any import__tree(Object value)
    {
        if (value is not null)
        {
            String type_name = value.GetType().FullName;
            if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any")
            {
                return ((MDER_V_Any)value).memory_value;
            }
            if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
            {
                return import__tree_qualified((KeyValuePair<String,Object>)value);
            }
        }
        return import__tree_unqualified(value);
    }

    private MD_Any import__tree_qualified(KeyValuePair<String,Object> value)
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
                    return this.memory.MD_Boolean((Boolean)v);
                }
                break;
            case "Integer":
                if (type_name == "System.Int32")
                {
                    return this.memory.MD_Integer((Int32)v);
                }
                if (type_name == "System.Numerics.BigInteger")
                {
                    return this.memory.MD_Integer((BigInteger)v);
                }
                break;
            case "Fraction":
                if (type_name == "System.Decimal")
                {
                    return this.memory.MD_Fraction((Decimal)v);
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
                        return this.memory.MD_Fraction((Int32)numerator, (Int32)denominator);
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
                        return this.memory.MD_Fraction((BigInteger)numerator, (BigInteger)denominator);
                    }
                    if (numerator.GetType().FullName == "Muldis.Data_Engine_Reference.MDER_V_Any"
                        && ((Muldis.Data_Engine_Reference.MDER_V_Any)numerator).memory_value.WKBT == Well_Known_Base_Type.MD_Integer
                        && denominator.GetType().FullName == "Muldis.Data_Engine_Reference.MDER_V_Any"
                        && ((Muldis.Data_Engine_Reference.MDER_V_Any)denominator).memory_value.WKBT == Well_Known_Base_Type.MD_Integer)
                    {
                        if (((Muldis.Data_Engine_Reference.MDER_V_Any)denominator).memory_value.MD_Integer() == 0)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Fraction with a denominator of zero."
                            );
                        }
                        return this.memory.MD_Fraction(
                            ((Muldis.Data_Engine_Reference.MDER_V_Any)numerator  ).memory_value.MD_Integer(),
                            ((Muldis.Data_Engine_Reference.MDER_V_Any)denominator).memory_value.MD_Integer()
                        );
                    }
                }
                break;
            case "Bits":
                if (type_name == "System.Collections.BitArray")
                {
                    // BitArrays are mutable so clone argument to protect our internals.
                    return this.memory.MD_Bits(new BitArray((BitArray)v));
                }
                break;
            case "Blob":
                if (type_name == "System.Byte[]")
                {
                    // Arrays are mutable so clone argument to protect our internals.
                    return this.memory.MD_Blob(((Byte[])v).ToArray());
                }
                break;
            case "Text":
                if (type_name == "System.String")
                {
                    Dot_Net_String_Unicode_Test_Result tr = this.memory.Test_Dot_Net_String((String)v);
                    if (tr == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Text with a malformed .NET String."
                        );
                    }
                    return this.memory.MD_Text(
                        (String)v,
                        (tr == Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
                    );
                }
                break;
            case "Array":
                if (type_name.StartsWith("System.Collections.Generic.List`"))
                {
                    return this.memory.MD_Array(
                        new List<MD_Any>(((List<Object>)v).Select(
                            m => import__tree(m)
                        ))
                    );
                }
                break;
            case "Set":
                if (type_name.StartsWith("System.Collections.Generic.List`"))
                {
                    return this.memory.MD_Set(
                        new List<Multiplied_Member>(((List<Object>)v).Select(
                            m => new Multiplied_Member(import__tree(m))
                        ))
                    );
                }
                break;
            case "Bag":
                if (type_name.StartsWith("System.Collections.Generic.List`"))
                {
                    return this.memory.MD_Bag(
                        new List<Multiplied_Member>(((List<Object>)v).Select(
                            m => new Multiplied_Member(import__tree(m))
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
                    return this.memory.MD_Tuple(
                        new Dictionary<String,MD_Any>(attrs.ToDictionary(
                            a => a.Key, a => import__tree(a.Value)))
                    );
                }
                if (type_name.StartsWith("System.Collections.Generic.Dictionary`"))
                {
                    Dictionary<String,Object> attrs = (Dictionary<String,Object>)v;
                    foreach (String atnm in attrs.Keys)
                    {
                        if (this.memory.Test_Dot_Net_String(atnm)
                            == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Tuple with attribute"
                                    + " name that is a malformed .NET String."
                            );
                        }
                    }
                    return this.memory.MD_Tuple(
                        new Dictionary<String,MD_Any>(attrs.ToDictionary(
                            a => a.Key, a => import__tree(a.Value)))
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
                    return this.memory.MD_Tuple(
                        new Dictionary<String,MD_Any>(
                            attr_names.ToDictionary(a => a, a => this.memory.MD_True))
                    );
                }
                if (type_name.StartsWith("System.Collections.Generic.HashSet`"))
                {
                    HashSet<String> attr_names = (HashSet<String>)v;
                    foreach (String atnm in attr_names)
                    {
                        if (this.memory.Test_Dot_Net_String(atnm)
                            == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Heading with attribute"
                                    + " name that is a malformed .NET String."
                            );
                        }
                    }
                    return this.memory.MD_Tuple(
                        new Dictionary<String,MD_Any>(
                            attr_names.ToDictionary(a => a, a => this.memory.MD_True))
                    );
                }
                break;
            case "Tuple_Array":
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any"
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value.WKBT == Well_Known_Base_Type.MD_Tuple
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value.Member_Status_in_WKT(MD_Well_Known_Type.Heading) == true)
                {
                    MD_Any hv = ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value;
                    MD_Any bv = this.memory.MD_Array_C0;
                    return this.memory.MD_Tuple_Array(hv, bv);
                }
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any"
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value.WKBT == Well_Known_Base_Type.MD_Array)
                {
                    MD_Any bv = ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value;
                    if (!this.memory.Array__Is_Relational(bv))
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Tuple_Array from a MD_Array whose"
                                + " members aren't all MD_Tuple with a common heading."
                        );
                    }
                    MD_Any hv = this.memory.Array__Pick_Arbitrary_Member(bv);
                    if (hv is null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Tuple_Array from empty MD_Array."
                        );
                    }
                    return this.memory.MD_Tuple_Array(hv, bv);
                }
                break;
            case "Relation":
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any"
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value.WKBT == Well_Known_Base_Type.MD_Tuple
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value.Member_Status_in_WKT(MD_Well_Known_Type.Heading) == true)
                {
                    MD_Any hv = ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value;
                    MD_Any bv = this.memory.MD_Set_C0;
                    return this.memory.MD_Relation(hv, bv);
                }
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any"
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value.WKBT == Well_Known_Base_Type.MD_Set)
                {
                    MD_Any bv = ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value;
                    if (!this.memory.Set__Is_Relational(bv))
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Relation from a MD_Set whose"
                                + " members aren't all MD_Tuple with a common heading."
                        );
                    }
                    MD_Any hv = this.memory.Set__Pick_Arbitrary_Member(bv);
                    if (hv is null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Relation from empty MD_Set."
                        );
                    }
                    return this.memory.MD_Relation(hv, bv);
                }
                break;
            case "Tuple_Bag":
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any"
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value.WKBT == Well_Known_Base_Type.MD_Tuple
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value.Member_Status_in_WKT(MD_Well_Known_Type.Heading) == true)
                {
                    MD_Any hv = ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value;
                    MD_Any bv = this.memory.MD_Bag_C0;
                    return this.memory.MD_Tuple_Bag(hv, bv);
                }
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any"
                    && ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value.WKBT == Well_Known_Base_Type.MD_Bag)
                {
                    MD_Any bv = ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value;
                    if (!this.memory.Bag__Is_Relational(bv))
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Tuple_Bag from a MD_Bag whose"
                                + " members aren't all MD_Tuple with a common heading."
                        );
                    }
                    MD_Any hv = this.memory.Bag__Pick_Arbitrary_Member(bv);
                    if (hv is null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Tuple_Bag from empty MD_Bag."
                        );
                    }
                    return this.memory.MD_Tuple_Bag(hv, bv);
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
                    MD_Any attrs_cv = import__tree(attrs);
                    if (attrs_cv.WKBT != Well_Known_Base_Type.MD_Tuple)
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
                        if (this.memory.Test_Dot_Net_String((String)label)
                            == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Article with label"
                                    + " that is a malformed .NET String."
                            );
                        }
                        return this.memory.MD_Article(
                            this.memory.MD_Attr_Name((String)label),
                            attrs_cv
                        );
                    }
                    if (label.GetType().FullName == "System.String[]")
                    {
                        foreach (String s in ((String[])label))
                        {
                            if (this.memory.Test_Dot_Net_String(s)
                                == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                            {
                                throw new ArgumentException
                                (
                                    paramName: "value",
                                    message: "Can't select MD_Article with label"
                                        + " that includes a malformed .NET String."
                                );
                            }
                        }
                        return this.memory.MD_Article(
                            this.memory.MD_Array(new List<MD_Any>(((String[])label).Select(
                                m => this.memory.MD_Attr_Name(m)
                            ))),
                            attrs_cv
                        );
                    }
                    if (label.GetType().FullName == "Muldis.Data_Engine_Reference.MDER_V_Any")
                    {
                        return this.memory.MD_Article(import__tree(label), attrs_cv);
                    }
                }
                break;
            case "New_Variable":
                if (type_name == "Muldis.Data_Engine_Reference.MDER_V_Any")
                {
                    return this.memory.New_MD_Variable(
                        ((Muldis.Data_Engine_Reference.MDER_V_Any)v).memory_value);
                }
                break;
            case "New_Process":
                if (v is null)
                {
                    return this.memory.New_MD_Process();
                }
                break;
            case "New_Stream":
                if (v is null)
                {
                    return this.memory.New_MD_Stream();
                }
                break;
            case "New_External":
                return this.memory.New_MD_External(v);
            case "Excuse":
                if (v is null)
                {
                    return this.memory.Simple_MD_Excuse("No_Reason");
                }
                if (type_name == "System.DBNull")
                {
                    return this.memory.Simple_MD_Excuse("No_Reason");
                }
                if (type_name == "System.String")
                {
                    if (this.memory.Test_Dot_Net_String((String)v)
                        == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Excuse with label"
                                + " that is a malformed .NET String."
                        );
                    }
                    return this.memory.Simple_MD_Excuse((String)v);
                }
                throw new NotImplementedException();
            case "Attr_Name":
                if (type_name == "System.String")
                {
                    if (this.memory.Test_Dot_Net_String((String)v)
                        == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MD_Attr_Name with a malformed .NET String."
                        );
                    }
                    return this.memory.MD_Attr_Name((String)v);
                }
                break;
            case "Attr_Name_List":
                if (type_name == "System.String[]")
                {
                    foreach (String s in ((String[])v))
                    {
                        if (this.memory.Test_Dot_Net_String(s)
                            == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MD_Attr_Name_List with"
                                    + " member that is a malformed .NET String."
                            );
                        }
                    }
                    return this.memory.MD_Array(new List<MD_Any>(((String[])v).Select(
                        m => this.memory.MD_Attr_Name(m)
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

    private MD_Any import__tree_unqualified(Object value)
    {
        if (value is null)
        {
            return this.memory.Simple_MD_Excuse("No_Reason");
        }
        String type_name = value.GetType().FullName;
        if (type_name == "System.DBNull")
        {
            return this.memory.Simple_MD_Excuse("No_Reason");
        }
        if (type_name == "System.Boolean")
        {
            return this.memory.MD_Boolean((Boolean)value);
        }
        if (type_name == "System.Int32")
        {
            return this.memory.MD_Integer((Int32)value);
        }
        if (type_name == "System.Numerics.BigInteger")
        {
            return this.memory.MD_Integer((BigInteger)value);
        }
        if (type_name == "System.Decimal")
        {
            return this.memory.MD_Fraction((Decimal)value);
        }
        if (type_name == "System.Collections.BitArray")
        {
            // BitArrays are mutable so clone argument to protect our internals.
            return this.memory.MD_Bits(new BitArray((BitArray)value));
        }
        if (type_name == "System.Byte[]")
        {
            // Arrays are mutable so clone argument to protect our internals.
            return this.memory.MD_Blob(((Byte[])value).ToArray());
        }
        if (type_name == "System.String")
        {
            Dot_Net_String_Unicode_Test_Result tr = this.memory.Test_Dot_Net_String((String)value);
            if (tr == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
            {
                throw new ArgumentException
                (
                    paramName: "value",
                    message: "Can't select MD_Text with a malformed .NET String."
                );
            }
            return this.memory.MD_Text(
                (String)value,
                (tr == Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
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
        MD_Any v = value.memory_value;
        switch (v.WKBT)
        {
            case Well_Known_Base_Type.MD_Boolean:
                return v.MD_Boolean().Value;
            case Well_Known_Base_Type.MD_Integer:
                return v.MD_Integer();
            case Well_Known_Base_Type.MD_External:
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
        MD_Any v = value.memory_value;
        switch (v.WKBT)
        {
            case Well_Known_Base_Type.MD_Boolean:
                return new KeyValuePair<String,Object>("Boolean",
                    v.MD_Boolean().Value);
            case Well_Known_Base_Type.MD_Integer:
                return new KeyValuePair<String,Object>("Integer",
                    v.MD_Integer());
            case Well_Known_Base_Type.MD_Fraction:
                throw new NotImplementedException();
            case Well_Known_Base_Type.MD_Bits:
                throw new NotImplementedException();
            case Well_Known_Base_Type.MD_Blob:
                throw new NotImplementedException();
            case Well_Known_Base_Type.MD_Text:
                throw new NotImplementedException();
            case Well_Known_Base_Type.MD_Array:
                throw new NotImplementedException();
            case Well_Known_Base_Type.MD_Set:
                throw new NotImplementedException();
            case Well_Known_Base_Type.MD_Bag:
                throw new NotImplementedException();
            case Well_Known_Base_Type.MD_Tuple:
                throw new NotImplementedException();
            case Well_Known_Base_Type.MD_Article:
                throw new NotImplementedException();
            case Well_Known_Base_Type.MD_Variable:
                throw new NotImplementedException();
            case Well_Known_Base_Type.MD_Process:
                throw new NotImplementedException();
            case Well_Known_Base_Type.MD_Stream:
                throw new NotImplementedException();
            case Well_Known_Base_Type.MD_External:
                return new KeyValuePair<String,Object>("New_External",
                    v.MD_External());
            case Well_Known_Base_Type.MD_Excuse:
                throw new NotImplementedException();
            default:
                throw new NotImplementedException();
        }
    }
}
