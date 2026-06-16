# Guía de extensibilidad — Crear un nuevo importador de productos

## Descripción general

DarkKitchen permite importar productos desde fuentes externas (archivos, APIs, bases de datos, etc.) mediante un sistema de **plugins por reflection**. El sistema descubre importadores automáticamente en tiempo de ejecución sin necesidad de recompilar la aplicación principal.

Un desarrollador tercero puede crear un nuevo importador siguiendo únicamente esta guía, referenciando un solo assembly de contrato (`DarkKitchen.Importer.dll`).

## Arquitectura del mecanismo

```
┌─────────────────────────────┐
│     DarkKitchen.Importer    │  ← Assembly de contrato (única dependencia)
│  ┌────────────────────────┐ │
│  │   IProductImporter     │ │  ← Interfaz a implementar
│  │   ImportedProduct      │ │  ← DTO de producto
│  │   ImportedProductImage │ │  ← DTO de imagen
│  │   ImporterParameter    │ │  ← Declaración de parámetros
│  └────────────────────────┘ │
└─────────────────────────────┘
              ▲
              │ referencia
┌─────────────────────────────┐
│  Tu plugin (ej. CSV, API)   │  ← Class library .NET 8
│  MiImportador.dll           │
└─────────────────────────────┘
              │ se copia a
              ▼
┌─────────────────────────────┐
│  WebApi/bin/Plugins/         │  ← Carpeta escaneada en runtime
└─────────────────────────────┘
```

En runtime, `ReflectionImporterLoader` escanea la carpeta `Plugins/`, carga cada `.dll`, busca clases que implementen `IProductImporter`, las instancia con `Activator.CreateInstance` y las expone al sistema.

El escaneo ocurre **en cada llamada** a `GET /api/products/importers` (no se cachea), y cada `.dll` se carga **en memoria** (`Assembly.Load(byte[])`) en lugar de mantener el archivo abierto. Gracias a esto, los importadores se pueden **agregar y eliminar en tiempo de ejecución** sin reiniciar la aplicación y sin que el archivo quede bloqueado por el proceso.

## Interfaz `IProductImporter`

```csharp
namespace DarkKitchen.Importer;

public interface IProductImporter
{
    // Nombre único del importador (se muestra en el menú dinámico de la UI)
    string Name { get; }

    // Descripción del importador
    string Description { get; }

    // Parámetros que necesita el importador (la UI arma un formulario dinámico con estos)
    IReadOnlyCollection<ImporterParameter> Parameters { get; }

    // Ejecuta la importación y devuelve los productos leídos
    IReadOnlyCollection<ImportedProduct> Import(IReadOnlyDictionary<string, string> arguments);
}
```

## Tipos de soporte

### `ImporterParameter`
Describe un parámetro que el importador necesita. La UI genera un campo de formulario por cada uno.

```csharp
public record ImporterParameter(
    string Name,        // Identificador del parámetro (clave en el diccionario de arguments)
    string Label,       // Etiqueta visible en la UI
    string Description, // Texto de ayuda
    bool Required       // Si es obligatorio
);
```

### `ImportedProduct`
DTO que representa un producto importado. Cada campo se mapea a las propiedades del dominio `Product`.

```csharp
public record ImportedProduct(
    string Name,                                    // 10-50 caracteres
    decimal Price,
    string Description,                             // 20-500 caracteres
    string Line,                                    // No vacío
    string Category,                                // No vacío
    IReadOnlyCollection<ImportedProductImage> Images, // 1-3 imágenes, URLs deben terminar en .jpg
    bool Active
);
```

### `ImportedProductImage`
```csharp
public record ImportedProductImage(
    string Path,       // URL de la imagen (debe terminar en .jpg)
    decimal SizeInKb   // Tamaño en KB
);
```

## Validaciones del dominio

El sistema valida cada producto importado contra las reglas del dominio `Product`. Si un producto no cumple, se registra el error y la importación continúa con el resto. Las validaciones son:

- **Name**: entre 10 y 50 caracteres
- **Description**: entre 20 y 500 caracteres
- **Line**: no puede estar vacío
- **Category**: no puede estar vacío
- **Images**: entre 1 y 3 imágenes, todas con URL terminada en `.jpg`
- **Code**: se genera automáticamente (formato `PROD-XXXXXXXX`), no lo provee el importador

## Paso a paso: crear un nuevo importador

### 1. Crear un proyecto Class Library .NET 8

```bash
dotnet new classlib -n DarkKitchen.Importer.MiFormato -f net8.0
```

### 2. Agregar referencia al contrato

```bash
dotnet add reference ../DarkKitchen.Importer/DarkKitchen.Importer.csproj
```

O si se trabaja fuera de la solución, referenciar directamente el DLL:

```xml
<ItemGroup>
  <Reference Include="DarkKitchen.Importer">
    <HintPath>ruta/a/DarkKitchen.Importer.dll</HintPath>
  </Reference>
</ItemGroup>
```

**Importante:** no referenciar ningún otro proyecto de la solución.

### 3. Implementar `IProductImporter`

Ejemplo — importador desde CSV:

```csharp
using DarkKitchen.Importer;

namespace DarkKitchen.Importer.Csv;

public class CsvProductImporter : IProductImporter
{
    public string Name => "CSV";

    public string Description => "Importa productos desde un archivo CSV";

    public IReadOnlyCollection<ImporterParameter> Parameters =>
    [
        new ImporterParameter("filePath", "Ruta del archivo", "Ruta al archivo CSV", true),
        new ImporterParameter("delimiter", "Delimitador", "Carácter delimitador (default: ,)", false)
    ];

    public IReadOnlyCollection<ImportedProduct> Import(IReadOnlyDictionary<string, string> arguments)
    {
        if (!arguments.TryGetValue("filePath", out var filePath) || string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("El parámetro 'filePath' es requerido.");

        var delimiter = arguments.TryGetValue("delimiter", out var d) ? d : ",";

        // Leer y parsear el CSV...
        var lines = File.ReadAllLines(filePath).Skip(1); // skip header
        var products = new List<ImportedProduct>();

        foreach (var line in lines)
        {
            var cols = line.Split(delimiter);
            products.Add(new ImportedProduct(
                Name: cols[0],
                Price: decimal.Parse(cols[1]),
                Description: cols[2],
                Line: cols[3],
                Category: cols[4],
                Images: [new ImportedProductImage(cols[5], decimal.Parse(cols[6]))],
                Active: bool.Parse(cols[7])
            ));
        }

        return products;
    }
}
```

### 4. Compilar

```bash
dotnet build -c Release
```

### 5. Copiar el DLL a la carpeta Plugins

Copiar el archivo compilado al directorio `Plugins/` dentro del output de `DarkKitchen.WebApi`:

```
DarkKitchen.WebApi/bin/Release/net8.0/Plugins/DarkKitchen.Importer.Csv.dll
```

**No es necesario** copiar `DarkKitchen.Importer.dll` a Plugins (ya está en el directorio principal de la aplicación).

### 6. Usar el importador (sin reiniciar)

**No es necesario reiniciar la aplicación.** Con solo copiar el DLL a `Plugins/`, el sistema lo descubre en la siguiente llamada. Si estás en la UI, recargá la pantalla de importación para que vuelva a pedir la lista. Estará disponible en:

- `GET /api/products/importers` — aparecerá con nombre "CSV", sus parámetros y descripción
- `POST /api/products/import` — se puede invocar con `{ "importerName": "CSV", "parameters": { "filePath": "..." } }`

### Eliminar un importador en tiempo de ejecución

Del mismo modo, **borrar el `.dll` de `Plugins/` lo quita de las opciones** sin reiniciar: como cada `.dll` se carga en memoria (no se bloquea el archivo), el SO permite eliminarlo mientras la app corre, y el siguiente `GET /api/products/importers` ya no lo enumera.

> Nota para entorno de desarrollo: si arrancás el backend con `dotnet run` (que recompila), el target `CopyPlugins` del `.csproj` vuelve a copiar los importadores de ejemplo (JSON/XML) a `Plugins/`. Eso solo afecta a esos dos DLLs de la solución; los plugins de terceros que copiás a mano no se restauran.

## API REST

### Listar importadores disponibles

```
GET /api/products/importers
Authorization: Bearer <token-admin>
```

Response:
```json
[
  {
    "name": "JSON",
    "description": "Importa productos desde un archivo JSON",
    "parameters": [
      {
        "name": "filePath",
        "label": "Ruta del archivo",
        "description": "Ruta absoluta al archivo JSON de productos",
        "required": true
      }
    ]
  }
]
```

### Ejecutar importación

```
POST /api/products/import
Authorization: Bearer <token-admin>
Content-Type: application/json
```

Body:
```json
{
  "importerName": "JSON",
  "parameters": {
    "filePath": "C:/datos/productos.json"
  }
}
```

Response:
```json
{
  "importedCount": 3,
  "errors": []
}
```

Response parcial (algunos productos inválidos):
```json
{
  "importedCount": 2,
  "errors": [
    "Producto 'AB': Product name must be between 10 and 50 characters."
  ]
}
```

## Requisitos técnicos del plugin

- Target framework: `net8.0`
- Debe referenciar **únicamente** `DarkKitchen.Importer`
- La clase importadora debe ser `public`, no abstracta, y tener constructor sin parámetros
- El `Name` debe ser único entre todos los importadores instalados
- La búsqueda por nombre es case-insensitive

## Importadores incluidos

La solución incluye dos importadores de ejemplo compilados:

- **JSON** (`DarkKitchen.Importer.Json.dll`): lee archivos JSON, parámetro `filePath`
- **XML** (`DarkKitchen.Importer.Xml.dll`): lee archivos XML, parámetro `filePath`

Los archivos de muestra están en `Datos/productos.json` y `Datos/productos.xml`.
