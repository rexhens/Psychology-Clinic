using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace MVC_BLL.Services
{
	internal class LogsService : IHostedService
	{
		private readonly IWriteLog _logMethod;
		private readonly IServiceProvider _serviceProvider;
		public LogsService(IConfiguration configuration, IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
			var logMethod = configuration["ConnectionStrings:LogsConfig"];
			switch (logMethod)
			{
				case "Database":
					_logMethod = WriteLogToDatabase;
					break;
				default:
					throw new ArgumentOutOfRangeException("Uncorrect logger type");
			}
		}
		public async void WriteLogToDatabase(DAL.Models.AuditLog auditLog)
		{
			try
			{
				using (var scope = _serviceProvider.CreateScope())
				{
					var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
				    await unitOfWork.LogsRepository.AddAsync(auditLog);
					unitOfWork.Commit();
				}
			}
			catch
			{

			}
		}
		public Task StartAsync(CancellationToken cancellationToken)
		{
			ClientsService.OnLogOccured += _logMethod;
			TreatmentsService.OnLogOccured += _logMethod;
			BillingsService.OnLogOccured += _logMethod;
			AppointmentsService.OnLogOccured += _logMethod;
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			ClientsService.OnLogOccured -= _logMethod;
			TreatmentsService.OnLogOccured -= _logMethod;
			BillingsService.OnLogOccured -= _logMethod;
			AppointmentsService.OnLogOccured -= _logMethod;
			return Task.CompletedTask;
		}
	}
}
