dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Model/Migrations/Npgsql/ -c NpgsqlProductsContext -- --connection "Host=localhost;Database=ProductsContext;Username=postgres;Password=postgres"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c NpgsqlProductsContext -- --connection "Host=localhost;Database=ProductsContext;Username=postgres;Password=postgres"

dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Model/Migrations/Sqlite/ -c SqliteProductsContext -- --connection "Data Source=ProductsContext.db"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c SqliteProductsContext -- --connection "Data Source=ProductsContext.db"
