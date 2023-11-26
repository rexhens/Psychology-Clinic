using DAL;
using DAL.Models;
using DAL.Repositories;
using MVC_BLL.Models;
using MVC_BLL.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL.Services
{
	public interface ITreatmentsService
	{
		bool Add(TreatmentAddModel model);
		public bool Add(MVC_BLL.Models.Treatment model);
		bool Update(Models.Treatment model);
		bool Delete(int id);
		Task<Models.Client> GetClientAsync(int id, int userId);
		List<Models.Treatment> GetAll();
		Models.Treatment GetById(int id);
		Task<TreatmentAddModel> GetByIdAddModelAsync(int id);
		public StandardViewResponse<bool> ValidateTreatment(Models.Treatment model);
		public StandardViewResponse<bool> ValidateTreatment(TreatmentAddModel model);
		Task<Models.Treatment> GetTreatmentByClientId(int id, int userId);
	}
	public class TreatmentsService : BaseService, ITreatmentsService
	{
		public static event IWriteLog OnLogOccured;
		public TreatmentsService(IServiceProvider unitOfWork) : base(unitOfWork)
		{

		}
		public bool Add(TreatmentAddModel model)
		{
			try
			{
				var added = _unitOfWork.TreatmentRepository.Add(
					new DAL.Models.Treatment
					{
						StartDate = model.StartDate,
						EndDate = model.EndDate,
						ClientId = model.ClientId,
						Description = model.Description,
						Cause = model.Cause,

					});
				_unitOfWork.Commit();
				OnLogOccured?.Invoke(new DAL.Models.AuditLog
				{
					Description = "A Treatment was added",
					Name = "Treatment",
					ObjectId = model.ClientId
				});
				return true;

			}
			catch (Exception ex)
            {
                _loggerService.LogError($"Error in Add Treatment method: {ex.Message}");

            }
            return false;
		}
		public bool Add(MVC_BLL.Models.Treatment model)
		{
			try
			{
				var treatment = new MVC_BLL.Models.Treatment
				{
					StartDate = model.StartDate,
					EndDate = model.EndDate,
					ClientId = model.ClientId,
					Description = model.Description,
					Cause = model.Cause,

				};
				var validate = ValidateTreatment(model);
				if (validate.Status == ViewResponseStatus.Ok)
				{
					var added = _unitOfWork.TreatmentRepository.Add(
						new DAL.Models.Treatment
						{
							StartDate = model.StartDate,
							EndDate = model.EndDate,
							ClientId = model.ClientId,
							Description = model.Description,
							Cause = model.Cause,

						});

					_unitOfWork.Commit();
					OnLogOccured?.Invoke(new DAL.Models.AuditLog
					{
						Description = "A Treatment was added",
						Name = "Treatment",
						ObjectId = model.ClientId
					});

					return true;
				}

			}
			catch (Exception ex)
            {
                _loggerService.LogError($"Error in Add Treatment method: {ex.Message}");

            }
            return false;
		}

		public bool Delete(int id)
		{
			try
			{
				var deleted = _unitOfWork.TreatmentRepository.Delete(id);
				if (deleted)
				{
					_unitOfWork.Commit();
					OnLogOccured?.Invoke(new DAL.Models.AuditLog
					{
						Description = "A Treatment was deleted",
						Name = "Treatment",
						ObjectId = id
					});
					return true;
				}
			}
			catch (Exception ex)
            {
                _loggerService.LogError($"Error in Delete Treatment method: {ex.Message}");

            }
            return false;
		}

		public List<Models.Treatment> GetAll()
		{
			try
			{
				var dalTreatments = _unitOfWork.TreatmentRepository.GetAll();
				var result = new List<Models.Treatment>();
				foreach (var treatment in dalTreatments)
				{
					result.Add(new Models.Treatment
					{
						Id = treatment.Id,
						Description = treatment.Description,
						Cause = treatment.Cause,
						StartDate = treatment.StartDate,
						EndDate = treatment.EndDate,
						ClientId = treatment.ClientId,

					});
				}
				if (result != null)
				{
					return result;
				}
			}
			catch(Exception ex)
			{
                _loggerService.LogError($"Error in GetAll method: {ex.Message}");

            }
            return null;
		}

		public async Task<Models.Client> GetClientAsync(int id, int userId)
		{
			try
			{

				var treatment = await _unitOfWork.TreatmentRepository.GetByClientIdAsync(id);
				if (treatment.ClientId != userId)
				{
					throw new StandardViewResponseException("Nuk jeni i autorizuar per te modifikuar kete llogari!");
				}
				var client = await _unitOfWork.ClientsRepository.GetByIdAsync(id);
				Models.Client result = new Models.Client
				{
					Id = client.Id,
					Name = client.Name,
					Surname = client.Surname,
					Email = client.Email,
					PhoneNum = client.PhoneNumber,
					ImgUrlProfilePhoto = client.ImgUrlProfilePic,
					TherapistLicenseNo = client.TherapistLicenseNo = 1,
				};
				return result;
			}
			catch (Exception ex)
            {
                _loggerService.LogError($"Error in GetTClientById method: {ex.Message}");

            }
            return null;
		}

		public Models.Treatment GetById(int id)
		{
			try
			{
				var treatment = _unitOfWork.TreatmentRepository.GetById(id);
				if (treatment != null)
				{
					Models.Treatment bllTreatment = new Models.Treatment
					{
						Id = treatment.Id,
						ClientId = treatment.ClientId,
						Description = treatment.Description,
						StartDate = treatment.StartDate,
						EndDate = treatment.EndDate,
						Cause = treatment.Cause,

					};
					return bllTreatment;
				}
			}
			catch(Exception ex)
			{
                _loggerService.LogError($"Error in GetTreatmentById method: {ex.Message}");

            }
            return null;
		}

		public async Task<TreatmentAddModel> GetByIdAddModelAsync(int id)
		{
			try
			{
				var treatment = await _unitOfWork.TreatmentRepository.GetByClientIdAsync(id);
				if (treatment != null)
				{
					TreatmentAddModel bllTreatment = new TreatmentAddModel
					{
						ClientId = treatment.ClientId,
						Description = treatment.Description,
						StartDate = treatment.StartDate,
						EndDate = treatment.EndDate,
						Cause = treatment.Cause,
					};
					return bllTreatment;
				}
			}
			catch(Exception ex)
			{
                _loggerService.LogError($"Error in GetByIdAddModelAsync method: {ex.Message}");

            }
            return null;
		}

		public bool Update(Models.Treatment model)
		{
			try
			{
				var foundedTreatment = _unitOfWork.TreatmentRepository.GetById(model.Id);
				if (foundedTreatment == null)
				{
					return false;
				}

				foundedTreatment.Description = model.Description;
				foundedTreatment.EndDate = model.EndDate;
				foundedTreatment.StartDate = model.StartDate;
				foundedTreatment.Cause = model.Cause;
				_unitOfWork.Commit();
				OnLogOccured?.Invoke(new DAL.Models.AuditLog
				{
					Description = "A new Treatment was Updated",
					Name = "Treatment",
					ObjectId = foundedTreatment.Id
				});
				return true;
			}
			catch(Exception ex) { _loggerService.LogError($"Error in UpdateTreatment method: {ex.Message}");
                return false;

            }
        }

		public StandardViewResponse<bool> ValidateTreatment(Models.Treatment model)
		{
			try
			{
				if (model.StartDate < DateTime.Now)
				{
					return new StandardViewResponse<bool>(false, "Starting date cannot be in the past!");
				}
				else if (model.EndDate < DateTime.Now)
				{
					return new StandardViewResponse<bool>(false, "Ending date cannot be in the past!");
				}
				if (model.StartDate > model.EndDate)
				{
					return new StandardViewResponse<bool>(false, "Ending Date must be after starting date!");

				}
				var treatmentList = GetAll();
				foreach (Models.Treatment treatment in treatmentList)
				{
					if (treatment.ClientId == model.ClientId)
					{
						return new StandardViewResponse<bool>(false, "This client has already his/her treatment!");
					}
				}
			}
			catch(Exception ex)
			{
                _loggerService.LogError($"Error in ValidateTreatment method: {ex.Message}");

            }
            return new StandardViewResponse<bool>(true);
		}
		public StandardViewResponse<bool> ValidateTreatment(TreatmentAddModel model)
		{
			try
			{
				if (model.StartDate < DateTime.Now)
				{
					return new StandardViewResponse<bool>(false, "Starting date cannot be in the past!");
				}
				else if (model.EndDate < DateTime.Now)
				{
					return new StandardViewResponse<bool>(false, "Ending date cannot be in the past!");
				}
				if (model.StartDate > model.EndDate)
				{
					return new StandardViewResponse<bool>(false, "Ending Date must be after starting date!");

				}
				var treatmentList = GetAll();
				foreach (Models.Treatment treatment in treatmentList)
				{
					if (treatment.ClientId == model.ClientId)
					{
						return new StandardViewResponse<bool>(false, "This client has already his/her treatment!");
					}
				}
			}
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in ValidateTreatment method: {ex.Message}");
            }
            return new StandardViewResponse<bool>(true);
		}

		public async Task<Models.Treatment> GetTreatmentByClientId(int clientId, int userId)
		{
			try
			{
				if (clientId != userId)
				{
					throw new StandardViewResponseException("Nuk jeni i autorizuar per te modifikuar kete llogari!");
				}
				var dalTreatment = await _unitOfWork.TreatmentRepository.GetByClientIdAsync(clientId);
				if (dalTreatment != null)
				{
					var treatment = new Models.Treatment
					{
						Id = dalTreatment.Id,
						Cause = dalTreatment.Cause,
						StartDate = dalTreatment.StartDate,
						EndDate = dalTreatment.EndDate,
						ClientId = clientId
					};
					return treatment;
				}
			}
            catch (Exception ex)
            {
                _loggerService.LogError($"Error in GetTreatmentByClientId method: {ex.Message}");

            }
            return null;
		}
	}

}