using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types

namespace AzureStorageTableSample
{
    class Program
    {
        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <remarks>https://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-tables/</remarks>
        static void Main(string[] args)
        {
            // Parse the connection string and return a reference to the storage account.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("Historian"); //_등 특수기호 허용않됨

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            // Create the batch operation.
            TableBatchOperation batchOperation = new TableBatchOperation();

            // Create a new customer entity.
            GenericValue<bool> val1 = new GenericValue<bool>("DEV1.Connected", Guid.NewGuid().ToString());
            val1.Value = true;
            table.Execute(TableOperation.Insert(val1));

            GenericValue<string> val2 = new GenericValue<string>("DEV1.Version", Guid.NewGuid().ToString());
            val2.Value = "v1.3";
            table.Execute(TableOperation.Insert(val2));

            GenericValue<int> val3 = new GenericValue<int>("DEV1.DevNo", Guid.NewGuid().ToString());
            val3.Value = (new Random()).Next(0, 31);
            table.Execute(TableOperation.Insert(val3));

            #region 데이타 조회
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<GenericValue<int>> query
                = new TableQuery<GenericValue<int>>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "DEV1.DevNo"));

            // Print the fields for each customer.
            foreach (GenericValue<int> entity in table.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1}\t{2}", entity.PartitionKey, entity.RowKey, entity.Value);
            }
            #endregion

            Console.ReadLine();
        }
    }

    public class GenericValue<T> : TableEntity
    {
        public GenericValue(string lastName, string firstName) //키
        {
            this.PartitionKey = lastName;
            this.RowKey = firstName;
        }

        public GenericValue() { }

        public T Value { get; set; }

    }
}
