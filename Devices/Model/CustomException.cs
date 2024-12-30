public class PrinterException : Exception
{
    public PrinterException() : base() { }

    public PrinterException(string errorMessage) : base(errorMessage)
    {
    }

    public PrinterException(string errorMessage, Exception exception) : base(errorMessage, exception) { }
}
