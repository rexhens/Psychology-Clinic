using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Billing : BaseEntity<int>
    {
        public decimal Amount { get; set; }
        public DateTime BillingDate {  get; set; }
        public int ClientId { get;set; }
        public int AppointmentId { get; set; }
        public string Status { get; set; }
    }
}
