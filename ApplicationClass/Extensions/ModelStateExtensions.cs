using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DomainLayer.Common;

public static class ModelStateExtensions
{
    public static OperationResult ToOperationResult(this ModelStateDictionary modelState) =>
        OperationResult.Fail(modelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .Distinct()
            .ToArray());
}