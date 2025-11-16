# ?? INICIO RÁPIDO - BOSKO E-COMMERCE API

## ? 3 PASOS PARA EMPEZAR

### 1?? CONFIGURAR BASE DE DATOS (2 minutos)

**Opción A: Con Script SQL (Recomendado)**
```sql
-- 1. Abre SQL Server Management Studio
-- 2. Conecta a: localhost
-- 3. Abre el archivo: Database/BoskoDB-Setup.sql
-- 4. Presiona F5 para ejecutar
-- 5. ? Listo! Base de datos creada con datos de prueba
```

**Opción B: Con Migraciones EF Core**
```bash
dotnet ef migrations add InitialBoskoSetup
dotnet ef database update
```

### 2?? EJECUTAR EL PROYECTO (30 segundos)

```bash
# Opción A: Visual Studio
# Presiona F5

# Opción B: Terminal
dotnet run
```

### 3?? VERIFICAR QUE FUNCIONA (1 minuto)

**Abre en tu navegador:**
```
https://localhost:5006/swagger
```

**Prueba estos endpoints:**
- `GET /api/products` ? Deberías ver 15 productos
- `GET /api/categories` ? Deberías ver 5 categorías
- `GET /api/products?categoryId=1` ? Productos de "Camisas"

---

## ?? PRUEBA DESDE TU FRONTEND

### Angular (TypeScript)

```typescript
// product.service.ts
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private apiUrl = 'https://localhost:5006/api';

  constructor(private http: HttpClient) {}

  getProducts(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/products`);
  }

  getCategories(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/categories`);
  }

  getProductsByCategory(categoryId: number): Observable<any[]> {
    return this.http.get<any[]>(
      `${this.apiUrl}/products?categoryId=${categoryId}`
    );
  }
}
```

```typescript
// product-list.component.ts
export class ProductListComponent implements OnInit {
  products: any[] = [];

  constructor(private productService: ProductService) {}

  ngOnInit() {
    this.productService.getProducts().subscribe({
      next: (data) => {
        this.products = data;
        console.log('? Productos cargados:', data);
      },
      error: (err) => console.error('? Error:', err)
    });
  }
}
```

---

## ?? RESPUESTAS QUE OBTENDRÁS

### GET /api/products
```json
[
  {
    "id": 1,
    "name": "Camisa Oxford Azul",
    "description": "Camisa clásica de algodón 100%",
    "price": 49.99,
    "stock": 25,
    "image": "https://images.unsplash.com/...",
    "categoryId": 1,
    "categoryName": "Camisas",
    "createdAt": "2024-11-16T10:30:00Z"
  }
]
```

### GET /api/categories
```json
[
  {
    "id": 1,
    "name": "Camisas",
    "description": "Colección de camisas elegantes",
    "image": "https://images.unsplash.com/..."
  }
]
```

---

## ??? TROUBLESHOOTING

### ? Error: "Cannot open database BoskoDB"
```bash
? Solución: Ejecuta Database/BoskoDB-Setup.sql en SSMS
```

### ? Error: "Port 5006 already in use"
```bash
? Solución: Cambia el puerto en appsettings.json y launchSettings.json
```

### ? Error: CORS en frontend
```bash
? Solución: Verifica que tu frontend esté en puerto 4200 o 4300
```

### ? Error: "SQL Server not found"
```bash
? Solución: 
   1. Verifica que SQL Server esté corriendo
   2. Actualiza el connection string en appsettings.json
```

---

## ?? ARCHIVOS IMPORTANTES

```
DBTest-BACK/
??? ?? Database/BoskoDB-Setup.sql     ? Script SQL completo
??? ?? Frontend/typescript-interfaces.ts ? Interfaces para Angular
??? ?? RESPONSE-TO-FRONTEND.md        ? Documentación detallada
??? ?? README.md                      ? Documentación completa
??? ?? PROJECT-SUMMARY.txt            ? Resumen visual
```

---

## ? TODO LISTO!

Si los 3 pasos funcionaron, estás listo para integrar con tu frontend.

**Endpoints disponibles:**
- ? `GET /api/products`
- ? `GET /api/products?categoryId=1`
- ? `GET /api/products/{id}`
- ? `POST /api/products`
- ? `PUT /api/products/{id}`
- ? `DELETE /api/products/{id}`
- ? `GET /api/categories`
- ? `GET /api/categories/{id}`

**Datos de prueba:**
- ? 5 categorías
- ? 15 productos

**Swagger:**
- ? https://localhost:5006/swagger

---

## ?? PRÓXIMOS PASOS

1. Copia las interfaces de `Frontend/typescript-interfaces.ts`
2. Actualiza tu servicio Angular con los endpoints
3. Comienza a consumir el API
4. ¡Desarrolla tu e-commerce! ??

---

**¿Necesitas ayuda?**  
Revisa: `RESPONSE-TO-FRONTEND.md` para documentación detallada
