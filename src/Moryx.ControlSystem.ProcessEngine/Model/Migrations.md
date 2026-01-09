dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Model/Migrations/Npgsql/ -c NpgsqlProcessContext -- --connection "Host=localhost;Database=ProcessContext;Username=postgres;Password=postgres"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c NpgsqlProcessContext -- --connection "Host=localhost;Database=ProcessContext;Username=postgres;Password=postgres"

dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Model/Migrations/Sqlite/ -c SqliteProcessContext -- --connection "Data Source=ProcessContext.db"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c SqliteProcessContext -- --connection "Data Source=ProcessContext.db"
