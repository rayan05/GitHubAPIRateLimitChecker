using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace APIRateLimitChecker
{
    public class Program
    {
        static string token = String.Empty;
        static readonly string baseUrl = "https://api.github.com";
        static readonly decimal percentage = 10;

        static async Task Main()
        {
            Console.WriteLine(string.Format("This Apllication checks Github API's rate limit. If the available rate is less than {0}%, it will print 1. else 0 ", percentage));
            Console.Write("Please Enter the Github Personal Access Token (PAT): ");
            token = Console.ReadLine();

            await APIRateLimitCheckAsync();

            Console.ReadLine();
        }

        public static async Task APIRateLimitCheckAsync()
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AppName", "1.0"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);

            var rate = await ExecuteAPICallAsync(client);

            if (rate != null)
            {
                var exceeded = IsRateLimitExceed(rate, percentage);
                Console.WriteLine(Convert.ToByte(exceeded));
            }
            else
            {
                //Error occured. Cannot calculate the available rate.
                Console.WriteLine("-1");
            }
        }

        public static bool IsRateLimitExceed(dynamic rate, decimal percentage)
        {
            decimal limit = rate.limit;
            decimal remaining = rate.remaining;

            return ((remaining / limit) * 100) < percentage;
        }

        public static async Task<dynamic> ExecuteAPICallAsync(HttpClient client)
        {
            dynamic rate = null; 

            try
            {
                var response = await client.GetAsync("/rate_limit");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var contents = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<dynamic>(contents);
                    rate = result.rate;
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine(response.StatusCode.ToString() + ". Please check your PAT is correct...");
                }
                else
                {
                    Console.WriteLine(response.StatusCode.ToString());
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return rate;
        }
    }
}
