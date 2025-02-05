using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace MS_SustainabilityManager_SDK_Sample
{
    class Program
    {
        static async Task Main()
        {
            // Build configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string clientId = configuration["Dataverse:ClientId"];
            string url = configuration["Dataverse:Url"];

            string connectionString = $"AuthType=OAuth;Url={url};ClientId={clientId};RedirectUri=http://localhost;LoginPrompt=Auto;";

            try
            {
                using var serviceClient = new ServiceClient(connectionString);
                if (!serviceClient.IsReady)
                {
                    Console.WriteLine("Failed to connect to Dataverse.");
                    return;
                }

                Console.WriteLine("Connected to Microsoft Sustainability Manager. Fetching emissions data...");

                // Retrieve emissions data
                List<EmissionRecord> emissionsData = await FetchEmissionsDataAsync(serviceClient);

                if (emissionsData.Count == 0)
                {
                    Console.WriteLine("No emissions data found.");
                    return;
                }

                // Generate CSV file
                string csvFilePath = $"EmissionsReport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                WriteToCsv(emissionsData, csvFilePath);

                Console.WriteLine($"✅ Emissions data successfully exported: {csvFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }

        static async Task<List<EmissionRecord>> FetchEmissionsDataAsync(ServiceClient serviceClient)
        {
            var emissionsData = new List<EmissionRecord>();

            try
            {
                QueryExpression query = new QueryExpression("msdyn_emission")
                {
                    ColumnSet = new ColumnSet(
                        "msdyn_name",
                        "msdyn_origincorrelationid",
                        "msdyn_activityname",
                        "msdyn_scope",
                        "msdyn_co2e",
                        "msdyn_co2eunit"
                    )
                };

                EntityCollection results = await Task.Run(() => serviceClient.RetrieveMultiple(query));

                foreach (Entity record in results.Entities)
                {
                    emissionsData.Add(new EmissionRecord
                    {
                        Name = record.GetAttributeValue<string>("msdyn_name") ?? "N/A",
                        OriginCorrelationId = record.GetAttributeValue<string>("msdyn_origincorrelationid") ?? "N/A",
                        ActivityName = record.GetAttributeValue<string>("msdyn_activityname") ?? "N/A",
                        Scope = record.GetAttributeValue<OptionSetValue>("msdyn_scope")?.Value.ToString() ?? "N/A",
                        CO2E = record.GetAttributeValue<decimal>("msdyn_co2e"),
                        CO2EUnit = record.GetAttributeValue<EntityReference>("msdyn_co2eunit")?.Name ?? "N/A"
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error fetching emissions data: {ex.Message}");
            }

            return emissionsData;
        }

        // Write data to CSV file
        static void WriteToCsv(List<EmissionRecord> emissions, string filePath)
        {
            try
            {
                using var writer = new StreamWriter(filePath);
                using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                });

                csv.WriteRecords(emissions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error writing CSV file: {ex.Message}");
            }
        }
    }

    // Class to structure emissions data
    public class EmissionRecord
    {
        public string Name { get; set; }
        public string OriginCorrelationId { get; set; }
        public string ActivityName { get; set; }
        public string Scope { get; set; }
        public decimal CO2E { get; set; }
        public string CO2EUnit { get; set; }
    }
}
