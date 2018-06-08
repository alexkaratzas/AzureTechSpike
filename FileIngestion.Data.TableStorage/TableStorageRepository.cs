using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace FileIngestion.Data.TableStorage
{
    public interface ITableStorageRepository
    {
        Task CreateItemAsync<T>(T item) where T:TableEntity;
    }

    public class TableStorageRepository : ITableStorageRepository
    {
        public Task CreateItemAsync<T>(T item) where T:TableEntity
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse
                ("DefaultEndpointsProtocol=https;AccountName=batchingestion;AccountKey=9cGevc1Xm2uIUuPLO3eU451ZSJvMwQRCOB9DeMYPjKDo0ON1R/7U1UssbypTs329sHxHENJ7kuOTHuWANHW52Q==");

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("CosmosTests");

            TableOperation insertOperation = TableOperation.Insert(item);

            return table.ExecuteAsync(insertOperation);
        }
    }
}
