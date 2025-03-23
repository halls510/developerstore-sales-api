export interface GeoLocationResponse {
  lat: string;
  long: string;
}

export interface AddressResponse {
  city: string;
  street: string;
  number: number;
  zipcode: string;
  geolocation: GeoLocationResponse;
}

export interface NameResponse {
  firstname: string;
  lastname: string;
}

// Enum de roles e status do usuário
export enum UserRole {
  Admin = 'Admin',
  Manager = 'Manager',
  Customer = 'Customer'
}

export enum UserStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Suspended = 'Suspended'
}


export interface Geolocation {
  lat: number;
  long: number;
}

export interface Address {
  street: string;
  number: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
  geolocation: Geolocation; // ✅ Ajustado para lat e long
}

export interface User {
  id: number;
  name: string;
  email: string;
  role: UserRole;
  status: UserStatus;
  address: Address;
  createdAt: Date;
  updatedAt: Date;
}


// Modelo principal da resposta de um usuário
export interface GetUserResponse {
  id: number;
  email: string;
  username: string;
  name: NameResponse;
  address: AddressResponse;
  phone: string;
  role: UserRole;
  status: UserStatus;
}

export interface CreateUserRequest {
  email: string;
  username: string;
  password: string;
  name: NameRequest;
  address: AddressRequest;
  phone: string;
  status: 'Active' | 'Inactive';
  role: 'Admin' | 'Manager' | 'Customer';
}

export interface NameRequest {
  firstname: string;
  lastname: string;
}

export interface AddressRequest {
  city: string;
  street: string;
  number: number;
  zipcode: string;
  geolocation: GeoLocationRequest;
}

export interface GeoLocationRequest {
  lat: string;
  long: string;
}


export interface CreateUserResponse {
  id: number;
  email: string;
  username: string;
  name: NameResponse;
  address: AddressResponse;
  phone: string;
  role: 'Admin' | 'Manager' | 'Customer';
  status: 'Active' | 'Inactive';
}

export interface NameResponse {
  firstname: string;
  lastname: string;
}

export interface AddressResponse {
  city: string;
  street: string;
  number: number;
  zipcode: string;
  geolocation: GeoLocationResponse;
}

export interface GeoLocationResponse {
  lat: string;
  long: string;
}

export interface UpdateUserRequest {
  email: string;
  username: string;
  password: string;
  name: NameRequest;
  address: AddressRequest;
  phone: string;
  status: UserStatus;
  role: UserRole;
}

export interface UpdateUserResponse {
  id: number;
  email: string;
  username: string;
  name: NameResponse;
  address: AddressResponse;
  phone: string;
  role: UserRole;
  status: UserStatus;
}