# Azure functions demo

This is a sample app I put together for the [Birmingham .NET meetup](https://www.meetup.com/birmingham-net-meetup/events/297588603/). Supporting slides [here](https://1drv.ms/p/c/c53cd493bf53c0eb/Eb2fSrHDjJZIn7oftxmPZYUBpvQwe0x6DE0_Cyjh5JES2w).

## Local Setup

- .NET 8 sdk
- [Azure Functions core tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-csharp#install-the-azure-functions-core-tools)
- [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio%2Cblob-storage#install-azurite)
- Sql Server (for Sql trigger example)
- Visual Studio 2022 latest (not required, but recommended for debugging)
- Add a file called `local.settings.json` in the `./src/CustomerOnboarding/` directory that has required local settings. Should look like this:

   ```json
    {
        "IsEncrypted": false,
        "Values": {
          "AzureWebJobsStorage": "UseDevelopmentStorage=true",
          "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
        },
        "ConnectionStrings": {
          "CustomerOnboardingDbConnection": "<sql-connection-string>",
          "StorageAccountConnection": "<azure-storage-account-connection-string>"
        }
    }