public class SubmitJobDto
{
    public string PrinterId { get; set; }
    public PrintData PrintData { get; set; }
    public PrinterSettings Settings { get; set; }
    public PayloadSchema BeforePrintCallback { get; set; }
    public PayloadSchema PrintFailureCallback { get; set; }
}
