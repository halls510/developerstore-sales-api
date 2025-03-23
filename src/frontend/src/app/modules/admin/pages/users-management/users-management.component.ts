import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {
  CreateUserRequest,
  GetUserResponse,
  UpdateUserRequest,
  UserStatus // ✅ Enum importado
} from 'src/app/models/user.model';
import { UsersService } from 'src/app/services/users.service';
import { PaginationQuery } from 'src/app/models/pagination-query.model';
import { passwordValidator } from 'src/app/validators/password.validator';

@Component({
  selector: 'app-users-management',
  templateUrl: './users-management.component.html',
  styleUrls: ['./users-management.component.scss']
})
export class UsersManagementComponent implements OnInit {
  users: GetUserResponse[] = [];

  searchQuery = '';
  selectedStatus: string = 'Todos';

  currentPage = 1;
  pageSize = 10;
  totalPages = 0;
  totalCount = 0;
  orderBy?: string;

  showModal = false;
  isEditing = false;
  editingUserId: number | null = null;

  form!: FormGroup;
  apiErrors: { error: string; detail: string }[] = [];
  isLoading = false;
  errorMessage: string | null = null;

  // ✅ Disponibiliza enum no template
  userStatus = UserStatus;

  constructor(private usersService: UsersService, private fb: FormBuilder) {}

  ngOnInit(): void {
    this.loadUsers();
    this.initializeForm();
  }

  initializeForm(): void {
    this.form = this.fb.group({
      name: this.fb.group({
        firstname: ['', Validators.required],
        lastname: ['', Validators.required]
      }),
      email: ['', [Validators.required, Validators.email]],
      username: ['', Validators.required],
      password: ['', [Validators.required, passwordValidator]],
      phone: [''],
      status: [UserStatus.Active, Validators.required],
      role: ['Customer', Validators.required],
      address: this.fb.group({
        city: [''],
        street: [''],
        number: [0],
        zipcode: [''],
        geolocation: this.fb.group({
          lat: [''],
          long: ['']
        })
      })
    });
  }

  loadUsers(): void {
    this.isLoading = true;
    this.errorMessage = null;

    const filters: { [key: string]: string[] } = {};
    if (this.searchQuery){ 
      filters['username'] = [`*${this.searchQuery}*`];      
    }
    if (this.selectedStatus !== 'Todos') filters['status'] = [this.selectedStatus];

    const query: PaginationQuery = {
      _page: this.currentPage,
      _size: this.pageSize,
      _order: this.orderBy,
      filters
    };

    this.usersService.getUsers(query).subscribe({
      next: (res) => {
        console.log(res,query);
        this.users = res.data;
        this.totalPages = res.totalPages;
        this.totalCount = res.totalCount;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Erro ao carregar usuários.';
        console.error('[Erro] Buscar usuários:', err);
        this.isLoading = false;
      }
    });
  }

  changePage(page: number): void {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.loadUsers();
  }

  sort(by: string, direction: 'asc' | 'desc' = 'asc'): void {
    this.orderBy = `${by} ${direction}`;
    this.loadUsers();
  }

  onSearch(): void {
    this.currentPage = 1;
    this.loadUsers();
  }

  onStatusChange(): void {
    this.currentPage = 1;
    this.loadUsers();
  }

  openCreateModal(): void {
    this.initializeForm();
    this.isEditing = false;
    this.editingUserId = null;
    this.apiErrors = [];
    this.showModal = true;
  }

  editUser(user: GetUserResponse): void {
    this.form.patchValue({
      name: user.name,
      email: user.email,
      username: user.username,
      password: '',
      phone: user.phone,
      status: user.status,
      role: user.role,
      address: {
        ...user.address,
        geolocation: {
          lat: user.address.geolocation.lat,
          long: user.address.geolocation.long
        }
      }
    });

    this.isEditing = true;
    this.editingUserId = user.id;
    this.apiErrors = [];
    this.showModal = true;
  }

  closeModal(): void {
    this.showModal = false;
    this.apiErrors = [];
  }

  saveUser(): void {
    console.log('saveUser');

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const userData = this.form.value;
    this.apiErrors = [];
    console.log('userData',userData);
    const observer = {
      next: () => {
        this.loadUsers();
        this.closeModal();
        alert(`Usuário ${this.isEditing ? 'atualizado' : 'criado'} com sucesso!`);
      },
      error: (err: any) => {
        this.apiErrors = this.extractApiErrors(err);
      }
    };

    if (this.isEditing && this.editingUserId !== null) {
      const updatePayload: UpdateUserRequest = userData;
      this.usersService.updateUser(this.editingUserId, updatePayload).subscribe(observer);
    } else {
      const createPayload: CreateUserRequest = userData;
      console.log('userData 2:: ',userData);
      this.usersService.createUser(createPayload).subscribe(observer);
    }
  }

  deleteUser(userId: number): void {
    const confirmDelete = confirm('Tem certeza que deseja excluir este usuário?');
    if (!confirmDelete) return;

    this.usersService.deleteUser(userId).subscribe({
      next: () => {
        this.loadUsers();
        alert(`Usuário excluído com sucesso.`);
      },
      error: (error) => {
        this.errorMessage = 'Erro ao excluir o usuário.';
        console.error('[Erro] Excluir usuário:', error);
      }
    });
  }

  private extractApiErrors(error: any): { error: string; detail: string }[] {
    if (Array.isArray(error?.error?.errors)) return error.error.errors;
    if (error?.error?.message) return [{ error: 'Erro', detail: error.error.message }];
    return [{ error: 'Erro inesperado', detail: 'Erro inesperado ao salvar o usuário.' }];
  }
}
