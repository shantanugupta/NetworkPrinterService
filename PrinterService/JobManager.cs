using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrinterService
{
    public class JobManager(IHubContext<PrintHub> hubContext) : IJobManager
    {
        private readonly IHubContext<PrintHub> _hubContext = hubContext;
        private readonly PrinterManager _printerManager = new(hubContext);
        private readonly ConcurrentDictionary<string, PrintJob> _jobs = new();

        public async Task<PrintJob> CreateJob(SubmitJobDto input, string connectionId, string userId, string correlationId)
        {
            var jobId = Guid.NewGuid().ToString();

            using (LogContext.PushProperty(Constants.Method, nameof(CreateJob)))
            using (LogContext.PushProperty(Constants.JobId, jobId))
            using (LogContext.PushProperty(Constants.UserId, userId))
            using (LogContext.PushProperty(Constants.ConnectionId, connectionId))
            using (LogContext.PushProperty(Constants.PrinterId, input.PrinterId))
            {
                var printerSettings = input.Settings;

                var worker = await PrinterManager.AddPrinter(input.PrinterId, printerSettings);

                var job = new PrintJob
                {
                    JobId = jobId,
                    PrinterId = input.PrinterId,
                    PrintData = input.PrintData,
                    ConnectionId = connectionId,
                    UserId = userId,
                    Status = "Pending",
                    BeforePrintCallback = input.BeforePrintCallback,
                    PrintFailureCallback = input.PrintFailureCallback,
                    CorrelationId = correlationId
                };

                _jobs[job.JobId] = job;
                return job;
            }
        }

        public async Task ProcessJobAsync()
        {
            var workers = PrinterManager.GetAllPrinters();

            workers.Values.Where(w => w.Throtller.CurrentCount > 0
                                        && _jobs.Any(j => j.Value.PrinterId == w.PrinterId)
                                )
                .ToArray()
                .AsParallel()
                .ForAll(async printer => await ExecuteWorker(printer));
        }

        private async Task ExecuteWorker(PrinterWorker printer)
        {
            using (LogContext.PushProperty(Constants.PrinterId, printer.PrinterId))
            using (LogContext.PushProperty(Constants.Ip, printer.Settings.ServerIP))
            using (LogContext.PushProperty(Constants.Port, printer.Settings.PrinterPort))
            using (LogContext.PushProperty(Constants.PrinterName, printer.Settings.PrinterName))
            using (LogContext.PushProperty(Constants.Method, nameof(ExecuteWorker)))
            {
                await _hubContext.Clients.All.SendAsync(Constants.JobStatusUpdate
                , $"{printer.PrinterId} is running. Throttler Count: {printer.Throtller.CurrentCount}");

                PrintJob job = null;

                // Attempt to get a job for the current printer
                foreach (var jobKey in _jobs.Keys)
                {
                    if (_jobs.TryGetValue(jobKey, out var candidateJob) &&
                        candidateJob.PrinterId == printer.PrinterId)
                    {
                        //Check if this printer is free to take next job
                        //if yes, take a job out of a queue and process
                        if (_jobs.TryRemove(jobKey, out job))
                        {
                            Log.Debug($"{{{Constants.Action}}},{{{Constants.JobId}}},{{{Constants.JobStatus}}}", nameof(_jobs.TryRemove), job.JobId, job.Status);
                            break; // Exit the loop with a valid job
                        }
                    }
                }

                if (job == null)
                {
                    await _hubContext.Clients.All.SendAsync(Constants.JobStatusUpdate, $"{printer.PrinterId} available");
                    return;
                }

                using (LogContext.PushProperty(Constants.UserId, job.UserId))
                using (LogContext.PushProperty(Constants.ConnectionId, job.ConnectionId))
                using (LogContext.PushProperty(Constants.JobPrinterId, job.PrinterId))
                using (LogContext.PushProperty(Constants.JobId, job.JobId))
                using (LogContext.PushProperty(Constants.CorrelationId, job.CorrelationId))
                {
                    try
                    {

                        Log.Debug($"{{{Constants.Action}}},{{{Constants.JobStatus}}}", nameof(printer.Print), job.Status);
                        await printer.Print(job);

                        job.Status = "Completed";
                        Log.Debug($"{{{Constants.Action}}},{{{Constants.JobStatus}}}", nameof(printer.Print), job.Status);

                    }
                    catch (Exception ex)
                    {
                        Log.Error($"{{@{Constants.Exception}}}", ex);
                        job.Status = "Failed";
                    }
                    finally
                    {
                        UpdateJobStatus(job);
                    }
                }
            }
        }

        public async Task CancelJobAsync(string jobId)
        {
            using (LogContext.PushProperty(Constants.Method, nameof(CancelJobAsync)))
            using (LogContext.PushProperty(Constants.JobId, jobId))
            {
                if (_jobs.TryRemove(jobId, out var job))
                {
                    job.Status = "Removed";
                    Log.Debug($"{{{Constants.JobId}}},{{{Constants.JobStatus}}}", jobId, job.Status);
                    await UpdateJobStatus(job);
                }
            }
        }

        public async Task CancelAllJobs(string printerId)
        {
            using (LogContext.PushProperty(Constants.Method, nameof(CancelAllJobs)))
            using (LogContext.PushProperty(Constants.PrinterId, printerId))
            {
                if (string.IsNullOrEmpty(printerId))
                {
                    _jobs.Clear();
                    Log.Debug("All jobs removed");
                }
                else
                {
                    foreach (var job in _jobs)
                    {
                        if (job.Value.PrinterId == printerId)
                            _jobs.TryRemove(job);
                    }
                    Log.Debug("Jobs removed for printer");
                }
            }
        }

        public async Task ChangePrinterAsync(string jobId, string newPrinterId)
        {
            using (LogContext.PushProperty(Constants.Method, nameof(GetSubmittedJobs)))
            using (LogContext.PushProperty(Constants.JobId, jobId))
            using (LogContext.PushProperty(Constants.NewPrinterId, newPrinterId))
            {
                if (_jobs.TryGetValue(jobId, out var job))
                {
                    Log.Debug($"Printer change {{{Constants.OldPrinter}}},{{{Constants.NewPrinterId}}}", job.PrinterId, newPrinterId);
                    job.PrinterId = newPrinterId;
                    await UpdateJobStatus(job);
                }
            }
        }

        private async Task UpdateJobStatus(PrintJob job)
        {
            Log.Debug($"Status: {{{Constants.Status}}}", job.Status);
            await _hubContext.Clients.User(job.UserId).SendAsync(Constants.JobStatusUpdate, job);
        }

        public async Task<Dictionary<string, PrintJob>> GetSubmittedJobs(string printerId = null)
        {
            using (LogContext.PushProperty(Constants.Method, nameof(GetSubmittedJobs)))
            using (LogContext.PushProperty(Constants.PrinterId, printerId))
            {
                var jobs = _jobs.Where(j => j.Value.PrinterId == printerId || string.IsNullOrEmpty(printerId)).ToDictionary();
                Log.Debug($"Jobs count {{{Constants.JobCount}}}", jobs.Count());
                return jobs;
            }
        }

        public async Task<Dictionary<string, string>> GetPrinterStatus(string printerId)
        {
            Dictionary<string, string> printersStatus = new Dictionary<string, string>();
            using (LogContext.PushProperty(Constants.Method, nameof(GetPrinterStatus)))
            {
                var printers = PrinterManager.GetAllPrinters();

                foreach (var printer in printers.Where(p => p.Key == printerId || string.IsNullOrEmpty(printerId)))
                {
                    var status = await printer.Value.GetPrinterStatus();
                    var attrib = status.GetType().GetField(status.ToString())
                        .GetCustomAttributes(typeof(DisplayAttribute), false)[0] as DisplayAttribute;

                    printersStatus.Add(printer.Value.PrinterId, attrib.Description);
                }

                return printersStatus;
            }
        }
    }
}