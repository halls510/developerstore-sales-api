import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MySalesComponent } from './my-sales.component';

describe('MySalesComponent', () => {
  let component: MySalesComponent;
  let fixture: ComponentFixture<MySalesComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [MySalesComponent]
    });
    fixture = TestBed.createComponent(MySalesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
