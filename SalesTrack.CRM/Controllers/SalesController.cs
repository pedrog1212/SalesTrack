using Microsoft.AspNetCore.Mvc;
using Npgsql;
using SalesTrack.KPIService;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using SalesTrack.KPIService.Models;

namespace SalesTrack.CRM.Controllers
{
    public class SalesController : Controller
    {
        private readonly OpenAiKpiService _kpiService;

        public SalesController(OpenAiKpiService kpiService)
        {
            _kpiService = kpiService;
        }

        public async Task<IActionResult> Analyze()
        {
            string salesJson = await LoadSalesAsJson(); // your logic
            string result = await _kpiService.AnalyzeSalesAsync(salesJson);
            ViewBag.Analysis = result;
            return View();
        }


    private async Task<string> LoadSalesAsJson()
        {
            var salesData = new List<SaleItem>();

            // replace with your actual connection string
            string connectionString = "Host=localhost;Port=5432;Username=your_user;Password=your_password;Database=SalesTrack";

            using var conn = new NpgsqlConnection(connectionString);
            await conn.OpenAsync();

            // Adjust these column names to your real table
            string query = @"SELECT product_name, quantity, unit_price FROM sales_orders";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                salesData.Add(new SaleItem
                {
                    product = reader.GetString(0),
                    quantity = reader.GetInt32(1),
                    price = reader.GetDecimal(2)
                });
            }

            // serialize to JSON
            return JsonSerializer.Serialize(salesData);
        }

    }
}
