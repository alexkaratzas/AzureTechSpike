using System;

namespace FileIngestion.Domain
{
    public class DataRow
    {
        public string ObjectId { get; set; }
        public DateTime Timestamp { get; set; }
        public string TimeResolution { get; set; }
        public string Hypothesis { get; set; }
        public int Value { get; set; }
        public int Constant { get; set; }
        public string Key => ObjectId;
    }
}