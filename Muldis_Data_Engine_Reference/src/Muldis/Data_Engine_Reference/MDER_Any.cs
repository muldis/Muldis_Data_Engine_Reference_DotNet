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
    internal MDER_Any()
    {
    }

    public override String ToString()
    {
        return this._preview_as_String();
    }

    public override Boolean Equals(Object? obj)
    {
        return (obj is MDER_Any specific_obj)
            ? this._MDER_Any__same(specific_obj)
            : false;
    }

    public override Int32 GetHashCode()
    {
        return this._identity_as_String().GetHashCode();
    }

    // The MDER_Machine VM that this MDER_Any "value" lives in.
    public abstract MDER_Machine machine();

    // Surrogate identity for this MDER_Any with a simpler representation.
    // Two MDER_Any have the same surrogate identity iff MDL considers them
    // to be the "same" value; otherwise they have different surrogates.
    // The surrogate is the value serialized as a MUON Plain Text artifact.
    // The surrogate is only valid during the lifetime of the host process
    // and only intended for use in the private memory of the MDER_Machine
    // that made it; it should not be used for interchange or persistence.
    // The surrogate may be lazily generated and cached.
    // The surrogate is intended to support implementation of collections
    // of the general case of a MDER_Any that index elements by logical
    // value, such as set-like collections, that are simple and performant.
    internal abstract String _identity_as_String();

    // Content "preview quick look" for this MDER_Any to aid debugging.
    // The preview is the value serialized as a MUON Plain Text artifact.
    // The preview is not guaranteed to represent the entire logical value
    // and two MDER_Any that MDL considers to be the "same" value may not
    // have the same preview; its purpose is to provide the gist only.
    internal abstract String _preview_as_String();

    // Exports this MDER_Any as a MUON Plain Text artifact for interchange.
    // The export is guaranteed to represent the entire logical value in a
    // round-trippable form with another MDER_Any.
    // Two MDER_Any that MDL considers to be the "same" value may not
    // have the same export; for example, elements of non-ordered
    // collections might appear in different orders.
    public MDER_Text as_MUON_Plain_Text_artifact_as_MDER_Text()
    {
        String artifact_as_String = this._as_MUON_Plain_Text_artifact("");
        Internal_Unicode_Test_Result test_result
            = Internal_Unicode.test_String(artifact_as_String);
        return this.machine()._MDER_Text(
            "(Muldis_Object_Notation_Syntax:([Plain_Text,"
            + " \"https://muldis.com\", \"0.300.0\"]:\u000A"
            + artifact_as_String + "))\u000A",
            test_result == Internal_Unicode_Test_Result.Valid_Has_Non_BMP,
            false
        );
    }

    // Exports this MDER_Any as a MUON Plain Text artifact for interchange.
    // The export is guaranteed to represent the entire logical value in a
    // round-trippable form with another MDER_Any.
    // Two MDER_Any that MDL considers to be the "same" value may not
    // have the same export; for example, elements of non-ordered
    // collections might appear in different orders.
    public MDER_Blob as_MUON_Plain_Text_artifact_as_MDER_Blob()
    {
        throw new NotImplementedException();
    }

    internal abstract String _as_MUON_Plain_Text_artifact(String indent);

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
            // We should always get here for any singleton specific types:
            // MDER_Ignorance, MDER_False, MDER_True.
            return true;
        }
        if (!Type.Equals(topic_0.GetType(), topic_1.GetType()))
        {
            return false;
        }
        return (topic_0, topic_1) switch
        {
            (MDER_Ignorance specific_topic_0, MDER_Ignorance specific_topic_1) =>
                // We should never get here.
                throw new InvalidOperationException(),
            (MDER_False specific_topic_0, MDER_False specific_topic_1) =>
                // We should never get here.
                throw new InvalidOperationException(),
            (MDER_True specific_topic_0, MDER_True specific_topic_1) =>
                // We should never get here.
                throw new InvalidOperationException(),
            (MDER_Integer specific_topic_0, MDER_Integer specific_topic_1) =>
                specific_topic_0._MDER_Integer__same(specific_topic_1),
            (MDER_Fraction specific_topic_0, MDER_Fraction specific_topic_1) =>
                specific_topic_0._MDER_Fraction__same(specific_topic_1),
            (MDER_Bits specific_topic_0, MDER_Bits specific_topic_1) =>
                specific_topic_0._MDER_Bits__same(specific_topic_1),
            (MDER_Blob specific_topic_0, MDER_Blob specific_topic_1) =>
                specific_topic_0._MDER_Blob__same(specific_topic_1),
            (MDER_Text specific_topic_0, MDER_Text specific_topic_1) =>
                specific_topic_0._MDER_Text__same(specific_topic_1),
            (MDER_Array specific_topic_0, MDER_Array specific_topic_1) =>
                specific_topic_0._MDER_Array__same(specific_topic_1),
            (MDER_Set specific_topic_0, MDER_Set specific_topic_1) =>
                specific_topic_0._MDER_Set__same(specific_topic_1),
            (MDER_Bag specific_topic_0, MDER_Bag specific_topic_1) =>
                specific_topic_0._MDER_Bag__same(specific_topic_1),
            (MDER_Heading specific_topic_0, MDER_Heading specific_topic_1) =>
                specific_topic_0._MDER_Heading__same(specific_topic_1),
            (MDER_Tuple specific_topic_0, MDER_Tuple specific_topic_1) =>
                specific_topic_0._MDER_Tuple__same(specific_topic_1),
            (MDER_Tuple_Array specific_topic_0, MDER_Tuple_Array specific_topic_1) =>
                specific_topic_0._MDER_Tuple_Array__same(specific_topic_1),
            (MDER_Relation specific_topic_0, MDER_Relation specific_topic_1) =>
                specific_topic_0._MDER_Relation__same(specific_topic_1),
            (MDER_Tuple_Bag specific_topic_0, MDER_Tuple_Bag specific_topic_1) =>
                specific_topic_0._MDER_Tuple_Bag__same(specific_topic_1),
            (MDER_Article specific_topic_0, MDER_Article specific_topic_1) =>
                specific_topic_0._MDER_Article__same(specific_topic_1),
            (MDER_Excuse specific_topic_0, MDER_Excuse specific_topic_1) =>
                specific_topic_0._MDER_Excuse__same(specific_topic_1),
            (MDER_Variable specific_topic_0, MDER_Variable specific_topic_1) =>
                // Every Muldis Data Language Handle object is always distinct from every other one.
                false,
            (MDER_Process specific_topic_0, MDER_Process specific_topic_1) =>
                // Every Muldis Data Language Handle object is always distinct from every other one.
                false,
            (MDER_Stream specific_topic_0, MDER_Stream specific_topic_1) =>
                // Every Muldis Data Language Handle object is always distinct from every other one.
                false,
            (MDER_External specific_topic_0, MDER_External specific_topic_1) =>
                // Every Muldis Data Language Handle object is always distinct from every other one.
                false,
            _ => throw new NotImplementedException(),
        };
    }
}
