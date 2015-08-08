using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace MyFixIt3.Persistence
{
    class FixItContext : DbContext
    {
        public FixItContext() : base("name = myFixItDB")
        {
        }

        public DbSet<FixItTask> FixItTasks { get; set; }
    }

    public class EFConfiguration : DbConfiguration
    {
        public EFConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}
