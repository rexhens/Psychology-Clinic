using DAL.Repositories;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
	public interface IClientsRepository
	{
		Task<Client> GetByIdAsync(int id);
		Task<bool> DeleteAsync(int id);
		Task<int> AddAsync(Client entity);
		IEnumerable<Client> GetAll();
		Client GetByName(string name);
		Task<Client> GetByUserNameAsync(string username);
	}
	internal class ClientRepository : IClientsRepository
	{
		private readonly DbSet<Client> _set;
		public ClientRepository(Psychology_ClinicDBContext dbContext)
		{
			_set = dbContext.Set<Client>();
		}

		public async Task<int> AddAsync(Client entity)
		{
			var ent = await _set.AddAsync(entity);
			if (ent != null)
			{
				return 1;
			}
			return 0;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await GetByIdAsync(id);
			_set.Remove(entity);
			return true;

		}

		public IEnumerable<Client> GetAll()
		{
			return _set
				.Select(x => new Client
				{
					Id = x.Id,
					Name = x.Name,
					PhoneNumber = x.PhoneNumber,
					Surname  = x.Surname,
				    Email = x.Email,
				    TherapistLicenseNo = x.TherapistLicenseNo,
				    ImgUrlProfilePic = x.ImgUrlProfilePic,
				    UserName = x.UserName,
					 PasswordHash = x.PasswordHash,
					  PhoneNumberConfirmed = x.PhoneNumberConfirmed,
				    
				})
				.ToList();
		}

        public async Task<Client> GetByUserNameAsync(string userName)
        {
            return await _set.FirstOrDefaultAsync(x =>x.UserName == userName);
        }

        public async Task<Client> GetByIdAsync(int id)
		{
			return await _set.FindAsync(id);
		}

		public Client GetByName(string name)
		{
			return _set.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
		}


	}
}
