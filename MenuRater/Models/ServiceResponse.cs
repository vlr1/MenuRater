namespace MenuRater.Models
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }

        public ServiceResponse(T data, string message = null)
        {
            Data = data;
            Success = true;
            Message = message ?? "Success";
        }

        public ServiceResponse(string message)
        {
            Success = false;
            Message = message;
        }
    }
}
