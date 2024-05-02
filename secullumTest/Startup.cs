using secullumTest.infrastructure;
using secullumTest.Repositories;

namespace secullumTest {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            ConfigureEndpoints(app);
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();
            services.AddScoped<IRecordRepository, RecordRepository>();

            services.AddScoped<DbConnection>();
        }

        private static void ConfigureEndpoints(IApplicationBuilder app) {
            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapGet("/", async context => {
                    await context.Response.WriteAsync("Secullum");
                });
            });
        }
    }
}