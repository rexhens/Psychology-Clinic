using DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
	public interface ITreatmentRepository : IBaseRepository<Treatment,int>
	{
        Task<bool> DeleteByClientIdAsync(int id);
        Task<Treatment> GetByClientIdAsync(int clientId);
        //public bool UpdateDatabaseRecord(Treatment t);

    }

    public class TreatmentRepository : BaseRepository<Treatment, int>, ITreatmentRepository
    {
        private readonly Psychology_ClinicDBContext _ClinicDBContext;
        public TreatmentRepository(Psychology_ClinicDBContext context) : base(context)
        {
            _ClinicDBContext = context;
        }

        public async Task<bool> DeleteByClientIdAsync(int id)
        {
            Treatment exist = await _ClinicDBContext.Treatments.FirstAsync(x => x.ClientId == id);
            if(exist != null)
            {
                 _ClinicDBContext.Treatments.Remove(exist);
            }
            return true;
        }

		public async Task<Treatment> GetByClientIdAsync(int clientId)
		{
			return await _ClinicDBContext.Treatments.FirstOrDefaultAsync(x => x.ClientId == clientId);
		}

	
	}
}
