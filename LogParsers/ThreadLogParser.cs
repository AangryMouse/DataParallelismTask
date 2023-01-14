using System;
using System.IO;
using System.Linq;
using System.Threading;
using LogParsing.LogParsers;

namespace DataParallelismTask.LogParsers
{
    internal class ThreadLogParser : ILogParser
    {
        private readonly FileInfo _file;
        private readonly Func<string, string?> _tryGetIdFromLine;

        public ThreadLogParser(FileInfo file, Func<string, string?> tryGetIdFromLine)
        {
            _file = file;
            _tryGetIdFromLine = tryGetIdFromLine;
        }

        public string[] GetRequestedIdsFromLogFile()
        {
            var lines = File.ReadAllLines(_file.FullName);
            var lineCount = lines.Length;
            var processorCount = Environment.ProcessorCount;
            var threads = new Thread[processorCount];
            var tasksPerThread = (int)Math.Ceiling((double)lineCount / processorCount);
            for (var i = 0; i < processorCount; ++i)
            {
                var from = tasksPerThread * i;
                var to = Math.Min(tasksPerThread * (i + 1), lineCount);
                threads[i] = new Thread(ParseLogs);

                void ParseLogs()
                {
                    for (var index = from; index < to; ++index)
                        lines[index] = _tryGetIdFromLine(lines[index]);
                }
            }

            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
                thread.Join();
            return lines.Where(l => l is not null)
                .ToArray();
        }
    }
}