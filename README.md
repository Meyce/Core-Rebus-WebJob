# Core-Rebus-WebJob

Sample service using Rebus and hostable in an Azure WebJob.

When deploying to Azure remember to add a run.cmd/run.sh(depending on app service windows/linux) with the command to start the app.

echo dotnet CoreRebusWebJobHandlerService.dll -> run.cmd
