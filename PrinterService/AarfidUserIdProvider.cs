using Microsoft.AspNetCore.SignalR;

namespace PrinterService
{
    public class AarfidUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            // Extract the GUID from query string or claims
            return connection.User?.FindFirst("UserGuid")?.Value ??
                   connection.GetHttpContext()?.Request.Query["userGuid"];
        }
    }
}
