using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;
using todoalejo.Functions.Entities;

namespace todoalejo.Functions
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run([TimerTrigger("0 */2 * * * *")] TimerInfo myTimer,
        [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
        ILogger log)
        {
            log.LogInformation($"Deleting completed tasking at: {DateTime.Now}");
            string filter = TableQuery.GenerateFilterConditionForBool("IsCompleted", QueryComparisons.Equal, true);
            TableQuery<TodoEntity> query = new TableQuery<TodoEntity>().Where(filter);
            TableQuerySegment<TodoEntity> completeTodos = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            int deleted = 0;

            foreach (TodoEntity completeTodo in completeTodos)
            {
                await todoTable.ExecuteAsync(TableOperation.Delete(completeTodo));
                deleted++;
            }

            log.LogInformation($"Deleted {deleted} items at {DateTime.Now}");
        }
    }
}
