using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication1.Services.Interfaces;
using WebApplication1.Services.Repositories;
using Google.Cloud.SecretManager.V1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Google.Cloud.Diagnostics.AspNetCore;

namespace WebApplication1
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddGoogleExceptionLogging(options =>
            {
                options.ProjectId = "pfc2021";
                options.ServiceName = "PFC2021SWD63a";
                options.Version = "0.01";
            });

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddControllersWithViews();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddControllersWithViews();
            services.AddRazorPages();

            string clientId = GetGoogleClientId();

            services.AddAuthentication()
                   .AddGoogle(options =>
                   {
                       IConfigurationSection googleAuthNSection =
                           Configuration.GetSection("Authentication:Google");

                       options.ClientId = clientId;
                       options.ClientSecret = googleAuthNSection["ClientSecret"];
                   });


            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
            });

            //Blogsrepository is to be initialized whenever there is a request for IBlogsRepository
            services.AddScoped<IBlogsRepository, BlogsFirestoreRepository>();
            services.AddScoped<IPostsRepository, PostsRepository>();
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<IPubSubRepository, PubSubRepository>();
            services.AddScoped<ILogRepository, LogRepository>();

          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();


            app.UseGoogleExceptionLogging();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }


        /*
         * to store in the secret manager
         * 
         * connectionstring
         * oAuth2.0 client id + secret key
         * 3rd party api key using to send emails
         * 
         */ 


        public string GetGoogleClientId()
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Build the resource name.
            SecretVersionName secretVersionName = new SecretVersionName("pfc2021", "ApiClientId", "1" );

            // Call the API.
            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

            // Convert the payload to a string. Payloads are bytes by default.
            String payload = result.Payload.Data.ToStringUtf8();

            dynamic keys = JsonConvert.DeserializeObject(payload);
            
            JObject jObject = JObject.Parse(payload);
            JToken jKey = jObject["Authentication:Google:ClientId"];
            return jKey.ToString();
             
        }
    }
}
