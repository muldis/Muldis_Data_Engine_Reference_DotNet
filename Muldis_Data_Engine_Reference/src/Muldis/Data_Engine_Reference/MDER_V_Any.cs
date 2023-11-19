using System;

using Muldis.Data_Engine_Reference.Internal;

namespace Muldis.Data_Engine_Reference;

public class MDER_V_Any
{
    internal MDER_Machine m_machine;
    internal MD_Any m_value;

    internal MDER_V_Any(MDER_Machine machine, MD_Any value)
    {
        m_machine = machine;
        m_value   = value;
    }

    public override String ToString()
    {
        return m_value.ToString();
    }

    public MDER_Machine MDER_Machine()
    {
        return m_machine;
    }
}
