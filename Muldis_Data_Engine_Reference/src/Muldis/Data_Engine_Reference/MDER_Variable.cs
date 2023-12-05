namespace Muldis.Data_Engine_Reference;

internal class MDL_Variable : MDL_Any
{
    internal MDL_Any current_value;

    internal MDL_Variable(Memory memory, MDL_Any initial_current_value)
        : base(memory, Well_Known_Base_Type.MDL_Variable)
    {
        this.current_value = initial_current_value;
    }
}
