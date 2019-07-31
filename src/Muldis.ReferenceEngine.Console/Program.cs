using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

[assembly: CLSCompliant(true)]

namespace Muldis.ReferenceEngine.Console
{
    public class Program
    {
        public static void Main(String[] args)
        {
            if (args.Length != 2)
            {
                // TODO: Review whether hostExecutablePath reflects actual
                // main program invocation syntax or not, and accounts for
                // the varied host operating systems or compiled vs debugged.
                String hostExecutablePath
                    = System.Reflection.Assembly.GetEntryAssembly().Location;
                System.Console.WriteLine("Usage: " + hostExecutablePath
                    + " <MUSE entrance class name> <source code file path>");
                return;
            }

            // Name of class we expect to implement Muldis Service Protocol entrance interface.
            String museEntranceClassName = args[0];

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
                return;
            }

            // Die unless the entrance class explicitly declares it implements MUSE.
            if (entranceClass.GetMethod("ProvidesMuldisServiceProtocolEntrance") == null)
            {
                System.Console.WriteLine(
                    "The requested Muldis Service Protocol entrance class"
                    + " [" + museEntranceClassName + "]"
                    + " doesn't declare that it provides a version of the MUSE API"
                    + " by declaring the method [ProvidesMuldisServiceProtocolEntrance].");
                return;
            }

            // Instantiate object of a Muldis Service Protocol entrance class.
            MuseEntrance entrance = (MuseEntrance)Activator.CreateInstance(entranceClass);

            // Request a factory object implementing a specific version of the
            // MUSE or what the entrance considers the next best fit version;
            // this would die if it thinks it can't satisfy an acceptable version.
            // We will use this for all the main work.
            MuseFactory factory = entrance.NewMuseFactory(
                new String[] {"Muldis_Service_Protocol", "http://muldis.com", "0.300.0"});
            if (factory == null)
            {
                System.Console.WriteLine(
                    "The requested Muldis Service Protocol entrance class"
                    + " [" + museEntranceClassName + "]"
                    + " doesn't provide the specific MUSE version needed.");
                return;
            }

            // Request a VM object implementing a specific version of the
            // data model or what the factory considers the next best fit version;
            // this would die if it thinks it can't satisfy an acceptable version.
            // We will use this for all the main work.
            MuseMachine machine = factory.NewMuseMachine(
                new String[] {"Muldis_Data_Language", "http://muldis.com", "0.300.0"});
            if (machine == null)
            {
                System.Console.WriteLine(
                    "The requested Muldis Service Protocol entrance class"
                    + " [" + museEntranceClassName + "]"
                    + " doesn't provide a factory class that provides"
                    + " the specific data model version needed.");
                return;
            }

            // File system path for the file containing plain text source
            // code that the user wishes to execute as their main program.
            String sourceCodeFilePath = args[1];

            // If the user-specified file path is absolute, Path.Combine()
            // will just use that as the final path; otherwise it is taken
            // as relative to the host executable's current working directory.
            sourceCodeFilePath = Path.Combine(
                Directory.GetCurrentDirectory(), sourceCodeFilePath);

            if (!File.Exists(sourceCodeFilePath))
            {
                System.Console.WriteLine("The requested source code providing file"
                    + " [" + sourceCodeFilePath + "] doesn't exist.");
                return;
            }

            Byte[] sourceCodeFileContent;
            try
            {
                sourceCodeFileContent = File.ReadAllBytes(sourceCodeFilePath);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("The requested source code providing file"
                    + " [" + sourceCodeFilePath + "] couldn't be read:"
                    + " " + e.ToString());
                return;
            }

            // Import the user-specified source code file's raw content into
            // the MUSE-implementing virtual machine where it would be used.
            MuseValue source_code_blob = machine.MuseImport(sourceCodeFileContent);

            // Try to parse the file content into canonical format VM source code.
            MuseValue maybe_source_code_text = machine.MuseEvaluate(
                machine.MuseImport(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Text_from_UTF_8_Blob"})),
                machine.MuseImport(new KeyValuePair<String, Object>("Tuple", new Object[] {source_code_blob}))
            );
            if ((Boolean)machine.MuseExportQualified(machine.MuseEvaluate(
                machine.MuseImport(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Excuse"})),
                machine.MuseImport(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text})))).Value)
            {
                System.Console.WriteLine("The requested source code providing file"
                    + " [" + sourceCodeFilePath + "] is not well-formed UTF-8 text:"
                    + " " + maybe_source_code_text.ToString());
                return;
            }
            MuseValue maybe_source_code = machine.MuseEvaluate(
                machine.MuseImport(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "MDPT_Parsing_Unit_Text_to_Any"})),
                machine.MuseImport(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text}))
            );

            // Temporary Executor test.
            MuseValue sum = machine.MuseEvaluate(
                machine.MuseImport(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Integer_plus"})),
                machine.MuseImport(new KeyValuePair<String, Object>("Tuple", new Object[] {27,39}))
            );
            MuseValue that = machine.MuseImport(new KeyValuePair<String, Object>("Tuple", new Object[] {27,39}));
            MuseValue that_too = machine.MuseImport(new KeyValuePair<String, Object>("Tuple", new Dictionary<String,Object>()
                {{"\u0014", 25}, {"aye", "zwei"}, {"some one", "other two"}}
            ));
            MuseValue the_other = machine.MuseImport("Fr ⊂ ac 💩 ti ÷ on");
            MuseValue f0 = machine.MuseImport(014.0M);
            MuseValue f1 = machine.MuseImport(2.3M);
            MuseValue f2 = machine.MuseImport(02340233.23402532000M);
            MuseValue f3 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(13,5)));
            MuseValue f4 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(27,6)));
            MuseValue f5 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(35,-41)));
            MuseValue f6 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(new BigInteger(-54235435432),new BigInteger(32543252))));
            MuseValue f7 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(26,13)));
            MuseValue f8 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5,1)));
            MuseValue f9 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5,-1)));
        }
    }
}
