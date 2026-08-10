# Deployment Checklist

Use this checklist before each deployment.

## Development

- [ ] Run dotnet user-secrets init once in this project.
- [ ] Set all required secrets with dotnet user-secrets set.
- [ ] Confirm local database connectivity.
- [ ] Run dotnet build and confirm success.
- [ ] Run dotnet run and verify health endpoint returns 200 at /health.
- [ ] Test one payment flow against Wompi sandbox.

## Staging

- [ ] Create environment variables in hosting platform.
- [ ] Set ConnectionStrings\_\_DefaultConnection.
- [ ] Set Wompi\_\_IntegrityKey.
- [ ] Set Wompi\_\_PublicKey.
- [ ] Set Wompi\_\_PrivateKey.
- [ ] Set Wompi\_\_ApiUrl.
- [ ] Set Wompi\_\_EventsKey.
- [ ] Enable HTTPS and verify certificate is valid.
- [ ] Configure load balancer health check path to /health.
- [ ] Verify forwarded headers are enabled by confirming https redirect behavior behind proxy.
- [ ] Run database migrations in staging.
- [ ] Execute smoke tests for users, products, transactions, and payment webhook.

## Production

- [ ] Store secrets in managed vault service.
- [ ] Grant app identity access to secret store only for required keys.
- [ ] Confirm all six required variables are available at runtime.
- [ ] Verify no sensitive values exist in appsettings files or deployment logs.
- [ ] Configure autoscaling policy based on CPU or request rate.
- [ ] Configure alerts for failed health checks and 5xx error rate.
- [ ] Verify rollback strategy and previous stable artifact availability.
- [ ] Run post-deploy smoke test including webhook signature validation.

## Azure Specific

- [ ] In App Service, add variables under Environment variables.
- [ ] For secrets, use Key Vault references.
- [ ] Enable App Service Health Check using /health.
- [ ] Verify Always On is enabled for API workloads.
- [ ] Review Application Insights exceptions after deployment.

## AWS Specific

- [ ] In ECS task definition, use Secrets Manager for sensitive values.
- [ ] Configure target group health check path as /health.
- [ ] Ensure ALB forwards X-Forwarded-For and X-Forwarded-Proto.
- [ ] Review CloudWatch logs for startup validation errors.
- [ ] Confirm security group rules allow expected inbound traffic only.

## Quick Verification Commands

Run from project root:

1. dotnet build
2. dotnet run
3. curl http://localhost:5000/health

If the health endpoint fails, inspect startup logs first. The app now validates configuration at startup and fails early when required values are missing.
