namespace Bocchify_Api.Contracts
{
    public class BaseResponse<T>
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
        public T? Data { get; set; }
    }
}