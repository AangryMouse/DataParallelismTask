using System;
using System.IO;
using System.Linq;
using LogParsing.LogParsers;

namespace DataParallelismTask.LogParsers
{
    internal class PLinqLogParser : ILogParser
    {
        private readonly FileInfo _file;
        private readonly Func<string, string?> _tryGetIdFromLine;

        public PLinqLogParser(FileInfo file, Func<string, string?> tryGetIdFromLine)
        {
            _file = file;
            _tryGetIdFromLine = tryGetIdFromLine;
        }

        public string[] GetRequestedIdsFromLogFile()
        {
            return File.ReadLines(_file.FullName)
                .AsParallel()
                .Select(_tryGetIdFromLine)
                .Where(l => l is not null)
                .ToArray()!;
        }
    }
}