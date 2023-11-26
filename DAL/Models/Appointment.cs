using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Appointment : BaseEntity<int>
    {

        [Column("AppointmentDate")]
        public DateTime DateReserved { get; set; }
        public string Notes { get; set; }   
        public int ClientId { get; set; }

    }
}
