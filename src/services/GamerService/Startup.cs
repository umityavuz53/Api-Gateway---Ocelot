namespace GamerService
{
    using System;
    using System.Text;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.IdentityModel.Tokens;

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
            SymmetricSecurityKey signInKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Security"]));
            string authenticationProviderKey = "TestKey";
            services.AddAuthentication(option => option.DefaultAuthenticateScheme = authenticationProviderKey)
                .AddJwtBearer(authenticationProviderKey, options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                                                                {
                                                                    ValidateIssuerSigningKey = true,
                                                                    IssuerSigningKey = signInKey,
                                                                    ValidateIssuer = true,
                                                                    ValidIssuer = Configuration["JWT:Issuer"],
                                                                    ValidateAudience = true,
                                                                    ValidAudience = Configuration["JWT:Audience"],
                                                                    ValidateLifetime = true,
                                                                    ClockSkew = TimeSpan.Zero,
                                                                    RequireExpirationTime = true
                                                                };
                    });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
