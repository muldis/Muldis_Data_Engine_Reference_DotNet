using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Muldis.DatabaseProtocol;

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
                t => t.Name == "Provides_Dot_Net_Muldis_DBP"))
            {
                System.Console.WriteLine(
                    "The requested MDBP provider information class"
                    + " [" + MDBP_provider_info_class_name + "]"
                    + " doesn't declare that it provides a MDBP API"
                    + " by declaring the method [Provides_Dot_Net_Muldis_DBP].");
                return;
            }

            // Instantiate object of a Muldis DataBase Protocol provider information class.
            IInfo provider_info
                = (IInfo)Activator.CreateInstance(provider_info_class);

            // Request a VM object implementing a specific version of the MDBP or
            // what the info provider considers the next best fit version;
            // this would die if it thinks it can't satisfy an acceptable version.
            // We will use this for all the main work.
            IMachine machine = provider_info.Want_VM_API(
                new String[] {"Muldis_DBP_DotNet", "http://muldis.com", "0.1.0.-9"});
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
            IMD_Any source_code_blob = machine.MD_Any(source_code_file_content);

            // Try to parse the file content into canonical format VM source code.
            IMD_Any maybe_source_code_text = machine.Evaluates(
                machine.MD_Any(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Text_from_UTF_8_Blob"})),
                machine.MD_Any(new KeyValuePair<String, Object>("Tuple", new Object[] {source_code_blob}))
            );
            if ((machine.Evaluates(
                machine.MD_Any(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Excuse"})),
                machine.MD_Any(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text})))).Export_Boolean())
            {
                System.Console.WriteLine("The requested source code providing file"
                    + " [" + source_code_file_path + "] is not well-formed UTF-8 text:"
                    + " " + maybe_source_code_text.ToString());
                return;
            }
            IMD_Any maybe_source_code = machine.Evaluates(
                machine.MD_Any(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "MDPT_Parsing_Unit_Text_to_Any"})),
                machine.MD_Any(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text}))
            );

            // Temporary Executor test.
            IMD_Any sum = machine.Evaluates(
                machine.MD_Any(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Integer_plus"})),
                machine.MD_Any(new KeyValuePair<String, Object>("Tuple", new Object[] {27,39}))
            );
            IMD_Any that = machine.MD_Any(new KeyValuePair<String, Object>("Tuple", new Object[] {27,39}));
            IMD_Any that_too = machine.MD_Any(new KeyValuePair<String, Object>("Tuple", new Dictionary<String,Object>()
                {{"\u0014", 25}, {"aye", "zwei"}, {"some one", "other two"}}
            ));
            IMD_Any the_other = machine.MD_Any("Fr ⊂ ac 💩 ti ÷ on");
            IMD_Any f0 = machine.MD_Any(014.0M);
            IMD_Any f1 = machine.MD_Any(2.3M);
            IMD_Any f2 = machine.MD_Any(02340233.23402532000M);
            IMD_Any f3 = machine.MD_Any(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(13,5)));
            IMD_Any f4 = machine.MD_Any(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(27,6)));
            IMD_Any f5 = machine.MD_Any(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(35,-41)));
            IMD_Any f6 = machine.MD_Any(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(new BigInteger(-54235435432),new BigInteger(32543252))));
            IMD_Any f7 = machine.MD_Any(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(26,13)));
            IMD_Any f8 = machine.MD_Any(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5,1)));
            IMD_Any f9 = machine.MD_Any(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5,-1)));
        }
    }
}
