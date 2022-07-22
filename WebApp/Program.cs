using Crypto.Eth.Snapshot;
using Crypto.Eth.Snapshot.Model;
using DataManagement;
using WebApp.Models.Snapshot;
using WebApp.SnapshotUnits;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("DatabaseOptions"));
builder.Services.Configure<BlockchainOptions>(builder.Configuration.GetSection("BlockchainOptions"));
builder.Services.AddScoped<AddressUtils>();
builder.Services.AddScoped<SnapshotDataAccessObject<SnapshotBlock, List<ProcessedBlock>>>();
builder.Services.AddSingleton<TokenSnapshotUnit>();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
