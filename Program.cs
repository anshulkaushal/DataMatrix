using DataMatrix.Services;

var builder = WebApplication.CreateBuilder(args);

// 1) Force DataDirectory to a writable App Service location
//    This ensures |DataDirectory| doesn't resolve inside wwwroot and avoids DB under wwwroot\umbraco\Data
var home = Environment.GetEnvironmentVariable("HOME"); // usually "C:\home"
var dataDir = !string.IsNullOrWhiteSpace(home)
    ? Path.Combine(home, "site", "data", "umbraco")
    : Path.Combine(AppContext.BaseDirectory, "App_Data", "umbraco");

Directory.CreateDirectory(dataDir);
AppContext.SetData("DataDirectory", dataDir);

// 2) Ensure WebRoot is correct (prevents wwwroot\wwwroot issues)
builder.WebHost.UseWebRoot("wwwroot");

// 3) OPTIONAL: If you have a bad publish nesting, this helps align content root
// builder.WebHost.UseContentRoot(AppContext.BaseDirectory);

builder.Services.AddScoped<ISqliteFormService, SqliteFormService>();

builder.CreateUmbracoBuilder()
    .AddBackOffice()
    .AddWebsite()
    .AddComposers()
    .Build();

var app = builder.Build();

await app.BootUmbracoAsync();

// 4) Ensure media folder exists (ONLY helps if your deployment area is writable)
// If WEBSITE_RUN_FROM_PACKAGE=1, this may fail silently or not persist.
try
{
    var mediaPath = Path.Combine(app.Environment.WebRootPath ?? "", "media");
    if (!string.IsNullOrWhiteSpace(app.Environment.WebRootPath))
        Directory.CreateDirectory(mediaPath);
}
catch
{
    // Intentionally ignore to avoid blocking startup on read-only filesystems
}

app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
    })
    .WithEndpoints(u =>
    {
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });

await app.RunAsync();
