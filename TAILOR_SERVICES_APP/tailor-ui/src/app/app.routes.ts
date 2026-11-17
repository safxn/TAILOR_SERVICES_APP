import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: 'customers', pathMatch: 'full' },
  {
    path: 'customers',
    loadChildren: () => import('./customers/customers.routes').then(m => m.CUSTOMER_ROUTES)
  },
  // later:
  // { path: 'orders', loadChildren: () => import('./orders/orders.routes').then(m => m.ORDER_ROUTES) },
  // { path: 'measurements', loadChildren: () => import('./measurements/measurements.routes').then(m => m.MEASUREMENT_ROUTES) },

  { path: '**', redirectTo: 'customers' }   // unknown URL fallback
];
