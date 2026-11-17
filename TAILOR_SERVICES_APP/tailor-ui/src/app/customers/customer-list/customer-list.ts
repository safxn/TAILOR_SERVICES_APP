import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TableModule, TableLazyLoadEvent } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { Customer } from '../customer';
import { CustomerGridDto, GridRequest } from '../customer.model';

@Component({
  selector: 'app-customer-list',
  imports: [RouterLink, TableModule, ButtonModule, InputTextModule, DatePipe],
  templateUrl: './customer-list.html'
})
export class CustomerList {
  private api = inject(Customer);

  customers = signal<CustomerGridDto[]>([]);
  total = signal(0);
  loading = signal(false);
  search = signal('');
  private lastEvent: TableLazyLoadEvent = { first: 0, rows: 10 };

  load(event: TableLazyLoadEvent) {
    this.lastEvent = event;
    const pageSize = event.rows ?? 10;
    const page = Math.floor((event.first ?? 0) / pageSize) + 1;

    const request: GridRequest = { page, pageSize };

    if (typeof event.sortField === 'string') {
      request.sorts = [{ field: event.sortField, dir: event.sortOrder === -1 ? 'desc' : 'asc' }];
    }

    const text = this.search().trim();
    if (text) {
      // Matches name OR phone OR address
      request.filter = {
        logic: 'or',
        filters: [
          { field: 'name', operator: 'contains', value: text },
          { field: 'phone', operator: 'contains', value: text },
          { field: 'address', operator: 'contains', value: text }
        ]
      };
    }

    this.loading.set(true);
    this.api.grid(request).subscribe({
      next: res => {
        this.customers.set(res.data);
        this.total.set(res.total);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  onSearch(value: string) {
    this.search.set(value);
    // Go back to page 1 and reload
    this.load({ ...this.lastEvent, first: 0 });
  }

  remove(id: string) {
    if (!confirm('Delete this customer?')) return;
    this.api.delete(id).subscribe({
      next: () => this.load(this.lastEvent),
      error: err => alert(err.error?.errors?.join('\n') ?? 'Delete failed')
    });
  }
}
