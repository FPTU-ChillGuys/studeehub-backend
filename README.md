Setup .env file

open terminal on solution then run:
dotnet ef migrations add InitialCreate --project ./studeehub.Persistence --startup-project ./studeehub.API --context StudeeHubDBContext
dotnet ef database update --project ./studeehub.Persistence --startup-project ./studeehub.API --context StudeeHubDBContext
