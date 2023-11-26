using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL.Models.Requests
{
	public class LogInRequestModel
	{
		public string UserName { get; set; }
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
