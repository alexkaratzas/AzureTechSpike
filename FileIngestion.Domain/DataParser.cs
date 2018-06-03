using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FileIngestion.Domain
{
    public static class DataParser
    {
        public static IEnumerable<DataRow> ParserFile(Stream stream)
        {
            stream.Position = 0;

            string text;
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }

            return text.Split(new[] {Environment.NewLine}, StringSplitOptions.None)
                .Select(ParseLine)
                .Where(t => t != null)
                .ToArray();
        }

        private static DataRow ParseLine(string line)
        {
            var fields = line.Split(new[] {","}, StringSplitOptions.None).ToArray();
            if (fields.Length != 6)
            {
                return null;
            }

            if (!DateTime.TryParseExact(fields[1], "dd.MM.yyyy HH:mm", CultureInfo.CurrentCulture, DateTimeStyles.None, out var date))
            {
                throw new Exception($"{fields[1]} is not a valid date");
            }

            return new DataRow
            {
                ObjectId = fields[0],
                Timestamp = date,
                TimeResolution = fields[2],
                Hypothesis = fields[3],
                Value = Convert.ToInt32(fields[4]),
                Constant = Convert.ToInt32(fields[5])
            };
        }
    }
}
