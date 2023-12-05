using System.Collections;
using System.Numerics;

namespace Muldis.Data_Engine_Reference;

public class MDER_Machine
{
    private readonly Internal_Memory memory;
    private readonly Executor executor;

    public MDER_Machine()
    {
        this.memory = new Internal_Memory();
        this.executor = this.memory.executor;
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
        return new MDER_V_Any(this, ((MDER_Variable)variable.memory_value).current_value);
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
        ((MDER_Variable)variable.memory_value).current_value = new_current.memory_value;
    }

    public MDER_V_Any MDER_import(Object value)
    {
        if (value is not null && String.Equals(value.GetType().FullName, "Muldis.Data_Engine_Reference.MDER_V_Any"))
        {
            return (MDER_V_Any)value;
        }
        return new MDER_V_Any(this, import__tree(value));
    }

    public MDER_V_Any MDER_import_qualified(KeyValuePair<String, Object> value)
    {
        return new MDER_V_Any(this, import__tree_qualified(value));
    }

    private MDER_Any import__tree(Object value)
    {
        if (value is not null)
        {
            String type_name = value.GetType().FullName;
            if (String.Equals(type_name, "Muldis.Data_Engine_Reference.MDER_V_Any"))
            {
                return ((MDER_V_Any)value).memory_value;
            }
            if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
            {
                return import__tree_qualified((KeyValuePair<String, Object>)value);
            }
        }
        return import__tree_unqualified(value);
    }

    private MDER_Any import__tree_qualified(KeyValuePair<String, Object> value)
    {
        Object v = value.Value;
        // Note that .NET guarantees the .Key is never null.
        if (v is null)
        {
            if (String.Equals(value.Key, "Ignorance"))
            {
                return this.memory.MDER_Ignorance();
            }
            throw new ArgumentNullException
            (
                paramName: "value",
                message: "Can't select MDER_Any with a KeyValuePair operand"
                    + " with a null Value property (except with [Ignorance] Key)."
            );
        }
        String type_name = v is null ? null : v.GetType().FullName;
        switch (value.Key)
        {
            case "Boolean":
                if (String.Equals(type_name, "System.Boolean"))
                {
                    return this.memory.MDER_Boolean((Boolean)v);
                }
                break;
            case "Integer":
                if (String.Equals(type_name, "System.Int32"))
                {
                    return this.memory.MDER_Integer((Int32)v);
                }
                if (String.Equals(type_name, "System.Numerics.BigInteger"))
                {
                    return this.memory.MDER_Integer((BigInteger)v);
                }
                break;
            case "Fraction":
                if (String.Equals(type_name, "System.Decimal"))
                {
                    return this.memory.MDER_Fraction((Decimal)v);
                }
                if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
                {
                    Object numerator   = ((KeyValuePair<Object, Object>)v).Key;
                    Object denominator = ((KeyValuePair<Object, Object>)v).Value;
                    // Note that .NET guarantees the .Key is never null.
                    if (denominator is null)
                    {
                        throw new ArgumentNullException
                        (
                            paramName: "value",
                            message: "Can't select MDER_Fraction with a null denominator."
                        );
                    }
                    if (String.Equals(numerator.GetType().FullName, "System.Int32")
                        && String.Equals(denominator.GetType().FullName, "System.Int32"))
                    {
                        if (((Int32)denominator) == 0)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MDER_Fraction with a denominator of zero."
                            );
                        }
                        return this.memory.MDER_Fraction((Int32)numerator, (Int32)denominator);
                    }
                    if (String.Equals(numerator.GetType().FullName, "System.Numerics.BigInteger")
                        && String.Equals(denominator.GetType().FullName, "System.Numerics.BigInteger"))
                    {
                        if (((BigInteger)denominator) == 0)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MDER_Fraction with a denominator of zero."
                            );
                        }
                        return this.memory.MDER_Fraction((BigInteger)numerator, (BigInteger)denominator);
                    }
                    if (String.Equals(numerator.GetType().FullName, "Muldis.Data_Engine_Reference.MDER_V_Any")
                        && ((MDER_V_Any)numerator).memory_value.WKBT == Internal_Well_Known_Base_Type.MDER_Integer
                        && String.Equals(denominator.GetType().FullName, "Muldis.Data_Engine_Reference.MDER_V_Any")
                        && ((MDER_V_Any)denominator).memory_value.WKBT == Internal_Well_Known_Base_Type.MDER_Integer)
                    {
                        if (((MDER_Integer)((MDER_V_Any)denominator).memory_value).as_BigInteger == 0)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MDER_Fraction with a denominator of zero."
                            );
                        }
                        return this.memory.MDER_Fraction(
                            ((MDER_Integer)((MDER_V_Any)numerator  ).memory_value).as_BigInteger,
                            ((MDER_Integer)((MDER_V_Any)denominator).memory_value).as_BigInteger
                        );
                    }
                }
                break;
            case "Bits":
                if (String.Equals(type_name, "System.Collections.BitArray"))
                {
                    // BitArrays are mutable so clone argument to protect our internals.
                    return this.memory.MDER_Bits(new BitArray((BitArray)v));
                }
                break;
            case "Blob":
                if (String.Equals(type_name, "System.Byte[]"))
                {
                    // Arrays are mutable so clone argument to protect our internals.
                    return this.memory.MDER_Blob(((Byte[])v).ToArray());
                }
                break;
            case "Text":
                if (String.Equals(type_name, "System.String"))
                {
                    Dot_Net_String_Unicode_Test_Result tr = this.memory.Test_Dot_Net_String((String)v);
                    if (tr == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MDER_Text with a malformed .NET String."
                        );
                    }
                    return this.memory.MDER_Text(
                        (String)v,
                        (tr == Dot_Net_String_Unicode_Test_Result.Valid_Has_Non_BMP)
                    );
                }
                break;
            case "Array":
                if (type_name.StartsWith("System.Collections.Generic.List`"))
                {
                    return this.memory.MDER_Array(
                        new List<MDER_Any>(((List<Object>)v).Select(
                            m => import__tree(m)
                        ))
                    );
                }
                break;
            case "Set":
                if (type_name.StartsWith("System.Collections.Generic.List`"))
                {
                    return this.memory.MDER_Set(
                        new List<Multiplied_Member>(((List<Object>)v).Select(
                            m => new Multiplied_Member(import__tree(m))
                        ))
                    );
                }
                break;
            case "Bag":
                if (type_name.StartsWith("System.Collections.Generic.List`"))
                {
                    return this.memory.MDER_Bag(
                        new List<Multiplied_Member>(((List<Object>)v).Select(
                            m => new Multiplied_Member(import__tree(m))
                        ))
                    );
                }
                break;
            case "Heading":
                if (String.Equals(type_name, "System.Object[]"))
                {
                    HashSet<String> attr_names = new HashSet<String>();
                    for (Int32 i = 0; i < ((Object[])v).Length; i++)
                    {
                        if (((Object[])v)[0] is not null && (Boolean)((Object[])v)[0])
                        {
                            attr_names.Add(Char.ConvertFromUtf32(i));
                        }
                    }
                    return this.memory.MDER_Heading(attr_names);
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
                                message: "Can't select MDER_Heading with attribute"
                                    + " name that is a malformed .NET String."
                            );
                        }
                    }
                    return this.memory.MDER_Heading(new HashSet<String>(attr_names));
                }
                break;
            case "Tuple":
                if (String.Equals(type_name, "System.Object[]"))
                {
                    Dictionary<String, Object> attrs = new Dictionary<String, Object>();
                    for (Int32 i = 0; i < ((Object[])v).Length; i++)
                    {
                        attrs.Add(Char.ConvertFromUtf32(i), ((Object[])v)[i]);
                    }
                    return this.memory.MDER_Tuple(
                        new Dictionary<String, MDER_Any>(attrs.ToDictionary(
                            a => a.Key, a => import__tree(a.Value)))
                    );
                }
                if (type_name.StartsWith("System.Collections.Generic.Dictionary`"))
                {
                    Dictionary<String, Object> attrs = (Dictionary<String, Object>)v;
                    foreach (String atnm in attrs.Keys)
                    {
                        if (this.memory.Test_Dot_Net_String(atnm)
                            == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MDER_Tuple with attribute"
                                    + " name that is a malformed .NET String."
                            );
                        }
                    }
                    return this.memory.MDER_Tuple(
                        new Dictionary<String, MDER_Any>(attrs.ToDictionary(
                            a => a.Key, a => import__tree(a.Value)))
                    );
                }
                break;
            case "Tuple_Array":
                if (String.Equals(type_name, "Muldis.Data_Engine_Reference.MDER_V_Any")
                    && ((MDER_V_Any)v).memory_value.WKBT == Internal_Well_Known_Base_Type.MDER_Heading)
                {
                    MDER_Heading hv = (MDER_Heading)((MDER_V_Any)v).memory_value;
                    MDER_Array bv = this.memory.MDER_Array_C0;
                    return this.memory.MDER_Tuple_Array(hv, bv);
                }
                if (String.Equals(type_name, "Muldis.Data_Engine_Reference.MDER_V_Any")
                    && ((MDER_V_Any)v).memory_value.WKBT == Internal_Well_Known_Base_Type.MDER_Array)
                {
                    MDER_Array bv = (MDER_Array)((MDER_V_Any)v).memory_value;
                    if (!this.memory.Array__Is_Relational(bv))
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MDER_Tuple_Array from a MDER_Array whose"
                                + " members aren't all MDER_Tuple with a common heading."
                        );
                    }
                    MDER_Heading hv = this.memory.Tuple__Heading((MDER_Tuple)this.memory.Array__Pick_Arbitrary_Member(bv));
                    if (hv is null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MDER_Tuple_Array from empty MDER_Array."
                        );
                    }
                    return this.memory.MDER_Tuple_Array(hv, bv);
                }
                break;
            case "Relation":
                if (String.Equals(type_name, "Muldis.Data_Engine_Reference.MDER_V_Any")
                    && ((MDER_V_Any)v).memory_value.WKBT == Internal_Well_Known_Base_Type.MDER_Heading)
                {
                    MDER_Heading hv = (MDER_Heading)((MDER_V_Any)v).memory_value;
                    MDER_Set bv = this.memory.MDER_Set_C0;
                    return this.memory.MDER_Relation(hv, bv);
                }
                if (String.Equals(type_name, "Muldis.Data_Engine_Reference.MDER_V_Any")
                    && ((MDER_V_Any)v).memory_value.WKBT == Internal_Well_Known_Base_Type.MDER_Set)
                {
                    MDER_Set bv = (MDER_Set)((MDER_V_Any)v).memory_value;
                    if (!this.memory.Set__Is_Relational(bv))
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MDER_Relation from a MDER_Set whose"
                                + " members aren't all MDER_Tuple with a common heading."
                        );
                    }
                    MDER_Heading hv = this.memory.Tuple__Heading((MDER_Tuple)this.memory.Set__Pick_Arbitrary_Member(bv));
                    if (hv is null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MDER_Relation from empty MDER_Set."
                        );
                    }
                    return this.memory.MDER_Relation(hv, bv);
                }
                break;
            case "Tuple_Bag":
                if (String.Equals(type_name, "Muldis.Data_Engine_Reference.MDER_V_Any")
                    && ((MDER_V_Any)v).memory_value.WKBT == Internal_Well_Known_Base_Type.MDER_Heading)
                {
                    MDER_Heading hv = (MDER_Heading)((MDER_V_Any)v).memory_value;
                    MDER_Bag bv = this.memory.MDER_Bag_C0;
                    return this.memory.MDER_Tuple_Bag(hv, bv);
                }
                if (String.Equals(type_name, "Muldis.Data_Engine_Reference.MDER_V_Any")
                    && ((MDER_V_Any)v).memory_value.WKBT == Internal_Well_Known_Base_Type.MDER_Bag)
                {
                    MDER_Bag bv = (MDER_Bag)((MDER_V_Any)v).memory_value;
                    if (!this.memory.Bag__Is_Relational(bv))
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MDER_Tuple_Bag from a MDER_Bag whose"
                                + " members aren't all MDER_Tuple with a common heading."
                        );
                    }
                    MDER_Heading hv = this.memory.Tuple__Heading((MDER_Tuple)this.memory.Bag__Pick_Arbitrary_Member(bv));
                    if (hv is null)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MDER_Tuple_Bag from empty MDER_Bag."
                        );
                    }
                    return this.memory.MDER_Tuple_Bag(hv, bv);
                }
                break;
            case "Article":
                if (type_name.StartsWith("System.Collections.Generic.KeyValuePair`"))
                {
                    Object label = ((KeyValuePair<Object, Object>)v).Key;
                    Object attrs = ((KeyValuePair<Object, Object>)v).Value;
                    // Note that .NET guarantees the .Key is never null.
                    if (attrs is null)
                    {
                        throw new ArgumentNullException
                        (
                            paramName: "value",
                            message: "Can't select MDER_Article with a null Article attrs."
                        );
                    }
                    MDER_Any attrs_cv = import__tree(attrs);
                    if (attrs_cv.WKBT != Internal_Well_Known_Base_Type.MDER_Tuple)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MDER_Article with an Article attrs"
                                + " that doesn't evaluate as a MDER_Tuple."
                        );
                    }
                    MDER_Tuple attrs_cv_as_MDER_Tuple = (MDER_Tuple)attrs_cv;
                    if (String.Equals(label.GetType().FullName, "System.String"))
                    {
                        if (this.memory.Test_Dot_Net_String((String)label)
                            == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MDER_Article with label"
                                    + " that is a malformed .NET String."
                            );
                        }
                        return this.memory.MDER_Article(
                            this.memory.MDER_Attr_Name((String)label),
                            attrs_cv_as_MDER_Tuple
                        );
                    }
                    if (String.Equals(label.GetType().FullName, "System.String[]"))
                    {
                        foreach (String s in ((String[])label))
                        {
                            if (this.memory.Test_Dot_Net_String(s)
                                == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                            {
                                throw new ArgumentException
                                (
                                    paramName: "value",
                                    message: "Can't select MDER_Article with label"
                                        + " that includes a malformed .NET String."
                                );
                            }
                        }
                        return this.memory.MDER_Article(
                            this.memory.MDER_Array(new List<MDER_Any>(((String[])label).Select(
                                m => this.memory.MDER_Attr_Name(m)
                            ))),
                            attrs_cv_as_MDER_Tuple
                        );
                    }
                    if (String.Equals(label.GetType().FullName, "Muldis.Data_Engine_Reference.MDER_V_Any"))
                    {
                        return this.memory.MDER_Article(import__tree(label), attrs_cv_as_MDER_Tuple);
                    }
                }
                break;
            case "Excuse":
                if (String.Equals(type_name, "System.String"))
                {
                    if (this.memory.Test_Dot_Net_String((String)v)
                        == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                    {
                        throw new ArgumentException
                        (
                            paramName: "value",
                            message: "Can't select MDER_Excuse with label"
                                + " that is a malformed .NET String."
                        );
                    }
                    return this.memory.Simple_MDER_Excuse((String)v);
                }
                throw new NotImplementedException();
            case "New_Variable":
                if (String.Equals(type_name, "Muldis.Data_Engine_Reference.MDER_V_Any"))
                {
                    return this.memory.New_MDER_Variable(
                        ((MDER_V_Any)v).memory_value);
                }
                break;
            case "New_Process":
                if (v is null)
                {
                    return this.memory.New_MDER_Process();
                }
                break;
            case "New_Stream":
                if (v is null)
                {
                    return this.memory.New_MDER_Stream();
                }
                break;
            case "New_External":
                return this.memory.New_MDER_External(v);
            case "Attr_Name_List":
                if (String.Equals(type_name, "System.String[]"))
                {
                    foreach (String s in ((String[])v))
                    {
                        if (this.memory.Test_Dot_Net_String(s)
                            == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
                        {
                            throw new ArgumentException
                            (
                                paramName: "value",
                                message: "Can't select MDER_Attr_Name_List with"
                                    + " member that is a malformed .NET String."
                            );
                        }
                    }
                    return this.memory.MDER_Array(new List<MDER_Any>(((String[])v).Select(
                        m => this.memory.MDER_Attr_Name(m)
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

    private MDER_Any import__tree_unqualified(Object value)
    {
        if (value is null)
        {
            return this.memory.MDER_Ignorance();
        }
        String type_name = value.GetType().FullName;
        if (String.Equals(type_name, "System.Boolean"))
        {
            return this.memory.MDER_Boolean((Boolean)value);
        }
        if (String.Equals(type_name, "System.Int32"))
        {
            return this.memory.MDER_Integer((Int32)value);
        }
        if (String.Equals(type_name, "System.Numerics.BigInteger"))
        {
            return this.memory.MDER_Integer((BigInteger)value);
        }
        if (String.Equals(type_name, "System.Decimal"))
        {
            return this.memory.MDER_Fraction((Decimal)value);
        }
        if (String.Equals(type_name, "System.Collections.BitArray"))
        {
            // BitArrays are mutable so clone argument to protect our internals.
            return this.memory.MDER_Bits(new BitArray((BitArray)value));
        }
        if (String.Equals(type_name, "System.Byte[]"))
        {
            // Arrays are mutable so clone argument to protect our internals.
            return this.memory.MDER_Blob(((Byte[])value).ToArray());
        }
        if (String.Equals(type_name, "System.String"))
        {
            Dot_Net_String_Unicode_Test_Result tr = this.memory.Test_Dot_Net_String((String)value);
            if (tr == Dot_Net_String_Unicode_Test_Result.Is_Malformed)
            {
                throw new ArgumentException
                (
                    paramName: "value",
                    message: "Can't select MDER_Text with a malformed .NET String."
                );
            }
            return this.memory.MDER_Text(
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
        MDER_Any v = value.memory_value;
        switch (v.WKBT)
        {
            case Internal_Well_Known_Base_Type.MDER_Ignorance:
                return null;
            case Internal_Well_Known_Base_Type.MDER_False:
                return false;
            case Internal_Well_Known_Base_Type.MDER_True:
                return true;
            case Internal_Well_Known_Base_Type.MDER_Integer:
                return ((MDER_Integer)v).as_BigInteger;
            case Internal_Well_Known_Base_Type.MDER_External:
                return ((MDER_External)v).external_value;
            default:
                return MDER_export_qualified(value);
        }
    }

    public KeyValuePair<String, Object> MDER_export_qualified(MDER_V_Any value)
    {
        if (value is null)
        {
            throw new ArgumentNullException("value");
        }
        MDER_Any v = value.memory_value;
        switch (v.WKBT)
        {
            case Internal_Well_Known_Base_Type.MDER_Ignorance:
                return new KeyValuePair<String, Object>("Ignorance",null);
            case Internal_Well_Known_Base_Type.MDER_False:
                return new KeyValuePair<String, Object>("Boolean",false);
            case Internal_Well_Known_Base_Type.MDER_True:
                return new KeyValuePair<String, Object>("Boolean",true);
            case Internal_Well_Known_Base_Type.MDER_Integer:
                return new KeyValuePair<String, Object>("Integer",
                    ((MDER_Integer)v).as_BigInteger);
            case Internal_Well_Known_Base_Type.MDER_Fraction:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Bits:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Blob:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Text:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Array:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Set:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Bag:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Heading:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Tuple:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Article:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Excuse:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Variable:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Process:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_Stream:
                throw new NotImplementedException();
            case Internal_Well_Known_Base_Type.MDER_External:
                return new KeyValuePair<String, Object>("New_External",
                    ((MDER_External)v).external_value);
            default:
                throw new NotImplementedException();
        }
    }
}
