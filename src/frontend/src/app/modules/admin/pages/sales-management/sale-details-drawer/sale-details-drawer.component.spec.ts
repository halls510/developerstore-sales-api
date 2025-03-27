import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleDetailsDrawerComponent } from './sale-details-drawer.component';

describe('SaleDetailsDrawerComponent', () => {
  let component: SaleDetailsDrawerComponent;
  let fixture: ComponentFixture<SaleDetailsDrawerComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [SaleDetailsDrawerComponent]
    });
    fixture = TestBed.createComponent(SaleDetailsDrawerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
