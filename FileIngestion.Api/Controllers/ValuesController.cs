using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileIngestion.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FileIngestion.Api.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{filename}")]
        public async Task<int> Get(string filename)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=batchingestion;AccountKey=9cGevc1Xm2uIUuPLO3eU451ZSJvMwQRCOB9DeMYPjKDo0ON1R/7U1UssbypTs329sHxHENJ7kuOTHuWANHW52Q==;EndpointSuffix=core.windows.net");

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mdap-batch-files");

            // Retrieve reference to a blob named "test.csv"
            CloudBlockBlob blockBlobReference = container.GetBlockBlobReference(filename);

            using (var memoryStream = new MemoryStream())
            {
                //downloads blob's content to a stream
                await blockBlobReference.DownloadToStreamAsync(memoryStream);

                var rows = DataParser.ParserFile(memoryStream);

                return rows.Count();
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
