using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL
{

	public class StandardViewResponseException : Exception
	{
		public StandardViewResponseException(string message) : base(message)
		{

		}
	}
}
