
namespace test
{
    using AttRest;
    using AttRest.Enum;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

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
            //加入AttRest插件
            services.AddAttRest(AttClientFrame.Vue|AttClientFrame.React|AttClientFrame.Angular2|AttClientFrame.JQuery,typeof(Program).Assembly);
            //必须允许跨域
            services.AddCors(options =>
            {

                options.AddPolicy("any", builder =>
                {
                    builder.WithMethods("GET", "POST", "HEAD", "PUT", "DELETE", "OPTIONS")
                       .AllowAnyOrigin().AllowAnyHeader(); //允许任何来源的主机访问
                                          //.AllowCredentials()//指定处理cookie
                });
            });
            //添加控制器
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<CorsMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("AllowAllOrigins");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireCors("any");
            });
        }
    }
}
