dotnet ef migrations add MORYX10 --project --startup-project ../StartProject.Asp/StartProject.Asp.csproj ./Moryx.Identity.AccessManagement.csproj --context MoryxIdentitiesDbContext --output-dir Migrations --connection "Username=postgres;Password=postgres;Host=localhost;Port=5432;Persist Security Info=True;Database=IdentityServer"


