# Deployment Environment Guide (Azure/AWS)

This API is now prepared to read sensitive configuration from environment variables.
Use this document as the source of truth for local, Azure, and AWS setup.

For operational readiness before each release, use DEPLOYMENT_CHECKLIST.md.

## 1) Variables you must define

ASP.NET Core uses double underscore (`__`) to map nested configuration keys.

- `ConnectionStrings__DefaultConnection`
- `Wompi__IntegrityKey`
- `Wompi__PublicKey`
- `Wompi__PrivateKey`
- `Wompi__ApiUrl`
- `Wompi__EventsKey`

## 2) Local development (recommended: User Secrets)

Run these commands in the project root:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=shopniu_api_db;Username=postgres;Password=postgres"
dotnet user-secrets set "Wompi:IntegrityKey" "your_integrity_key"
dotnet user-secrets set "Wompi:PublicKey" "your_public_key"
dotnet user-secrets set "Wompi:PrivateKey" "your_private_key"
dotnet user-secrets set "Wompi:ApiUrl" "https://api-sandbox.co.uat.wompi.dev/v1"
dotnet user-secrets set "Wompi:EventsKey" "your_events_key"
```

## 3) Azure App Service

In your App Service:

1. Go to `Settings > Environment variables`.
2. Add each variable from section 1.
3. Save and restart the app.

Important:

- Mark sensitive values as secrets.
- Prefer Azure Key Vault references for production secrets.

## 4) AWS (ECS / Elastic Beanstalk)

### ECS/Fargate

- Add variables in task definition under `environment` or `secrets`.
- Prefer AWS Secrets Manager for:
  - `ConnectionStrings__DefaultConnection`
  - `Wompi__PrivateKey`
  - `Wompi__EventsKey`

### Elastic Beanstalk

- Go to `Configuration > Software > Environment properties`.
- Add variables from section 1.
- Apply changes.

## 5) Health endpoint for load balancers

The API now exposes:

- `GET /health`

Use this endpoint as health check target in Azure/AWS load balancers.

## 6) Current startup behavior

- App fails fast if `ConnectionStrings:DefaultConnection` is missing.
- App validates Wompi configuration at startup.
- Forwarded headers are enabled for reverse proxy deployments.
- HSTS is enabled in non-development environments.

## 7) Recommended deployment flow

1. Configure environment variables or secret references.
2. Run database migration in target environment.
3. Deploy artifact.
4. Validate /health endpoint.
5. Execute smoke tests for core endpoints and payment webhook.
