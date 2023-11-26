using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
	public interface IUnitOfWork
	{
		IAppointmentsRepository AppointmentRepository { get; }
		IBillingsRepository BillingsRepository { get; }
		ITreatmentRepository TreatmentRepository { get; }
		ITherapistRepository TherapistRepository { get; }
		ILogsRepository LogsRepository { get; }
		IClientsRepository ClientsRepository { get; }
		Task<T> ExecuteTransaction<T>(Func<Task<T>> action);

		void Commit();
	}
	internal class UnitOfWork : IUnitOfWork
	{

		private readonly Psychology_ClinicDBContext _clientsDBContext;
		public UnitOfWork(Psychology_ClinicDBContext clientContext)
		{
			_clientsDBContext = clientContext;
		}
		private IClientsRepository _clientsRepository;
		public IClientsRepository ClientsRepository { get { 
				if (_clientsRepository == null)
				{
					_clientsRepository = new ClientRepository(_clientsDBContext);
				}
				return _clientsRepository; } }

		private IAppointmentsRepository _appointmentRepository;
		public IAppointmentsRepository AppointmentRepository
		{
			get
			{
				if (_appointmentRepository == null)
				{
					_appointmentRepository = new AppointmentRepository(_clientsDBContext);
				}
				return _appointmentRepository;

			}
		}

		private IBillingsRepository _billingsRepository;
		public IBillingsRepository BillingsRepository
		{
			get
			{
				if (_billingsRepository == null)
				{
					_billingsRepository = new BillingsRepository(_clientsDBContext);
				}
				return _billingsRepository;
			}
		}
		private ITreatmentRepository _treatmentRepository;
		public ITreatmentRepository TreatmentRepository
		{
			get
			{
				if (_treatmentRepository == null)
				{
					_treatmentRepository = new TreatmentRepository(_clientsDBContext);
				}
				return _treatmentRepository;
			}

		}

		private ITherapistRepository therapistRepository;
		public ITherapistRepository TherapistRepository
		{
			get
			{
				if (therapistRepository == null)
				{
					therapistRepository = new TherapistRepository(_clientsDBContext);
				}
				return therapistRepository;
			}
			set
			{
				therapistRepository = value;
			}
		}

		private ILogsRepository _logsRepository;
		public ILogsRepository LogsRepository
		{
			get
			{
				_logsRepository ??= new LogsRepository(_clientsDBContext);
				return _logsRepository;
			}
		}
        public void Commit()
		{
			_clientsDBContext.SaveChanges();
		}

        public async Task<T> ExecuteTransaction<T>(Func<Task<T>> action)
        {
            var transaction = await _clientsDBContext.Database.BeginTransactionAsync();
            try
            {
                var result = await action();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
