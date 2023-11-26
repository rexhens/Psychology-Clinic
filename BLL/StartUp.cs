using MVC_BLL.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVC_BLL
{
    public class StartUp
    {

        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddRazorPages();
            DAL.StartUp.RegisterServices(services,configuration);
            services.AddScoped<IClientService, ClientsService>();
            services.AddScoped<IAppointmentService, AppointmentsService>();
            services.AddScoped<IBillingService,BillingsService>();
            services.AddScoped<ITreatmentsService, TreatmentsService>();
            services.AddScoped<ITherapistService, TherapistService>();
            services.AddHostedService<LogsService>();
            services.AddSingleton<ILoggerService, LoggerService>();
                
        }
        public static void Start(string ConnectionString,string LogsDirectory)
        {
            DAL.StartUp.Start(ConnectionString);
            Utils.LogsDirectory = LogsDirectory;
           
        }

    }
}
