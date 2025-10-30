dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/Npgsql/ -c NpgsqlShiftsContext -- --connection "Host=localhost;Database=ShiftsContext;Username=postgres;Password=postgres"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c NpgsqlShiftsContext -- --connection "Host=localhost;Database=ShiftsContext;Username=postgres;Password=postgres"

dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/Sqlite/ -c SqliteShiftsContext -- --connection "Data Source=ShiftsContext.db"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c SqliteShiftsContext -- --connection "Data Source=ShiftsContext.db"
