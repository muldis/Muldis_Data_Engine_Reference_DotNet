namespace Muldis.Data_Engine_Reference;

public class MDER_Variable : MDER_Any
{
    internal MDER_Any current_value;

    internal MDER_Variable(MDER_Machine machine, MDER_Any initial_current_value)
        : base(machine)
    {
        this.current_value = initial_current_value;
    }
}
