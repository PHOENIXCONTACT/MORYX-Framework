## Migration calls

## Second Migration NpgSql
dotnet ef migrations add InitialCreate --startup-project ..\..\src\StartProject\StartProject.csproj --output-dir .\Model\Migrations\Npgsql\ --context NpgsqlShiftsContext
dotnet ef database update InitialCreate --startup-project ..\..\src\StartProject\StartProject.csproj --context NpgsqlShiftsContext

## Second Migration SqLite
dotnet ef migrations add InitialCreate --startup-project ..\..\src\StartProject\StartProject.csproj --output-dir .\Model\Migrations\Sqlite\ --context SqliteIdentifierContext
dotnet ef database update InitialCreate --startup-project ..\..\src\StartProject\StartProject.csproj --context SqliteIdentifierContext