using System.Text;
using System.Text.RegularExpressions;

namespace Muldis.Data_Engine_Reference;

// Muldis.Data_Engine_Reference.MDER_Any
// Represents a Muldis Data Language "value", which is an individual constant that
// is not fixed in time or space.  Every Muldis Data Language value is unique,
// eternal, and immutable; it has no address and can not be updated.
// Several distinct MDER_Any objects may denote the same Muldis Data Language
// "value"; however, any time that two MDER_Any are discovered to
// denote the same Muldis Data Language value, any references to one may be safely
// replaced by references to the other.
// A design objective is implementing the "flyweight pattern",
// where MDER_Any objects share components directly or indirectly as much
// as is safely possible.  For example, if we detect duplication at
// runtime, we may replace references to one
// MDER_Any with another where the underlying virtual machine or
// garbage collector doesn't natively provide that ability.
// Similarly, proving equality of two "value" can often short-circuit.
// While MDER_Any are immutable from a user's perspective, their
// components may in fact mutate for memory sharing or consolidating.
// Iff a Muldis Data Language "value" is a "Handle" then it references something
// that possibly can mutate, such as a Muldis Data Language "variable".

public abstract class MDER_Any
{
    // MDER_Machine this Muldis Data Language "value" lives in.
    private readonly MDER_Machine __machine;

    // Muldis Data Language most specific well known base data type (__WKBT) this
    // "value" is a member of.  Helps interpret which subtype of MDER_Any
    // we have without having to use the language's type reflection system.
    // Some of these types have their own subset of specialized
    // representation formats for the sake of optimization.
    private readonly Internal_Well_Known_Base_Type __WKBT;

    // Normalized serialization of the Muldis Data Language "value" that its host
    // MDER_Any represents.  This is calculated lazily if needed,
    // typically when the "value" is a member of an indexed collection.
    // The serialization format either is or resembles a Muldis Object Notation Plain Text
    // literal for selecting the value, in the form of character strings
    // whose character code points are typically in the 0..127 range.
    private String? __cached_MDER_Any_identity;

    internal MDER_Any(MDER_Machine machine,
        Internal_Well_Known_Base_Type WKBT)
    {
        this.__machine = machine;
        this.__WKBT = WKBT;
    }

    public override String ToString()
    {
        return this._preview_as_String();
    }

    public MDER_Machine machine()
    {
        return this.__machine;
    }

    internal Internal_Well_Known_Base_Type _WKBT()
    {
        return this.__WKBT;
    }

    // Provides utility pure functions that accept any Muldis Data Language "value"
    // and derive a .NET String that uniquely identifies it.
    // This class is deterministic and guarantees that iff 2 MDER_Any are
    // logically considered to be the "same" Muldis Data Language value then they will
    // map to exactly the same .NET String value, and moreover, that iff 2
    // MDER_Any are logically considered to NOT be the "same" Muldis Data Language value,
    // they are guaranteed to map to distinct .NET String values.
    // The outputs of this generator are intended for internal use only,
    // where the outputs are transient and only intended to be used within
    // the same in-memory Muldis.Data_Engine_Reference VM instance that generated them.
    // The intended use of this class is to produce normalized identity
    // values for .NET collection indexes, Dictionary keys for example, or
    // otherwise support the means of primary/last resort for set-like
    // operations like duplicate elimination, relational joins, and more.
    // The outputs of this class may possibly conform to the
    // Muldis_Object_Notation_Plain_Text specification and be parseable to yield the
    // original input values, but that is not guaranteed; even if that is
    // the case, the outputs might be considerably less "pretty" as a
    // trade-off to make the generating faster and less error-prone.
    // A normal side effect of using Internal_Identity_Generator on a MDER_Any/etc
    // value is to update a cache therein to hold the serialization result.

    internal String _identity_as_String()
    {
        if (this.__cached_MDER_Any_identity is null)
        {
            this.__cached_MDER_Any_identity = this._as_MUON_Any_artifact("");
        }
        return this.__cached_MDER_Any_identity;
    }

    // Provides utility pure functions that accept any Muldis Data Language "value"
    // and derive a .NET String that provides a "preview quick look"
    // serialization of that value.  The intended use of this class is to
    // underlie a .NET ToString() override for all .NET values representing
    // Muldis Data Language values so that debuggers including MS Visual Studio can
    // assist programmers at easily determining what the current logical
    // values of their program variables are, which would otherwise be
    // quite labour intensive for humans inspecting MDER_Any object fields.
    // This class is explicitly NOT guaranteed to be deterministic and so
    // different runs of its functions on what are logically the same
    // Muldis Data Language values might produce different String outputs; reasons for
    // that include not sorting collection elements and truncating results.
    // Given its purpose in aiding debugging, it is intended that this
    // class will NOT populate any calculation caching fields in MDER_Any/etc
    // objects, to help avoid heisenbugs.

    internal String _preview_as_String()
    {
        return this._as_MUON_Any_artifact("");
    }

    // TODO: Some of this text has obsolete notions and it should be rewritten.
    // Provides utility pure functions that accept any Muldis Data Language "Package"
    // value, which is a native Muldis Data Language "standard compilation unit", and
    // derive a "Text" value that is this "Package" encoded in compliance
    // with the "Muldis_Object_Notation_Plain_Text 'https://muldis.com' '0.300.0'"
    // formal specification.  This outputs of this generator are intended
    // for external use, whether for storage in foo.mdpt disk files or
    // other places, viewing by users, and reading by other programs, as a
    // means of interchange with any other system conforming to the spec.
    // This generator actually handles any Muldis Data Language "value", not just a "Package".
    // This class is completely deterministic and its exact output Muldis Data Language
    // Text/etc values are determined entirely by its input Package/etc values.
    // This generator is expressly kept as simple as possible such that it
    // has zero configuration options and only does simple pretty-printing;
    // it is only meant so that the Muldis_D_Foundation has a competent
    // serialization capability built-in whose result is easy enough to
    // read and that is easy to diff changes for.
    // This generator makes only the "foundational" flavor of
    // Muldis_Object_Notation_Plain_Text and provides a literal serialization, including
    // that all annotation and decoration values are output verbatim, with
    // no interpretation and with nothing left out.  Round-tripping will
    // only work as intended with a parser that doesn't produce decorations.
    // Typically it is up to bootstrapped higher-level libraries written in
    // Muldis Data Language to provide other generating options that are better
    // pretty-printed or provide any configuration or that serialize with
    // non-foundational Muldis_Object_Notation_Plain_Text or interpret decorations.
    // While this generator's output includes a "parsing unit predicate" on
    // request (its optionality is the sole configuration option), that
    // does not include any preceeding "shebang line", which is up to the
    // caller where such is desired.

    public MDER_Text as_MUON_Plain_Text_artifact_as_MDER_Text()
    {
        String artifact_as_String = this._as_MUON_Any_artifact("");
        Internal_Unicode_Test_Result test_result
            = Internal_Unicode.test_String(artifact_as_String);
        return this.__machine._MDER_Text(
            "(Muldis_Object_Notation_Syntax:([Plain_Text,"
            + " \"https://muldis.com\", \"0.300.0\"]:\u000A"
            + artifact_as_String + "))\u000A",
            test_result == Internal_Unicode_Test_Result.Valid_Has_Non_BMP
        );
    }

    public MDER_Blob as_MUON_Plain_Text_artifact_as_MDER_Blob()
    {
        throw new NotImplementedException();
    }

    internal String _as_MUON_Any_artifact(String indent)
    {
        switch (this.__WKBT)
        {
            case Internal_Well_Known_Base_Type.MDER_Ignorance:
                return "0iIGNORANCE";
            case Internal_Well_Known_Base_Type.MDER_False:
                return "0bFALSE";
            case Internal_Well_Known_Base_Type.MDER_True:
                return "0bTRUE";
            case Internal_Well_Known_Base_Type.MDER_Integer:
                return ((MDER_Integer)this)._as_MUON_Integer_artifact();
            case Internal_Well_Known_Base_Type.MDER_Fraction:
                return ((MDER_Fraction)this)._as_MUON_Fraction_artifact();
            case Internal_Well_Known_Base_Type.MDER_Bits:
                return ((MDER_Bits)this)._as_MUON_Bits_artifact();
            case Internal_Well_Known_Base_Type.MDER_Blob:
                return ((MDER_Blob)this)._as_MUON_Blob_artifact();
            case Internal_Well_Known_Base_Type.MDER_Text:
                return ((MDER_Text)this)._as_MUON_Text_artifact();
            case Internal_Well_Known_Base_Type.MDER_Array:
                return ((MDER_Array)this)._as_MUON_Array_artifact(indent);
            case Internal_Well_Known_Base_Type.MDER_Set:
                return ((MDER_Set)this)._as_MUON_Set_artifact(indent);
            case Internal_Well_Known_Base_Type.MDER_Bag:
                return ((MDER_Bag)this)._as_MUON_Bag_artifact(indent);
            case Internal_Well_Known_Base_Type.MDER_Heading:
                return ((MDER_Heading)this)._as_MUON_Heading_artifact();
            case Internal_Well_Known_Base_Type.MDER_Tuple:
                return ((MDER_Tuple)this)._as_MUON_Tuple_artifact(indent);
            case Internal_Well_Known_Base_Type.MDER_Tuple_Array:
                return ((MDER_Tuple_Array)this)._as_MUON_Tuple_Array_artifact(indent);
            case Internal_Well_Known_Base_Type.MDER_Relation:
                return ((MDER_Relation)this)._as_MUON_Relation_artifact(indent);
            case Internal_Well_Known_Base_Type.MDER_Tuple_Bag:
                return ((MDER_Tuple_Bag)this)._as_MUON_Tuple_Bag_artifact(indent);
            case Internal_Well_Known_Base_Type.MDER_Article:
                return ((MDER_Article)this)._as_MUON_Article_artifact(indent);
            case Internal_Well_Known_Base_Type.MDER_Excuse:
                return ((MDER_Excuse)this)._as_MUON_Excuse_artifact(indent);
            case Internal_Well_Known_Base_Type.MDER_Variable:
                // We display something useful for debugging purposes, but no
                // (transient) MDER_Variable can actually be rendered as MUON.
                return "`Some MDER_Variable value is here.`";
            case Internal_Well_Known_Base_Type.MDER_Process:
                // We display something useful for debugging purposes, but no
                // (transient) MDER_Process can actually be rendered as MUON.
                return "`Some MDER_Process value is here.`";
            case Internal_Well_Known_Base_Type.MDER_Stream:
                // We display something useful for debugging purposes, but no
                // (transient) MDER_Stream can actually be rendered as MUON.
                return "`Some MDER_Stream value is here.`";
            case Internal_Well_Known_Base_Type.MDER_External:
                // We display something useful for debugging purposes, but no
                // (transient) MDER_External can actually be rendered as MUON.
                return "`Some MDER_External value is here.`";
            default:
                return "\\TODO_FIX_UN_HANDLED_TYPE\\\"" + this._WKBT().ToString() + "\"";
        }
    }

    // The following 2 methods might have been in MDER_Text but are in
    // MDER_Any instead temporarily because other value classes use them too.

    internal String _from_String_as_MUON_Text_artifact(String topic)
    {
        if (String.Equals(topic, ""))
        {
            return "\"\"";
        }
        if (topic.Length == 1 && ((Int32)(Char)topic[0]) <= 0x1F)
        {
            // Format as a code-point-text.
            return "0t" + ((Int32)(Char)topic[0]);
        }
        if (Regex.IsMatch(topic, @"\A[A-Za-z_][A-Za-z_0-9]*\z"))
        {
            // Format as a nonquoted-alphanumeric-text.
            return topic;
        }
        // Else, format as a quoted text.
        if (String.Equals(topic, "") || !Regex.IsMatch(topic,
            "[\u0000-\u001F\"\\`\u007F-\u009F]"))
        {
            return "\"" + topic + "\"";
        }
        StringBuilder sb = new StringBuilder(topic.Length);
        for (Int32 i = 0; i < topic.Length; i++)
        {
            Int32 c = (Int32)(Char)topic[i];
            switch (c)
            {
                case 0x7:
                    sb.Append(@"\a");
                    break;
                case 0x8:
                    sb.Append(@"\b");
                    break;
                case 0x9:
                    sb.Append(@"\t");
                    break;
                case 0xA:
                    sb.Append(@"\n");
                    break;
                case 0xB:
                    sb.Append(@"\v");
                    break;
                case 0xC:
                    sb.Append(@"\f");
                    break;
                case 0xD:
                    sb.Append(@"\r");
                    break;
                case 0x1B:
                    sb.Append(@"\e");
                    break;
                case 0x22:
                    sb.Append(@"\q");
                    break;
                case 0x5C:
                    sb.Append(@"\k");
                    break;
                case 0x60:
                    sb.Append(@"\g");
                    break;
                default:
                    if (c <= 0x1F || (c >= 0x7F && c <= 0x9F))
                    {
                        sb.Append(@"\(0t" + c.ToString() + ")");
                    }
                    else
                    {
                        sb.Append((Char)c);
                    }
                    break;
            }
        }
        return "\"" + sb.ToString() + "\"";
    }

    internal Boolean _MDER_Any__same(MDER_Any topic_1)
    {
        MDER_Any topic_0 = this;
        if (Object.ReferenceEquals(topic_0, topic_1))
        {
            // We should always get here for any singleton well known base types:
            // MDER_Ignorance, MDER_False, MDER_True.
            return true;
        }
        if (topic_0.__WKBT != topic_1.__WKBT)
        {
            return false;
        }
        if (topic_0.__cached_MDER_Any_identity is not null
            && topic_1.__cached_MDER_Any_identity is not null)
        {
            if (String.Equals(topic_0.__cached_MDER_Any_identity,
                topic_1.__cached_MDER_Any_identity))
            {
                return true;
            }
            return false;
        }
        switch (topic_0.__WKBT)
        {
            case Internal_Well_Known_Base_Type.MDER_Ignorance:
            case Internal_Well_Known_Base_Type.MDER_False:
            case Internal_Well_Known_Base_Type.MDER_True:
                // We should never get here.
                throw new InvalidOperationException();
            case Internal_Well_Known_Base_Type.MDER_Integer:
                return ((MDER_Integer)topic_0)._MDER_Integer__same((MDER_Integer)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Fraction:
                return ((MDER_Fraction)topic_0)._MDER_Fraction__same((MDER_Fraction)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Bits:
                return ((MDER_Bits)topic_0)._MDER_Bits__same((MDER_Bits)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Blob:
                return ((MDER_Blob)topic_0)._MDER_Blob__same((MDER_Blob)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Text:
                return ((MDER_Text)topic_0)._MDER_Text__same((MDER_Text)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Array:
                return ((MDER_Array)topic_0)._MDER_Array__same((MDER_Array)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Set:
                return ((MDER_Set)topic_0)._MDER_Set__same((MDER_Set)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Bag:
                return ((MDER_Bag)topic_0)._MDER_Bag__same((MDER_Bag)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Heading:
                return ((MDER_Heading)topic_0)._MDER_Heading__same((MDER_Heading)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Tuple:
                return ((MDER_Tuple)topic_0)._MDER_Tuple__same((MDER_Tuple)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Tuple_Array:
                return ((MDER_Tuple_Array)topic_0)._MDER_Tuple_Array__same((MDER_Tuple_Array)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Relation:
                return ((MDER_Relation)topic_0)._MDER_Relation__same((MDER_Relation)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Tuple_Bag:
                return ((MDER_Tuple_Bag)topic_0)._MDER_Tuple_Bag__same((MDER_Tuple_Bag)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Article:
                return ((MDER_Article)topic_0)._MDER_Article__same((MDER_Article)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Excuse:
                return ((MDER_Excuse)topic_0)._MDER_Excuse__same((MDER_Excuse)topic_1);
            case Internal_Well_Known_Base_Type.MDER_Variable:
            case Internal_Well_Known_Base_Type.MDER_Process:
            case Internal_Well_Known_Base_Type.MDER_Stream:
            case Internal_Well_Known_Base_Type.MDER_External:
                // Every Muldis Data Language Handle object is always distinct from every other one.
                return false;
            default:
                throw new NotImplementedException();
        }
    }
}

public class MDER_Any_Comparer : EqualityComparer<MDER_Any>
{
    public override Boolean Equals(MDER_Any? topic_0, MDER_Any? topic_1)
    {
        if (Object.ReferenceEquals(topic_0, topic_1))
        {
            // We get here if both args are null, if that is possible.
            return true;
        }
        if (topic_0 is null || topic_1 is null)
        {
            return false;
        }
        return topic_0._MDER_Any__same(topic_1);
    }

    public override Int32 GetHashCode(MDER_Any topic)
    {
        if (topic is null)
        {
            // Would we ever get here?
            return 0;
        }
        return topic._identity_as_String().GetHashCode();
    }
}
