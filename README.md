# Microsoft Sustainability Manager SDK Sample

## Overview
This sample demonstrates how to access Microsoft Sustainability Manager data using the Dataverse SDK and C# code. Specifically, it retrieves data from the `msdyn_emission` entity (Emission entity) and exports the output to a CSV file. This example is intended to help customers and partners who want to interact with Microsoft Sustainability Manager using Dataverse.

## Authentication Method
The sample uses **OAuth** authentication with a `ClientId` to connect to Microsoft Dataverse. Authentication details are read from the `appsettings.json` file. The connection string is built dynamically in the code as follows:

```
AuthType=OAuth;
Url={Dataverse URL};
ClientId={ClientId};
RedirectUri=http://localhost;
LoginPrompt=Auto;
```

## SDK Used
This project utilizes the **Microsoft.PowerPlatform.Dataverse.Client** SDK to interact with Dataverse. The SDK enables querying and retrieving data from the `msdyn_emission` entity, which stores emissions-related data in Microsoft Sustainability Manager.

### Dependencies:
- `Microsoft.PowerPlatform.Dataverse.Client`
- `Microsoft.Xrm.Sdk`
- `Microsoft.Extensions.Configuration`
- `CsvHelper`

## Functionality
### Steps Performed:
1. **Read Configuration:** The program reads authentication details from `appsettings.json`.
2. **Authenticate with Dataverse:** Establishes a connection using `ServiceClient`.
3. **Retrieve Emission Data:** Queries the `msdyn_emission` entity using `QueryExpression`.
4. **Export Data to CSV:** The retrieved data is formatted and written to a CSV file.

### Key Methods:
- `FetchEmissionsDataAsync(ServiceClient serviceClient)`: Queries Dataverse for emissions data.
- `WriteToCsv(List<EmissionRecord> emissions, string filePath)`: Writes retrieved data to a CSV file.

## Usage Instructions
1. **Setup Dependencies:** Ensure you have the necessary NuGet packages installed.
2. **Configure Authentication:** Create an `appsettings.json` file with the required credentials:
    ```json
    {
      "Dataverse": {
        "ClientId": "your-client-id",
        "Url": "https://your-org.api.crm.dynamics.com"
      }
    }
    ```
3. **Run the Application:** Execute the program, which connects to Dataverse, retrieves emissions data, and generates a CSV report.

## Output
Upon successful execution, an emissions report in CSV format is generated with the following columns:
- `msdyn_name`
- `msdyn_origincorrelationid`
- `msdyn_activityname`
- `msdyn_scope`
- `msdyn_co2e`
- `msdyn_co2eunit`

The CSV file will be named in the format: `EmissionsReport_YYYYMMDD_HHMMSS.csv`.

## Error Handling
- If authentication fails, the program logs an error and exits.
- If no emissions data is found, a message is displayed.
- Any CSV writing errors are logged to the console.

## Conclusion
This sample provides a practical guide to accessing Microsoft Sustainability Manager emissions data using Dataverse SDK in C#. It can be extended to fetch additional entities or integrate with other sustainability workflows.

