@echo off
chcp 65001 >nul
color 0A
title ASSC Setup Wizard

setlocal EnableDelayedExpansion

set DOTNET_OK=0
set EF_OK=0
set SQL_OK=0

call :ShowHeader
call :CheckDotnet
call :CheckEF
call :CheckSQL
call :ShowResult

echo.
echo Нажмите любую клавишу для выхода...
pause >nul
exit /b

:: ===============================
:ShowHeader
echo ================================
echo        ASSC SETUP WIZARD
echo ================================
echo.
exit /b

:: ===============================
:CheckDotnet
set DOTNET_OK=0
set DOTNET_ATTEMPTS=0

:DotnetLoop
set /a DOTNET_ATTEMPTS+=1
echo Проверка .NET SDK 8...
set "DOTNET_VERSION="
set "DOTNET_8_FOUND=0"
for /f "delims=" %%V in ('dotnet --list-sdks 2^>nul') do (
    echo %%V | findstr /R "^8\." >nul
    if !errorlevel! EQU 0 (
        set "DOTNET_8_FOUND=1"
        if not defined DOTNET_VERSION set "DOTNET_VERSION=%%V"
    )
)
if "!DOTNET_8_FOUND!"=="1" (
    set DOTNET_OK=1
    echo [✔] Найден .NET SDK 8.x: !DOTNET_VERSION!
    goto DotnetDone
)
for /f "delims=" %%V in ('dotnet --version 2^>nul') do set "DOTNET_VERSION=%%V"
if defined DOTNET_VERSION (
    echo [❌] Найден .NET SDK !DOTNET_VERSION! (требуется 8.x)
) else (
    echo [❌] .NET SDK не найден
)

if !DOTNET_ATTEMPTS! GEQ 2 (
    echo [✖] Повторная проверка .NET завершена, но требуемая версия не установлена
    goto DotnetDone
)

set /p "DOTNET_ANSWER=Установить .NET SDK 8? (да/нет): "
if /i "!DOTNET_ANSWER!"=="да" (
    echo Устанавливаю .NET SDK 8...
    winget install -e --id Microsoft.DotNet.SDK.8 --accept-package-agreements --accept-source-agreements
    if !errorlevel! EQU 0 (
        echo [✔] Установка завершена. Далее перезапустите снова setup.bat...
        pause >nul
        goto DotnetLoop
    ) else (
        echo [❌] Ошибка установки. Открываю страницу загрузки...
        start https://dotnet.microsoft.com/en-us/download/dotnet/8.0
        pause >nul
        goto DotnetLoop
    )
)

echo [!] Пропуск установки .NET SDK
:DotnetDone
exit /b

:: ===============================
:CheckEF
set EF_OK=0
set EF_ATTEMPTS=0

:EFLoope
set /a EF_ATTEMPTS+=1
echo.
echo Проверка Entity Framework CLI...
set "EF_VERSION="
for /f "delims=" %%V in ('dotnet ef --version 2^>nul') do set "EF_VERSION=%%V"
if defined EF_VERSION (
    set EF_OK=1
    echo [✔] Найден EF CLI: !EF_VERSION!
    goto EFDone
)

echo [❌] EF CLI не найден
if !EF_ATTEMPTS! GEQ 2 (
    echo [✖] Повторная проверка EF CLI завершена, но компонент не установлен
    goto EFDone
)

set /p "EF_ANSWER=Установить EF CLI? (да/нет): "
if /i "!EF_ANSWER!"=="да" (
    echo Устанавливаю EF CLI...
    dotnet tool install -g dotnet-ef
    echo Проверяю EF CLI заново...
    pause >nul
    goto EFLoope
)

echo [!] Пропуск установки EF CLI
:EFDone
exit /b

:: ===============================
:CheckSQL
set SQL_OK=0
set SQL_ATTEMPTS=0

:SQLLoop
set /a SQL_ATTEMPTS+=1
echo.
echo Проверка SQL Server tools...
sqlcmd -? >nul 2>&1
if %errorlevel%==0 (
    set SQL_OK=1
    echo [✔] SQL Server tools найдены
    goto SQLDone
)

echo [❌] SQL Server tools не найдены
if !SQL_ATTEMPTS! GEQ 2 (
    echo [✖] Повторная проверка SQL Server tools завершена, но компонент не установлен
    goto SQLDone
)

set /p "SQL_ANSWER=Открыть страницу установки SQL Server? (да/нет): "
if /i "!SQL_ANSWER!"=="да" (
    echo Устанавливаю SQL Server Express...
    winget install -e --id Microsoft.SQLServer.2022.Express --accept-package-agreements --accept-source-agreements
    if !errorlevel! EQU 0 (
        echo [✔] Установка завершена. Повторяю проверку...
        pause >nul
        goto SQLLoop
    ) else (
        echo [❌] Ошибка установки. Открываю страницу загрузки...
        start https://www.microsoft.com/sql-server/sql-server-downloads
        pause >nul
        goto SQLLoop
    )
)

echo [!] Пропуск установки SQL Server tools
:SQLDone
exit /b

:: ===============================
:ShowResult
echo.
echo ================================
echo            ИТОГ
echo ================================
echo.
if %DOTNET_OK%==1 (echo .NET:      OK) else (echo .NET:      NOT OK)
if %EF_OK%==1 (echo EF CLI:    OK) else (echo EF CLI:    NOT OK)
if %SQL_OK%==1 (echo SQL Server: OK) else (echo SQL Server: NOT OK)
echo.
if %DOTNET_OK%==1 if %EF_OK%==1 if %SQL_OK%==1 (
    echo ✔ SYSTEM READY
) else (
    echo ❌ INSTALL REQUIRED COMPONENTS
)
exit /b