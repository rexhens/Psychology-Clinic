using MVC_BLL.Services;
using Microsoft.AspNetCore.Mvc;
using MVC_BLL;
using Microsoft.AspNetCore.Authorization;

namespace Psychology_Clinic.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentsService;
        private readonly IClientService _clientService;
        private readonly IBillingService _billingService;
        public AppointmentsController(IAppointmentService appointmentsService, IClientService clientService, IBillingService billingService)
        {
            _appointmentsService = appointmentsService;
            _clientService = clientService;
            _billingService = billingService;
        }
        [Authorize(Roles ="Admin")]
        public IActionResult Index()
        {
            var list = _appointmentsService.GetAll();
            ViewBag.Clients = _clientService.GetAll();
            foreach(var app in list)
            {
                app.BillingId = _billingService.GetBillingIdByAppId(app.Id);
            }
            return View(list);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Add(int id)
        {
            var existAppointment = _appointmentsService.GetAppointmentById(id);
            ViewBag.Clients = _clientService.GetAll();
            return View(existAppointment);

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add(int id, MVC_BLL.Models.Requests.AppointmentAddModel appointment)
        {
            string errorMessage = null;
            if (ModelState.IsValid)
            {
                var appointmentList = _appointmentsService.GetAll();
                var validDate = _appointmentsService.ValidateDateAppointment(appointment, appointmentList);
                if (validDate.Status == MVC_BLL.Models.ViewResponseStatus.Ok)
                {
                    bool added = false;
                    if (User.IsInRole("Admin"))
                    {
						added = await _appointmentsService.CreateAppointmentAsync(appointment, appointment.ClientId);
					}
                    else
                    {
                        added = await _appointmentsService.CreateAppointmentAsync(appointment, User.GetUserId());
                    }
                    if (added)
                    {
                        await _appointmentsService.GenerateBill(appointment);
                        if (User.IsInRole("Admin"))
                        {
							return RedirectToAction(nameof(Index));
						}else
                        {
                            appointment.ClientId = User.GetUserId();
                            return RedirectToAction("Details", "Clients", new {id = appointment.ClientId});
                        }
					}
                }
                else
                {
                    errorMessage =validDate.ErrorMessage;
                    //ModelState.AddModelError("DateReserved", errorMessage);
				}

            }
            ViewBag.Clients = _clientService.GetAll();
			ViewBag.ErrorMessage = errorMessage;
			return View(appointment);
        }
        [HttpGet]
        [Authorize]
        public IActionResult Edit(int id)
        {
            var existAppointment = _appointmentsService.GetAppointmentById(id);
            if (existAppointment != null)
            {
                ViewBag.Clients = _clientService.GetAll();
                return View(existAppointment);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize]
        public IActionResult Edit(int id, MVC_BLL.Models.Appointment appointment)
        {

            if (ModelState.IsValid)
            {
                var appointmentList = _appointmentsService.GetAll();
                var validDate = _appointmentsService.ValidateDateAppointmentEdited(appointment, appointmentList);
                if (validDate.Status == MVC_BLL.Models.ViewResponseStatus.Ok)
                {
                    bool editedAppointment = false;
                    if (User.IsInRole("Admin"))
                    {
                        editedAppointment = _appointmentsService.UpdateAppointment(id, appointment, appointment.ClientId);
                    }
                    else
                    {
                        editedAppointment = _appointmentsService.UpdateAppointment(id, appointment, User.GetUserId());
                    }
                    if (editedAppointment)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    ModelState.AddModelError("DateReserved", validDate.ErrorMessage);
                }


            }
            ViewBag.Clients = _clientService.GetAll();
            return View(appointment);
        }
        [HttpGet]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var existAppointment = _appointmentsService.GetAppointmentById(id);
            if (existAppointment != null)
            {
                return View(existAppointment);
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
		[Authorize]
		public IActionResult Delete(int id, MVC_BLL.Models.Appointment appointment)
        {
            bool deleted = false;
            if(appointment.DateReserved < DateTime.Now)
            {
				 deleted = _appointmentsService.DeleteAppointment(id);
			}
            else
            {
				if (User.IsInRole("Admin"))
				{
					return RedirectToAction(nameof(Index));
				}
				else
				{
					return RedirectToAction("Details", "Clients", new { id = User.GetUserId() });
				}
			}
			if (deleted)
            {
				if (User.IsInRole("Admin"))
				{
					return RedirectToAction(nameof(Index));
				}
				else
				{
					return RedirectToAction("Details", "Clients", new { id = User.GetUserId() });
				}
			}

            return View(appointment);

        }
        [HttpGet]
        [Authorize]
        public IActionResult Details(int id)
        {
            var existAppointment = _appointmentsService.GetAppointmentById(id);
            if (existAppointment != null)
            {
                return View(existAppointment);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize]
        public IActionResult Billing(int id)
        {
            var existBilling = _billingService.GetByAppointmentId(id);
            if (existBilling != null)
            {
                ViewBag.Clients = _clientService.GetAll();
                ViewBag.Appointments = _appointmentsService.GetAll();
                return View(existBilling);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
