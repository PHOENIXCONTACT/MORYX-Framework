## Migration calls

# Add Migration

Add-Migration -Name InitialCreate -ProjectName Moryx.Products.Model -ConnectionString "Username=postgres;Password=postgres;Host=localhost;Port=5432;Persist Security Info=True;Database=products" -ConnectionProviderName Npgsql -Verbose

# Update Migration

Update-Database -TargetMigration InitialCreate -ProjectName Moryx.Products.Model -ConnectionString "Username=postgres;Password=postgres;Host=localhost;Port=5432;Persist Security Info=True;Database=products" -ConnectionProviderName Npgsql -Verbose
