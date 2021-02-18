using APIRateLimitChecker;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace APIRateLimitCheckerTest
{
    public class Tests
    {
        StringContent content;

        [SetUp]
        public void Setup()
        {
            content = new StringContent(@"{
  'resources': {
    'core': {
      'limit': 5000,
      'remaining': 4999,
      'reset': 1372700873
    },
                   'search': {
                'limit': 30,
      'remaining': 18,
      'reset': 1372697452
                   },
    'graphql': {
                'limit': 5000,
      'remaining': 4993,
      'reset': 1372700389
    },
    'integration_manifest': {
                'limit': 5000,
      'remaining': 4999,
      'reset': 1551806725
    },
    'code_scanning_upload': {
                'limit': 500,
      'remaining': 499,
      'reset': 1551806725
    }
        },
  'rate': {
    'limit': 5000,
    'remaining': 4999,
    'reset': 1372700873
  }
}");
        }

        [Test]
        public async Task TestAPICall()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = content,
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://api.github.com"),
            };

            var result = await Program.ExecuteAPICallAsync(httpClient);

            int limit = result.limit;
            int remaining = result.remaining;

            Assert.IsNotNull(result);
            Assert.AreEqual(5000, limit);
            Assert.AreEqual(4999, remaining);
        }

        [Test]
        public void TestRateLimt()
        {
            //Arrange
            dynamic rate = new ExpandoObject();
            rate.limit = 5000;
            rate.remaining = 4800;
            var percentage = 10;

            //Assert
            var result = Program.IsRateLimitExceed(rate, percentage);
            Assert.IsFalse(result);

            rate.remaining = 20;
            var result2 = Program.IsRateLimitExceed(rate, percentage);
            Assert.IsTrue(result2);
        }
    }
}