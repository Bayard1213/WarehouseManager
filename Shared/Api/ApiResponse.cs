using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseManager.Shared.Api
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ApiResponse(string message, T? data = default, bool success = true)
        {
            Success = success;
            Message = message;
            Data = data;
        }

    }
    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse(string message, bool success = true)
            : base(message, null, success)
        {
        }
    }
}
