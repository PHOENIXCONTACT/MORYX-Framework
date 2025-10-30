dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/Npgsql/ -c NpgsqlResourcesContext -- --connection "Host=localhost;Database=ResourcesContext;Username=postgres;Password=postgres"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c NpgsqlResourcesContext -- --connection "Host=localhost;Database=ResourcesContext;Username=postgres;Password=postgres"

dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/Sqlite/ -c SqliteResourcesContext -- --connection "Data Source=ResourcesContext.db"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c SqliteResourcesContext -- --connection "Data Source=ResourcesContext.db"
