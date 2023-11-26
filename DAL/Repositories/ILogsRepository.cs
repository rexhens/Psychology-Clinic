using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Repositories
{
	public interface ILogsRepository : IBaseRepository<AuditLog, int>
	{
		Task AddAsync(Models.AuditLog audit);
	}
	public class LogsRepository : BaseRepository<AuditLog, int>, ILogsRepository
	{
		private readonly Psychology_ClinicDBContext _dbContext;
		public LogsRepository(Psychology_ClinicDBContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(AuditLog audit)
		{
			 await _dbContext.AddAsync(audit);
		}
	}
}
