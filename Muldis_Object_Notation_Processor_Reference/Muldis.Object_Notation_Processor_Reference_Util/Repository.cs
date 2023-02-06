using System;
using System.IO;

[assembly: CLSCompliant(true)]

namespace Muldis.Object_Notation_Processor_Reference_Util;

public class Repository
{
    private Logger logger;

    public Repository(Logger logger)
    {
        this.logger = logger;
    }

    // This method generally only will throw an exception if it was invoked with invalid arguments,
    // or if it seems to have a coding error from not anticipating a particular file type.
    // Otherwise, it returns true if it succeeded in processing its argument node,
    // and it returns false if it failed in processing its argument node.
    // This method implements a recursive operation over a regular file or a directory containing
    // either regular files or directories.  The method's "resume_when_failure" Boolean argument
    // determines whether the entire recursive operation is halted immediately if any individual
    // node fails to be processed (false argument), or if it attempts to keep going and process the
    // remainder of the nodes (true argument).
    public Boolean process_file_or_dir_recursive(IProcessor processor,
        FileSystemInfo path_in, FileSystemInfo path_out, Boolean resume_when_failure)
    {
        if (!((path_in is FileInfo && path_out is FileInfo)
            || (path_in is DirectoryInfo && path_out is DirectoryInfo)))
        {
            throw new ArgumentException(
                "process_file_or_dir_recursive(): routine argument path_in " + path_in.FullName
                    + " is not of the same class (FileInfo or DirectoryInfo)"
                    + " as argument path_out " + path_out.FullName + ".");
        }
        // We expect that whatever calls this routine has already normalized the paths to absolute.
        // We don't need to validate this in .NET because FileSystemInfo objects are known to
        // become fully qualified at the moment of creation.
        // For safety we want to ensure the input tree and output tree don't overlap.
        // Logically this also means that neither of them can be the filesystem root.
        if (path_in.FullName.StartsWith(path_out.FullName)
            || path_out.FullName.StartsWith(path_in.FullName))
        {
            this.logger.failure("Fatal: Can't input from file or dir at path " + path_in.FullName
                + " or can't output to new file or dir at path " + path_out.FullName
                + " because either those two paths are the same or one is contained in the other.");
            return false;
        }
        // We intentionally do not support following symbolic links for safety/simplicity reasons.
        if (path_in.LinkTarget != null)
        {
            this.logger.failure("Fatal: Can't input from file or dir at path " + path_in.FullName
                + " because that is a symbolic link and we don't support following those.");
            return false;
        }
        if (path_out.LinkTarget != null)
        {
            this.logger.failure("Fatal: Can't output to file or dir at path " + path_out.FullName
                + " because that is a symbolic link and we don't support following those.");
            return false;
        }
        // Gracefully handle user-specified paths not existing, though we still have to later handle
        // the case of it disappearing between this check and when we actually try to read it.
        if (!path_in.Exists)
        {
            this.logger.failure("Fatal: Can't input from file or dir at path " + path_in.FullName
                + " because no file or dir exists there.");
            return false;
        }
        // For safety we don't ever want to overwrite existing files or directories;
        // if one wants to re-run a processing task they must remove previous run's output first,
        // or include a date stamp in the output path that changes per run, or something.
        if (path_out.Exists)
        {
            this.logger.failure("Fatal: Can't output to new file/dir at path " + path_out.FullName
                + " because some other file or dir already exists there.");
            return false;
        }
        // For simplicity we require that the immediate parent dir of the named output path
        // already exists, and we will not be creating any nonexisting ancestor dirs.
        DirectoryInfo path_out_parent = path_out is FileInfo
            ? ((FileInfo)path_out).Directory
            : ((DirectoryInfo)path_out).Parent;
        if (!path_out_parent.Exists)
        {
            this.logger.failure("Fatal: Can't output to new file/dir at path " + path_out.FullName
                + " because its parent dir " + path_out_parent.FullName + " doesn't exist.");
            return false;
        }
        // For simplicity or to allow flexibility, we ignore whether or not input files are
        // writable or executable or hidden, as that shouldn't matter to us, probably.
        if (path_in is FileInfo)
        {
            // Gracefully handle user-specified paths not being readable to us, we still have to
            // later handle the case of us having lost privilege when we actually try to read it.
            // Or not, since .NET doesn't seem to provide an easy "is readable" method.
            this.logger.notice("Starting the process from file at path "
                + path_in.FullName + " to file at path " + path_out.FullName);
            // Open existing file for input in octet streaming mode, should fail if doesn't exist.
            // Open nonexisting file for output in octet streaming mode, fail if already exists.
            try
            {
                using (FileStream stream_in
                    = ((FileInfo)path_in).Open(FileMode.Open, FileAccess.Read))
                {
                    using (FileStream stream_out
                        = ((FileInfo)path_out).Open(FileMode.CreateNew, FileAccess.Write))
                    {
                        // Note that Stream.ReadByte()
                        // returns one of 0..255 when there is another octet
                        // and it returns -1 when there is none / the end of stream was passed.
                        processor.process(stream_in, stream_out);
                    }
                }
            }
            catch (IOException e)
            {
                this.logger.failure("Fatal: An IOException occurred while attempting to process"
                    + " from file at path " + path_in.FullName
                    + " to file at path " + path_out.FullName + "; details: " + e);
                return false;
            }
            this.logger.notice("Finished the process from file at path "
                + path_in.FullName + " to file at path " + path_out.FullName);
            return true;
        }
        // For simplicity or to allow flexibility, we ignore whether or not input dirs are
        // writable or executable or hidden, as that shouldn't matter to us, probably.
        if (path_in is DirectoryInfo)
        {
            // Gracefully handle user-specified paths not being readable to us, we still have to
            // later handle the case of us having lost privilege when we actually try to read it.
            // Or not, since .NET doesn't seem to provide an easy "is readable" method.
            this.logger.notice("Starting the process from dir at path "
                + path_in.FullName + " to dir at path " + path_out.FullName);
            // Create nonexisting output directory, fail if already exists.
            try
            {
                ((DirectoryInfo)path_out).Create();
            }
            catch (IOException e)
            {
                this.logger.failure("Fatal: An IOException occurred while attempting to create"
                    + " output dir " + path_out.FullName + "; details: " + e);
                return false;
            }
            this.logger.notice("Created new empty output dir at path " + path_out.FullName);
            // Iterate children of input dir and make corresponding children of output dir.
            // DirectoryInfo.GetFileSystemInfos() does NOT include "." or ".." etc in child list.
            try
            {
                // Each child_path_in is a fully-qualified absolute path of the child already.
                foreach (FileSystemInfo child_path_in
                    in ((DirectoryInfo)path_in).GetFileSystemInfos())
                {
                    // Get the unqualified file name of the child, which is the last element.
                    String child_common_unqualified_path = child_path_in.Name;
                    // Append unqualified child file name to fully qualified path of output parent.
                    FileSystemInfo child_path_out = child_path_in is FileInfo
                        ? new FileInfo(Path.Combine(
                            path_out.FullName, child_common_unqualified_path))
                        : new DirectoryInfo(Path.Combine(
                            path_out.FullName, child_common_unqualified_path));
                    // Recurse to actually process the child.
                    Boolean was_child_success = process_file_or_dir_recursive(
                        processor, child_path_in, child_path_out, resume_when_failure);
                    // If we should abort all processing as soon as any child node fails, do so.
                    if (!was_child_success && !resume_when_failure)
                    {
                        return false;
                    }
                    // Else if we should continue on with trying to process the rest of the child
                    // nodes regardless of whether the child we just tried had failed, then do so.
                }
            }
            catch (IOException e)
            {
                this.logger.failure("Fatal: An IOException occurred while attempting to process"
                    + " from dir at path " + path_in.FullName
                    + " to dir at path " + path_out.FullName + "; details: " + e);
                return false;
            }
            this.logger.notice("Finished the process from dir at path "
                + path_in.FullName + " to dir at path " + path_out.FullName);
            return true;
        }
        throw new NotSupportedException(
            "process_file_or_dir_recursive(): unexpected situation; path_in " + path_in.FullName
                + " is neither a symbolic link or regular file or directory.");
    }
}
