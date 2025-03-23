#!/bin/sh
# Start .NET API in the background
dotnet /app/AccessControll-API.dll &

# Start Nginx
nginx -g "daemon off;"
