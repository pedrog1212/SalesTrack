using SalesTrack.WebFrontend.Services;
using SalesTrack.KPIService;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using SalesTrack.CRM.Data;
using System.Configuration;
using SalesTrack.CRM.Services;

// load variables from .env into Environment
Env.Load();

string apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");


var builder = WebApplication.CreateBuilder(args);

// listen on all network interfaces, including inside a container.
builder.WebHost.UseUrls("http://0.0.0.0:80");

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);


Console.WriteLine($"***** API Base URL: {builder.Configuration["ApiBaseUrl"]}");
Console.WriteLine("***** Using connection string: " + builder.Configuration.GetConnectionString("DefaultConnection"));


// base address logic 
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
});

// SalesTrack.KPIService/OpenAiKpiService.cs
builder.Services.AddScoped<OpenAiKpiService>();

// SalesTrack.CRM/OrderInventoryService.cs to update Inventory by Sales Orders consumption
builder.Services.AddScoped<OrderInventoryService>();


// Register CrmDbContext in Program.cs
builder.Services.AddDbContext<CrmDbContext>(options =>

    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")) // CrmDbConnection 
    .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information)
);

// Register the HTTP Clients
builder.Services.AddHttpClient<CustomerApiClient>(client =>
{
    //client.BaseAddress = new Uri("https://localhost:7179");  // this matches the CRM API port
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
});

builder.Services.AddHttpClient<OrderApiClient>(client =>
{
    //client.BaseAddress = new Uri("https://localhost:7179"); // this matches the CRM API port
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
});

builder.Services.AddHttpClient<OrderStatusApiClient>(client =>
{
    //client.BaseAddress = new Uri("https://localhost:7179"); // this matches the CRM API port
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
});

builder.Services.AddHttpClient<OrderTypeApiClient>(client =>
{
    //client.BaseAddress = new Uri("https://localhost:7179"); // this matches the CRM API port
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
});

builder.Services.AddHttpClient<OrderItemsApiClient>(client =>
{
    //client.BaseAddress = new Uri("https://localhost:7179"); // this matches the CRM API port
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
});

builder.Services.AddHttpClient<InventoryApiClient>(client =>
{
    //client.BaseAddress = new Uri("https://localhost:7179"); // this matches the CRM API port
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseCors();

//app.MapControllers(); // Required to map [Route] based endpoints
app.MapDefaultControllerRoute(); // for view routes like /Home/Index


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

//app.MapGet("/", () => "Hello from Docker!");

app.Run();


