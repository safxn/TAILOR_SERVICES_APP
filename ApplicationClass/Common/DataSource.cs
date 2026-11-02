using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace DomainLayer.Common;

public sealed class DataSourceRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<SortDescriptor>? Sorts { get; set; }
    public FilterDescriptor? Filter { get; set; }
}

public sealed class SortDescriptor
{
    public string Field { get; set; } = "";
    public string Dir { get; set; } = "asc";
}

public sealed class FilterDescriptor
{
    public string? Logic { get; set; }          // "and" / "or"
    public string? Field { get; set; }
    public string? Operator { get; set; }       // eq, neq, lt, lte, gt, gte, contains, ...
    public object? Value { get; set; }
    public List<FilterDescriptor>? Filters { get; set; }
}

public sealed class DataSourceResult
{
    public IEnumerable Data { get; set; } = Array.Empty<object>();
    public int Total { get; set; }
}

public static class DataSourceExtensions
{
    public static async Task<DataSourceResult> ToDataSourceResultAsync<T>(
        this IQueryable<T> query, DataSourceRequest? request, CancellationToken ct = default)
    {
        request ??= new DataSourceRequest();

        // 1. Filter
        if (request.Filter is not null)
        {
            var args = new List<object>();
            var expr = BuildFilter<T>(request.Filter, args);
            if (expr.Length > 0)
                query = query.Where(expr, args.ToArray());
        }

        var total = await query.CountAsync(ct);

        // 2. Sort (field names are checked against T's real properties)
        var orderParts = new List<string>();
        foreach (var s in request.Sorts ?? [])
        {
            var prop = FindProperty<T>(s.Field);
            if (prop is null) continue;
            var dir = string.Equals(s.Dir, "desc", StringComparison.OrdinalIgnoreCase) ? "desc" : "asc";
            orderParts.Add($"{prop.Name} {dir}");
        }
        if (orderParts.Count == 0)
        {
            var fallback = FindProperty<T>("Id") ?? typeof(T).GetProperties().FirstOrDefault();
            if (fallback is not null) orderParts.Add(fallback.Name);
        }
        if (orderParts.Count > 0)
            query = query.OrderBy(string.Join(", ", orderParts));

        // 3. Page
        var pageSize = Math.Clamp(request.PageSize, 1, 500);
        var page = Math.Max(request.Page, 1);
        var data = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return new DataSourceResult { Data = data, Total = total };
    }

    private static PropertyInfo? FindProperty<T>(string? name) =>
        string.IsNullOrWhiteSpace(name)
            ? null
            : typeof(T).GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

    private static string BuildFilter<T>(FilterDescriptor f, List<object> args)
    {
        // Group of filters
        if (f.Filters is { Count: > 0 })
        {
            var parts = f.Filters.Select(x => BuildFilter<T>(x, args)).Where(s => s.Length > 0).ToList();
            if (parts.Count == 0) return "";
            var logic = string.Equals(f.Logic, "or", StringComparison.OrdinalIgnoreCase) ? " or " : " and ";
            return "(" + string.Join(logic, parts) + ")";
        }

        // Single filter
        var prop = FindProperty<T>(f.Field);
        if (prop is null || string.IsNullOrWhiteSpace(f.Operator)) return "";

        var name = prop.Name;
        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
        var isNullable = !prop.PropertyType.IsValueType || Nullable.GetUnderlyingType(prop.PropertyType) is not null;
        var op = f.Operator.ToLowerInvariant();

        if (op == "isnull") return isNullable ? $"{name} == null" : "";
        if (op == "isnotnull") return isNullable ? $"{name} != null" : "";

        if (!TryConvert(AsString(f.Value), type, out var value)) return "";
        args.Add(value!);
        var p = $"@{args.Count - 1}";
        var isString = type == typeof(string);

        return op switch
        {
            "eq" => $"{name} == {p}",
            "neq" => $"{name} != {p}",
            "lt" => $"{name} < {p}",
            "lte" => $"{name} <= {p}",
            "gt" => $"{name} > {p}",
            "gte" => $"{name} >= {p}",
            "contains" when isString => $"{name}.Contains({p})",
            "doesnotcontain" when isString => $"!{name}.Contains({p})",
            "startswith" when isString => $"{name}.StartsWith({p})",
            "endswith" when isString => $"{name}.EndsWith({p})",
            _ => Drop(args)
        };

        static string Drop(List<object> a) { a.RemoveAt(a.Count - 1); return ""; }
    }

    private static string? AsString(object? v) => v switch
    {
        null => null,
        JsonElement { ValueKind: JsonValueKind.Null or JsonValueKind.Undefined } => null,
        JsonElement { ValueKind: JsonValueKind.String } je => je.GetString(),
        JsonElement je => je.GetRawText(),
        IFormattable f => f.ToString(null, CultureInfo.InvariantCulture),
        _ => v.ToString()
    };

    private static bool TryConvert(string? s, Type type, out object? result)
    {
        result = null;
        if (s is null) return false;
        try
        {
            if (type == typeof(string)) result = s;
            else if (type == typeof(DateTime))
                result = DateTime.Parse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            else if (type == typeof(DateTimeOffset))
                result = DateTimeOffset.Parse(s, CultureInfo.InvariantCulture);
            else
                result = TypeDescriptor.GetConverter(type).ConvertFromInvariantString(s);
            return result is not null;
        }
        catch { return false; }
    }
}