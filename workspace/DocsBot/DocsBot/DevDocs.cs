using Microsoft.WindowsAzure.Storage.Table;

namespace DocsBot;

public class Docs : TableEntity
{
    public Docs(string partitionKey, string rowKey)
    {
        this.PartitionKey = partitionKey;
        this.RowKey = rowKey;
    }

    public Docs()
    {
    }

    public string DocIndex { get; set; }
    public string DocLink { get; set; }
    public string DocName { get; set; }
}