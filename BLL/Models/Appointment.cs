using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime DateReserved { get; set; }
        public string Notes { get; set; }
        public int ClientId { get; set; }
        public string GetAppointmentDay()
        {
            return DateReserved.ToString("dddd");
        }
        public string GetAppointmentMonth()
        {
            return DateReserved.ToString("MMMM");
        }
        public string HourFinished()
        {
            var dateFinished = DateReserved.AddHours(1);
            return dateFinished.ToString("HH:mm");
        }
        public int BillingId { get; set; }
        public string Type { get; set; }   
        private string _imgUrlPhoto { get;set; }
        public string? ImgUrlPhoto
        {
            get
            {
                if(_imgUrlPhoto == null)
                {
                    _imgUrlPhoto = "https://i.pinimg.com/736x/ce/e0/fc/cee0fc2838197c1e64c8bb17046c5d25.jpg";
				}
                return _imgUrlPhoto;
            }
            set
            {
                _imgUrlPhoto = value;
            }
        }
        public string? DeletedClientName { get; set; }
        public string? DeletedClientSurName { get; set; }


    }
}
