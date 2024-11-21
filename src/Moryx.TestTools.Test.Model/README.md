dotnet ef migrations add InitialCreate  --startup-project ..\StartProject.Core\StartProject.Core.csproj
dotnet ef database update InitialCreate  --startup-project ..\StartProject.Core\StartProject.Core.csproj

