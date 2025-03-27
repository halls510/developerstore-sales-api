import { Component, OnInit } from '@angular/core';
import { ProductsService } from 'src/app/services/products.service';
import { Product, ProductResponse } from 'src/app/models/product.model';
import { CategoriesResponse } from 'src/app/models/category.model';
import { PaginationQuery } from 'src/app/models/pagination-query.model';
import { HttpClient } from '@angular/common/http';
import { ApiResponseWithData, UploadResponse } from 'src/app/models/upload.model';

@Component({
  selector: 'app-products-management',
  templateUrl: './products-management.component.html',
  styleUrls: ['./products-management.component.scss']
})
export class ProductsManagementComponent implements OnInit {
  products: ProductResponse[] = [];
  categories: string[] = [];

  selectedProduct?: Product;
  selectedCategory: string = 'Todos';
  searchQuery: string = '';

  // UI States
  isLoading = false;
  showModal = false;
  isEditing = false;
  errorMessage: string | null = null;
  apiErrors: { error: string; detail: string }[] = [];
  isUploading = false;
  imagePreview: string | null = null;

  // Paginação
  currentPage = 1;
  pageSize = 10;
  totalPages = 0;
  totalCount = 0;
  orderBy?: string;

  // Produto novo/edição
  newProduct: Product = this.getEmptyProduct();
  selectedFile: File | null = null;
  isNewCategory = false;
  newCategory: string = '';

  constructor(private productsService: ProductsService, private http: HttpClient) {}

  ngOnInit(): void {
    this.loadCategories();
    this.fetchProducts();
  }

  fetchProducts(): void {
    this.isLoading = true;
    this.errorMessage = null;

    // const filters: any = {};
    const filters: { [key: string]: string[] } = {};
    if (this.searchQuery) filters['title'] = [`${this.searchQuery}*`];
    if (this.selectedCategory !== 'Todos') filters['category'] = [this.selectedCategory];

    const query: PaginationQuery = {
      _page: this.currentPage,
      _size: this.pageSize,
      _order: this.orderBy,
      filters
    };

    this.productsService.getProducts(query).subscribe({
      next: (res) => {    
        console.log(res,query);
        this.products = res.data;
        this.totalPages = res.totalPages;
        this.totalCount = res.totalCount;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Erro ao carregar os produtos.';
        console.error('[Erro] Buscar produtos:', err);
        this.isLoading = false;
      }
    });
  }

  loadCategories(): void {
    this.productsService.getCategories().subscribe({
      next: (response: CategoriesResponse) => {
        this.categories = ['Todos', ...(response?.data || [])];
      },
      error: (error) => console.error('Erro ao carregar categorias:', error)
    });
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.fetchProducts();
  }

  sort(by: string, direction: 'asc' | 'desc' = 'asc'): void {
    this.orderBy = `${by} ${direction}`;
    this.fetchProducts();
  }

  onSearch(): void {
    this.currentPage = 1;
    this.fetchProducts();
  }

  onCategoryChange(): void {
    this.currentPage = 1;
    this.fetchProducts();
  }

  openCreateModal(): void {
    this.newProduct = this.getEmptyProduct();
    this.isEditing = false;
    this.showModal = true;
    this.apiErrors = [];
    this.newCategory = '';
    this.isNewCategory = false;
    this.imagePreview = null;
  }

  editProduct(product: Product): void {
    this.newProduct = { ...product };
    this.imagePreview = product.image;
    this.isEditing = true;
    this.showModal = true;
    this.apiErrors = [];
  }

  closeModal(): void {
    this.showModal = false;
    this.apiErrors = [];
    this.imagePreview = null;
  }

  checkNewCategory(): void {
    this.isNewCategory = this.newProduct.category === 'other';
  }

  handleImageUpload(event: any): void {
    const file: File = event.target.files[0];
    if (!file) return;
  
    // 1. Mostrar preview com base64, mas NÃO salva no produto
    const reader = new FileReader();
    reader.onload = () => {
      this.imagePreview = reader.result as string;
    };
    reader.readAsDataURL(file);
  
    // 2. Upload da imagem
    const formData = new FormData();
    formData.append('file', file);
    this.isUploading = true;
  
    this.http.post<ApiResponseWithData<UploadResponse>>('/api/upload', formData).subscribe({
      next: (res) => {
        if (res.success && res.data?.url) {
          console.log('Upload realizado com sucesso:', res);
          this.newProduct.image = res.data.url;
        } else {
          console.warn('Upload falhou:', res);
          this.apiErrors.push({
            error: 'Erro no Upload',
            detail: res.message || 'O upload falhou sem mensagem específica.'
          });
        }
        this.isUploading = false;
      },
      error: (err) => {
        console.error('Erro no upload da imagem:', err);
        this.apiErrors.push({
          error: 'Erro HTTP',
          detail: 'Não foi possível enviar a imagem. Tente novamente.'
        });
        this.isUploading = false;
      }
    });
  }

  saveProduct(): void {
    if (!this.newProduct.image) {
      this.apiErrors = [{
        error: 'Imagem obrigatória',
        detail: 'É necessário enviar uma imagem antes de salvar o produto.'
      }];
      return;
    }

    if (this.isNewCategory && this.newCategory.trim()) {
      this.newProduct.category = this.newCategory;
      this.categories.push(this.newCategory);
    }

    const action = this.isEditing
      ? this.productsService.updateProduct(this.newProduct.id, this.newProduct)
      : this.productsService.createProduct(this.newProduct);

    action.subscribe({
      next: () => {
        this.fetchProducts();
        this.closeModal();
        alert(`Produto ${this.isEditing ? 'atualizado' : 'adicionado'} com sucesso!`);
      },
      error: (err) => {
        this.apiErrors = this.extractApiErrors(err);
      }
    });
  }

  deleteProduct(id: number): void {
    const confirmDelete = confirm('Tem certeza que deseja excluir este produto?');
    if (!confirmDelete) return;

    this.productsService.deleteProduct(id).subscribe({
      next: () => {
        this.fetchProducts();
        alert(`Produto ${id} excluído com sucesso.`);
      },
      error: (error) => {
        this.errorMessage = 'Erro ao excluir o produto.';
        console.error('[Erro] Excluir produto:', error);
      }
    });
  }

  private getEmptyProduct(): Product {
    return {
      id: 0,
      title: '',
      price: 0,
      category: '',
      image: '',
      description: '',
      rating: { rate: 0, count: 0 }
    };
  }

  private extractApiErrors(error: any): { error: string; detail: string }[] {
    if (Array.isArray(error?.error?.errors)) return error.error.errors;
    if (error?.error?.message) return [{ error: 'Erro', detail: error.error.message }];
    return [{ error: 'Erro inesperado', detail: 'Erro inesperado ao salvar o produto.' }];
  }
}
