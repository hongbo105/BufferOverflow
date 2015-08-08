using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFixIt3.Persistence
{
    public interface IFixItTaskRepository
    {
        Task CreateAsync(FixItTask taskToAdd);
        Task DeleteAsync(FixItTask taskToRemove);
        Task<FixItTask> FindTaskByIdAsync(int id);
        Task DeleteTaskByIdAsync(int id);
        Task UpdateAsync(FixItTask taskToSave);
        Task<List<FixItTask>> FindOpenTasksByOwnerAsync(string userName);
        Task<List<FixItTask>> FindTasksByCreatorAsync(string userName); 
    }
}
