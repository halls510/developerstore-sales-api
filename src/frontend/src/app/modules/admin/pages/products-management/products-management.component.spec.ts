import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductsManagementComponent } from './products-management.component';

describe('ProductsManagementComponent', () => {
  let component: ProductsManagementComponent;
  let fixture: ComponentFixture<ProductsManagementComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [ProductsManagementComponent]
    });
    fixture = TestBed.createComponent(ProductsManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
