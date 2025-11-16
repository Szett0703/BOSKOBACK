# ?? API Documentation - Para el Equipo Frontend

## ?? Información de Conexión

### Base URL
```
https://localhost:5001
```

### CORS Habilitado para:
- `http://localhost:4200`
- `http://localhost:4300`
- `https://localhost:4200`
- `https://localhost:4300`

---

## ?? Endpoints Disponibles

### **Base Path**: `/api/productos`

---

### 1?? GET - Obtener todos los productos
```http
GET https://localhost:5001/api/productos
```

**Response 200 OK:**
```json
[
  {
    "id": 1,
    "nombre": "Laptop HP",
    "descripcion": "Laptop de alto rendimiento",
    "precio": 1500.50,
    "stock": 10,
    "categoria": "Electrónica",
    "fechaCreacion": "2024-01-15T10:30:00"
  },
  {
    "id": 2,
    "nombre": "Mouse Logitech",
    "descripcion": "Mouse inalámbrico",
    "precio": 25.99,
    "stock": 50,
    "categoria": "Accesorios",
    "fechaCreacion": "2024-01-16T11:00:00"
  }
]
```

---

### 2?? GET - Obtener un producto por ID
```http
GET https://localhost:5001/api/productos/{id}
```

**Ejemplo:**
```http
GET https://localhost:5001/api/productos/1
```

**Response 200 OK:**
```json
{
  "id": 1,
  "nombre": "Laptop HP",
  "descripcion": "Laptop de alto rendimiento",
  "precio": 1500.50,
  "stock": 10,
  "categoria": "Electrónica",
  "fechaCreacion": "2024-01-15T10:30:00"
}
```

**Response 404 Not Found:**
```json
{
  "message": "Producto con Id 999 no encontrado"
}
```

---

### 3?? POST - Crear un nuevo producto
```http
POST https://localhost:5001/api/productos
Content-Type: application/json
```

**Request Body:**
```json
{
  "nombre": "Teclado Mecánico",
  "descripcion": "Teclado RGB para gaming",
  "precio": 89.99,
  "stock": 30,
  "categoria": "Accesorios"
}
```

**Notas importantes:**
- ? **NO envíes** el campo `id` (se genera automáticamente)
- ? **NO envíes** el campo `fechaCreacion` (se asigna automáticamente en el servidor)
- ? **Campos requeridos**: `nombre`, `precio`, `stock`
- ? **Campos opcionales**: `descripcion`, `categoria`

**Response 201 Created:**
```json
{
  "id": 3,
  "nombre": "Teclado Mecánico",
  "descripcion": "Teclado RGB para gaming",
  "precio": 89.99,
  "stock": 30,
  "categoria": "Accesorios",
  "fechaCreacion": "2024-01-17T14:25:00"
}
```

---

### 4?? PUT - Actualizar un producto existente
```http
PUT https://localhost:5001/api/productos/{id}
Content-Type: application/json
```

**Ejemplo:**
```http
PUT https://localhost:5001/api/productos/3
```

**Request Body:**
```json
{
  "id": 3,
  "nombre": "Teclado Mecánico RGB",
  "descripcion": "Teclado RGB para gaming - Actualizado",
  "precio": 79.99,
  "stock": 25,
  "categoria": "Accesorios",
  "fechaCreacion": "2024-01-17T14:25:00"
}
```

**Notas importantes:**
- ? El `id` en el body **DEBE** coincidir con el `id` de la URL
- ? Debes enviar **TODOS** los campos del producto (incluyendo `fechaCreacion`)
- ? Si no quieres cambiar un campo, envía su valor actual

**Response 204 No Content** (éxito, sin body)

**Response 400 Bad Request:**
```json
{
  "message": "El Id de la URL no coincide con el Id del producto"
}
```

**Response 404 Not Found:**
```json
{
  "message": "Producto con Id 999 no encontrado"
}
```

---

### 5?? DELETE - Eliminar un producto
```http
DELETE https://localhost:5001/api/productos/{id}
```

**Ejemplo:**
```http
DELETE https://localhost:5001/api/productos/3
```

**Response 204 No Content** (éxito, sin body)

**Response 404 Not Found:**
```json
{
  "message": "Producto con Id 999 no encontrado"
}
```

---

## ?? Modelo TypeScript/Interface

```typescript
export interface Producto {
  id: number;                    // Generado automáticamente (NO enviar en POST)
  nombre: string;                // Requerido, máx 100 caracteres
  descripcion?: string | null;   // Opcional, máx 255 caracteres
  precio: number;                // Requerido, decimal(10,2)
  stock: number;                 // Requerido, entero
  categoria?: string | null;     // Opcional, máx 50 caracteres
  fechaCreacion: Date | string;  // Generado automáticamente (NO enviar en POST)
}

// Interface para CREAR productos (sin id ni fechaCreacion)
export interface ProductoCreate {
  nombre: string;
  descripcion?: string | null;
  precio: number;
  stock: number;
  categoria?: string | null;
}
```

---

## ??? Ejemplo de Servicio Angular

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface Producto {
  id: number;
  nombre: string;
  descripcion?: string | null;
  precio: number;
  stock: number;
  categoria?: string | null;
  fechaCreacion: Date | string;
}

export interface ProductoCreate {
  nombre: string;
  descripcion?: string | null;
  precio: number;
  stock: number;
  categoria?: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class ProductoService {
  private apiUrl = 'https://localhost:5001/api/productos';

  constructor(private http: HttpClient) { }

  // GET - Obtener todos los productos
  getProductos(): Observable<Producto[]> {
    return this.http.get<Producto[]>(this.apiUrl)
      .pipe(catchError(this.handleError));
  }

  // GET - Obtener un producto por ID
  getProducto(id: number): Observable<Producto> {
    return this.http.get<Producto>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  // POST - Crear un nuevo producto
  createProducto(producto: ProductoCreate): Observable<Producto> {
    return this.http.post<Producto>(this.apiUrl, producto)
      .pipe(catchError(this.handleError));
  }

  // PUT - Actualizar un producto existente
  updateProducto(id: number, producto: Producto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, producto)
      .pipe(catchError(this.handleError));
  }

  // DELETE - Eliminar un producto
  deleteProducto(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`)
      .pipe(catchError(this.handleError));
  }

  // Manejo de errores
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Ocurrió un error desconocido';
    
    if (error.error instanceof ErrorEvent) {
      // Error del lado del cliente
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Error del lado del servidor
      if (error.status === 404) {
        errorMessage = error.error?.message || 'Recurso no encontrado';
      } else if (error.status === 400) {
        errorMessage = error.error?.message || 'Solicitud inválida';
      } else if (error.status === 0) {
        errorMessage = 'No se pudo conectar al servidor. Verifica que el backend esté corriendo.';
      } else {
        errorMessage = `Error del servidor: ${error.status}`;
      }
    }
    
    console.error(errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
```

---

## ?? Ejemplo de Uso en Componente

```typescript
import { Component, OnInit } from '@angular/core';
import { ProductoService, Producto, ProductoCreate } from './services/producto.service';

@Component({
  selector: 'app-productos',
  templateUrl: './productos.component.html'
})
export class ProductosComponent implements OnInit {
  productos: Producto[] = [];
  loading = false;
  error: string | null = null;

  constructor(private productoService: ProductoService) { }

  ngOnInit(): void {
    this.loadProductos();
  }

  // Cargar todos los productos
  loadProductos(): void {
    this.loading = true;
    this.error = null;
    
    this.productoService.getProductos().subscribe({
      next: (data) => {
        this.productos = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = err.message;
        this.loading = false;
        console.error('Error al cargar productos:', err);
      }
    });
  }

  // Crear un nuevo producto
  crearProducto(): void {
    const nuevoProducto: ProductoCreate = {
      nombre: 'Producto de prueba',
      descripcion: 'Descripción del producto',
      precio: 99.99,
      stock: 20,
      categoria: 'Categoría de prueba'
    };

    this.productoService.createProducto(nuevoProducto).subscribe({
      next: (producto) => {
        console.log('Producto creado:', producto);
        this.loadProductos(); // Recargar la lista
      },
      error: (err) => {
        console.error('Error al crear producto:', err);
        this.error = err.message;
      }
    });
  }

  // Actualizar un producto
  actualizarProducto(producto: Producto): void {
    const productoActualizado: Producto = {
      ...producto,
      nombre: producto.nombre + ' (Actualizado)',
      precio: producto.precio + 10
    };

    this.productoService.updateProducto(producto.id, productoActualizado).subscribe({
      next: () => {
        console.log('Producto actualizado correctamente');
        this.loadProductos(); // Recargar la lista
      },
      error: (err) => {
        console.error('Error al actualizar producto:', err);
        this.error = err.message;
      }
    });
  }

  // Eliminar un producto
  eliminarProducto(id: number): void {
    if (confirm('¿Estás seguro de eliminar este producto?')) {
      this.productoService.deleteProducto(id).subscribe({
        next: () => {
          console.log('Producto eliminado correctamente');
          this.loadProductos(); // Recargar la lista
        },
        error: (err) => {
          console.error('Error al eliminar producto:', err);
          this.error = err.message;
        }
      });
    }
  }
}
```

---

## ?? Errores Comunes y Soluciones

### 1. `ERR_CONNECTION_REFUSED`
**Causa**: El backend no está corriendo o está en un puerto diferente.

**Solución**:
- Verifica que el backend esté corriendo en `https://localhost:5001`
- Abre Swagger para confirmar: `https://localhost:5001/swagger`

---

### 2. CORS Error
**Causa**: El puerto de tu aplicación Angular no está en la lista de CORS permitidos.

**Solución**:
- El backend ya está configurado para puertos `4200` y `4300`
- Si usas otro puerto, notifica al equipo backend

---

### 3. Error 404 en todos los endpoints
**Causa**: URL incorrecta.

**Solución**:
- Verifica que uses `/api/productos` (NO `/api/products` o `/api/categories`)
- URL completa: `https://localhost:5001/api/productos`

---

### 4. Error 400 en PUT
**Causa**: El `id` en la URL no coincide con el `id` en el body.

**Solución**:
```typescript
// ? Incorrecto
updateProducto(5, { id: 3, nombre: "...", ... })

// ? Correcto
updateProducto(5, { id: 5, nombre: "...", ... })
```

---

## ? Checklist de Integración

- [ ] Backend corriendo en `https://localhost:5001`
- [ ] Swagger accesible y funcionando
- [ ] Servicio Angular creado con la URL correcta
- [ ] Interface `Producto` definida en TypeScript
- [ ] HttpClient importado en el módulo
- [ ] Manejo de errores implementado
- [ ] Probado GET (listar productos)
- [ ] Probado GET (obtener un producto)
- [ ] Probado POST (crear producto)
- [ ] Probado PUT (actualizar producto)
- [ ] Probado DELETE (eliminar producto)

---

## ?? Contacto

Si encuentras algún problema con la API o necesitas endpoints adicionales, contacta al equipo de backend.

**Fecha de última actualización**: Noviembre 2024
