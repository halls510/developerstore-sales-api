import { Component, Input, HostListener } from '@angular/core';
import { GetSaleByIdResponse } from 'src/app/models/sale.model';

@Component({
  selector: 'app-sale-details-drawer',
  templateUrl: './sale-details-drawer.component.html',
  styleUrls: ['./sale-details-drawer.component.scss']
})
export class SaleDetailsDrawerComponent {
  @Input() sale!: GetSaleByIdResponse;
  @Input() visible = false;
  @Input() onClose!: () => void;

  // 🔒 Fecha o drawer ao pressionar ESC
  @HostListener('document:keydown.escape', ['$event'])
  handleEscape(event: KeyboardEvent) {
    if (this.visible) {
      this.onClose();
    }
  }
}
