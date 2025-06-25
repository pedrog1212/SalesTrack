using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesTrack.CRM.Data;
using SalesTrack.Shared.DTOs;

namespace SalesTrack.CRM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderStatusesController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public OrderStatusesController(CrmDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderStatusDto>>> Get()
        {
            var statuses = await _context.OrderStatuses
                .Select(s => new OrderStatusDto
                {
                    Id = s.Id,
                    StatusName = s.StatusName
                })
                .ToListAsync();

            return Ok(statuses);
        }
    }
}
