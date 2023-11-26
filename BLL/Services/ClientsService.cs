using DAL;
using DAL.Repositories;
using MVC_BLL.Models;
using MVC_BLL.Models.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MVC_BLL.Services
{
	public interface IClientService
	{
		List<MVC_BLL.Models.Client> GetAll();
		Task<Client> GetByIdAsync(int id);
		Task<StandardViewResponse<ClientAddModel>> GetByIdClientAddModelAsync(int id);
		Task<StandardViewResponse<bool>> AddClientAsync(ClientAddModel client);
		Task<StandardViewResponse<bool>> DeleteClientAsync(int id);
		Task<StandardViewResponse<bool>> EditClientAsync(Client client, int id);
		StandardViewResponse<bool> ValidateClient(ClientAddModel client);
	}

	public class ClientsService : BaseService, IClientService
	{


		public static event IWriteLog OnLogOccured;

		public ClientsService(IServiceProvider unitOfWork) : base(unitOfWork)
		{

		}
		public List<MVC_BLL.Models.Client> GetAll()
		{
			List<MVC_BLL.Models.Client> result = new List<MVC_BLL.Models.Client>();
			try
			{
				MVC_BLL.Models.Client clientResult = null;
				IEnumerable<DAL.Models.Client> dalClients = _unitOfWork.ClientsRepository.GetAll();
				foreach (var client in dalClients)
				{
					clientResult = new MVC_BLL.Models.Client
					{
						Id = client.Id,
						Name = client.Name,
						Surname = client.Surname,
						Email = client.Email,
						PhoneNum = client.PhoneNumber,
						TherapistLicenseNo = client.TherapistLicenseNo,
						UserName = client.UserName,
						ImgUrlProfilePhoto = client.ImgUrlProfilePic,

					};
					result.Add(clientResult);
				}
			}
			catch(Exception ex)
			{
                _loggerService.LogError($"Error in GetAll Clients method: {ex.Message}");
                throw new StandardViewResponseException("Cannot take any client from DB!");
			}
			return result;
		}
		public async Task<StandardViewResponse<bool>> AddClientAsync(ClientAddModel client)
		{
			try
			{
				DAL.Models.Client dalClient = null;
				dalClient = new DAL.Models.Client
				{

					Name = client.Name,
					Surname = client.Surname,
					Email = client.Email,
					PhoneNumber = client.PhoneNum,
					ImgUrlProfilePic = client.ImgUrlProfilePhoto,
					UserName = client.UserName,
					TherapistLicenseNo = 1
				};
				await _unitOfWork.ClientsRepository.AddAsync(dalClient);
				_unitOfWork.Commit();
				OnLogOccured?.Invoke(new DAL.Models.AuditLog
				{
					Description = "A client was added",
					Name = "Client",
					ObjectId = dalClient.Id
				});
				return new StandardViewResponse<bool>(true);
			}
			catch (Exception ex)
			{
                _loggerService.LogError($"Error in Add Clients method: {ex.Message}");
                return new StandardViewResponse<bool>(false, ex.Message);
			}
			//  return new StandardViewResponse<bool>(false, "Client could not be added!");           
		}
		public async Task<StandardViewResponse<bool>> EditClientAsync(Client client, int id)
		{
			try
			{
				var existClient = await _unitOfWork.ClientsRepository.GetByIdAsync(id);
				if (existClient != null)
				{
					var edited = await _unitOfWork.ClientsRepository.GetByIdAsync(id);
					edited.Id = client.Id;
					edited.Name = client.Name;
					edited.Surname = client.Surname;
					edited.Email = client.Email;
					edited.PhoneNumber = client.PhoneNum;
					edited.ImgUrlProfilePic = client.ImgUrlProfilePhoto;
					edited.TherapistLicenseNo = client.TherapistLicenseNo = 1;

					if (edited != null)
					{
						_unitOfWork.Commit();
						OnLogOccured?.Invoke(new DAL.Models.AuditLog
						{
							Description = "A client was edited",
							Name = "Client",
							ObjectId = edited.Id
						});
						return new StandardViewResponse<bool>(true);
					}

				}

			}
			catch (Exception ex)
			{
                _loggerService.LogError($"Error in Edit Clients method: {ex.Message}");
                return new StandardViewResponse<bool>(false, ex.Message);
			}
			return new StandardViewResponse<bool>(false, "Client could not be edited!");


		}
		public async Task<StandardViewResponse<bool>> DeleteClientAsync(int id)
		{
			try
			{
				var deleted = await _unitOfWork.ClientsRepository.DeleteAsync(id);
				var deletedAppointments = await _unitOfWork.AppointmentRepository.GetAppointmentsByClientIdAsync(id);
				foreach (var appointment in deletedAppointments)
				{
					if(appointment.DateReserved >= DateTime.UtcNow)
					{
						_unitOfWork.AppointmentRepository.DeleteByClientId(id);
						_unitOfWork.BillingsRepository.DeleteByAppointmentID(appointment.Id);
					}
				}
				if (deleted)
				{
					_unitOfWork.Commit();
					OnLogOccured?.Invoke(new DAL.Models.AuditLog
					{
						Description = "A client was deleted",
						Name = "Client",
						ObjectId = id
					});
					return new StandardViewResponse<bool>(true);
				}

			}
			catch (Exception ex)
			{
                _loggerService.LogError($"Error in Delete Clients method: {ex.Message}");
                return new StandardViewResponse<bool>(false, ex.Message);
			}
			return new StandardViewResponse<bool>(false, "The client could not be deleted!");
		}




		public async Task<Client> GetByIdAsync(int id)
		{
			try
			{


				var exist = await _unitOfWork.ClientsRepository.GetByIdAsync(id);
				if (exist != null)
				{
					Client client = new Client
					{
						Id = exist.Id,
						Name = exist.Name,
						Surname = exist.Surname,
						Email = exist.Email,
						PhoneNum = exist.PhoneNumber,
						ImgUrlProfilePhoto = exist.ImgUrlProfilePic,
						TherapistLicenseNo = exist.TherapistLicenseNo,
						UserName = exist.UserName,
					};
					return client;
				}
				else
				{
                    throw new StandardViewResponseException("The client does not exist!");
				}
			}
			catch (Exception ex)
			{
                _loggerService.LogError($"Error in GetById Clients method: {ex.Message}");
                return null;
			}

		}



		public async Task<StandardViewResponse<ClientAddModel>> GetByIdClientAddModelAsync(int id)
		{
			try
			{
				var exist = await _unitOfWork.ClientsRepository.GetByIdAsync(id);
				if (exist != null)
				{
					ClientAddModel client = new ClientAddModel()
					{

						Name = exist.Name,
						Surname = exist.Surname,
						Email = exist.Email,
						PhoneNum = exist.PhoneNumber,
						ImgUrlProfilePhoto = exist.ImgUrlProfilePic,
						UserName = exist.UserName,
					};
					return new StandardViewResponse<ClientAddModel>(client);
				}
				else
				{
					throw new StandardViewResponseException("The client does not exist!");
				}
			}
			catch (Exception ex)
			{
                _loggerService.LogError($"Error in GetByIdClientAddModelAsync method: {ex.Message}");
                return new StandardViewResponse<ClientAddModel>(null, ex.Message);
			}
		}

		public StandardViewResponse<bool> ValidateClient(ClientAddModel client)
		{
			try
			{
				var clientList = _unitOfWork.ClientsRepository.GetAll();
				foreach (var cl in clientList)
				{
					if (client.UserName == cl.UserName)
					{

						return new StandardViewResponse<bool>(false, "Another Client exists with the same UserName!");

					}
				}
			}
			catch (Exception ex)
			{
                _loggerService.LogError($"Error in ValidateClient method: {ex.Message}");
			}
            return new StandardViewResponse<bool>(true);

        }
    }
}
