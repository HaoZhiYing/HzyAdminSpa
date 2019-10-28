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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using Admin.AppService;
    using Microsoft.AspNetCore.SpaServices;
    using VueCliMiddleware;

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

            #region ʹ�� ��ҳ��
            services.AddSpaStaticFiles(opt => opt.RootPath = "ClientApp/hzyAdminVue/dist");
            #endregion

            services
                .AddControllers(options =>
                {
                    options.Filters.Add(new AppService.AppExceptionFilter());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            #region AdminConfig
            services.AdminConfigureServices(Configuration);
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            #region ʹ�� ��ҳ��
            app.UseSpaStaticFiles();
            #endregion

            app.UseRouting();

            #region ʹ�ÿ��� ����: ͨ���ս��·�ɣ�CORS �м����������Ϊ�ڶ�UseRouting��UseEndpoints�ĵ���֮��ִ�С� ���ò���ȷ�������м��ֹͣ�������С�
            app.UseCors("ApiAny");
            #endregion

            #region AdminConfig
            app.AdminConfigure(env, applicationLifetime);
            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                #region ʹ�� ��ҳ��
                if (env.IsDevelopment())
                {
                    // Note: only use vuecliproxy in development. 
                    // Production should use "UseSpaStaticFiles()" and the webpack dist
                    endpoints.MapToVueCliProxy(
                        "{*path}",
                        new SpaOptions { SourcePath = "ClientApp/hzyAdminVue/" },
                        npmScript: (System.Diagnostics.Debugger.IsAttached) ? "serve" : null,
                        port: 6666,
                        regex: "Compiled successfully"
                        );
                }

                #endregion

            });

            #region ʹ�� ��ҳ�� .net core 2.2 

            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = "ClientApp/hzyAdminVue/";
            //    if (env.IsDevelopment())
            //    {
            //        //vue-cli-service serve
            //        //spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");//ʹ�������Ҫ�Լ����� vue ��Ŀ
            //        spa.UseVueCli(npmScript: "serve", port: 6666); //�Զ���������
            //    }
            //});

            #endregion


        }
    }
}
