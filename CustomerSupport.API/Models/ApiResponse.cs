namespace CustomerSupport.API.Models
{
    // ProductResponseDTO
    // ApiResponse<List<ProductResponseDTO>>
    public sealed class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public IReadOnlyCollection<string> Errors { get; set; } = [];
        public string? TraceId { get; set; }

        public static ApiResponse<T> SuccessResponse(
            T? data,
            string message = "Request completed successfully.")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> FailureResponse(
            string message,
            IReadOnlyCollection<string>? errors = null,
            string? traceId = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? [],
                TraceId = traceId
            };
        }
    }
}
