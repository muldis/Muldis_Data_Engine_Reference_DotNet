using System;
using System.Collections.Generic;

namespace Muldis.Data_Engine_Reference_App;

public static class MuseEntrance
{
    public static MuseFactory NewMuseFactory(String museEntranceClassName)
    {
        // Try and load the entrance class, or die.
        // Note, generally the class name needs to be assembly-qualified for
        // GetType() to find it; eg "Company.Project.Class,Company.Project" works.
        Type entranceClass = Type.GetType(museEntranceClassName);
        if (entranceClass == null)
        {
            System.Console.WriteLine(
                "The requested Muldis Service Protocol entrance class"
                + " [" + museEntranceClassName + "] can't be loaded;"
                + " perhaps it needs to be fully qualified with the assembly name.");
            return null;
        }

        // Die unless the entrance class explicitly declares it implements MUSE.
        if (entranceClass.GetMethod("ProvidesMuldisServiceProtocolEntrance") == null)
        {
            System.Console.WriteLine(
                "The requested Muldis Service Protocol entrance class"
                + " [" + museEntranceClassName + "]"
                + " doesn't declare that it provides a version of the MUSE API"
                + " by declaring the method [ProvidesMuldisServiceProtocolEntrance].");
            return null;
        }

        // Instantiate object of a Muldis Service Protocol entrance class.
        Object entrance = Activator.CreateInstance(entranceClass);

        // Request a factory object implementing a specific version of the
        // MUSE or what the entrance considers the next best fit version;
        // this would die if it thinks it can't satisfy an acceptable version.
        // We will use this for all the main work.
        String[] requestedMuseVersion = new String[]
            {"Muldis_Service_Protocol", "https://muldis.com", "0.300.0"};
        Object factory = entranceClass.GetMethod("NewMuseFactory")
            .Invoke(entrance, new Object[] { requestedMuseVersion });
        if (factory == null)
        {
            System.Console.WriteLine(
                "The requested Muldis Service Protocol entrance class"
                + " [" + museEntranceClassName + "]"
                + " doesn't provide the specific MUSE version needed.");
            return null;
        }

        return new MuseFactory().init(factory);
    }
}

public class MuseFactory
{
    internal Object m_factory;

    internal MuseFactory init(Object factory)
    {
        m_factory = factory;
        return this;
    }

    public void ProvidesMuldisServiceProtocolFactory() {}

    public MuseMachine NewMuseMachine(Object requestedModelVersion)
    {
        Object machine = m_factory.GetType().GetMethod("NewMuseMachine")
            .Invoke(m_factory, new Object[] { requestedModelVersion });
        if (machine == null)
        {
            return null;
        }
        return new MuseMachine().init(this, machine);
    }
}

public class MuseMachine
{
    internal MuseFactory m_factory;
    internal Object      m_machine;

    internal MuseMachine init(MuseFactory factory, Object machine)
    {
        m_factory = factory;
        m_machine = machine;
        return this;
    }

    public void ProvidesMuldisServiceProtocolMachine() {}

    public MuseFactory MuseFactory()
    {
        return m_factory;
    }

    public MuseValue MuseEvaluate(MuseValue function, MuseValue args = null)
    {
        if (function == null)
        {
            throw new ArgumentNullException("function");
        }
        return new MuseValue().init(this,
            m_machine.GetType().GetMethod("MuseEvaluate")
            .Invoke(m_machine, new Object[]
            { function.m_value, args == null ? null : args.m_value }));
    }

    public void MusePerform(MuseValue procedure, MuseValue args = null)
    {
        if (procedure == null)
        {
            throw new ArgumentNullException("procedure");
        }
        m_machine.GetType().GetMethod("MusePerform")
            .Invoke(m_machine, new Object[]
            { procedure.m_value, args == null ? null : args.m_value });
    }

    public MuseValue MuseCurrent(MuseValue variable)
    {
        if (variable == null)
        {
            throw new ArgumentNullException("variable");
        }
        return new MuseValue().init(this,
            m_machine.GetType().GetMethod("MuseCurrent")
            .Invoke(m_machine, new Object[] { variable.m_value }));
    }

    public void MuseAssign(MuseValue variable, MuseValue newCurrent)
    {
        if (variable == null)
        {
            throw new ArgumentNullException("variable");
        }
        if (newCurrent == null)
        {
            throw new ArgumentNullException("newCurrent");
        }
        m_machine.GetType().GetMethod("MuseAssign")
            .Invoke(m_machine, new Object[] { variable.m_value, newCurrent.m_value });
    }

    public MuseValue MuseImport(Object value)
    {
        return new MuseValue().init(this,
            m_machine.GetType().GetMethod("MuseImport")
            .Invoke(m_machine, new Object[] { value }));
    }

    public MuseValue MuseImportQualified(KeyValuePair<String,Object> value)
    {
        return new MuseValue().init(this,
            m_machine.GetType().GetMethod("MuseImportQualified")
            .Invoke(m_machine, new Object[] { value }));
    }

    public Object MuseExport(MuseValue value)
    {
        if (value == null)
        {
            throw new ArgumentNullException("value");
        }
        return m_machine.GetType().GetMethod("MuseExport")
            .Invoke(m_machine, new Object[] { value.m_value });
    }

    public KeyValuePair<String,Object> MuseExportQualified(MuseValue value)
    {
        if (value == null)
        {
            throw new ArgumentNullException("value");
        }
        return (KeyValuePair<String,Object>)m_machine.GetType().GetMethod("MuseExportQualified")
            .Invoke(m_machine, new Object[] { value.m_value });
    }
}

public class MuseValue
{
    internal MuseMachine m_machine;
    internal Object      m_value;

    internal MuseValue init(MuseMachine machine)
    {
        m_machine = machine;
        return this;
    }

    internal MuseValue init(MuseMachine machine, Object value)
    {
        m_machine = machine;
        m_value   = value;
        return this;
    }

    public override String ToString()
    {
        return m_value.ToString();
    }

    public void ProvidesMuldisServiceProtocolValue() {}

    public MuseMachine MuseMachine()
    {
        return m_machine;
    }

    internal Object temp_kludge_get_MUSE_server_MuseValue()
    {
        return m_value;
    }
}
