@echo off
chcp 65001 >nul
title Start ASSC

REM Запуск ASP.NET Core приложения ASSC
cd /d "%~dp0"

necho Запуск ASSC...
dotnet run --project "%~dp0ASSC.csproj"

necho.
echo Программа завершила работу.
pause