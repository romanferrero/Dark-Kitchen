# Dark Kitchen

Sistema de gestión para una cocina fantasma: catálogo de productos, pedidos con
ciclo de estados, promociones con descuentos, usuarios con roles y permisos,
reportes de administración y auditoría de cambios.

API REST en .NET 8 y aplicación web en Angular 21.

## Stack

| Capa | Tecnología |
|---|---|
| Backend | .NET 8, ASP.NET Core, Entity Framework Core 8 |
| Base de datos | SQL Server (Express) |
| Frontend | Angular 21 (standalone components, signals), Tailwind CSS 4 |
| Tests | MSTest, Moq, EF InMemory |
| Autenticación | JWT + BCrypt para el hash de contraseñas |

## Arquitectura

Clean Architecture: las interfaces viven en assemblies separados de sus
implementaciones, de modo que las dependencias solo pueden apuntar hacia
adentro.

```
src/
  DarkKitchen.Domain           Entidades, enums y el State pattern de pedidos
  DarkKitchen.IBusinessLogic   Interfaces de servicios + DTOs
  DarkKitchen.BusinessLogic    Servicios, validadores y estrategias de descuento
  DarkKitchen.IDataAccess      Interfaces de repositorios
  DarkKitchen.DataAccess       DbContext, repositorios y migraciones
  DarkKitchen.ServiceFactory   Composition root (registro de dependencias)
  DarkKitchen.WebApi           Controllers, filtros, mappers y modelos
  DarkKitchen.Importer         Contrato público para importadores de productos
  DarkKitchen.Importer.Json    Plugin de importación desde JSON
  DarkKitchen.Importer.Xml     Plugin de importación desde XML
Tests/                         Un proyecto de tests por capa
appweb/primeraApp/             Aplicación Angular
Datos/                         Scripts SQL, imágenes y colección de Postman
```

## Requisitos

- .NET SDK 8.0 (ver [global.json](global.json))
- SQL Server Express
- Node.js con npm 10+

## Puesta en marcha

### 1. Base de datos

La cadena de conexión por defecto apunta a `.\SQLEXPRESS` y a la base
`DarkKitchenDb`. Se configura en
[src/DarkKitchen.WebApi/appsettings.json](src/DarkKitchen.WebApi/appsettings.json).

Hay dos formas de crearla:

**Con migraciones** — crea el esquema y siembra usuarios y tipos de envío:

```bash
dotnet ef database update --project src/DarkKitchen.DataAccess --startup-project src/DarkKitchen.WebApi
```

**Con los scripts SQL** — además de lo anterior, carga productos, imágenes,
pedidos y promociones de ejemplo:

```
Datos/empty_schema.sql    crea las 9 tablas
Datos/seed_data.sql       carga los datos de ejemplo
```

### 2. API

```bash
dotnet run --project src/DarkKitchen.WebApi
```

Queda escuchando en `http://localhost:5128` y `https://localhost:7193`.
CORS está habilitado únicamente para `http://localhost:4200`.

### 3. Frontend

```bash
cd appweb/primeraApp
npm install
npm start
```

Disponible en `http://localhost:4200`. La URL de la API se configura en
[src/environments/environment.ts](appweb/primeraApp/src/environments/environment.ts).

## Usuarios de prueba

Vienen cargados por el seed. Son credenciales de desarrollo local.

| Email | Contraseña | Rol |
|---|---|---|
| `admin@darkkitchen.com` | `Admin@Passw0rd!!xx` | Admin |
| `roman.dispatcher@darkkitchen.com` | `Dispatch@Passw0rd!!x` | Dispatcher |
| `maia@gmail.com` | `Cliente@Passw0rd!!aa` | Client |

Los permisos de cada rol están definidos en
[RolePermissions.cs](src/DarkKitchen.WebApi/Filters/RolePermissions.cs).

## Tests

```bash
dotnet test DarkKitchen.sln
```

379 tests distribuidos en cuatro proyectos: BusinessLogic (213), WebApi (112),
DataAccess (48) y ServiceFactory (6). No requieren base de datos: usan el
proveedor en memoria de EF Core.

## Importación de productos

Los productos pueden importarse desde archivos externos mediante plugins que se
descubren por reflexión en tiempo de ejecución. La API explora una carpeta
`Plugins/` y expone los importadores que encuentra, sin necesidad de recompilar
la solución.

Vienen dos implementaciones, con archivos de ejemplo en
[Datos/productos.json](Datos/productos.json) y
[Datos/productos.xml](Datos/productos.xml).

Para escribir uno nuevo, ver
[docs/guia-extensibilidad-importadores.md](docs/guia-extensibilidad-importadores.md).

## API

La colección de Postman con el flujo completo de endpoints está en
[Datos/](Datos/).
