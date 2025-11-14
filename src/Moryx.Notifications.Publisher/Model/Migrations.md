dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Model/Migrations/Npgsql/ -c NpgsqlNotificationsContext -- --connection "Host=localhost;Database=NotificationsContext;Username=postgres;Password=postgres"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c NpgsqlNotificationsContext -- --connection "Host=localhost;Database=NotificationsContext;Username=postgres;Password=postgres"

dotnet ef migrations add InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -o ./Model/Migrations/Sqlite/ -c SqliteNotificationsContext -- --connection "Data Source=NotificationsContext.db"
dotnet ef database update InitialCreate -s ../StartProject.Asp/StartProject.Asp.csproj -c SqliteNotificationsContext -- --connection "Data Source=NotificationsContext.db"
