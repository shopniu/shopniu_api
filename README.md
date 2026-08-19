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
- **Env vars/secrets** (connection string de producción, `Wompi__*`, `Identity__Issuer`): configuradas en el portal de Azure.
- **Secrets de GitHub (org):** `AZURE_CREDENTIALS` y `NUGET_PACKAGES_TOKEN`.

### Checklist pre-deploy

- [ ] Variables de entorno definidas en el host (sección arriba).
- [ ] `ConnectionStrings__DefaultConnection` apuntando a la Postgres de Azure.
- [ ] `Wompi__*` con las claves correctas (probar webhook con firma).
- [ ] Migraciones aplicadas en el ambiente destino.
- [ ] `/health` responde 200.
- [ ] HSTS/forwarded headers activos (no-Development).
- [ ] Sin valores sensibles en `appsettings` ni logs.

## Troubleshooting

- **401 / NU1301 en restore:** variable `GITHUB_PACKAGES_TOKEN` no definida o PAT expirado.
- **401 en endpoints:** el token no es válido o el issuer (`Identity__Issuer`) no coincide con identity.
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
