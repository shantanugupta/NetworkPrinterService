using System;
using System.Collections.Generic;

public class PrintJob
{
    public string CorrelationId { get; set; }

    // Unique identifier for each job, assigned when the job is created.
    public string JobId { get; set; }

    // ID of the printer assigned to this job (e.g., "Printer1", "Printer2").
    public string PrinterId { get; set; }

    // Template to be used for printing. This allows different formats, layouts, etc.
    public string Template { get; set; }

    // The actual data to be printed (could be text, binary data, or an XML/JSON structure).
    public PrintData PrintData { get; set; }

    // Client connection ID (from SignalR), so the server knows which client to notify.
    public string ConnectionId { get; set; }

    // User ID guid for identifying single user. Notification will be sent to clients using this user id
    public string UserId { get; set; }

    // Current status of the print job, such as "Pending", "In Progress", "Completed", "Cancelled".
    public string Status { get; set; }

    // Timestamp when the job was created, useful for tracking and reporting.
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Additional properties or metadata about the print job can be added as needed.
    public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

    // A flag to mark if the job is cancelled
    public bool IsCancelled { get; set; } = false;

    public PayloadSchema BeforePrintCallback { get; set; }
    public PayloadSchema PrintFailureCallback { get; set; }
}
