using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Orders.Infrastructure.Data;
using Orders.Infrastructure.Entities;
using Orders.Test.CustomFactories;
using Orders.Test.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Orders.Core.Models.Dto;
using TechTalk.SpecFlow;

namespace Orders.Test.Tests.ApiTests.Steps;

[Binding]
public class PayRestaurantFeeStep
{
    private readonly PayRestaurantFeeWebApplicationFactory<Program> _factory;
    private HttpClient? _client;
    private HttpResponseMessage? _response;
    private PayDto _dto;

    public PayRestaurantFeeStep(PayRestaurantFeeWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Given(@"an authenticated restaurant owner is logged into the system")]
    public void GivenAnAuthenticatedRestaurantOwnerIsLoggedIntoTheSystem()
    {
        _client = _factory.CreateClient();

        //Mock JWT token
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + JwtTokenHelper.GetRestaurantOwnerJwtToken());
    }

    [When(@"an unpaid restaurant fee exists in the system")]
    public async Task WhenAnUnpaidRestaurantFeeExists()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();

        // Populate the database with a fee
        var testRestaurantFee = new RestaurantFee
        {
            Id = 1,
            OrderId = 1,
            Amount = 10,
            PaymentStatus = 0 //Unpaid
        };

        await context.RestaurantFees.AddAsync(testRestaurantFee);
        await context.SaveChangesAsync();
    }

    [When(@"the restaurant owner provides the necessary payment information")]
    public void WhenTheRestaurantOwnerProvidesTheNecessaryPaymentInformation()
    {
        _dto = new PayDto { FeeId = 1 }; //The id of the fee we created in the previous step
    }


    [When(@"the restaurant owner pays the restaurant fee")]
    public async Task WhenTheRestaurantOwnerPaysTheRestaurantFee()
    {
        var dtoJson = JsonConvert.SerializeObject(_dto);
        var content = new StringContent(dtoJson, Encoding.UTF8, "application/json");

        _response = await _client!.PostAsync($"api/Payment/restaurant-fee", content);
    }

    [Then(@"the restaurant fee is paid")]
    public async Task ThenTheRestaurantFeeIsPaid()
    {
        _response!.StatusCode.Should().Be(HttpStatusCode.OK);

        //Check the in-memory database to see if the fee was paid
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OrderContext>();
        var fee = await context.RestaurantFees.FirstOrDefaultAsync();
        fee.PaymentStatus.Should().Be(PaymentStatus.Paid);
    }
}