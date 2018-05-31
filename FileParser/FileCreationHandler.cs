
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FileIngestion.Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FileParser
{
    public static class FileCreationHandler
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        [FunctionName("BlobTriggerFileCreation")]
        public static async Task Run([BlobTrigger("mdap-batch-files/{name}")]Stream myBlob, string name, TraceWriter log)
        {
            var guid = Guid.NewGuid();

            Log($"Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes", guid, log);

            var stopwatch = Stopwatch.StartNew();
            var rows = DataParser.ParserFile(myBlob);

            Log($"Finished parsing {rows.Count()} row(s) in {stopwatch.ElapsedMilliseconds} ms", guid, log);

            stopwatch.Stop();

            var response = await HttpClient.GetAsync("https://fileingestionapi.azurewebsites.net/api/values");

            var data = await response.Content.ReadAsStringAsync();

            Log($"Response Data: {data}", guid, log);
        }

        private static void Log(string message, Guid guid, TraceWriter log)
        {
            log.Info($"{GetLogPrefix(guid)} {message}");
        }

        private static string GetLogPrefix(Guid guid)
        {
            return
                $"Blob trigger function instance ID: {Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")} Run ID: {guid}";
        }
    }
}
