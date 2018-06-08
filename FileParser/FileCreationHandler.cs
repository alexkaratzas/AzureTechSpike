
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FileIngestion.Data.CosmosDb;
using FileIngestion.Data.TableStorage;
using FileIngestion.Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FileParser
{
    public static class FileCreationHandler
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private static readonly IDocumentDbRepository<DataRow> Repository = new DocumentDbRepository<DataRow>();
        private static readonly ITableStorageRepository LogRepository = new TableStorageRepository();

        [FunctionName("BlobTriggerFileCreation")]
        public static async Task Run([BlobTrigger("mdap-batch-files/{name}")]
            Stream myBlob, string name, TraceWriter log)
        {
            var guid = Guid.NewGuid();

            try
            {
                Log($"Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes", guid, log);

                var startTime = DateTime.Now;

                var stopwatch = Stopwatch.StartNew();
                var rows = DataParser.ParserFile(myBlob).ToArray();

                var elapsed = stopwatch.ElapsedMilliseconds;
                Log($"Finished parsing {rows.Count()} row(s) in {elapsed} ms", guid, log);

                var rowArray = rows.ToArray();

                List<Task> tasks = new List<Task>();
                foreach (var dataRow in rowArray)
                {
                    var task = Repository.CreateItemAsync(dataRow);
                    tasks.Add(task);
                }

                //rowArray.Select(Repository.CreateItemAsync).ToArray();

                Log("Finished calling inserts. Awaiting insertion tasks...", guid, log);

                await Task.WhenAll(tasks);

                stopwatch.Stop();

                Log($"Finished inserting {rows.Count()} row(s) in {stopwatch.ElapsedMilliseconds - elapsed} ms", guid, log);

                Log($"Total execution time {stopwatch.ElapsedMilliseconds} ms", guid, log);

                LogEvent("Parse and insert finished", startTime, DateTime.Now, name);

                //var response = await HttpClient.GetAsync($"https://fileingestionapi.azurewebsites.net/api/values/{name}");

                //var data = await response.Content.ReadAsStringAsync();

                //Log($"Response Data: {data}", guid, log);
            }
            catch (Exception ex)
            {
                log.Error($"{GetLogPrefix(guid)} Error occured during ingestion API invocation: {ex.Message}", ex);
                throw;
            }
        }

        private static void Log(string message, Guid guid, TraceWriter log)
        {
            log.Info($"{GetLogPrefix(guid)} {message}");
        }

        private static void LogEvent(string description, DateTime startTime, DateTime endTime, string filename)
        {
            LogRepository.CreateItemAsync(new TestIngestionLog
            {
                Description = description,
                StartTime = startTime,
                EndTime = endTime,
                Filename = filename,
                Timestamp = DateTimeOffset.Now,
                TestType = "SaveOneByOne",
                RowKey = Guid.NewGuid().ToString()
            }).Wait();
        }

        private static string GetLogPrefix(Guid guid)
        {
            return
                $"Blob trigger function instance ID: {Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")} Run ID: {guid}";
        }
    }
}
