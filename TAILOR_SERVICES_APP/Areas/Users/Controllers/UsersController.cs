using DomainLayer;
using DomainLayer.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TAILOR_SERVICES_APP.Areas.Users.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(ILogger<UsersController> logger,
        IUserRepository repository) : ControllerBase
    {
        // DataTables calls this (POST, JSON body)
        [HttpPost("datatable")]
        public async Task<IActionResult> DataTable([FromBody] DataTablesRequest request, CancellationToken ct)
        {
            try
            {
                var result = await repository.GetGridAsync(request.ToDataSourceRequest(), ct);
                return Ok(new DataTablesResponse
                {
                    Draw = request.Draw,
                    RecordsTotal = result.Total,
                    RecordsFiltered = result.Total,
                    Data = result.Data
                });
            }
            catch (OperationCanceledException)
            {
                return Ok(new DataTablesResponse { Draw = request.Draw });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Users DataTable failed");
                return Ok(new DataTablesResponse { Draw = request.Draw, Error = "Could not load users." });
            }
        }

        // Plain paged GET (optional): /api/users?page=1&pageSize=10
        [HttpGet("data")]
        public async Task<DataSourceResult> Get([FromQuery] DataSourceRequest request, CancellationToken ct)
        {
            try
            {
                return await repository.GetGridAsync(request, ct);
            }
            catch (OperationCanceledException)
            {
                return new DataSourceResult();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load users");
                return new DataSourceResult();
            }
        }

        // For dropdowns
        [HttpGet("list")]
        public async Task<IActionResult> List(CancellationToken ct)
        {
            var result = await repository.GetGridAsync(new DataSourceRequest { Page = 1, PageSize = 500 }, ct);
            return Ok(result.Data);
        }
    }
}