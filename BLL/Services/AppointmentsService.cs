using DAL;
using DAL.Models;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using MVC_BLL.Models;
using MVC_BLL.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL.Services
{
    public interface IAppointmentService
    {
        Task<bool> CreateAppointmentAsync(MVC_BLL.Models.Requests.AppointmentAddModel appointment, int userId);
        bool UpdateAppointment(int id, Models.Appointment appointment, int userId);
        bool DeleteAppointment(int id);
        List<MVC_BLL.Models.Appointment> GetAll();
        Task<List<MVC_BLL.Models.Appointment>> GetAppointemntsByClientIdAsync(int id, int userId);
        MVC_BLL.Models.Appointment GetAppointmentById(int id);
        public bool ValidateDateAppointment(Models.Appointment appointment, List<Models.Appointment> appointmentsList);
        public StandardViewResponse<bool> ValidateDateAppointment(AppointmentAddModel appointment, List<Models.Appointment> appointmentsList);
        public Task GenerateBill(Models.Requests.AppointmentAddModel appointment);
        public StandardViewResponse<bool> ValidateDateAppointmentEdited(Models.Appointment appointment, List<Models.Appointment> appointmentsList);



    }
    public class AppointmentsService : BaseService, IAppointmentService
    {
        public static event IWriteLog OnLogOccured;
        public AppointmentsService(IServiceProvider unitOfWork) : base(unitOfWork)
        {

        }
        public List<Models.Appointment> GetAll()
        {
            try
            {
                IEnumerable<DAL.Models.Appointment> appointments = _unitOfWork.AppointmentRepository.GetAll();
                if (appointments != null)
                {
                    List<MVC_BLL.Models.Appointment> bllAppointments = new List<Models.Appointment>();
                    foreach (var appointment in appointments)
                    {
                        Models.Appointment app = new MVC_BLL.Models.Appointment();
                        app.Id = appointment.Id;
                        app.DateReserved = appointment.DateReserved;
                        app.Notes = appointment.Notes;
                        app.ClientId = appointment.ClientId;
                        bllAppointments.Add(app);
                    }
                    return bllAppointments;
                }
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in GetAll Appointments method: {ex.Message}");
            }

            return null;
        }
        public async Task<bool> CreateAppointmentAsync(MVC_BLL.Models.Requests.AppointmentAddModel appointment, int userId)
        {
            try
            {
                var existClient = await _unitOfWork.AppointmentRepository.GetAppointmentsByClientIdAsync(appointment.ClientId);
                if (existClient == null)
                {
                    return false;
                }
                if (appointment.ClientId != userId)
                {
                    throw new StandardViewResponseException("Nuk jeni i autorizuar per te modifikuar kete llogari!");
                }

                var added = _unitOfWork.AppointmentRepository.Add(new DAL.Models.Appointment
                {
                    DateReserved = appointment.DateReserved,
                    Notes = appointment.Notes,
                    ClientId = appointment.ClientId,
                });
                _unitOfWork.Commit();
                OnLogOccured?.Invoke(new DAL.Models.AuditLog
                {
                    Description = "A new Appointment was Generated",
                    Name = "Appointment",
                    ObjectId = appointment.ClientId
                });
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in CreateAppointment method: {ex.Message}");

            }
        
            return true;
        }

        public bool DeleteAppointment(int id)
        {
            try
            {
                var app = _unitOfWork.AppointmentRepository.GetById(id);
                if (app.DateReserved <= DateTime.Now)
                {
                    return false;
                }
                var deletedBill = _unitOfWork.BillingsRepository.DeleteByAppointmentID(id);
                var deletedAppointment = _unitOfWork.AppointmentRepository.Delete(id);
                if (deletedAppointment && deletedBill)
                {
                    _unitOfWork.Commit();
                    OnLogOccured?.Invoke(new DAL.Models.AuditLog
                    {
                        Description = "A Appointment was Deleted",
                        Name = "Apointment",
                        ObjectId = id
                    });
                    OnLogOccured?.Invoke(new DAL.Models.AuditLog
                    {
                        Description = "A Billing was Deleted",
                        Name = "Billing",
                        ObjectId = id
                    });
                    return true;
                }
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in DeleteAppointment method: {ex.Message}");

            }
            return false;
        }



        public Models.Appointment GetAppointmentById(int id)
        {
            try
            {
                var dalAppointment = _unitOfWork.AppointmentRepository.GetById(id);
                if (dalAppointment == null || dalAppointment.Id == 0)
                {
                    return null;
                }
                var bllAppointment = new Models.Appointment
                {
                    Id = dalAppointment.Id,
                    DateReserved = dalAppointment.DateReserved,
                    Notes = dalAppointment.Notes,
                    ClientId = dalAppointment.ClientId,

                };
                return bllAppointment;
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in GetAppointmentById method: {ex.Message}");
                return null;
            }
        }

        public bool UpdateAppointment(int id, MVC_BLL.Models.Appointment appointment, int userId)
        {
            try
            {
                var exist = _unitOfWork.AppointmentRepository.GetById(id);
                if (exist == null)
                {
                    return false;
                }
                if (appointment.ClientId != userId)
                {
                    throw new StandardViewResponseException("Nuk jeni i autorizuar per te modifikuar kete llogari!");
                }
                exist.Id = appointment.Id;
                exist.DateReserved = appointment.DateReserved;
                exist.Notes = appointment.Notes;
                exist.ClientId = appointment.ClientId;
                _unitOfWork.Commit();
                OnLogOccured?.Invoke(new DAL.Models.AuditLog
                {
                    Description = "An Appointment was Updated",
                    Name = "Appointment",
                    ObjectId = appointment.Id
                });
                return true;

            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in UpdateAppointment method: {ex.Message}");
                return false;
            }

        }

        public async Task<List<Models.Appointment>> GetAppointemntsByClientIdAsync(int id, int userId)
        {
            List<Models.Appointment> appointmentsList = new List<Models.Appointment>();
            try
            {
                var dalAppointments = await _unitOfWork.AppointmentRepository.GetAppointmentsByClientIdAsync(id);
                if (dalAppointments == null)
                {
                    return null;
                }
                if (id != userId)
                {
                    throw new StandardViewResponseException("Nuk jeni i autorizuar per te modifikuar kete llogari!");
                }
                foreach (var appointment in dalAppointments)
                {
                    Models.Appointment app = new Models.Appointment
                    {
                        Id = appointment.Id,
                        DateReserved = appointment.DateReserved,
                        Notes = appointment.Notes,
                        ClientId = appointment.ClientId,
                    };
                    appointmentsList.Add(app);
                }
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in GetAppointmetnsByClientIdAsync method: {ex.Message}");

            }
            return appointmentsList;
        }

        public bool ValidateDateAppointment(Models.Appointment appointment, List<Models.Appointment> appointmentsList)
        {
            try
            {
                if (appointment.DateReserved < DateTime.Now)
                {
                    return false;
                }
                if (appointment.DateReserved.TimeOfDay < TimeSpan.FromHours(7) || appointment.DateReserved.TimeOfDay
                    > TimeSpan.FromHours(20))
                {
                    return false;
                }
                foreach (var app in appointmentsList)
                {

                    if (app.DateReserved.Year == appointment.DateReserved.Year && app.DateReserved.Month == appointment.DateReserved.Month
                       && app.DateReserved.Day == appointment.DateReserved.Day && app.DateReserved.Hour == appointment.DateReserved.Hour

                       || app.DateReserved.Year == appointment.DateReserved.Year && app.DateReserved.Month == appointment.DateReserved.Month
                       && app.DateReserved.Day == appointment.DateReserved.Day && app.DateReserved.Hour + 1 == appointment.DateReserved.Hour
                        && app.DateReserved.Minute > appointment.DateReserved.Minute

                         || app.DateReserved.Year == appointment.DateReserved.Year && app.DateReserved.Month == appointment.DateReserved.Month
                       && app.DateReserved.Day == appointment.DateReserved.Day && app.DateReserved.Hour - 1 == appointment.DateReserved.Hour
                        && app.DateReserved.Minute != appointment.DateReserved.Minute

                        )
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in ValidateAppointment method: {ex.Message}");

            }
            return true;

        }
        public StandardViewResponse<bool> ValidateDateAppointmentEdited(Models.Appointment appointment, List<Models.Appointment> appointmentsList)
        {
            try
            {
                if (appointment.DateReserved < DateTime.Now)
                {
                    return new StandardViewResponse<bool>(false, "Appointment cannot be in the past!");
                }
                if (appointment.DateReserved.TimeOfDay < TimeSpan.FromHours(7) || appointment.DateReserved.TimeOfDay
                   > TimeSpan.FromHours(20))
                {
                    return new StandardViewResponse<bool>(false, "Appointment must be within working hours!");
                }
                if (appointment.DateReserved.TimeOfDay < TimeSpan.FromHours(7) || appointment.DateReserved.TimeOfDay
                   > TimeSpan.FromHours(20))
                {
                    return new StandardViewResponse<bool>(false, "Appointment must be between working hours!");
                }
                foreach (var app in appointmentsList)
                {
                    if (app.Id == appointment.Id)
                    {
                        continue;
                    }

                    if (app.DateReserved.Year == appointment.DateReserved.Year && app.DateReserved.Month == appointment.DateReserved.Month
                        && app.DateReserved.Day == appointment.DateReserved.Day && app.DateReserved.Hour == appointment.DateReserved.Hour

                        || app.DateReserved.Year == appointment.DateReserved.Year && app.DateReserved.Month == appointment.DateReserved.Month
                        && app.DateReserved.Day == appointment.DateReserved.Day && app.DateReserved.Hour + 1 == appointment.DateReserved.Hour
                         && app.DateReserved.Minute > appointment.DateReserved.Minute

                          || app.DateReserved.Year == appointment.DateReserved.Year && app.DateReserved.Month == appointment.DateReserved.Month
                        && app.DateReserved.Day == appointment.DateReserved.Day && app.DateReserved.Hour - 1 == appointment.DateReserved.Hour
                         && app.DateReserved.Minute != appointment.DateReserved.Minute

                         )
                    {
                        return new StandardViewResponse<bool>(false, "Another appointment is reserved in these hours!");
                    }

                }
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in ValidateAppointment method: {ex.Message}");

            }
            return new StandardViewResponse<bool>(true);

        }

        public StandardViewResponse<bool> ValidateDateAppointment(AppointmentAddModel appointment, List<Models.Appointment> appointmentsList)
        {
            try
            {
                if (appointment.DateReserved < DateTime.Now)
                {
                    return new StandardViewResponse<bool>(false, "Appointment date must be actual!");
                }
                if (appointment.DateReserved.TimeOfDay <= TimeSpan.FromHours(7) ||
                    appointment.DateReserved.TimeOfDay >= TimeSpan.FromHours(20))
                {
                    return new StandardViewResponse<bool>(false, "Appointment date must be during working hours!");
                }
                foreach (var app in appointmentsList)
                {

                    if (app.DateReserved.Year == appointment.DateReserved.Year && app.DateReserved.Month == appointment.DateReserved.Month
                       && app.DateReserved.Day == appointment.DateReserved.Day && app.DateReserved.Hour == appointment.DateReserved.Hour

                       || app.DateReserved.Year == appointment.DateReserved.Year && app.DateReserved.Month == appointment.DateReserved.Month
                       && app.DateReserved.Day == appointment.DateReserved.Day && app.DateReserved.Hour + 1 == appointment.DateReserved.Hour
                        && app.DateReserved.Minute > appointment.DateReserved.Minute

                         || app.DateReserved.Year == appointment.DateReserved.Year && app.DateReserved.Month == appointment.DateReserved.Month
                       && app.DateReserved.Day == appointment.DateReserved.Day && app.DateReserved.Hour - 1 == appointment.DateReserved.Hour
                        && app.DateReserved.Minute != appointment.DateReserved.Minute

                        )
                    {
                        return new StandardViewResponse<bool>(false, "Another appointment is schaduled in these hours!");
                    }

                }
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in ValidateAppointment method: {ex.Message}");

            }
            return new StandardViewResponse<bool>(true);

        }

        public async Task GenerateBill(AppointmentAddModel appointment)
        {
            try
            {
                int id = await _unitOfWork.AppointmentRepository.GetAppointmentIdByDateAsync(appointment.DateReserved);
                if (appointment.Type == "Therapy session")
                {
                    _unitOfWork.BillingsRepository.Add(new DAL.Models.Billing
                    {
                        Amount = 50.00M,
                        AppointmentId = id,
                        ClientId = appointment.ClientId,
                        BillingDate = DateTime.Now,
                        Status = "Not Paid"

                    });
                    _unitOfWork.Commit();
                }
                else if (appointment.Type == "Consulting")
                {
                    _unitOfWork.BillingsRepository.Add(new DAL.Models.Billing
                    {
                        Amount = 75.00M,
                        AppointmentId = id,
                        ClientId = appointment.ClientId,
                        BillingDate = DateTime.Now,
                        Status = "Not Paid"
                    });
                    _unitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in GenerateBill method: {ex.Message}");

            }
        }


    }
}
