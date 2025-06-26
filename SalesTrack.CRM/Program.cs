using SalesTrack.CRM.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SalesTrack.CRM.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CrmDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CrmDbConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SalesTrack API", Version = "v1" });
});

// Add DbContext, etc.

builder.Services.AddScoped<OrderInventoryService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SalesTrack API V1");
    });
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("https://salestrack.onrender.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

app.UseCors("AllowFrontend");


// backend routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHttpsRedirection();

app.UseRouting(); //required

app.UseAuthorization();

app.MapControllers(); // THIS IS MANDATORY

app.Run();

