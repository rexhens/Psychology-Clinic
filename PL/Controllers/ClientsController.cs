using MVC_BLL.Models.Requests;
using MVC_BLL.Services;
using Microsoft.AspNetCore.Mvc;
using MVC_BLL.Models;
using Microsoft.AspNetCore.Authorization;
using MVC_BLL;

namespace Psychology_Clinic.Controllers
{
	//Roles -> admin,client
	[Authorize]
	public class ClientsController : Controller
	{
		readonly IClientService _clientService;
		private readonly IAppointmentService _appointmentService;
		private readonly IBillingService _billingService;
		private readonly ITreatmentsService _treatmentsService;
		public ClientsController(IClientService clientService, IAppointmentService appointmentService, IBillingService billingService, ITreatmentsService treatmentsService)
		{
			_clientService = clientService;
			_appointmentService = appointmentService;
			_billingService = billingService;
			_treatmentsService = treatmentsService;
		}
		[Authorize(Roles = "Admin")]
		public ActionResult Index()
		{
			var clients = _clientService.GetAll();
			if (clients != null)
				return View(clients);
			return View();
		}
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult> AddAsync()
		{
			return View();
		}


		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult> AddAsync(ClientAddModel model)
		{
			if (ModelState.IsValid)
			{
				if (_clientService.ValidateClient(model).Status == MVC_BLL.Models.ViewResponseStatus.Ok)
				{

					var added = await _clientService.AddClientAsync(model);
					if (added.Status == MVC_BLL.Models.ViewResponseStatus.Ok)
					{
						return RedirectToAction("Index");
					}
					ViewBag.ActionResponse = added;
				}
				else
				{
					ModelState.AddModelError("UserName", "Another Client exist with this UserName!");
				}
			}
			return View(model);

		}
		[HttpGet]
		[Authorize]
		public async Task<ActionResult> Edit(int id)
		{
			var exist = await _clientService.GetByIdAsync(id);
			if (exist != null)
			{
				return View(exist);
			}
			return RedirectToAction("Index");
		}
		[HttpPost]
		[Authorize]
		public async Task<IActionResult> EditAsync(int id, MVC_BLL.Models.Client client)
		{
			if (ModelState.IsValid)
			{
				var editedAcc = await _clientService.EditClientAsync(client, id);
				if (editedAcc.Status == MVC_BLL.Models.ViewResponseStatus.Ok)
				{
					if (User.IsInRole("Client"))
					{
						return RedirectToAction("Details", new { id = id });
					}
					return RedirectToAction(nameof(Index));
				}
				ViewBag.ActionResponse = editedAcc;
				ModelState.AddModelError("Name", editedAcc.ErrorMessage);
			}

			return View(client);
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult> DeleteAsync(int id)
		{
			var exist = await _clientService.GetByIdAsync(id);
			if (exist != null)
			{
				var appointments = _appointmentService.GetAll().Where(x => x.ClientId == id && x.DateReserved > DateTime.UtcNow);
				var treatment = await _treatmentsService.GetTreatmentByClientId(id,id);
				if (treatment == null)
				{
					ViewBag.Treatment = 0;
				}
				else
				{
					ViewBag.Treatment = 1;
						}
				ViewBag.Appointments = appointments.Count();
				return View(exist);
			}
			return RedirectToAction("Index");

		}

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<ActionResult> DeleteAsync(MVC_BLL.Models.Client client, int id)
		{

			var deleted = await _clientService.DeleteClientAsync(id);
			if (deleted.Status == MVC_BLL.Models.ViewResponseStatus.Ok)
			{
				return RedirectToAction(nameof(Index));
			}
			ViewBag.ActionResponse = deleted;
			return View(client);
		}

		[HttpGet]
		public async Task<ActionResult> DetailsAsync(int id)
		{
			var existClient = await _clientService.GetByIdAsync(id);
			if (existClient != null)
			{
				int[] numberDisplay = new int[3];
				List<Appointment> appointmentDisplay = null;
				if (User.IsInRole("Admin"))
				{
					appointmentDisplay = await _appointmentService.GetAppointemntsByClientIdAsync(id, id);
				}
				else
				{
					appointmentDisplay = await _appointmentService.GetAppointemntsByClientIdAsync(id, User.GetUserId());
				}
				numberDisplay[0] = appointmentDisplay.Count();
				if(User.IsInRole("Admin"))
				{
					numberDisplay[1] = _billingService.GetByClientId(id, id).Count();

				}
				else
				{
					numberDisplay[1] = _billingService.GetByClientId(id, User.GetUserId()).Count();
				}
				MVC_BLL.Models.Treatment treatmentDisplay = null;
				if (User.IsInRole("Admin"))
				{
					treatmentDisplay = await _treatmentsService.GetTreatmentByClientId(id, id);
				}
				else
				{
					treatmentDisplay = await _treatmentsService.GetTreatmentByClientId(id, User.GetUserId());

				}
				if (treatmentDisplay != null)
				{
					numberDisplay[2] = 1;
				}
				else
				{
					numberDisplay[2] = 0;
				}
				ViewBag.DisplayNumbers = numberDisplay;
				return View(existClient);
			}
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<ActionResult> Appointments(int id)
		{
			List<MVC_BLL.Models.Appointment> appointmentList = null;
			if (User.IsInRole("Admin"))
			{
				appointmentList = await _appointmentService.GetAppointemntsByClientIdAsync(id, id);
			}
			else
			{
				appointmentList = await _appointmentService.GetAppointemntsByClientIdAsync(id, User.GetUserId());
			}	

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
			List<Billing> billingsList = null;
			if(User.IsInRole("Admin"))
			{
				billingsList = _billingService.GetByClientId(id, id);
			}
			else
			{
				billingsList = _billingService.GetByClientId(id, User.GetUserId());
			}
		
			if (billingsList != null)
			{
				ViewData["Clients"] = _clientService.GetAll();

				ViewBag.Appointments = _appointmentService.GetAll();
				return View(billingsList);
			}
			return RedirectToAction(nameof(Index));
		}
		[HttpGet]
		public async Task<ActionResult> TreatmentAsync(int id)
		{
			MVC_BLL.Models.Treatment treatment = await _treatmentsService.GetTreatmentByClientId(id, User.GetUserId());
			if (treatment != null)
			{
				ViewBag.Client = await _clientService.GetByIdAsync(treatment.ClientId);
			}
			return View(treatment);
		}

	}
}
