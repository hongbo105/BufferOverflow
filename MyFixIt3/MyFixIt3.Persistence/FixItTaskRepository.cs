using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MyFixIt.Logging;

namespace MyFixIt3.Persistence
{
    public class FixItTaskRepository : IFixItTaskRepository, IDisposable
    {
        private FixItContext _context = new FixItContext();
        private readonly Logger _logger = new Logger();

        public async Task CreateAsync(FixItTask taskToAdd)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                _context.FixItTasks.Add(taskToAdd);
                await _context.SaveChangesAsync();
                watch.Stop();
                _logger.Information("Add a task takes {0}, Task to add: {1}", watch.Elapsed, taskToAdd);
            }

            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to add a FixIt task");
                throw;
            }

        }

        public async Task DeleteAsync(FixItTask taskToRemove)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                _context.FixItTasks.Remove(taskToRemove);
                await _context.SaveChangesAsync();
                watch.Stop();
                _logger.TraceApi("MyFixIt3", "FixItTaskRepository.RemoveAsync", watch.Elapsed, "Removed task {0}",
                    taskToRemove);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to remove a FixIt task");
                throw;
            }
        }

        public async Task<FixItTask> FindTaskByIdAsync(int id)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                var task = await _context.FixItTasks.FindAsync(id);
                watch.Stop();
                _logger.TraceApi("MyFixIt3", "FixItTaskRepository.FindTaskByIdAsync", watch.Elapsed,
                    "Find task by id {0}",
                    id);
                return task;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to find a FixIt task by id");
                throw;
            }
        }

        public async Task DeleteTaskByIdAsync(int id)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                var fixIt = await FindTaskByIdAsync(id);
                await DeleteAsync(fixIt);
                watch.Stop();
                _logger.TraceApi("MyFixIt3", "FixItTaskRepository.FindTaskByIdAsync", watch.Elapsed,
                    "Find task by id {0}",
                    id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to find a FixIt task by id");
                throw;
            }
        }

        public async Task UpdateAsync(FixItTask taskToSave)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                _context.Entry(taskToSave).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                watch.Stop();
                _logger.TraceApi("SQL database", "UpdateAsync", watch.Elapsed, "taskToSave={0}", taskToSave);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in UpdateAsync(tasktosave={0}", taskToSave);
                throw;
            }
        }

        public async Task<List<FixItTask>> FindOpenTasksByOwnerAsync(string userName)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                var tasks = await _context.FixItTasks
                    .Where(t => t.Owner == userName && t.IsDone == false)
                    .OrderByDescending(t => t.FixItTaskId)
                    .ToListAsync();
                watch.Stop();
                _logger.TraceApi("SQL Database", "FindOpenTasksByOwnerAsync", watch.Elapsed, "username={0}", userName);
                return tasks;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in FindOpenTasksByOwnerAsync, username = {0}", userName);
                throw;
            }
        }

        public async Task<List<FixItTask>> FindTasksByCreatorAsync(string userName)
        {
            var watch = Stopwatch.StartNew();

            try
            {
                var tasks = await _context.FixItTasks
                    .Where(t => t.CreatedBy == userName)
                    .OrderByDescending(t => t.FixItTaskId)
                    .ToListAsync();
                watch.Stop();
                _logger.TraceApi("SQL Database", "FindTasksByCreatorAsync", watch.Elapsed, "username={0}", userName);
                return tasks;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in FindTasksByCreatorAsync, username = {0}", userName);
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool dispose)
        {
            if(dispose == true)
            {
                if(_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
    }
}
