using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_BLL;
using MVC_BLL.Models;
using MVC_BLL.Models.Requests;
using MVC_BLL.Services;

namespace Psychology_Clinic.Controllers
{
	[Authorize]
	public class TreatmentsController : Controller
	{
		private readonly ITreatmentsService _treatmentsService;
		private readonly IClientService _clientService;
		private readonly IBillingService _billingService;
		private readonly IAppointmentService _appointmentService;
		public TreatmentsController(ITreatmentsService treatmentsService, IClientService clientService, IBillingService billingService, IAppointmentService appointmentService)
		{
			_treatmentsService = treatmentsService;
			_clientService = clientService;
			_billingService = billingService;
			_appointmentService = appointmentService;
		}

		[Authorize(Roles = "Admin")]
		public IActionResult Index()
		{
			ViewBag.Clients = _clientService.GetAll();
			return View(_treatmentsService.GetAll());
		}
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> AddAsync(int id)
		{
			TreatmentAddModel treatment = await _treatmentsService.GetByIdAddModelAsync(id);
			if (treatment != null)
			{
				return RedirectToAction("Index");
			}
			ViewBag.Clients = _clientService.GetAll();
			return View(treatment);
		}
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public IActionResult Add(TreatmentAddModel model)
		{
			string errorMsg = null;
			if (ModelState.IsValid)
			{
				var validate = _treatmentsService.ValidateTreatment(model);
				if (validate.Status == MVC_BLL.Models.ViewResponseStatus.Ok)
				{
					var added = _treatmentsService.Add(model);
					if (added)
					{
						return RedirectToAction("Index");
					}
				}
				else
				{
					if (validate.ErrorMessage == "Starting date cannot be in the past!")
					{
						ModelState.AddModelError("StartDate", validate.ErrorMessage);
					}
					else if (validate.ErrorMessage == "Ending date cannot be in the past!")
					{
						ModelState.AddModelError("EndDate", validate.ErrorMessage);

					}
					else if (validate.ErrorMessage == "Ending Date must be after starting date!")
					{
						ModelState.AddModelError("EndDate", validate.ErrorMessage);
					}
					else if(validate.ErrorMessage == "This client has already his/her treatment!")
					{
						errorMsg = validate.ErrorMessage;
					}
				}
			}
			ViewBag.ErrorMessage = errorMsg;
			ViewBag.Clients = _clientService.GetAll();
			return View(model);
		}
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public IActionResult Edit(int id)
		{
			var exist = _treatmentsService.GetById(id);
			if (exist == null)
				return RedirectToAction("Index");
			ViewBag.Clients = _clientService.GetAll();
			return View(exist);
		}
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public IActionResult Edit(MVC_BLL.Models.Treatment treatment)
		{
			if (ModelState.IsValid)
			{
				var validate = _treatmentsService.ValidateTreatment(treatment);

				if (validate.Status == MVC_BLL.Models.ViewResponseStatus.Ok)
				{
					var edited = _treatmentsService.Update(treatment);
					if (edited)
					{
						return RedirectToAction("Index");
					}
				}
				else
				{
					if (validate.ErrorMessage == "Starting date cannot be in the past!")
					{
						ModelState.AddModelError("StartDate", validate.ErrorMessage);
					}
					else if (validate.ErrorMessage == "Ending date cannot be in the past!")
					{
						ModelState.AddModelError("EndDate", validate.ErrorMessage);

					}
					else if (validate.ErrorMessage == "Ending Date must be after starting date!")
					{
						ModelState.AddModelError("EndDate", validate.ErrorMessage);
					}
					else if (validate.ErrorMessage == "This client has already his/her treatment!")
					{
						ModelState.AddModelError("ClientId", validate.ErrorMessage);

					}

				}
				ViewBag.Clients = _clientService.GetAll();
			}
			return View(treatment);
		}
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public IActionResult Delete(int id)
		{
			var exist = _treatmentsService.GetById(id);
			if (exist == null)
				return RedirectToAction("Index");
			return View(exist);
		}
		[HttpPost]
		public IActionResult Delete(MVC_BLL.Models.Treatment treatment)
		{
			var deleted = _treatmentsService.Delete(treatment.Id);
			if (deleted)
			{
				return RedirectToAction("Index");
			}
			return View(treatment);
		}
		[HttpGet]
		public async Task<IActionResult> DetailsAsync(int id)
		{
			var exist = _treatmentsService.GetById(id);
			if (exist == null)
				return RedirectToAction(nameof(Index));
			if (User.IsInRole("Admin"))
			{
				ViewBag.Client = await _treatmentsService.GetClientAsync(exist.ClientId, exist.ClientId);
			}
			else
			{
				ViewBag.Client = await _treatmentsService.GetClientAsync(exist.ClientId, User.GetUserId());

			}
			ViewBag.Clients = _clientService.GetAll();
			return View(exist);

		}
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> ClientAsync(int id)
		{
			var exist = _treatmentsService.GetById(id);
			var client = await _clientService.GetByIdAsync(exist.ClientId);
			int[] numberDisplay = new int[3];
			List<MVC_BLL.Models.Appointment> appointmentDisplay = null;

			appointmentDisplay = await _appointmentService.GetAppointemntsByClientIdAsync(exist.ClientId, exist.ClientId);

			numberDisplay[0] = appointmentDisplay.Count();
			numberDisplay[1] = _billingService.GetByClientId(exist.ClientId, exist.ClientId).Count();
			MVC_BLL.Models.Treatment treatmentDisplay = null;

			treatmentDisplay = await _treatmentsService.GetTreatmentByClientId(exist.ClientId, exist.ClientId);
			if (treatmentDisplay != null)
			{
				numberDisplay[2] = 1;
			}
			else
			{
				numberDisplay[2] = 0;
			}
			ViewBag.DisplayNumbers = numberDisplay;
			return View(client);
		}
	}
}
