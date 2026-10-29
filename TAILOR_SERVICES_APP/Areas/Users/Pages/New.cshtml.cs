using DomainLayer;
using DomainLayer.Common;
using DomainLayer.Dtos.Users;
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

            public string? FullName { get; set; }

            public string? Email { get; set; }

            public string? Password { get; set; }

            public string? PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var getStatus = await _repository.GetByIdAsync(id, cancellationToken);
                    if (getStatus.Succeeded)
                        return Redirect("/users");

                    var result = getStatus.Value;
                    Input = new()
                    {
                        Id = id,
                        FullName = result.FullName,
                        Email = result.Email,
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
                return Redirect("/users");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    UserCreateDto dataObject = new()
                    {
                        FullName = Input.FullName!,
                        Email = Input.Email!,
                        PhoneNumber = Input.PhoneNumber!,
                        Password = Input.Password!
                    };
                    return new JsonResult(await _repository.CreateAsync(dataObject, cancellationToken));
                }
                else
                {
                    UserUpdateDto dataObject = new()
                    {
                        Id = id,
                        FullName = Input.FullName!,
                        Email = Input.Email!,
                        PhoneNumber = Input.PhoneNumber!,
                        Password = Input.Password,
                    };
                    return new JsonResult(await _repository.UpdateAsync(dataObject, cancellationToken));
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
                return new JsonResult(OperationResult.Fail(SystemMessage.ServerError));   
            }
        }
    }
}