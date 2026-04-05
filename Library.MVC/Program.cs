using Library.Domain.Enrichers;
using Library.MVC.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

// --- Build WebApplication builder ---
var builder = WebApplication.CreateBuilder(args);

// --- Add HttpContextAccessor for UserName enrichment ---
builder.Services.AddHttpContextAccessor();

// --- Configure Serilog ---
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "LibraryApp")
    .Enrich.WithEnvironmentName()
    .Enrich.With(new UserNameEnricher(builder.Services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>()))
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog(); // Replace default logger

// --- Add services ---
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// --- Add EF DbContext ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Add Identity ---
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// --- Global exception handling + friendly error pages ---
app.UseExceptionHandler("/Home/Error"); // Always use custom error page
app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}"); // Handle 404/403 etc

if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // only for non-development
}
else
{
    app.UseDeveloperExceptionPage(); // optional: shows stack trace
}

// --- Apply migrations and seed data ---
await using (var scope = app.Services.CreateAsyncScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();

        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await DbSeeder.SeedAsync(context, userManager, roleManager);

        Log.Information("Database migration and seeding completed successfully.");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred while migrating or seeding the database.");
        throw; // terminate if migration/seeding fails
    }
}

// --- Configure HTTP request pipeline ---
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// --- Routes ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// --- Startup logging ---
Log.Information("Application starting up.");

// --- Run app ---
try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}