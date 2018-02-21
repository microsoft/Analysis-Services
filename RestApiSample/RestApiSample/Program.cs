using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace RestApiSample
{
    class Program
    {
        static void Main(string[] args)
        {
            CallRefreshAsync();
            Console.ReadLine();
        }

        private static async void CallRefreshAsync()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://<rollout>.asazure.windows.net/servers/<serverName>/models/<resource>/");

            // Send refresh request
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await UpdateToken());

            RefreshRequest refreshRequest = new RefreshRequest()
            {
                type = "full",
                maxParallelism = 10
            };

            HttpResponseMessage response = await client.PostAsJsonAsync("refreshes", refreshRequest);
            response.EnsureSuccessStatusCode();
            Uri location = response.Headers.Location;
            Console.WriteLine(response.Headers.Location);

            // Check the response
            while (true) // Will exit while loop when exit Main() method (it's running asynchronously)
            {
                string output = "";

                // Refresh token if required
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await UpdateToken());

                response = await client.GetAsync(location);
                if (response.IsSuccessStatusCode)
                {
                    output = await response.Content.ReadAsStringAsync();
                }

                Console.Clear();
                Console.WriteLine(output);

                Thread.Sleep(5000);
            }
        }

        private static async Task<string> UpdateToken()
        {
            string resourceURI = "https://*.asazure.windows.net";
            string clientID = "<App ID>"; // Native app with necessary API permissions

            string authority = "https://login.windows.net/common/oauth2/authorize";
            //string authority = "https://login.windows.net/<TenantID>/oauth2/authorize"; // Authority address can optionally use tenant ID in place of "common". If service principal or B2B enabled, this is a requirement.
            AuthenticationContext ac = new AuthenticationContext(authority);

            //Interactive login if not cached:
            AuthenticationResult ar = await ac.AcquireTokenAsync(resourceURI, clientID, new Uri("urn:ietf:wg:oauth:2.0:oob"), new PlatformParameters(PromptBehavior.Auto));

            //Username/password:
            //UserPasswordCredential cred = new UserPasswordCredential("<User ID (UPN e-mail format)>", "<Password>");
            //AuthenticationResult ar = await ac.AcquireTokenAsync(resourceURI, clientID, cred);

            //Service principal:
            //12/19/2017: Bug disallows use of service principals. At time of writing, the fix is being rolled out to production clusters. Please retry soon if not working by the time you try it.
            //ClientCredential cred = new ClientCredential("<App ID>", "<App Key>");
            //AuthenticationResult ar = await ac.AcquireTokenAsync(resourceURI, cred);

            return ar.AccessToken;
        }
    }

    class RefreshRequest
    {
        public string type { get; set; }
        public int maxParallelism { get; set; }
    }
}
