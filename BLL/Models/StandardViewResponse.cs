using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL.Models
{
    public enum ViewResponseStatus
    {
        Ok,
        Error
    }
    public class StandardViewResponse<T>
    {
        public StandardViewResponse(T value, string? errorMessage = null)
        {
            Value = value;
            ErrorMessage = errorMessage;
        }

      
        public string? ErrorMessage { get; set; }
        public T Value { get; set; }
        public ViewResponseStatus Status
        {
            get
            {
                return string.IsNullOrEmpty(ErrorMessage) ? ViewResponseStatus.Ok : ViewResponseStatus.Error;
            }
        }


    }
}
