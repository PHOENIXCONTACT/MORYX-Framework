## Migration calls

dotnet ef migrations add InitialCreate --startup-project ..\StartProject\StartProject.csproj --output-dir Model/Migrations
dotnet ef database update InitialCreate --startup-project ..\StartProject\StartProject.csproj

