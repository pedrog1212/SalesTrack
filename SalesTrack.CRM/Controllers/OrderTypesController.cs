using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesTrack.CRM.Data;
using SalesTrack.CRM.Models;
using SalesTrack.Shared.DTOs;

namespace SalesTrack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderTypesController : ControllerBase
    {
        private readonly CrmDbContext _context;

        public OrderTypesController(CrmDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderTypeDto>>> GetOrderTypes()
        {
            var types = await _context.OrderTypes
                .Select(t => new OrderTypeDto
                {
                    Id = t.Id,
                    TypeName = t.TypeName
                })
                .ToListAsync();

            return Ok(types);
        }
    }
}
