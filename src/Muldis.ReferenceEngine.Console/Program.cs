using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                // TODO: Review whether host_executable_path reflects actual
                // main program invocation syntax or not, and accounts for
                // the varied host operating systems or compiled vs debugged.
                String host_executable_path
                    = System.Reflection.Assembly.GetEntryAssembly().Location;
                System.Console.WriteLine("Usage: " + host_executable_path
                    + " <MDBP_provider_info_class_name> <source_code_file_path>");
                return;
            }

            // Name of class we expect to implement Muldis DataBase Protocol provider information interface.
            String MDBP_provider_info_class_name = args[0];

            // Try and load the provider info class, or die.
            // Note, generally the class name needs to be assembly-qualified for
            // GetType() to find it; eg "Company.Project.Class,Company.Project" works.
            Type provider_info_class = Type.GetType(MDBP_provider_info_class_name);
            if (provider_info_class == null)
            {
                System.Console.WriteLine(
                    "The requested MDBP provider information class"
                    + " [" + MDBP_provider_info_class_name + "] can't be loaded;"
                    + " perhaps it needs to be fully qualified with the assembly name.");
                return;
            }

            // Die unless the provider info class explicitly declares it implements MDBP.
            if (!Enumerable.Any(provider_info_class.GetMethods(),
                t => t.Name == "ProvidesMuldisDatabaseProtocolInfo"))
            {
                System.Console.WriteLine(
                    "The requested MDBP provider information class"
                    + " [" + MDBP_provider_info_class_name + "]"
                    + " doesn't declare that it provides a MDBP API"
                    + " by declaring the method [ProvidesMuldisDatabaseProtocolInfo].");
                return;
            }

            // Instantiate object of a Muldis DataBase Protocol provider information class.
            MdbpInfo provider_info
                = (MdbpInfo)Activator.CreateInstance(provider_info_class);

            // Request a VM object implementing a specific version of the MDBP or
            // what the info provider considers the next best fit version;
            // this would die if it thinks it can't satisfy an acceptable version.
            // We will use this for all the main work.
            MdbpMachine machine = provider_info.MdbpWantMachineApi(
                new String[] {"Muldis_Database_Protocol", "http://muldis.com", "0.201.0"});
            if (machine == null)
            {
                System.Console.WriteLine(
                    "The requested Muldis DataBase Protocol provider"
                    + " information class [" + MDBP_provider_info_class_name + "]"
                    + " doesn't provide the specific MDBP version needed.");
                return;
            }

            // File system path for the file containing plain text source
            // code that the user wishes to execute as their main program.
            String source_code_file_path = args[1];

            // If the user-specified file path is absolute, Path.Combine()
            // will just use that as the final path; otherwise it is taken
            // as relative to the host executable's current working directory.
            source_code_file_path = Path.Combine(
                Directory.GetCurrentDirectory(), source_code_file_path);

            if (!File.Exists(source_code_file_path))
            {
                System.Console.WriteLine("The requested source code providing file"
                    + " [" + source_code_file_path + "] doesn't exist.");
                return;
            }

            Byte[] source_code_file_content;
            try
            {
                source_code_file_content = File.ReadAllBytes(source_code_file_path);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("The requested source code providing file"
                    + " [" + source_code_file_path + "] couldn't be read:"
                    + " " + e.ToString());
                return;
            }

            // Import the user-specified source code file's raw content into
            // the MDBP-implementing virtual machine where it would be used.
            MdbpValue source_code_blob = machine.MdbpImport(source_code_file_content);

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
                    + " [" + source_code_file_path + "] is not well-formed UTF-8 text:"
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
