using Ixs.DNA;
using MessagePack;
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
        public async Task<(short ResultStatus, T ResultData)> DeserializeObjectFromBinFileAsync<T>(string path)
        {
            // Normalize path.
            path = NormalizedPath(path);

            // Resolve to absolute path.
            path = ResolvePath(path);

            // Lock the task.
            return await AsyncLock.LockResultAsync(path, async () =>
            {
                // Run the synchronous file access as a new task.
                return await CoreDI.Task.Run(() =>
                {
                    short resultStatus = 0;
                    T resultObj = default;

                    try
                    {
                        byte[] data;
                        using (FileStream stream = File.Open(path, FileMode.Open))
                        {
                            byte[] buffer = new byte[16 * 1024];
                            using (MemoryStream ms = new MemoryStream())
                            {
                                int read;
                                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                                    ms.Write(buffer, 0, read);
                                data = ms.ToArray();
                            }
                        }

                        resultObj = MessagePackSerializer.Deserialize<T>(data);
                    }
                    catch (FileNotFoundException)
                    {
                        resultStatus = 1;
                    }
                    catch (IOException)
                    {
                        resultStatus = 1;
                    }
                    catch (Exception)
                    {
                        resultStatus = 2;
                    }

                    return (resultStatus, resultObj);
                });
            });
        }

        /// <inheritdoc/>
        public async Task<short> SerializeObjectToBinFileAsync<T>(T obj, string path)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            // Normalize path.
            path = NormalizedPath(path);

            // Resolve to absolute path.
            path = ResolvePath(path);

            // Lock the task.
            return await AsyncLock.LockResultAsync(path, async () =>
            {
                // Run the synchronous file access as a new task.
                return await CoreDI.Task.Run(() =>
                {
                    short result = 0;

                    // Check if the file exists (create one if does not exist)...
                    TryCreatePath(path);

                    try
                    {
                        byte[] data = MessagePackSerializer.Serialize(obj);
                        using (FileStream stream = File.Open(path, FileMode.Create))
                        {
                            // Write the data to the file, byte by byte.
                            for (int i = 0; i < data.Length; ++i)
                                stream.WriteByte(data[i]);

                            // Set the stream position to the beginning of the file.
                            stream.Seek(0, SeekOrigin.Begin);

                            // Read and verify the data.
                            for (int i = 0; i < stream.Length; ++i)
                                if (data[i] != stream.ReadByte())
                                {
                                    result = 2;
                                    break;
                                }
                        }
                    }
                    catch (FileNotFoundException)
                    {
                        result = 1;
                    }
                    catch (IOException)
                    {
                        result = 1;
                    }
                    catch (Exception)
                    {
                        result = 2;
                    }

                    return result;
                });
            });
        }

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
            await AsyncLock.LockAsync(path, async () =>
            {
                // Run the synchronous file access as a new task.
                await CoreDI.Task.Run(() =>
                {
                    // Check if the file exists...
                    TryCreatePath(path);

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
            await AsyncLock.LockAsync(path, async () =>
            {
                // Run the synchronous file access as a new task.
                await CoreDI.Task.Run(() =>
                {
                    // Check if the file exists...
                    TryCreatePath(path);

                    // Write the log message to file.
                    using (var writer = (TextWriter)new StreamWriter(File.Open(path, append ? FileMode.Append : FileMode.Create)))
                        for (int i = 0; i < lines.Count; ++i)
                            writer.WriteLine(lines[i]);
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

        #region Private Helpers

        /// <summary>
        /// Creates directories (including file - if any) if does not exist
        /// </summary>
        /// <param name="path">The path</param>
        private void TryCreatePath(string path)
        {
            // Check if the file exists...
            if (!File.Exists(path))
            {
                // Create it
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                var f = File.Create(path);
                f.Close();
            }
        }

        #endregion
    }
}
