using IdeaCenter.Models;
using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Net;
using System.Text.Json;


namespace IdeaCenter
{
    [TestFixture]
    public class IdeaCenterTests
    {
        private RestClient client;
        private static string lastCreatedIdeaId;
        private const string baseUrl = "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:84";

        [OneTimeSetUp]

        public void Setup()
        {
            string token = GetJwtTolken("tsve@example.com", "123tsve12"); //to add my creds

            var option = new RestClientOptions(baseUrl)
            {
                Authenticator = new JwtAuthenticator(token)
            };

            client = new RestClient(option);
        }




        private string GetJwtTolken(string email, string password)
        {
            var loginClient = new RestClient(baseUrl);
            var request = new RestRequest("/api/User/Authentication", Method.Post);

            request.AddJsonBody(new {email, password });
            var response = loginClient.Execute(request);

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);

            return json.GetProperty("accessToken").GetString() ?? string.Empty;

        }


        [Test, Order(1)]
        public void CreatedNewIdea_ShouldReturnStatusCodeOk()
        {
            var idea = new
            {
                Title = "New Idea",
                Url = "",
                Description = "Fantastic new idea"
            };
            var request = new RestRequest("/api/Idea/Create", Method.Post);
            request.AddJsonBody(idea);
            var response = client.Execute(request);
            
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);


            
            Assert.That(response.Content, Does.Contain("Successfully created!"));

        }


        [Test, Order(2)]

        public void GetAllIdias_ShouldReturnList()
        {
            var request = new RestRequest("/api/Idea/All", Method.Get);

            var response = client.Execute(request);

            var responseItem = JsonSerializer.Deserialize<List<ApiResponseDTO>>(response.Content);


            Assert.That(response.StatusCode, Is.EqualTo(((HttpStatusCode)HttpStatusCode.OK)));

            Assert.That(responseItem, Is.Not.Empty);

            Assert.That(responseItem, Is.Not.Null);

            lastCreatedIdeaId = responseItem.LastOrDefault()?.Id;
        }


       // [Test, Order(3)]
        //public void EditIdeaTile_ShouldReturnOK()
        //{
        //    var editRequest = new IdeaDTO

        //    {
         //      Title = "changed Idea", 
        //       Description = "Briliant New Idea " ,
          //     Url = " "
         //   };
    
            

           // var request = new RestRequest($"/api/Idea/Edit", Method.Put);
        
        //    request.AddQueryParameter("ideaId", lastCreatedIdeaId);

          //  request.AddJsonBody(editRequest);

          //  var response = client.Execute(request);

          //  var editResponse = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            
          //  Assert.That(response.StatusCode, Is.EqualTo((HttpStatusCode)HttpStatusCode.OK));

           // Assert.That(response.Content, Does.Contain("Successfully edited"));

       // }

       // [Test, Order(4)]
        //public void DeleteIdea_ShouldReturnOK()

       // {
       //     var request = new RestRequest($"/api/Idea/Delete", Method.Delete);
       //     request.AddQueryParameter("ideaId", lastCreatedIdeaId);
       //     var response = client.Execute(request);
        //    var deleteResponse = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);
        //    Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        //    Assert.That(response.Content, Does.Contain("Deleted successfully!"));
        //}


        [Test, Order(5)]

        public void CreateIdea_WithoutRequiredFilds_ShouldReturnBadRequest()
        {
            var idea = new
            {
                name = "",
                descriptionn = ""
            };
            var request = new RestRequest("/api/Idea/Create", Method.Post);

            request.AddJsonBody(idea);
            var response = client.Execute(request);

            Assert.That(response.StatusCode, Is.EqualTo((HttpStatusCode)HttpStatusCode.BadRequest));

        }

       // [Test, Order(6)]

       // public void EditingNonExistingIdea_ShouldReturnNotFound()
       // {
          //  string fakeId = "123";
          //  var changes = new[]
         //   {
          //      new { path = "/name", op = "replace", value = "New Title"}
          //  };

           // var request = new RestRequest($"/api/Idea/Edit/", Method.Put);
           /// request.AddJsonBody(changes);

           // var response = client.Execute(request);

           // Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

            //Assert.That(response.Content, Does.Contain("No food revues..."));
       // }


        //[Test, Order(7)]
       // public void DeleteNonExistingIdea_ShouldReturnBadRequest()
        //{


         //   var request = new RestRequest($"/api/Idea/Delete", Method.Delete);
         //   request.AddQueryParameter("ideaId", lastCreatedIdeaId);
         //   var response = client.Execute(request);
          //  var deleteResponse = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

         //   Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

          //  Assert.That(response.Content, Does.Contain("There is no such idea!"));


        //}

        [OneTimeTearDown]
        public void Cleanup()
        {
            client?.Dispose();
        }
    }
}




