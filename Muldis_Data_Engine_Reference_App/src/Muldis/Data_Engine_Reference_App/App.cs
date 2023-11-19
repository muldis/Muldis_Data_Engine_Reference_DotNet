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
        MuseMachine machine = new MuseMachine();

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
        // the MUSE-implementing virtual machine where it would be used.
        MuseValue source_code_blob = machine.MuseImport(sourceCodeFileContent);
        System.Console.WriteLine("Debug: source_code_blob = [" + source_code_blob + "]");

        // Try to parse the file content into canonical format VM source code.
        MuseValue maybe_source_code_text = machine.MuseEvaluate(
            machine.MuseImport(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Text_from_UTF_8_Blob"})),
            machine.MuseImport(new KeyValuePair<String, Object>("Tuple", new Object[] {source_code_blob}))
        );
        if ((Boolean)machine.MuseExportQualified(machine.MuseEvaluate(
            machine.MuseImport(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Excuse"})),
            machine.MuseImport(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text })))).Value)
        {
            System.Console.WriteLine("The requested source code providing file"
                + " [" + sourceCodeFilePath + "] is not well-formed UTF-8 text:"
                + " " + maybe_source_code_text.ToString());
            return;
        }
        System.Console.WriteLine("Debug: maybe_source_code_text = [" + maybe_source_code_text + "]");
        MuseValue maybe_source_code = machine.MuseEvaluate(
            machine.MuseImport(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "MDPT_Parsing_Unit_Text_to_Any"})),
            machine.MuseImport(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text }))
        );
        System.Console.WriteLine("Debug: maybe_source_code = [" + maybe_source_code + "]");

        // Temporary Executor test.
        MuseValue sum = machine.MuseEvaluate(
            machine.MuseImport(new KeyValuePair<String, Object>("Attr_Name_List", new String[] {"foundation", "Integer_plus"})),
            machine.MuseImport(new KeyValuePair<String, Object>("Tuple", new Object[] {27,39}))
        );
        System.Console.WriteLine("Test data: sum = [" + sum + "]");
        MuseValue that = machine.MuseImport(new KeyValuePair<String, Object>("Tuple", new Object[] {27,39}));
        System.Console.WriteLine("Test data: that = [" + that + "]");
        MuseValue that_too = machine.MuseImport(new KeyValuePair<String, Object>("Tuple", new Dictionary<String,Object>()
            {{"\u0014", 25}, {"aye", "zwei"}, {"some one", "other two"}}
        ));
        System.Console.WriteLine("Test data: that_too = [" + that_too + "]");
        MuseValue the_other = machine.MuseImport("Fr ⊂ ac 💩 ti ÷ on");
        System.Console.WriteLine("Test data: the_other = [" + the_other + "]");
        MuseValue f0 = machine.MuseImport(014.0M);
        System.Console.WriteLine("Test data: f0 = [" + f0 + "]");
        MuseValue f1 = machine.MuseImport(2.3M);
        System.Console.WriteLine("Test data: f1 = [" + f1 + "]");
        MuseValue f2 = machine.MuseImport(02340233.23402532000M);
        System.Console.WriteLine("Test data: f2 = [" + f2 + "]");
        MuseValue f3 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(13,5)));
        System.Console.WriteLine("Test data: f3 = [" + f3 + "]");
        MuseValue f4 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(27,6)));
        System.Console.WriteLine("Test data: f4 = [" + f4 + "]");
        MuseValue f5 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(35,-41)));
        System.Console.WriteLine("Test data: f5 = [" + f5 + "]");
        MuseValue f6 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(new BigInteger(-54235435432),new BigInteger(32543252))));
        System.Console.WriteLine("Test data: f6 = [" + f6 + "]");
        MuseValue f7 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(26,13)));
        System.Console.WriteLine("Test data: f7 = [" + f7 + "]");
        MuseValue f8 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5,1)));
        System.Console.WriteLine("Test data: f8 = [" + f8 + "]");
        MuseValue f9 = machine.MuseImport(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5,-1)));
        System.Console.WriteLine("Test data: f9 = [" + f9 + "]");
    }
}
