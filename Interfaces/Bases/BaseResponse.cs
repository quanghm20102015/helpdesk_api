namespace Interfaces.Base
{
    /// <summary>
    /// BaseResponse
    /// </summary>
    /// <typeparam name="T"> Type of response status </typeparam>
    public class BaseResponse<T>
    {
        /// <summary>
        /// ResponseMessage
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Status of response 
        /// </summary>
        public T Status { get; set; }
    }
}
