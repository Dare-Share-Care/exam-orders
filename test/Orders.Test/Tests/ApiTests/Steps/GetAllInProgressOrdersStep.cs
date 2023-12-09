using System.Net;
using Newtonsoft.Json;
using Orders.Test.CustomFactories;
using Orders.Infrastructure.Data;
using Orders.Infrastructure.Entities;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Orders.Test.Helpers;
using TechTalk.SpecFlow;

namespace Orders.Test.Tests.ApiTests.Steps;

[Binding]
public class GetAllInProgressOrdersStep
{
    private readonly GetAllInProgressOrdersWebApplicationFactory<Program> _factory;
    private HttpClient? _client;
    private HttpResponseMessage? _response;

    public GetAllInProgressOrdersStep(GetAllInProgressOrdersWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Given(@"a courier user is logged into the system")]
    public void GivenACourierUserIsLoggedIntoTheSystem()
    {
        _client = _factory.CreateClient();

        //Mock JWT token
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + JwtTokenHelper.GetCourierJwtToken());
    }

    [Given(@"orders exists in the system with the order status ‘in progress’")]
    public async Task GivenOrdersExistsInTheSystemWithTheOrderStatusInProgress()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        // Populate the database with some orders
        var orders = OrderTestHelper.GetTestOrders();
        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();
    }

    [When(@"the user requests a list of all orders with the given status")]
    public async Task WhenTheUserRequestsAListOfAllOrdersWithTheGivenStatus()
    {
        _response = await _client.GetAsync("api/Order/ready-for-delivery");
    }

    [Then(@"the system returns a list of all the orders with the given status")]
    public async Task ThenTheSystemReturnsAListOfAllTheOrdersWithTheGivenStatus()
    {
        _response!.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await _response.Content.ReadAsStringAsync();
        var ordersFromResponse = JsonConvert.DeserializeObject<List<Order>>(responseContent);

        // TODO: Adjust the assertion based on the expected in-progress orders
        ordersFromResponse!.Count.Should().BeGreaterThan(0); // Example assertion, change as needed
    }
}