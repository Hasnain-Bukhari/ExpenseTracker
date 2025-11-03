#!/bin/bash
# Start script for Railway deployment
# Railway sets the PORT environment variable automatically
# ASP.NET Core will use it automatically
cd out || exit 1
exec dotnet ExpenseTracker.API.dll

