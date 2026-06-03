using Microsoft.EntityFrameworkCore;
using InternPortal.Data;
using InternPortal.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=interns_v2.db"));

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Ensure migrations are applied if not run manually before (optional, but let's keep database seeded)
    if (!db.Mentors.Any())
    {
        db.Mentors.AddRange(
            new Mentor { Name = "Mentor 1" },
            new Mentor { Name = "Mentor 2" },
            new Mentor { Name = "Mentor 3" },
            new Mentor { Name = "Mentor 4" },
            new Mentor { Name = "Mentor 5" },
            new Mentor { Name = "Mentor 6" },
            new Mentor { Name = "Mentor 7" },
            new Mentor { Name = "Mentor 8" },
            new Mentor { Name = "Mentor 9" }
        );
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
