namespace Muldis.Data_Engine_Reference;

public class MDER_V_Any
{
    private readonly MDER_Machine __machine;
    internal readonly MDER_Any memory_value;

    internal MDER_V_Any(MDER_Machine machine, MDER_Any memory_value)
    {
        this.__machine = machine;
        this.memory_value = memory_value;
    }

    public override String ToString()
    {
        return this.memory_value.ToString();
    }

    public MDER_Machine machine()
    {
        return this.__machine;
    }
}
