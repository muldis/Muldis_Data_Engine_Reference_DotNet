using System;
using System.Collections.Generic;
using System.IO;

namespace Muldis.ReferenceEngine.Core
{
    // Muldis.ReferenceEngine.Core.Storage
    // For fetching data from and storing data to the host file system.
    // All data is interpreted here simply in terms of plain octet arrays;
    // it is up to callers to map this as character data where appropriate.
    // For now we assume all files are small enough to fit in memory at
    // once and we don't offer the complexity of supporting streams.
    // WARNING/TODO: Current safety checks or precautions are minimal;
    // the following are practices that should be followed but aren't:
    // 1.  There are race conditions between checking for the prior
    // existence or non-existence of things in the file system and acting
    // on those tests, so accidental file overwriting could happen; the
    // proper fix involves making the test and file usage atomic, which
    // probably requires more piecemeal file operations.
    // 2.  While we try to use a sandbox for file operations, defined by
    // File_System_Base_Dir which presumably comes originally from a
    // top level application config file or command line option, we
    // currently don't check for well-formedness or that there aren't the
    // likes of ".." or "\\" or "C:\" etc in child paths, which would
    // allow escape from the sandbox thanks to Path.Combine() etc.

    internal class Storage
    {
        private String m_file_system_base_dir;

        internal Storage(String file_system_base_dir)
        {
            if (!Directory.Exists(file_system_base_dir))
            {
                throw new DirectoryNotFoundException(
                    "Expected file system base directory"
                    + "[" + file_system_base_dir + "] doesn't exist.");
            }
            m_file_system_base_dir = file_system_base_dir;
        }

        internal String File_System_Base_Dir()
        {
            return m_file_system_base_dir;
        }

        private String build_file_path(String[] file_child_path)
        {
            List<String> parts = new List<String>() { m_file_system_base_dir };
            parts.AddRange(file_child_path);
            return Path.Combine(parts.ToArray());
        }

        internal Boolean File_Exists(String[] file_child_path)
        {
            return File.Exists(build_file_path(file_child_path));
        }

        internal Byte[] Fetch_Existing_File(String[] file_child_path)
        {
            String file_path = build_file_path(file_child_path);
            if (!File.Exists(file_path))
            {
                throw new FileNotFoundException("Can't fetch content of file at"
                    + "[" + file_path + "]; that file doesn't exist.");
            }
            return File.ReadAllBytes(file_path);
        }

        internal void Store_Not_Existing_File(String[] file_child_path, Byte[] file_content)
        {
            String file_path = build_file_path(file_child_path);
            if (File.Exists(file_path))
            {
                // Important for safety; WriteAllBytes overwrites an existing file,
                // although there is still the race conditions.
                throw new IOException("Can't store content as new file at"
                    + "[" + file_path + "]; another file already exists there.");
            }
            // TODO: If appropriate, create intermediate directories first;
            // the WriteAllBytes documentation doesn't say if it does so.
            File.WriteAllBytes(file_path, file_content);
        }
    }
}
