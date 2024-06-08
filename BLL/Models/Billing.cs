using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL.Models
{
    public class Billing
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="You must enter the amount!")]
        public decimal Amount { get; set; }
        public DateTime BillingDate { get; set; }
        [Required(ErrorMessage ="A valid Client must be selected!")]
        public int ClientId { get; set; }
        [Required(ErrorMessage ="A valid Appointment must be selected!")]
        public int AppointmentId { get; set; }
        public string? _billPhoto { get; set; }
        public string? BillPhoto
        {
            get

            {
                if (_billPhoto == null)
                {
                    _billPhoto = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTrJgrd1FTZ8VZtcKK0U1DTpCDeXrQewZdMh1s0Tdjv_VgyFZ6tFTk9gap4ETXaAaj2RwY&usqp=CAU";  }
                return _billPhoto;
            }
            set
            {
                _billPhoto = value;
            }
        }
        private string _status { get; set; }    
        public string Status { get
            {
                if(_status == null)
                {
                    _status = "Not Paid";
                }
                return _status;
            }
            set
            {
                _status = value;
            }
        }  
        public DateTime AppointmentDate { get; set; }
        public bool PayBill()
        {
            Status = "Paid";
            return true;
        }
        public string? DeletedClientName { get; set; }
        public string? DeletedClientSurName { get; set; }

    }
}
