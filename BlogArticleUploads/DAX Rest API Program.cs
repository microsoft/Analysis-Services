using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace ConsoleApp1
{
    public class QueryRequest
    {
        public class Query
        {
            public string query { get; set; }
        }

        public List<Query> queries { get; set; }

        public QueryRequest()
        {
        }

        public QueryRequest(string daxQuery)
        {
            queries = new List<Query> { new Query { query = daxQuery } };
        }
    }

    public class ParsedResponse
    {
        public class Result
        {
            public class Table
            {
                public List<Dictionary<string, object>> rows { get; set; }
            }
            public List<Table> tables { get; set; }
        }
        public List<Result> results { get; set; }
    }
  
    class Program
    {
        /// <summary>
        /// Please fill in the application parameters.
        /// </summary>
        private static string clientId = "<Provide your app's client Id>";
        private static string tenantID = "<Provide your Azure tenant Id>";
        private static string replyUrl = "<Provide your app's reply Url>";
        private static string resourceID = "https://analysis.windows.net/powerbi/api";

        /// <summary>
        /// Please provide the DAX Rest API parameters.
        /// </summary>
        private static Guid datasetId = new Guid("<Provide a dataset Id>");
        private static string daxQuery = @"<Provide a DAX query>";

        static void Main(string[] args)
        {
            string authToken = GetToken().Result;


            string jsonResponse;
            if (QueryDataset(datasetId, daxQuery, authToken, out jsonResponse))
            {
                var queryResponse = new JavaScriptSerializer().Deserialize<ParsedResponse>(jsonResponse);
                foreach (var row in queryResponse.results[0].tables[0].rows)
                {
                    foreach (var keyValuePair in row)
                    {
                        Console.WriteLine($"{keyValuePair.Key}:{keyValuePair.Value}");
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("The Web request did not succeed.");
            }

            Console.WriteLine("Press [Enter] to exit the console app...");
            Console.ReadLine();
        }

        private static async Task<string> GetToken()
        {
            IPublicClientApplication PublicClientApp = PublicClientApplicationBuilder.Create(clientId)
                                                            .WithRedirectUri(replyUrl)
                                                            .WithAuthority(AzureCloudInstance.AzurePublic, tenantID)
                                                            .Build();
            AuthenticationResult authResult = await PublicClientApp.AcquireTokenInteractive(scopes: new[] { resourceID + "/Dataset.Read.All" })
                                                                    .ExecuteAsync();

            return authResult.AccessToken;
        }

        static bool QueryDataset(Guid datasetId, string daxQuery, string authToken, out string jsonResponse)
        {

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", authToken);

            string url = $"https://api.powerbi.com/v1.0/myorg/datasets/{datasetId}/executeQueries";

            var requestBody = new JavaScriptSerializer().Serialize(
                new QueryRequest(daxQuery)
                );


            var response = client.PostAsync(url, new StringContent(requestBody, UnicodeEncoding.UTF8, "application/json")).Result;
            jsonResponse = response.Content.ReadAsStringAsync().Result;

            return response.IsSuccessStatusCode;
        }
    }
}
