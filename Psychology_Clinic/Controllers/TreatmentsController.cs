using Microsoft.AspNetCore.Mvc;
using MVC_BLL.Models.Requests;
using MVC_BLL.Services;

namespace Psychology_Clinic.Controllers
{
	public class TreatmentsController : Controller
	{
		private readonly ITreatmentsService _treatmentsService;
		private readonly IClientService _clientService;
		public TreatmentsController(ITreatmentsService treatmentsService, IClientService clientService)
		{
			_treatmentsService = treatmentsService;
			_clientService = clientService;
		}
		public IActionResult Index()
		{
			ViewBag.Clients = _clientService.GetAll();
			return View(_treatmentsService.GetAll());
		}
		[HttpGet]
		public IActionResult Add(int id)
		{
			TreatmentAddModel treatment = _treatmentsService.GetByIdAddModel(id);
			if(treatment == null)
			{
				return RedirectToAction("Index");	
			}
			return View(treatment);
		}
		[HttpPost]
		public IActionResult Add(TreatmentAddModel model)
		{
			var added = _treatmentsService.Add(model);
			if(added)
			{
				return RedirectToAction("Index");
			}
			return View(model);
		}
	}
}
