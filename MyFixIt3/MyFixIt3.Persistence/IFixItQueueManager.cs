using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFixIt3.Persistence
{
    public interface IFixItQueueManager
    {
        Task SendMessageAsync(FixItTask newTask);
        Task ProcessMessageAsync();
    }
}
