# Expense Tracker

Expense tracking app: .NET 8 API + Vue 3 (Vuetify) frontend, PostgreSQL database.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [PostgreSQL 15+](https://www.postgresql.org/download/)

---

## Quick start

### 1. Database

Create the database (or use default `postgres`). The API uses the connection in `ExpenseTrackerAPI/src/ExpenseTracker.API/appsettings.json` (default: `Database=postgres`, `Username=hasnainbukhari`, empty password).

```bash
# If using default postgres DB, just run the schema:
psql -d postgres -f ExpenseTrackerAPI/scripts/init-schema.sql
```

If your Postgres user is different, use `-U your_username`. Run the script once per database.

### 2. Backend API

Open the solution in **Rider** (or VS) and run **ExpenseTracker.API**, or from a terminal:

```bash
cd ExpenseTrackerAPI/src/ExpenseTracker.API
dotnet restore
dotnet run
```

The API listens on the URL shown in the console (e.g. `http://localhost:5000` or `https://localhost:5001`). Connection string is in `appsettings.json`; override with the `CONNECTION_STRING` environment variable if needed.

### 3. Frontend

In a new terminal:

```bash
cd ExpenseTrackerUI
npm install
npm run dev
```

Open the URL shown (e.g. `http://localhost:5173`). Ensure `.env` has `VITE_API_BASE` pointing at your API (e.g. `http://localhost:5001/v1/` if the API runs on port 5001).

---

## Project structure

```
ExpenseTracker/
├── ExpenseTrackerAPI/           # .NET 8 Web API
│   ├── src/
│   │   ├── ExpenseTracker.API/  # Controllers, startup
│   │   ├── ExpenseTracker.Service/
│   │   ├── ExpenseTracker.Repository/
│   │   └── ExpenseTracker.Dtos/
│   └── scripts/
│       └── init-schema.sql      # DB schema (run once)
├── ExpenseTrackerUI/            # Vue 3 + Vuetify + Vite
│   ├── src/
│   │   ├── components/
│   │   ├── views/
│   │   ├── stores/
│   │   └── lib/
│   └── .env                     # VITE_API_BASE
└── README.md
```

---

## Configuration

| Where | What |
|-------|------|
| **API → DB** | `ExpenseTrackerAPI/src/ExpenseTracker.API/appsettings.json` → `ConnectionStrings:DefaultConnection` or `CONNECTION_STRING` |
| **Frontend → API** | `ExpenseTrackerUI/.env` → `VITE_API_BASE` (e.g. `http://localhost:5001/v1/`) |

---

## Troubleshooting

- **Role "postgres" does not exist** — On macOS Homebrew, the default superuser is your Mac username. Use that in `appsettings.json` (`Username=hasnainbukhari`, password empty). The repo is already set up for this.
- **Column does not exist** — Run `ExpenseTrackerAPI/scripts/init-schema.sql` on the same database the API uses.
- **Frontend can’t reach API** — Check `VITE_API_BASE` in `.env` matches the API URL and port; restart `npm run dev` after changing `.env`.

---

## License

MIT.
