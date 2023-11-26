using MVC_BLL.Models.Requests;
using MVC_BLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace Psychology_Clinic.Controllers
{
	public class ClientsController : Controller
	{
		readonly IClientService _clientService;
		private readonly IAppointmentService _appointmentService;
		private readonly IBillingService _billingService;
		public ClientsController(IClientService clientService, IAppointmentService appointmentService, IBillingService billingService)
		{
			_clientService = clientService;
			_appointmentService = appointmentService;
			_billingService = billingService;
		}
		public ActionResult Index()
		{
			var clients = _clientService.GetAll();
			if (clients != null)
				return View(clients);
			return View();
		}
		[HttpGet]
		public ActionResult Add(int id)
		{
			var existClient = _clientService.GetByIdClientAddModel(id);
			if (existClient == null)
			{
				return View(existClient);
			}
			return RedirectToAction("Index");
		}


		[HttpPost]
		public ActionResult Add(ClientAddModel model)
		{
			if (ModelState.IsValid)
			{
				if (_clientService.ValidateClient(model))
				{

					var added = _clientService.AddClient(model);
					if (added)
					{
						return RedirectToAction("Index");
					}
				}
				else
				{
					ModelState.AddModelError("", "Another Client exist with this name.");
				}
			}
			return View(model);

		}
		[HttpGet]

		public ActionResult Edit(int id)
		{
			var exist = _clientService.GetById(id);
			if (exist != null || exist.Id != 0)
			{
				return View(exist);
			}
			return RedirectToAction("Index");
		}
		[HttpPost]
		public IActionResult Edit(int id, MVC_BLL.Models.Client client)
		{
			if (ModelState.IsValid)
			{
				var editedAcc = _clientService.EditClient(client, id);
				if (editedAcc)
				{
					return RedirectToAction(nameof(Index));
				}
			}
			return View(client);
		}

		[HttpGet]

		public ActionResult Delete(int id)
		{
			var exist = _clientService.GetById(id);
			if (exist != null)
			{
				return View(exist);
			}
			return RedirectToAction("Index");

		}

		[HttpPost]

		public ActionResult Delete(MVC_BLL.Models.Client client, int id)
		{

				var deleted = _clientService.DeleteClient(id);
				if (deleted)
				{
					return RedirectToAction(nameof(Index));
				}

			
			return View(client);
		}

		[HttpGet]
		public ActionResult Details(int id)
		{
			var existClient = _clientService.GetById(id);
			if (existClient != null)
			{
				return View(existClient);
			}
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public ActionResult Appointments(int id)
		{
			var appointmentList = _appointmentService.GetAppointemntsByClientId(id);
			if (appointmentList != null)
			{
				ViewBag.Clients = _clientService.GetAll();
				return View(appointmentList);
			}
			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Billings(int id)
		{
			var billingsList = _billingService.GetByClientId(id);
			if (billingsList != null)
			{
				ViewBag.Clients = _clientService.GetAll();
				ViewBag.Appointments = _appointmentService.GetAll();
				return View(billingsList);
			}
			return RedirectToAction(nameof(Index));
		}

	}
}
