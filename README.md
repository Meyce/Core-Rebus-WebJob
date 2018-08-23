# Core-Rebus-WebJob

Sample service using Rebus and hostable in an Azure WebJob.

To run locally, add a connection string to the appsettings.config and rebuild solution.

Before deploying to Azure app service add a run.cmd/run.sh(depending on app service windows/linux) with the command below to start the app.

echo dotnet CoreRebusWebJobHandlerService.dll -> run.cmd
