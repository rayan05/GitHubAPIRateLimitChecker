using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace APIRateLimitChecker
{
    public class Program
    {
        static string token = "ccb855827aec1b7c5cf3460d1ca43ec5c29310af";
        static string baseUrl = "https://api.github.com";
        static decimal percentage = 10;

        static async Task Main(string[] args)
        {
            Console.WriteLine("This Apllication checks Github API's available rate. If it available rate is less than 10%, it will print 1. else 0 ");
            await APIRateLimitCheckAsync();
            Console.ReadLine();
        }

        public static async Task APIRateLimitCheckAsync()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);

            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("AppName", "1.0"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);

            var rate = await ExecuteAPICallAsync(client);

            var exceeded = IsRateLimitExceed(rate, percentage);
            Console.WriteLine(Convert.ToByte(exceeded));
        }

        public static bool IsRateLimitExceed(dynamic rate, decimal percentage)
        {
            decimal limit = rate.limit;
            decimal remaining = rate.remaining;

            return ((remaining / limit) * 100) < percentage;
        }

        public static async Task<dynamic> ExecuteAPICallAsync(HttpClient client)
        {

            var response = await client.GetAsync("/rate_limit");
            var contents = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<dynamic>(contents);
            var rate = result.rate;

            return rate;
        }
    }
}
