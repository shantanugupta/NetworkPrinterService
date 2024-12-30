using System.Collections.Generic;

public class PayloadSchema
{
    public string Endpoint { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public string Body { get; set; }
    public string RequestType { get; set; }
}
