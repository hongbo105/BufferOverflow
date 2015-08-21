using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using MyFixIt3.Persistence;
using MyFixIt.Logging;
using Autofac;

namespace MyFixItWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        private IContainer container;
        private ILogger logger;

        public override void Run()
        {
            logger.Information("MyFixItWorkerRole is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            var builder = new ContainerBuilder();

            builder.RegisterType<FixItQueueManager>().As<IFixItQueueManager>();
            builder.RegisterType<FixItTaskRepository>().As<IFixItTaskRepository>().SingleInstance();
            builder.RegisterType<Logger>().As<ILogger>().SingleInstance();

            container = builder.Build();

            logger = container.Resolve<ILogger>();

            bool result = base.OnStart();

            logger.Information("MyFixItWorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            logger.Information("MyFixItWorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            logger.Information("MyFixItWorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                logger.Information("MyFixItWorkerRole RunAsync");

                using (var scope = container.BeginLifetimeScope())
                {
                    Trace.TraceInformation("MyFixItWorkerRole RunAsync");
                    var queueManager = scope.Resolve<IFixItQueueManager>();
                    await queueManager.ProcessMessageAsync(cancellationToken);
                    logger.Information("Working");
                    await Task.Delay(1000);
                }
            }
        }
    }
}
