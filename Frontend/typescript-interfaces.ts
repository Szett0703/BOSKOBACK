// ============================================
// INTERFACES TYPESCRIPT PARA FRONTEND
// Bosko E-Commerce - Frontend Integration
// ============================================

/**
 * Producto completo con información de categoría
 * Usado en respuestas GET
 */
export interface Product {
  id: number;
  name: string;
  description: string | null;
  price: number;
  stock: number;
  image: string | null;
  categoryId: number | null;
  categoryName: string | null;
  createdAt: string; // ISO 8601 format
}

/**
 * Categoría
 * Usado en respuestas GET
 */
export interface Category {
  id: number;
  name: string;
  description: string;
  image: string | null;
}

/**
 * DTO para crear o actualizar producto
 * Usado en POST y PUT
 */
export interface ProductCreate {
  name: string;
  description?: string;
  price: number;
  stock: number;
  image?: string;
  categoryId?: number;
}

// ============================================
// CONFIGURACIÓN DE API
// ============================================

/**
 * URL base del API
 * Cambia según tu entorno
 */
export const API_CONFIG = {
  development: 'https://localhost:5006/api',
  production: 'https://api.bosko.com/api' // Ajustar en producción
};

export const API_BASE_URL = API_CONFIG.development;

// ============================================
// ENDPOINTS
// ============================================

export const ENDPOINTS = {
  // Products
  products: {
    getAll: `${API_BASE_URL}/products`,
    getById: (id: number) => `${API_BASE_URL}/products/${id}`,
    getByCategory: (categoryId: number) => `${API_BASE_URL}/products?categoryId=${categoryId}`,
    create: `${API_BASE_URL}/products`,
    update: (id: number) => `${API_BASE_URL}/products/${id}`,
    delete: (id: number) => `${API_BASE_URL}/products/${id}`
  },
  
  // Categories
  categories: {
    getAll: `${API_BASE_URL}/categories`,
    getById: (id: number) => `${API_BASE_URL}/categories/${id}`
  }
};

// ============================================
// SERVICIO DE EJEMPLO (ANGULAR)
// ============================================

/**
 * Ejemplo de servicio en Angular
 * Copia esto en tu archivo product.service.ts
 */

/*
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product, ProductCreate, Category } from './models/product.model';
import { ENDPOINTS } from './config/api.config';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  
  constructor(private http: HttpClient) {}

  // ========== PRODUCTS ==========
  
  getAllProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(ENDPOINTS.products.getAll);
  }

  getProductById(id: number): Observable<Product> {
    return this.http.get<Product>(ENDPOINTS.products.getById(id));
  }

  getProductsByCategory(categoryId: number): Observable<Product[]> {
    return this.http.get<Product[]>(ENDPOINTS.products.getByCategory(categoryId));
  }

  createProduct(product: ProductCreate): Observable<Product> {
    return this.http.post<Product>(ENDPOINTS.products.create, product);
  }

  updateProduct(id: number, product: ProductCreate): Observable<void> {
    return this.http.put<void>(ENDPOINTS.products.update(id), product);
  }

  deleteProduct(id: number): Observable<void> {
    return this.http.delete<void>(ENDPOINTS.products.delete(id));
  }

  // ========== CATEGORIES ==========

  getAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(ENDPOINTS.categories.getAll);
  }

  getCategoryById(id: number): Observable<Category> {
    return this.http.get<Category>(ENDPOINTS.categories.getById(id));
  }
}
*/

// ============================================
// EJEMPLOS DE USO EN COMPONENTES
// ============================================

/**
 * Ejemplo 1: Obtener todos los productos
 */
/*
export class ProductListComponent implements OnInit {
  products: Product[] = [];

  constructor(private productService: ProductService) {}

  ngOnInit() {
    this.productService.getAllProducts().subscribe({
      next: (data) => {
        this.products = data;
        console.log('Productos cargados:', this.products);
      },
      error: (error) => {
        console.error('Error al cargar productos:', error);
      }
    });
  }
}
*/

/**
 * Ejemplo 2: Filtrar productos por categoría
 */
/*
export class CategoryProductsComponent {
  categoryId = 1; // ID de la categoría
  products: Product[] = [];

  constructor(private productService: ProductService) {}

  loadProductsByCategory() {
    this.productService.getProductsByCategory(this.categoryId).subscribe({
      next: (data) => {
        this.products = data;
        console.log(`Productos de categoría ${this.categoryId}:`, this.products);
      },
      error: (error) => {
        console.error('Error:', error);
      }
    });
  }
}
*/

/**
 * Ejemplo 3: Crear un nuevo producto
 */
/*
export class ProductCreateComponent {
  newProduct: ProductCreate = {
    name: 'Camisa Nueva',
    description: 'Una camisa elegante',
    price: 49.99,
    stock: 10,
    image: 'https://example.com/image.jpg',
    categoryId: 1
  };

  constructor(private productService: ProductService) {}

  saveProduct() {
    this.productService.createProduct(this.newProduct).subscribe({
      next: (createdProduct) => {
        console.log('Producto creado:', createdProduct);
        // Redirigir o mostrar mensaje de éxito
      },
      error: (error) => {
        console.error('Error al crear producto:', error);
      }
    });
  }
}
*/

/**
 * Ejemplo 4: Obtener todas las categorías para un dropdown
 */
/*
export class ProductFormComponent implements OnInit {
  categories: Category[] = [];
  selectedCategoryId: number | null = null;

  constructor(private productService: ProductService) {}

  ngOnInit() {
    this.productService.getAllCategories().subscribe({
      next: (data) => {
        this.categories = data;
        console.log('Categorías disponibles:', this.categories);
      },
      error: (error) => {
        console.error('Error al cargar categorías:', error);
      }
    });
  }
}
*/

// ============================================
// RESPUESTAS DE EJEMPLO DEL API
// ============================================

/**
 * Ejemplo de respuesta: GET /api/products
 */
export const EXAMPLE_PRODUCTS_RESPONSE: Product[] = [
  {
    id: 1,
    name: "Camisa Oxford Azul",
    description: "Camisa clásica de algodón 100%, perfecta para el día a día",
    price: 49.99,
    stock: 25,
    image: "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=500",
    categoryId: 1,
    categoryName: "Camisas",
    createdAt: "2024-11-16T10:30:00Z"
  },
  {
    id: 2,
    name: "Jean Slim Fit Oscuro",
    description: "Pantalón jean ajustado de mezclilla premium",
    price: 79.99,
    stock: 40,
    image: "https://images.unsplash.com/photo-1542272604-787c3835535d?w=500",
    categoryId: 2,
    categoryName: "Pantalones",
    createdAt: "2024-11-16T10:31:00Z"
  }
];

/**
 * Ejemplo de respuesta: GET /api/categories
 */
export const EXAMPLE_CATEGORIES_RESPONSE: Category[] = [
  {
    id: 1,
    name: "Camisas",
    description: "Colección de camisas elegantes para hombre y mujer",
    image: "https://images.unsplash.com/photo-1596755094514-f87e34085b2c?w=800"
  },
  {
    id: 2,
    name: "Pantalones",
    description: "Pantalones casuales y formales de alta calidad",
    image: "https://images.unsplash.com/photo-1473966968600-fa801b869a1a?w=800"
  }
];

// ============================================
// UTILIDADES
// ============================================

/**
 * Formatea el precio con símbolo de moneda
 */
export function formatPrice(price: number): string {
  return new Intl.NumberFormat('es-ES', {
    style: 'currency',
    currency: 'EUR'
  }).format(price);
}

/**
 * Formatea la fecha de creación
 */
export function formatDate(dateString: string): string {
  return new Date(dateString).toLocaleDateString('es-ES', {
    year: 'numeric',
    month: 'long',
    day: 'numeric'
  });
}

/**
 * Verifica si un producto está disponible
 */
export function isProductAvailable(product: Product): boolean {
  return product.stock > 0;
}

/**
 * Obtiene la URL de la imagen o una por defecto
 */
export function getProductImage(product: Product): string {
  return product.image || 'assets/images/no-image.jpg';
}
