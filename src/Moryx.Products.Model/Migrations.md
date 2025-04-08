## Migration calls

// Set environment variable
$env:EFCORETOOLSDB = "Host=localhost;Database=ag;Username=postgres;Password=postgres"
dotnet ef migrations add InitialCreate --context ProductsContext --output-dir .\Migrations\Npgsql\ --startup-project ..\StartProject.Asp\StartProject.Asp.csproj


// Set environment variable
$env:EFCORETOOLSDB = "DataSource=products.sqlite"
dotnet ef migrations add NewDefaults --context SqliteProductsContext --output-dir .\Migrations\Sqlite\ --startup-project ..\StartProject.Asp\StartProject.Asp.csproj
