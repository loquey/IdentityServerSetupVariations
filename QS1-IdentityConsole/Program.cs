using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace QS1_IdentityConsole
{
    class Program
    {

        static private string _ApiBaseUrl = "http://localhost:6001";
        static private string _IdentityBaseUrl = "http://localhost:5001";

        static void Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();
            IConfiguration secrets = new ConfigurationBuilder().AddUserSecrets("a08b1a32-6d3b-45d6-90dd-d92adc3a36a8").Build();
            //var appConfig = config.GetSection(nameof(RooOptionst.Size)).Get<string>();
            var appConfig = config.GetSection(nameof(RooOptionst.Size)).Get<string>();
            var dbSettings = secrets.GetSection("Size").Get<string>();
            //var dbSettings = config.GetSection(nameof(DB)).Get<DB>();

            Console.WriteLine(appConfig);
            Console.WriteLine($"Size {dbSettings}");
            //Console.WriteLine($"Username : {dbSettings.Username}, Password: {dbSettings.Password}");

            Console.WriteLine("Hello World!");

            //Logic();
            Console.ReadLine();
        }


        static private async void Logic()
        {
            var token = await GetToken();
            if (token != null) CallApi(token);
        }

        static private async Task<TokenResponse> GetToken()
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(_IdentityBaseUrl);
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return null;
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "Client-1",
                ClientSecret = "golem",
                Scope = "APIIdentity",
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return null;
            }

            Console.WriteLine(tokenResponse.Json);
            return tokenResponse;
        }

        private static async void CallApi(TokenResponse token)
        {
            var apiClient = new HttpClient();
            //apiClient.SetBearerToken(token.AccessToken);
            var response = await apiClient.GetAsync($"{_ApiBaseUrl}/Identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

        }
    }

    class ApplicationOptions
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    class RooOptionst
    {
        public string Size { get; set; }
        public ApplicationOptions App { get; set; }
    }

    public class DB
    {
        public string Password { get; set; }
        public string Username { get; set; }
    }
}
