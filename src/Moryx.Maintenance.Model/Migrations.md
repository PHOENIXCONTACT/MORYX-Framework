## Migration calls

##  NpgSql Migration
dotnet ef migrations add InitialCreate --output-dir .\Model\Migrations\Npgsql\ --context NpgsqlMaintenanceContext
dotnet ef database update InitialCreate --context NpgsqlMaintenanceContext

##  SqLite Migration
dotnet ef migrations add InitialCreate --startup-project ..\..\src\StartProject.Asp\StartProject.Asp.csproj --output-dir .\Model\Migrations\Sqlite\ --context SqliteMaintenanceContext
dotnet ef database update InitialCreate --startup-project ..\..\src\StartProject\StartProject.csproj --context SqliteMaintenanceContext
