using Microsoft.AspNetCore.Mvc;
using MVC_BLL.Services;
using NuGet.Protocol.Core.Types;
using Psychology_Clinic.Models;
using System.Diagnostics;

namespace Psychology_Clinic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITherapistService _repository;
        public HomeController(ILogger<HomeController> logger,ITherapistService therapistService)
        {
            _repository = therapistService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.Therapists = _repository.GetTherapists();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Clients()
        {
            return View();
        }
        public IActionResult Appointments()
        {
            return View();
        }
        public IActionResult Billings()
        {
            return View();
        }
		public IActionResult Therapists()
		{
			return View();
		}
		

      
    }
}