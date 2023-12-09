using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Orders.Core.Models.Dto;
using Orders.Infrastructure.Data;
using Orders.Test.CustomFactories;
using Orders.Test.Helpers;
using TechTalk.SpecFlow;

namespace Orders.Test.Steps;

[Binding]
public class CreateOrderStep
{
    private readonly CreateOrderWebApplicationFactory<Program> _factory;
    private HttpClient? _client;
    private HttpResponseMessage? _response;
    private CreateOrderDto _dto;

    public CreateOrderStep(CreateOrderWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Given(@"an authenticated customer is logged into the system")]
    public void GivenAnAuthenticatedCustomerIsLoggedIntoTheSystem()
    {
        _client = _factory.CreateClient();
        
        //Mock JWT token
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + JwtTokenHelper.GetCustomerJwtToken());
    }

    [When(@"the user has chosen a menu to order from")]
    public void WhenTheUserHasChosenAMenuToOrderFrom()
    {
        //Dto containing the order information, including restaurant and menu
        _dto = OrderTestHelper.GetTestCreateOrderDto();
    }

    [When(@"the user creates a new order")]
    public async Task WhenTheUserCreatesANewOrder()
    {
        var dtoJson = JsonConvert.SerializeObject(_dto);
        var content = new StringContent(dtoJson, Encoding.UTF8, "application/json");
        
        _response = await _client.PostAsync("api/order/create", content);
        var fisk = _response;
    }

    [Then(@"the order is created in the system")]
    public async Task ThenTheOrderIsCreatedInTheSystem()
    {
        _response!.StatusCode.Should().Be(HttpStatusCode.OK);
        
        //Check the in-memory database to see if the order was created
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();
        var order = await context.Orders.FirstOrDefaultAsync();
        order.Should().NotBeNull();
    }
}