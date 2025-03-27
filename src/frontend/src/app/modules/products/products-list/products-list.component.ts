import { Component, OnInit } from '@angular/core';
import { ProductsService } from 'src/app/services/products.service';
import { ProductResponse } from 'src/app/models/product.model';
import { CategoriesResponse } from 'src/app/models/category.model';
import { PaginationQuery } from 'src/app/models/pagination-query.model';
import { CartsService } from 'src/app/services/carts.service';
import { CartItem, CartRequest, CartResponse, CartStatus } from 'src/app/models/cart.model';
import { AuthService } from 'src/app/services/auth.service';
import { ApiResponseWithData } from 'src/app/models/api-response-with-data.model';
import { PaginatedResponse } from 'src/app/models/paginated-response.model';

@Component({
  selector: 'app-products-list',
  templateUrl: './products-list.component.html',
  styleUrls: ['./products-list.component.scss']
})
export class ProductsListComponent implements OnInit {
  products: ProductResponse[] = [];
  categories: string[] = [];
  selectedCategory: string = 'Todos';
  searchQuery: string = '';
  isLoading = false;
  errorMessage: string | null = null;

  currentPage = 1;
  pageSize = 8;
  totalPages = 0;
  totalCount = 0;

  cart: CartResponse | null = null;
  userId: number | null = null;

  constructor(
    private productsService: ProductsService,
    private cartService: CartsService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.userId = this.authService.getUserId();

    if (!this.userId) {
      console.error('Usuário não autenticado ou token inválido.');
      return;
    }

    this.loadCategories();
    this.fetchProducts();
  }

  loadCategories(): void {
    this.productsService.getCategories().subscribe({
      next: (response: CategoriesResponse) => {
        this.categories = ['Todos', ...(response?.data || [])];
      },
      error: (err) => console.error('Erro ao carregar categorias:', err)
    });
  }

  fetchProducts(): void {
    this.isLoading = true;
    this.errorMessage = null;

    const filters: { [key: string]: string[] } = {};
    if (this.searchQuery) filters['title'] = [`*${this.searchQuery}*`];
    if (this.selectedCategory !== 'Todos') filters['category'] = [this.selectedCategory];

    const query: PaginationQuery = {
      _page: this.currentPage,
      _size: this.pageSize,
      filters
    };

    this.productsService.getProducts(query).subscribe({
      next: (res: PaginatedResponse<ProductResponse>) => {
        this.products = res.data.map(product => ({
          ...product,
          image: product.image || 'assets/default-product.jpg'
        }));
        this.totalPages = res.totalPages;
        this.totalCount = res.totalCount;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Erro ao carregar produtos.';
        console.error('Erro ao buscar produtos:', err);
        this.isLoading = false;
      }
    });
  }

  onCategoryChange(): void {
    this.currentPage = 1;
    this.fetchProducts();
  }

  onSearch(): void {
    this.currentPage = 1;
    this.fetchProducts();
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.fetchProducts();
  }

  addToCart(product: ProductResponse): void {
    const now = new Date().toISOString();

    const calculateDiscount = (quantity: number, unitPrice: number): number => {
      if (quantity > 20) return 0;
      if (quantity >= 10) return quantity * unitPrice * 0.2;
      if (quantity >= 4) return quantity * unitPrice * 0.1;
      return 0;
    };

    const createCartWithProduct = () => {
      const quantity = 1;
      const unitPrice = product.price;
      const discount = calculateDiscount(quantity, unitPrice);
      const total = quantity * unitPrice - discount;

      const newCart: CartRequest = {
        userId: this.userId!,
        date: now,
        products: [{
          productId: product.id,
          quantity,
          unitPrice,
          discount,
          total
        }]
      };

      this.cartService.createCart(newCart).subscribe({
        next: (res: ApiResponseWithData<CartResponse>) => {
          this.cart = res.data;
          console.log('✅ Carrinho criado, ID:', res.data.id);
          alert(`${product.title} foi adicionado ao carrinho!`);
        },
        error: (err) => {
          console.error('Erro ao criar carrinho:', err);
          alert('Erro ao criar carrinho e adicionar produto.');
        }
      });
    };

    const updateCart = (cart: CartResponse) => {
      const existingItem = cart.products?.find(p => p.productId === product.id);
      let updatedProducts: CartItem[] = [];

      if (existingItem) {
        if (existingItem.quantity >= 20) {
          alert('❌ Não é possível adicionar mais de 20 itens iguais ao carrinho.');
          return;
        }

        updatedProducts = cart.products!.map(p => {
          if (p.productId === product.id) {
            const newQuantity = p.quantity + 1;
            const discount = calculateDiscount(newQuantity, p.unitPrice);
            return {
              ...p,
              quantity: newQuantity,
              discount,
              total: newQuantity * p.unitPrice - discount
            };
          }
          return p;
        });
      } else {
        const quantity = 1;
        const unitPrice = product.price;
        const discount = calculateDiscount(quantity, unitPrice);
        const total = quantity * unitPrice - discount;

        updatedProducts = [
          ...(cart.products ?? []),
          {
            productId: product.id,
            quantity,
            unitPrice,
            discount,
            total
          }
        ];
      }

      const updateRequest: CartRequest = {
        userId: cart.userId,
        date: now,
        products: updatedProducts
      };

      this.cartService.updateCart(cart.id, updateRequest).subscribe({
        next: (res: ApiResponseWithData<CartResponse>) => {
          this.cart = res.data;
          console.log('📝 Carrinho atualizado, ID:', res.data.id);
          alert(`${product.title} foi adicionado ao carrinho!`);
        },
        error: (err) => {
          console.error('Erro ao atualizar carrinho:', err);
          alert('Erro ao adicionar produto ao carrinho.');
        }
      });
    };

    const fetchUserCart = () => {
      const query: PaginationQuery = {
        _page: 1,
        _size: 20,
        filters: { UserId: [this.userId!.toString()] }
      };

      this.cartService.getCarts(query).subscribe({
        next: (res) => {
          const carts = res.data ?? [];
          const activeCart = carts.find(c => c.status === CartStatus.Active);

          if (activeCart) {
            console.log('🔍 Carrinho ativo encontrado:', activeCart.id);
            updateCart(activeCart);
          } else {
            console.log('🆕 Nenhum carrinho ativo encontrado. Criando um novo...');
            createCartWithProduct();
          }
        },
        error: (err) => {
          console.error('Erro ao buscar carrinhos do usuário:', err);
          createCartWithProduct();
        }
      });
    };

    fetchUserCart();
  }
}
