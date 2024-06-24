using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using SpecFlow.Internal.Json;
using System.Net;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.CommonModels;

namespace SpecFlow_APIAutomation.StepDefinitions
{
    [Binding]
    public class UserAPIStepDefinitions
    {
        private RestClient client;
        private RestResponse response;
        string baseUrl = "https://reqres.in/";
        // Use ScenarioContext to share data between steps
        private readonly ScenarioContext _scenarioContext;


        UserAPIStepDefinitions(ScenarioContext scenarioContext)
        {
            client = new RestClient(baseUrl);
            _scenarioContext = scenarioContext;
        }
        


        [Given(@"a valid user id")]
        public void GivenAValidUserId()
        {
            var userId = 2;
        }

        //Scenario: Retrieve User Information - [GET Single User]


        [Given(@"I request information for the user with id ""([^""]*)""")]
        public void GivenIRequestInformationForTheUserWithId(string id)
        {
            var request = new RestRequest("/api/users/", Method.Get);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("id", id);
            var response = client.Execute(request);
            // Store the value in ScenarioContext
            _scenarioContext["myResponse"] = response;

        }


        [When(@"the response should contain the user details:")]
        public void WhenTheResponseShouldContainTheUserDetails(Table table)
        {
            var myResponse = _scenarioContext["myResponse"] as RestResponse;
            // Assuming the API response is JSON, you can deserialize it
            var responseObject = JsonConvert.DeserializeObject<UserDetailsResponse>(myResponse.Content);
            //Console.WriteLine(myResponse.Content);

            // Access the first row of the table
            var expectedUserDetails = table.Rows[0];

            // Perform assertions on user details
            Assert.AreEqual(expectedUserDetails["FirstName"], responseObject.Data.first_name);
            Assert.AreEqual(expectedUserDetails["LastName"], responseObject.Data.last_name);
        }
        public class UserDetailsResponse
        {
            public UserData Data { get; set; }
        }

        public class UserData
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
        }

        [Then(@"the response status code should be (.*)")]
        public void ThenTheResponseStatusCodeShouldBe(int statusCode)
        {
            var myResponse = _scenarioContext["myResponse"] as RestResponse;
            Assert.AreEqual(HttpStatusCode.OK, myResponse.StatusCode);
            Console.WriteLine(myResponse.StatusCode);


        }

        //Scenario: Add a New User [POST Create]

        [Given(@"a new user with the following details:")]
        public void GivenANewUserWithTheFollowingDetails(Table table)
        {
            var expectedUserDetails = table.Rows[0];
            string name = expectedUserDetails["Name"];
            string job = expectedUserDetails["Job"];

            _scenarioContext["name"] = name;
            _scenarioContext["job"] = job;

        }

        [When(@"I add the new user via API")]
        public void WhenIAddTheNewUserViaAPI()
        {
            var request = new RestRequest("/api/users/", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            // Specify the user information to be added in the request body
            var newUser = new
            {
                Name = _scenarioContext["name"],
                Job = _scenarioContext["job"]
            };

            // Serialize the user object to JSON and add it to the request body
            request.AddJsonBody(newUser);

            var response = client.Execute(request);
            Console.WriteLine(response.Content);
            // Store the value in ScenarioContext
            _scenarioContext["Response2"] = response;
        }

        [Then(@"the response status code should return (.*)")]
        public void ThenTheResponseStatusCodeShouldbe(int statusCode)
        {
            var response2 = _scenarioContext["Response2"] as RestResponse;
            Assert.AreEqual(HttpStatusCode.Created, response2.StatusCode);
            Console.WriteLine(response2.StatusCode);
            


        }

        [Then(@"the response should contain the newly added user details")]
        public void ThenTheResponseShouldContainTheNewlyAddedUserDetails()
        {
            var myResponse = _scenarioContext["Response2"] as RestResponse;
            Console.WriteLine(myResponse.Content);
            var name = _scenarioContext["name"];
            var job = _scenarioContext["job"];

            JsonResponseNewUser jsonResponseNewUSerObj = JsonConvert.DeserializeObject<JsonResponseNewUser>(myResponse.Content);
            if ((jsonResponseNewUSerObj.name != null) && (jsonResponseNewUSerObj.job) != null )
            {
                var Name = jsonResponseNewUSerObj.name.ToString();
                var Job = jsonResponseNewUSerObj.job.ToString();
                Assert.AreEqual(name, Name);
                Assert.AreEqual(job, Job);
            }
        }
       
        public class JsonResponseNewUser
        {
            public string name { get; set; }
            public string job { get; set; }
          
        }

        //Scenario: Successful API Login - [POST Successful]
        [Given(@"Given a user with valid credentials:")]
        public void GivenGivenAUserWithValidCredentials(Table table)
        {
            var expectedUserDetails = table.Rows[0];
            string Email = expectedUserDetails["Email"];
            string Password = expectedUserDetails["Password"];

            _scenarioContext["email"] = Email;
            _scenarioContext["password"] = Password;
        }

        [When(@"the user sends a POST request to the login endpoint with the credentials")]
        public void WhenTheUserSendsAPOSTRequestToTheLoginEndpointWithTheCredentials()
        {
            var email = _scenarioContext["email"];
            var password = _scenarioContext["password"];
            var request = new RestRequest("/api/login/", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody("{\"email\":\"eve.holt@reqres.in\",\"password\":\"cityslicka\"}");
            var response = client.Execute(request);
            // Store the value in ScenarioContext
            _scenarioContext["myResponse"] = response;


        }

        [Then(@"the API should respond with a (.*) OK status code")]
        public void ThenTheAPIShouldRespondWithAOKStatusCode(int p0)
        {
            var myResponse = _scenarioContext["myResponse"] as RestResponse;
            Assert.AreEqual(HttpStatusCode.OK, myResponse.StatusCode);
            Console.WriteLine(myResponse.StatusCode);
        }

        [Then(@"the response should contain a valid access token")]
        public void ThenTheResponseShouldContainAValidAccessToken()
        {
            var myResponse = _scenarioContext["myResponse"] as RestResponse;
            String expectedToken = "QpwL5tke4Pnpja7X4";
            // Assuming the API response is JSON, you can deserialize it
            JsonResponseToken jsonResponseTokenObj = JsonConvert.DeserializeObject<JsonResponseToken>(myResponse.Content);
            if(jsonResponseTokenObj.token != null )
            {
                var token1 = jsonResponseTokenObj.token.ToString();
                Console.WriteLine(token1);
                Assert.AreEqual(expectedToken, token1);
            }

        }

        public class JsonResponseToken
        {
            public string token { get; set; }
        }

        //Scenario: UnSuccessful API Login - [POST UnSuccessful]

        [Given(@"Given a user with missing credentials:")]
        public void GivenGivenAUserWithMissingCredentials(Table table)
        {
            var expectedUserDetails = table.Rows[0];
            string Email = expectedUserDetails["Email"];

            _scenarioContext["email"] = Email;
        }

        [When(@"the user sends a POST request to the login endpoint with the missing credentials")]
        public void WhenTheUserSendsAPOSTRequestToTheLoginEndpointWithTheMissingCredentials()
        {
            var email = _scenarioContext["email"];
            var request = new RestRequest("/api/login/", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody("{\"email\":\"eve.holt@reqres.in\"}");
            var response = client.Execute(request);
            // Store the value in ScenarioContext
            _scenarioContext["myResponse"] = response;

        }

        [Then(@"the API should respond with a (.*) BadRequest status code")]
        public void ThenTheAPIShouldRespondWithABadRequestStatusCode(int p0)
        {
            var myResponse = _scenarioContext["myResponse"] as RestResponse;
            Assert.AreEqual(HttpStatusCode.BadRequest, myResponse.StatusCode);
            Console.WriteLine(myResponse.StatusCode);
        }

        [Then(@"the response should contain a error as ""([^""]*)""")]
        public void ThenTheResponseShouldContainAErrorAs(string errorMessage)
        {
            var myResponse = _scenarioContext["myResponse"] as RestResponse;
            Console.WriteLine("Response Message:"+myResponse.Content);
            // Assuming the API response is JSON, you can deserialize it
            JsonResponseError jsonResponseErrorObj = JsonConvert.DeserializeObject<JsonResponseError>(myResponse.Content);
            if (jsonResponseErrorObj.error != null)
            {
                var errorMsg = jsonResponseErrorObj.error.ToString();
                Assert.AreEqual(errorMessage, errorMsg);
            }
        }

        public class JsonResponseError
        {
            public string error { get; set; }
        }

    }
}