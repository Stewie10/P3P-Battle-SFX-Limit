# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/p3ppc.hdsfx/*" -Force -Recurse
dotnet publish "./p3ppc.hdsfx.csproj" -c Release -o "$env:RELOADEDIIMODS/p3ppc.hdsfx" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location