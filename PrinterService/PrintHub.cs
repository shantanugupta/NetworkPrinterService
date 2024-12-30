using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrinterService
{
    public class PrintHub(IJobManager jobManager) : Hub<IPrintHub>
    {
        private readonly IJobManager _jobManager = jobManager;

        public async Task<Dictionary<string, PrintJob>> GetSubmittedJobs(string printerId, string correlationId = null)
        {
            correlationId = string.IsNullOrEmpty(correlationId) ? GetOrGenerateCorrelationId() : correlationId;
            using (LogContext.PushProperty(Constants.CorrelationId, correlationId))
            using (LogContext.PushProperty(Constants.ConnectionId, Context.ConnectionId))
            using (LogContext.PushProperty(Constants.UserId, Context.UserIdentifier))
            using (LogContext.PushProperty(Constants.PrinterId, printerId))
            {
                var jobs = await _jobManager.GetSubmittedJobs(printerId);
                return jobs;
            }
        }

        public async Task<PrintJob> SubmitPrintJob(SubmitJobDto input, string correlationId)
        {
            //Before submitting a job to a queue, check if printer is already a part of printer pool
            //if not, add this printer to a pool
            //Printer settings need not be carried in PrinterJob
            correlationId = string.IsNullOrEmpty(correlationId) ? GetOrGenerateCorrelationId():correlationId;

            using (LogContext.PushProperty(Constants.ConnectionId, Context.ConnectionId))
            using (LogContext.PushProperty(Constants.UserId, Context.UserIdentifier))
            using (LogContext.PushProperty(Constants.CorrelationId, correlationId))
            {
                PrintJob job = await _jobManager.CreateJob(input, Context.ConnectionId, Context.UserIdentifier, correlationId);
                return job; // Return JobId to client
            }
        }

        public async Task CancelJob(string jobId, string correlationId = null)
        {
            correlationId = string.IsNullOrEmpty(correlationId) ? GetOrGenerateCorrelationId() : correlationId;
            using (LogContext.PushProperty(Constants.CorrelationId, correlationId))
            using (LogContext.PushProperty(Constants.ConnectionId, Context.ConnectionId))
            using (LogContext.PushProperty(Constants.UserId, Context.UserIdentifier))
            {
                await _jobManager.CancelJobAsync(jobId);
            }
        }

        public async Task CancelAllJobs(string printerId = null, string correlationId = null)
        {
            correlationId = string.IsNullOrEmpty(correlationId) ? GetOrGenerateCorrelationId() : correlationId;
            using (LogContext.PushProperty(Constants.CorrelationId, correlationId))
            using (LogContext.PushProperty(Constants.ConnectionId, Context.ConnectionId))
            using (LogContext.PushProperty(Constants.UserId, Context.UserIdentifier))
            using (LogContext.PushProperty(Constants.PrinterId, printerId))
            {
                await _jobManager.CancelAllJobs(printerId);
            }
        }

        public async Task ChangePrinter(string jobId, string newPrinterId, string correlationId = null)
        {
            correlationId = string.IsNullOrEmpty(correlationId) ? GetOrGenerateCorrelationId() : correlationId;
            using (LogContext.PushProperty(Constants.CorrelationId, correlationId))
            using (LogContext.PushProperty(Constants.ConnectionId, Context.ConnectionId))
            using (LogContext.PushProperty(Constants.UserId, Context.UserIdentifier))
            using (LogContext.PushProperty(Constants.JobId, jobId))
            using (LogContext.PushProperty(Constants.PrinterId, newPrinterId))
            { 
                await _jobManager.ChangePrinterAsync(jobId, newPrinterId);
            }
        }

        private string GetOrGenerateCorrelationId()
        {
            // Try to get correlation ID from the client context
            if (Context.Items.TryGetValue(Constants.CorrelationId, out var id))
                return id.ToString();

            // Generate a new one if not available
            var correlationId = Guid.NewGuid().ToString();
            Context.Items[Constants.CorrelationId] = correlationId;
            return correlationId;
        }

        public async Task<Dictionary<string, string>> GetPrinterStatus(string printerId, string correlationId = null)
        {
            correlationId = string.IsNullOrEmpty(correlationId) ? GetOrGenerateCorrelationId() : correlationId;

            using (LogContext.PushProperty(Constants.ConnectionId, Context.ConnectionId))
            using (LogContext.PushProperty(Constants.UserId, Context.UserIdentifier))
            using (LogContext.PushProperty(Constants.CorrelationId, correlationId))
            using (LogContext.PushProperty(Constants.PrinterId, printerId))
            using (LogContext.PushProperty(Constants.Method, nameof(GetPrinterStatus)))
            {
                var status = await _jobManager.GetPrinterStatus(printerId);

                Log.Debug($"{{${Constants.PrinterStatus}}}", status);
                return status;
            }
        }
    }

    public interface IPrintHub
    {
        Task<string> SubmitPrintJob(SubmitJobDto input);

        Task CancelJob(string jobId, string correlationId =null);

        Task CancelAllJobs(string correlationId = null);

        Task ChangePrinter(string jobId, string newPrinterId, string correlationId = null);

        Task<Dictionary<string, PrintJob>> GetSubmittedJobs(string printerId, string correlationId = null);

        Task<Dictionary<string, string>> GetPrinterStatus(string printerId, string correlationId = null);
    }
}