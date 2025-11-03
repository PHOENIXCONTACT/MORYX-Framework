dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/SqlServer -c SqlServerTestModelContext
dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/Sqlite -c SqliteTestModelContext
dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/Npgsql -c NpgsqlTestModelContext

dotnet ef database update InitialCreate --startup-project ..\StartProject.Core\StartProject.Core.csproj
