using Homework10__Htmx_.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ContactsAppContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Contacts}/{action=Index}/{id?}")
    .WithStaticAssets();

var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ContactsAppContext>();

if (!db.Contacts.Any())
{
    db.Seed();
    db.SaveChanges();
}

app.Run();
