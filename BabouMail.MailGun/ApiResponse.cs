using System.Collections.Generic;
using System.Linq;

namespace BabouMail.MailGun
{
    public class ApiResponse
    {
        public ApiResponse()
        {
            Errors = new List<ApiError>();
        }

        public bool Success => !Errors.Any();
        public List<ApiError> Errors { get; set; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T Data { get; set; }
    }

    public class ApiError
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string PropertyName { get; set; }
    }
}
