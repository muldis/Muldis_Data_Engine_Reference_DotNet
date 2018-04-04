using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            IExecutor vme = machine.Executor();
            IImporter vmi = machine.Importer();

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
            IMD_Blob source_code_blob = vmi.MD_Blob(source_code_file_content);

            // Try to parse the file content into canonical format VM source code.
            IMD_Any maybe_source_code_text = vme.Evaluates(
                vmi.MD_Attr_Name_List(new String[] {"foundation", "Text_from_UTF_8_Blob"}),
                vmi.MD_Tuple(source_code_blob)
            );
            if (((IMD_Boolean)vme.Evaluates(
                vmi.MD_Attr_Name_List(new String[] {"foundation", "Excuse"}),
                vmi.MD_Tuple(maybe_source_code_text))).Export_Boolean())
            {
                System.Console.WriteLine("The requested source code providing file"
                    + " [" + source_code_file_path + "] is not well-formed UTF-8 text:"
                    + " " + maybe_source_code_text.ToString());
                return;
            }
            IMD_Any maybe_source_code = vme.Evaluates(
                vmi.MD_Attr_Name_List(new String[] {"foundation", "MDPT_Parsing_Unit_Text_to_Any"}),
                vmi.MD_Tuple(maybe_source_code_text)
            );

            // Temporary Executor test.
            IMD_Integer sum = (IMD_Integer)vme.Evaluates(
                vmi.MD_Attr_Name_List(new String[] {"foundation", "Integer_plus"}),
                vmi.MD_Tuple(27,39)
            );
            IMD_Tuple that = vmi.MD_Tuple(27,39);
            IMD_Tuple that_too = vmi.MD_Tuple(attrs: new Dictionary<String,Object>()
                {{"\u0014", 25}, {"aye", "zwei"}, {"some one", "other two"}}
            );
            IMD_Text the_other = vmi.MD_Text("Fr ⊂ ac 💩 ti ÷ on");
            IMD_Fraction f0 = vmi.MD_Fraction(014.0M);
            IMD_Fraction f1 = vmi.MD_Fraction(2.3M);
            IMD_Fraction f2 = vmi.MD_Fraction(02340233.23402532000M);
            IMD_Fraction f3 = vmi.MD_Fraction(13,5);
            IMD_Fraction f4 = vmi.MD_Fraction(27,6);
            IMD_Fraction f5 = vmi.MD_Fraction(35,-41);
            IMD_Fraction f6 = vmi.MD_Fraction(-54235435432,32543252);
            IMD_Fraction f7 = vmi.MD_Fraction(26,13);
            IMD_Fraction f8 = vmi.MD_Fraction(5,1);
            IMD_Fraction f9 = vmi.MD_Fraction(5,-1);
        }
    }
}
