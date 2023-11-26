using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_BLL;
using MVC_BLL.Services;

namespace Psychology_Clinic.Controllers
{
    [Authorize]
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
		[Authorize(Roles = "Admin")]
		public IActionResult Index(int? selectedOption)
        {
            if (selectedOption.HasValue)
            {
                var list1 = FilterBillingsByOption(selectedOption.Value);
                ViewBag.Clients = _clientService.GetAll();
                ViewBag.Appointments = _appointmentService.GetAll();
                return View(list1);
            }
            var list = _billingService.GetAll();
            ViewBag.Clients = _clientService.GetAll();
            ViewBag.Appointments = _appointmentService.GetAll();
            return View(list);
        }
        private IEnumerable<MVC_BLL.Models.Billing> FilterBillingsByOption(int selectedOption)
        {
            var billings = _billingService.GetAll();
            var currentDate = DateTime.Today;
            _billingService.GetAppointmentsDate(billings);
            switch (selectedOption) // 1 -> Future 0 -> Past
            {
                case 0:

                    return billings.Where(b => b.AppointmentDate < currentDate);
                case 1:
                    return billings.Where(b => b.AppointmentDate > currentDate);
                case 10:
                    return billings.Where(b => b.AppointmentDate <= currentDate.AddDays(10));
                case -10:
                    return billings.Where(b => b.AppointmentDate <= currentDate.AddDays(-10));
                case -30:
                    return billings.Where(b => b.AppointmentDate >= currentDate.AddDays(-30));
                case 30:
                    return billings.Where(b => b.BillingDate <= currentDate.AddDays(30));
                default:
                    return billings;
            }
        }

        [HttpGet]
		[Authorize(Roles = "Admin")]
		public IActionResult Add(int id)
        {
            MVC_BLL.Models.Billing exist = null;

			if (User.IsInRole("Admin"))
            {
			 exist = _billingService.GetBilling(id, User.GetUserId(),"Admin");

            }
            else
            {
				exist = _billingService.GetBilling(id, User.GetUserId());

			}
			if (exist != null)
            {
                return RedirectToAction("Index");
            }
            return View(exist);
        }
        [HttpPost]
		[Authorize(Roles = "Admin")]
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
            MVC_BLL.Models.Billing bill = null;

			ViewBag.Clients = _clientService.GetAll();
            ViewBag.Appointments = _appointmentService.GetAll();
            if (User.IsInRole("Admin"))
            {
                bill = _billingService.GetBilling(id, User.GetUserId(), "Admin");
            }
            else
            {
                bill = _billingService.GetBilling(id, User.GetUserId());

			}
            return View(bill);
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public IActionResult PayBill(int id)
		{
			MVC_BLL.Models.Billing bill = null;
			if (User.IsInRole("Admin"))
			{
				bill = _billingService.GetBilling(id, User.GetUserId(), "Admin");
			}
			else
			{
				bill = _billingService.GetBilling(id, User.GetUserId());

			}
			if (bill != null)
            {
                var edited = _billingService.EditBill(bill);
                if (edited)
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            return View("Error");
        }
    }
}
