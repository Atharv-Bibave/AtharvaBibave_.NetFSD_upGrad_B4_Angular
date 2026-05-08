namespace EMS.WebAPI.DTOs
{
    public sealed class ApiResponse<T>
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public T? Data { get; init; }
        public IEnumerable<string>? Errors { get; init; }
    }

    
    // Non-generic factory and convenience type.
    // Use ApiResponse.Ok(data) / ApiResponse.Fail(...) to build typed responses.
    public sealed class ApiResponse
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public IEnumerable<string>? Errors { get; init; }

        // ── Non-generic (no-data) factories

        public static ApiResponse Ok(string message = "Success")
            => new() { Success = true, Message = message };

        public static ApiResponse Created(string message = "Created successfully.")
            => new() { Success = true, Message = message };

        public static ApiResponse Fail(string message, IEnumerable<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors };

        // ── Generic (with-data) factories 
        public static ApiResponse<T> Ok<T>(T data, string message = "Success")
            => new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Created<T>(T data, string message = "Created successfully.")
            => new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Fail<T>(string message, IEnumerable<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors };
    }
}
