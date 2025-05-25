///************************************************************************
///	|| Creation History ||
///-----------------------------------------------------------------------
///	Copyright     :	Copyright© NAAS Solutions Limited. All rights reserved.
///	Author	      :	Arafat Hossain
///	Purpose	      :	Add middleware and dependencies
///	Creation Date :	13-Dec-2023
/// =======================================================================
///  || Modification History ||
///  ----------------------------------------------------------------------
///  Sl No.	Date:		    Author:			    Ver:	    Area of Change:
///  1.     
///	 ----------------------------------------------------------------------
///	***********************************************************************

using Domain.StaticClass;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using RetailerSelfCareApi.Middlewares;
using System.Globalization;
using System.IO.Compression;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

namespace RetailerSelfCareApi
{
    public class Startup
    {
        internal static void ConfigureServices(IServiceCollection service, bool isWindows)
        {
            service.AddResponseCompression(option =>
            {
                option.EnableForHttps = true;
                option.Providers.Add<BrotliCompressionProvider>();
                //option.Providers.Add<GzipCompressionProvider>();
            });

            service.Configure<BrotliCompressionProviderOptions>(option =>
            {
                option.Level = CompressionLevel.Fastest;
            });

            //service.Configure<GzipCompressionProviderOptions>(option =>
            //{
            //    option.Level = CompressionLevel.Fastest;
            //});

            //For Nginx Deploy
            if (!isWindows)
            {
                service.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders =
                        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                });
            }

            service.AddControllersWithViews()
                .AddMvcOptions((options) =>
                {
                    options.Filters.Add(new ConsumesAttribute(MimeTypes.Json, [MimeTypes.X_WwW_Form_UrlEncoded]));
                });

            service.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            service.AddEndpointsApiExplorer();
            service.AddLogging();

            service.AddLocalization();

            // Mapster Maping
            service.RegisterMapping();

            // pdf converter to DI
            service.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
            service.Configure<RouteOptions>(options =>
            {
                options.AppendTrailingSlash = true;
            });
        }


        internal static void ConfigureMethod(WebApplication app, IWebHostEnvironment webHostEnvironment, bool isWindows)
        {
            //For Nginx Deploy
            if (!isWindows)
            {
                app.UseForwardedHeaders();
            }

            RequestLocalizationOptions localizationOptions = new();

            CultureInfo[] supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("bn-BD")
            };

            localizationOptions.SupportedCultures = supportedCultures;
            localizationOptions.SupportedUICultures = supportedCultures;
            localizationOptions.SetDefaultCulture("en-US");
            localizationOptions.ApplyCurrentCultureToResponseHeaders = true;

            app.UseResponseCompression();

            app.UseRequestLocalization(localizationOptions);

            if (webHostEnvironment.IsDevelopment())
            {

            }

            if (webHostEnvironment.IsProduction())
            {
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseMiddleware<GlobalExceptionHandler>();

            app.UseMiddleware<JweMiddleware>();

            app.MapControllers();

            app.UseRouting();

            //#pragma warning disable ASP0014 // Suggest using top level route registrations
            //            app.UseEndpoints(endpoints =>
            //            {
            //                endpoints.MapGet("/", context =>
            //                {
            //                    context.Response.Redirect("home/index");
            //                    return Task.CompletedTask;
            //                });
            //            });
            //#pragma warning restore ASP0014 // Suggest using top level route registrations


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

        }
    }
}