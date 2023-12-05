namespace Muldis.Data_Engine_Reference;

public class MDER_V_Any
{
    private readonly MDER_Machine machine;
    internal readonly MDL_Any memory_value;

    internal MDER_V_Any(MDER_Machine machine, MDL_Any memory_value)
    {
        this.machine = machine;
        this.memory_value = memory_value;
    }

    public override String ToString()
    {
        return this.memory_value.ToString();
    }

    public MDER_Machine MDER_Machine()
    {
        return this.machine;
    }
}
