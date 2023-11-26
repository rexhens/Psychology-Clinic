using DAL.Models;
using DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DAL
{
    public class StartUp
    {

        public static void RegisterServices(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddRazorPages();
            serviceCollection.AddDbContext<Psychology_ClinicDBContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DBConString"));
            });
            serviceCollection.AddDefaultIdentity<Client>(options => options.SignIn.RequireConfirmedAccount = false)
                  .AddRoles<Role>()
                   .AddEntityFrameworkStores<Psychology_ClinicDBContext>();


        


            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
        }
        public static void Start(string ConnectionString)
        {
            Utils.ConnectionString = ConnectionString;
        }
            private static async Task CreateRoles(RoleManager<Role> roleManager)
            {
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    var role = new Role { Id = 2, Name = "Admin" };
                    await roleManager.CreateAsync(role);
                }
                // Add other roles if needed
            } 

    }
}
