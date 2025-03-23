import { Component, OnInit } from '@angular/core';
import { CartResponse, CartStatus, CartItem } from 'src/app/models/cart.model';
import { Product as ProductModel } from 'src/app/models/product.model';
import { AuthService } from 'src/app/services/auth.service';
import { CartsService } from 'src/app/services/carts.service';
import { ProductsService } from 'src/app/services/products.service';

@Component({
  selector: 'app-carts',
  templateUrl: './carts.component.html',
  styleUrls: ['./carts.component.scss']
})
export class CartsComponent implements OnInit {
  activeCart: CartResponse | null = null;
  productsMap = new Map<number, ProductModel>();
  isLoading = false;
  error: string | null = null;

  constructor(
    private authService: AuthService,
    private cartsService: CartsService,
    private productsService: ProductsService
  ) {}

  ngOnInit(): void {
    const userId = this.authService.getUserId();
    if (!userId) {
      this.error = 'Usuário não autenticado.';
      return;
    }
    this.loadActiveCart(userId);
  }

  loadActiveCart(userId: number): void {
    this.isLoading = true;

    this.cartsService.getCartByUserId(userId).subscribe({
      next: (res) => {
        const carts = res.data;
        const active = carts.find(c => c.status === CartStatus.Active)
          || carts.reverse().find(c => !c.status || c.status === CartStatus.Active);

        this.activeCart = active || null;

        if (this.activeCart?.products?.length) {
          this.loadProductsFromCart(this.activeCart.products);
        } else {
          this.isLoading = false;
        }
      },
      error: (err) => {
        console.error('❌ Erro ao buscar carrinho:', err);
        this.error = 'Erro ao buscar carrinho.';
        this.isLoading = false;
      }
    });
  }

  loadProductsFromCart(cartItems: CartItem[]): void {
    const productIds = [...new Set(cartItems.map(item => item.productId))];
    let loadedCount = 0;
    this.productsMap.clear();
    this.isLoading = true;

    productIds.forEach(productId => {
      this.productsService.getProductById(productId).subscribe({
        next: (res) => {
          const product = res.data;
          this.productsMap.set(product.id, product);
          loadedCount++;

          if (loadedCount === productIds.length) {
            this.isLoading = false;
          }
        },
        error: (err) => {
          console.error(`❌ Erro ao buscar produto #${productId}:`, err);
          this.error = `Erro ao buscar produto com ID ${productId}`;
          this.isLoading = false;
        }
      });
    });
  }

  getProductName(productId: number): string {
    return this.productsMap.get(productId)?.title || `Produto #${productId}`;
  }

  onQuantityChange(item: CartItem): void {
    if (!this.activeCart?.products) return;

    if (item.quantity > 20) {
      alert('❌ Máximo permitido: 20 unidades.');
      item.quantity = 20;
    } else if (item.quantity < 1) {
      item.quantity = 1;
    }

    this.updateCart(this.activeCart.products);
  }

  removeItem(productId: number): void {
    if (!this.activeCart?.products) return;

    const remaining = this.activeCart.products.filter(p => p.productId !== productId);
    if (remaining.length === 0) {
      alert('O carrinho não pode ficar vazio.');
      return;
    }

    this.updateCart(remaining);
  }

  updateCart(updatedItems: CartItem[]): void {
    if (!this.activeCart) return;

    const updatedProducts: CartItem[] = updatedItems.map(p => {
      const product = this.productsMap.get(p.productId);
      const unitPrice = product?.price ?? p.unitPrice;
      return {
        productId: p.productId,
        quantity: p.quantity,
        unitPrice,
        discount: 0,
        total: unitPrice * p.quantity
      };
    });

    const updateRequest = {
      userId: this.activeCart.userId,
      date: new Date().toISOString(),
      products: updatedProducts
    };

    this.cartsService.updateCart(this.activeCart.id, updateRequest).subscribe({
      next: (res) => {
        this.activeCart = res.data;
        this.loadProductsFromCart(res.data.products);
      },
      error: (err) => {
        console.error('Erro ao atualizar carrinho:', err);
        alert('Erro ao atualizar carrinho.');
      }
    });
  }

  checkout(): void {
    if (!this.activeCart) return;

    this.cartsService.checkoutCart(this.activeCart.id).subscribe({
      next: () => {
        alert('✅ Checkout realizado com sucesso!');
        this.activeCart = null;
        this.productsMap.clear();
      },
      error: () => {
        alert('Erro ao realizar checkout.');
      }
    });
  }
}
