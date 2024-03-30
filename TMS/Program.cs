using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.CodeAnalysis.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;
using TMS.Auth.Contexts;
using TMS.Auth.Seeders;
using TMS.Business;
using TMS.Common.Helpers;
using TMS.Data.Entities;
using static TMS.Auth.ConfigHelper;
using static TMS.Data.ConfigureData;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .InjectAuthServices(builder.Configuration)
    .InjectData(builder.Configuration)
    .InjectBusiness()
    .AddScoped<SmsHelper>()
.AddScoped<InvoiceHelper>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization()
    .AddRazorRuntimeCompilation();

// Configure supported cultures
var supportedCultures = new[]
{
        new CultureInfo("en"),
        new CultureInfo("ur"),
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Add services to the container.
//builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var seederManager = builder.Services?.BuildServiceProvider()?.GetService<SeederManager>();
if (seederManager != null)
{
    await seederManager.SeedData();
}
app.Run();
