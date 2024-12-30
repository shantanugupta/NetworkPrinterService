using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Context;
using System.Threading;
using System.Threading.Tasks;

namespace PrinterService
{
    public class Worker(IHubContext<PrintHub> hubContext, IJobManager jobManager) : BackgroundService
    {
        private readonly IHubContext<PrintHub> _hubContext = hubContext;
        private readonly IJobManager _jobManager = jobManager;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information($"Starting {{{Constants.Method}}}", nameof(ExecuteAsync));
            //Implement your background task logic here
            while (!stoppingToken.IsCancellationRequested)
            {
                using (LogContext.PushProperty(Constants.Hub, nameof(PrintHub)))
                {
                    await _jobManager.ProcessJobAsync();
                    // Background processing, e.g., managing print jobs
                    await Task.Delay(3000, stoppingToken); // Example delay
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Perform any cleanup if needed
            await base.StopAsync(cancellationToken);
        }
    }
}