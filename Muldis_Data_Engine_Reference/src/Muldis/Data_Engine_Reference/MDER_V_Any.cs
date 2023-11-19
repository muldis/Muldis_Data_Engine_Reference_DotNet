using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Muldis.Data_Engine_Reference;

public class MuseValue
{
    internal MuseMachine m_machine;
    internal Core.MD_Any m_value;

    internal MuseValue init(MuseMachine machine)
    {
        m_machine = machine;
        return this;
    }

    internal MuseValue init(MuseMachine machine, Core.MD_Any value)
    {
        m_machine = machine;
        m_value   = value;
        return this;
    }

    public override String ToString()
    {
        return m_value.ToString();
    }

    public MuseMachine MuseMachine()
    {
        return m_machine;
    }
}
