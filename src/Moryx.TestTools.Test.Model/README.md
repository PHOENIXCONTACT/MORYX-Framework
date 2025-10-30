dotnet ef migrations add InitialCreate --startup-project ..\StartProject.Asp\StartProject.Asp.csproj --context NpgsqlTestModelContext
dotnet ef database update InitialCreate --startup-project ..\StartProject.Asp\StartProject.Asp.csproj --context NpgsqlTestModelContext

