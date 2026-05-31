# 📚 Sistema Pubs - Windows Forms C#

## Estructura del Proyecto

```
PubsProject/
├── Data/
│   └── ConexionDB.cs          ← Conexión SQL Server + helpers
├── Forms/
│   ├── FrmLogin.cs            ← Login con 3 intentos
│   ├── FrmPrincipal.cs        ← Menú MDI principal
│   ├── FrmAuthors.cs          ← CRUD Autores
│   ├── FrmPublishers.cs       ← CRUD Editoriales
│   ├── FrmTitles.cs           ← CRUD Títulos/Libros
│   └── FormsRestantes.cs      ← CRUD: Tiendas, Puestos, Empleados,
│                                 Autor-Título, Ventas, Regalías,
│                                 Descuentos, Info Editorial
├── Program.cs                 ← Punto de entrada
└── PubsProject.csproj         ← Archivo de proyecto VS
```

---

## ⚙️ Pasos para ejecutar

### 1. Crear la base de datos en SQL Server
- Abre SQL Server Management Studio (SSMS)
- Ejecuta el archivo `instpubs.sql` que se te proporcionó
- Verifica que la base de datos `pubs` se creó correctamente

### 2. Configurar la cadena de conexión
Abre `Data/ConexionDB.cs` y ajusta según tu servidor:

```csharp
// Ejemplo para SQL Server Express local:
"Server=.\SQLEXPRESS;Database=pubs;Integrated Security=True;"

// Ejemplo con usuario y contraseña:
"Server=.\SQLEXPRESS;Database=pubs;User Id=sa;Password=tu_clave;"

// Ejemplo servidor con nombre específico:
"Server=MIPC\SQLSERVER2019;Database=pubs;Integrated Security=True;"
```

### 3. Abrir en Visual Studio
- Abre Visual Studio 2019 o superior
- Selecciona **Abrir > Proyecto/Solución**
- Selecciona el archivo `PubsProject.csproj`
- Presiona **F5** para compilar y ejecutar

---

## 🔐 Credenciales de Login

| Campo    | Valor     |
|----------|-----------|
| Usuario  | `admin`   |
| Clave    | `admin123`|

> El sistema permite **máximo 3 intentos**. Si se superan, la aplicación se cierra automáticamente.

---

## 📋 Tablas y formularios incluidos

| Tabla          | Formulario        | Operaciones         |
|----------------|-------------------|---------------------|
| `authors`      | FrmAuthors        | Crear, Leer, Actualizar, Eliminar |
| `publishers`   | FrmPublishers     | Crear, Leer, Actualizar, Eliminar |
| `titles`       | FrmTitles         | Crear, Leer, Actualizar, Eliminar |
| `stores`       | FrmStores         | Crear, Leer, Actualizar, Eliminar |
| `jobs`         | FrmJobs           | Crear, Leer, Actualizar, Eliminar |
| `employee`     | FrmEmployee       | Crear, Leer, Actualizar, Eliminar |
| `titleauthor`  | FrmTitleAuthor    | Crear, Leer, Actualizar, Eliminar |
| `sales`        | FrmSales          | Crear, Leer, Actualizar, Eliminar |
| `roysched`     | FrmRoySched       | Crear, Leer, Actualizar, Eliminar |
| `discounts`    | FrmDiscounts      | Crear, Leer, Actualizar, Eliminar |
| `pub_info`     | FrmPubInfo        | Crear, Leer, Actualizar, Eliminar |

---

## 💡 Notas importantes

- El campo `job_id` en la tabla `jobs` es **IDENTITY** (autoincremento), no se puede editar
- La tabla `pub_info` tiene campo `logo` de tipo `image`; este sistema solo gestiona `pr_info` (texto)
- Para la tabla `employee`, el `emp_id` debe seguir el patrón: `[A-Z][A-Z][A-Z][1-9][0-9][0-9][0-9][0-9][FM]`
  - Ejemplo válido: `ABC12345F`
- El `au_id` en authors debe seguir el patrón: `###-##-####`
  - Ejemplo válido: `123-45-6789`
