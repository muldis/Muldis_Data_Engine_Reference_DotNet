using System.Collections;
using System.Numerics;

using Muldis.Data_Engine_Reference;

namespace Muldis.Data_Engine_Reference_App;

public sealed class App
{
    private enum App_Arg_Name
    {
        task,
        verbose,
        @in,
    }

    private enum App_Task
    {
        version,
        help,
        perform,
    }

    private App()
    {
    }

    public static void Main(String[] raw_app_args)
    {
        task_version();
        Dictionary<App_Arg_Name, String?> app_args
            = normalized_app_args(raw_app_args);
        if (app_args.Count == 0)
        {
            System.Console.WriteLine(
                "Fatal: Task-naming primary app argument is missing.");
            task_help();
            return;
        }
        App_Task task;
        if (!Enum.TryParse<App_Task>(app_args[App_Arg_Name.task], out task))
        {
            System.Console.WriteLine(
                "Fatal: Unrecognized task: " + app_args[App_Arg_Name.task]);
            task_help();
            return;
        }
        switch (task)
        {
            case App_Task.version:
                // Skip since app always does this when it starts.
                break;
            case App_Task.help:
                task_help();
                break;
            case App_Task.perform:
                task_perform(app_args);
                break;
            default:
                task_help();
                break;
        }
    }

    private static Dictionary<App_Arg_Name, String?> normalized_app_args(
        String[] raw_app_args)
    {
        // The "task" arg is expected to be positional, and the others named.
        // A positional arg does NOT start with "--",
        // a named looks like "--foo=bar" or "--foo".
        Dictionary<App_Arg_Name, String?> app_args
            = new Dictionary<App_Arg_Name, String?>();
        if (raw_app_args.Length > 0 && !raw_app_args[0].StartsWith("--"))
        {
            for (Int32 i = 1; i < raw_app_args.Length; i++)
            {
                String raw_app_arg = raw_app_args[i];
                if (raw_app_arg.StartsWith("--"))
                {
                    String arg_name;
                    String? arg_value;
                    if (raw_app_arg.Contains("="))
                    {
                        // Named arg format "--foo=bar",
                        // the most generic format.
                        Int32 eq_pos = raw_app_arg.IndexOf("=");
                        arg_name = raw_app_arg.Substring(2, eq_pos - 2);
                        arg_value = raw_app_arg.Substring(eq_pos + 1);
                    }
                    else
                    {
                        // Named arg format "--foo",
                        // a Boolean where present is true, absent false.
                        arg_name = raw_app_arg.Substring(2);
                        arg_value = null;
                    }
                    App_Arg_Name app_arg_name;
                    if (Enum.TryParse<App_Arg_Name>(
                        arg_name, out app_arg_name))
                    {
                        app_args.Add(app_arg_name, arg_value);
                    }
                    else
                    {
                        System.Console.WriteLine(
                            "Warning: Ignoring unrecognized argument: "
                            + arg_name);
                    }
                }
            }
            // Assign the positional "task" last so it takes
            // precedence over any named one.
            app_args.Add(App_Arg_Name.task, raw_app_args[0]);
        }
        return app_args;
    }

    private static void task_version()
    {
        System.Console.WriteLine("This application is"
            + " Muldis.Data_Engine_Reference_App.App version 0.1.");
    }

    private static void task_help()
    {
        String APP_NAME = "sh muldisder.sh";
        System.Console.WriteLine("Usage:");
        System.Console.WriteLine("  " + APP_NAME + " version");
        System.Console.WriteLine("  " + APP_NAME + " help");
        foreach (String generic_task in new String[] {"perform"})
        {
            System.Console.WriteLine("  " + APP_NAME + " " + generic_task
                + " [--verbose]"
                + " --in=<input file path>"
            );
        }
    }

    private static void task_perform(
        Dictionary<App_Arg_Name, String?> app_args)
    {
        System.Console.WriteLine("This is task perform.");
        Boolean verbose = app_args.ContainsKey(App_Arg_Name.verbose);
        if (!app_args.ContainsKey(App_Arg_Name.@in))
        {
            System.Console.WriteLine(
                "Fatal: Task perform: Missing argument: " + App_Arg_Name.@in);
            return;
        }
        String? raw_arg_path_in = app_args[App_Arg_Name.@in];
        if (raw_arg_path_in is null || String.Equals(raw_arg_path_in, ""))
        {
            System.Console.WriteLine(
                "Fatal: Task perform: Missing argument: " + App_Arg_Name.@in);
            return;
        }

        // Request a VM object implementing the Muldis Data Language.
        // We will use this for all the main work.
        MDER_Machine vm = new MDER_Machine();

        // File system path for the file containing plain text source
        // code that the user wishes to execute as their main program.
        String sourceCodeFilePath = raw_arg_path_in;

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

        test_import_type_specific_way(vm, sourceCodeFilePath, sourceCodeFileContent);
        test_import_MUON_hosted_way(vm, sourceCodeFilePath, sourceCodeFileContent);
    }

    public static void test_import_type_specific_way(
        MDER_Machine vm, String sourceCodeFilePath, Byte[] sourceCodeFileContent)
    {
        System.Console.WriteLine("Debug: test_import_type_specific_way");

        MDER_Ignorance ig = vm.MDER_Ignorance();
        System.Console.WriteLine("Test data: ig = [" + ig + "]");

        MDER_Boolean bf = vm.MDER_Boolean(false);
        System.Console.WriteLine("Test data: bf = [" + bf + "]");
        MDER_Boolean bt = vm.MDER_Boolean(true);
        System.Console.WriteLine("Test data: bt = [" + bt + "]");

        MDER_Integer i0 = vm.MDER_Integer(0);
        System.Console.WriteLine("Test data: i0 = [" + i0 + "]");
        MDER_Integer in3 = vm.MDER_Integer(-3);
        System.Console.WriteLine("Test data: in3 = [" + in3 + "]");
        MDER_Integer i52 = vm.MDER_Integer(52);
        System.Console.WriteLine("Test data: i52 = [" + i52 + "]");
        MDER_Integer i2323 = vm.MDER_Integer(BigInteger.Pow(23,23));
        System.Console.WriteLine("Test data: i2323 = [" + i2323 + "]");

        MDER_Bits bitse = vm.MDER_Bits(new BitArray(0));
        System.Console.WriteLine("Test data: bitse = [" + bitse + "]");
        MDER_Bits bits755 = vm.MDER_Bits(new BitArray(new bool[]
            {true, true, true, true, false, true, true, false, true}));
        System.Console.WriteLine("Test data: bits755 = [" + bits755 + "]");

        MDER_Blob blobe = vm.MDER_Blob(new byte[] {});
        System.Console.WriteLine("Test data: blobe = [" + blobe + "]");
        MDER_Blob blob4 = vm.MDER_Blob(new byte[] {0xA7,0x05,0xE4,0x16});
        System.Console.WriteLine("Test data: blob4 = [" + blob4 + "]");

        MDER_Text texte = vm.MDER_Text("");
        System.Console.WriteLine("Test data: texte = [" + texte + "]");
        MDER_Text texth = vm.MDER_Text("hello");
        System.Console.WriteLine("Test data: texth = [" + texth + "]");

        // Import the user-specified source code file's raw content into
        // the Muldis Data Language implementing virtual vm where it would be used.
        MDER_Blob source_code_blob = vm.MDER_Blob(sourceCodeFileContent);
        System.Console.WriteLine("Debug: source_code_blob = [" + source_code_blob + "]");

        // Try to parse the file content into canonical format VM source code.
        MDER_Any maybe_source_code_text = vm.MDER_evaluate(
            vm.MDER_import(new KeyValuePair<String, Object>("Nesting", new String[] {"foundation", "Text_from_UTF_8_Blob"})),
            vm.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {source_code_blob}))
        );
        if ((Boolean)vm.MDER_export_qualified(vm.MDER_evaluate(
            vm.MDER_import(new KeyValuePair<String, Object>("Nesting", new String[] {"foundation", "Excuse"})),
            vm.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text })))).Value!)
        {
            System.Console.WriteLine("The requested source code providing file"
                + " [" + sourceCodeFilePath + "] is not well-formed UTF-8 text:"
                + " " + maybe_source_code_text.ToString());
            return;
        }
        System.Console.WriteLine("Debug: maybe_source_code_text = [" + maybe_source_code_text + "]");
        MDER_Any maybe_source_code = vm.MDER_evaluate(
            vm.MDER_import(new KeyValuePair<String, Object>("Nesting", new String[] {"foundation", "MDPT_Parsing_Unit_Text_to_Any"})),
            vm.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text }))
        );
        System.Console.WriteLine("Debug: maybe_source_code = [" + maybe_source_code + "]");

        // Temporary Executor test.
        MDER_Any sum = vm.MDER_evaluate(
            vm.MDER_import(new KeyValuePair<String, Object>("Nesting", new String[] {"foundation", "Integer_plus"})),
            vm.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {27, 39}))
        );
        System.Console.WriteLine("Test data: sum = [" + sum + "]");
        MDER_Any that = vm.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {27, 39}));
        System.Console.WriteLine("Test data: that = [" + that + "]");
        MDER_Any that_too = vm.MDER_import(new KeyValuePair<String, Object>("Tuple", new Dictionary<String, Object>()
            {{"\u0014", 25}, {"aye", "zwei"}, {"some one", "other two"}}
        ));
        System.Console.WriteLine("Test data: that_too = [" + that_too + "]");
        MDER_Text the_other = vm.MDER_Text("Fr ⊂ ac 💩 ti ÷ on");
        System.Console.WriteLine("Test data: the_other = [" + the_other + "]");
        MDER_Any f0 = vm.MDER_import(014.0M);
        System.Console.WriteLine("Test data: f0 = [" + f0 + "]");
        MDER_Any f1 = vm.MDER_import(2.3M);
        System.Console.WriteLine("Test data: f1 = [" + f1 + "]");
        MDER_Any f2 = vm.MDER_import(02340233.23402532000M);
        System.Console.WriteLine("Test data: f2 = [" + f2 + "]");
        MDER_Any f3 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(13, 5)));
        System.Console.WriteLine("Test data: f3 = [" + f3 + "]");
        MDER_Any f4 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(27, 6)));
        System.Console.WriteLine("Test data: f4 = [" + f4 + "]");
        MDER_Any f5 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(35, -41)));
        System.Console.WriteLine("Test data: f5 = [" + f5 + "]");
        MDER_Any f6 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(new BigInteger(-54235435432), new BigInteger(32543252))));
        System.Console.WriteLine("Test data: f6 = [" + f6 + "]");
        MDER_Any f7 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(26, 13)));
        System.Console.WriteLine("Test data: f7 = [" + f7 + "]");
        MDER_Any f8 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5, 1)));
        System.Console.WriteLine("Test data: f8 = [" + f8 + "]");
        MDER_Any f9 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5, -1)));
        System.Console.WriteLine("Test data: f9 = [" + f9 + "]");
    }

    public static void test_import_MUON_hosted_way(
        MDER_Machine vm, String sourceCodeFilePath, Byte[] sourceCodeFileContent)
    {
        System.Console.WriteLine("Debug: test_import_MUON_hosted_way");

        // Import the user-specified source code file's raw content into
        // the Muldis Data Language implementing virtual vm where it would be used.
        MDER_Any source_code_blob = vm.MDER_import(sourceCodeFileContent);
        System.Console.WriteLine("Debug: source_code_blob = [" + source_code_blob + "]");

        // Try to parse the file content into canonical format VM source code.
        MDER_Any maybe_source_code_text = vm.MDER_evaluate(
            vm.MDER_import(new KeyValuePair<String, Object>("Nesting", new String[] {"foundation", "Text_from_UTF_8_Blob"})),
            vm.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {source_code_blob}))
        );
        if ((Boolean)vm.MDER_export_qualified(vm.MDER_evaluate(
            vm.MDER_import(new KeyValuePair<String, Object>("Nesting", new String[] {"foundation", "Excuse"})),
            vm.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text })))).Value!)
        {
            System.Console.WriteLine("The requested source code providing file"
                + " [" + sourceCodeFilePath + "] is not well-formed UTF-8 text:"
                + " " + maybe_source_code_text.ToString());
            return;
        }
        System.Console.WriteLine("Debug: maybe_source_code_text = [" + maybe_source_code_text + "]");
        MDER_Any maybe_source_code = vm.MDER_evaluate(
            vm.MDER_import(new KeyValuePair<String, Object>("Nesting", new String[] {"foundation", "MDPT_Parsing_Unit_Text_to_Any"})),
            vm.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {maybe_source_code_text }))
        );
        System.Console.WriteLine("Debug: maybe_source_code = [" + maybe_source_code + "]");

        // Temporary Executor test.
        MDER_Any sum = vm.MDER_evaluate(
            vm.MDER_import(new KeyValuePair<String, Object>("Nesting", new String[] {"foundation", "Integer_plus"})),
            vm.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {27, 39}))
        );
        System.Console.WriteLine("Test data: sum = [" + sum + "]");
        MDER_Any that = vm.MDER_import(new KeyValuePair<String, Object>("Tuple", new Object[] {27, 39}));
        System.Console.WriteLine("Test data: that = [" + that + "]");
        MDER_Any that_too = vm.MDER_import(new KeyValuePair<String, Object>("Tuple", new Dictionary<String, Object>()
            {{"\u0014", 25}, {"aye", "zwei"}, {"some one", "other two"}}
        ));
        System.Console.WriteLine("Test data: that_too = [" + that_too + "]");
        MDER_Any the_other = vm.MDER_import("Fr ⊂ ac 💩 ti ÷ on");
        System.Console.WriteLine("Test data: the_other = [" + the_other + "]");
        MDER_Any f0 = vm.MDER_import(014.0M);
        System.Console.WriteLine("Test data: f0 = [" + f0 + "]");
        MDER_Any f1 = vm.MDER_import(2.3M);
        System.Console.WriteLine("Test data: f1 = [" + f1 + "]");
        MDER_Any f2 = vm.MDER_import(02340233.23402532000M);
        System.Console.WriteLine("Test data: f2 = [" + f2 + "]");
        MDER_Any f3 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(13, 5)));
        System.Console.WriteLine("Test data: f3 = [" + f3 + "]");
        MDER_Any f4 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(27, 6)));
        System.Console.WriteLine("Test data: f4 = [" + f4 + "]");
        MDER_Any f5 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(35, -41)));
        System.Console.WriteLine("Test data: f5 = [" + f5 + "]");
        MDER_Any f6 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(new BigInteger(-54235435432), new BigInteger(32543252))));
        System.Console.WriteLine("Test data: f6 = [" + f6 + "]");
        MDER_Any f7 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(26, 13)));
        System.Console.WriteLine("Test data: f7 = [" + f7 + "]");
        MDER_Any f8 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5, 1)));
        System.Console.WriteLine("Test data: f8 = [" + f8 + "]");
        MDER_Any f9 = vm.MDER_import(new KeyValuePair<String, Object>("Fraction", new KeyValuePair<Object, Object>(5, -1)));
        System.Console.WriteLine("Test data: f9 = [" + f9 + "]");
    }
}
