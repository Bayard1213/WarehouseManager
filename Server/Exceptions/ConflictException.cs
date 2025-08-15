namespace WarehouseManager.Server.Exceptions
{
    //409
    public class ConflictException : BusinessException
    {
        public ConflictException() { }

        public ConflictException(string message) : base(message) { }

        public ConflictException(string message, Exception innerException) : base(message, innerException) { }
    }
}
