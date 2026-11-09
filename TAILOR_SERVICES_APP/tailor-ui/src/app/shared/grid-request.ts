import { TableLazyLoadEvent } from 'primeng/types/table';
export interface SortDescriptor { field: string; dir: 'asc' | 'desc'; }
export interface FilterDescriptor {
  logic?: 'and' | 'or'; field?: string; operator?: string; value?: unknown; filters?: FilterDescriptor[];
}
export interface DataSourceRequest {
  page: number; pageSize: number; sorts?: SortDescriptor[]; filter?: FilterDescriptor;
}

// PrimeNG matchMode -> the operators your BuildFilter understands
const OPERATORS: Record<string, string> = {
  contains: 'contains', notContains: 'doesnotcontain', startsWith: 'startswith', endsWith: 'endswith',
  equals: 'eq', is: 'eq', dateIs: 'eq',
  notEquals: 'neq', isNot: 'neq', dateIsNot: 'neq',
  lt: 'lt', before: 'lt', dateBefore: 'lt',
  lte: 'lte',
  gt: 'gt', after: 'gt', dateAfter: 'gt',
  gte: 'gte'
};

export function toDataSourceRequest(e: TableLazyLoadEvent, globalFields: string[] = []): DataSourceRequest {
  const pageSize = e.rows ?? 10;
  const page = Math.floor((e.first ?? 0) / pageSize) + 1;

  let sorts: SortDescriptor[] = [];
  if (e.multiSortMeta?.length) {
    sorts = e.multiSortMeta.map(s => ({ field: s.field, dir: s.order === -1 ? 'desc' : 'asc' }));
  } else if (typeof e.sortField === 'string' && e.sortField) {
    sorts = [{ field: e.sortField, dir: e.sortOrder === -1 ? 'desc' : 'asc' }];
  }

  const groups: FilterDescriptor[] = [];
  for (const [field, meta] of Object.entries(e.filters ?? {})) {
    const items = Array.isArray(meta) ? meta : meta ? [meta] : [];
    if (field === 'global') {
      const term = items[0]?.value?.toString().trim();
      if (term && globalFields.length)
        groups.push({ logic: 'or', filters: globalFields.map(f => ({ field: f, operator: 'contains', value: term })) });
      continue;
    }
    const parts = items
      .filter(i => i.value !== null && i.value !== undefined && i.value !== '' && OPERATORS[i.matchMode ?? ''])
      .map(i => ({ field, operator: OPERATORS[i.matchMode!], value: i.value }));
    if (parts.length)
      groups.push({ logic: items[0]?.operator === 'or' ? 'or' : 'and', filters: parts });
  }

  return {
    page, pageSize,
    sorts,
    filter: groups.length ? { logic: 'and', filters: groups } : undefined
  };
}
