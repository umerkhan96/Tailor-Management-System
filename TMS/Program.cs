using Microsoft.EntityFrameworkCore;
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
// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

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
