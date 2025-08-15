namespace WarehouseManager.Server.Exceptions
{
    public class ValidationException : BusinessException
    {
        public ValidationException() { }

        public ValidationException(string message) : base(message) { }

        public ValidationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
