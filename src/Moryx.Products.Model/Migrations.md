## Migration calls

dotnet ef migrations add InitialCreate --startup-project ..\StartProject\StartProject.csproj
dotnet ef database update InitialCreate --startup-project ..\StartProject\StartProject.csproj