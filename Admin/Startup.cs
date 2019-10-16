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
            services.AdminConfigureServices(Configuration);
            #endregion

            #region ʹ�� ��ҳ��
            services.AddSpaStaticFiles(opt => opt.RootPath = "ClientApp/hzyAdminVue/dist");
            #endregion

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");

                //endpoints.MapControllers();

                #region ʹ�� ��ҳ��
                // Note: only use vuecliproxy in development. 
                // Production should use "UseSpaStaticFiles()" and the webpack dist
                if (env.IsDevelopment())
                {
                    //endpoints.MapToVueCliProxy(
                    //                    "{*path}",
                    //                    new SpaOptions { SourcePath = "ClientApp/hzyAdminVue" },
                    //                    //npmScript: (System.Diagnostics.Debugger.IsAttached) ? "serve" : null,
                    //                    npmScript: "serve",
                    //                    port: 6666
                    //                    );
#if DEBUG
                    //if (System.Diagnostics.Debugger.IsAttached)
                    //    endpoints.MapToVueCliProxy("{*path}", new SpaOptions { SourcePath = "ClientApp/hzyAdminVue" }, "dev", port: 6666, regex: "����ɹ�!");



                    //if (System.Diagnostics.Debugger.IsAttached)
                    //endpoints.MapToVueCliProxy(
                    //                    "{*path}",
                    //                    new SpaOptions { SourcePath = "ClientApp/hzyAdminVue" },
                    //                    //npmScript: (System.Diagnostics.Debugger.IsAttached) ? "serve" : null,
                    //                    npmScript: "serve",
                    //                    port: 6666
                    //                    );


                    //else
#endif
                    // note: output of vue cli or quasar cli should be wwwroot
                    //endpoints.MapFallbackToFile("ClientApp/hzyAdminVue/public/index.html");

                }

                //#if DEBUG
                //                if (System.Diagnostics.Debugger.IsAttached)
                //                    //endpoints.MapToVueCliProxy("{*path}", new SpaOptions { SourcePath = "ClientApp" }, "dev", regex: "Compiled successfully");
                //                    endpoints.MapToVueCliProxy("{*path}", new SpaOptions { SourcePath = "ClientApp/hzyAdminVue" }, "dev", port: 6666, regex: "����ɹ�!");
                //                else
                //#endif
                //                    // note: output of vue cli or quasar cli should be wwwroot
                //                    endpoints.MapFallbackToFile("index.html");

                #endregion


            });



            #region ʹ�� ��ҳ�� .net core 2.2 

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
