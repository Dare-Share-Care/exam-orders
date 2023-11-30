using System.Net;
using Orders.Test.CustomFactories;
using TechTalk.SpecFlow;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Orders.Test.Helpers;
using Orders.Web.Data;
using Orders.Web.Entities;

namespace Orders.Test.Steps;

[Binding]
public class GetAllOrdersStep
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    private HttpClient? _client;
    private HttpResponseMessage? _response;

    public GetAllOrdersStep(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }


    [Given(@"an authenticated user is logged into the system")]
    public void GivenAnAuthenticatedUserIsLoggedIntoTheSystem()
    {
        // Create a client to send requests to the test server representing the user
        _client = _factory.CreateClient();
        
        //TODO: Add authentication logic here
    }
    
    
    [Given(@"orders exists in the system")]
    public void GivenOrdersExistsInTheSystem()
    {
        //Populate the database with some test orders
        var orders = OrderTestHelper.GetTestOrders();
        
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();
        context.Orders.AddRange(orders);
        context.SaveChanges();
    }

    [When(@"the user requests a list of all orders")]
    public async Task WhenTheUserRequestsAListOfAllOrders()
    {
        _response = await _client.GetAsync("api/Order/all");
    }

    [Then(@"the system returns a list of all the orders")]
    public async Task ThenTheSystemReturnsAListOfAllOrders()
    {
        //Assert the response status code
        _response!.StatusCode.Should().Be(HttpStatusCode.OK); //Expects 200 OK
        
        //Assert the response content
        var responseContent = await _response.Content.ReadAsStringAsync();
        var ordersFromResponse = JsonConvert.DeserializeObject<List<Order>>(responseContent);
        var expectedOrdersCount = OrderTestHelper.GetTestOrders().Count;
        
        //Expects 3 orders
        ordersFromResponse!.Count.Should().Be(3);
    }
}