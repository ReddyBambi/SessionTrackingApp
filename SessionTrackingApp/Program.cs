
using SessionTrackingApp.Middleware;
using SessionTrackingApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

// Register repositories
builder.Services.AddScoped<SessionTrackingApp.Repositories.ISessionRepository, SessionTrackingApp.Repositories.SessionRepository>();

// Register services
builder.Services.AddScoped<SessionTrackerService>();
builder.Services.AddScoped<SessionTrackingApp.Services.ISessionService, SessionTrackingApp.Services.SessionService>();

// SQL Server-backed distributed cache
builder.Services.AddDistributedSqlServerCache(options =>
{
    options.ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.SchemaName = "sessionDB";
    options.TableName = "SessionCache";
});

// Session config
builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Set session timeout as needed
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseMiddleware<SessionTrackerMiddleware>();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();



















//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();
