using System;
using System.Collections.Generic;
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

            return text.Split(new[] {Environment.NewLine}, StringSplitOptions.None).Select(ParseLine).ToArray();
        }

        private static DataRow ParseLine(string line)
        {
            var fields = line.Split(new[] {";"}, StringSplitOptions.None);

            return new DataRow
            {
                ObjectId = fields[0],
                Timestamp = DateTime.Parse(fields[1]),
                TimeResolution = fields[2],
                Hypothesis = fields[3],
                Value = Convert.ToInt32(fields[4]),
                Constant = Convert.ToInt32(fields[5])
            };
        }
    }

    public class DataRow
    {
        public string ObjectId { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimeResolution { get; set; }
        public string Hypothesis { get; set; }
        public int Value { get; set; }
        public int Constant { get; set; }
    }
}
