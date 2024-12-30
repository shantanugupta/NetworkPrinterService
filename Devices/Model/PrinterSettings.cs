public class PrinterSettings
{
    public string ServerIP { get; set; }
    public string PrinterName { get; set; }
    public string PrinterPort { get; set; }
    public string PrinterConType { get; set; }
    public string ReaderPower { get; set; }
    public string HardwareType { get; set; }
    public string ProductFamily { get; set; }
    public string DisplayLineName { get; set; }
    public string BrandedHardwareType { get; set; }

    public override string ToString()
    {
        return $"{ServerIP}:{PrinterName}:{PrinterPort}:{PrinterConType}:{ReaderPower}:{HardwareType}:{ProductFamily}:{DisplayLineName}:{BrandedHardwareType}".ToLower();
    }
}
