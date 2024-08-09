/*

       Linkguard.cc | Linkguard™ - 2024

       DO NOT USE THIS CODE IN YOUR PROJECTS *AS-IS*!! IT IS ONLY MEANT AS A DEMONSTRATION OF HOW TO INTEGRATE LINKGUARD TO YOUR PROJECTS!!

       Offical Documentation: https://docs.linkguard.cc

*/

using System.Text.Json;

class Program
{
    static async Task Main()
    {
        // Your project name goes here (I.E: this refers to https://linkguard.cc/dotnet)
        string projectName = "dotnet";
        Console.Title = "Linkguard.cc | Dotnet Demonstration";
        Console.Write("License Key: ");
        string? licenseKey = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(licenseKey))
        {
            Console.WriteLine("Invalid License Key Format");
            Environment.Exit(1);
        }

        using (HttpClient client = new HttpClient())
        {
            string url = $"https://linkguard.cc/v1/project/{projectName}/licenses/{licenseKey}";
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                using (JsonDocument jsonDocument = JsonDocument.Parse(responseBody))
                {
                    JsonElement root = jsonDocument.RootElement;

                    if (root.TryGetProperty("valid", out JsonElement validElement) && validElement.GetBoolean())
                    {
                        if (root.TryGetProperty("expire", out JsonElement expireElement) && expireElement.ValueKind == JsonValueKind.Number)
                        {
                            long expireTimestamp = expireElement.GetInt64();

                            // Converting Unix timestamp to date string
                            DateTime expireDate = DateTimeOffset.FromUnixTimeMilliseconds(expireTimestamp).UtcDateTime;

                            // From here on you can implement your own logic
                            Console.WriteLine($"License is valid. It expires on: {expireDate}");
                            Console.ReadKey();
                            Environment.Exit(0);
                        }
                        else
                        {
                            Console.WriteLine("Unexpected missing expiry field");
                            Environment.Exit(1);
                        }
                    }
                    else
                    {
                        Console.WriteLine("The license is invalid or expired.");
                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                Environment.Exit(1);
            }
        }
    }
}
