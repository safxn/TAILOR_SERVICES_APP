import { inject, Service } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CustomerDetailDto, CustomerGridDto, CustomerSaveDto, GridRequest, GridResult } from './customer.model';

@Service()
export class Customer {
  private http = inject(HttpClient);
  private baseUrl = '/api/customers';

  grid(request: GridRequest) {
    return this.http.post<GridResult<CustomerGridDto>>(`${this.baseUrl}/grid`, request);
  }

  get(id: string) {
    return this.http.get<CustomerDetailDto>(`${this.baseUrl}/${id}`);
  }

  create(dto: CustomerSaveDto) {
    return this.http.post<void>(this.baseUrl, dto);
  }

  update(id: string, dto: CustomerSaveDto) {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
