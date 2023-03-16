using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DocsBot;

public static class FindDoc
{
    [FunctionName("FindDocs")]
    public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        // Read input data from query parameters
        
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        var partitionKey = data.Type;

        var storageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString");

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
        var tableClient = storageAccount.CreateCloudTableClient();

        var table = tableClient.GetTableReference("Docs");


        TableQuery<Docs> query = new TableQuery<Docs>().Where(
            TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("DocIndex", QueryComparisons.Equal, data.Index.ToString())));

        var entities = new List<Docs>();

        TableContinuationToken continuationToken = null;
        do
        {
            TableQuerySegment<Docs> queryResult = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
            continuationToken = queryResult.ContinuationToken;
            entities.AddRange(queryResult.Results);
        } while (continuationToken != null);

        if (entities.Count > 0)
        {
            return new OkObjectResult(entities);
        }

        return new NotFoundResult();
    }
}

