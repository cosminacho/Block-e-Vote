using AutoMapper;
using EVoting.Common.Blockchain;
using EVoting.Server.Data;
using EVoting.Server.Hubs;
using EVoting.Server.Models;
using EVoting.Server.Services.AuthService;
using EVoting.Server.Services.CAuthService;
using EVoting.Server.Services.NodeService;
using EVoting.Server.Utils;
using EVoting.Server.Utils.Seeders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace EVoting.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)

        {

            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequiredLength = 12;
                    options.User.RequireUniqueEmail = true;
                    // options.SignIn.RequireConfirmedAccount = true;
                    options.SignIn.RequireConfirmedEmail = true;
                    // options.SignIn.RequireConfirmedPhoneNumber = true;
                    // options.Stores.ProtectPersonalData = true; TODO: to check
                })
                .AddEntityFrameworkStores<EVotingDbContext>()
                .AddDefaultTokenProviders();

            services.AddDbContext<EVotingDbContext>(options =>
            {
                // options.UseSqlServer(Configuration.GetConnectionString("MsSQLDBConnection"));
                options.UseMySql(Configuration.GetConnectionString("MySQLDBConnection"));
            });

            services.AddAutoMapper(Assembly.GetAssembly(typeof(AMapperProfile)));


            services.AddSignalR().AddJsonProtocol();

            services.AddCors(options =>
            {
                options.AddPolicy("Policy", buiilder =>
                {
                    buiilder.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin().Build();

                });
            });

            services.AddTransient<CandidatesSeed>();


            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ICAuthService, CAuthService>();
            services.AddSingleton<INodeService, NodeService>();


            services.AddSingleton<Blockchain>();


            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CandidatesSeed candidatesSeed)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            candidatesSeed.SeedCandidates();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRouting();
            app.UseCors("Policy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<BlockchainHub>("/hub/blockchain");
            });
        }
    }
}
