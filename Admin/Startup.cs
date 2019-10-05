using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Admin
{
    using System.Text.Json;
    using Admin.AppService;
    using VueCliMiddleware;

    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment _IWebHostEnvironment)
        {
            Configuration = configuration;
            env = _IWebHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region �������� ���ÿ�����
            services.AddCors(options =>
            {
                options.AddPolicy("ApiAny", builder =>
                {
                    builder.WithOrigins("*")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    //.AllowAnyOrigin()
                    //.AllowCredentials();
                });
            });
            #endregion

            services
                .AddControllersWithViews(options =>
                {
                    options.Filters.Add(new AppService.AppExceptionFilter());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            #region AdminConfig
            services.AdminConfigureServices(Configuration, env);
            #endregion

            #region ʹ�� ��ҳ��
            //In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/hzyAdminVue/dist";
            });
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHostApplicationLifetime applicationLifetime)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            #region ʹ�� ��ҳ��
            //app.UseSpaStaticFiles();
            #endregion

            app.UseRouting();

            #region ʹ�ÿ��� ����: ͨ���ս��·�ɣ�CORS �м����������Ϊ�ڶ�UseRouting��UseEndpoints�ĵ���֮��ִ�С� ���ò���ȷ�������м��ֹͣ�������С�
            app.UseCors("ApiAny");
            #endregion

            #region AdminConfig
            app.AdminConfigure(env, loggerFactory, applicationLifetime);
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            #region ʹ�� ��ҳ��

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp/hzyAdminVue/";
                if (env.IsDevelopment())
                {
                    //vue-cli-service serve
                    //spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");//ʹ�������Ҫ�Լ����� vue ��Ŀ
                    spa.UseVueCli(npmScript: "serve", port: 6666); //�Զ���������
                }
            });

            #endregion


        }
    }
}
