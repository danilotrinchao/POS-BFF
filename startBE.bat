@echo off
setlocal

rem Iniciar API 1
cd /d C:\caminho\para\a\api1\publish
start dotnet NomeDaSuaAPI1.dll

rem Iniciar API 2
cd /d C:\caminho\para\a\api2\publish
start dotnet NomeDaSuaAPI2.dll

rem Iniciar API 3
cd /d C:\caminho\para\a\api3\publish
start dotnet NomeDaSuaAPI3.dll

endlocal
pause
