@echo off
setlocal

rem Iniciar API 1 na porta 5000
cd /d C:\caminho\para\a\api1\publish
start dotnet NomeDaSuaAPI1.dll --urls "https://localhost:44363"

rem Iniciar API 2 na porta 5001
cd /d C:\caminho\para\a\api2\publish
start dotnet NomeDaSuaAPI2.dll --urls "https://localhost:7250"

rem Iniciar API 3 na porta 5002
cd /d C:\caminho\para\a\api3\publish
start dotnet NomeDaSuaAPI3.dll --urls "https://localhost:7209"

endlocal
pause
