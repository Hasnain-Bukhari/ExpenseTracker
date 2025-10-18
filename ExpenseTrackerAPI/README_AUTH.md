Auth subsystem summary

Run locally with Docker Compose:

1. Build and start services:
   docker-compose up --build

2. The API will be available at http://localhost:5000 (Swagger at /swagger when in Development)

Env vars:
- CONNECTION_STRING
- JWT_SECRET
- ASPNETCORE_ENVIRONMENT

Migration:
- docker/db-init runs SQL to create tables on Postgres container init

Next steps:
- Implement production SocialVerifier and email sender
- Add unit/integration tests
