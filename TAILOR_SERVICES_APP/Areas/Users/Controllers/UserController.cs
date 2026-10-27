using DomainLayer;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TAILOR_SERVICES_APP.Areas.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(ILogger<UsersController> logger,
    IUserRepository repository) : ControllerBase
    {
        private readonly ILogger<UsersController> _logger = logger;
        private readonly IUserRepository _repository = repository;

        [HttpGet]
        public async Task<DataSourceResult> Get([DataSourceRequest] DataSourceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await _repository.GetGridAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
#if DEBUG
                if (ex.Message != "A task was canceled.")
                    System.Diagnostics.Debugger.Break();
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
                _logger.Error(ex);
                return new DataSourceResult();
            }
        }

        [HttpGet("list")]
        public async Task<IActionResult> List(CancellationToken ct)
        {
            var request = new DataSourceRequest { Page = 1, PageSize = 1000 };
            var result = await _repository.GetGridAsync(request, ct);
            return Ok(result.Data);
        }
    }
}
    
