using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using LogParsing.LogParsers;

namespace DataParallelismTask.LogParsers
{
    internal class ParallelLogParser : ILogParser
    {
        private readonly FileInfo _file;
        private readonly Func<string, string?> _tryGetIdFromLine;

        public ParallelLogParser(FileInfo file, Func<string, string?> tryGetIdFromLine)
        {
            _file = file;
            _tryGetIdFromLine = tryGetIdFromLine;
        }

        public string[] GetRequestedIdsFromLogFile()
        {
            var lines = File.ReadLines(_file.FullName);
            var result = new ConcurrentStack<string>();
            Parallel.ForEach(lines, l =>
            {
                var log = _tryGetIdFromLine(l);
                if (log is not null)
                    result.Push(log);
            });

            return result.ToArray();
        }
    }
}