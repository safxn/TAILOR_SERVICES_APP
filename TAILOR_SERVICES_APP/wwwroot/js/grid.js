window.appGrid = function (selector, columns, extra) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? "";
    const opMap = {
        "=": "eq", "!=": "neq", "<": "lt", "<=": "lte", ">": "gt", ">=": "gte",
        "like": "contains", "starts": "startswith", "ends": "endswith"
    };

    return new Tabulator(selector, Object.assign({
        layout: "fitColumns",
        pagination: true,
        paginationMode: "remote",
        paginationSize: 10,
        paginationSizeSelector: [10, 25, 50],
        sortMode: "remote",
        filterMode: "remote",
        ajaxURL: location.pathname + "?handler=Grid",
        ajaxRequestFunc: async function (url, config, params) {
            const size = params.size || 10;
            const filters = params.filter || [];
            const res = await fetch(url, {
                method: "POST",
                headers: { "Content-Type": "application/json", "RequestVerificationToken": token },
                body: JSON.stringify({
                    page: params.page || 1,
                    pageSize: size,
                    sorts: (params.sort || []).map(s => ({ field: s.field, dir: s.dir })),
                    filter: filters.length ? {
                        logic: "and",
                        filters: filters.map(f => ({
                            field: f.field, operator: opMap[f.type] || "contains", value: f.value
                        }))
                    } : null
                })
            });
            if (!res.ok) throw new Error("Server returned " + res.status);
            const json = await res.json();
            return { last_page: Math.max(1, Math.ceil(json.total / size)), data: json.data };
        },
        columns: columns
    }, extra || {}));
};