using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Orders.Test.CustomFactories;
using Orders.Web.Data;
using Orders.Web.Entities;
using FluentAssertions;
using Newtonsoft.Json;
using Orders.Test.Helpers;
using TechTalk.SpecFlow;

namespace Orders.Test.Steps
{
    [Binding]
    public class GetAllOrdersStep
    {
        private readonly GetAllOrdersWebApplicationFactory<Program> _factory;
        private HttpClient? _client;
        private HttpResponseMessage? _response;

        public GetAllOrdersStep(GetAllOrdersWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Given(@"an authenticated user is logged into the system")]
        public void GivenAnAuthenticatedUserIsLoggedIntoTheSystem()
        {
            _client = _factory.CreateClient();
            //TODO: Add authentication logic here
        }

        [Given(@"orders exists in the system")]
        public async Task GivenOrdersExistsInTheSystem()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<OrderContext>();
            
            // Populate the database with some orders
            var orders = OrderTestHelper.GetTestOrders();
            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();
        }

        [When(@"the user requests a list of all orders")]
        public async Task WhenTheUserRequestsAListOfAllOrders()
        {
            _response = await _client.GetAsync("api/Order/all");
        }

        [Then(@"the system returns a list of all the orders")]
        public async Task ThenTheSystemReturnsAListOfAllOrders()
        {
            _response!.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await _response.Content.ReadAsStringAsync();
            var ordersFromResponse = JsonConvert.DeserializeObject<List<Order>>(responseContent);

            ordersFromResponse!.Count.Should().Be(3);
        }
    }
}
