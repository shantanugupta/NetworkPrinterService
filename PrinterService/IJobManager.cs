using System.Collections.Generic;
using System.Threading.Tasks;

namespace PrinterService
{
    public interface IJobManager
    {
        Task<PrintJob> CreateJob(SubmitJobDto input, string connectionId, string userId, string correlationId);
        Task ProcessJobAsync();
        Task CancelJobAsync(string jobId);
        Task CancelAllJobs(string printerId);
        Task ChangePrinterAsync(string jobId, string newPrinterId);
        Task<Dictionary<string, PrintJob>> GetSubmittedJobs(string printerId);
        Task<Dictionary<string, string>> GetPrinterStatus(string printerId);
    }
}