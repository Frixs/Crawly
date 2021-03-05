using Ixs.DNA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Handles reading/writing and querying the file system.
    /// </summary>
    public class BaseFileManager : IFileManager
    {
        /// <inheritdoc/>
        public async Task WriteTextToFileAsync(string text, string path, bool append = false)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            // Normalize path.
            path = NormalizedPath(path);

            // Resolve to absolute path.
            path = ResolvePath(path);

            // Lock the task.
            await AsyncLock.LockAsync(nameof(WriteTextToFileAsync) + path, async () =>
            {
                // Run the synchronous file access as a new task.
                await CoreDI.Task.Run(() =>
                {
                    // Write the log message to file.
                    using (var writer = (TextWriter)new StreamWriter(File.Open(path, append ? FileMode.Append : FileMode.Create)))
                        writer.Write(text);
                });
            });
        }

        /// <inheritdoc/>
        public async Task WriteLinesToFileAsync(List<string> lines, string path, bool append = false)
        {
            if (lines == null)
                throw new ArgumentNullException(nameof(lines));

            // Normalize path.
            path = NormalizedPath(path);

            // Resolve to absolute path.
            path = ResolvePath(path);

            // Lock the task.
            await AsyncLock.LockAsync(nameof(WriteTextToFileAsync) + path, async () =>
            {
                // Run the synchronous file access as a new task.
                await CoreDI.Task.Run(() =>
                {
                    // Write the log message to file.
                    using (var writer = (TextWriter)new StreamWriter(File.Open(path, append ? FileMode.Append : FileMode.Create)))
                        for (int i = 0; i < lines.Count; ++i)
                            writer.WriteLine(lines[0]);
                });
            });
        }

        /// <inheritdoc/>
        public long LineCount(string path)
        {
            long lineCount = 0;

            // Normalize path.
            path = NormalizedPath(path);

            // Resolve to absolute path.
            path = ResolvePath(path);

            // Count lines.
            using (var reader = new StreamReader(path))
            {
                while (reader.ReadLine() != null)
                    lineCount++;
            }

            return lineCount;
        }

        /// <inheritdoc/>
        public List<string> ReadLines(string path)
        {
            var lines = new List<string>();

            // Normalize path.
            path = NormalizedPath(path);

            // Resolve to absolute path.
            path = ResolvePath(path);

            // Get lines.
            using (var reader = new StreamReader(path))
            {
                var line = reader.ReadLine();
                var c = 0;
                while (line != null)
                {
                    lines.Add(line);
                    line = reader.ReadLine();
                    c++;
                }
            }

            return lines;
        }

        /// <inheritdoc/>
        public bool IsInUse(FileInfo file)
        {
            FileStream stream = null;
            bool isOpened = false;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                // The file is unavailable because it is:
                //  - Still being written to.
                //  - Or being processed by another thread.
                //  - Or does not exist (has already been processed).
                isOpened = true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            // File is not in use.
            return isOpened;
        }

        /// <inheritdoc/>
        public string NormalizedPath(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return path?.Replace('/', '\\').Trim();
            else
                return path?.Replace('\\', '/').Trim();
        }

        /// <inheritdoc/>
        public string ResolvePath(string path)
        {
            // Resolve the path to absolute.
            return Path.GetFullPath(path);
        }
    }
}
