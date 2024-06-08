using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVC_BLL;
using MVC_BLL.Services;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

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
        public async Task<IActionResult> PayBillAsync(int id)
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
                var client = await _clientService.GetByIdAsync(bill.ClientId);
                var appointment = _appointmentService.GetAppointmentById(bill.AppointmentId);
                if (edited)
                {
 
                    Random random = new Random();
                    int randNumber = random.Next(0, 100000);
					return await GeneratePDF(randNumber,client.Name,client.Surname,client.Email,client.PhoneNum,bill.BillingDate, bill.Amount,appointment.Type,appointment.DateReserved);
                }

            }
            return View("Error");
        }

        public async Task<IActionResult> GeneratePDF(int InvoiceNo,string clientName,string surname,string email,string phoneNum,DateTime billDate,
            decimal amount,string typeAppointment,DateTime dateTimeApp) 
        {
            var document = new PdfDocument();
			string HtmlContent = @$"
<div class='container mt-6 mb-7'>
    <div class='row justify-content-center'>
        <div class='col-lg-12 col-xl-7'>
            <div class='card'>
                <div class='card-body p-5'>
                    <h2>Bill of {clientName + " " + surname}</h2>
                    <p class='fs-sm'>
                        This is the bill payment of <strong>{amount}</strong> (USD) for the service of {typeAppointment}.
                    </p>

                    <div class='border-top border-gray-200 pt-4 mt-4'>
                        <div class='row'>
                            <div class='col-md-6'>
                                <div class='text-muted mb-2'>Payment No.</div>
                                <strong>#{InvoiceNo}</strong>
                            </div>
                            <div class='col-md-6 text-md-end'>
                                <div class='text-muted mb-2'>Billing Date</div>
                                <strong>{billDate.ToString("dd/MM/yyyy")}</strong>
                            </div>
                            <div class='col-md-6 text-md-end'>
                                <div class='text-muted mb-2'>Appointment Date</div>
                                <strong>{dateTimeApp.ToString("dd/MM/yyyy")}</strong>
                            </div>
                        </div>
                    </div>

                    <div class='border-top border-gray-200 mt-4 py-4'>
                        <div class='row'>
                            <div class='col-md-6'>
                                <div class='text-muted mb-2'>Client</div>
                                <strong>{clientName}</strong>
                                <p class='fs-sm'>
                                    Vore, Tirane 1001<br>
                                    <a href='#!' class='text-purple'>{clientName}@email.com</a>
                                </p>
                            </div>
                            <div class='col-md-6 text-md-end'>
                                <div class='text-muted mb-2'>Payment To</div>
                                <strong>Themes LLC</strong>
                                <p class='fs-sm'>
                                    Kavaja St., Tirana 1111<br>
                                    <a href='#!' class='text-purple'>clinic@email.com</a>
                                </p>
                            </div>
                        </div>
                    </div>

                    <table class='table border-bottom border-gray-200 mt-3'>
                        <thead>
                            <tr>
                                <th scope='col' class='fs-sm text-dark text-uppercase-bold-sm px-0'>Description</th>
                                <th scope='col' class='fs-sm text-dark text-uppercase-bold-sm text-end px-0'>Amount</th>
                            </tr>
                        </thead>
                        <tbody>
                 
                            <tr>
                                <td class='px-0'>{typeAppointment}</td>
                                <td class='text-end px-0'>${amount}</td>
                            </tr>
                        </tbody>
                    </table>
                        <div class='d-flex justify-content-end mt-3'>
                            <h5 class='me-3'>Total:</h5>
                            <h5 class='text-success'>${amount} USD</h5>
                        </div>
                    </div>
                </div>
              
            </div>
        </div>
    </div>
</div>";
			PdfGenerator.AddPdfPages(document, HtmlContent, PageSize.A4);
            byte[]? response = null;
            using (MemoryStream ms = new MemoryStream())
            {

                document.Save(ms);
                response = ms.ToArray();
            }
            string fileName = "bill_no_"+InvoiceNo +".pdf";
            return File(response, "Bills/pdf", fileName);
        }
    }
}
