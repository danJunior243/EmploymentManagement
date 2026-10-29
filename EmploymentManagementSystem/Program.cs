using EmploymentManagement.BLL;
using EmploymentManagement.BLL.Services;
using EmploymentManagement.Data;
using EmploymentManagement.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EmploymentManagementSystem.Seed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
// AddScoped veut dire qu'une nouvelle instance du service est créée pour chaque requête HTTP, et partagée pendant cette requête.
// C'est la durée de vie qui convient aux services qui utilisent un DbContext
builder.Services.AddScoped<EmployeService>();
builder.Services.AddScoped<DepartementService>();
builder.Services.AddScoped<CongeService>();
builder.Services.AddScoped<PaieService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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



app.Run();

