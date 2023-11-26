using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
	public class Therapist : BaseEntity<int>
	{
		//public int LicenseNo { get; set; }
		public string FullName { get; set; }
		public int YearsExperience { get; set; }
		public string Degree { get; set; }
		public string? Department { get; set; }
	}
}
