dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/Npgsql/ -c NpgsqlTestModelContext -- --connection "Host=localhost;Database=TestModelContext;Username=postgres;Password=postgres"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c NpgsqlTestModelContext -- --connection "Host=localhost;Database=TestModelContext;Username=postgres;Password=postgres"

dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/Sqlite/ -c SqliteTestModelContext -- --connection "Data Source=TestModelContext.db"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c SqliteTestModelContext -- --connection "Data Source=TestModelContext.db"
