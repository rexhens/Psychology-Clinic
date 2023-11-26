using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL.Models.Requests
{
	public class TreatmentAddModel
	{
		[Required(ErrorMessage ="You must enter the Treatment Starting Date!")]
		public DateTime StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public string? Description { get; set; }
		[Required(ErrorMessage ="You must enter the name of the client you are reffering")]
		public int ClientId { get; set; }
		[Required(ErrorMessage ="You must enter the couse of the treatment")]
		public string Cause { get; set; }
	}
}
