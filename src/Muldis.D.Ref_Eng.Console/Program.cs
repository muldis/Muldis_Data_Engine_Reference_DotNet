using System;
using System.IO;
using Muldis.DBP;

[assembly: CLSCompliant(true)]

namespace Muldis.D.Ref_Eng.Console
{
    public class Program
    {
        public static void Main(String[] args)
        {
            if (args.Length != 1)
            {
                // TODO: Review whether host_executable_path reflects actual
                // main program invocation syntax or not, and accounts for
                // the varied host operating systems or compiled vs debugged.
                String host_executable_path
                    = System.Reflection.Assembly.GetEntryAssembly().Location;
                System.Console.WriteLine(
                    "Usage: " + host_executable_path + " <source_code_file_path>");
                return;
            }

            // File system path for the file containing plain text source
            // code that the user wishes to execute as their main program.
            String source_code_file_path = args[0];

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

            // Instantiate object of a Muldis DataBase Protocol provider information class.
            IInfo provider_info = new Muldis.D.Ref_Eng.Info();

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
                    + " information class [Muldis.D.Ref_Eng.Info]"
                    + " doesn't provide the specific MDBP version needed.");
                return;
            }

            // Import the user-specified source code file's raw content into
            // the MDBP-implementing virtual machine where it would be used.
            IMD_Blob source_code_blob = machine.Importer().MD_Blob(source_code_file_content);
        }
    }
}
