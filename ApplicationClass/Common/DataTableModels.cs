namespace DomainLayer.Common;

public sealed class DataTablesRequest
{
    public int Draw { get; set; }
    public int Start { get; set; }
    public int Length { get; set; } = 10;
    public DataTablesSearch? Search { get; set; }
    public List<DataTablesColumn> Columns { get; set; } = [];
    public List<DataTablesOrder> Order { get; set; } = [];

    /// <summary>Converts the DataTables request into the request the repository understands.</summary>
    public DataSourceRequest ToDataSourceRequest()
    {
        var size = Length <= 0 ? 500 : Math.Min(Length, 500);   // -1 means "All" in DataTables
        var page = Length <= 0 ? 1 : (Start / size) + 1;

        // Sorting: only columns marked orderable, using the column's "data" name
        var sorts = Order
            .Where(o => o.Column >= 0 && o.Column < Columns.Count)
            .Select(o => (Col: Columns[o.Column], Order: o))
            .Where(x => x.Col.Orderable && !string.IsNullOrWhiteSpace(x.Col.Data))
            .Select(x => new SortDescriptor { Field = x.Col.Data!, Dir = x.Order.Dir })
            .ToList();

        var groups = new List<FilterDescriptor>();

        // Global search box: OR across all searchable columns
        var term = Search?.Value?.Trim();
        if (!string.IsNullOrEmpty(term))
        {
            var any = Columns
                .Where(c => c.Searchable && !string.IsNullOrWhiteSpace(c.Data))
                .Select(c => new FilterDescriptor { Field = c.Data, Operator = "contains", Value = term })
                .ToList();
            if (any.Count > 0) groups.Add(new FilterDescriptor { Logic = "or", Filters = any });
        }

        // Per-column search boxes: AND
        foreach (var c in Columns.Where(c => c.Searchable
                                          && !string.IsNullOrWhiteSpace(c.Data)
                                          && !string.IsNullOrWhiteSpace(c.Search?.Value)))
        {
            groups.Add(new FilterDescriptor { Field = c.Data, Operator = "contains", Value = c.Search!.Value });
        }

        return new DataSourceRequest
        {
            Page = page,
            PageSize = size,
            Sorts = sorts,
            Filter = groups.Count == 0 ? null : new FilterDescriptor { Logic = "and", Filters = groups }
        };
    }
}

public sealed class DataTablesColumn
{
    public string? Data { get; set; }
    public bool Searchable { get; set; } = true;
    public bool Orderable { get; set; } = true;
    public DataTablesSearch? Search { get; set; }
}

public sealed class DataTablesOrder
{
    public int Column { get; set; }
    public string Dir { get; set; } = "asc";
}

public sealed class DataTablesSearch
{
    public string? Value { get; set; }
}

public sealed class DataTablesResponse
{
    public int Draw { get; set; }
    public int RecordsTotal { get; set; }
    public int RecordsFiltered { get; set; }
    public System.Collections.IEnumerable Data { get; set; } = Array.Empty<object>();
    public string? Error { get; set; }
}