# Core-Rebus-WebJob

Sample service using Rebus and hostable in an Azure WebJob.

Before deploying to Azure app service add a run.cmd/run.sh(depending on app service windows/linux) with the command below to start the app.

echo dotnet CoreRebusWebJobHandlerService.dll -> run.cmd
