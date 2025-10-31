dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/Npgsql/ -c NpgsqlOrdersContext -- --connection "Host=localhost;Database=OrdersContext;Username=postgres;Password=postgres"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c NpgsqlOrdersContext -- --connection "Host=localhost;Database=OrdersContext;Username=postgres;Password=postgres"

dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/Sqlite/ -c SqliteOrdersContext -- --connection "Data Source=OrdersContext.db"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c SqliteOrdersContext -- --connection "Data Source=OrdersContext.db"
