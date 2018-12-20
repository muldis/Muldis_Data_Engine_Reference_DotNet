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
                    + " <MDBP entrance class name> <source code file path>");
                return;
            }

            // Name of class we expect to implement Muldis DataBase Protocol entrance interface.
            String mdbpEntranceClassName = args[0];

            // Try and load the entrance class, or die.
            // Note, generally the class name needs to be assembly-qualified for
            // GetType() to find it; eg "Company.Project.Class,Company.Project" works.
            Type entranceClass = Type.GetType(mdbpEntranceClassName);
            if (entranceClass == null)
            {
                System.Console.WriteLine(
                    "The requested Muldis DataBase Protocol entrance class"
                    + " [" + mdbpEntranceClassName + "] can't be loaded;"
                    + " perhaps it needs to be fully qualified with the assembly name.");
                return;
            }

            // Die unless the entrance class explicitly declares it implements MDBP.
            if (entranceClass.GetMethod("ProvidesMuldisDatabaseProtocolEntrance") == null)
            {
                System.Console.WriteLine(
                    "The requested Muldis DataBase Protocol entrance class"
                    + " [" + mdbpEntranceClassName + "]"
                    + " doesn't declare that it provides a version of the MDBP API"
                    + " by declaring the method [ProvidesMuldisDatabaseProtocolEntrance].");
                return;
            }

            // Instantiate object of a Muldis DataBase Protocol entrance class.
            MdbpEntrance entrance = (MdbpEntrance)Activator.CreateInstance(entranceClass);

            // Request a factory object implementing a specific version of the
            // MDBP or what the entrance considers the next best fit version;
            // this would die if it thinks it can't satisfy an acceptable version.
            // We will use this for all the main work.
            MdbpFactory factory = entrance.NewMdbpFactory(
                new String[] {"Muldis_Database_Protocol", "http://muldis.com", "0.201.0"});
            if (factory == null)
            {
                System.Console.WriteLine(
                    "The requested Muldis DataBase Protocol entrance class"
                    + " [" + mdbpEntranceClassName + "]"
                    + " doesn't provide the specific MDBP version needed.");
                return;
            }

            // Request a VM object implementing a specific version of the
            // data model or what the factory considers the next best fit version;
            // this would die if it thinks it can't satisfy an acceptable version.
            // We will use this for all the main work.
            MdbpMachine machine = factory.NewMdbpMachine(
                new String[] {"Muldis_Data_Language", "http://muldis.com", "0.201.0"});
            if (machine == null)
            {
                System.Console.WriteLine(
                    "The requested Muldis DataBase Protocol entrance class"
                    + " [" + mdbpEntranceClassName + "]"
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
            // the MDBP-implementing virtual machine where it would be used.
            MdbpValue source_code_blob = machine.MdbpImport(sourceCodeFileContent);

            // Try to parse the file content into canonical format VM source code.
            MdbpValue maybe_source_code_text = machine.MdbpEvaluate(
                machine.MdbpImport(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Text_from_UTF_8_Blob"})),
                machine.MdbpImport(new KeyValuePair<String, Object>("Tuple", new Object[] {source_code_blob}))
            );
            if ((Boolean)machine.MdbpExportQualified(machine.MdbpEvaluate(
                machine.MdbpImport(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Excuse"})),
                machine.MdbpImport(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text})))).Value)
            {
                System.Console.WriteLine("The requested source code providing file"
                    + " [" + sourceCodeFilePath + "] is not well-formed UTF-8 text:"
                    + " " + maybe_source_code_text.ToString());
                return;
            }
            MdbpValue maybe_source_code = machine.MdbpEvaluate(
                machine.MdbpImport(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "MDPT_Parsing_Unit_Text_to_Any"})),
                machine.MdbpImport(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text}))
            );

            // Temporary Executor test.
            MdbpValue sum = machine.MdbpEvaluate(
                machine.MdbpImport(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Integer_plus"})),
                machine.MdbpImport(new KeyValuePair<String, Object>("Tuple", new Object[] {27,39}))
            );
            MdbpValue that = machine.MdbpImport(new KeyValuePair<String, Object>("Tuple", new Object[] {27,39}));
            MdbpValue that_too = machine.MdbpImport(new KeyValuePair<String, Object>("Tuple", new Dictionary<String,Object>()
                {{"\u0014", 25}, {"aye", "zwei"}, {"some one", "other two"}}
            ));
            MdbpValue the_other = machine.MdbpImport("Fr ⊂ ac 💩 ti ÷ on");
            MdbpValue f0 = machine.MdbpImport(014.0M);
            MdbpValue f1 = machine.MdbpImport(2.3M);
            MdbpValue f2 = machine.MdbpImport(02340233.23402532000M);
            MdbpValue f3 = machine.MdbpImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(13,5)));
            MdbpValue f4 = machine.MdbpImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(27,6)));
            MdbpValue f5 = machine.MdbpImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(35,-41)));
            MdbpValue f6 = machine.MdbpImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(new BigInteger(-54235435432),new BigInteger(32543252))));
            MdbpValue f7 = machine.MdbpImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(26,13)));
            MdbpValue f8 = machine.MdbpImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5,1)));
            MdbpValue f9 = machine.MdbpImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5,-1)));
        }
    }
}
