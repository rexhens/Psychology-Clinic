using MVC_BLL.Services;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Add(int id)
        {
            var existAppointment = _appointmentsService.GetAppointmentById(id);
            ViewBag.Clients = _clientService.GetAll();
            return View(existAppointment);



        }

        [HttpPost]
        public IActionResult Add(int id, MVC_BLL.Models.Requests.AppointmentAddModel appointment)
        {
            string errorMessage = null;
            if (ModelState.IsValid)
            {
                var appointmentList = _appointmentsService.GetAll();
                var validDate = _appointmentsService.ValidateDateAppointmentAddModel(appointment, appointmentList);
                if (validDate)
                {
                    var added = _appointmentsService.CreateAppointment(appointment);
                    _appointmentsService.GenerateBill(appointment);
                    if (added)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    errorMessage = "Another appointment is reserved in these hours.";
				}

            }
            ViewBag.Clients = _clientService.GetAll();
			ViewBag.ErrorMessage = errorMessage;
			return View(appointment);
        }
        [HttpGet]
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
        public IActionResult Edit(int id, MVC_BLL.Models.Appointment appointment)
        {
            string errorMessage = null;
            if (ModelState.IsValid)
            {
                var appointmentList = _appointmentsService.GetAll();
                var validDate = _appointmentsService.ValidateDateAppointment(appointment, appointmentList);
                if (validDate)
                {
                    var editedAppointment = _appointmentsService.UpdateAppointment(id, appointment);
                    if (editedAppointment)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    errorMessage = "Another appointment is reserved in these hours.";

				}

            }
            ViewBag.ErrorMessage = errorMessage;

			return View(appointment);
        }
        [HttpGet]
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
        public IActionResult Delete(int id, MVC_BLL.Models.Appointment appointment)
        {

            var deleted = _appointmentsService.DeleteAppointment(id);
            if (deleted)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(appointment);

        }
        [HttpGet]
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
