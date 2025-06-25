using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SalesTrack.KPIService;
using System.Threading.Tasks;
using SalesTrack.KPIService.Models;

namespace SalesTrack.WebFrontend.Pages.KPI
{
    public class KPIDashboardModel : PageModel
    {
        private readonly OpenAiKpiService _kpiService;

        public string AnalysisResult { get; set; }

        public KPIDashboardModel(OpenAiKpiService kpiService)
        {
            _kpiService = kpiService;
        }

        public async Task OnGetAsync()
        {
            // normally you'd get real sales data from your DB
            string salesData = @"
                [
                  { ""product"": ""Laptop"", ""quantity"": 3, ""price"": 800 },
                  { ""product"": ""Mouse"", ""quantity"": 10, ""price"": 20 },
                  { ""product"": ""Monitor"", ""quantity"": 2, ""price"": 150 }
                ]
            ";

            AnalysisResult = await _kpiService.AnalyzeSalesAsync(salesData);
        }
    }
}
