using DomainLayer;
using DomainLayer.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TAILOR_SERVICES_APP.Areas.Users.Pages
{
    internal sealed class NewModel(ILogger<NewModel> logger,
     IUserRepository repository) : PageModel
    {
        private readonly ILogger<NewModel> _logger = logger;
        private readonly IUserRepository _repository = repository;

        public InputModel Input { get; set; } = null!;

        internal sealed class InputModel : InputVersionField
        {
            public string? Id { get; set; }

            public string? Name { get; set; }

            public string? Email { get; set; }

            public string? Password { get; set; }

            public string? ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    var getStatus = await _repository.GetAsync(Id, cancellationToken);
                    if (getStatus.HasErrors)
                        return Redirect("/crews");

                    var result = getStatus.Result;
                    Input = new()
                    {
                        Id = Id,
                        Name = result.Name,
                        Code = result.Code,
                        WorkUnitTypeName = result.WorkUnitTypeName,
                        WorkUnitId = result.WorkUnitId,
                        WorkUnitName = result.WorkUnitName,
                        Description = result.Description,
                        ConcurrencyToken = result.ConcurrencyToken
                    };
                }
                else
                {
                    Input = new();
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                if (ex.Message != "A task was canceled.")
                    System.Diagnostics.Debugger.Break();
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
                _logger.Error(ex);
                return Redirect("/crews");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            StatusGenericHandler status = new();
            try
            {
                if (!string.IsNullOrWhiteSpace(Id))
                {
                    ModelState.Remove($"Input.{nameof(Input.TypeId)}");
                    ModelState.Remove($"Input.{nameof(Input.WorkUnitId)}");
                }

                if (!ModelState.IsValid)
                {
                    status.AddError(SystemMessage.InValidModel);
                    return new JsonResult(status);
                }

                if (string.IsNullOrWhiteSpace(Id))
                {
                    CrewInsertDto dataObject = new()
                    {
                        Name = Input.Name,
                        Code = Input.Code,
                        Description = Input.Description,
                        WorkUnitId = Input.WorkUnitId,
                        SelectedUserIds = Input.SelectedUserIds
                    };
                    status = (StatusGenericHandler)await _repository.InsertAsync(dataObject, cancellationToken);
                }
                else
                {
                    CrewUpdateDto dataObject = new()
                    {
                        Id = Id,
                        Name = Input.Name,
                        Code = Input.Code,
                        Description = Input.Description,
                        SelectedUserIds = Input.SelectedUserIds,
                        DeletedUserIds = Input.DeletedUserIds,
                        ConcurrencyToken = Input.ConcurrencyToken
                    };
                    status = (StatusGenericHandler)await _repository.UpdateAsync(dataObject, cancellationToken);
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                if (ex.Message != "A task was canceled.")
                    System.Diagnostics.Debugger.Break();
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
                _logger.Error(ex);
                status.AddError(SystemMessage.ServerError);
            }
            return new JsonResult(status);
        }

        public async Task<IActionResult> OnGetUserSelectAsync(CancellationToken cancellationToken)
        {
            try
            {
                return new PartialViewResult
                {
                    ViewName = "/Areas/Settings/Pages/Crews/_UserSelect.cshtml",
                    ViewData = new ViewDataDictionary(ViewData)
                };
            }
            catch (Exception ex)
            {
#if DEBUG
                if (ex.Message != "A task was canceled.")
                    System.Diagnostics.Debugger.Break();
                System.Diagnostics.Debug.WriteLine(ex.Message);
#endif
                _logger.Error(ex);
                return BadRequest(SystemMessage.ServerError);
            }
        }

    }
}