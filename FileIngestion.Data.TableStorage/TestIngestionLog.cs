using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace FileIngestion.Data.TableStorage
{
    public class TestIngestionLog : TableEntity
    {
        public string Filename { get; set; }
        public string TestType { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMs => (EndTime - StartTime).Milliseconds;
        public string Description { get; set; }
    }
}