using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using Xunit;

namespace BattleshipTest
{
    public class CalibrationControllerIntegrationTest : IClassFixture<WebApplicationFactory<Battleship.Api.Startup>>
    {

        /// <summary>
        /// 
        /// </summary>
        private readonly HttpClient httpClient;

        public CalibrationControllerIntegrationTest(
            WebApplicationFactory<Battleship.Api.Startup> factory
            )
        {
            httpClient = factory.CreateClient();
        }

        [Fact]
        public void Test1()
        {
            var response = httpClient.GetAsync("api/calibration").GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();
        }

    }

}