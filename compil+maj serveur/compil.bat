@echo off

echo "Tapez 1 pour compilation Linux et 2 pour windows"
set/p choix="choisissez: "

if %choix%==1 goto LINUX

dotnet publish -c Release
start D:\programmation\C#\Serveur-AFKSimulator\bin\Release\net8.0\publish
goto END

:LINUX
dotnet publish -c Release -r linux-x64
start D:\programmation\C#\Serveur-AFKSimulator\bin\Release\net8.0\linux-x64\publish

:END

