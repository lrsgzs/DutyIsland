Write-Host 开始构建 DutyIsland
dotnet publish -p:CreateCipx=true -o .\bin\Release\net8.0-windows\ -c Release
