using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MatchDayPlanner.Data;
using MatchDayPlanner.Areas.Identity.Data;
using MatchDayPlanner.Features.GameSchedules.Create;
using MatchDayPlanner.Core.Interfaces;
using MatchDayPlanner.DAL;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AuthDbConnection") ?? throw new InvalidOperationException("Connection string 'DbConnection' not found.");

builder.Services.AddDbContext<AuthDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<AuthDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddScoped<CreateGameScheduleValidator>();

builder.Services.AddScoped<CreateGameScheduleHandler>();

builder.Services.AddScoped<IGameScheduleRepository>(provider =>
{
    var configuration =
        provider.GetRequiredService<IConfiguration>();

    var connectionString =
        configuration.GetConnectionString("DbConnection")
        ?? throw new InvalidOperationException(
            "Connection string 'DbConnection' is missing.");

    return new GameScheduleRepository(connectionString);
});

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Team}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
