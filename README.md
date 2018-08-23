# Core-Rebus-WebJob

Sample service using Rebus and hostable in an Azure WebJob.

To run locally, add a connection string to the appsettings.json and rebuild solution.

Before deploying to Azure app service add a run.cmd/run.sh(depending on app service windows/linux) with the command below to start the app.

these instructions can be used for deploying to Azure.
https://blogs.msdn.microsoft.com/benjaminperkins/2017/03/07/how-to-deploy-a-net-core-console-application-to-azure-webjob/

echo dotnet CoreRebusWebJobHandlerService.dll -> run.cmd
