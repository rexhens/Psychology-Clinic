using DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using MVC_BLL.Services;

namespace PL.Controllers
{
	public class TherapistsController : Controller
	{
		private readonly ITherapistService _repository;
        public TherapistsController(ITherapistService therapistRepository)
        {
            _repository = therapistRepository;
        }
        public IActionResult Index()
		{
			var list = _repository.GetTherapists();
			return View(list);
		}
	}
}
