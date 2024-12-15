using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SportsApp.Models;
using StoreApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

// Add DbContext for both your AppDbContext and IdentityDbContext
builder.Services.AddDbContext<RepositoryContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("sqlconnection"));
});

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("IdentityDBConnection"));
});

var app = builder.Build();

// Ensure roles and users are created on application start
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeedData.EnsurePopulated(services); // Ensure roles and users are populated
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication(); // Authentication middleware
app.UseAuthorization();  // Authorization middleware

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");

app.Run();
