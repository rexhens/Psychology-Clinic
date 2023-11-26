using Microsoft.AspNetCore.Mvc;

namespace Psychology_Clinic.Controllers
{
	public class UsersController : Controller	
	{
		[HttpPost]
		public IActionResult Login(MVC_BLL.Models.Requests.LogInRequestModel logInRequestModel)
		{
			if(logInRequestModel.UserName == "admin" && logInRequestModel.Password == "admin")
			{
				HttpContext.Session.SetString("User", logInRequestModel.UserName);
			//Merr Ip address
			//	HttpContext.Session.SetString("UserIp", Request.ServerVariables["REMOTE-ADDR"]);
			}
				return View();
		}
		[HttpPost]
		public IActionResult LogOut()
		{
			HttpContext.Session.Clear();
			return RedirectToAction("Index");
		}
	}
}
