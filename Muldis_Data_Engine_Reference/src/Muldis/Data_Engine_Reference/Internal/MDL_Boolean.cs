using System;

namespace Muldis.Data_Engine_Reference.Internal;

internal class MDL_Boolean : MDL_NQA
{
    internal readonly Nullable<Boolean> as_Boolean;

    internal MDL_Boolean(Memory memory, Nullable<Boolean> as_Boolean)
        : base(memory, Well_Known_Base_Type.MDL_Boolean)
    {
        this.as_Boolean = as_Boolean;
    }
}
