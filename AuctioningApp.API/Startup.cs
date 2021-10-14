using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using AuctioningApp.Infrastructure.Context;
using AuctioningApp.Domain.RepositoryInterfaces;
using AuctioningApp.Domain.BLL.ServiceInterfaces;
using AuctioningApp.Infrastructure.MSSQL_Repositories;
using AuctioningApp.Domain.BLL.Services;
using AuctioningApp.API.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.SignalR;

namespace AuctioningApp.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var key = Encoding.ASCII.GetBytes(
                Configuration.GetSection("AppSettings:Token").Value
                );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddControllers();

            services.AddDbContext<MSSQL_Context>(options => options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=AuctionAppDb;Trusted_Connection=True;"));
            services.AddScoped<IAuctionsRepository, AuctionsRepository>();
            services.AddScoped<IBidsRepository, BidsRepository>();
            services.AddScoped<ICategoriesRepository, CategoriesRepository>();
            services.AddScoped<IProductsRepository, ProductsRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IAuctionsService, AuctionsService>();
            services.AddScoped<ICategoriesService, CategoriesService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<AuctionsHub, AuctionsHub>();
            services.AddScoped<ProductService, ProductService>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("http://localhost:3000")
                        .AllowCredentials());
            });
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseStaticFiles();

            GlobalHost.Configuration.DefaultMessageBufferSize = 500;

            app.UseEndpoints(endpoints => { 
                endpoints.MapControllers();
                endpoints.MapHub<AuctionsHub>("/auctionshub");
            });
        }
    }
}
