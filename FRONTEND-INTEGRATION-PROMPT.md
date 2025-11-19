# 🚀 PROMPT PARA FRONTEND - Integración de Endpoints Backend

**Fecha:** 19 de Noviembre 2025  
**Backend Status:** ✅ Todos los endpoints implementados  
**Tarea:** Integrar los nuevos endpoints en Angular

---

## 📋 ENDPOINTS DISPONIBLES

### 1️⃣ PERFIL DE USUARIO

#### ✅ GET /api/users/me - Obtener perfil
**Headers:** `Authorization: Bearer {token}`

**Response:**
```typescript
interface UserProfile {
  id: number;
  name: string;
  email: string;
  phone: string | null;
  role: string;
  provider: string;
  isActive: boolean;
  avatarUrl: string | null;
  createdAt: string;
  updatedAt: string;
  totalOrders: number;
  totalSpent: number;
  preferences: {
    notifications: boolean;
    newsletter: boolean;
    language: string | null;
  } | null;
}
```

---

#### ✅ PUT /api/users/me - Actualizar perfil
**Headers:** `Authorization: Bearer {token}`  
**Body:**
```typescript
interface UpdateProfile {
  name: string;
  email: string;
  phone?: string;
}
```

---

#### ✅ PUT /api/users/me/password - Cambiar contraseña
**Headers:** `Authorization: Bearer {token}`  
**Body:**
```typescript
interface ChangePassword {
  currentPassword: string;
  newPassword: string;
}
```

**Validaciones:**
- `currentPassword`: Requerido, debe coincidir con la actual
- `newPassword`: Requerido, mínimo 6 caracteres, diferente de la actual
- ⚠️ Usuarios de Google no pueden cambiar contraseña (error 403)

---

#### ✅ PUT /api/users/me/preferences - Actualizar preferencias
**Headers:** `Authorization: Bearer {token}`  
**Body:**
```typescript
interface UpdatePreferences {
  notifications: boolean;
  newsletter: boolean;
  language?: string; // "es" | "en"
}
```

---

#### ✅ POST /api/users/me/avatar - Subir avatar
**Headers:** `Authorization: Bearer {token}`  
**Content-Type:** `multipart/form-data`

**Body:**
```typescript
// FormData con key "avatar"
const formData = new FormData();
formData.append('avatar', file); // File object
```

**Validaciones:**
- Tipos permitidos: `image/jpeg`, `image/png`, `image/webp`
- Tamaño máximo: 5 MB

**Response:**
```typescript
{
  success: true,
  message: "Avatar actualizado correctamente",
  data: "https://localhost:5006/uploads/avatars/user-1-20251119-150530.jpg"
}
```

---

#### ✅ DELETE /api/users/me - Desactivar cuenta
**Headers:** `Authorization: Bearer {token}`

**Response:**
```typescript
{
  success: true,
  message: "Cuenta desactivada correctamente. Puedes reactivarla contactando soporte",
  data: true
}
```

---

### 2️⃣ DIRECCIONES

#### ✅ GET /api/addresses - Listar direcciones
**Headers:** `Authorization: Bearer {token}`

**Response:**
```typescript
interface Address {
  id: number;
  userId: number;
  label: string | null;
  street: string;
  city: string;
  state: string | null;
  postalCode: string;
  country: string;
  phone: string | null;
  isDefault: boolean;
  createdAt: string;
  updatedAt: string;
}

// Response format
{
  success: true,
  message: "Direcciones obtenidas exitosamente",
  data: Address[]
}
```

---

#### ✅ POST /api/addresses - Crear dirección
**Headers:** `Authorization: Bearer {token}`  
**Body:**
```typescript
interface CreateAddress {
  label: string; // Required
  street: string; // Required
  city: string; // Required
  state?: string;
  postalCode: string; // Required, 4-20 caracteres
  country: string; // Required
  phone?: string; // Formato de teléfono
  isDefault?: boolean; // Default: false
}
```

**Lógica automática:**
- ✅ Si es la primera dirección, se marca como predeterminada automáticamente
- ✅ Si `isDefault: true`, desmarca las demás

---

#### ✅ PUT /api/addresses/{id} - Actualizar dirección
**Headers:** `Authorization: Bearer {token}`  
**Body:** Mismo que `CreateAddress`

---

#### ✅ DELETE /api/addresses/{id} - Eliminar dirección
**Headers:** `Authorization: Bearer {token}`

**Restricción:**
- ❌ No se puede eliminar la dirección predeterminada si hay otras
- ✅ Si es la única dirección, sí se puede eliminar

**Error 400:**
```typescript
{
  success: false,
  message: "No se puede eliminar la dirección predeterminada. Establece otra dirección como predeterminada primero."
}
```

---

#### ✅ PUT /api/addresses/{id}/set-default - Establecer predeterminada
**Headers:** `Authorization: Bearer {token}`

**Response:** Devuelve la dirección actualizada con `isDefault: true`

---

### 3️⃣ ADMIN - PEDIDOS

#### ✅ PUT /api/admin/orders/{id} - Editar pedido
**Headers:** `Authorization: Bearer {admin-token}`  
**Roles permitidos:** `Admin`, `Employee`

**Body:**
```typescript
interface UpdateOrder {
  shippingAddress: {
    fullName: string;
    phone: string;
    street: string;
    city: string;
    state: string;
    postalCode: string;
    country: string;
  };
  notes?: string;
}
```

**Restricciones:**
- ⚠️ Solo pedidos en estado `"pending"` pueden editarse
- ✅ Solo se edita dirección y notas (NO items ni totales)

---

#### ✅ POST /api/admin/orders/{id}/cancel - Cancelar pedido
**Headers:** `Authorization: Bearer {admin-token}`  
**Roles permitidos:** `Admin`, `Employee`

**Body:**
```typescript
interface CancelOrder {
  reason: string; // Requerido, mínimo 10 caracteres
}
```

**Restricciones:**
- ⚠️ No se puede cancelar pedidos `"delivered"`
- ✅ Stock se restaura automáticamente
- ✅ Se registra en `OrderStatusHistory`

---

## 🛠️ TAREAS PENDIENTES EN ANGULAR

### 1. Descomentar código existente

#### **Archivo:** `src/app/pages/profile/profile.component.ts`
**Línea 105:** Descomentar
```typescript
this.loadAddresses(); // ← DESCOMENTAR ESTA LÍNEA
```

---

### 2. Implementar upload de avatar

#### **Archivo:** `src/app/pages/profile/profile.component.ts`

Agregar método:
```typescript
onAvatarSelected(event: any): void {
  const file: File = event.target.files[0];
  
  if (!file) return;

  // Validar tamaño (5 MB)
  if (file.size > 5 * 1024 * 1024) {
    this.toastr.error('El archivo es demasiado grande. Máximo 5 MB');
    return;
  }

  // Validar tipo
  const allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];
  if (!allowedTypes.includes(file.type)) {
    this.toastr.error('Tipo de archivo no permitido. Solo JPEG, PNG o WEBP');
    return;
  }

  this.uploadAvatar(file);
}

uploadAvatar(file: File): void {
  const formData = new FormData();
  formData.append('avatar', file);

  this.http.post<ApiResponse<string>>(`${environment.apiUrl}/users/me/avatar`, formData)
    .subscribe({
      next: (response) => {
        if (response.success) {
          this.userProfile.avatarUrl = response.data;
          this.toastr.success('Avatar actualizado correctamente');
        }
      },
      error: (error) => {
        this.toastr.error(error.error?.message || 'Error al subir avatar');
      }
    });
}
```

#### **Archivo:** `src/app/pages/profile/profile.component.html`

Agregar input de archivo:
```html
<div class="avatar-upload">
  <img [src]="userProfile.avatarUrl || 'assets/default-avatar.png'" 
       alt="Avatar" 
       class="avatar-preview">
  
  <input type="file" 
         #avatarInput 
         accept="image/jpeg,image/png,image/webp"
         (change)="onAvatarSelected($event)"
         style="display: none;">
  
  <button type="button" 
          class="btn btn-sm btn-primary" 
          (click)="avatarInput.click()">
    Cambiar foto
  </button>
</div>
```

---

### 3. Descomentar métodos de admin

#### **Archivo:** `src/app/admin/pages/orders/order-management.component.ts`

**Método `saveOrderChanges()` (línea ~280):**
```typescript
saveOrderChanges(): void {
  if (this.editForm.invalid) {
    this.toastr.error('Por favor completa todos los campos requeridos');
    return;
  }

  const updateData = {
    shippingAddress: this.editForm.value.shippingAddress,
    notes: this.editForm.value.notes
  };

  // DESCOMENTAR ESTAS LÍNEAS:
  this.orderService.updateOrder(this.selectedOrder.id, updateData).subscribe({
    next: (order) => {
      this.toastr.success('Pedido actualizado exitosamente');
      this.closeEditModal();
      this.loadOrders();
    },
    error: (error) => {
      this.toastr.error(error.error?.message || 'Error al actualizar el pedido');
    }
  });
}
```

**Método `confirmCancelOrder()` (línea ~320):**
```typescript
confirmCancelOrder(): void {
  if (!this.cancelReason || this.cancelReason.trim().length < 10) {
    this.toastr.error('La razón debe tener al menos 10 caracteres');
    return;
  }

  // DESCOMENTAR ESTAS LÍNEAS:
  this.orderService.cancelOrder(this.selectedOrder.id, this.cancelReason).subscribe({
    next: () => {
      this.toastr.success('Pedido cancelado exitosamente');
      this.closeCancelModal();
      this.loadOrders();
    },
    error: (error) => {
      this.toastr.error(error.error?.message || 'Error al cancelar el pedido');
    }
  });
}
```

---

### 4. Agregar métodos en servicios (si no existen)

#### **Archivo:** `src/app/services/order-admin.service.ts`

```typescript
updateOrder(id: number, data: any): Observable<Order> {
  return this.http.put<ApiResponse<Order>>(`${this.apiUrl}/${id}`, data).pipe(
    map(response => response.data)
  );
}

cancelOrder(id: number, reason: string): Observable<boolean> {
  return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/${id}/cancel`, { reason }).pipe(
    map(response => response.data)
  );
}
```

---

## ✅ CHECKLIST DE INTEGRACIÓN

### Perfil de Usuario:
- [ ] Descomentar `loadAddresses()` en `profile.component.ts` línea 105
- [ ] Implementar `onAvatarSelected()` y `uploadAvatar()`
- [ ] Agregar input file para avatar en HTML
- [ ] Probar actualización de perfil (nombre, email, teléfono)
- [ ] Probar cambio de contraseña
- [ ] Probar actualización de preferencias

### Direcciones:
- [ ] Verificar que `address.service.ts` usa `/api/addresses`
- [ ] Probar creación de dirección
- [ ] Probar edición de dirección
- [ ] Probar eliminación de dirección
- [ ] Probar establecer dirección predeterminada
- [ ] Verificar que la primera dirección se marca como predeterminada

### Admin - Pedidos:
- [ ] Descomentar `updateOrder()` en `order-management.component.ts`
- [ ] Descomentar `cancelOrder()` en `order-management.component.ts`
- [ ] Agregar métodos en `order-admin.service.ts` si faltan
- [ ] Probar edición de pedido (solo pending)
- [ ] Probar cancelación de pedido
- [ ] Verificar que stock se restaura después de cancelar

---

## 🔒 IMPORTANTE - SEGURIDAD

Todos los endpoints requieren:
- ✅ JWT Token válido en header `Authorization: Bearer {token}`
- ✅ Usuario autenticado y activo (`IsActive = true`)
- ✅ Endpoints de admin requieren rol `Admin` o `Employee`

**Manejo de errores:**
```typescript
error: (error: HttpErrorResponse) => {
  if (error.status === 401) {
    this.router.navigate(['/login']);
    this.toastr.error('Sesión expirada');
  } else if (error.status === 403) {
    this.toastr.error('No tienes permisos para esta acción');
  } else {
    this.toastr.error(error.error?.message || 'Error en el servidor');
  }
}
```

---

## 📊 TESTING

### Probar con Postman/Thunder Client:

1. **Login:**
```http
POST https://localhost:5006/api/auth/login
{
  "email": "test@example.com",
  "password": "Test123!"
}
```

2. **Copiar el token** del response

3. **Probar endpoints:**
```http
GET https://localhost:5006/api/users/me
Authorization: Bearer {token}

GET https://localhost:5006/api/addresses
Authorization: Bearer {token}
```

---

## 🚀 PRÓXIMOS PASOS

1. **Ejecutar scripts SQL pendientes** (ver `SQL_Scripts/EJECUTAR_TODOS_LOS_SCRIPTS_PENDIENTES.sql`)
2. **Reiniciar backend:** `dotnet run`
3. **Descomentar código en Angular**
4. **Implementar upload de avatar**
5. **Testing completo de funcionalidades**
6. **Aplicar estilos Bosko (gradientes azules)**

---

## 📞 ENDPOINTS RESUMEN

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/users/me` | Obtener perfil | ✅ JWT |
| PUT | `/api/users/me` | Actualizar perfil | ✅ JWT |
| PUT | `/api/users/me/password` | Cambiar contraseña | ✅ JWT |
| PUT | `/api/users/me/preferences` | Actualizar preferencias | ✅ JWT |
| POST | `/api/users/me/avatar` | Subir avatar | ✅ JWT |
| DELETE | `/api/users/me` | Desactivar cuenta | ✅ JWT |
| GET | `/api/addresses` | Listar direcciones | ✅ JWT |
| POST | `/api/addresses` | Crear dirección | ✅ JWT |
| PUT | `/api/addresses/{id}` | Actualizar dirección | ✅ JWT |
| DELETE | `/api/addresses/{id}` | Eliminar dirección | ✅ JWT |
| PUT | `/api/addresses/{id}/set-default` | Establecer predeterminada | ✅ JWT |
| PUT | `/api/admin/orders/{id}` | Editar pedido (admin) | ✅ JWT + Admin |
| POST | `/api/admin/orders/{id}/cancel` | Cancelar pedido (admin) | ✅ JWT + Admin |

---

**Fecha de creación:** 19 de Noviembre 2025  
**Backend Status:** ✅ 100% Implementado  
**Frontend Status:** ⏳ Pendiente integración  
**Prioridad:** 🔥 ALTA
