using DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL.Models.Requests
{
    public class AppointmentAddModel :IValidatableObject
    {
        [Required] 
        public DateTime DateReserved { get; set; }
        private string? notes;
        public string? Notes { 
            get
            {
                if(notes == null)
                {
                    return "-";
                }
                return notes;
            } set
            {
                notes = value;
            }
            }
        public int ClientId { get; set; }
        [Required]
        public string Type { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DateReserved <= DateTime.UtcNow)
            {
                yield return new ValidationResult("The reservation date must be actual!");
            }
           
        }
    }
}
