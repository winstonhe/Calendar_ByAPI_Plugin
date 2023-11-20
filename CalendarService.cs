using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Identity.Client;


namespace CalendarPlugin
{
    public class CalendarService
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string tenantId;
        private readonly string authority;
        private readonly string resource;
        private string accessToken;

        public CalendarService(string clientId, string clientSecret, string tenantId, string resource)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.tenantId = tenantId;
            this.authority = $"https://login.microsoftonline.com/{tenantId}";
            this.resource = resource;
        }

        public async Task<string> GetAccessToken()
        {
            if (this.accessToken != null) return this.accessToken;

            var cca = ConfidentialClientApplicationBuilder.Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri(authority))
                .Build();

            var result = await cca.AcquireTokenForClient(new string[] { $"{resource}/.default" })
                .ExecuteAsync();

            this.accessToken = result.AccessToken;
            return this.accessToken;
        }

        public async Task<string> GetWorkHours()
        {
            string accessToken = await GetAccessToken();

            string apiUrl = $"{resource}/api/data/v9.1/systemusers";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new HttpRequestException($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
        }

        public async Task<string> CreateWorkingHours()
        {
            try
            {
                string apiUrl = "https://winston.crm5.dynamics.com/api/data/v9.0/msdyn_SaveCalendar";

                // Create the payload as a C# object
                var calendarEventInfo = new
                {
                    UseV2 = true,
                    CalendarId = "ea48c61f-dc9c-47c5-8e4f-87a184280a3d",
                    ObjectTypeCode = 1150,
                    ResourceId = "d0eb244b-6f5d-ec11-8f8f-0022481629a8",
                    TimeZoneCode = 210,
                    StartDate = "2023-11-13T00:00:00.000Z",
                    IsVaried = false,
                    RulesAndRecurrences = new[]
                    {
                new
                {
                    Rules = new[]
                    {
                        new
                        {
                            StartTime = "2023-11-13T11:00:00.000Z",
                            EndTime = "2023-11-13T16:30:00.000Z",
                            Duration = 330,
                            Effort = 1,
                            TimeCode = 0,
                            SubCode = 1
                        }
                        ,
                        new
                        {
                            StartTime = "2023-11-13T16:30:00.000Z",
                            EndTime = "2023-11-13T16:45:00.000Z",
                            Duration = 15,
                            Effort = 1,
                            TimeCode = 2,
                            SubCode = 4
                        }
                          ,
                        new
                        {
                            StartTime = "2023-11-13T16:45:00.000Z",
                            EndTime = "2023-11-13T17:45:00.000Z",
                            Duration = 60,
                            Effort = 1,
                            TimeCode = 0,
                            SubCode = 1
                        }
                    }
                }
            }
                };

                // Serialize the object to JSON
                string payload = JsonConvert.SerializeObject(new { CalendarEventInfo = JsonConvert.SerializeObject(calendarEventInfo) });

                using (HttpClient client = new HttpClient())
                {
                    // Set the content type of the request
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string accessToken = await GetAccessToken();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    // Create the StringContent with the payload
                    StringContent content = new StringContent(payload, Encoding.UTF8, "application/json");

                    // Send the POST request
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        return ("POST request was successful.");

                    }
                    else
                    {
                        return ("POST request failed. Status code: " + response.StatusCode);
                    }
                }
            }
            catch (Exception ex)
            {
                return ("POST request failed. exception details: " + ex.Message);
            }
        }
    }
}
