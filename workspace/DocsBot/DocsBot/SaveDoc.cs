using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace DocsBot;

public static class SaveDoc
{
    [FunctionName("SaveDocs")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Create Record");

        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);
        var partitionKey = data.Type;
        var rowKey = Guid.NewGuid().ToString();

        var storageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString");

        // Create CloudTableClient object
        var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
        var tableClient = storageAccount.CreateCloudTableClient();

        // Get reference to the table
        var table = tableClient.GetTableReference("Docs");

        // Create the table entity
        var entity = new Docs(partitionKey, rowKey)
        {
            DocIndex = data.Index,
            DocName = data.Name,
            DocLink = data.Link
        };

        // Create the TableOperation object for the insert
        var insertOperation = TableOperation.Insert(entity);

        // Execute the insert operation
        await table.ExecuteAsync(insertOperation);

        return new OkResult();
    }
}