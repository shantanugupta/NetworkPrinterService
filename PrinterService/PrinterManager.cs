using Devices;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Context;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace PrinterService
{
    public class PrinterManager
    {
        private static IHubContext<PrintHub> _hubContext;
        private static readonly ConcurrentDictionary<string, PrinterWorker> _printers = new();

        public static ConcurrentDictionary<string, PrinterWorker> GetAllPrinters()
        {
            return _printers;
        }

        public PrinterManager(IHubContext<PrintHub> hubContext)
        {
            _hubContext = hubContext;
        }

        // Static method to get a printer without an instance of PrinterManager
        public async static Task<PrinterWorker> AddPrinter(string printerId, PrinterSettings settings)
        {
            using (LogContext.PushProperty(Constants.Method, nameof(AddPrinter)))
            {
                PrinterWorker worker = _printers.GetOrAdd(printerId, p =>
                {
                    Log.Information($"{{{Constants.Action}}},{{@{Constants.Settings}}}", nameof(_printers.GetOrAdd), settings);
                    return new PrinterWorker(printerId, _hubContext, settings);
                });

                if (worker.Settings.ToString() != settings.ToString())
                {
                    Log.Information($"{{{Constants.Action}}},{{@{Constants.Settings}}}", nameof(worker.UpdatePrinter), settings);
                    worker.UpdatePrinter(settings);
                }

                return worker;
            }
        }

        // Static method to get a printer without an instance of PrinterManager
        public static PrinterWorker GetPrinter(string printerId)
        {
            PrinterWorker printer = _printers.ContainsKey(printerId) ? _printers[printerId] : null;
            if(printer == null)
            {
                Log.Debug($"{{{Constants.Method}}}", nameof(GetPrinter));
            }

            return printer;
        }

        // Example instance method
        public bool IsPrinterAvailable(string printerId)
        {
            // Check if the printer is available (for example, not currently processing a job)
            return true; // Update with actual availability logic
        }
    }
}