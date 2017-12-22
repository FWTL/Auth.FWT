using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Auth.FWT.Core.Helpers
{
    public static class FileHelpers
    {
        public static IEnumerable<string> ReadLines(string path, long? limit = null)
        {
            string line;

            using (StreamReader reader = new StreamReader(path))
            {
                long counter = 0;
                if (limit.HasValue)
                {
                    while (limit.Value > counter++ && (line = reader.ReadLine()) != null)
                    {
                        yield return line;
                    }
                }
                else
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        yield return line;
                    }
                }
            }
        }

        public static string ReturnFileText(string path)
        {
            return File.ReadAllText(path);
        }

        public static IEnumerable<string> RunSeedSQLFiles(string pathToDictionary)
        {
            var paths = Directory.EnumerateFiles(pathToDictionary, "*.sql", SearchOption.AllDirectories).Where(x => !x.Contains("old_"));
            foreach (var path in paths)
            {
                string sql = File.ReadAllText(path);
                MarkAsOld(path);
                yield return sql;
            }
        }

        public static void WriteLines(IEnumerable<string> collection, string path, bool append = false)
        {
            using (StreamWriter writer = new StreamWriter(path, append))
            {
                foreach (var line in collection)
                {
                    writer.WriteLine(line);
                }
            }
        }

        private static void MarkAsOld(string path)
        {
            var oldFileName = "old_" + Path.GetFileName(path);
            File.Move(path, Path.GetDirectoryName(path) + "/" + oldFileName);
        }
    }
}
