using DomainLayer;
using DomainLayer.Common;
using DomainLayer.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace TAILOR_SERVICES_APP.Areas.Customers.Controllers
{
    [Area("Customers")]
    [Route("api/customers")]
    [ApiController]
    public class CustomersController(ICustomerRepository repo) : ControllerBase
    {
        [HttpPost("grid")]
        public async Task<IActionResult> Grid([FromBody] DataSourceRequest request, CancellationToken ct)
            => Ok(await repo.GetGridAsync(request, ct));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken ct)
        {
            var result = await repo.GetByIdAsync(id, ct);
            return result.Succeeded ? Ok(result.Value) : NotFound(new { errors = result.Errors });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerSaveDto dto, CancellationToken ct)
        {
            var userId = "temp-user"; // replace with the logged-in user's id later
            var result = await repo.CreateAsync(dto, userId, ct);
            return result.Succeeded ? Ok() : BadRequest(new { errors = result.Errors });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CustomerSaveDto dto, CancellationToken ct)
        {
            var result = await repo.UpdateAsync(id, dto, ct);
            return result.Succeeded ? Ok() : BadRequest(new { errors = result.Errors });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id, CancellationToken ct)
        {
            var result = await repo.DeleteAsync(id, ct);
            return result.Succeeded ? Ok() : BadRequest(new { errors = result.Errors });
        }
    }
}