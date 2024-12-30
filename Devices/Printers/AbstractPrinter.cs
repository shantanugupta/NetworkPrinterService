using Smart.DataTransferObject;
using System.Xml;

namespace Devices
{
    public abstract class AbstractPrinter 
    {
        public  PrinterSettings Settings;

        #region External methods for client applications

        public abstract event EventHandler<string> NotifyProgress;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="beforePrint">PrintJob, tagNumber</param>
        /// <param name="afterPrint">PrintJob, tagNumber</param>
        /// <returns></returns>
        public abstract Task<Response<bool>> StartPrinting(PrintJob item
            , Func<string, PayloadSchema, Task<Response<bool>>> beforePrint
            , Func<string, PayloadSchema, Task<Response<bool>>> afterPrint
            );

        #endregion

        #region Internal abstract methods

        protected abstract string Connect(string printerName);

        protected abstract string Disconnect();

        protected abstract PrinterStatus GetStatus();

        public abstract Task<PrinterStatus> GetPrinterStatus();

        protected abstract void GetInfo();

        protected abstract PrinterResponse RejectCard();

        protected abstract string ReadTag();

        protected abstract void Notify(string command, string message);

        #endregion

        #region Reusable printer calling methods

        /// <summary>
        /// This method should initialize SdkWrapper object
        /// </summary>
        /// <returns></returns>
        //protected abstract bool InitialiseDriver();

        protected XmlNode GetNodeAtPath(string xmlString, string xPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            return doc.SelectSingleNode(xPath);
        }
        
        #endregion
    }
}
