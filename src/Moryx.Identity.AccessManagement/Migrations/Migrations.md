dotnet add ../StartProject.Asp/StartProject.Asp.csproj reference Moryx.Identity.AccessManagement.csproj
dotnet ef migrations add MORYX10 --project ./Moryx.Identity.AccessManagement.csproj --startup-project ../StartProject.Asp/StartProject.Asp.csproj --context MoryxIdentitiesDbContext --output-dir Migrations -- --connection "Username=postgres;Password=postgres;Host=localhost;Port=5432;Persist Security Info=True;Database=IdentityServer"
dotnet remove ../StartProject.Asp/StartProject.Asp.csproj reference Moryx.Identity.AccessManagement.csproj
