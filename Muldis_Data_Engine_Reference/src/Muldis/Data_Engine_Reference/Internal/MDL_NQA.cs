namespace Muldis.Data_Engine_Reference.Internal;

// Muldis.Data_Engine_Reference.Internal.MDL_NQA
// Not Quite Any.
// This is a temporary class while a bunch of new sub-classes are refactored
// out of MDL_Any, and then the contstructor/etc this provides can go
// into MDL_Any itself.

internal abstract class MDL_NQA : MDL_Any
{
    internal MDL_NQA(Memory memory, Well_Known_Base_Type WKBT)
    {
        this.memory = memory;
        this.WKBT = WKBT;
    }
}
