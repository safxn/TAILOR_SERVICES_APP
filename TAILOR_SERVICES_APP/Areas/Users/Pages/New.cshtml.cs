using DomainLayer;
using DomainLayer.Common;
using DomainLayer.Dtos.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.ComponentModel.DataAnnotations;

namespace TAILOR_SERVICES_APP.Areas.Users.Pages
{
    public sealed class NewModel(ILogger<NewModel> logger,      // CHANGED: internal -> public
      IUserRepository repository) : PageModel
    {
        private readonly ILogger<NewModel> _logger = logger;
        private readonly IUserRepository _repository = repository;

        [BindProperty]                                          // CHANGED: added
        public InputModel Input { get; set; } = null!;

        public sealed class InputModel : InputVersionField      // CHANGED: internal -> public
        {
            public string? Id { get; set; }

            [Display(Name = "Name"), StringLength(4096)]
            public string? FullName { get; set; }

            [Display(Name = "Email")]
            public string? Email { get; set; }

            [Display(Name = "Password")]
            public string? Password { get; set; }

            [Display(Name = "Phone Number")]
            public string? PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    var getStatus = await _repository.GetByIdAsync(id, cancellationToken);
                    if (!getStatus.Succeeded)                   // CHANGED: was "if (getStatus.Succeeded)"
                        return Redirect("/users");

                    var result = getStatus.Value;
                    Input = new()
                    {
                        Id = id,
                        FullName = result.FullName,
                        Email = result.Email,
                        PhoneNumber = result.PhoneNumber,       // CHANGED: added so edit shows the phone
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
                        NewPassword = Input.Password,           // CHANGED: was "Password", the repository reads NewPassword
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