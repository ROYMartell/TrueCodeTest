# TrueCodeTest

Набор микросервисов на .NET 8 / PostgreSQL:
- **Migrator** — отдельное консольное приложение, накатывает миграции на две БД (`users_db`, `finance_db`).
- **CurrencyFetcher** — фоновый `BackgroundService`, периодически тянет курсы с `cbr.ru` и сохраняет в `finance_db`.
- **Users.Api** — регистрация / логин / refresh / logout, JWT access + refresh-токены с ротацией и revoke.
- **Finance.Api** — справочник валют, избранное пользователя.
- **Gateway** — YARP reverse-proxy с JWT-валидацией и Swagger UI.

HTTP-сервисы **Users.Api** и **Finance.Api** реализованы на **ASP.NET Core Minimal API**.

## Требования

- .NET SDK 8.0.206+ (зафиксировано в `global.json`)
- Docker Desktop (для `docker compose`)
- Опционально — Postgres 16 локально (например порт 5432)

## Запуск через docker compose

```bash
docker compose build
docker compose up -d
```

Доступные endpoint'ы:

- Gateway: http://localhost:5100/swagger — в выпадающем списке **Users** и **Finance** (отдельные `swagger.json` с каждого сервиса через YARP)
- Users.Api: http://localhost:5101/swagger
- Finance.Api: http://localhost:5102/swagger
- Postgres: `localhost:5432`, базы `users_db` и `finance_db` (создаются init-скриптом `deploy/docker/init-db.sql` при первом старте контейнера)

Порядок запуска в compose:
1. `postgres` поднимается первым, создаёт обе БД через init-скрипт, ожидает `pg_isready`.
2. `migrator` прогоняется один раз до «successfully exited» (накатывает EF миграции в обе БД).
3. `currency-fetcher`, `users-api`, `finance-api` стартуют после успешного migrator.
4. `gateway` стартует после users/finance.

> Подпись JWT можно переопределить переменной `JWT_SIGNING_KEY` в `.env` (минимум 32 байта).

## Локальный запуск без Docker

1. Поднять Postgres с двумя БД:
   ```powershell
   docker compose up -d postgres
   ```
2. Прогнать миграции:
   ```powershell
   dotnet run --project src/Migrator/TrueCodeTest.Migrator
   ```
3. Запустить сервисы (в разных терминалах):
   ```powershell
   dotnet run --project src/Users/TrueCodeTest.Users.Api
   dotnet run --project src/Finance/TrueCodeTest.Finance.Api
   dotnet run --project src/CurrencyFetcher/TrueCodeTest.CurrencyFetcher
   dotnet run --project src/Gateway/TrueCodeTest.Gateway
   ```

## Тесты

```powershell
dotnet test TrueCodeTest.sln
```

Покрытие — unit-тесты на handlers в `Users.Application` и `Finance.Application` + `JwtTokenService`.

## Сценарий end-to-end

1. `POST /api/users/register` — `{ "name": "alice", "password": "secret123" }` → `201 Created`.
2. `POST /api/users/login` → `{ accessToken, refreshToken, ... }`.
3. `GET /api/finance/currencies` c `Authorization: Bearer {access}` → список валют из ЦБ.
4. `POST /api/finance/favorites/840` — добавить USD в избранное.
5. `GET /api/finance/favorites` — избранные валюты с актуальными полями (`rate`, `nominal`, …).
6. `POST /api/users/refresh` — ротация токенов (старый refresh revoked, новый выдан).
7. `POST /api/users/logout` — revoke текущего refresh-токена.

## Структура репозитория

```
src/
  Shared/                 # общие библиотеки: Kernel, Mediator (ValidationBehavior), Web (ProblemDetails + Swagger + exception handler), Auth, Contracts
  Users/
    TrueCodeTest.Users.Domain
    TrueCodeTest.Users.Application
    TrueCodeTest.Users.Infrastructure
    TrueCodeTest.Users.Contracts
    TrueCodeTest.Users.Mappers
    TrueCodeTest.Users.Api
  Finance/
    TrueCodeTest.Finance.Domain
    TrueCodeTest.Finance.Application
    TrueCodeTest.Finance.Infrastructure
    TrueCodeTest.Finance.Contracts
    TrueCodeTest.Finance.Mappers
    TrueCodeTest.Finance.Api
  Migrator/TrueCodeTest.Migrator
  CurrencyFetcher/TrueCodeTest.CurrencyFetcher
  Gateway/TrueCodeTest.Gateway
tests/
  TrueCodeTest.Users.UnitTests
  TrueCodeTest.Finance.UnitTests
deploy/docker/                    # Dockerfile'ы по сервисам
docker-compose.yml
```
