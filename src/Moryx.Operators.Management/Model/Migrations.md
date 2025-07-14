## Migration calls

## Second Migration NpgSql
dotnet ef migrations add InitialCreate --startup-project ..\..\src\StartProject.Asp\StartProject.Asp.csproj --output-dir .\Model\Migrations\Npgsql\ --context NpgsqlOperatorsContext
dotnet ef database update InitialCreate --startup-project ..\..\src\StartProject\StartProject.csproj --context NpgsqlOperatorsContext

## Second Migration SqLite
dotnet ef migrations add InitialCreate --startup-project ..\..\src\StartProject.Asp\StartProject.Asp.csproj --output-dir .\Model\Migrations\Sqlite\ --context SqliteOperatorsContext
dotnet ef database update InitialCreate --startup-project ..\..\src\StartProject\StartProject.csproj --context SqliteOperatorsContext