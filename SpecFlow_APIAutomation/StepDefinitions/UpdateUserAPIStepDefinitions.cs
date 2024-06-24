using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System;
using System.Net;
using TechTalk.SpecFlow;

namespace SpecFlow_APIAutomation.StepDefinitions
{
    [Binding]
    public class UpdateUserAPIStepDefinitions
    {
        private RestClient client;
        private RestResponse response;
        string baseUrl = "https://reqres.in/";
        // Use ScenarioContext to share data between steps
        private readonly ScenarioContext _scenarioContext;


        UpdateUserAPIStepDefinitions(ScenarioContext scenarioContext)
        {
            client = new RestClient(baseUrl);
            _scenarioContext = scenarioContext;
        }

        [Given(@"the API endpoint for updating user information")]
        public void GivenTheAPIEndpointForUpdatingUserInformation()
        {
            
        }

        [When(@"I send a PUT request for user ID ""([^""]*)"" with updated job ""([^""]*)""")]
        public void WhenISendAPUTRequestForUserIDWithUpdatedJob(string name, string job)
        {
            var request = new RestRequest("/api/users/2", Method.Put);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody("{\"name\":\"morpheus\",\"job\":\"zion resident\"}");
            var response = client.Execute(request);
            // Store the value in ScenarioContext
            _scenarioContext["myResponse"] = response;
        }

        [Then(@"the user information should be successfully updated with the job ""([^""]*)""")]
        public void ThenTheUserInformationShouldBeSuccessfullyUpdatedWithTheJob(string updatedJob)
        {
            var myResponse = _scenarioContext["myResponse"] as RestResponse;
            // Assuming the API response is JSON, you can deserialize it
            JsonResponseJob jsonResponseJobObj = JsonConvert.DeserializeObject<JsonResponseJob>(myResponse.Content);
            if (jsonResponseJobObj.job != null)
            {
                var job = jsonResponseJobObj.job.ToString();
                Console.WriteLine(job);
                Assert.AreEqual(updatedJob,job);
            }

        }

        public class JsonResponseJob
        {
            public string job { get; set; }
        }

        [Then(@"the response code should be (.*)")]
        public void ThenTheResponseCodeShouldBe(int statusCode)
        {
            var myResponse = _scenarioContext["myResponse"] as RestResponse;
            Assert.AreEqual(HttpStatusCode.OK, myResponse.StatusCode);
            Console.WriteLine(myResponse.StatusCode);
        }



    }
}
