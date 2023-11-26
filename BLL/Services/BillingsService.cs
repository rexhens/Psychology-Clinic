using QRCoder;
using System.ComponentModel;
using System.Drawing;


namespace MVC_BLL.Services
{
    public interface IBillingService
    {
        bool Add(Models.Billing billing);
        bool Delete(int id);
        List<Models.Billing> GetAll();
        Models.Billing GetBilling(int id, int userId, string role = "");
        List<Models.Billing> GetByClientId(int clientId, int userId);
        Models.Billing GetByAppointmentId(int appointmentId);
        int GetBillingIdByAppId(int appointmentId);
        bool EditBill(Models.Billing billing);
        List<Models.Billing> FilterBills(int noDays);
        void GetAppointmentsDate(List<Models.Billing> bills);
        void GenerateQRCode(Models.Billing model);
    }
    public class BillingsService : BaseService, IBillingService
    {
        public static event IWriteLog OnLogOccured;
        public BillingsService(IServiceProvider unit) : base(unit)
        {

        }

        public bool Add(Models.Billing billing)
        {
            try
            {
                var addedDal = _unitOfWork.BillingsRepository.Add(new DAL.Models.Billing
                {
                    Amount = billing.Amount,
                    BillingDate = billing.BillingDate,
                    ClientId = billing.ClientId,
                    AppointmentId = billing.AppointmentId,
                    Status = billing.Status,
                });

                _unitOfWork.Commit();
                OnLogOccured?.Invoke(new DAL.Models.AuditLog
                {
                    Description = "A new Bill was Generated Manually",
                    Name = "Bill",
                    ObjectId = billing.Id
                });
                return true;

            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in Add Billing method: {ex.Message}");

            }
            return false;
        }

        public bool Delete(int id)
        {
            try
            {
                var deleted = _unitOfWork.BillingsRepository.Delete(id);
                if (deleted)
                {
                    _unitOfWork.Commit();
                    OnLogOccured?.Invoke(new DAL.Models.AuditLog
                    {
                        Description = "A Bill was deleted",
                        Name = "Bill",
                        ObjectId = id
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in Delete Billing method: {ex.Message}");

            }
            return false;
        }

        public bool EditBill(Models.Billing billing)
        {
            try
            {
                var dbBill = _unitOfWork.BillingsRepository.GetById(billing.Id);
                dbBill.Status = "Paid";
                _unitOfWork.Commit();
                OnLogOccured?.Invoke(new DAL.Models.AuditLog
                {
                    Description = "A Bill was Edited",
                    Name = "Bill",
                    ObjectId = billing.Id
                });
                return true;

            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in EditBilling method: {ex.Message}");

            }
            return false;
        }

        public List<Models.Billing> GetAll()
        {
            List<Models.Billing> result = new List<Models.Billing>();
            try
            {
                var dalList = _unitOfWork.BillingsRepository.GetAll();
                foreach (var dal in dalList)
                {
                    result.Add(new Models.Billing
                    {
                        Id = dal.Id,
                        Amount = dal.Amount,
                        BillingDate = dal.BillingDate,
                        ClientId = dal.ClientId,
                        AppointmentId = dal.AppointmentId,
                        Status = dal.Status,
                    });
                }

            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in GetAll method: {ex.Message}");

            }
            return result;
        }

        public Models.Billing GetBilling(int id, int userId, string role = "")
        {

            Models.Billing billing = null;
            try
            {
                var dalBilling = _unitOfWork.BillingsRepository.GetById(id);
                if (dalBilling == null)
                {
                    return null;
                }
                if (dalBilling.ClientId != userId && role != "Admin")
                {
                    throw new StandardViewResponseException("Nuk jeni i autorizuar per te modifikuar kete llogari!");
                }
                billing = new Models.Billing
                {
                    Id = dalBilling.Id,
                    Amount = dalBilling.Amount,
                    BillingDate = dalBilling.BillingDate,
                    ClientId = dalBilling.ClientId,
                    AppointmentId = dalBilling.AppointmentId,
                    Status = dalBilling.Status,
                };
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in GetBilling method: {ex.Message}");
            }
            return billing;
        }

        public int GetBillingIdByAppId(int appointmentId)
        {
            try
            {
                var list = _unitOfWork.BillingsRepository.GetAll();
                foreach (var billing in list)
                {
                    if (billing.AppointmentId == appointmentId)
                    {
                        return billing.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in GetBillingByAppointmentId method: {ex.Message}");
            }
            return 0;

        }

        public MVC_BLL.Models.Billing GetByAppointmentId(int appointmentId)
        {
            try
            {
                var billingList = GetAll();
                foreach (var billing in billingList)
                {
                    if (billing.AppointmentId == appointmentId)
                    {
                        return billing;
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in GetByAppointmentId Billing method: {ex.Message}");
            }
            return null;
        }

        public List<MVC_BLL.Models.Billing> GetByClientId(int clientId, int userId)
        {
            var result = new List<MVC_BLL.Models.Billing>();
            try
            {
                if (clientId != userId)
                {
                    throw new StandardViewResponseException("Nuk jeni i autorizuar per te modifikuar kete llogari!");
                }
                var dalList = GetAll();
                foreach (var billing in dalList)
                {
                    if (billing.ClientId == clientId)
                    {
                        result.Add(billing);
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in GetByClientId method: {ex.Message}");

            }
            return result;

        }


        public List<MVC_BLL.Models.Billing> FilterBills(int noDays)
        {
            var billingList = new List<Models.Billing>();
            try
            {
                var currentDate = DateTime.Now;
                var list = GetAll();
                foreach (var billing in list)
                {
                    if (billing.BillingDate <= currentDate.AddDays(noDays) && billing.BillingDate >= DateTime.Now)
                    {
                        billingList.Add(billing);
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in FilterBillings method: {ex.Message}");
            }
            return billingList;

        }

        public void GetAppointmentsDate(List<Models.Billing> bills)
        {
            try
            {
                var result = _unitOfWork.AppointmentRepository.GetAll().ToList();
                for (int i = 0; i < bills.Count(); i++)
                {
                    for (int j = 0; j < result.Count(); j++)
                    {
                        if (result[j].Id == bills[i].AppointmentId)
                        {
                            bills[i].AppointmentDate = result[j].DateReserved;

                        }
                    }
                }
            }

            catch (Exception ex)
            {
                _loggerService.LogError($"Error in ValidateTreatment method: {ex.Message}");


            }
        }

        public void GenerateQRCode(Models.Billing model)
        {
            //try
            //{
            //	// Create a URL with the model.Id
            //	string url = $"https://localhost:7170/Billings/Details/{model.Id}";

            //	// Create a QRCodeGenerator
            //	QRCodeGenerator qrGenerator = new QRCodeGenerator();
            //	QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

            //	// Create a QR code image
            //	QRCode qrCode = new QRCode(qrCodeData);
            //	Bitmap qrCodeImage = qrCode.GetGraphic(10, Color.Black, Color.White, true);

            //	// Save or display the QR code image as needed
            //	// For example, save it as a file:
            //	qrCodeImage.Save("path_to_save_qr_code.png"); // Provide the path to save the image
            //}
            //catch (Exception ex)
            //{
            //	// Handle exceptions, log errors, etc.
            //}
        }
    }

}
