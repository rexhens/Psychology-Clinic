using Microsoft.AspNetCore.Mvc;
using MVC_BLL.Services;

namespace Psychology_Clinic.Controllers
{
	public class BillingsController : Controller
	{

		private readonly IBillingService _billingService;
		private readonly IClientService _clientService;
		private readonly IAppointmentService _appointmentService;
		public BillingsController(IBillingService billingService, IClientService clientService, IAppointmentService appointmentService)
		{
			_billingService = billingService;
			_clientService = clientService;
			_appointmentService = appointmentService;

		}
		public IActionResult Index(int? noDays = 5)
		{
			var list = _billingService.GetAll();
			ViewBag.Clients = _clientService.GetAll();
			ViewBag.Appointments = _appointmentService.GetAll();
			return View(list);
		}
		[HttpGet]
		public IActionResult Add(int id)
		{
			var exist = _billingService.GetBilling(id);
			if (exist != null)
			{
				return RedirectToAction("Index");
			}
			return View(exist);
		}
		[HttpPost]
		public IActionResult Add(MVC_BLL.Models.Billing billing)
		{
			var added = _billingService.Add(billing);
			if (added)
			{
				return RedirectToAction(nameof(Index));
			}
			return View(billing);
		}
		[HttpGet]
		public IActionResult Details(int id)
		{
			ViewBag.Clients = _clientService.GetAll();
			ViewBag.Appointments = _appointmentService.GetAll();
			var bill = _billingService.GetBilling(id);
			return View(bill);
		}

		[HttpPost]
		public IActionResult PayBill(int id)
		{
			var bill = _billingService.GetBilling(id);
			if (bill != null)
			{
				var edited = _billingService.EditBill(bill);
				if(edited)
				{
					return RedirectToAction(nameof(Index));
				}

			}
			return View("Error");
		}
	}
}
