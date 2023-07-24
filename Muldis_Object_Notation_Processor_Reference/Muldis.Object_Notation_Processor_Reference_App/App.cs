using Muldis.Object_Notation_Processor_Reference_Util;
using Muldis.Object_Notation_Processor_Reference_Util.Processor;

[assembly: CLSCompliant(true)]

namespace Muldis.Object_Notation_Processor_Reference_App;

public class App
{
    private enum App_Arg_Name
    {
        task,
        verbose,
        resume,
        @in,
        @out,
    }

    private enum App_Task
    {
        version,
        help,
        duplicate,
        analyze,
        validate,
        format,
        textify,
        untextify,
        blobify,
        unblobify,
    }

    private App()
    {
    }

    public static void Main(String[] raw_app_args)
    {
        task_version();
        Dictionary<App_Arg_Name, String> app_args = normalized_app_args(raw_app_args);
        if (app_args.Count == 0)
        {
            System.Console.WriteLine("Fatal: Task-naming primary app argument is missing.");
            task_help();
            return;
        }
        App_Task task;
        if (!Enum.TryParse<App_Task>(app_args[App_Arg_Name.task], out task))
        {
            System.Console.WriteLine("Fatal: Unrecognized task: " + app_args[App_Arg_Name.task]);
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
            case App_Task.duplicate:
            case App_Task.analyze:
            case App_Task.validate:
            case App_Task.format:
            case App_Task.textify:
            case App_Task.untextify:
            case App_Task.blobify:
            case App_Task.unblobify:
                generic_task_process(task, app_args);
                break;
            default:
                task_help();
                break;
        }
    }

    private static Dictionary<App_Arg_Name, String> normalized_app_args(String[] raw_app_args)
    {
        // The "task" arg is expected to be positional, and the others named.
        // A positional arg does NOT start with "--", a named looks like "--foo=bar" or "--foo".
        Dictionary<App_Arg_Name, String> app_args = new Dictionary<App_Arg_Name, String>();
        if (raw_app_args.Length > 0 && !raw_app_args[0].StartsWith("--"))
        {
            for (Int32 i = 1; i < raw_app_args.Length; i++)
            {
                String raw_app_arg = raw_app_args[i];
                if (raw_app_arg.StartsWith("--"))
                {
                    String arg_name;
                    String arg_value;
                    if (raw_app_arg.Contains("="))
                    {
                        // Named arg format "--foo=bar", the most generic format.
                        Int32 eq_pos = raw_app_arg.IndexOf("=");
                        arg_name = raw_app_arg.Substring(2, eq_pos - 2);
                        arg_value = raw_app_arg.Substring(eq_pos + 1);
                    }
                    else
                    {
                        // Named arg format "--foo", a Boolean where present is true, absent false.
                        arg_name = raw_app_arg.Substring(2);
                        arg_value = null;
                    }
                    App_Arg_Name app_arg_name;
                    if (Enum.TryParse<App_Arg_Name>(arg_name, out app_arg_name))
                    {
                        app_args.Add(app_arg_name, arg_value);
                    }
                    else
                    {
                        System.Console.WriteLine(
                            "Warning: Ignoring unrecognized argument: " + arg_name);
                    }
                }
            }
            // Assign the positional "task" last so it takes precedence over any named one.
            app_args.Add(App_Arg_Name.task, raw_app_args[0]);
        }
        return app_args;
    }

    private static void task_version()
    {
        System.Console.WriteLine("This application is"
            + " Muldis.Object_Notation_Processor_Reference_App.App version 0.1.");
    }

    private static void task_help()
    {
        String APP_NAME = "sh muonp.sh";
        System.Console.WriteLine("Usage:");
        System.Console.WriteLine("  " + APP_NAME + " version");
        System.Console.WriteLine("  " + APP_NAME + " help");
        foreach (String generic_task in new String[] {"duplicate", "analyze", "validate", "format",
            "textify", "untextify", "blobify", "unblobify"})
        {
            System.Console.WriteLine("  " + APP_NAME + " " + generic_task
                + " [--verbose]"
                + " [--resume]"
                + " --in=<input file or directory path>"
                + " --out=<output file or directory path>"
            );
        }
    }

    private static void generic_task_process(
        App_Task task, Dictionary<App_Arg_Name, String> app_args)
    {
        System.Console.WriteLine("This is task " + task + ".");
        IProcessor processor = task switch
        {
            App.App_Task.duplicate => new Duplicate(),
            _ => throw new NotImplementedException(
                "generic_task_process(): task " + task + " is not handled.")
        };
        Boolean verbose = app_args.ContainsKey(App_Arg_Name.verbose);
        Boolean resume_when_failure = app_args.ContainsKey(App_Arg_Name.resume);
        if (!app_args.ContainsKey(App_Arg_Name.@in))
        {
            System.Console.WriteLine(
                "Fatal: Task " + task + ": Missing argument: " + App_Arg_Name.@in);
            return;
        }
        if (!app_args.ContainsKey(App_Arg_Name.@out))
        {
            System.Console.WriteLine(
                "Fatal: Task " + task + ": Missing argument: " + App_Arg_Name.@out);
            return;
        }
        // If a file or dir exists at the "in" path then both the path_in
        // and path_out will be given the kind of FileSystemInfo subclass
        // object that is appropriate for the kind of thing that "in" points to.
        FileSystemInfo path_in = File.Exists(app_args[App_Arg_Name.@in])
            ? new FileInfo(app_args[App_Arg_Name.@in])
            : new DirectoryInfo(app_args[App_Arg_Name.@in]);
        FileSystemInfo path_out = File.Exists(app_args[App_Arg_Name.@in])
            ? new FileInfo(app_args[App_Arg_Name.@out])
            : new DirectoryInfo(app_args[App_Arg_Name.@out]);
        Logger logger = new Logger(Console.Out, verbose ? Console.Out : null);
        Repository repository = new Repository(logger);
        repository.process_file_or_dir_recursive(processor, path_in, path_out, resume_when_failure);
    }
}
