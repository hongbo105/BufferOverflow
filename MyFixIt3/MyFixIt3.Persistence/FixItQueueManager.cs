using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Queue;
using MyFixIt.Logging;
using Newtonsoft.Json;

namespace MyFixIt3.Persistence
{
    public class FixItQueueManager : IFixItQueueManager
    {
        // private CloudQueue _queue;
        private readonly string _queueName = "myfixitqueue";
        private readonly string _storageConnectionstring = "DefaultEndpointsProtocol=https;AccountName=botest;AccountKey=3qLZdu8FX41IkL7FKJHcXtWqJx2+6vFXPIme5A9hSpYKPfggcpN6O0yH3ZXLJH0z80vOecAH4SDxEc/uw9/27w==";
        private Logger _logger = new Logger();
        private FixItTaskRepository _taskRepo = new FixItTaskRepository();
        public FixItQueueManager()
        {
            
        }

        public async Task SendMessageAsync(FixItTask newTask)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                var storageAccount = CloudStorageAccount.Parse(_storageConnectionstring);
                var queueClient = storageAccount.CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference(_queueName);
                await queue.CreateIfNotExistsAsync();
                var taskInJson = JsonConvert.SerializeObject(newTask);
                var taskMessage = new CloudQueueMessage(taskInJson);
                await queue.AddMessageAsync(taskMessage);
                watch.Stop();
                _logger.TraceApi("Queue Manager", "SendMessageAsync", watch.Elapsed, "Message added to queue: {0}", newTask);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in SendMessgeAsync, task:{0}", newTask);   
                throw;
            }
        }

        public async Task ProcessMessageAsync(CancellationToken token)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                var storageAccount = CloudStorageAccount.Parse(_storageConnectionstring);
                var queueClient = storageAccount.CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference(_queueName);
                await queue.CreateIfNotExistsAsync();

                while (!token.IsCancellationRequested)
                {
                    var taskInJson = await queue.GetMessageAsync();
                    if (taskInJson != null)
                    {
                        var task = JsonConvert.DeserializeObject<FixItTask>(taskInJson.AsString);
                        await _taskRepo.CreateAsync(task);
                        await queue.DeleteMessageAsync(taskInJson);
                        watch.Stop();
                        _logger.TraceApi("Queue Manager", "SendMessageAsync", watch.Elapsed, "Task to process: {0}",
                            task);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in ProcessMessageAsync");
                throw;
            }
        }
    }
}
