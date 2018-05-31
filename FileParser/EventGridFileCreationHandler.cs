using System;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Host;

namespace FileParser
{
    public static class EventGridFileCreationHandler
    {
        [FunctionName(("BlobTriggerEventGrid"))]
        public static void Run([EventGridTrigger] EventGridEvent eventGridEvent, TraceWriter log)
        {
            log.Info($"Event Grid trigger function instance ID: {Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")} processed event: {eventGridEvent.Data}");
        }
    }
}