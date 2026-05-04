@echo off

set /p ADMIN_EMAIL=Введите email администратора:
set /p ADMIN_PASSWORD=Введите пароль:

setx ADMIN_EMAIL %ADMIN_EMAIL%
setx ADMIN_PASSWORD %ADMIN_PASSWORD%

dotnet run
pause