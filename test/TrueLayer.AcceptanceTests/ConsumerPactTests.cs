using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using PactNet.Matchers;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Shouldly;
using TrueLayer.Payments.Model;
using Xunit;

namespace TrueLayer.AcceptanceTests;


public class ConsumerPactTests : IClassFixture<ConsumerPactClassFixture>
{

    private IMockProviderService _mockProviderService;
    private string _mockProviderServiceBaseUri;
    paymentRequestModel.paymentRequest paymentRequest=new paymentRequestModel.paymentRequest();
    paymentResponseModel.CreatePaymentResponseModel paymentResponse = new
        paymentResponseModel.CreatePaymentResponseModel();
    public ConsumerPactTests(ConsumerPactClassFixture fixture)
    {

        _mockProviderService = fixture.MockProviderService;
        _mockProviderService
            .ClearInteractions(); //NOTE: Clears any previously registered interactions before the test is run
        _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
    }

    [Fact]
    public void MakeValidPayment()
    {
        // Arrange
        paymentRequest = CreatePaymentRequest();
     //   PaymentUserResponse paymentUserResponse = new PaymentUserResponse("userId");
     //   CreatePaymentResponse paymentResponse =
       //     new CreatePaymentResponse("testId", "testToken", paymentUserResponse);

       paymentResponse = CreatePaymentResponse();
        _mockProviderService.Given("There is valid data")
            .UponReceiving("A valid POST request for posting a payment request")
            .With(new ProviderServiceRequest
            {
                Method = HttpVerb.Post,
                Path = "/payments",
                Headers = new Dictionary<string, object>
                {
                    {"Content-Type", "application/json; charset=utf-8"},
                },
                Body = paymentRequest
            })
            .WillRespondWith(new ProviderServiceResponse
            {
                Status = 201,
                Headers = new Dictionary<string, object> {{"Content-Type", "application/json; charset=utf-8"}},
                Body = Match.Type(paymentResponse)
            });

        var response = ConsumerApiClient.postCreatePaymentAPI(_mockProviderServiceBaseUri,paymentRequest)
            .GetAwaiter().GetResult();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.ShouldNotBeNull();
    }

    paymentRequestModel.paymentRequest CreatePaymentRequest()
    {
        FileInfo fi = new FileInfo("../../../paymentRequest.json");
            String paymentRequestFile = fi.FullName;
            paymentRequest= JsonConvert.DeserializeObject<paymentRequestModel.paymentRequest>(System.IO.File.ReadAllText(paymentRequestFile));
            return paymentRequest;
    }
    paymentResponseModel.CreatePaymentResponseModel CreatePaymentResponse()
    {
        FileInfo fi = new FileInfo("../../../paymentResponse.json");
        String paymentResponseFile = fi.FullName;
        paymentResponse= JsonConvert.DeserializeObject<paymentResponseModel.CreatePaymentResponseModel>(System.IO.File.ReadAllText(paymentResponseFile));
        return paymentResponse;
    }
    }






