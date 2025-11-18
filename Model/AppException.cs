using System.Net;

namespace Model.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public object? Details { get; }

        public AppException(string message, int statusCode = 400, object? details = null)
            : base(message)
        {
            StatusCode = statusCode;
            Details = details;
        }

        // V?n h? tr? g?i ki?u c? (có HttpStatusCode)
        public AppException(string message, HttpStatusCode statusCode, object? details = null)
            : this(message, (int)statusCode, details) { }
    }
}