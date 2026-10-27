using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Common
{
    public static class GridConfig
    {
        public const int PageSize = 20;
        public const int ButtonCount = 5;
    }

    public enum GridMessage
    {
        [Display(Name = "Row")] RowNumber,
        [Display(Name = "Edit")] Edit,
        [Display(Name = "{0} - {1} of {2} items")] DisplayMessage,
        [Display(Name = "No items to display")] EmptyMessage,
        [Display(Name = "Go to the first page")] FirstMessage,
        [Display(Name = "Go to the previous page")] PreviousMessage,
        [Display(Name = "Go to the next page")] NextMessage,
        [Display(Name = "Go to the last page")] LastMessage,
        [Display(Name = "items per page")] ItemsPerPageMessage,
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
        }
    }
}