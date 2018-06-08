using System;
using FileIngestion.Data.TableStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileIngestion.Test
{
    [TestClass]
    public class TableStorageTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var repo = new TableStorageRepository();

            repo.CreateItemAsync(new TestIngestionLog
            {
                Description = "Test",
                StartTime = DateTime.Now,
                EndTime = DateTime.Now,
                Filename = "test.test",
                Timestamp = DateTimeOffset.Now,
                PartitionKey = "Test",
                TestType = "Test",
                RowKey = Guid.NewGuid().ToString()
            }).Wait();
        }
    }
}
