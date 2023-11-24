namespace Muldis.Data_Engine_Reference.Internal;

// TODO: Some of this text has obsolete notions and it should be rewritten.
// Muldis.Data_Engine_Reference.Internal.Standard_Generator
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

internal class Standard_Generator : MUON_Generator
{
    internal MDL_Any MDL_Any_to_MDL_Text_MDPT_Parsing_Unit(MDL_Any value)
    {
        return value.memory.MDL_Text(
            "(Muldis_Object_Notation_Syntax:([Plain_Text,"
            + " \"https://muldis.com\", \"0.300.0\"]:\u000A"
            + Any_Selector(value, "") + "))\u000A",
            false
        );
    }

    protected override String Any_Selector(MDL_Any value, String indent)
    {
        return Any_Selector_Foundation_Dispatch(value, indent);
    }
}
