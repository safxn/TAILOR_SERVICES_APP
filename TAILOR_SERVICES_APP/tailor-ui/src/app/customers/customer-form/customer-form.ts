import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { ButtonModule } from 'primeng/button';
import { MessageModule } from 'primeng/message';
import { Customer } from '../customer';

@Component({
  selector: 'app-customer-form',
  imports: [ReactiveFormsModule, RouterLink, InputTextModule, ButtonModule, MessageModule],
  templateUrl: './customer-form.html'
})
export class CustomerForm implements OnInit {
  private fb = inject(FormBuilder);
  private api = inject(Customer);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  id = this.route.snapshot.paramMap.get('id');
  errors = signal<string[]>([]);

  // Validators mirror your C# attributes: Name required/150, Phone 20, Address 250
  form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(150)]],
    phone: ['' as string | null, Validators.maxLength(20)],
    address: ['' as string | null, Validators.maxLength(250)],
    notes: ['' as string | null]
  });

  ngOnInit() {
    if (this.id) {
      this.api.get(this.id).subscribe(c => this.form.patchValue(c));
    }
  }

  save() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    const v = this.form.getRawValue();
    const dto = { name: v.name!, phone: v.phone, address: v.address, notes: v.notes };
    const request$ = this.id ? this.api.update(this.id, dto) : this.api.create(dto);

    request$.subscribe({
      next: () => this.router.navigate(['/customers']),
      error: err => this.errors.set(err.error?.errors ?? ['Something went wrong'])
    });
  }
}
