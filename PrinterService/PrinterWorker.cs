using Devices;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Context;
using Smart.DataTransferObject;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PrinterService
{
    /// <summary>
    /// One physical hardware
    /// </summary>
    /// <param name="hubContext"></param>
    /// <param name="settings"></param>
    public class PrinterWorker : IPrinterWorker
    {
        private readonly IHubContext<PrintHub> _hubContext;

        public SemaphoreSlim Throtller { get; private set; }
        public string PrinterId { get; private set; }
        /// <summary>
        /// Physical hardware - S3110 or XID
        /// </summary>
        private AbstractPrinter Printer { get; set; }
        public PrinterSettings Settings { get; set; }
        private PrintJob _job;

        private PrinterWorker() { }

        public PrinterWorker(string printerId, IHubContext<PrintHub> hubContext, PrinterSettings settings) : this()
        {
            PrinterId = printerId;
            _hubContext = hubContext;
            Settings = settings;
            Throtller = new SemaphoreSlim(1, 1);

            UpdatePrinter(settings);

            _hubContext.Clients.All.SendAsync(Constants.JobStatusUpdate, $"Readpoint: {printerId} with IP {settings.ServerIP} added to printer pool");
        }

        /// <summary>
        /// Creates a printer object either for XID or S3110
        /// </summary>
        /// <param name="settings"></param>
        public void UpdatePrinter(PrinterSettings settings)
        {
            //Make this method thread safe.
            //If printing is in progress, wait for updating the worker.

            //if (settings.HardwareType == "XID8300DS")
            //    Printer = new XID8300();
            //else
            //    Printer = new S3110();

            Printer.NotifyProgress += Printer_NotifyProgress;

            Printer.Settings = settings;
        }

        async public Task<PrintJob> Print(PrintJob job)
        {
            if (Throtller.CurrentCount > 0)
                await Throtller.WaitAsync(TimeSpan.Zero);

            _job = job;
            //Adding context again to ensure it's thread safe
            using (LogContext.PushProperty(Constants.UserId, _job.UserId))
            using (LogContext.PushProperty(Constants.ConnectionId, _job.ConnectionId))
            using (LogContext.PushProperty(Constants.JobPrinterId, _job.PrinterId))
            using (LogContext.PushProperty(Constants.JobId, _job.JobId))
            using (LogContext.PushProperty(Constants.Method, nameof(Printer.StartPrinting)))
            {
                try
                {
                    job.Status = "In Progress";
                    await UpdateJobStatus(job); // Notify client

                    Func<string, PayloadSchema, Task<Response<bool>>> beforePrintAction = BeforePrintAction;
                    Func<string, PayloadSchema, Task<Response<bool>>> failAction = FailAction;

                    Log.Debug($"Job status {{{Constants.JobStatus}}}", job.Status);
                    await Printer.StartPrinting(job, beforePrintAction, failAction);
                    job.Status = "Completed";
                    Log.Debug($"Job status {{{Constants.JobStatus}}}", job.Status);
                    await UpdateJobStatus(job); // Notify client
                }
                catch (Exception ex)
                {
                    Log.Error($"{{@{Constants.Exception}}}", ex);
                    job.Status = ex.Message;
                    await UpdateJobStatus(job); // Notify client
                }
                finally
                {
                    Throtller.Release();
                }
            }
            return job;
        }

        private void Printer_NotifyProgress(object? sender, string e)
        {
            if (_job == null)
                _hubContext.Clients.All.SendAsync(Constants.JobStatusUpdate, $"{sender.ToString()}:{e}");
            else
                _hubContext.Clients.User(_job.UserId).SendAsync(Constants.JobStatusUpdate, $"{sender.ToString()}:{e}");
        }

        private async Task<Response<bool>> BeforePrintAction(string tagNumber, PayloadSchema schema)
        {
            var body = schema.Body.Replace("{0}", tagNumber);
            Log.Debug($"{{{Constants.Endpoint}}},{{@{Constants.Body}}}", schema.Endpoint, body);

            var result = await SequreController.CallWebhook(schema.Endpoint, schema.Headers, body);

            Log.Debug($"{{@{Constants.BeforePrintActionResult}}}", result);
            return result;
        }

        private async Task<Response<bool>> FailAction(string tagNumber, PayloadSchema schema)
        {
            var url = string.Format(schema.Endpoint, tagNumber);
            Log.Debug($"{{{Constants.Endpoint}}}", url);

            var result = await SequreController.CallWebhook(url, schema.Headers, schema.Body);
            Log.Debug($"{{@{Constants.FailActionResult}}}", result);
            return result;
        }

        async private Task UpdateJobStatus(PrintJob job)
        {
            Log.Debug($"{{@{Constants.JobStatus}}}", job.Status);

            await _hubContext.Clients.User(job.UserId).SendAsync(Constants.JobStatusUpdate, job);
        }

        public async Task<PrinterStatus> GetPrinterStatus()
        {
            using (LogContext.PushProperty(Constants.Method, nameof(GetPrinterStatus)))
            {
                return await Printer.GetPrinterStatus();
            }
        }
    }

    public interface IPrinterWorker
    {
        Task<PrintJob> Print(PrintJob job);
    }
}