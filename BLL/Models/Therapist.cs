using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL.Models
{
	public class Therapist
	{
		public int LicenseNo { get; set; }
		public string FullName { get; set; }
		public int YearsExperience { get; set; }
		public string Degree { get; set; }
		public int Id { get; internal set; }
        public string? Department { get; set; }
    }
}
