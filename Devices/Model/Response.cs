using System.Collections.Generic;

namespace Smart.DataTransferObject
{
    public class Response<T>
    {
        public KeyValuePair<int, string> Error { get; set; }

        public int Count { get; set; }

        public T Entity { get; set; }

        public Response()
        {
            Error = new KeyValuePair<int, string>(0, "Success!!");
        }

        public Response(T entity):this()
        {
            Entity = entity;
        }
    }
}
