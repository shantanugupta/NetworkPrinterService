using System.Xml.Serialization;

namespace Devices
{
    internal class ApiResponse
    {
        public int ResponseCode { get; set; }
        public string ResponseData { get; set; }
        public string ResponseMsg { get; set; }
    }

    [XmlRoot("Printer")]
    public class PrinterResponse
    {
        [XmlElement("PrinterName")]
        public string PrinterName { get; set; }

        [XmlElement("Status")]
        public string Status { get; set; }

        [XmlIgnore]
        public int ErrorCode { get; set; }

        [XmlElement("ErrorCode")]
        public string ErrorCodeString
        {
            get { return ErrorCode.ToString(); }
            set
            {
                if (int.TryParse(value, out int code))
                {
                    ErrorCode = code;
                }
                else
                {
                    ErrorCode = -1; // Or some other default/error value
                }
            }
        }
    }


}
