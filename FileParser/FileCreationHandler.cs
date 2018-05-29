
using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FileParser
{
    public static class FileCreationHandler
    {
        [FunctionName("BlobTriggerFileCreation")]
        public static void Run([BlobTrigger("mdap-batch-files/{name}")]Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"Blob trigger function instance ID: {Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")} Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }

        [FunctionName(("BlobTriggerEventGrid"))]
        public static void Run([EventGridTrigger]JObject eventGridEvent, TraceWriter log)
        {
            log.Info(eventGridEvent.ToString(Formatting.Indented));
        }
    }
}
