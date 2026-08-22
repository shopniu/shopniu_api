# shopniu-api

API de negocio de ShopNiu: catálogo, usuarios, transacciones y pagos (Wompi). ASP.NET Core Web API sobre .NET 10, autenticada con tokens OpenIddict emitidos por `shopniu-identity`.

## Stack

- ASP.NET Core (`net10.0`), `Microsoft.NET.Sdk.Web`
- Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.10
- EF Core + Npgsql (PostgreSQL) 10.0.0
- OpenIddict.AspNetCore + OpenIddict.EntityFrameworkCore 7.6.0
- FluentValidation.DependencyInjectionExtensions 12.1.1
- Microsoft.Extensions.Http.Resilience 10.8.0
- Swashbuckle 10.2.3
- Azure.Storage.Blobs / Azure.Identity (media en Blob Storage, SAS + Managed Identity)
- SkiaSharp (variantes de imagen: web/thumb)
- Shopniu.Shared 1.0.3 (paquete privado)

## Configuración

Secciones clave en `appsettings.json` (sobreescribibles por variables de entorno con `__`):

| Clave | Descripción |
|---|---|
| `ConnectionStrings:DefaultConnection` | Postgres, base `shopniu_api_db` |
| `Wompi:IntegrityKey` / `PublicKey` / `PrivateKey` / `EventsKey` / `ApiUrl` | Integración de pagos (sandbox en dev) |
| `Identity:Issuer` | Issuer de identity (`https://localhost:7145/` en dev) |
| `Scalability:RateLimiting` | Límites de rate limiting |
| `Database:Migration/Seeding:RunOnStartup` | Migraciones/seeders al iniciar (false por defecto) |
| `Storage:AccountName` / `ContainerName` / `PublicBaseUrl` | Blob Storage para media (contenedor público `media`) |
| `Storage:UseConnectionString` / `ConnectionString` | Modo dev con Azurite (solo local) |
| `Storage:ManagedIdentityClientId` | Managed Identity (producción) para firmar SAS |
| `Storage:SasDurationMinutes` / `MaxSizeBytes` / `AllowedContentTypes` | Límites de upload |
| `GET /health` | Endpoint de health check para load balancers |

**Repos:** https://github.com/DanielAmado11/shopniu_api (rama `main`).

## Correr localmente

```powershell
dotnet restore
dotnet build -c Release
dotnet run
```

Con docker:

```powershell
docker compose up -d --build shopniu-api
```

Puerto: **8080 (host) → 8080 (contenedor)**. Requiere Postgres (`shopniu_api_db`) e identity para validar tokens.

### Media en local (Azurite)

`docker compose` levanta un contenedor **Azurite** (emulador de Blob Storage) en el puerto 10000. El API lo usa automáticamente en `Development` (connection string + URLs públicas por `http://localhost:10000/devstoreaccount1/media/...`). Al arrancar, el API crea el contenedor `media` si no existe.

Si corrés la API con `dotnet run` sin docker, levantá Azurite aparte:

```powershell
docker run -d --name azurite -p 10000:10000 mcr.microsoft.com/azure-storage/azurite
```

### Media (imágenes)

El modelo de media vive en `Domain/Entities/MediaEntity/MediaAsset.cs`: variantes `original`/`web`/`thumb`, vínculo opcional a `Product` (`ProductId`) y flag `IsMain` que alimenta `Product.ImageUrl`. Las imágenes se guardan en un **contenedor público** de Blob Storage; el front sube directo a Blob con una **SAS de escritura efímera** emitida por este servicio (el binario nunca pasa por el gateway). Al confirmar, se generan las variantes con **SkiaSharp** (web 1280px, thumb 320px, JPEG).

Endpoints (`/api/v1/media`, política `product.create`):

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/v1/media/upload-url` | Devuelve `{ uploadUrl (SAS), blobPath, publicUrl }` |
| POST | `/api/v1/media` | Confirma el upload: valida imagen, genera variantes, persiste `MediaAsset` (opcional `productId`/`isMain`) |
| POST | `/api/v1/media/{id}/main` | Marca la imagen como principal del producto (sincroniza `Product.ImageUrl`) |
| POST | `/api/v1/media/link` | Vincula media huérfana a un producto (`{ productId, mediaIds }`) |
| DELETE | `/api/v1/media/{id}` | Borra blobs (original + variantes) y el registro |

Flujo típico del front: pedir `upload-url` → `PUT` directo a Blob → `POST /media` (confirmar) → si el producto no existía aún, `POST /media/link`. La primera imagen de un producto se vuelve la principal automáticamente.

### Paquete privado (auth)

Igual que en `shopniu-identity`: el `nuget.config` lee `%GITHUB_PACKAGES_TOKEN%`; el PAT (`read:packages`) va como variable de entorno local y como secret `NUGET_PACKAGES_TOKEN` en CI.

### Variables de entorno requeridas (producción / staging)

```text
ConnectionStrings__DefaultConnection
Wompi__IntegrityKey
Wompi__PublicKey
Wompi__PrivateKey
Wompi__ApiUrl
Wompi__EventsKey
Storage__AccountName
Storage__PublicBaseUrl
Storage__ManagedIdentityClientId
```

Para desarrollo local con User Secrets:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=shopniu_api_db;Username=postgres;Password=postgres"
dotnet user-secrets set "Wompi:IntegrityKey" "your_integrity_key"
dotnet user-secrets set "Wompi:PublicKey" "your_public_key"
dotnet user-secrets set "Wompi:PrivateKey" "your_private_key"
dotnet user-secrets set "Wompi:ApiUrl" "https://api-sandbox.co.uat.wompi.dev/v1"
dotnet user-secrets set "Wompi:EventsKey" "your_events_key"
```

Comportamiento en startup: falla rápido si falta `ConnectionStrings:DefaultConnection`; valida la config de Wompi; forwarded headers habilitados (detrás de proxy); HSTS activo fuera de Development.

## CI/CD

Workflow `.github/workflows/ci.yml` (push a `main` y PRs):

1. **build** — restore (con `GITHUB_PACKAGES_TOKEN: ${{ secrets.NUGET_PACKAGES_TOKEN }}`), `dotnet format --verify-no-changes`, build Release, test.
2. **docker-publish** — imagen a `ghcr.io/danielamado11/shopniu_api` (tags `latest`/`sha`/semver). Build-args `NUGET_GITHUB_TOKEN` y `GITHUB_ACTOR` para el restore dentro del Dockerfile.
3. **deploy** — actualiza la container app en Azure.

## Deploy a Azure (Container Apps)

- **Container app:** `shopniu-api`
- **Entorno:** `thankfulmushroom-4e17c339` (westus)
- **Resource group:** `shopniu`
- **Imagen:** `ghcr.io/danielamado11/shopniu_api:<sha>`, `targetPort 8080`
- **Registry GHCR:** `ghcr.io`, usuario `DanielAmado11`, password `NUGET_PACKAGES_TOKEN`
- **Env vars/secrets** (connection string de producción, `Wompi__*`, `Identity__Issuer`, `Storage__*`): configuradas en el portal de Azure.
- **Secrets de GitHub (org):** `AZURE_CREDENTIALS` y `NUGET_PACKAGES_TOKEN`.

### Media en producción (recursos Azure)

- **Storage account** `shopniumedia` (Standard_LRS, hot, *anonymous blob access* habilitado) con contenedor `media` en **lectura pública** (`Blob`).
- **Managed identity** `id-shopniu-media` con rol **Storage Blob Data Contributor** sobre la cuenta, asociada a la container app. El API firma SAS con user-delegation (sin connection strings).
- **CORS** en el storage account: `AllowedOrigins` = origen de `shopniu-web`, `AllowedMethods` = `PUT` (necesario para el upload directo desde el navegador).

Provisionar (una vez):

```powershell
az storage account create -n shopniumedia -g shopniu -l westus --sku Standard_LRS --kind StorageV2 --allow-blob-public-access true
az storage container create -n media --account-name shopniumedia --public-access blob --auth-mode login
az identity create -n id-shopniu-media -g shopniu
az role assignment create --assignee <mi-client-id> --role "Storage Blob Data Contributor" --scope /subscriptions/<sub>/resourceGroups/shopniu/providers/Microsoft.Storage/storageAccounts/shopniumedia
az containerapp identity assign -n shopniu-api -g shopniu --user-assigned <mi-resource-id>
# En la container app: env vars Storage__AccountName, Storage__PublicBaseUrl, Storage__ManagedIdentityClientId
# CORS: az storage cors add ...
```

### Checklist pre-deploy

- [ ] Variables de entorno definidas en el host (sección arriba).
- [ ] `ConnectionStrings__DefaultConnection` apuntando a la Postgres de Azure.
- [ ] `Wompi__*` con las claves correctas (probar webhook con firma).
- [ ] `Storage__*` con la cuenta, base URL pública y Managed Identity correctas.
- [ ] Storage account con acceso anónimo habilitado, contenedor `media` en lectura pública y CORS con `PUT`.
- [ ] Migraciones aplicadas en el ambiente destino.
- [ ] `/health` responde 200.
- [ ] HSTS/forwarded headers activos (no-Development).
- [ ] Sin valores sensibles en `appsettings` ni logs.

## Troubleshooting

- **401 / NU1301 en restore:** variable `GITHUB_PACKAGES_TOKEN` no definida o PAT expirado.
- **401 en endpoints:** el token no es válido o el issuer (`Identity__Issuer`) no coincide con identity.
- **403 en upload/confirm:** el permiso `product.create` no está en el token (faltó re-login) o la SAS venció (10 min por defecto).
- **403 de Managed Identity al firmar SAS:** el rol `Storage Blob Data Contributor` no está asignado a `id-shopniu-media` sobre la cuenta.
- **CORS al subir desde el navegador:** falta `PUT` en `AllowedMethods` o el origen del front en `AllowedOrigins` del storage account.
- **`dotnet format --verify-no-changes` falla en CI:** correr `dotnet format` local y commitear.

## Convención de commits

Misma convención para todo el workspace (ver `AGENTS.md` en la raíz del proyecto):

```
tipo(scope): descripción en español
```

- **tipo** (obligatorio): `feat`, `fix`, `test`, `refactor`, `chore`, `docs`
- **scope** (opcional): área afectada, ej. `transactions`, `webhook`, `db`, `ci/cd`
- **descripción**: en español, minúsculas, concisa, en pasado o imperativo (ej. `se corrigió`)

Ejemplos:

```
feat(transactions): persistir user payment data y delivery al crear transacción
fix(cart): se corrigió el cálculo del subtotal
chore(db): migración AddDeliveryAndPaymentDataFlow
docs: documentar convención de commits
```

Antes de commitear: `dotnet build -c Release` sin errores y `dotnet format --verify-no-changes` en 0.
