using DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public interface IAppointmentsRepository : IBaseRepository<Appointment,int>
    {
      
        Task<List<Models.Appointment>> GetAppointmentsByClientIdAsync(int id);
        Task<int> GetAppointmentIdByDateAsync(DateTime date);
        void DeleteByClientId(int id);
    }
    internal class AppointmentRepository : BaseRepository<Appointment, int>, IAppointmentsRepository
    {
        private readonly Psychology_ClinicDBContext _dbContext;
        public AppointmentRepository(Psychology_ClinicDBContext dbContext) : base(dbContext) 
        {
            _dbContext = dbContext;
        }

        public void DeleteByClientId(int id)
        {
            var appointment = _dbContext.Appointments.FirstOrDefault(x => x.ClientId == id);
            _dbContext.Appointments.Remove(appointment);
        }

        public async Task<int> GetAppointmentIdByDateAsync(DateTime date)
        {
            var appointment = await _dbContext.Appointments
                .Where(x => x.DateReserved == date)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            if (appointment != 0)
            {
                return appointment;
            }

           
            return -1; 
        }


        public async Task<List<Appointment>> GetAppointmentsByClientIdAsync(int id)
        {
            return await _dbContext.Appointments.Where(x => x.ClientId == id).ToListAsync();
            
        }
    }


}
       
    

