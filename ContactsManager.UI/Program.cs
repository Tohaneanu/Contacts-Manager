using Serilog;
using Contacts_Manager;
using Contacts_Manager.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Default logging
//builder.Host.ConfigureLogging(loggingProvider =>
//{
//    loggingProvider.ClearProviders();
//    loggingProvider.AddConsole();
//});
//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration)//read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services);//read out current app's services and make them available to serilog
});

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}

app.UseSerilogRequestLogging();
//Http logging
app.UseHttpLogging();

if (!builder.Environment.IsEnvironment("Test"))
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");
}

app.UseStaticFiles();

app.UseRouting(); //identifying action method based route
app.UseAuthentication(); //reading identity cookie
app.UseAuthorization(); //validates access permissions of the user
app.MapControllers(); //execute the filter pipeline(action + filters)
////conventional routing for admin
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Home}/{action=Index}");
//    //Admin/Home/Index
//    //Admin
//});

app.Run();

public partial class Program { } //make the auto-generated Program accessible programmatically