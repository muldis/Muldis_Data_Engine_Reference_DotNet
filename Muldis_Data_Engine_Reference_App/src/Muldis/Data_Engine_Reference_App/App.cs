using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

using Muldis.Data_Engine_Reference;

namespace Muldis.Data_Engine_Reference_App;

public class App
{
    public static void Main(String[] args)
    {
        if (args.Length != 1)
        {
            // TODO: Review whether hostExecutablePath reflects actual
            // main program invocation syntax or not, and accounts for
            // the varied host operating systems or compiled vs debugged.
            String hostExecutablePath
                = System.Reflection.Assembly.GetEntryAssembly().Location;
            System.Console.WriteLine("Usage: " + hostExecutablePath
                + " <source code file path>");
            return;
        }

        // Request a VM object implementing the Muldis Data Language.
        // We will use this for all the main work.
        MDER_Machine machine = new MDER_Machine();

        // File system path for the file containing plain text source
        // code that the user wishes to execute as their main program.
        String sourceCodeFilePath = args[0];

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
        // the Muldis Data Language implementing virtual machine where it would be used.
        MDER_V_Any source_code_blob = machine.MDER_import(sourceCodeFileContent);
        System.Console.WriteLine("Debug: source_code_blob = [" + source_code_blob + "]");

        // Try to parse the file content into canonical format VM source code.
        MDER_V_Any maybe_source_code_text = machine.MDER_evaluate(
            machine.MDER_import(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Text_from_UTF_8_Blob"})),
            machine.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {source_code_blob}))
        );
        if ((Boolean)machine.MDER_export_qualified(machine.MDER_evaluate(
            machine.MDER_import(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Excuse"})),
            machine.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text })))).Value)
        {
            System.Console.WriteLine("The requested source code providing file"
                + " [" + sourceCodeFilePath + "] is not well-formed UTF-8 text:"
                + " " + maybe_source_code_text.ToString());
            return;
        }
        System.Console.WriteLine("Debug: maybe_source_code_text = [" + maybe_source_code_text + "]");
        MDER_V_Any maybe_source_code = machine.MDER_evaluate(
            machine.MDER_import(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "MDPT_Parsing_Unit_Text_to_Any"})),
            machine.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text }))
        );
        System.Console.WriteLine("Debug: maybe_source_code = [" + maybe_source_code + "]");

        // Temporary Executor test.
        MDER_V_Any sum = machine.MDER_evaluate(
            machine.MDER_import(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Integer_plus"})),
            machine.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {27,39}))
        );
        System.Console.WriteLine("Test data: sum = [" + sum + "]");
        MDER_V_Any that = machine.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {27,39}));
        System.Console.WriteLine("Test data: that = [" + that + "]");
        MDER_V_Any that_too = machine.MDER_import(new KeyValuePair<String, Object>("Tuple", new Dictionary<String,Object>()
            {{"\u0014", 25}, {"aye", "zwei"}, {"some one", "other two"}}
        ));
        System.Console.WriteLine("Test data: that_too = [" + that_too + "]");
        MDER_V_Any the_other = machine.MDER_import("Fr ⊂ ac 💩 ti ÷ on");
        System.Console.WriteLine("Test data: the_other = [" + the_other + "]");
        MDER_V_Any f0 = machine.MDER_import(014.0M);
        System.Console.WriteLine("Test data: f0 = [" + f0 + "]");
        MDER_V_Any f1 = machine.MDER_import(2.3M);
        System.Console.WriteLine("Test data: f1 = [" + f1 + "]");
        MDER_V_Any f2 = machine.MDER_import(02340233.23402532000M);
        System.Console.WriteLine("Test data: f2 = [" + f2 + "]");
        MDER_V_Any f3 = machine.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(13,5)));
        System.Console.WriteLine("Test data: f3 = [" + f3 + "]");
        MDER_V_Any f4 = machine.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(27,6)));
        System.Console.WriteLine("Test data: f4 = [" + f4 + "]");
        MDER_V_Any f5 = machine.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(35,-41)));
        System.Console.WriteLine("Test data: f5 = [" + f5 + "]");
        MDER_V_Any f6 = machine.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(new BigInteger(-54235435432),new BigInteger(32543252))));
        System.Console.WriteLine("Test data: f6 = [" + f6 + "]");
        MDER_V_Any f7 = machine.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(26,13)));
        System.Console.WriteLine("Test data: f7 = [" + f7 + "]");
        MDER_V_Any f8 = machine.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5,1)));
        System.Console.WriteLine("Test data: f8 = [" + f8 + "]");
        MDER_V_Any f9 = machine.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5,-1)));
        System.Console.WriteLine("Test data: f9 = [" + f9 + "]");
    }
}
