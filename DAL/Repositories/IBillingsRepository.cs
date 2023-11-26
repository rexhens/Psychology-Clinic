using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IBillingsRepository : IBaseRepository<Billing,int>
    {
        IEnumerable<Billing> GetBillingByClientId (int ClientId);
        IEnumerable<Billing> GetBillingByAppointmentId(int appointmentId);
        bool DeleteByAppointmentID(int appointmentId);
        void DeleteByClientId(int id);
    }
    internal class BillingsRepository : BaseRepository<Billing,int>, IBillingsRepository
    {
        private readonly Psychology_ClinicDBContext _dbContext;
        public BillingsRepository(Psychology_ClinicDBContext dBContext) : base(dBContext)
        {
            _dbContext = dBContext;
        }

        public bool DeleteByAppointmentID(int appointmentId)
        {
            var deletedBill = _dbContext.Billings.FirstOrDefault(x=>x.AppointmentId == appointmentId);
            _dbContext.Billings.Remove(deletedBill);
            return true;
            
        }

        public void DeleteByClientId(int id)
        {
            var deletedBill = _dbContext.Billings.FirstOrDefault(x => x.ClientId == id);
            _dbContext.Billings.Remove(deletedBill);
           
        }

        public IEnumerable<Billing> GetBillingByAppointmentId(int appointmentId)
        {
            return _dbContext.Billings.Where(x => x.AppointmentId == appointmentId).ToList();
        }

        public IEnumerable<Billing> GetBillingByClientId(int ClientId)
        {
            return _dbContext.Billings.Where(x => x.ClientId == ClientId).ToList();   
        }
    }
}
