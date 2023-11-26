using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
	public class Treatment : BaseEntity<int>
	{
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string? Description { get; set; }
		public int ClientId { get; set; }
		public string Cause { get;set; }
	}
}
