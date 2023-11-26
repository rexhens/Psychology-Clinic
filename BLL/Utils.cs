using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL
{
    public static class Utils
    {
        public static string LogsDirectory { get; set; }
		public static int GetUserId(this ClaimsPrincipal user)
		{
			try
			{
				return int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
			}
			catch (Exception ex)
			{
				return -1;
			}
		}
	}
}
