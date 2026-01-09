dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/Npgsql/ -c NpgsqlTestModelContext -- --connection "Host=localhost;Database=TestModelContext;Username=postgres;Password=postgres"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c NpgsqlTestModelContext -- --connection "Host=localhost;Database=TestModelContext;Username=postgres;Password=postgres"

dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/Sqlite/ -c SqliteTestModelContext -- --connection "Data Source=TestModelContext.db"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c SqliteTestModelContext -- --connection "Data Source=TestModelContext.db"

dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Migrations/SqlServer/ -c SqlServerTestModelContext -- --connection "Server=localhost;Initial Catalog=TestModelContext;User Id=sa;Password=password;TrustServerCertificate=True;"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c SqlServerTestModelContext -- --connection "Server=localhost;Initial Catalog=TestModelContext;User Id=sa;Password=password;TrustServerCertificate=True;"
