using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class Psychology_ClinicDBContext : IdentityDbContext<Client,Role,int> //DB Name
    {
        public Psychology_ClinicDBContext(DbContextOptions<Psychology_ClinicDBContext> options) : base(options)
        {

        }
        //Tables
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<Billing> Billings { get; set; }
        public virtual DbSet<Treatment> Treatments { get; set; }
        public virtual DbSet<Therapist> Therapists { get; set; }
		public DbSet<AuditLog> AuditLogs { get; set; }
         
        

    }
}
