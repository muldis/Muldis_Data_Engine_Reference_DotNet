namespace Muldis.Data_Engine_Reference;

internal class MDER_Variable : MDER_Any
{
    internal MDER_Any current_value;

    internal MDER_Variable(Memory memory, MDER_Any initial_current_value)
        : base(memory, Well_Known_Base_Type.MDER_Variable)
    {
        this.current_value = initial_current_value;
    }
}
