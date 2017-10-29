using System;
using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using TabiApiClient;
using Xunit;

namespace TabiApiClient.Test
{
    public class TabiClientTest
    {
        [Fact]
        public async Task Test1Async()
        {
            ApiClient client = new ApiClient();
            client.MockHttpClient = SetupMockHttpClient();
            client.Mock = true;

            TokenResult token = await client.Authenticate(new UserMessage() { Username = "vpl", Password = "blue" });
            Assert.NotEqual("", token.Token);
            Assert.NotEqual(0, token.UserId);

        }

        private HttpClient SetupMockHttpClient()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost:8000/api/v1/token")
                    .Respond("application/json", "{'user_id': 1, 'token': 'Test McGee'}");

            return mockHttp.ToHttpClient();
        }

    }
}